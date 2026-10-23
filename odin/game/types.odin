package game

// The data model. Everything is addressed by index into the World's arrays (NONE = -1), mirroring the
// original's TU.Data / TU.Persistence split, so a World serialises as plain JSON.

NONE :: -1

Terrain :: enum u8 {
	Empty,
	Wall,
	Locked_Door,
	Door,
}

Character_Type :: enum u8 {
	Tizzy,
	Goblin,
}

Item_Type :: enum u8 {
	Key,
	Goblin_Corpse,
	Meat,
	Rotten_Meat,
	Club,
	Wooden_Shield,
	Bikini_Top,
	Thong,
}

Slot :: enum u8 {
	Weapon,
	Shield,
	Chest,
	Pelvis,
}

Verb :: enum u8 {
	Disengage,
	Fight,
	Eat,
	Use,
}

Stat :: enum u8 {
	Attack_Dice,
	Defend_Dice,
	Dignity,
	Dignity_Buff,
	Durability,
	Encumbrance,
	Health,
	Maximum_Attack,
	Maximum_Defend,
	Maximum_Durability,
	Maximum_Encumbrance,
	Maximum_Health,
}

// Sound effects; the order must match SOUND_FILES in web/index.html.
Sfx :: enum u8 {
	None,
	Enemy_Death,
	Enemy_Hit,
	Miss,
	Player_Death,
	Player_Hit,
	Shucks,
	Craft,
	Eat,
	Take,
	Tasty,
	WooHoo,
	Yoink,
}

// The 16-colour palette, in the original's numbering.
Hue :: enum u8 {
	Black,
	Blue,
	Green,
	Cyan,
	Red,
	Purple,
	Brown,
	Light_Gray,
	Dark_Gray,
	Light_Blue,
	Light_Green,
	Orange,
	Pink,
	Tan,
	Yellow,
	White,
}

Stats :: struct {
	value: [Stat]int,
	has:   [Stat]bool,
}

Equip_Entry :: struct {
	slot: Slot,
	item: int,
}

Character :: struct {
	recycled:  bool,
	type:      Character_Type,
	cell:      int,
	stats:     Stats,
	items:     [dynamic]int,
	equipment: [dynamic]Equip_Entry, // in the order things were equipped
	target:    int,
	epitaph:   string,
}

Item :: struct {
	recycled: bool,
	type:     Item_Type,
	stats:    Stats,
}

Cell :: struct {
	terrain:   Terrain,
	character: int,
	items:     [dynamic]int,
}

Message_Line :: struct {
	hue:  Hue,
	text: string,
}

Message :: struct {
	sfx:   Sfx,
	lines: [dynamic]Message_Line,
}

World :: struct {
	columns, rows:      int,
	cells:              [dynamic]Cell,
	characters:         [dynamic]Character,
	items:              [dynamic]Item,
	messages:           [dynamic]Message,
	avatar:             int,
	rng:                Rng,
	// Picker selections made through the menus (the original kept these as avatar metadata).
	selected_item_name: string,
	selected_item:      int,
	selected_slot:      Slot,
	has_selected_slot:  bool,
}
