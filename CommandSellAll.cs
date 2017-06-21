using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Core.Logging;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.SellAll
{
    public class CommandSellAll : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public bool AllowFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "sellall"; }
        }

        public string Help
        {
            get { return "Sells all of the items in your inventory for credits."; }
        }

        public string Syntax
        {
            get { return "/sellall"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "nexis.sellall" }; }
        }

        public void Execute(IRocketPlayer caller, params string[] param)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (SellAll.Instance.Configuration.Instance.SellAllEnabled)
            {
                // sell all player inventory items
                SellAll.Instance.SellAllItems(player);
                UnturnedChat.Say(player, SellAll.Instance.Translations.Instance.Translate("sellall_done"), Color.green);
            }
            else
            {
                // sellall is disabled in config
                UnturnedChat.Say(caller, SellAll.Instance.Translations.Instance.Translate("sellall_disabled"), Color.red);
            }
        }
    }
}