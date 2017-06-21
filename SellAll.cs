using System;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Rocket.API.Collections;
using SDG.Unturned;
using ZaupShop;

/**
 * SellAll RocketMod plugin
 * 
 * This plugin sells all player inventory items via the ZaupShop plugin,
 * which is also required.
 * 
 * author: Nexis <nexis@nexisrealms.com>
 * modified: 2017-20-06
 * 
 * *Note** a "page" simply refers to a location in storage where a player can 
 * store items. A weapon slot, their hands, a backpack, pants, shirts, etc.
 * 
 * "PAGE"
 * 0 = Primary Weapon
 * 1 = Secondary Weapon
 * 2 = Hands
 * 3 = Backpack
 * 4 = Vest
 * 5 = Shirt
 * 6 = Pants
 */
namespace NEXIS.SellAll
{
    public class SellAll : RocketPlugin<SellAllConfiguration>
    {
        public static SellAll Instance;

        protected override void Load()
        {
            Instance = this;
            Logger.Log("SellAll successfully loaded!", ConsoleColor.Green);
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"sellall_disabled", "Sorry, but the SellAll command is currently disabled!"},
                    {"sellall_done", "All inventory items sold!"}
                };
            }
        }

        /**
         * Sell a Page of Items
         * 
         * This function sells all player inventory items, by stepping through each
         * "page" of the inventory and sells each item. This is to ensure nothing is
         * missed or left behind.
         * @param {UnturnedPlayer} player Player data
         * @param {byte} page Page of items
         */
        public void Sell(UnturnedPlayer player, byte page)
        {
            foreach (var i in player.Inventory.items)
            {
                if (i == null) continue;
                for (byte w = 0; w < i.width; w++)
                {
                    for (byte h = 0; h < i.height; h++)
                    {
                        try
                        {
                            byte index = i.getIndex(w, h);
                            if (index == 255) continue;

                            ItemJar itm = player.Inventory.getItem(i.page, index);

                            // filter items to only those that match page
                            if (i.page == Convert.ToByte(page))
                            {
                                // sell item via ZaupShop plugin
                                ZaupShop.ZaupShop.Instance.Sell(player, new[] { itm.item.id.ToString() });

                                /**
                                 * DEV NOTE
                                 * there's no way to tell if ZaupShop succeeded in selling the item, so
                                 * even if it doesn't pay out, this next line will still delete the item 
                                 * anyway. "whoops"...
                                 */
                                i.removeItem(index);
                            }
                            
                            
                        }
                        catch { }
                    }
                }
            }
        }

        /**
         * Sell All Player Items
         * 
         * This function sells all player inventory items, by stepping through each
         * "page" of the inventory and sells each item. This is to ensure nothing is
         * missed or left behind.
         * @param {UnturnedPlayer} player Player data
         */
        public void SellAllItems(UnturnedPlayer player)
        {
            // sell everything in the player's hands first
            Sell(player, 2);

            // sell hats, glasses, and masks; if enabled
            if (SellAll.Instance.Configuration.Instance.SellPlayerClothing)
            {
                player.Player.clothing.askWearHat(0, 0, new byte[0], true);
                Sell(player, 2);
                
                player.Player.clothing.askWearGlasses(0, 0, new byte[0], true);
                Sell(player, 2);

                player.Player.clothing.askWearMask(0, 0, new byte[0], true);
                Sell(player, 2);
            }

            /* Pants */
            Sell(player, 6); // sell all items in pants

            if (SellAll.Instance.Configuration.Instance.SellPlayerClothing)
            {
                // remove player pants; placing them in hands
                player.Player.clothing.askWearPants(0, 0, new byte[0], true);
                Sell(player, 2); // sell pants in hands
            }

            /* Shirt */
            Sell(player, 5); // sell all items in shirt

            if (SellAll.Instance.Configuration.Instance.SellPlayerClothing)
            {
                // remove player shirt; placing them in hands
                player.Player.clothing.askWearShirt(0, 0, new byte[0], true);
                Sell(player, 2); // sell shirt in hands
            }

            /* Vest */
            Sell(player, 4); // sell all items in vest

            if (SellAll.Instance.Configuration.Instance.SellPlayerClothing)
            {
                // remove player vest; placing them in hands
                player.Player.clothing.askWearVest(0, 0, new byte[0], true);
                Sell(player, 2); // sell vest in hands
            }

            /* Backpack */
            Sell(player, 3); // sell all items in backpack

            if (SellAll.Instance.Configuration.Instance.SellPlayerClothing)
            {
                // remove player backpack; placing them in hands
                player.Player.clothing.askWearBackpack(0, 0, new byte[0], true);
                Sell(player, 2); // sell backpack in hands
            }

            /* Secondary Weapon */
            if (SellAll.Instance.Configuration.Instance.SellPlayerWeapons)
            {
                player.Player.equipment.tryEquip(1, 0, 0);
                Sell(player, 1);
            }

            /* Primary Weapon */
            if (SellAll.Instance.Configuration.Instance.SellPlayerWeapons)
            {
                player.Player.equipment.tryEquip(0, 0, 0);
                Sell(player, 0);
            }

        }

    }
}