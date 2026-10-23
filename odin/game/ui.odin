package game

import "core:fmt"
import "core:strconv"

// The front end: the original's BaseGameController / BaseGameState / BasePickerState state machine and
// every TU.Presentation state, as one State enum and a few switch procedures. Time only comes in through
// game_frame(dt), and storage through the Storage hooks, so all of it runs natively under test.

Command :: enum u8 {
	Up,
	Right,
	Down,
	Left,
	A,
	B,
}

State :: enum u8 {
	None,
	Splash,
	Main_Menu,
	About,
	Options,
	Volume,
	Load,
	Save,
	Abandon,
	Game_Menu,
	Embark,
	Neutral,
	Navigation,
	Move_North,
	Move_South,
	Move_West,
	Move_East,
	Interact_Character,
	Message,
	Tits_Up,
	Won,
	Action_Menu,
	Statistics,
	Ground,
	Ground_Detail,
	Ground_Item,
	Take,
	Inventory,
	Inventory_Detail,
	Inventory_Item,
	Drop,
	Equip,
	Equipment,
	Equipment_Detail,
	Unequip,
}

Menu_Item :: struct {
	label: string, // heap allocated, owned by the menu
	key:   string, // static
	value: int,
}

// Where the page keeps saves and settings (localStorage). get's result is only valid until the next call.
Storage :: struct {
	get: proc(key: string) -> (value: string, ok: bool),
	set: proc(key: string, value: string),
}

SLOT_COUNT :: 5
TITS_UP_DELAY_SECONDS :: 1.0
REPEAT_FIRST_SECONDS :: 1.0
REPEAT_NEXT_SECONDS :: 0.3
MAX_SFX_PER_FRAME :: 32

EMBARK_TEXT :: "Embark!"
LOAD_TEXT :: "Load..."
OPTIONS_TEXT :: "Options..."
ABOUT_TEXT :: "About..."
CONTINUE_TEXT :: "Continue Game"
SAVE_TEXT :: "Save..."
ABANDON_TEXT :: "Abandon Game"
FULLSCREEN_TEXT :: "Toggle Full Screen"
VOLUME_TEXT :: "Volume..."
NO_TEXT :: "No"
YES_TEXT :: "Yes"
GROUND_TEXT :: "Ground..."
INVENTORY_TEXT :: "Inventory"
EQUIPMENT_TEXT :: "Equipment"
STATISTICS_TEXT :: "Statistics"
TAKE_TEXT :: "Take"
DROP_TEXT :: "Drop"
EQUIP_TEXT :: "Equip"
UNEQUIP_TEXT :: "Unequip"

Game :: struct {
	world:              World,
	has_world:          bool,
	stack:              [16]State,
	depth:              int,
	start_enabled:      bool,
	menu:               [dynamic]Menu_Item,
	menu_index:         int,
	header:             string, // heap allocated
	canvas:             Canvas,
	pixels:             [VIEW_WIDTH * VIEW_HEIGHT]u32, // RGBA bytes, refreshed by game_frame
	time:               f64,
	show_until:         f64,
	held:               [Command]bool,
	next_fire:          [Command]f64,
	volume:             f32,
	sfx:                [MAX_SFX_PER_FRAME]Sfx, // drained by the platform after each frame
	sfx_count:          int,
	fullscreen_request: bool,
	storage:            Storage,
	seed:               u64,
	embarks:            u64,
}

// ---- lifecycle --------------------------------------------------------------------------------

game_init :: proc(g: ^Game, seed: u64, storage: Storage) {
	g^ = {}
	g.storage = storage
	g.seed = seed
	g.volume = 0.5
	g.start_enabled = true
	if value, ok := storage_get(g, "tu:volume"); ok {
		if n, nok := strconv.parse_int(value); nok && n >= 0 && n <= 10 {
			g.volume = f32(n) / 10
		}
	}
	set_current_state(g, .Splash, true)
}

game_destroy :: proc(g: ^Game) {
	menu_clear(g)
	delete(g.menu)
	delete(g.header)
	if g.has_world {
		world_destroy(&g.world)
	}
	g^ = {}
}

