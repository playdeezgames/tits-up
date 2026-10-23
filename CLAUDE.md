# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## State of the repo

"Tits Uup" ("A Game about Dying, not Boobs"; written "Tits UUp!!" in-game and in the `.vbproj`/itch slug `tits-uup`, July 2023, Weeksauce jam, never fully promoted) is a **VB.NET / MonoGame (DesktopGL) roguelike that is being ported to Odin / `js_wasm32`**. As of now there is no Odin code: everything under `src/` is the original VB.NET, which is the **reference implementation** to port from. Do not describe the game's deliberate design (pun title, bikini/thong equipment, etc.) as a problem.

The agent name for this repo is `sweet-tits`. Tasks are tracked in Coxswain (project `tits-up`; register first with `agent_register`, pass `agent: "sweet-tits"`), not in ad-hoc TODO lists.

The user's knowledge base is the BOK at `/home/yermom/git/bok-of-splorr/splorr` (read `Home.md` first). Relevant notes: `Tech/Odin wasm recipe.md` (build, frame loop, input, and the shared-framebuffer variant used by Murder Hobo / Fortune Hunter, which is the closest fit here), `Tech/Release standard.md`, `Tech/Shipping to itch.io.md`, `Tech/Urizen tileset.md`, `Gotchas.md`. Add new lessons to `Gotchas.md` there, not here. Sibling Odin ports to copy structure from: `~/git/tggd-20seconds`, `~/git/hamquest`, `~/git/FortuneHunter`.

## Commands (current VB.NET build)

.NET 8 SDK; no tests or linter exist. `TreatWarningsAsErrors` and `OptionStrict On` are set on the game project.

```bash
dotnet build src/src.sln
dotnet run --project src/TitsUp/TitsUUp.vbproj      # run from a build dir that has Content/, config.json
./shippit.sh                                         # LEGACY: publishes linux/windows/mac self-contained; see warning below
```

`shippit.sh` only publishes to `pub-*` (git-ignored); its `butler push` lines are commented out. It does not match the Release standard (see the BOK) and must be replaced by `tools/ship.sh` + a one-line wrapper. **Never `git push`, run `butler push` or any ship script unless the user explicitly says so**; if "push" is ambiguous, ask git or itch.

Planned Odin commands (from the BOK recipe; adapt once the layout exists):

```bash
odin build src-odin -target:js_wasm32 -out:build/web/game.wasm -o:speed   # -o:size for shipping
cp "$(odin root)/core/sys/wasm/js/odin.js" build/web/
python3 -m http.server -d build/web 8000     # wasm cannot load from file://
odin test <rules pkg> -define:ODIN_TEST_THREADS=1 -define:ODIN_TEST_NAMES=pkg.test_name   # single test; test files are #+build !js
```

Use `odin check -target:js_wasm32 -vet-shadowing -vet-unused` explicitly (`odin check` does not apply them). Seed the RNG in `main` (`rand.reset`) and bound every retry loop. Odin constant arrays cannot be indexed at runtime (declare with `:=`).

## Architecture of the VB.NET original

Strict layering via project references (`src/src.sln`):

`TitsUp` (exe, `Program.vb`, `Content/`, `config.json`) → `TU.Presentation` → `TU.Business` → `TU.Persistence` → `TU.Data`, plus the generic `AOS.UI` / `AOS.Presentation` / `SPLORR.Game` libraries. `FontMaker` is a side tool that turns `source.png` (a tile sheet) into the glyph JSON fonts.

- **TU.Data**: plain serialisable DTOs. `WorldData` holds lists of `MapData`, `CharacterData`, `ItemData`, `MessageData` plus the avatar's character id. Everything is addressed by integer id. This is the whole save format (JSON via System.Text.Json).
- **TU.Persistence**: `IWorld`/`ICell`/`ICharacter`/`IItem`/`IMap`/`IMessage` wrappers; each `*DataClient` is a view over a DTO inside `WorldData`. No game rules.
- **TU.Business**: the rules. `Model/` exposes `IWorldModel` (Map, Avatar, TargetCharacter, Messages, `IsTitsUp`, `HasWon`, Embark/Abandon/Save/Load); avatar behaviour is split into sub-models (Combat, Encumbrance, Equipment, Ground, Inventory, SelectedItem). Content is **data-driven by string-keyed descriptor modules**: `TerrainTypes`, `CharacterTypes`, `ItemTypes`, `EquipSlotTypes`, `RecipeTypes`, `MapTypes`, `StatisticTypes`, `VerbTypes` (each a `Friend Module` with a `descriptors` dictionary and a `ToXDescriptor` extension). `Initializers/` build a new world (maze map via `SPLORR.Game.Maze`, avatar, items). Randomness is `SPLORR.Game.RNG` (weighted-table generators over `System.Random`).
- **TU.Presentation**: a state machine. `GameController` registers every `GameState` (Navigation, Move*, ActionMenu, Ground, Inventory, Equipment, Take/Drop/Equip/Unequip, InteractCharacter, Message, Statistics, TitsUp = death, Won) on top of the boilerplate states in `AOS.UI` (Splash, MainMenu, Options, Volume, WindowSize, Load/Save with 5 slots, About, Confirm quit/abandon). States are given `(controller, setState, context)`; `TitsUpContext` supplies model, About text and save slots (`slotN.json`).
- **AOS.UI / AOS.Presentation**: render to a software `IPixelSink` display buffer (320x180 logical, scaled 3x by MonoGame), with a 16-colour `Hue` palette, bitmap `Font`s loaded from glyph JSON (`Content/CyFont*.json`; `TitsUp.json` is the **tileset stored as glyphs** of 12x12 pixels, indexed by `ChrW(n)`, derived from vurmux's Urizen 1-bit tileset, CC0), and input mapped from `Content/keys.json` to abstract `Command`s (A, B, Up, Down, Left, Right). `Host.vb` is the only MonoGame-specific piece; sound is `Content/*.wav` via `BoilerplateSfx`.

The original is already a pure software framebuffer plus a command-driven state machine, so the port maps directly onto the BOK's shared-framebuffer variant: port Data+Business as a pure Odin rules package (RNG and clock passed in, natively testable), keep the state machine, draw the 320x180 buffer through one JS blit import, and replace file I/O (below).

## Port-specific traps

- **Saves and settings are files** (`slotN.json`, `config.json`): replace with `localStorage`, with **namespaced keys** (e.g. `tu:slot1`); every itch HTML5 game shares one origin. Window size / fullscreen options are meaningless on the web; drop or repurpose.
- Descriptor modules use `Dictionary` iteration order and weighted RNG: port insertion order faithfully, since draw order changes outcomes.
- Keep the credit to vurmux's Urizen tileset (shown in About) and `aboot.txt` content.

## Shipping (itch.io)

Page slug `tits-uup` under `thegrumpygamedev`. The page already exists with native (windows/linux/mac) builds from the 2023 VB releases; the browser build goes to a new `html` channel and does not replace them. Follow the BOK's Release standard: `ITCH_DESCRIPTION.md`, `assets/cover.png` (630x500; present), `assets/screenshots/` (currently in `ss/`), `devlog/YYYYMMDD/`, `tools/ship.sh` (tests, `-o:size` build, zip with `index.html` at root, push only with `--push`) and a one-line `shippit.sh` wrapper. The user sets the project kind, ticks "played in the browser", and pastes page copy by hand. State the AI-assisted port honestly on the page.
