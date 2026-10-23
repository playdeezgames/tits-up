package game

import "core:fmt"

// The rules, ported from TU.Business (CharacterExtensions, ItemExtensions, the type descriptors).
// Randomness comes from w.rng; messages are queued on the world for the UI to show.

// ---- derived statistics -----------------------------------------------------------------------

char_name :: proc(w: ^World, id: int) -> string {
	return CHARACTERS[ch(w, id).type].name
}

item_name :: proc(w: ^World, id: int) -> string {
	return ITEMS[it(w, id).type].name
}

// Armour and weapons show their wear; everything else is just the name. Result is on the temp allocator.
item_full_name :: proc(w: ^World, id: int) -> string {
	item := it(w, id)
	desc := &ITEMS[item.type]
	if desc.is_weapon || desc.is_armor {
		return fmt.tprintf("%s(%d/%d)", desc.name, stat_try(&item.stats, .Durability), stat_try(&item.stats, .Maximum_Durability))
	}
	return desc.name
}

item_encumbrance :: proc(w: ^World, id: int) -> int {
	return stat_try(&it(w, id).stats, .Encumbrance)
}

item_set_durability :: proc(w: ^World, id, durability: int) {
	item := it(w, id)
	stat_set(&item.stats, .Durability, clamp(durability, 0, stat_try(&item.stats, .Maximum_Durability)))
}

item_is_broken :: proc(w: ^World, id: int) -> bool {
	return stat_try(&it(w, id).stats, .Durability) == 0
}

// Base statistic plus whatever the equipped items add.
stat_total :: proc(w: ^World, id: int, k: Stat) -> int {
	total := stat_try(&ch(w, id).stats, k)
	for item in equipped_items(w, id) {
		total += stat_try(&it(w, item).stats, k)
	}
	return total
}

health :: proc(w: ^World, id: int) -> int {
	return ch(w, id).stats.value[.Health]
}

maximum_health :: proc(w: ^World, id: int) -> int {
	return ch(w, id).stats.value[.Maximum_Health]
}

set_health :: proc(w: ^World, id, value: int) {
	c := ch(w, id)
	c.stats.value[.Health] = clamp(value, 0, maximum_health(w, id))
}

is_tits_up :: proc(w: ^World, id: int) -> bool {
	return health(w, id) == 0
}

is_enemy :: proc(w: ^World, id: int) -> bool {
	return CHARACTERS[ch(w, id).type].enemy
}

encumbrance :: proc(w: ^World, id: int) -> int {
	total := 0
	for item in ch(w, id).items {
		total += item_encumbrance(w, item)
	}
	for item in equipped_items(w, id) {
		total += item_encumbrance(w, item)
	}
	return total
}

maximum_encumbrance :: proc(w: ^World, id: int) -> int {
	return stat_total(w, id, .Maximum_Encumbrance)
}

is_overencumbered :: proc(w: ^World, id: int) -> bool {
	return encumbrance(w, id) > maximum_encumbrance(w, id)
}

dignity :: proc(w: ^World, id: int) -> int {
	total := ch(w, id).stats.value[.Dignity]
	for item in equipped_items(w, id) {
		total += stat_try(&it(w, item).stats, .Dignity_Buff)
	}
	return total
}

// ---- adjacency --------------------------------------------------------------------------------

NEIGHBOR_DELTAS := [4][2]int{{0, -1}, {0, 1}, {-1, 0}, {1, 0}}

Neighbors :: struct {
	cells: [4]int,
	count: int,
}

neighbors :: proc(w: ^World, cell: int) -> (out: Neighbors) {
	column, row := cell_column(w, cell), cell_row(w, cell)
	for d in NEIGHBOR_DELTAS {
		n := cell_index(w, column + d.x, row + d.y)
		if n != NONE {
			out.cells[out.count] = n
			out.count += 1
		}
	}
	return
}

Enemies :: struct {
	ids:   [4]int,
	count: int,
}

adjacent_enemies :: proc(w: ^World, id: int) -> (out: Enemies) {
	n := neighbors(w, ch(w, id).cell)
	for i in 0 ..< n.count {
		other := w.cells[n.cells[i]].character
		if other != NONE && is_enemy(w, other) {
			out.ids[out.count] = other
			out.count += 1
		}
	}
	return
}

// ---- combat -----------------------------------------------------------------------------------

// A die here is 1d6/6: it adds 1 only on a six.
roll_dice_pool :: proc(w: ^World, dice: int) -> int {
	total := 0
	for _ in 0 ..< dice {
		total += rng_dice(&w.rng, "1d6/6")
	}
	return total
}