storage_get :: proc(g: ^Game, key: string) -> (string, bool) {
	if g.storage.get == nil {
		return "", false
	}
	return g.storage.get(key)
}

// Raise or release a command. A held command fires at once, again after a second, then every 0.3 s.
game_command :: proc(g: ^Game, command: Command, down: bool) {
	if down && !g.held[command] {
		g.held[command] = true
		g.next_fire[command] = g.time + REPEAT_FIRST_SECONDS
		handle_command(g, command)
	} else if !down {
		g.held[command] = false
	}
}

game_frame :: proc(g: ^Game, dt: f64) {
	g.time += dt
	for command in Command {
		if g.held[command] && g.time >= g.next_fire[command] {
			g.next_fire[command] = g.time + REPEAT_NEXT_SECONDS
			handle_command(g, command)
		}
	}
	render(g)
	canvas_to_rgba(&g.canvas, &g.pixels)
	free_all(context.temp_allocator)
}

take_sfx :: proc(g: ^Game) -> []Sfx {
	out := g.sfx[:g.sfx_count]
	g.sfx_count = 0
	return out
}

play_sfx :: proc(g: ^Game, sfx: Sfx) {
	if sfx != .None && g.sfx_count < MAX_SFX_PER_FRAME {
		g.sfx[g.sfx_count] = sfx
		g.sfx_count += 1
	}
}

current_state :: proc(g: ^Game) -> State {
	if g.depth == 0 {
		return .None
	}
	return g.stack[g.depth - 1]
}

// ---- state stack (BaseGameController.SetCurrentState) -----------------------------------------

set_current_state :: proc(g: ^Game, state: State, push: bool) {
	if !push {
		pop_state(g)
	}
	if state != .None && g.depth < len(g.stack) {
		g.stack[g.depth] = state
		g.depth += 1
		if g.start_enabled {
			state_start(g, state)
		}
	}
}

pop_state :: proc(g: ^Game) {
	if g.depth > 0 {
		g.depth -= 1
	}
	if g.depth > 0 && g.start_enabled {
		state_start(g, g.stack[g.depth - 1])
	}
}

set_state :: proc(g: ^Game, state: State) {
	set_current_state(g, state, false)
}

// Replace the top with `next`, then push `pushed` on top of it (so cancelling returns to `next`).
set_states :: proc(g: ^Game, pushed, next: State) {
	g.start_enabled = false
	set_current_state(g, next, false)
	g.start_enabled = true
	set_current_state(g, pushed, true)
}

// ---- menu helpers -----------------------------------------------------------------------------

menu_clear :: proc(g: ^Game) {
	for item in g.menu {
		delete(item.label)
	}
	clear(&g.menu)
}

menu_add :: proc(g: ^Game, label: string, key := "", value := 0) {
	append(&g.menu, Menu_Item{label = clone_string(label), key = key, value = value})
}

set_header :: proc(g: ^Game, text: string) {
	delete(g.header)
	g.header = clone_string(text)
}

is_picker :: proc(state: State) -> bool {
	#partial switch state {
	case .Main_Menu, .Options, .Volume, .Load, .Save, .Abandon, .Game_Menu, .Interact_Character, .Action_Menu,
	     .Ground, .Ground_Detail, .Ground_Item, .Inventory, .Inventory_Detail, .Inventory_Item, .Equipment, .Equipment_Detail:
		return true
	}
	return false
}

picker_cancel :: proc(state: State) -> State {
	#partial switch state {
	case .Main_Menu:
		return .None
	case .Options:
		return .None
	case .Volume:
		return .Options
	case .Load:
		return .Main_Menu
	case .Save, .Abandon:
		return .Game_Menu
	case .Game_Menu, .Interact_Character, .Action_Menu, .Ground:
		return .Neutral
	case .Ground_Detail, .Ground_Item:
		return .Ground
	case .Inventory:
		return .Action_Menu
	case .Inventory_Detail, .Inventory_Item:
		return .Inventory
	case .Equipment:
		return .Action_Menu
	case .Equipment_Detail:
		return .Equipment
	}
	return .None
}

