#+build !js
package game

import "core:testing"

@(test)
rng_dice_known_values :: proc(t: ^testing.T) {
	r: Rng
	rng_seed(&r, 7)
	for _ in 0 ..< 500 {
		v := rng_dice(&r, "2d2+-2d1")
		testing.expect(t, v >= 0 && v <= 2, "2d2+-2d1 is 0..2")
		d := rng_dice(&r, "1d6/6")
		testing.expect(t, d == 0 || d == 1, "1d6/6 is 0 or 1")
	}
	testing.expect_value(t, rng_dice(&r, ""), 0)
	testing.expect_value(t, rng_dice(&r, "3d1"), 3)
	testing.expect_value(t, rng_dice(&r, "2d1*3"), 6)
}

@(test)
rng_chance_is_roughly_weighted :: proc(t: ^testing.T) {
	r: Rng
	rng_seed(&r, 99)
	hits := 0
	for _ in 0 ..< 10000 {
		if rng_chance(&r, 4, 1) {
			hits += 1
		}
	}
	testing.expect(t, hits > 1700 && hits < 2300, "about 20% true")
}

new_test_world :: proc(seed: u64) -> World {
	return world_new(seed)
}

@(test)
new_world_matches_the_original_setup :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 40 {
		w := new_test_world(seed)
		defer world_destroy(&w)
		goblins, tizzies := 0, 0
		keys, tops, thongs := 0, 0, 0
		for c in w.characters {
			if c.recycled {
				continue
			}
			switch c.type {
			case .Tizzy:
				tizzies += 1
			case .Goblin:
				goblins += 1
			}
			for item in c.items {
				#partial switch w.items[item].type {
				case .Key:
					keys += 1
				case .Bikini_Top:
					tops += 1
				case .Thong:
					thongs += 1
				}
			}
		}
		testing.expect_value(t, tizzies, 1)
		testing.expect_value(t, goblins, GOBLIN_COUNT)
		testing.expect_value(t, keys, 1)
		testing.expect_value(t, tops, 1)
		testing.expect_value(t, thongs, 1)
		testing.expect(t, w.avatar != NONE)
		testing.expect_value(t, w.cells[cell_index(&w, 8, 4)].terrain, Terrain.Locked_Door)
		testing.expect_value(t, w.cells[0].terrain, Terrain.Wall)
		// Nobody spawns on a wall, and every character stands on the cell that points back at it.
		for cell, i in w.cells {
			if cell.character != NONE {
				testing.expect(t, TERRAIN[cell.terrain].tenable)
				testing.expect_value(t, w.characters[cell.character].cell, i)
			}
		}
		testing.expect(t, world_valid(&w), "a fresh world is valid")
	}
}

@(test)
tizzy_starts_with_the_original_statistics :: proc(t: ^testing.T) {
	w := new_test_world(5)
	defer world_destroy(&w)
	testing.expect_value(t, health(&w, w.avatar), 3)
	testing.expect_value(t, dignity(&w, w.avatar), -10)
	testing.expect_value(t, maximum_encumbrance(&w, w.avatar), 10)
	testing.expect_value(t, ch(&w, w.avatar).epitaph, "Better luck next time!")
}

// Puts the avatar next to a goblin with known gear and returns the goblin.
stage_fight :: proc(w: ^World) -> int {
	for &cell in w.cells {
		cell.character = NONE
	}
	avatar_cell := cell_index(w, 2, 4)
	ch(w, w.avatar).cell = avatar_cell
	w.cells[avatar_cell].character = w.avatar
	for &c, i in w.characters {
		if i != w.avatar {
			c.recycled = true
		}
	}
	goblin := character_create(w, .Goblin, cell_index(w, 3, 4))
	return goblin
}

@(test)
moving_into_a_goblin_targets_it_without_moving :: proc(t: ^testing.T) {
	w := new_test_world(11)
	defer world_destroy(&w)
	goblin := stage_fight(&w)
	before := ch(&w, w.avatar).cell
	avatar_move(&w, 1, 0)
	testing.expect_value(t, ch(&w, w.avatar).cell, before)
	testing.expect_value(t, ch(&w, w.avatar).target, goblin)
	avatar_move(&w, -1, 0)
	testing.expect_value(t, ch(&w, w.avatar).target, NONE)
	testing.expect_value(t, cell_column(&w, ch(&w, w.avatar).cell), 1)
}

