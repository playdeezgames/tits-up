#+build !js
package game

import "core:fmt"
import "core:os"
import "core:testing"

test_store: map[string]string

test_get :: proc(key: string) -> (string, bool) {
	v, ok := test_store[key]
	return v, ok
}

test_set :: proc(key, value: string) {
	if key in test_store {
		delete(test_store[key])
		test_store[key] = clone_string(value)
	} else {
		test_store[clone_string(key)] = clone_string(value)
	}
}

test_store_clear :: proc() {
	for k, v in test_store {
		delete(k)
		delete(v)
	}
	delete(test_store)
	test_store = nil
}

new_test_game :: proc(seed: u64) -> ^Game {
	g := new(Game)
	game_init(g, seed, Storage{get = test_get, set = test_set})
	return g
}

free_test_game :: proc(g: ^Game) {
	game_destroy(g)
	free(g)
}

press :: proc(g: ^Game, command: Command) {
	game_command(g, command, true)
	game_command(g, command, false)
	game_frame(g, 0.016)
}

lit_pixels :: proc(g: ^Game) -> int {
	n := 0
	for p in g.canvas.pixels {
		if p != .Black {
			n += 1
		}
	}
	return n
}

start_game :: proc(g: ^Game) {
	press(g, .A) // splash -> main menu
	press(g, .A) // Embark!
}

@(test)
splash_then_main_menu_then_embark :: proc(t: ^testing.T) {
	g := new_test_game(1)
	defer free_test_game(g)
	game_frame(g, 0.016)
	testing.expect_value(t, current_state(g), State.Splash)
	testing.expect(t, lit_pixels(g) > 100, "the splash draws something")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Main_Menu)
	testing.expect_value(t, len(g.menu), 4)
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Navigation)
	testing.expect(t, g.has_world)
	testing.expect(t, lit_pixels(g) > 500, "the map draws")
}

@(test)
options_returns_to_the_menu_that_opened_it :: proc(t: ^testing.T) {
	g := new_test_game(2)
	defer free_test_game(g)
	press(g, .A) // main menu
	press(g, .Down)
	press(g, .Down) // Options...
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Options)
	testing.expect_value(t, g.depth, 2)
	press(g, .B)
	testing.expect_value(t, current_state(g), State.Main_Menu)
	testing.expect_value(t, g.depth, 1)
	press(g, .B) // nothing below the main menu to return to
	testing.expect_value(t, current_state(g), State.Main_Menu)
}

@(test)
volume_menu_sets_and_persists_the_volume :: proc(t: ^testing.T) {
	defer test_store_clear()
	g := new_test_game(3)
	defer free_test_game(g)
	press(g, .A)
	press(g, .Down)
	press(g, .Down)
	press(g, .A) // options
	press(g, .Down)
	press(g, .A) // volume...
	testing.expect_value(t, current_state(g), State.Volume)
	testing.expect_value(t, g.menu_index, 5) // default 50%
	for _ in 0 ..< 3 {
		press(g, .Down)
	}
	press(g, .A)
	testing.expect_value(t, g.volume, 0.8)
	testing.expect_value(t, g.header, "Volume (Currently: 80%)")
	testing.expect_value(t, test_store["tu:volume"], "8")
	g2 := new_test_game(3)
	defer free_test_game(g2)
	testing.expect_value(t, g2.volume, 0.8)
}

@(test)
walking_into_a_goblin_opens_the_fight_menu :: proc(t: ^testing.T) {
	g := new_test_game(4)
	defer free_test_game(g)
	start_game(g)
	w := &g.world
	// Put a goblin right next to Tizzy.
	for &cell in w.cells {
		cell.character = NONE
	}
	for &c, i in w.characters {
		if i != w.avatar {
			c.recycled = true
		}
	}
	ac := cell_index(w, 3, 4)
	ch(w, w.avatar).cell = ac
	w.cells[ac].character = w.avatar
	goblin := character_create(w, .Goblin, cell_index(w, 4, 4))
	press(g, .Right)
	testing.expect_value(t, current_state(g), State.Interact_Character)
	testing.expect_value(t, ch(w, w.avatar).target, goblin)
	testing.expect_value(t, len(g.menu), 2)
	testing.expect_value(t, g.menu[0].label, "Fight!")
	testing.expect_value(t, g.header, "Goblin(1/1)")
	press(g, .Down)
	press(g, .A) // disengage
	testing.expect_value(t, current_state(g), State.Navigation)
	testing.expect_value(t, ch(w, w.avatar).target, NONE)
}