picker_header :: proc(state: State) -> string {
	#partial switch state {
	case .Main_Menu:
		return "Main Menu"
	case .Options:
		return "Options"
	case .Load:
		return "Load Game"
	case .Save:
		return "Save Game"
	case .Abandon:
		return "Are you sure you want to abandon the game?"
	case .Game_Menu:
		return "Game Menu"
	case .Action_Menu:
		return "Actions"
	case .Ground:
		return "On The Ground"
	case .Inventory:
		return "Inventory"
	case .Equipment:
		return "Equipment"
	}
	return "" // the dynamic headers are set when the state starts
}

controls_text :: proc(a, b: string) -> string {
	return fmt.tprintf("Space/(A) - %s | Esc/(B) - %s", a, b)
}

picker_status :: proc(state: State) -> string {
	#partial switch state {
	case .Main_Menu:
		return "Space/(A) - Select"
	case .Inventory_Detail:
		return controls_text("", "")
	}
	return controls_text("Select", "Cancel")
}

// ---- state start (OnStart) --------------------------------------------------------------------

state_start :: proc(g: ^Game, state: State) {
	if is_picker(state) {
		g.menu_index = 0
		menu_clear(g)
		set_header(g, picker_header(state))
	}
	w := &g.world
	#partial switch state {
	case .Main_Menu:
		menu_add(g, EMBARK_TEXT, EMBARK_TEXT)
		menu_add(g, LOAD_TEXT, LOAD_TEXT)
		menu_add(g, OPTIONS_TEXT, OPTIONS_TEXT)
		menu_add(g, ABOUT_TEXT, ABOUT_TEXT)
	case .Game_Menu:
		menu_add(g, CONTINUE_TEXT, CONTINUE_TEXT)
		menu_add(g, SAVE_TEXT, SAVE_TEXT)
		menu_add(g, OPTIONS_TEXT, OPTIONS_TEXT)
		menu_add(g, ABANDON_TEXT, ABANDON_TEXT)
	case .Options:
		menu_add(g, FULLSCREEN_TEXT, FULLSCREEN_TEXT)
		menu_add(g, VOLUME_TEXT, VOLUME_TEXT)
	case .Abandon:
		menu_add(g, NO_TEXT, NO_TEXT)
		menu_add(g, YES_TEXT, YES_TEXT)
	case .Volume:
		current := int(g.volume * 10 + 0.5)
		set_header(g, fmt.tprintf("Volume (Currently: %d%%)", current * 10))
		for i in 0 ..= 10 {
			menu_add(g, fmt.tprintf("%d%%", i * 10), "", i)
			if i == current {
				g.menu_index = i
			}
		}
	case .Load:
		for slot in 1 ..= SLOT_COUNT {
			if slot_exists(g, slot) {
				menu_add(g, fmt.tprintf("Slot %d", slot), "", slot)
			}
		}
		if len(g.menu) == 0 {
			menu_add(g, "No Saves Exist!", "", 0)
		}
	case .Save:
		for slot in 1 ..= SLOT_COUNT {
			menu_add(g, fmt.tprintf("Slot %d%s", slot, "(will overwrite)" if slot_exists(g, slot) else ""), "", slot)
		}
	case .Embark:
		if g.has_world {
			world_destroy(&g.world)
		}
		g.embarks += 1
		g.world = world_new(g.seed + g.embarks * 0x9E3779B97F4A7C15)
		g.has_world = true
		set_state(g, .Neutral)
	case .Neutral:
		switch {
		case has_messages(w):
			set_state(g, .Message)
		case is_tits_up(w, w.avatar):
			set_state(g, .Tits_Up)
		case has_won(w):
			set_state(g, .Won)
		case ch(w, w.avatar).target != NONE:
			set_state(g, .Interact_Character)
		case:
			set_state(g, .Navigation)
		}
	case .Move_North:
		avatar_move(w, 0, -1)
		set_state(g, .Neutral)
	case .Move_South:
		avatar_move(w, 0, 1)
		set_state(g, .Neutral)
	case .Move_West:
		avatar_move(w, -1, 0)
		set_state(g, .Neutral)
	case .Move_East:
		avatar_move(w, 1, 0)
		set_state(g, .Neutral)
	case .Message:
		play_sfx(g, w.messages[0].sfx)
	case .Tits_Up, .Won:
		g.show_until = g.time + TITS_UP_DELAY_SECONDS
	case .Interact_Character:
		target := ch(w, w.avatar).target
		set_header(g, fmt.tprintf("%s(%d/%d)", char_name(w, target), health(w, target), maximum_health(w, target)))
		for verb in character_verbs(w, target) {
			menu_add(g, VERB_NAMES[verb], "", int(verb))
		}
	case .Action_Menu:
		if has_ground_items(w) {
			menu_add(g, GROUND_TEXT, GROUND_TEXT)
		}
		if len(ch(w, w.avatar).items) > 0 {
			menu_add(g, INVENTORY_TEXT, INVENTORY_TEXT)
		}
		if len(ch(w, w.avatar).equipment) > 0 {
			menu_add(g, EQUIPMENT_TEXT, EQUIPMENT_TEXT)
		}
		menu_add(g, STATISTICS_TEXT, STATISTICS_TEXT)
	case .Ground:
		if !has_ground_items(w) {
			set_state(g, .Neutral)
			return
		}
		add_item_groups(g, w.cells[ch(w, w.avatar).cell].items[:], true)
	case .Ground_Detail:
		set_header(g, w.selected_item_name)
		ids := items_named(w, w.cells[ch(w, w.avatar).cell].items[:], w.selected_item_name, true)
		switch len(ids) {
		case 0:
			set_state(g, .Ground)
			return
		case 1:
			w.selected_item = ids[0]
			set_state(g, .Ground_Item)
			return
		}
		for id in ids {
			menu_add(g, item_name(w, id), "", id)
		}
	case .Ground_Item:
		set_header(g, w.selected_item_name)
		menu_add(g, TAKE_TEXT, TAKE_TEXT)
	case .Take:
		cell := ch(w, w.avatar).cell
		cell_remove_item(w, cell, w.selected_item)
		character_add_item(w, w.avatar, w.selected_item)
		clear_selection(w)
		set_state(g, .Ground)
	case .Inventory:
		if len(ch(w, w.avatar).items) == 0 {
			set_state(g, .Action_Menu)
			return
		}
		add_item_groups(g, ch(w, w.avatar).items[:], false)
	case .Inventory_Detail:
		set_header(g, w.selected_item_name)
		ids := items_named(w, ch(w, w.avatar).items[:], w.selected_item_name, false)
		switch len(ids) {
		case 0:
			set_state(g, .Inventory)
			return
		case 1:
			w.selected_item = ids[0]
			set_state(g, .Inventory_Item)
			return
		}
		for id in ids {
			menu_add(g, item_full_name(w, id), "", id)
		}
	case .Inventory_Item:
		set_header(g, item_full_name(w, w.selected_item))
		desc := &ITEMS[it(w, w.selected_item).type]
		if desc.can_equip {
			menu_add(g, EQUIP_TEXT, EQUIP_TEXT)
		}
		if desc.has_verb {
			menu_add(g, VERB_NAMES[desc.verb], "", int(desc.verb))
		}
		menu_add(g, DROP_TEXT, DROP_TEXT)
	case .Drop:
		cell := ch(w, w.avatar).cell
		cell_add_item(w, cell, w.selected_item)
		character_remove_item(w, w.avatar, w.selected_item)
		clear_selection(w)
		set_state(g, .Inventory)
	case .Equip:
		desc := &ITEMS[it(w, w.selected_item).type]
		character_equip(w, w.avatar, desc.slot, w.selected_item)
		set_state(g, .Neutral)
	case .Equipment:
		if len(ch(w, w.avatar).equipment) == 0 {
			set_state(g, .Action_Menu)
			return
		}
		for e in ch(w, w.avatar).equipment {
			menu_add(g, fmt.tprintf("%s: %s", SLOT_NAMES[e.slot], item_full_name(w, e.item)), "", int(e.slot))
		}
	case .Equipment_Detail:
		item, _ := equipped_in(ch(w, w.avatar), w.selected_slot)
		set_header(g, item_full_name(w, item))
		menu_add(g, UNEQUIP_TEXT, UNEQUIP_TEXT)
	case .Unequip:
		character_unequip(w, w.avatar, w.selected_slot)
		w.has_selected_slot = false
		set_state(g, .Equipment)
	}
}

