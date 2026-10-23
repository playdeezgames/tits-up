package game

import "core:encoding/json"

// Saves are the World as JSON. Loading validates every index and enum so a damaged or foreign
// localStorage value is refused instead of crashing the game.

world_save :: proc(w: ^World, allocator := context.allocator) -> (text: string, ok: bool) {
	data, err := json.marshal(w^, {}, allocator)
	if err != nil {
		return "", false
	}
	return string(data), true
}

world_load :: proc(text: string) -> (w: World, ok: bool) {
	err := json.unmarshal(transmute([]u8)text, &w, .JSON)
	if err != nil || !world_valid(&w) {
		world_destroy(&w)
		return {}, false
	}
	return w, true
}

world_valid :: proc(w: ^World) -> bool {
	if w.columns != MAP_COLUMNS || w.rows != MAP_ROWS || len(w.cells) != w.columns * w.rows {
		return false
	}
	nc, ni := len(w.characters), len(w.items)
	if w.avatar < 0 || w.avatar >= nc || w.characters[w.avatar].recycled {
		return false
	}
	if w.selected_item >= ni || w.selected_item < NONE {
		return false
	}
	for cell, i in w.cells {
		if cell.character < NONE || cell.character >= nc {
			return false
		}
		if cell.character != NONE && (w.characters[cell.character].recycled || w.characters[cell.character].cell != i) {
			return false
		}
		for item in cell.items {
			if item < 0 || item >= ni {
				return false
			}
		}
	}
	for c in w.characters {
		if c.recycled {
			continue
		}
		if c.cell < 0 || c.cell >= len(w.cells) || c.target < NONE || c.target >= nc {
			return false
		}
		for item in c.items {
			if item < 0 || item >= ni {
				return false
			}
		}
		for e in c.equipment {
			if e.item < 0 || e.item >= ni {
				return false
			}
		}
		if c.stats.value[.Health] < 0 || c.stats.value[.Maximum_Health] < c.stats.value[.Health] {
			return false
		}
	}
	for m in w.messages {
		for line in m.lines {
			if int(line.hue) >= len(Hue) {
				return false
			}
		}
	}
	return true
}