roll_attack :: proc(w: ^World, id: int) -> int {
	return clamp(roll_dice_pool(w, stat_total(w, id, .Attack_Dice)), 0, stat_total(w, id, .Maximum_Attack))
}

roll_defend :: proc(w: ^World, id: int) -> int {
	return clamp(roll_dice_pool(w, stat_total(w, id, .Defend_Dice)), 0, stat_total(w, id, .Maximum_Defend))
}

// Picks a random equipped weapon (or armour) for each point of wear; broken gear is destroyed.
do_wear :: proc(w: ^World, id: int, wear: int, message: int, weapons: bool) -> bool {
	result := false
	wear := wear
	for wear > 0 {
		pool := make([dynamic]int, context.temp_allocator)
		for item in equipped_items(w, id) {
			desc := &ITEMS[it(w, item).type]
			if desc.is_weapon if weapons else desc.is_armor {
				append(&pool, item)
			}
		}
		if len(pool) == 0 {
			break
		}
		item := pool[rng_range(&w.rng, 0, len(pool) - 1)]
		item_set_durability(w, item, stat_try(&it(w, item).stats, .Durability) - 1)
		if item_is_broken(w, item) {
			message_line(w, message, .Red, "%s's %s broke!", char_name(w, id), item_name(w, item))
			character_unequip_item(w, id, item)
			character_remove_item(w, id, item)
			item_recycle(w, item)
			result = true
		}
		wear -= 1
	}
	return result
}

// Returns true when something was lost (damage dealt, or gear broken) on either side.
attack :: proc(w: ^World, attacker, defender: int, header := "") -> bool {
	result := false
	message := message_new(w)
	if len(header) > 0 {
		message_line(w, message, .Light_Gray, "%s", header)
	}
	message_line(w, message, .Light_Gray, "%s attacks %s!", char_name(w, attacker), char_name(w, defender))
	attack_roll := roll_attack(w, attacker)
	message_line(w, message, .Light_Gray, "%s rolls an attack of %d", char_name(w, attacker), attack_roll)
	if attack_roll > 0 {
		if do_wear(w, attacker, attack_roll, message, true) {
			result = true
		}
		defend_roll := roll_defend(w, defender)
		message_line(w, message, .Light_Gray, "%s rolls a defend of %d", char_name(w, defender), defend_roll)
		damage := max(0, attack_roll - defend_roll)
		if do_wear(w, defender, damage, message, false) {
			result = true
		}
		if damage > 0 {
			message_line(w, message, .Light_Gray, "%s takes %d damage", char_name(w, defender), damage)
			set_health(w, defender, health(w, defender) - damage)
			if is_tits_up(w, defender) {
				ch(w, attacker).stats.value[.Dignity] += ch(w, defender).stats.value[.Dignity_Buff]
				set_epitaph(w, defender, fmt.aprintf("Killed by %s!", char_name(w, attacker)))
				message_line(w, message, .Light_Gray, "%s makes %s go tits up", char_name(w, attacker), char_name(w, defender))
				message_sfx(w, message, .Player_Death if is_avatar(w, defender) else .Enemy_Death)
				ch(w, attacker).target = NONE
				character_recycle(w, defender)
			} else {
				message_line(w, message, .Light_Gray, "%s has %d health remaining", char_name(w, defender), health(w, defender))
				message_sfx(w, message, .Player_Hit if is_avatar(w, defender) else .Enemy_Hit)
			}
			result = true
		} else {
			message_line(w, message, .Light_Gray, "%s misses!", char_name(w, attacker))
			message_sfx(w, message, .Miss)
		}
	} else {
		message_line(w, message, .Light_Gray, "%s misses!", char_name(w, attacker))
		message_sfx(w, message, .Miss)
	}
	return result
}

set_epitaph :: proc(w: ^World, id: int, text: string) {
	c := ch(w, id)
	delete(c.epitaph)
	c.epitaph = text
}

// ---- movement ---------------------------------------------------------------------------------

do_opportunity_attacks :: proc(w: ^World, id: int) {
	if !is_avatar(w, id) {
		return
	}
	enemies := adjacent_enemies(w, id)
	for i in 0 ..< enemies.count {
		if is_tits_up(w, id) {
			break
		}
		attack(w, enemies.ids[i], id, fmt.tprintf("Opportunity Attack %d of %d", i + 1, enemies.count))
	}
}

character_move :: proc(w: ^World, id: int, dx, dy: int) {
	cell := ch(w, id).cell
	next := cell_index(w, cell_column(w, cell) + dx, cell_row(w, cell) + dy)
	if next == NONE || !TERRAIN[w.cells[next].terrain].tenable {
		return
	}
	ch(w, id).target = w.cells[next].character // NONE when empty
	if w.cells[next].character != NONE {
		return
	}
	do_opportunity_attacks(w, id)
	w.cells[next].character = id
	w.cells[cell].character = NONE
	ch(w, id).cell = next
}