clear_selection :: proc(w: ^World) {
	delete(w.selected_item_name)
	w.selected_item_name = ""
	w.selected_item = NONE
}

has_ground_items :: proc(w: ^World) -> bool {
	for item in w.cells[ch(w, w.avatar).cell].items {
		if ITEMS[it(w, item).type].can_take {
			return true
		}
	}
	return false
}

// "Name(xN)" rows, grouped by name in order of first appearance.
add_item_groups :: proc(g: ^Game, items: []int, ground_only: bool) {
	w := &g.world
	names := make([dynamic]string, context.temp_allocator)
	counts := make([dynamic]int, context.temp_allocator)
	for item in items {
		desc := &ITEMS[it(w, item).type]
		if ground_only && !desc.can_take {
			continue
		}
		found := false
		for name, i in names {
			if name == desc.name {
				counts[i] += 1
				found = true
			}
		}
		if !found {
			append(&names, desc.name)
			append(&counts, 1)
		}
	}
	for name, i in names {
		menu_add(g, fmt.tprintf("%s(x%d)", name, counts[i]), name)
	}
}

items_named :: proc(w: ^World, items: []int, name: string, ground_only: bool) -> []int {
	out := make([dynamic]int, context.temp_allocator)
	for item in items {
		desc := &ITEMS[it(w, item).type]
		if desc.name == name && (!ground_only || desc.can_take) {
			append(&out, item)
		}
	}
	return out[:]
}

