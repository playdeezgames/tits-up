package game

// The software framebuffer: 320x180 hue indices, bitmap text and 12x12 tiles. Same drawing model as the
// original's IPixelSink: every write is bounds-checked and clipped.

VIEW_WIDTH :: 320
VIEW_HEIGHT :: 180
CELL_SIZE :: 12

Canvas :: struct {
	pixels: [VIEW_WIDTH * VIEW_HEIGHT]Hue,
}

// Each entry is 0xAABBGGRR, so the bytes in memory are R, G, B, A.
PALETTE := [Hue]u32 {
	.Black       = 0xFF000000,
	.Blue        = 0xFF000000 | 215 << 16 | 75 << 8 | 42,
	.Green       = 0xFF000000 | 20 << 16 | 105 << 8 | 29,
	.Cyan        = 0xFF000000 | 208 << 16 | 208 << 8 | 41,
	.Red         = 0xFF000000 | 35 << 16 | 35 << 8 | 173,
	.Purple      = 0xFF000000 | 192 << 16 | 38 << 8 | 129,
	.Brown       = 0xFF000000 | 25 << 16 | 74 << 8 | 129,
	.Light_Gray  = 0xFF000000 | 160 << 16 | 160 << 8 | 160,
	.Dark_Gray   = 0xFF000000 | 87 << 16 | 87 << 8 | 87,
	.Light_Blue  = 0xFF000000 | 255 << 16 | 175 << 8 | 157,
	.Light_Green = 0xFF000000 | 122 << 16 | 197 << 8 | 129,
	.Orange      = 0xFF000000 | 51 << 16 | 146 << 8 | 255,
	.Pink        = 0xFF000000 | 243 << 16 | 205 << 8 | 255,
	.Tan         = 0xFF000000 | 187 << 16 | 222 << 8 | 233,
	.Yellow      = 0xFF000000 | 51 << 16 | 238 << 8 | 255,
	.White       = 0xFFFFFFFF,
}

canvas_set :: proc(c: ^Canvas, x, y: int, hue: Hue) {
	if x < 0 || y < 0 || x >= VIEW_WIDTH || y >= VIEW_HEIGHT {
		return
	}
	c.pixels[x + y * VIEW_WIDTH] = hue
}

canvas_fill :: proc(c: ^Canvas, x, y, w, h: int, hue: Hue) {
	for dy in 0 ..< h {
		for dx in 0 ..< w {
			canvas_set(c, x + dx, y + dy, hue)
		}
	}
}

canvas_clear :: proc(c: ^Canvas, hue: Hue) {
	canvas_fill(c, 0, 0, VIEW_WIDTH, VIEW_HEIGHT, hue)
}

text_width :: proc(text: string) -> int {
	return len(text) * UI_FONT_WIDTH
}

// Characters outside the font (32..127) are skipped but still take their width.
draw_text :: proc(c: ^Canvas, x, y: int, text: string, hue: Hue) {
	x := x
	for r in text {
		index := int(r) - UI_FONT_FIRST
		if index >= 0 && index < UI_FONT_COUNT {
			glyph := &UI_GLYPHS[index]
			for row in 0 ..< UI_FONT_HEIGHT {
				bits := glyph[row]
				for col in 0 ..< UI_FONT_WIDTH {
					if bits & (1 << uint(col)) != 0 {
						canvas_set(c, x + col, y + row, hue)
					}
				}
			}
		}
		x += UI_FONT_WIDTH
	}
}

draw_text_centered :: proc(c: ^Canvas, y: int, text: string, hue: Hue) {
	draw_text(c, VIEW_WIDTH / 2 - text_width(text) / 2, y, text, hue)
}

// Draws set pixels of a 12x12 tile in the hue; unset pixels are transparent.
draw_tile :: proc(c: ^Canvas, x, y: int, glyph: u8, hue: Hue) {
	tile := &TILE_GLYPHS[glyph]
	for row in 0 ..< TILE_SIZE {
		bits := tile[row]
		for col in 0 ..< TILE_SIZE {
			if bits & (1 << uint(col)) != 0 {
				canvas_set(c, x + col, y + row, hue)
			}
		}
	}
}

status_bar :: proc(c: ^Canvas, text: string, foreground, background: Hue) {
	canvas_fill(c, 0, VIEW_HEIGHT - UI_FONT_HEIGHT, VIEW_WIDTH, UI_FONT_HEIGHT, background)
	draw_text_centered(c, VIEW_HEIGHT - UI_FONT_HEIGHT, text, foreground)
}

header_bar :: proc(c: ^Canvas, text: string, foreground, background: Hue) {
	canvas_fill(c, 0, 0, VIEW_WIDTH, UI_FONT_HEIGHT, background)
	draw_text_centered(c, 0, text, foreground)
}

canvas_to_rgba :: proc(c: ^Canvas, out: ^[VIEW_WIDTH * VIEW_HEIGHT]u32) {
	for p, i in c.pixels {
		out[i] = PALETTE[p]
	}
}
