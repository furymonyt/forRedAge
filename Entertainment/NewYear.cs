using GTANetworkAPI;
using System;
using AlyxSDK;
using Alyx.Core;

namespace Alyx.Entertainment
{
    class NewYear : Script
    {
        public static bool ActiveXMAS = false;
        private static nLog Log = new nLog("New Year");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                if (!ActiveXMAS) return;
                NAPI.Blip.CreateBlip(304, new Vector3(200.02164, -933.28644, 29.56127), 1f, 5, Main.StringToU16("Новогодняя Ёлка"), 255, 0, true, 0, 0);

                ColShape elka = NAPI.ColShape.CreateSphereColShape(new Vector3(200.02164, -933.28644, 29.56127), 8f);
                elka.OnEntityEnterColShape += (s, player) =>
                {
                    try
                    {
                        player.SetData("INTERACTIONCHECK", 159);
                    }
                    catch (Exception e) { Log.Write("EXCEPTION AT \"NEWYEAR\":\n" + e.ToString(), nLog.Type.Error); }
                };
                elka.OnEntityExitColShape += (s, player) =>
                {
                    try
                    {
                        player.SetData("INTERACTIONCHECK", 0);
                    }
                    catch (Exception e) { Log.Write("EXCEPTION AT \"NEWYEAR\":\n" + e.ToString(), nLog.Type.Error); }
                };

                Log.Write("XMAS TREE LOADED", nLog.Type.Success);
            } 
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"NEWYEAR\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        public static void TakeGiftINTree(Player player)
        {
            if (Main.Players[player].Cooldown > DateTime.Now.AddHours(-1))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Приходите за подарком позже", 3000);
                return;
            }
            var tryAdd = nInventory.TryAdd(player, new nItem(ItemType.Present));
            if (tryAdd == -1 || tryAdd > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                return;
            }
            Main.Players[player].Cooldown = DateTime.Now;
            nInventory.Add(player, new nItem(ItemType.Present));
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы получили подарок, приходите еще раз через час", 3000);
            MySQL.Query($"UPDATE `characters` SET `cooldown`='{MySQL.ConvertTime(DateTime.Now)}' WHERE `uuid`='{Main.Players[player].UUID}'");
        }
    }
}