// ---- save slots -------------------------------------------------------------------------------

slot_key :: proc(slot: int) -> string {
	return fmt.tprintf("tu:slot%d", slot)
}

slot_exists :: proc(g: ^Game, slot: int) -> bool {
	value, ok := storage_get(g, slot_key(slot))
	return ok && len(value) > 0
}

save_slot :: proc(g: ^Game, slot: int) {
	if !g.has_world || g.storage.set == nil {
		return
	}
	text, ok := world_save(&g.world, context.temp_allocator)
	if ok {
		g.storage.set(slot_key(slot), text)
	}
}

load_slot :: proc(g: ^Game, slot: int) -> bool {
	value, ok := storage_get(g, slot_key(slot))
	if !ok {
		return false
	}
	loaded, lok := world_load(value)
	if !lok {
		return false
	}
	if g.has_world {
		world_destroy(&g.world)
	}
	g.world = loaded
	g.has_world = true
	return true
}

abandon_game :: proc(g: ^Game) {
	if g.has_world {
		world_destroy(&g.world)
		g.has_world = false
	}
}

// ---- commands (HandleCommand) -----------------------------------------------------------------

handle_command :: proc(g: ^Game, command: Command) {
	state := current_state(g)
	w := &g.world
	if is_picker(state) {
		picker_command(g, state, command)
		return
	}
	#partial switch state {
	case .Splash:
		if command == .A {
			set_state(g, .Main_Menu)
		}
	case .About:
		set_state(g, .Main_Menu)
	case .Navigation:
		#partial switch command {
		case .B:
			set_state(g, .Game_Menu)
		case .A:
			set_state(g, .Action_Menu)
		case .Up:
			set_state(g, .Move_North)
		case .Down:
			set_state(g, .Move_South)
		case .Left:
			set_state(g, .Move_West)
		case .Right:
			set_state(g, .Move_East)
		}
	case .Message:
		if command == .A || command == .B {
			message_dismiss(w)
			set_state(g, .Neutral)
		}
	case .Statistics:
		if command == .A || command == .B {
			set_state(g, .Neutral)
		}
	case .Tits_Up, .Won:
		if command == .A && g.time > g.show_until {
			abandon_game(g)
			set_state(g, .Main_Menu)
		}
	}
}