@(test)
walls_block_movement :: proc(t: ^testing.T) {
	w := new_test_world(12)
	defer world_destroy(&w)
	stage_fight(&w)
	for _ in 0 ..< 10 {
		avatar_move(&w, -1, 0)
	}
	testing.expect_value(t, cell_column(&w, ch(&w, w.avatar).cell), 1)
}

@(test)
leaving_an_enemys_reach_costs_an_opportunity_attack :: proc(t: ^testing.T) {
	w := new_test_world(13)
	defer world_destroy(&w)
	stage_fight(&w) // Tizzy at (2,4), goblin at (3,4)
	avatar_move(&w, 0, -1)
	found := false
	for m in w.messages {
		for line in m.lines {
			if len(line.text) >= 18 && line.text[:18] == "Opportunity Attack" {
				found = true
			}
		}
	}
	testing.expect(t, found, "an opportunity attack message was queued")
}

@(test)
fighting_ends_with_damage_dealt_and_never_hangs :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 60 {
		w := new_test_world(seed)
		defer world_destroy(&w)
		goblin := stage_fight(&w)
		ch(&w, w.avatar).target = goblin
		hp := health(&w, w.avatar)
		do_character_verb(&w, goblin, .Fight, w.avatar)
		changed := health(&w, w.avatar) != hp || ch(&w, goblin).recycled || len(w.messages) > 0
		testing.expect(t, changed)
		testing.expect(t, health(&w, w.avatar) >= 0)
	}
}

@(test)
killing_a_goblin_drops_its_loot_and_awards_dignity :: proc(t: ^testing.T) {
	w := new_test_world(21)
	defer world_destroy(&w)
	goblin := stage_fight(&w)
	cell := ch(&w, goblin).cell
	character_add_item(&w, goblin, item_create(&w, .Goblin_Corpse))
	character_equip(&w, goblin, .Weapon, item_create(&w, .Club))
	dignity_before := ch(&w, w.avatar).stats.value[.Dignity]
	// Give Tizzy a guaranteed kill.
	stat_set(&ch(&w, w.avatar).stats, .Attack_Dice, 60)
	stat_set(&ch(&w, w.avatar).stats, .Maximum_Attack, 5)
	ch(&w, goblin).stats.value[.Defend_Dice] = 0
	for rounds := 0; !ch(&w, goblin).recycled && rounds < 50; rounds += 1 {
		attack(&w, w.avatar, goblin)
	}
	testing.expect(t, ch(&w, goblin).recycled)
	testing.expect_value(t, w.cells[cell].character, NONE)
	testing.expect_value(t, len(w.cells[cell].items), 2) // corpse and club
	testing.expect_value(t, ch(&w, w.avatar).stats.value[.Dignity], dignity_before + 1)
	testing.expect_value(t, ch(&w, w.avatar).target, NONE)
}

@(test)
dying_sets_the_epitaph :: proc(t: ^testing.T) {
	w := new_test_world(31)
	defer world_destroy(&w)
	goblin := stage_fight(&w)
	stat_set(&ch(&w, goblin).stats, .Attack_Dice, 60)
	stat_set(&ch(&w, goblin).stats, .Maximum_Attack, 9)
	ch(&w, w.avatar).stats.value[.Defend_Dice] = 0
	for rounds := 0; !is_tits_up(&w, w.avatar) && rounds < 50; rounds += 1 {
		attack(&w, goblin, w.avatar)
	}
	testing.expect(t, is_tits_up(&w, w.avatar))
	testing.expect_value(t, ch(&w, w.avatar).epitaph, "Killed by Goblin!")
	testing.expect(t, !ch(&w, w.avatar).recycled, "the avatar is never recycled")
}

