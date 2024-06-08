using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.Methods;
using Bloody.Core.Models.v1;
using BloodyMerchant.DB;
using BloodyWallet.API;
using ProjectM;
using Stunlock.Core;
using System;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace BloodyMerchant.Systems
{
    internal class VirtualBuySystem
    {

        private static bool withoutSpace = false;

        internal static void HandleOnPlayerBuffed(UserModel player, Entity buffEntity, PrefabGUID prefabGuid)
        {
            if (prefabGuid == Prefabs.AB_Interact_Trade)
            {
                var spellTarget = buffEntity.Read<SpellTarget>();
                var trader = spellTarget.Target._Entity;
                if (trader.Exists())
                {
                    try
                    {
                        if (!trader.Has<NameableInteractable>()) return;
                        // Buscamos el mercader que es a traves de NameableInteractable    
                        var nameableInteractable = trader.Read<NameableInteractable>();
                        var merchant = Database.Merchants.Where(x=> nameableInteractable.Name.Value == x.name).FirstOrDefault();
                        if (merchant != null)
                        {
                            if (InventoryUtilities.GetFreeSlotsCount(Plugin.SystemsCore.EntityManager, player.Character.Entity) == 0)
                            {
                                BuffSystem.Unbuff(player.Character.Entity, prefabGuid);
                                player.SendSystemMessage(FontColorChatSystem.Red("You must have inventory space to interact with this trader!"));
                                withoutSpace = true;
                                return;
                            }
                            withoutSpace = false;
                            if (WalletAPI.GetTotalTokensForUser(player.CharacterName, out int points))
                            {
                                if (player.TryGiveItem(new PrefabGUID(WalletAPI.GetPrefabGUID()), points, out var itemENtity))
                                {
                                }
                            }
                        }
                    } catch (Exception e) {
                        Plugin.Logger.LogError(e.Message);
                    }
                    
                    
                }
            }
        }

        internal static void HandleOnPlayerBuffRemoved(UserModel player, Entity buffEntity, PrefabGUID prefabGuid)
        {
            if (prefabGuid == Prefabs.AB_Interact_Trade)
            {
                var spellTarget = buffEntity.Read<SpellTarget>();
                var trader = spellTarget.Target._Entity;
                if (trader.Exists())
                {
                    try
                    {
                        // Buscamos el mercader que es a traves de NameableInteractable
                        if (!trader.Has<NameableInteractable>() || withoutSpace) return;
                        var nameableInteractable = trader.Read<NameableInteractable>();
                        var merchant = Database.Merchants.Where(x => nameableInteractable.Name.Value == new FixedString64Bytes(x.name).Value).FirstOrDefault();
                        if (merchant != null)
                        {
                            if (WalletAPI.GetTotalTokensForUser(player.CharacterName, out int points))
                            {
                                if (searchTotalPrefabsInInventory(player, new PrefabGUID(WalletAPI.GetPrefabGUID()), out int totalPoints))
                                {
                                    var totalRemove = (points - totalPoints);
                                    if (getPrefabFromInventory(player, new PrefabGUID(WalletAPI.GetPrefabGUID()), points - totalRemove))
                                    {
                                        if (WalletAPI.RemoveToken(totalRemove, "BloodyMerchant", player.Entity, player.Entity, out string message))
                                        {
                                            //player.SendSystemMessage(message);
                                        }
                                        else
                                        {
                                            player.SendSystemMessage(FontColorChatSystem.Red($"{message}"));
                                        }
                                    } else
                                    {
                                        player.SendSystemMessage(FontColorChatSystem.Red($"Error when trying to extract items from inventory"));
                                    }
                                }
                                else
                                {
                                    if (WalletAPI.RemoveToken(points, "BloodyMerchant", player.Entity, player.Entity, out string message))
                                    {
                                        player.SendSystemMessage(message);
                                    } else
                                    {
                                        player.SendSystemMessage(FontColorChatSystem.Red($"{message}"));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                        Plugin.Logger.LogError(e.Message);
                    }
                }
            }
        }

        internal static void MakeSpecialCurrenciesSoulbound()
        {
            var prefabEntity = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(WalletAPI.GetPrefabGUID())];
            var itemData = prefabEntity.Read<ItemData>();
            itemData.MaxAmount = 4000;
            itemData.ItemCategory |= ItemCategory.SoulBound;
            prefabEntity.Write(itemData);
            var map = Plugin.SystemsCore.GameDataSystem.ItemHashLookupMap;
            map[new PrefabGUID(WalletAPI.GetPrefabGUID())] = itemData;
        }

        public static bool searchTotalPrefabsInInventory(UserModel player, PrefabGUID prefabCurrencyGUID, out int total)
        {
            total = 0;
            try
            {
                var characterEntity = player.Character.Entity;
                total = InventoryUtilities.GetItemAmount(Plugin.SystemsCore.EntityManager, characterEntity, prefabCurrencyGUID);
                if (total >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }
        }

        public static bool getPrefabFromInventory(UserModel player, PrefabGUID prefabGUID, int quantity)
        {

            try
            {

                var prefabGameData = GameData.Items.GetPrefabById(prefabGUID);
                var userEntity = player.Character.Entity;

                if(InventoryUtilitiesServer.TryRemoveItem(Plugin.SystemsCore.EntityManager, userEntity, prefabGameData.PrefabGUID, quantity))
                {
                    return true;
                } else
                {
                    return false;
                }

            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error {error.Message}");
                return false;
            }
        }

    }
}