picker_command :: proc(g: ^Game, state: State, command: Command) {
	count := len(g.menu)
	#partial switch command {
	case .B:
		cancel := picker_cancel(state)
		if state == .Options {
			set_current_state(g, .None, false)
		} else if cancel != .None {
			set_state(g, cancel)
		}
	case .A:
		if count > 0 {
			picker_activate(g, state, g.menu[g.menu_index])
		}
	case .Up:
		if count > 0 {
			g.menu_index = (g.menu_index + count - 1) % count
		}
	case .Down:
		if count > 0 {
			g.menu_index = (g.menu_index + 1) % count
		}
	}
}

picker_activate :: proc(g: ^Game, state: State, item: Menu_Item) {
	w := &g.world
	#partial switch state {
	case .Main_Menu:
		switch item.key {
		case EMBARK_TEXT:
			set_state(g, .Embark)
		case LOAD_TEXT:
			set_state(g, .Load)
		case OPTIONS_TEXT:
			set_states(g, .Options, .Main_Menu)
		case ABOUT_TEXT:
			set_state(g, .About)
		}
	case .Game_Menu:
		switch item.key {
		case CONTINUE_TEXT:
			set_state(g, .Neutral)
		case SAVE_TEXT:
			set_state(g, .Save)
		case OPTIONS_TEXT:
			set_states(g, .Options, .Game_Menu)
		case ABANDON_TEXT:
			set_state(g, .Abandon)
		}
	case .Options:
		switch item.key {
		case FULLSCREEN_TEXT:
			g.fullscreen_request = true
		case VOLUME_TEXT:
			set_state(g, .Volume)
		}
	case .Volume:
		g.volume = f32(item.value) / 10
		play_sfx(g, .Player_Hit)
		if g.storage.set != nil {
			g.storage.set("tu:volume", fmt.tprintf("%d", item.value))
		}
		set_header(g, fmt.tprintf("Volume (Currently: %d%%)", item.value * 10))
	case .Abandon:
		if item.key == NO_TEXT {
			set_state(g, .Game_Menu)
		} else {
			abandon_game(g)
			set_state(g, .Main_Menu)
		}
	case .Load:
		if item.value > 0 && slot_exists(g, item.value) && load_slot(g, item.value) {
			set_state(g, .Neutral)
		} else {
			play_sfx(g, .Shucks)
			set_state(g, .Main_Menu)
		}
	case .Save:
		save_slot(g, item.value)
		set_state(g, .Game_Menu)
	case .Interact_Character:
		do_character_verb(w, ch(w, w.avatar).target, Verb(item.value), w.avatar)
		set_state(g, .Neutral)
	case .Action_Menu:
		switch item.key {
		case STATISTICS_TEXT:
			set_state(g, .Statistics)
		case GROUND_TEXT:
			set_state(g, .Ground)
		case INVENTORY_TEXT:
			set_state(g, .Inventory)
		case EQUIPMENT_TEXT:
			set_state(g, .Equipment)
		}
	case .Ground:
		delete(w.selected_item_name)
		w.selected_item_name = clone_string(item.key)
		set_state(g, .Ground_Detail)
	case .Ground_Detail:
		w.selected_item = item.value
		set_state(g, .Ground_Item)
	case .Ground_Item:
		if item.key == TAKE_TEXT {
			set_state(g, .Take)
		}
	case .Inventory:
		delete(w.selected_item_name)
		w.selected_item_name = clone_string(item.key)
		set_state(g, .Inventory_Detail)
	case .Inventory_Detail:
		w.selected_item = item.value
		set_state(g, .Inventory_Item)
	case .Inventory_Item:
		switch item.key {
		case DROP_TEXT:
			set_state(g, .Drop)
		case EQUIP_TEXT:
			set_state(g, .Equip)
		case:
			do_item_verb(w, w.selected_item, Verb(item.value), w.avatar)
			set_state(g, .Neutral)
		}
	case .Equipment:
		w.selected_slot = Slot(item.value)
		w.has_selected_slot = true
		set_state(g, .Equipment_Detail)
	case .Equipment_Detail:
		if item.key == UNEQUIP_TEXT {
			set_state(g, .Unequip)
		}
	}
}

