using RealLife.Core;
using RealLife.SDK;
using GTANetworkAPI;
using System;
using System.Collections.Generic;

namespace RealLife.MoneySystem
{
    class Bitcoin : Script
    {
        public static int PriceForBuyBitcoin;
        public static int PriceForSellBitcoin;
        private static nLog RLog = new nLog("Bitcoin");
        private static Random rnd = new Random();
        [ServerEvent(Event.ResourceStart)]
        public static void ResourceStart()
        {
            try
            {
                #region #Creating Marker && Shape && Blip
                foreach (Vector3 pos in BitcoinMag)
                {
                    NAPI.Blip.CreateBlip(683, pos, 0.8f, 25, "Покупка Bitcoin", 255, 0, true, 0, 0);
                    NAPI.Marker.CreateMarker(1, pos, new Vector3(), new Vector3(), 0.7f, new Color(255, 225, 64), false, 0);
                    ColShape shape = NAPI.ColShape.CreateCylinderColShape(pos, 1.5f, 2.5f, 0);
                    shape.OnEntityEnterColShape += (s, ent) =>
                    {
                        try
                        {
                            NAPI.Data.SetEntityData(ent, "INTERACTIONCHECK", 1592);
                        }
                        catch (Exception ex) { RLog.Write("shape.OnEntityEnterColShape: " + ex.ToString(), nLog.Type.Error); }
                    };
                    shape.OnEntityExitColShape += (s, ent) =>
                    {
                        try
                        {
                            NAPI.Data.SetEntityData(ent, "INTERACTIONCHECK", 0);
                        }
                        catch (Exception ex) { RLog.Write("shape.OnEntityExitColShape: " + ex.ToString(), nLog.Type.Error); }
                    };
                    RLog.Write($"Успешно загружено {BitcoinMag.Count} магазина.", nLog.Type.Success);
                }
                PriceForBuyBitcoin = rnd.Next(0, 300);
                PriceForSellBitcoin = rnd.Next(0, 300);
                RLog.Write($"Цена на закупку биткоина - {PriceForBuyBitcoin}. Цена на продажу биткоина - {PriceForSellBitcoin}.", nLog.Type.Success);
                Timers.Start(30000, () => GeneratePrice());
                #endregion
            }
            catch (Exception e) { RLog.Write("ResourceStart:" + e.ToString(), nLog.Type.Error); }
        }
        #region Colshape pos
        private static List<Vector3> BitcoinMag = new List<Vector3>
        {
            new Vector3(241.77258, 359.99728, 104.49254),
            new Vector3(),
            new Vector3(),
        };
        #endregion
        private static void GeneratePrice()
        {
            try
            {
                PriceForBuyBitcoin = rnd.Next(0, 300);
                PriceForSellBitcoin = rnd.Next(0, 300);
                RLog.Write($"Обновлены цены на биткоин!!!", nLog.Type.Success);
                RLog.Write($"Цена на закупку биткоина - {PriceForBuyBitcoin}. Цена на продажу биткоина - {PriceForSellBitcoin}.", nLog.Type.Success);
            }
            catch (Exception e) { RLog.Write("GeneratePrice:" + e.ToString(), nLog.Type.Error); }
        }
        public static void BuyBitcoin(Player player)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (Bank.Accounts[Main.Players[player].Bank].Balance < PriceForBuyBitcoin)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно средств на банк. счету", 3000);
                    return;
                }
                Bank.Change(Main.Players[player].Bank, -PriceForBuyBitcoin, false);
                Main.Players[player].Bitcoin += 1;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно купили 1 биткоин по цене {PriceForBuyBitcoin}.", 3000);
                MySQL.Query($"UPDATE characters SET bitcoin=bitcoin+1 WHERE uuid={Main.Players[player].UUID}");
                GameLog.Money($"Игрок - {Main.Players[player].UUID}", $"Server", PriceForBuyBitcoin, $"Купил 1 биткоин");
            }
            catch (Exception e) { RLog.Write("BuyBitcoin:" + e.ToString(), nLog.Type.Error); }
        }
        public static void SellBitcoin(Player player)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].Bitcoin <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно биткоинов", 3000);
                    return;
                }
                Bank.Change(Main.Players[player].Bank, +PriceForSellBitcoin, false);
                Main.Players[player].Bitcoin -= 1;
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно продали 1 биткоин по цене {PriceForSellBitcoin}.", 3000);
                MySQL.Query($"UPDATE characters SET bitcoin=bitcoin-1 WHERE uuid={Main.Players[player].UUID}");
                GameLog.Money($"Игрок - {Main.Players[player].UUID}", $"Server", PriceForSellBitcoin, $"Продал 1 биткоин");
            }
            catch (Exception e) { RLog.Write("SellBitcoin:" + e.ToString(), nLog.Type.Error); }
        }
    }
}