# BloodyMerchant

**BloodyMerchant** is a mod designed for V Rising, offering the capability to create custom in-game merchants, adding a layer of dynamic and immersive gameplay ( Vrising 1.0 ).

## IMPORTANT NOTE

## For the mod to work correctly you need Bloody.Core version 1.2.4 or higher. This is very important because otherwise the mod will not work. Updating this library does not break any of your other mods.

## NEW IN 1.0.8

- Now you can use [BloodyWallet](https://thunderstore.io/c/v-rising/p/Trodi/BloodyWallet/) to buy at the merchant with virtual currencies. Check the configuration file to activate this option.

# Instructions for BloodyWallet

1.- Edit the file ` config/trodi.Bloody.Wallet.cfg` and configure the PrefabGUID you want to use to purchase from the merchant in the `prefabGUIDExchange` option. 
2.- Edit the file ` config/BloodyMerchant.cfg` and set the value true to the `enabled` option.


The Prefab that you configured in Bloody.Wallet will appear in your inventory immediately after opening the merchant's store, if you do not have enough space it will not let you open a purchasing window.

**This Prefab cannot be thrown to the ground or picked up from the ground so you must take into account configuring a Prefab that is not used in the game such as [Crystals](https://vrising.gaming.tools/items /item_ingredient_crystal) or [Witchdust](https://vrising.gaming.tools/items/item_ingredient_witchdust) to avoid problems**

You can configure the merchant with other purchase items other than the BloodyWallet Prefab. It still works in the same way, but the ideal is that you change all of them to the BloodyWallet Prefab, so any object from your merchants can be purchased with the virtual currency.


<details>
<summary>Changelog</summary>

`1.0.8`
- Updated the timer system through Coroutine that brings the new version of Bloody.Core
- Now you can use BloodyWallet to buy at the merchant with virtual currencies.

`1.0.7`
- Added cleanicons command. This command will delete all the icons on the map, if you have any active merchant it will also delete their icon. 
- Added control of the number of items that can be added to a merchant. The limit is 33, after that number the client UI breaks.
- Fixed the error that caused the merchant in some places to not display the items correctly.
- System improvement and small fixes.

`1.0.6`
- Added a command to clear all items from a merchant

`1.0.5`
- Add or remove items from merchant in real time

`1.0.4`
- Bloody.Core dependency removed as dll and added as framework

`1.0.3`
- Refactored the Patch system
- Added Bloody.Core.
- Added restriction to the create command so that only valid merchant PrefabsGUIDs can be added
- Improved performance.

`1.0.0`
- Fix with Autorespawn
- Updated to a VRising 1.0 
- Added the functionality to show the icon on the merchant map

`0.2.0`
- Fix Autorefill
- Fixed a problem with products that did not work correctly

`0.1.0`
- Fix Autorefill by [@Backxtar](https://github.com/Backxtar)

`0.0.1`
- Initial public release of the mod
</details>

# Support this project

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/K3K8ENRQY)

## Mod Features
BloodyMerchant is a key component of the Blood Mod Pack. This plugin empowers you to craft personalized in-game traders, akin to BloodyShop. However, it leverages in-game traders, providing a more dynamic and immersive experience.

# Requirements

1. [BepInEx](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/)
2. [Bloodstone](https://thunderstore.io/c/v-rising/p/deca/Bloodstone/)
3. [VampireCommandFramework](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/)
4. [Bloody.Core](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/)

## Installation
1. Copy `BloodyMerchant.dll` to your `BepInEx/Plugins` directory.
2. Launch the server to create the config file; all configurations can be done in real-time in-game.

### Important note:
The system is not in real time, that is, first we create the merchant, we add products to it and we spawn.
If at any time we add or remove any product from the merchant we must kill it and spawn again!

## Merchant PrefabGUIDs

Only merchant PrefabGUIDs are supported.

```ansi
  "CHAR_Trader_Dunley_Gems_T02": 194933933,
  "CHAR_Trader_Dunley_Herbs_T02": 233171451,
  "CHAR_Trader_Dunley_Knowledge_T02": 281572043,
  "CHAR_Trader_Dunley_RareGoods_T02": -1594911649,
  "CHAR_Trader_Farbane_Gems_T01": -1168705805,
  "CHAR_Trader_Farbane_Herbs_T01": -375258845,
  "CHAR_Trader_Farbane_Knowledge_T01": -208499374,
  "CHAR_Trader_Farbane_RareGoods_T01": -1810631919,
  "CHAR_Trader_Gloomrot_T04": -1292194494,
  "CHAR_Trader_Noctem_Major": 1631713257,
  "CHAR_Trader_Noctem_Minor": 345283594,
  "CHAR_Trader_Silverlight_Gems_T03": -1990875761,
  "CHAR_Trader_Silverlight_Herbs_T03": 1687896942,
  "CHAR_Trader_Silverlight_Knowledge_T03": -915182578,
  "CHAR_Trader_Silverlight_RareGoods_T03": 739223277
```

## Commands


```ansi
.bm list
```
- Lists all available merchants on the server.
```ansi
.bm create <NameOfMerchant> [PrefabGUIDOfMerchant] [Immortal] [Move] [Autorespawn]
```
- Creates a custom merchant and adds it to the merchant's list.
  - **NameOfMerchant**: Unique identifier for the merchant.
  - **PrefabGUIDIfMerchant**: GUID for the merchant NPC to spawn.
  - **Immortal (True/False)**: Makes the merchant immortal and impervious to damage.
  - **Move (True/False)**: Enables or disables the merchant's movement.
  - **Auto respawn (True/False)**: Respawns the merchant when the server is back online.
  - Example: `.bm create test -208499374 true false true`
```ansi
.bm remove <NameOfMerchant>
```
- Removes the merchant from the list (requires killing the merchant while alive).
  - Example: `.bm remove test`
```ansi
.bm spawn <NameOfMerchant>
```
- Spawns your custom merchant.
  - Example: `.bm spawn test`
```ansi
.bm kill <NameOfMerchant>
```
- Kills the desired merchant.
  - Example: `.bm kill test`
```ansi
.bm cleanicon
```
- This command will delete all the icons on the map, if you have any active merchant it will also delete their icon. 
  - Example: `.bm cleanicon`
```ansi
.bm product add <NameOfMerchant> <ItemPrefabID> <CurrencyfabID> <Stack> <Price> <Stock> [Autorefill true/false]
```
- Adds products to the merchant in real-time.
  - **NameOfMerchat**: Unique merchant name set previously.
  - **ItemPrefabID**: Product item ID for the merchant to sell.
  - **CurrencyfabID**: ID of the item used as currency to buy the product.
  - **Stack**: Number of products received by the player for that item when purchased.
  - **Price**: Amount of currency item players need to purchase the item.
  - **Stock**: Availability of the item with the merchant (limited or unlimited, max is 99).
  - **Autorefill (True/False)**: Allows players to buy the item infinitely.
  - Example: `.bm product add test 1557814269 -77477508 1 1 99 true`
```ansi
.bm product remove <NameOfMerchant> <ItemPrefabID>
```
- Removes a product from the merchant in real-time.
  - Example: `.bm product remove test 1557814269`
```ansi
.bm product clean <NameOfMerchant>
```
- Removes a product from the merchant in real-time.
  - Example: `.bm product clean test`
```ansi
.bm product list <NameOfMerchant>
```
- Lists all products currently available for sale by a certain merchant.
  - Example:  `.bm product list test`
```ansi
.bm config show <NameOfMerchant>
```
- Shows the Immortal, Move, and Autospawn configuration for a certain merchant.
  - Example: `.bm config show test`
```ansi
.bm config immortal <NameOfMerchant> <true/false>
```
- Changes the immortal configuration for a certain merchant in real-time.
  - Example: `.bm config immortal test true`
```ansi
.bm config move <NameOfMerchant> <true/false>
```
- Changes the move configuration for a certain merchant in real-time.
  - Example: `.bm config move test true`
```ansi
.bm config autorespawn <NameOfMerchant> <true/false>
```
- Changes the auto-spawn configuration for a certain merchant in real-time.
  - Example: `.bm config autorespawn test true`

# Resourcess

[Complete items list of prefabs/GUID](https://discord.com/channels/978094827830915092/1117273637024714862/1117273642817044571)

# Credits

This mod idea was suggested by [@Vex](https://ideas.vrisingmods.com/posts/96/enhanced-traders) on our community idea tracker. Please vote and suggest your ideas [here](https://ideas.vrisingmods.com/).

[V Rising Mod Community](https://discord.gg/vrisingmods) is the best community of mods for V Rising.

[@Deca](https://github.com/decaprime), thank you for the exceptional frameworks [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework) and [BloodStone](https://github.com/decaprime/Bloodstone), based on [WetStone](https://github.com/molenzwiebel/Wetstone) by [@Molenzwiebel](https://github.com/molenzwiebel).

[@LecherousCthulhu](https://github.com/HasturDev) for sharing code on how to change the trader's inventory.

[@Willis](https://github.com/emelonakos) for being an amazing community modder, providing the initial code that helped bring this idea to life.

[@Backxtar](https://github.com/Backxtar) owner & founder of [Bloody Mary](https://discord.gg/sE2hqbxUU4) server, a talented modder who contributed by writing certain functions, debugging, and group efforts to make this mod work.

**Special thanks to the testers and supporters of the project:**

- @Vex, owner & founder of [Vexor RPG](https://discord.gg/JpVsKVvKNR) server, a tester and great supporter who provided his server as a test platform and took care of all the graphics and documentation.