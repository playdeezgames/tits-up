package game

import "core:fmt"

// ---- statistics -------------------------------------------------------------------------------

stat_try :: proc(s: ^Stats, k: Stat, default := 0) -> int {
	return s.value[k] if s.has[k] else default
}

stat_set :: proc(s: ^Stats, k: Stat, v: int) {
	s.value[k] = v
	s.has[k] = true
}

// ---- access -----------------------------------------------------------------------------------

ch :: proc(w: ^World, id: int) -> ^Character {
	return &w.characters[id]
}

it :: proc(w: ^World, id: int) -> ^Item {
	return &w.items[id]
}

cell_index :: proc(w: ^World, column, row: int) -> int {
	if column < 0 || row < 0 || column >= w.columns || row >= w.rows {
		return NONE
	}
	return column + row * w.columns
}

cell_column :: proc(w: ^World, index: int) -> int {
	return index % w.columns
}

cell_row :: proc(w: ^World, index: int) -> int {
	return index / w.columns
}

// ---- messages ---------------------------------------------------------------------------------

message_new :: proc(w: ^World) -> int {
	append(&w.messages, Message{})
	return len(w.messages) - 1
}

message_line :: proc(w: ^World, message: int, hue: Hue, format: string, args: ..any) {
	append(&w.messages[message].lines, Message_Line{hue, fmt.aprintf(format, ..args)})
}

message_sfx :: proc(w: ^World, message: int, sfx: Sfx) {
	w.messages[message].sfx = sfx
}

has_messages :: proc(w: ^World) -> bool {
	return len(w.messages) > 0
}

message_dismiss :: proc(w: ^World) {
	if len(w.messages) == 0 {
		return
	}
	message_destroy(&w.messages[0])
	ordered_remove(&w.messages, 0)
}

message_destroy :: proc(m: ^Message) {
	for line in m.lines {
		delete(line.text)
	}
	delete(m.lines)
}

// ---- creation ---------------------------------------------------------------------------------

item_create :: proc(w: ^World, type: Item_Type) -> int {
	item := Item {
		type = type,
	}
	desc := &ITEMS[type]
	for k in desc.has {
		stat_set(&item.stats, k, desc.stats[k])
	}
	for i in 0 ..< len(w.items) {
		if w.items[i].recycled {
			w.items[i] = item
			return i
		}
	}
	append(&w.items, item)
	return len(w.items) - 1
}

item_recycle :: proc(w: ^World, id: int) {
	it(w, id).recycled = true
}

// Creates the character, places it on the cell and sets its starting statistics (CreateCharacter in
// MapInitializer). The type's initialiser (goblin loot) is run by the caller.
character_create :: proc(w: ^World, type: Character_Type, cell: int) -> int {
	c := Character {
		type   = type,
		cell   = cell,
		target = NONE,
	}
	desc := &CHARACTERS[type]
	for k in desc.has {
		stat_set(&c.stats, k, desc.stats[k])
	}
	id := len(w.characters)
	for i in 0 ..< len(w.characters) {
		if w.characters[i].recycled {
			id = i
			break
		}
	}
	if id == len(w.characters) {
		append(&w.characters, c)
	} else {
		character_free(&w.characters[id])
		w.characters[id] = c
	}
	w.cells[cell].character = id
	return id
}

character_free :: proc(c: ^Character) {
	delete(c.items)
	delete(c.equipment)
	delete(c.epitaph)
}

world_destroy :: proc(w: ^World) {
	for &cell in w.cells {
		delete(cell.items)
	}
	delete(w.cells)
	for &c in w.characters {
		character_free(&c)
	}
	delete(w.characters)
	delete(w.items)
	for &m in w.messages {
		message_destroy(&m)
	}
	delete(w.messages)
	delete(w.selected_item_name)
	w^ = {}
}

// ---- inventory plumbing -----------------------------------------------------------------------

remove_value :: proc(list: ^[dynamic]int, value: int) {
	for v, i in list^ {
		if v == value {
			ordered_remove(list, i)
			return
		}
	}
}

character_add_item :: proc(w: ^World, id, item: int) {
	append(&ch(w, id).items, item)
}

character_remove_item :: proc(w: ^World, id, item: int) {
	remove_value(&ch(w, id).items, item)
}

cell_add_item :: proc(w: ^World, cell, item: int) {
	append(&w.cells[cell].items, item)
}

cell_remove_item :: proc(w: ^World, cell, item: int) {
	remove_value(&w.cells[cell].items, item)
}

equipped_in :: proc(c: ^Character, slot: Slot) -> (item: int, ok: bool) {
	for e in c.equipment {
		if e.slot == slot {
			return e.item, true
		}
	}
	return NONE, false
}

// Returns the item in the slot to the inventory.
character_unequip :: proc(w: ^World, id: int, slot: Slot) {
	c := ch(w, id)
	for e, i in c.equipment {
		if e.slot == slot {
			append(&c.items, e.item)
			ordered_remove(&c.equipment, i)
			return
		}
	}
}

character_unequip_item :: proc(w: ^World, id, item: int) {
	c := ch(w, id)
	for i := 0; i < len(c.equipment); {
		if c.equipment[i].item == item {
			append(&c.items, item)
			ordered_remove(&c.equipment, i)
		} else {
			i += 1
		}
	}
}

character_equip :: proc(w: ^World, id: int, slot: Slot, item: int) {
	character_unequip(w, id, slot)
	character_remove_item(w, id, item)
	append(&ch(w, id).equipment, Equip_Entry{slot, item})
}

// Distinct equipped items, in equip order.
equipped_items :: proc(w: ^World, id: int, allocator := context.temp_allocator) -> []int {
	c := ch(w, id)
	out := make([dynamic]int, 0, len(c.equipment), allocator)
	for e in c.equipment {
		found := false
		for o in out {
			if o == e.item {
				found = true
			}
		}
		if !found {
			append(&out, e.item)
		}
	}
	return out[:]
}

is_avatar :: proc(w: ^World, id: int) -> bool {
	return w.avatar == id
}

// Non-avatars vanish: their gear goes back into the pack and everything drops on the floor.
character_recycle :: proc(w: ^World, id: int) {
	if is_avatar(w, id) {
		return
	}
	for item in equipped_items(w, id) {
		character_unequip_item(w, id, item)
	}
	c := ch(w, id)
	for item in c.items {
		cell_add_item(w, c.cell, item)
	}
	clear(&c.items)
	w.cells[c.cell].character = NONE
	c.recycled = true
}