@(test)
saving_and_loading_through_the_menus :: proc(t: ^testing.T) {
	defer test_store_clear()
	g := new_test_game(5)
	defer free_test_game(g)
	start_game(g)
	testing.expect_value(t, current_state(g), State.Navigation)
	press(g, .B) // game menu
	testing.expect_value(t, current_state(g), State.Game_Menu)
	press(g, .Down)
	press(g, .A) // save...
	testing.expect_value(t, current_state(g), State.Save)
	testing.expect_value(t, len(g.menu), SLOT_COUNT)
	press(g, .Down)
	press(g, .A) // slot 2
	testing.expect_value(t, current_state(g), State.Game_Menu)
	testing.expect(t, "tu:slot2" in test_store)
	avatar_before := g.world.avatar
	cell_before := ch(&g.world, avatar_before).cell
	// Abandon, then load it back from the main menu.
	press(g, .Down)
	press(g, .Down)
	press(g, .Down)
	press(g, .A) // abandon game
	testing.expect_value(t, current_state(g), State.Abandon)
	press(g, .Down)
	press(g, .A) // yes
	testing.expect_value(t, current_state(g), State.Main_Menu)
	testing.expect(t, !g.has_world)
	press(g, .Down)
	press(g, .A) // load...
	testing.expect_value(t, current_state(g), State.Load)
	testing.expect_value(t, len(g.menu), 1)
	testing.expect_value(t, g.menu[0].label, "Slot 2")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Navigation)
	testing.expect_value(t, g.world.avatar, avatar_before)
	testing.expect_value(t, ch(&g.world, g.world.avatar).cell, cell_before)
}

@(test)
loading_with_no_saves_or_a_broken_save_fails_gracefully :: proc(t: ^testing.T) {
	defer test_store_clear()
	g := new_test_game(6)
	defer free_test_game(g)
	press(g, .A)
	press(g, .Down)
	press(g, .A) // load...
	testing.expect_value(t, g.menu[0].label, "No Saves Exist!")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Main_Menu)
	testing.expect(t, g.sfx_count > 0)
	test_set("tu:slot3", "{\"columns\":9,\"nonsense\":true}")
	press(g, .Down)
	press(g, .A)
	testing.expect_value(t, g.menu[0].label, "Slot 3")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Main_Menu)
	testing.expect(t, !g.has_world)
}

@(test)
inventory_and_equipment_menus_work :: proc(t: ^testing.T) {
	g := new_test_game(7)
	defer free_test_game(g)
	start_game(g)
	w := &g.world
	// Give Tizzy a club and a key, and drop a bikini top at her feet.
	club := item_create(w, .Club)
	character_add_item(w, w.avatar, club)
	character_add_item(w, w.avatar, item_create(w, .Key))
	cell_add_item(w, ch(w, w.avatar).cell, item_create(w, .Bikini_Top))
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Action_Menu)
	testing.expect_value(t, len(g.menu), 3) // Ground..., Inventory, Statistics
	// Ground -> Bikini Top -> (single item) -> Take
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Ground)
	testing.expect_value(t, g.menu[0].label, "Bikini Top(x1)")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Ground_Item)
	testing.expect_value(t, g.header, "Bikini Top")
	press(g, .A) // Take: back to the ground list, which is empty now, so on to Navigation
	testing.expect_value(t, current_state(g), State.Navigation)
	testing.expect_value(t, len(ch(w, w.avatar).items), 3)
	testing.expect(t, !has_ground_items(w))
	// Inventory -> Club -> Equip
	press(g, .A)
	testing.expect_value(t, len(g.menu), 2) // Inventory, Statistics
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Inventory)
	testing.expect_value(t, len(g.menu), 3)
	testing.expect_value(t, g.menu[0].label, "Club(x1)")
	testing.expect_value(t, g.menu[1].label, "Key(x1)")
	testing.expect_value(t, g.menu[2].label, "Bikini Top(x1)")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Inventory_Item)
	testing.expect_value(t, g.header, "Club(10/10)")
	testing.expect_value(t, g.menu[0].label, "Equip")
	testing.expect_value(t, g.menu[1].label, "Drop")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Navigation)
	item, ok := equipped_in(ch(w, w.avatar), .Weapon)
	testing.expect(t, ok)
	testing.expect_value(t, item, club)
	// Equipment -> Weapon -> Unequip
	press(g, .A)
	testing.expect_value(t, len(g.menu), 3) // Inventory, Equipment, Statistics
	press(g, .Down)
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Equipment)
	testing.expect_value(t, g.menu[0].label, "Weapon: Club(10/10)")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Equipment_Detail)
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Action_Menu) // nothing equipped: Equipment bounces back
	_, still := equipped_in(ch(w, w.avatar), .Weapon)
	testing.expect(t, !still)
	// The key: use it far from the door (complains), then drop it.
	press(g, .A) // Inventory (first entry)
	testing.expect_value(t, current_state(g), State.Inventory)
	testing.expect_value(t, g.menu[0].label, "Key(x1)") // the club went to the back of the pack
	press(g, .A) // Key
	testing.expect_value(t, current_state(g), State.Inventory_Item)
	testing.expect_value(t, g.menu[0].label, "Use")
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Message)
	testing.expect_value(t, g.sfx_count > 0, true)
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Navigation)
}

