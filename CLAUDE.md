# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## State of the repo

"Tits Uup" ("A Game about Dying, not Boobs"; written "T!ts UUp" in-game and on the itch page (retitled Oct 2026; the cover art still says "Tits UUp!!"), "Tits UUp!!" in the `.vbproj`, and the itch slug stays `tits-uup`; July 2023, Weeksauce jam) was a VB.NET / MonoGame (DesktopGL) roguelike. **It now has a complete Odin / `js_wasm32` port in `odin/`, `web/` and `tools/`, which is the live code.** `src/` is the original VB.NET, kept as the **reference** for the rules and the look (it still builds with `dotnet build src/src.sln`, .NET 8, `TreatWarningsAsErrors`); do not extend it. Do not describe the game's deliberate design (pun title, bikini/thong equipment, dice that only count on a six) as a problem.

The agent name for this repo is `sweet-tits`. Tasks are tracked in Coxswain (project `tits-up`; register first with `agent_register`, pass `agent: "sweet-tits"`), not in ad-hoc TODO lists. The repo is private and has no LICENSE on purpose.

The user's knowledge base is the BOK at `/home/yermom/git/bok-of-splorr/splorr` (read `Home.md` first). Relevant notes: `Tech/Odin wasm recipe.md` (this port follows its "platform-driven framebuffer" shape, as in Murder Hobo / Fortune Hunter), `Tech/Release standard.md`, `Tech/Shipping to itch.io.md`, `Tech/Urizen tileset.md`, `Gotchas.md`. Add new lessons to `Gotchas.md` there, not here. The BOK has no `Games/` note for this game yet.

## Commands

```bash
tools/test.sh               # regenerates font_data.odin, native tests (odin/game), odin check of both packages for js_wasm32
tools/build.sh [--size]     # build/web (game.wasm, odin.js, index.html, sfx/); --size for shipping
python3 -m http.server -d build/web 8000   # wasm cannot load from file://
odin test odin/game -collection:tu=odin -o:speed -define:ODIN_TEST_THREADS=1 -define:ODIN_TEST_NAMES=game.fighting_ends_with_damage_dealt_and_never_hangs   # one test
tools/make_shots.sh         # assets/screenshots/*.png from the real renderer (native, 3x)
python3 tools/gen_fonts.py  # assets/fonts/*.json -> odin/game/font_data.odin (generated; do not hand-edit)
tools/ship.sh [--push]      # test, size build, zip build/tits-uup-html5.zip; pushes to itch ONLY with --push. shippit.sh is a one-line wrapper
```

Packages are imported with `-collection:tu=odin`. Use `-vet-shadowing -vet-unused` explicitly (`odin check` does not apply them; the scripts do). Odin is `dev-2026-07` at `~/.local/bin/odin`.

**Never `git push`, run `tools/ship.sh --push`, `shippit.sh --push` or `butler push` unless the user explicitly says so**; if "push" is ambiguous, ask git or itch. A commit-and-push command may be blocked by the permission classifier; then the user runs it.

## Architecture of the Odin port

