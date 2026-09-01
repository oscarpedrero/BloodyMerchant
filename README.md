# BloodyMerchant — revived for V Rising 1.1

**This is a community fix, not a new mod.** All the design and original code belong to
[oscarpedrero](https://github.com/oscarpedrero/BloodyMerchant) and Trodi. I only got it
running again on the current game version.

Original project: https://github.com/oscarpedrero/BloodyMerchant

---

## What this is

BloodyMerchant lets server admins place custom NPC traders anywhere in the world and
decide exactly what they sell and what they charge for it.

It stopped working when V Rising went to 1.1. This fork gets it working again.

**Tested on:** `VRisingServer v1.1.13.0-r99712-b17`, BepInEx `6.0.0-be.733`,
VampireCommandFramework `0.11.0`, BloodyCore `2.0.2`.

---

## Read this first — Bloodstone

The original build depended on **Bloodstone**, which is abandoned and now actively
harmful on 1.1. It loads without complaint and then throws `MissingMethodException`
on network traffic, which breaks unrelated things across your whole server. It is
genuinely hard to diagnose, because the mod that breaks is rarely the one you're
looking at.

**This build does not use Bloodstone at all. Remove it.**

---

## What was broken, and what fixed it

Five separate problems. Four were code that no longer compiled against 1.1. The fifth
only showed up in-game.

### 1. Bloodstone dependency

Removed entirely. Bloodstone's only real job here was handing the mod the ECS `World`
and firing an "the game is ready" callback. BloodyCore 2.0.2 already does both.

- Added a small `Compat/VWorld.cs` that finds the server world directly
- Dropped `[BepInDependency("gg.deca.Bloodstone")]`, `IRunOnInitialized`, and `OnGameInitialized()`
- The world now arrives through BloodyCore's `EventsHandlerSystem.OnInitialize`
- Added `[BepInProcess("VRisingServer.exe")]` so it can't try to load on a client

### 2. `PrefabCollectionSystem.PrefabGuidToNameDictionary` was deleted

```
error CS1061: 'PrefabCollectionSystem' does not contain a definition for 'PrefabGuidToNameDictionary'
```

1.1 replaced it with `_PrefabDataLookup`, which maps `PrefabGUID → PrefabData`, and the
readable name now lives at `.AssetName.Value`. `LookupName()` in `ECSExtensions.cs` was
rewritten against the new lookup.

### 3. `SpawnUnitWithCallback` became ambiguous

```
error CS0121: The call is ambiguous between
  SpawnSystem.SpawnUnitWithCallback(Entity, PrefabGUID, float2, float, Action<Entity>)
  SpawnSystem.SpawnUnitWithCallback(Entity, PrefabGUID, float3, float, Action<Entity>)
```

1.1 added a `float3` overload. The original code used a target-typed `new(pos.x, pos.z)`,
which the compiler could no longer resolve. Made explicit: `new float2(pos.x, pos.z)`.

### 4. `Prefabs.Buff_BloodQuality_T01_OLD` no longer exists

1.1 removed every `_OLD` prefab. That buff was only ever a carrier for the
`BuffModificationTypes.MovementImpair` flag that keeps a merchant standing still —
the buff itself did nothing visible. Swapped for
`Buff_ChurchOfLight_Paladin_ImmaterialHomePos`, a live sibling of the buff
`MakeNPCImmortal` already uses, so the two stay independent.

### 5. A bad item ID silently killed the entire shop

This one only appeared in-game, and it's the reason this fork also ships a real
improvement rather than just a port.

**Symptom:** merchant spawns fine, stands still, talks, can be killed and cleaned up —
but his shop window is completely empty. Server log is spotless. Client log fills with:

```
[Error : Unity]  -- [TRADER] - Trader Sync Error!
```

hundreds of times a second.

**Cause:** one of the item IDs in `merchants.json` was not a real prefab in this version
of the game. V Rising's own `TraderSyncSystem` refuses to send *any* of the trader's
stock to the client if it can't resolve *one* item — so a single bad ID empties the
whole shop, with nothing server-side to explain why.

**Fix:** every trade row is now validated before it's written. An unresolvable item is
skipped with a plain-English warning naming the exact ID:

```
Merchant 'MyShop': item -257358505 is not a real item in this version of V Rising.
Skipping it - leaving it in would make the whole shop fail to load.
```

One bad row no longer takes the shop down with it.

---

## How this was actually found

Worth writing down, because the first four fixes were straightforward and this one
was not.

1. **Made it compile.** Removed Bloodstone, fixed the three API breaks. Four errors, four fixes.
   Build clean, mod loaded, `GameDataOnInitialize` fired. Looked done.

2. **Tested in-game.** Merchant spawned, stayed put, could be killed. Shop was empty.

3. **Checked the server log.** Nothing. No errors, no warnings, no exceptions. Dead end.

4. **Checked the *client* log** — and there it was: `[TRADER] - Trader Sync Error!`,
   over and over, starting the instant the merchant spawned. The failure was happening
   on the game's side, not the mod's, which is why the server had nothing to say.

5. **Resisted guessing.** Two earlier guesses in this project had already cost a round trip
   each. Instead: built a temporary diagnostic version that logged everything the mod does
   to the merchant entity — which components it has, what the game's own prefab shipped
   with, every trade row written with item names resolved, buffer lengths before and
   after, index range checks, inventory state.

6. **Ran it once.** The dump was ~60 lines. Everything was green — real trader prefab,
   all three buffers present, lengths matched, indexes in range — except one line:

   ```
   output  -257358505 x1 -> GUID Not Found
   cost    -257494203 x1 -> Item_Ingredient_Crystal
   ```

   The currency resolved. The item didn't. It was a bad ID in the test data all along.

7. **Verified.** Rebuilt with a known-good ID. Leather appeared in the shop, purchase
   went through, `Trader Sync Error` count in the log: **0**.

8. **Turned the diagnostic into a feature.** The temporary logging came back out, but the
   ID validation stayed in — because the trap that cost hours here will catch anyone
   who ever typos an item ID.

The lesson worth keeping: *when the server log is clean and the behaviour is still wrong,
read the client log.* And when you don't know, instrument it — don't guess.

---

## Installation

**Server-side only.** Players do not install anything.

1. [BepInEx for V Rising](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/)
2. [VampireCommandFramework](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/) 0.11.0+
3. [BloodyCore](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/) 2.0.2+ — **Trodi's, unmodified, install from source**
4. Drop `BloodyMerchant.dll` into `BepInEx/plugins/`
5. **Remove Bloodstone if you have it**

Optional: [Bloody.Wallet](https://thunderstore.io/c/v-rising/p/Trodi/BloodyWallet/) for currency support. Soft dependency — works fine without it.

---

## Commands

All admin-only. Command group is `.bm`.

| Command | What it does |
|---|---|
| `.bm list` | List every merchant you've created |
| `.bm create <name> [prefabID] [immortal] [canMove] [autorespawn]` | Create a merchant |
| `.bm spawn <name>` | Place him in the world at your feet |
| `.bm kill <name>` | Remove him from the world (keeps his config) |
| `.bm remove <name>` | Delete him permanently |
| `.bm cleanicons` | Clear orphaned map icons |
| `.bm product add <merchant> <itemID> <currencyID> <stack> <price> <stock>` | Add something to sell |
| `.bm product remove <merchant> <itemID> <currencyID>` | Remove a trade |
| `.bm product list <merchant>` | List a merchant's trades |
| `.bm product clean <merchant>` | Clear all trades |

### create flags, in order

```
.bm create Shop1 -1810631919 true false true
                  |          |    |     |
                  prefab     |    |     autorespawn
                             |    canMove  ← false means he stays put
                             immortal
```

Leave them off and you get the defaults: mortal, free to wander. If your merchant
walks away, that's why.

### create and spawn are not the same thing

This trips people up:

- **create** writes him to `merchants.json`. Permanent. Do it once.
- **spawn** puts him in the world
- **kill** takes him out of the world — he still exists in the file
- **remove** deletes him for good

"Merchant already exists" isn't an error to fix. It means he's already saved. Just spawn him.

---

## Verified item IDs

Pulled live from the game on 1.1.13. Use these to test — a bad ID is the single most
common reason a shop shows up empty.

| Item | ID |
|---|---|
| Leather | `-1907572080` |
| Coarse Thread | `-1562867444` |
| Gravedust | `-608131642` |
| Whetstone | `1252507075` |
| Crystal | `-257494203` |
| Straw Hat | `1375804543` |
| Rusted Helmet | `1364460757` |
| Necromancer Mitre | `607559019` |
| Woodcutter Axe | `1541522788` |
| Miner's Mace | `-687294429` |
| Twilight Snapper | `-570287766` |
| Fierce Stinger | `447901086` |

Working example — sells Leather for 1 Crystal, 10 in stock:

```
.bm create Shop1 -1810631919 true false true
.bm product add Shop1 -1907572080 -257494203 1 1 10
.bm spawn Shop1
```

---

## Credits

- **[oscarpedrero](https://github.com/oscarpedrero)** — created BloodyMerchant. All the design is his.
- **[Trodi](https://thunderstore.io/c/v-rising/p/Trodi/)** — BloodyCore, which this depends on and which is
  already maintained for 1.1. None of Trodi's code was modified.
- **[deca](https://github.com/decaprime)** — VampireCommandFramework
- **BepInEx team** — the loader everything sits on
- **The V Rising modding community** — for keeping the knowledge around after mods go quiet

If oscarpedrero or Trodi want these changes upstream, they're welcome to them — no
attribution needed, no PR required. Take the diff and run.

---

## Licensing

The original project's license applies. This fork changes nothing about that. It exists
so people can keep using the mod on 1.1, and it goes away happily the moment an official
update lands.

---

## Support the fix

Keeping abandoned mods alive is unpaid work. If this saved your server a headache and
you'd like to throw something my way:

**Cash App: `$Fartonice1081`**

Entirely optional. The mod is free and always will be.

*— Fartonice*
