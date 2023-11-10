# BloodyMerchant

**BloodyMerchant** is a mod designed for V Rising, offering the capability to create custom in-game merchants, adding a layer of dynamic and immersive gameplay.

![BloodyMerchant](https://github.com/oscarpedrero/BloodyMerchant/blob/master/Images/BloodyMerchant.png?raw=true)

<details>
<summary>Changelog</summary>

`0.1`
- Initial public release of the mod
</details>

## Mod Features
BloodyMerchant is a key component of the Blood Mod Pack. This plugin empowers you to craft personalized in-game traders, akin to BloodyShop. However, it leverages in-game traders, providing a more dynamic and immersive experience.

This server-side plugin mandates the following plugins for seamless operation:

1. [BepInEx](https://github.com/BepInEx/BepInEx)
2. [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework)
3. [Bloodstone](https://github.com/decaprime/Bloodstone)

## Installation
1. Copy `BloodyMerchant.dll` to your `BepInEx/Plugins` directory.
2. Launch the server to create the config file; all configurations can be done in real-time in-game.

## Commands
```ansi
.merchant list
```
- Lists all available merchants on the server.
```ansi
.merchant create <NameOfMerchant> [PrefabGUIDOfMerchant] [Immortal] [Move] [Autorespawn]
```
- Creates a custom merchant and adds it to the merchant's list.
  - **NameOfMerchant**: Unique identifier for the merchant.
  - **PrefabGUIDIfMerchant**: GUID for the merchant NPC to spawn.
  - **Immortal (True/False)**: Makes the merchant immortal and impervious to damage.
  - **Move (True/False)**: Enables or disables the merchant's movement.
  - **Auto respawn (True/False)**: Respawns the merchant when the server is back online.
  - Example: `.merchant create test -208499374 true false true`
```ansi
.merchant remove <NameOfMerchant>
```
- Removes the merchant from the list (requires killing the merchant while alive).
  - Example: `.merchant remove test`
```ansi
.merchant spawn <NameOfMerchant>
```
- Spawns your custom merchant.
  - Example: `.merchant spawn test`
```ansi
.merchant kill <NameOfMerchant>
```
- Kills the desired merchant.
  - Example: `.merchant kill test`
```ansi
.merchant product add <NameOfMerchant> <ItemPrefabID> <CurrencyfabID> <Stack> <Price> <Stock> [Autorefill true/false]
```
- Adds products to the merchant in real-time.
  - **NameOfMerchat**: Unique merchant name set previously.
  - **ItemPrefabID**: Product item ID for the merchant to sell.
  - **CurrencyfabID**: ID of the item used as currency to buy the product.
  - **Stack**: Number of products received by the player for that item when purchased.
  - **Price**: Amount of currency item players need to purchase the item.
  - **Stock**: Availability of the item with the merchant (limited or unlimited, max is 99).
  - **Autorefill (True/False)**: Allows players to buy the item infinitely.
  - Example: `.merchant product add test 1557814269 -77477508 1 1 99 true`
```ansi
.merchant product remove <NameOfMerchant> <ItemPrefabID>
```
- Removes a product from the merchant in real-time.
  - Example: `.merchant product remove test 1557814269`
```ansi
.merchant product list <NameOfMerchant>
```
- Lists all products currently available for sale by a certain merchant.
  - Example:  `.merchant product list test`
```ansi
.merchant config show <NameOfMerchant>
```
- Shows the Immortal, Move, and Autospawn configuration for a certain merchant.
  - Example: `.merchant config show test`
```ansi
.merchant config immortal <NameOfMerchant> <true/false>
```
- Changes the immortal configuration for a certain merchant in real-time.
  - Example: `.merchant config immortal test true`
```ansi
.merchant config move <NameOfMerchant> <true/false>
```
- Changes the move configuration for a certain merchant in real-time.
  - Example: `.merchant config move test true`
```ansi
.merchant config autorespawn <NameOfMerchant> <true/false>
```
- Changes the auto-spawn configuration for a certain merchant in real-time.
  - Example: `.merchant config autorespawn test true`

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

![Bloody](https://github.com/oscarpedrero/BloodyMerchant/blob/master/Images/Bloody.png?raw=true)