@(test)
dying_shows_the_epitaph_screen_then_the_menu :: proc(t: ^testing.T) {
	g := new_test_game(8)
	defer free_test_game(g)
	start_game(g)
	w := &g.world
	set_health(w, w.avatar, 0)
	set_epitaph(w, w.avatar, clone_string("Test epitaph"))
	press(g, .Left) // any action; Neutral sees the dead avatar
	testing.expect_value(t, current_state(g), State.Tits_Up)
	press(g, .A) // too early
	testing.expect_value(t, current_state(g), State.Tits_Up)
	game_frame(g, 1.5)
	press(g, .A)
	testing.expect_value(t, current_state(g), State.Main_Menu)
	testing.expect(t, !g.has_world)
}

@(test)
random_button_mashing_never_crashes :: proc(t: ^testing.T) {
	defer test_store_clear()
	for seed in u64(1) ..= 12 {
		g := new_test_game(seed)
		defer free_test_game(g)
		r: Rng
		rng_seed(&r, seed * 104729)
		for _ in 0 ..< 1500 {
			press(g, Command(rng_below(&r, len(Command))))
			if rng_below(&r, 50) == 0 {
				game_frame(g, 2)
			}
			testing.expect(t, g.depth >= 1 && g.depth <= 3, "the stack stays shallow")
			if g.depth < 1 {
				break
			}
		}
		free_all(context.temp_allocator)
	}
}

// With -define:DUMP_DIR=some/dir this writes PPM screenshots of the main screens
// (tools/make_shots.sh turns them into PNGs); otherwise the test does nothing.
DUMP_DIR :: #config(DUMP_DIR, "")

dump_ppm :: proc(g: ^Game, dir, name: string) {
	game_frame(g, 0.016)
	header := fmt.aprintf("P6\n%d %d\n255\n", VIEW_WIDTH, VIEW_HEIGHT)
	path := fmt.aprintf("%s/%s.ppm", dir, name)
	data: [dynamic]u8
	defer {
		delete(header)
		delete(path)
		delete(data)
	}
	append(&data, header)
	for p in g.pixels {
		append(&data, u8(p), u8(p >> 8), u8(p >> 16))
	}
	_ = os.write_entire_file(path, data[:])
}

@(test)
dump_screens :: proc(t: ^testing.T) {
	dir := DUMP_DIR
	if dir == "" {
		return
	}
	g := new_test_game(2026)
	defer free_test_game(g)
	dump_ppm(g, dir, "01-title")
	press(g, .A)
	dump_ppm(g, dir, "02-menu")
	press(g, .A)
	dump_ppm(g, dir, "03-map")
	w := &g.world
	character_add_item(w, w.avatar, item_create(w, .Meat))
	character_add_item(w, w.avatar, item_create(w, .Meat))
	character_add_item(w, w.avatar, item_create(w, .Rotten_Meat))
	character_add_item(w, w.avatar, item_create(w, .Key))
	character_add_item(w, w.avatar, item_create(w, .Club))
	bikini := item_create(w, .Bikini_Top)
	character_add_item(w, w.avatar, bikini)
	character_equip(w, w.avatar, .Chest, bikini)
	press(g, .A)
	press(g, .A) // inventory
	dump_ppm(g, dir, "04-inventory")
	press(g, .B)
	press(g, .Down)
	press(g, .Down)
	press(g, .A) // statistics
	dump_ppm(g, dir, "05-statistics")
	press(g, .B)
	order := [4]Command{.Up, .Down, .Left, .Right}
	for i := 0; i < 400 && ch(w, w.avatar).target == NONE; i += 1 {
		press(g, order[i % 4])
		if has_messages(w) || current_state(g) == State.Message {
			break
		}
	}
	dump_ppm(g, dir, "06-fight")
}