// ---- rendering --------------------------------------------------------------------------------

render :: proc(g: ^Game) {
	c := &g.canvas
	state := current_state(g)
	if is_picker(state) {
		render_picker(g, state)
		return
	}
	w := &g.world
	#partial switch state {
	case .Splash:
		canvas_clear(c, .Black)
		text := "Tits UUp!!"
		x := VIEW_WIDTH / 2 - text_width(text) / 2
		y := VIEW_HEIGHT / 2 - UI_FONT_HEIGHT / 2
		for dy in -1 ..= 1 {
			for dx in -1 ..= 1 {
				if dx != 0 || dy != 0 {
					draw_text(c, x + dx, y + dy, text, .Pink)
				}
			}
		}
		draw_text(c, x, y, text, .Red)
		status_bar(c, "Space/(A) - Continue", .Black, .Light_Gray)
	case .About:
		canvas_clear(c, .Black)
		h := UI_FONT_HEIGHT
		draw_text(c, 0, 0, "About Tits UUp!!", .Orange)
		draw_text(c, 0, h * 2, "Art:", .White)
		draw_text(c, 0, h * 3, "https://vurmux.itch.io/urizen-onebit-tileset", .White)
		draw_text(c, 0, h * 5, "A Production of TheGrumpyGameDev", .White)
		draw_text(c, 0, h * 7, "For Weeksauce (July 2023)", .White)
		draw_text(c, 0, h * 9, "Ported to Odin and WebAssembly (2026)", .White)
	case .Navigation:
		render_navigation(g)
	case .Message:
		canvas_clear(c, .Black)
		message := &w.messages[0]
		y := VIEW_HEIGHT / 2 - UI_FONT_HEIGHT * len(message.lines) / 2
		for line in message.lines {
			draw_text_centered(c, y, line.text, line.hue)
			y += UI_FONT_HEIGHT
		}
		status_bar(c, "Space/(A) - Dismiss", .Black, .Light_Gray)
	case .Statistics:
		canvas_clear(c, .Black)
		avatar := w.avatar
		y := VIEW_HEIGHT / 2 - UI_FONT_HEIGHT * 5 / 2
		lines := [5]struct {
			text: string,
			hue:  Hue,
		} {
			{fmt.tprintf("Health: %d/%d", health(w, avatar), maximum_health(w, avatar)), .Pink},
			{fmt.tprintf("Attack: Max %d Avg %.2f", stat_total(w, avatar, .Maximum_Attack), f64(stat_total(w, avatar, .Attack_Dice)) / 6.0), .Red},
			{fmt.tprintf("Defend: Max %d Avg %.2f", stat_total(w, avatar, .Maximum_Defend), f64(stat_total(w, avatar, .Defend_Dice)) / 6.0), .Green},
			{fmt.tprintf("Encumbrance: %d/%d", encumbrance(w, avatar), maximum_encumbrance(w, avatar)), .Blue},
			{fmt.tprintf("Dignity: %d", dignity(w, avatar)), .Purple},
		}
		for line in lines {
			draw_text_centered(c, y, line.text, line.hue)
			y += UI_FONT_HEIGHT
		}
		header_bar(c, "Statistics", .Orange, .Black)
		status_bar(c, "Space/(A)/Esc/(B) - Go Back", .Black, .Light_Gray)
	case .Tits_Up:
		canvas_clear(c, .Black)
		draw_text_centered(c, VIEW_HEIGHT / 2 - UI_FONT_HEIGHT / 2, "Yer tits up!!", .Red)
		draw_text_centered(c, VIEW_HEIGHT / 2 + UI_FONT_HEIGHT * 3 / 2, ch(w, w.avatar).epitaph, .Blue)
		draw_tile(c, VIEW_WIDTH / 2 - CELL_SIZE / 2, VIEW_HEIGHT / 4 - CELL_SIZE / 2, 2, .Brown)
		if g.time > g.show_until {
			status_bar(c, "Space/(A) - Main Menu", .Black, .Light_Gray)
		}
	case .Won:
		canvas_clear(c, .Black)
		draw_text_centered(c, VIEW_HEIGHT / 2 - UI_FONT_HEIGHT / 2, "You \"won\"!", .Green)
		draw_text_centered(c, VIEW_HEIGHT / 2 + UI_FONT_HEIGHT * 3 / 2, fmt.tprintf("Yer dignity: %d", dignity(w, w.avatar)), .Blue)
		draw_tile(c, VIEW_WIDTH / 2 - CELL_SIZE / 2, VIEW_HEIGHT / 4 - CELL_SIZE / 2, 1, .Brown)
		if g.time > g.show_until {
			status_bar(c, "Space/(A) - Main Menu", .Black, .Light_Gray)
		}
	case:
		// Transient states never stay current; if one does, show nothing rather than stale pixels.
		canvas_clear(c, .Black)
	}
}