// The avatar's move, which refuses when overloaded.
avatar_move :: proc(w: ^World, dx, dy: int) {
	if is_overencumbered(w, w.avatar) {
		message_line(w, message_new(w), .Light_Gray, "%s is carrying too much.", char_name(w, w.avatar))
		return
	}
	character_move(w, w.avatar, dx, dy)
}

has_won :: proc(w: ^World) -> bool {
	return w.cells[ch(w, w.avatar).cell].terrain == .Door
}

// ---- verbs ------------------------------------------------------------------------------------

// Character verbs, run against a target character (the goblin's Fight and Disengage).
ENEMY_VERBS := [2]Verb{.Fight, .Disengage}

character_verbs :: proc(w: ^World, id: int) -> []Verb {
	if CHARACTERS[ch(w, id).type].enemy {
		return ENEMY_VERBS[:]
	}
	return nil
}

MAX_FIGHT_ROUNDS :: 1000

// Fight: rounds repeat until somebody takes damage or loses gear. Misses stay in the log.
do_character_verb :: proc(w: ^World, target: int, verb: Verb, source: int) {
	switch verb {
	case .Fight:
		for _ in 0 ..< MAX_FIGHT_ROUNDS {
			for has_messages(w) {
				message_dismiss(w)
			}
			damage_occurred := attack(w, source, target)
			enemies := adjacent_enemies(w, source)
			for i in 0 ..< enemies.count {
				if is_tits_up(w, source) {
					break
				}
				if attack(w, enemies.ids[i], source, fmt.tprintf("Counter Attack %d of %d", i + 1, enemies.count)) {
					damage_occurred = true
				}
			}
			if damage_occurred {
				break
			}
		}
	case .Disengage:
		ch(w, source).target = NONE
	case .Eat, .Use:
	}
}

item_verbs :: proc(w: ^World, item: int) -> (verb: Verb, ok: bool) {
	desc := &ITEMS[it(w, item).type]
	return desc.verb, desc.has_verb
}

do_item_verb :: proc(w: ^World, item: int, verb: Verb, character: int) {
	switch it(w, item).type {
	case .Key:
		use_key(w, character, item)
	case .Meat:
		eat_meat(w, character, item)
	case .Rotten_Meat:
		eat_rotten_meat(w, character, item)
	case .Goblin_Corpse, .Club, .Wooden_Shield, .Bikini_Top, .Thong:
	}
}

use_key :: proc(w: ^World, character, item: int) {
	n := neighbors(w, ch(w, character).cell)
	for i in 0 ..< n.count {
		if w.cells[n.cells[i]].terrain == .Locked_Door {
			w.cells[n.cells[i]].terrain = .Door
			character_remove_item(w, character, item)
			item_recycle(w, item)
			return
		}
	}
	m := message_new(w)
	message_line(w, m, .Red, "You need to be near the door to unlock it!")
	message_sfx(w, m, .Shucks)
}

eat_meat :: proc(w: ^World, character, item: int) {
	m := message_new(w)
	message_line(w, m, .Light_Gray, "%s eats %s", char_name(w, character), item_name(w, item))
	message_sfx(w, m, .Tasty)
	character_remove_item(w, character, item)
	item_recycle(w, item)
	message_line(w, m, .Green, "%s regains 1 health!", char_name(w, character))
	set_health(w, character, health(w, character) + 1)
	message_line(w, m, .Light_Gray, "%s has %d health!", char_name(w, character), health(w, character))
}

eat_rotten_meat :: proc(w: ^World, character, item: int) {
	m := message_new(w)
	message_line(w, m, .Light_Gray, "%s eats %s", char_name(w, character), item_name(w, item))
	character_remove_item(w, character, item)
	item_recycle(w, item)
	message_line(w, m, .Red, "%s is rotten!", item_name(w, item))
	message_line(w, m, .Light_Gray, "%s takes 1 damage!", char_name(w, character))
	set_health(w, character, health(w, character) - 1)
	if is_tits_up(w, character) {
		set_epitaph(w, character, fmt.aprintf("Be careful which meat you put in yer mouth!"))
		message_line(w, m, .Light_Gray, "%s is tits up!", char_name(w, character))
		message_sfx(w, m, .Player_Death)
	} else {
		message_line(w, m, .Light_Gray, "%s has %d health left!", char_name(w, character), health(w, character))
		message_sfx(w, m, .Player_Hit)
	}
}