@(test)
equipment_wears_and_breaks :: proc(t: ^testing.T) {
	w := new_test_world(41)
	defer world_destroy(&w)
	club := item_create(&w, .Club)
	character_add_item(&w, w.avatar, club)
	character_equip(&w, w.avatar, .Weapon, club)
	testing.expect_value(t, stat_total(&w, w.avatar, .Attack_Dice), 3) // 1 + the club's 2
	testing.expect_value(t, item_full_name(&w, club), "Club(10/10)")
	message := message_new(&w)
	do_wear(&w, w.avatar, 4, message, true)
	testing.expect_value(t, item_full_name(&w, club), "Club(6/10)")
	do_wear(&w, w.avatar, 6, message, true)
	testing.expect(t, it(&w, club).recycled)
	testing.expect_value(t, len(ch(&w, w.avatar).equipment), 0)
	testing.expect_value(t, len(ch(&w, w.avatar).items), 0)
}

@(test)
armor_adds_dignity_and_encumbrance_room :: proc(t: ^testing.T) {
	w := new_test_world(51)
	defer world_destroy(&w)
	top := item_create(&w, .Bikini_Top)
	thong := item_create(&w, .Thong)
	for item in ([2]int{top, thong}) {
		character_add_item(&w, w.avatar, item)
	}
	character_equip(&w, w.avatar, .Chest, top)
	character_equip(&w, w.avatar, .Pelvis, thong)
	testing.expect_value(t, dignity(&w, w.avatar), 10)
	testing.expect_value(t, maximum_encumbrance(&w, w.avatar), 25)
}

@(test)
equipping_into_an_occupied_slot_swaps :: proc(t: ^testing.T) {
	w := new_test_world(61)
	defer world_destroy(&w)
	a, b := item_create(&w, .Club), item_create(&w, .Club)
	character_add_item(&w, w.avatar, a)
	character_add_item(&w, w.avatar, b)
	character_equip(&w, w.avatar, .Weapon, a)
	character_equip(&w, w.avatar, .Weapon, b)
	item, ok := equipped_in(ch(&w, w.avatar), .Weapon)
	testing.expect(t, ok)
	testing.expect_value(t, item, b)
	testing.expect_value(t, len(ch(&w, w.avatar).items), 1)
	testing.expect_value(t, ch(&w, w.avatar).items[0], a)
}

@(test)
eating_meat_heals_and_rotten_meat_hurts :: proc(t: ^testing.T) {
	w := new_test_world(71)
	defer world_destroy(&w)
	set_health(&w, w.avatar, 1)
	meat := item_create(&w, .Meat)
	character_add_item(&w, w.avatar, meat)
	do_item_verb(&w, meat, .Eat, w.avatar)
	testing.expect_value(t, health(&w, w.avatar), 2)
	testing.expect(t, it(&w, meat).recycled)
	set_health(&w, w.avatar, 1)
	rot := item_create(&w, .Rotten_Meat)
	character_add_item(&w, w.avatar, rot)
	do_item_verb(&w, rot, .Eat, w.avatar)
	testing.expect(t, is_tits_up(&w, w.avatar))
	testing.expect_value(t, ch(&w, w.avatar).epitaph, "Be careful which meat you put in yer mouth!")
	set_health(&w, w.avatar, 3)
	meat2 := item_create(&w, .Meat)
	character_add_item(&w, w.avatar, meat2)
	do_item_verb(&w, meat2, .Eat, w.avatar)
	testing.expect_value(t, health(&w, w.avatar), 3) // clamped to the maximum
}

@(test)
the_key_opens_the_door_and_stepping_through_wins :: proc(t: ^testing.T) {
	w := new_test_world(81)
	defer world_destroy(&w)
	stage_fight(&w)
	key := item_create(&w, .Key)
	character_add_item(&w, w.avatar, key)
	// Away from the door the key does nothing but complain.
	do_item_verb(&w, key, .Use, w.avatar)
	testing.expect_value(t, w.cells[cell_index(&w, 8, 4)].terrain, Terrain.Locked_Door)
	testing.expect(t, !it(&w, key).recycled)
	for has_messages(&w) {
		message_dismiss(&w)
	}
	// Walk next to the door (the goblin staged at (3,4) is out of the way on row 3).
	cell := cell_index(&w, 7, 4)
	w.cells[ch(&w, w.avatar).cell].character = NONE
	ch(&w, w.avatar).cell = cell
	w.cells[cell].character = w.avatar
	do_item_verb(&w, key, .Use, w.avatar)
	testing.expect_value(t, w.cells[cell_index(&w, 8, 4)].terrain, Terrain.Door)
	testing.expect(t, it(&w, key).recycled)
	testing.expect(t, !has_won(&w))
	character_move(&w, w.avatar, 1, 0)
	testing.expect(t, has_won(&w))
}

