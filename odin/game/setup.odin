package game

// New-game setup (MapInitializer / WorldInitializer and the goblin's initialiser).

world_new :: proc(seed: u64) -> World {
	w: World
	rng_seed(&w.rng, seed)
	w.avatar = NONE
	w.selected_item = NONE
	w.columns, w.rows = MAP_COLUMNS, MAP_ROWS
	for _ in 0 ..< w.columns * w.rows {
		append(&w.cells, Cell{terrain = .Empty, character = NONE})
	}
	// Custom initialiser: walls around the edge, a locked door in the middle of the east wall.
	for column in 0 ..< w.columns {
		w.cells[cell_index(&w, column, 0)].terrain = .Wall
		w.cells[cell_index(&w, column, w.rows - 1)].terrain = .Wall
	}
	for row in 1 ..= w.rows - 2 {
		w.cells[cell_index(&w, 0, row)].terrain = .Wall
		w.cells[cell_index(&w, w.columns - 1, row)].terrain = .Wall
	}
	w.cells[cell_index(&w, w.columns - 1, w.rows / 2)].terrain = .Locked_Door

	populate(&w, .Tizzy, 1)
	populate(&w, .Goblin, GOBLIN_COUNT)
	for loot in ([3]Item_Type{.Key, .Bikini_Top, .Thong}) {
		goblin := random_goblin(&w)
		character_add_item(&w, goblin, item_create(&w, loot))
	}
	for i in 0 ..< len(w.characters) {
		if w.characters[i].type == .Tizzy {
			w.avatar = i
			set_epitaph(&w, i, clone_string("Better luck next time!"))
		}
	}
	return w
}

clone_string :: proc(s: string) -> string {
	b := make([]u8, len(s))
	copy(b, s)
	return string(b)
}

populate :: proc(w: ^World, type: Character_Type, count: int) {
	for _ in 0 ..< count {
		candidates := make([dynamic]int, context.temp_allocator)
		for cell, i in w.cells {
			if TERRAIN[cell.terrain].tenable && cell.character == NONE {
				append(&candidates, i)
			}
		}
		cell := candidates[rng_range(&w.rng, 0, len(candidates) - 1)]
		id := character_create(w, type, cell)
		if type == .Goblin {
			init_goblin(w, id)
		}
	}
}

init_goblin :: proc(w: ^World, id: int) {
	character_add_item(w, id, item_create(w, .Goblin_Corpse))
	for _ in 0 ..< rng_dice(&w.rng, "2d2+-2d1") {
		if rng_chance(&w.rng, 4, 1) {
			character_add_item(w, id, item_create(w, .Rotten_Meat))
		} else {
			character_add_item(w, id, item_create(w, .Meat))
		}
	}
	if rng_chance(&w.rng, 2, 1) {
		character_equip(w, id, .Weapon, item_create(w, .Club))
	}
	if rng_chance(&w.rng, 5, 1) {
		character_equip(w, id, .Shield, item_create(w, .Wooden_Shield))
	}
}

random_goblin :: proc(w: ^World) -> int {
	goblins := make([dynamic]int, context.temp_allocator)
	for cell in w.cells {
		if cell.character != NONE && ch(w, cell.character).type == .Goblin {
			append(&goblins, cell.character)
		}
	}
	return goblins[rng_range(&w.rng, 0, len(goblins) - 1)]
}
