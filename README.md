# Skip Pet End Screen

A [BepInEx 5](https://github.com/BepInEx/BepInEx) plugin for Hearthstone that shows the **standard end-of-game screen** instead of the pet sequence when you have a pet equipped.

With a pet equipped, every game ends with the pet presentation: the board-scene view, the pet's win/loss reaction, a delayed banner, then pet XP and reward popups. This mod restores the classic ending — blurred board, victory/defeat banner, done — for wins, losses, and Battlegrounds alike. Your pet is completely unaffected during the game itself, and **pet XP and level rewards still accrue** server-side; only their end-screen popups are skipped.

Pairs naturally with [No-Pets](https://github.com/reqvamhs/No-Pets) (which hides opponent pets in-game); the two are independent — install either or both. Since this mod does exactly one thing, its toggle defaults to **on**: installing it is the opt-in.

Purely cosmetic and strictly local — only what *your* client renders changes.

> ⚠️ **Disclaimer:** All client-side mods technically violate Blizzard's Terms of Service and carry a ban risk. Use at your own risk. This project is not affiliated with or endorsed by Blizzard Entertainment.

## How it works

The entire pet ending hangs on two questions the game asks itself, and both have native "no pet" answers this plugin selects via [Harmony](https://github.com/BepInEx/HarmonyX) prefixes. `GameEntity.PetEndgameSpell` creates the whole pet presentation and returns null for petless players — forcing null is byte-identical to not having a pet, and covers victory and defeat in one place since the win/loss reaction is chosen inside that method. `GameState.HasPetEndgame` tells the end screen how to style itself (the standard blur/vignette is applied on the no-pet path) — forcing false restores the classic look. Two companion prefixes reuse the built-in "nothing to show" paths of the pet XP and reward popups, and a pet cutscene controller spawned for the end screen is kept from creating a pet object. Everything acts only in actual gameplay; collection pet previews are untouched.

## Installation

You need BepInEx 5 in your Hearthstone folder — via **either** route — then the mod DLL.

**Option A — via Firestone (easiest if you already use it):**
1. With Hearthstone closed, open Firestone → Settings → General → Mods and enable mods.
   This installs Firestone's integrated BepInEx into the game folder.
2. Launch Hearthstone once and quit, so `<GameDir>\BepInEx\plugins\` gets created.

**Option B — plain BepInEx:**
1. Download [BepInEx 5.4.x **x64**](https://github.com/BepInEx/BepInEx/releases) and extract
   the zip directly into your Hearthstone folder (next to `Hearthstone.exe`), so you end up
   with `<GameDir>\winhttp.dll` and `<GameDir>\BepInEx\`.
2. Launch Hearthstone once and quit.

**Then, for both routes:**
1. Download `HsSkipPetEndScreen.dll` from [Releases](../../releases) and drop it into
   `<GameDir>\BepInEx\plugins\`.
2. Launch Hearthstone. Verify by finding `Skip Pet End Screen ... loaded.` in
   `<GameDir>\BepInEx\LogOutput.log`, or by finishing a game with a pet equipped.

## Configuration (optional)

Edit `<GameDir>\BepInEx\config\HsSkipPetEndScreen.cfg` (created on first launch):

```ini
[Features]
## Show the standard end-of-game screen instead of the pet sequence
## (pet scene, XP and reward popups). The pet is unaffected during games.
SkipPetEndScreen = true
```

## Building from source

Requirements: a .NET SDK (8/9/10), BepInEx 5 installed in the game folder (the project references its DLLs from there).

1. Clone the repo.
2. Edit `<GameDir>` in `HsSkipPetEndScreen.csproj` to your Hearthstone install path.
3. `dotnet build -c Release` — the post-build step copies the DLL into `BepInEx\plugins` automatically.

No game files are included in this repository; the project compiles against `Assembly-CSharp.dll` from your own installation.

## Compatibility

- Built and verified against the July 2026 Hearthstone build.
- Hearthstone patches can rename or change the hooked methods (`GameEntity.PetEndgameSpell`, `GameState.HasPetEndgame`, the end screen's pet popup methods). If the mod stops working after a game update, check `LogOutput.log` for Harmony errors and watch this repo for an updated release.
- Coexists with other BepInEx/Firestone mods, including the author's other Hearthstone mods.

## Uninstall

Delete `HsSkipPetEndScreen.dll` from `BepInEx\plugins`. To remove BepInEx entirely, delete `winhttp.dll` from the game folder.

## License

[MIT](LICENSE)