- `odin/game` (package `game`, **no platform imports**, native-testable): `types.odin` (World: flat arrays addressed by index, `NONE = -1`), `descriptors.odin` (static content tables as enumerated arrays), `world.odin` + `rules.odin` (inventory plumbing, combat, wear, movement, verbs; messages are queued on the world), `setup.odin` (new game), `save.odin` (World as JSON, `world_valid` checks every index before a load is accepted), `rng.odin` (own SplitMix64 plus the original's dice-string roller), `gfx.odin` + `font_data.odin` (320x180 hue-index canvas, 6x8 UI font, 12x12 tiles), `ui.odin` (the whole state machine and renderer). Test files are `#+build !js`.
- `odin/platform/web` (`#+build js`): `step` (odin.js calls it every frame with seconds), `platform_*` exports, and imports for localStorage and entropy. `web/index.html` is the JS shim: it blits `state.pixels` (RGBA) to a canvas, maps keys and the touch pad to `platform_command`, plays `sfx/*.wav` through Web Audio (the `SOUND_FILES` order must match `Sfx` in `types.odin`), and asks for fullscreen inside the key handler.
- `Game` is one struct; the platform gives it a `Storage{get,set}` and a seed. Time only enters through `game_frame(dt)`; held commands repeat after 1.0 s then every 0.3 s, as in the original.
- UI states mirror the original's `BaseGameController`: `State` enum, a depth-16 stack, `set_current_state/set_states`, and `state_start` standing in for `OnStart`. Pickers share one `menu` array; transient states (Embark, Move*, Take, Drop, Equip, Unequip, Neutral) only run in `state_start` and redirect.

Deliberate departures from the original: saves and volume are localStorage keys `tu:slot1..5` and `tu:volume` (itch shares one origin, keep keys prefixed); no Window Size option and no Quit / ConfirmQuit (it is a browser tab); the one map is hard-coded (the original's generic map-type table had a single entry and its maze generator was never used); items and equipment keep insertion order instead of .NET HashSet/Dictionary order, so the RNG draws of a given seed do not reproduce the original's; fights and the Fight verb loop are capped at 1000 rounds.

## Architecture of the VB.NET original (reference)

Strict layering via project references (`src/src.sln`):

`TitsUp` (exe, `Program.vb`, `Content/`, `config.json`) → `TU.Presentation` → `TU.Business` → `TU.Persistence` → `TU.Data`, plus the generic `AOS.UI` / `AOS.Presentation` / `SPLORR.Game` libraries. `FontMaker` is a side tool that turns `source.png` (a tile sheet) into the glyph JSON fonts.

- **TU.Data**: plain serialisable DTOs. `WorldData` holds lists of `MapData`, `CharacterData`, `ItemData`, `MessageData` plus the avatar's character id. Everything is addressed by integer id. This is the whole save format (JSON via System.Text.Json).
- **TU.Persistence**: `IWorld`/`ICell`/`ICharacter`/`IItem`/`IMap`/`IMessage` wrappers; each `*DataClient` is a view over a DTO inside `WorldData`. No game rules.
- **TU.Business**: the rules. `Model/` exposes `IWorldModel` (Map, Avatar, TargetCharacter, Messages, `IsTitsUp`, `HasWon`, Embark/Abandon/Save/Load); avatar behaviour is split into sub-models (Combat, Encumbrance, Equipment, Ground, Inventory, SelectedItem). Content is **data-driven by string-keyed descriptor modules**: `TerrainTypes`, `CharacterTypes`, `ItemTypes`, `EquipSlotTypes`, `RecipeTypes`, `MapTypes`, `StatisticTypes`, `VerbTypes` (each a `Friend Module` with a `descriptors` dictionary and a `ToXDescriptor` extension). `Initializers/` build a new world (maze map via `SPLORR.Game.Maze`, avatar, items). Randomness is `SPLORR.Game.RNG` (weighted-table generators over `System.Random`).
- **TU.Presentation**: a state machine. `GameController` registers every `GameState` (Navigation, Move*, ActionMenu, Ground, Inventory, Equipment, Take/Drop/Equip/Unequip, InteractCharacter, Message, Statistics, TitsUp = death, Won) on top of the boilerplate states in `AOS.UI` (Splash, MainMenu, Options, Volume, WindowSize, Load/Save with 5 slots, About, Confirm quit/abandon). States are given `(controller, setState, context)`; `TitsUpContext` supplies model, About text and save slots (`slotN.json`).
- **AOS.UI / AOS.Presentation**: render to a software `IPixelSink` display buffer (320x180 logical, scaled 3x by MonoGame), with a 16-colour `Hue` palette, bitmap `Font`s loaded from glyph JSON (`Content/CyFont*.json`; `TitsUp.json` is the **tileset stored as glyphs** of 12x12 pixels, indexed by `ChrW(n)`, derived from vurmux's Urizen 1-bit tileset, CC0), and input mapped from `Content/keys.json` to abstract `Command`s (A, B, Up, Down, Left, Right). `Host.vb` is the only MonoGame-specific piece; sound is `Content/*.wav` via `BoilerplateSfx`.

The original is already a pure software framebuffer plus a command-driven state machine, so the port maps directly onto the BOK's shared-framebuffer variant: port Data+Business as a pure Odin rules package (RNG and clock passed in, natively testable), keep the state machine, draw the 320x180 buffer through one JS blit import, and replace file I/O (below).

## Shipping (itch.io)

Page slug `tits-uup` under `thegrumpygamedev`; the user deleted the 2023 page and recreated it in Oct 2026 (still a draft when first pushed); the browser build is the only upload, on the `html` channel (first push Oct 25 2026, build #2207583; the user ticks "played in the browser" by hand). Layout follows the BOK's Release standard: `ITCH_DESCRIPTION.md` (pasted in by the user, includes the AI-assisted-port disclosure), `assets/cover.png` (630x500), `assets/screenshots/`, `devlog/YYYYMMDD/devlog.md`, `tools/ship.sh`, wrapper `shippit.sh`. The old `ss/` folder holds screenshots of the VB version and is left for the user to remove. Credit vurmux's Urizen tileset (CC0) in About and on the page.