render_picker :: proc(g: ^Game, state: State) {
	c := &g.canvas
	canvas_clear(c, .Black)
	fh := UI_FONT_HEIGHT
	canvas_fill(c, 0, VIEW_HEIGHT / 2 - fh / 2, VIEW_WIDTH, fh, .Blue)
	y := VIEW_HEIGHT / 2 - fh / 2 - g.menu_index * fh
	for item, i in g.menu {
		x := VIEW_WIDTH / 2 - text_width(item.label) / 2
		draw_text(c, x, y, item.label, .Black if i == g.menu_index else .Blue)
		y += fh
	}
	header_bar(c, g.header, .Orange, .Black)
	status_bar(c, picker_status(state), .Black, .Light_Gray)
}

render_navigation :: proc(g: ^Game) {
	c := &g.canvas
	w := &g.world
	canvas_clear(c, .Dark_Gray)
	avatar_cell := ch(w, w.avatar).cell
	avatar_column, avatar_row := cell_column(w, avatar_cell), cell_row(w, avatar_cell)
	offset_x := VIEW_WIDTH / 2 - CELL_SIZE / 2 - avatar_column * CELL_SIZE
	offset_y := VIEW_HEIGHT / 2 - CELL_SIZE / 2 - avatar_row * CELL_SIZE
	for column in 0 ..< w.columns {
		plot_x := offset_x + column * CELL_SIZE
		if plot_x >= VIEW_WIDTH || plot_x <= -CELL_SIZE {
			continue
		}
		for row in 0 ..< w.rows {
			plot_y := offset_y + row * CELL_SIZE
			if plot_y >= VIEW_HEIGHT || plot_y <= -CELL_SIZE {
				continue
			}
			index := cell_index(w, column, row)
			cell := &w.cells[index]
			adjacent := abs(column - avatar_column) + abs(row - avatar_row) == 1
			enemy := cell.character != NONE && is_enemy(w, cell.character)
			canvas_fill(c, plot_x, plot_y, CELL_SIZE, CELL_SIZE, .Red if adjacent && enemy else .Black)
			terrain := &TERRAIN[cell.terrain]
			draw_tile(c, plot_x, plot_y, terrain.glyph, terrain.hue)
			if len(cell.items) > 0 {
				desc := &ITEMS[it(w, cell.items[0]).type]
				draw_tile(c, plot_x, plot_y, desc.glyph, desc.hue)
			}
			if cell.character != NONE {
				desc := &CHARACTERS[ch(w, cell.character).type]
				draw_tile(c, plot_x, plot_y, desc.glyph, desc.hue)
			}
		}
	}
	draw_text(c, 0, 0, fmt.tprintf("H: %d/%d", health(w, w.avatar), maximum_health(w, w.avatar)), .Pink)
	if is_overencumbered(w, w.avatar) {
		header_bar(c, "**ENCUMBERED**", .Black, .Red)
	}
	status_bar(c, controls_text("Action Menu", "Game Menu"), .Black, .Light_Gray)
}
