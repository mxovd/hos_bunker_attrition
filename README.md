# Bunker Attrition (Hex of Steel mod)

A small Harmony mod that makes bunkers weaken and eventually surrender when they sit on enemy-controlled territory. Intended to solve the "invincible bunkers left behind the front line" problem.

## What it does
- Each turn, any bunker standing on land owned by a player it is at war with loses HP (percent of max, with a minimum chip).
- When HP reaches zero the bunker is destroyed/removed.
- Only affects bunker units (detected via unit name contains "Bunker").

## Tuning
Adjust the constants in [Scripts/BunkerAttritionMod.cs](Scripts/BunkerAttritionMod.cs):
- `DamagePercentPerTurn` (default 0.20f)
- `MinimumDamagePerTurn` (default 4)

## Building
Use the helper script to build/package (and optionally install) the mod:

```
python hos_mod_utils.py -d        # build + package
python hos_mod_utils.py -di       # build + package + install to your Hex of Steel MODS folder
python hos_mod_utils.py -dri      # same, but also refresh Libraries from your game install first
```

Notes on flags:
- `-d` / `--deploy`: runs `dotnet build` (Release), stages the DLL + Manifest.json under `package/<version>/Bunker Attrition/`.
- `-i` / `--install`: after deploy, copies the staged folder into your detected Hex of Steel `MODS` directory.
- `-r` / `--refresh-libs`: copies fresh game DLLs into `Libraries/` before the build (skipped otherwise).

If you prefer manual IDE builds, target .NET Framework 4.x and drop the built DLL plus `Manifest.json` into a mod folder under the game’s `MODS` directory.

## Credits
- Mod by @mxovd. Built on Harmony and Hex of Steel mod API.
