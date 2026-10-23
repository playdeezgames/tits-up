package game

// Static content tables, in the original's TerrainTypes / CharacterTypes / ItemTypes / ... modules.
// Glyph numbers index the 12x12 tileset (font_data.odin).

Terrain_Desc :: struct {
	name:    string,
	glyph:   u8,
	hue:     Hue,
	tenable: bool,
}

TERRAIN := [Terrain]Terrain_Desc {
	.Empty       = {"Empty", 0, .Black, true},
	.Wall        = {"Wall", 3, .Light_Gray, false},
	.Locked_Door = {"Locked Door", 4, .Orange, false},
	.Door        = {"Door", 7, .Orange, true},
}

Character_Desc :: struct {
	name:  string,
	glyph: u8,
	hue:   Hue,
	enemy: bool,
	// Starting statistics.
	stats: [Stat]int,
	has:   bit_set[Stat],
}

CHARACTERS := [Character_Type]Character_Desc {
	.Tizzy = {
		name = "Tizzy",
		glyph = 1,
		hue = .Brown,
		has = {.Health, .Maximum_Health, .Attack_Dice, .Maximum_Attack, .Defend_Dice, .Maximum_Defend, .Maximum_Encumbrance, .Dignity, .Dignity_Buff},
		stats = #partial {
			.Health = 3,
			.Maximum_Health = 3,
			.Attack_Dice = 1,
			.Maximum_Attack = 1,
			.Defend_Dice = 4,
			.Maximum_Defend = 2,
			.Maximum_Encumbrance = 10,
			.Dignity = -10,
			.Dignity_Buff = 0,
		},
	},
	.Goblin = {
		name = "Goblin",
		glyph = 5,
		hue = .Green,
		enemy = true,
		has = {.Health, .Maximum_Health, .Attack_Dice, .Maximum_Attack, .Defend_Dice, .Maximum_Defend, .Dignity, .Dignity_Buff},
		stats = #partial {
			.Health = 1,
			.Maximum_Health = 1,
			.Attack_Dice = 4,
			.Maximum_Attack = 2,
			.Defend_Dice = 1,
			.Maximum_Defend = 1,
			.Dignity = 0,
			.Dignity_Buff = 1,
		},
	},
}

Item_Desc :: struct {
	name:      string,
	glyph:     u8,
	hue:       Hue,
	slot:      Slot,
	can_equip: bool,
	is_weapon: bool,
	is_armor:  bool,
	can_take:  bool,
	verb:      Verb, // the item's one verb, if has_verb
	has_verb:  bool,
	stats:     [Stat]int,
	has:       bit_set[Stat],
}

ITEMS := [Item_Type]Item_Desc {
	.Key = {
		name = "Key", glyph = 8, hue = .Orange, can_take = true, has_verb = true, verb = .Use,
		has = {.Encumbrance}, stats = #partial {.Encumbrance = 0},
	},
	.Goblin_Corpse = {
		name = "Goblin Corpse", glyph = 6, hue = .Green, can_take = false,
		has = {.Encumbrance}, stats = #partial {.Encumbrance = 25},
	},
	.Meat = {
		name = "Meat", glyph = 9, hue = .Red, can_take = true, has_verb = true, verb = .Eat,
		has = {.Encumbrance}, stats = #partial {.Encumbrance = 0},
	},
	.Rotten_Meat = {
		name = "Meat", glyph = 9, hue = .Red, can_take = true, has_verb = true, verb = .Eat,
		has = {.Encumbrance}, stats = #partial {.Encumbrance = 0},
	},
	.Club = {
		name = "Club", glyph = 10, hue = .Brown, can_take = true, can_equip = true, slot = .Weapon, is_weapon = true,
		has = {.Encumbrance, .Durability, .Maximum_Durability, .Attack_Dice, .Maximum_Attack},
		stats = #partial {.Encumbrance = 5, .Durability = 10, .Maximum_Durability = 10, .Attack_Dice = 2, .Maximum_Attack = 0},
	},
	.Wooden_Shield = {
		name = "Wooden Shield", glyph = 11, hue = .Brown, can_take = true, can_equip = true, slot = .Shield, is_armor = true,
		has = {.Encumbrance, .Durability, .Maximum_Durability, .Defend_Dice, .Maximum_Defend},
		stats = #partial {.Encumbrance = 5, .Durability = 10, .Maximum_Durability = 10, .Defend_Dice = 2, .Maximum_Defend = 1},
	},
	.Bikini_Top = {
		name = "Bikini Top", glyph = 12, hue = .Pink, can_take = true, can_equip = true, slot = .Chest, is_armor = true,
		has = {.Maximum_Encumbrance, .Maximum_Durability, .Durability, .Dignity_Buff},
		stats = #partial {.Maximum_Encumbrance = 10, .Maximum_Durability = 25, .Durability = 25, .Dignity_Buff = 10},
	},
	.Thong = {
		name = "Thong", glyph = 13, hue = .Pink, can_take = true, can_equip = true, slot = .Pelvis, is_armor = true,
		has = {.Maximum_Encumbrance, .Maximum_Durability, .Durability, .Dignity_Buff},
		stats = #partial {.Maximum_Encumbrance = 5, .Maximum_Durability = 50, .Durability = 50, .Dignity_Buff = 10},
	},
}

SLOT_NAMES := [Slot]string {
	.Weapon = "Weapon",
	.Shield = "Shield",
	.Chest  = "Chest",
	.Pelvis = "Pelvis",
}

VERB_NAMES := [Verb]string {
	.Disengage = "Disengage",
	.Fight     = "Fight!",
	.Eat       = "Eat",
	.Use       = "Use",
}

// Map type "Initial": the only map. 9x9, walled border, a locked door in the middle of the east wall.
MAP_COLUMNS :: 9
MAP_ROWS :: 9
GOBLIN_COUNT :: 12