@(test)
too_much_weight_stops_the_avatar :: proc(t: ^testing.T) {
	w := new_test_world(91)
	defer world_destroy(&w)
	stage_fight(&w)
	for _ in 0 ..< 3 {
		character_add_item(&w, w.avatar, item_create(&w, .Wooden_Shield))
	}
	testing.expect(t, is_overencumbered(&w, w.avatar))
	before := ch(&w, w.avatar).cell
	avatar_move(&w, 0, -1)
	testing.expect_value(t, ch(&w, w.avatar).cell, before)
	testing.expect(t, has_messages(&w))
}

@(test)
corpses_are_not_takable :: proc(t: ^testing.T) {
	testing.expect(t, !ITEMS[.Goblin_Corpse].can_take)
	testing.expect(t, ITEMS[.Meat].can_take)
}

@(test)
saves_round_trip_and_garbage_is_refused :: proc(t: ^testing.T) {
	w := new_test_world(101)
	defer world_destroy(&w)
	set_health(&w, w.avatar, 2)
	message_line(&w, message_new(&w), .Red, "hello %d", 5)
	text, ok := world_save(&w)
	testing.expect(t, ok)
	defer delete(text)
	loaded, lok := world_load(text)
	testing.expect(t, lok)
	defer world_destroy(&loaded)
	testing.expect_value(t, loaded.avatar, w.avatar)
	testing.expect_value(t, health(&loaded, loaded.avatar), 2)
	testing.expect_value(t, len(loaded.characters), len(w.characters))
	testing.expect_value(t, len(loaded.items), len(w.items))
	testing.expect_value(t, ch(&loaded, loaded.avatar).epitaph, "Better luck next time!")
	testing.expect_value(t, loaded.messages[0].lines[0].text, "hello 5")
	testing.expect_value(t, loaded.rng.state, w.rng.state)
	for i in 0 ..< len(w.cells) {
		testing.expect_value(t, loaded.cells[i].terrain, w.cells[i].terrain)
		testing.expect_value(t, loaded.cells[i].character, w.cells[i].character)
		testing.expect_value(t, len(loaded.cells[i].items), len(w.cells[i].items))
	}
	for bad in ([]string{"", "{}", "null", "[1,2]", "{\"columns\":9,\"rows\":9}", "garbage", text[:len(text) / 2]}) {
		_, bok := world_load(bad)
		testing.expect(t, !bok, bad)
	}
}

@(test)
random_play_never_breaks_invariants :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 25 {
		w := new_test_world(seed)
		defer world_destroy(&w)
		r: Rng
		rng_seed(&r, seed * 7919)
		for step in 0 ..< 600 {
			if is_tits_up(&w, w.avatar) || has_won(&w) {
				break
			}
			for has_messages(&w) {
				message_dismiss(&w)
			}
			target := ch(&w, w.avatar).target
			if target != NONE && !ch(&w, target).recycled {
				verb := Verb.Fight if rng_below(&r, 4) != 0 else Verb.Disengage
				do_character_verb(&w, target, verb, w.avatar)
			} else {
				d := NEIGHBOR_DELTAS[rng_below(&r, 4)]
				avatar_move(&w, d.x, d.y)
			}
			// occasionally loot and use things
			c := ch(&w, w.avatar)
			if step % 7 == 0 {
				for item in w.cells[c.cell].items {
					if ITEMS[it(&w, item).type].can_take {
						cell_remove_item(&w, c.cell, item)
						character_add_item(&w, w.avatar, item)
						break
					}
				}
				for item in c.items {
					desc := &ITEMS[it(&w, item).type]
					if desc.can_equip && !is_overencumbered(&w, w.avatar) {
						character_equip(&w, w.avatar, desc.slot, item)
						break
					}
					if desc.has_verb {
						do_item_verb(&w, item, desc.verb, w.avatar)
						break
					}
				}
			}
			testing.expect(t, world_valid(&w), "world stays valid")
		}
		free_all(context.temp_allocator)
	}
}
