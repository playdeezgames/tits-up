#!/usr/bin/env python3
"""Draws assets/cover.png (630x500) out of the game's own pieces: the Urizen tileset, the 6x8 UI font and the
16-colour palette in odin/game/gfx.odin. Logical canvas 315x250, shown at 2x with nearest-neighbour.
Usage: python3 tools/make_cover.py"""
import json, os, random, re
from PIL import Image

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..")
W, H, SCALE = 315, 250, 2


def load_palette():
    pal = {"Black": (0, 0, 0), "White": (255, 255, 255)}
    for line in open(os.path.join(ROOT, "odin/game/gfx.odin")):
        m = re.match(r"\s*\.(\w+)\s*= 0xFF000000 \| (\d+) << 16 \| (\d+) << 8 \| (\d+)", line)
        if m:
            b, g, r = int(m[2]), int(m[3]), int(m[4])
            pal[m[1]] = (r, g, b)
    return pal


def load_glyphs(name):
    d = json.load(open(os.path.join(ROOT, "assets/fonts", name)))
    return d["Height"], d["Glyphs"]


PAL = load_palette()
TILE_H, TILES = load_glyphs("TitsUp.json")
FONT_H, FONT = load_glyphs("CyFont5x7.json")
img = Image.new("RGB", (W, H), PAL["Black"])
px = img.load()


def put(x, y, hue):
    if 0 <= x < W and 0 <= y < H:
        px[x, y] = PAL[hue]


def tile(x, y, n, hue):
    for row, cols in TILES[chr(n)]["Lines"].items():
        for col in cols:
            put(x + col, y + int(row), hue)


def text_width(s, scale):
    return sum(FONT[ch]["Width"] for ch in s) * scale


def text(x, y, s, hue, scale=1):
    for ch in s:
        g = FONT[ch]
        for row, cols in g["Lines"].items():
            for col in cols:
                for dy in range(scale):
                    for dx in range(scale):
                        put(x + col * scale + dx, y + int(row) * scale + dy, hue)
        x += g["Width"] * scale


def outlined(x, y, s, fg, outline, scale):
    for dy in range(-scale, scale + 1, scale):
        for dx in range(-scale, scale + 1, scale):
            if dx or dy:
                text(x + dx, y + dy, s, outline, scale)
    text(x, y, s, fg, scale)


COLS, ROWS = 26, 20
X0, Y0 = (W - COLS * 12) // 2, (H - ROWS * 12) // 2

# Walls round the room, a locked door in the east wall.
for c in range(COLS):
    for r in range(ROWS):
        if c in (0, COLS - 1) or r in (0, ROWS - 1):
            tile(X0 + c * 12, Y0 + r * 12, 3, "Light_Gray")
tile(X0 + (COLS - 1) * 12, Y0 + 12 * 12, 4, "Orange")

# Title panel (cells kept clear of scatter).
title, tag = "T!ts UUp", ["A game about dying,", "not boobs (sorry!)"]
ts = 5
tx = (W - text_width(title, ts)) // 2
ty = Y0 + 4 * 12 + 2
outlined(tx, ty, title, "Red", "Pink", ts)
for i, line in enumerate(tag):
    text((W - text_width(line, 2)) // 2, ty + 56 + i * 20, line, "Light_Gray", 2)
panel = (2, 3, COLS - 3, 12)  # cells c0, r0, c1, r1 reserved for the title block

rng = random.Random(7)
taken = {(3, 15)}  # Tizzy's cell
tile(X0 + 3 * 12, Y0 + 15 * 12, 1, "Brown")
tile(X0 + 4 * 12, Y0 + 15 * 12, 10, "Brown")  # club in hand's reach


def free_cell():
    while True:
        c, r = rng.randrange(1, COLS - 1), rng.randrange(1, ROWS - 1)
        if (c, r) in taken or (panel[0] <= c <= panel[2] and panel[1] <= r <= panel[3]):
            continue
        if abs(c - 3) + abs(r - 15) < 3 or (c == COLS - 2 and r == 12):
            continue
        taken.add((c, r))
        return c, r


for _ in range(16):
    c, r = free_cell()
    tile(X0 + c * 12, Y0 + r * 12, 5, "Green")
for n, hue in ((9, "Red"), (9, "Red"), (8, "Orange"), (11, "Brown"), (12, "Pink"), (13, "Pink")):
    c, r = free_cell()
    tile(X0 + c * 12, Y0 + r * 12, n, hue)

out = img.resize((W * SCALE, H * SCALE), Image.NEAREST)
path = os.path.join(ROOT, "assets", "cover.png")
out.save(path)
print("wrote", path, out.size)
