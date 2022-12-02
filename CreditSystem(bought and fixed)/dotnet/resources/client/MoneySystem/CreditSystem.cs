using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.GUI;
using NeptuneEvo.Houses;
using Redage.SDK;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.MoneySystem
{
    class CreditSystem : Script
    {
        #region [logo]
        /*   __________         ______           __________         ______           _ ________      _     _      _    
            | |______  |       / ____ \         | |______  |       / ____ \         | |  ___   |    |_|   | |    / /
            | |      | |      / /    \ \        | |      | |      / /    \ \        | | |___|  |     _    | |   / /
            | |______| |     / /      \ \       | |______| |     / /      \ \       | |________|    | |   | |  / /
            | |________|    / /________\ \      | |________|    / /________\ \      | |  \ \        | |   | | / /                                                      
            | |            / /__________\ \     | |            / /__________\ \     | |   \ \       | |   | | \ \
            | |           / /            \ \    | |           / /            \ \    | |    \ \      | |   | |  \ \
            | |          / /              \ \   | |          / /              \ \   | |     \ \     | |   | |   \ \
            |_|         /_/                \_\  |_|         /_/                \_\  |_|      \_\    |_|   |_|    \_\
       */
        #endregion

        #region [Инфа о кредитной системе]
        private static nLog Log = new nLog("CreditSystem");
        private static Config config = new Config("Credit");
        private static float colRadius = config.TryGet<float>("sRadius", 2);
        private static float colshRadius = config.TryGet<float>("sRadius", 2);
        private static Vector3 colPosition = new Vector3(237.82747, -413.12378, 47.08484);
        private static Vector3 colshPosition = new Vector3(232.84325, -411.32956, 47.08484);
        private static Vector3 blipPosition = new Vector3(237.73126, -413.09866, 48.111946);
        private static string blipName = config.TryGet<string>("bName", "Кредит");
        private static byte blipColor = config.TryGet<byte>("bColor", 11);
        private static uint blipID = config.TryGet<uint>("bID", 207);
        private static ColShape colShape;
        private static ColShape colshShape;
        private static Blip blip;
        private static Dictionary<Player, (ushort, ushort, ushort)> Sum = new Dictionary<Player, (ushort, ushort, ushort)>();
        #endregion

        #region[onResourceStart]

        [ServerEvent(Event.ResourceStart)]
        public static void standOnColshape()
        {
            colShape = NAPI.ColShape.CreateCylinderColShape(colPosition, colRadius, 2, dimension: 0);
            colShape.OnEntityEnterColShape += (ColShape shape, Player client) =>
            {
                try
                {
                    NAPI.Data.SetEntityData(client, "INTERACTIONCHECK", 7123);
                }
                catch (Exception ex) { Console.WriteLine("shape: " + ex.Message); }
            };
            colShape.OnEntityExitColShape += (ColShape shape, Player client) =>
            {
                try
                {
                    NAPI.Data.SetEntityData(client, "INTERACTIONCHECK", 0);
                }
                catch (Exception ex) { Console.WriteLine("shape: " + ex.Message); }
            };

            colShape.Position = colPosition;
            colshShape = NAPI.ColShape.CreateCylinderColShape(colshPosition, colshRadius, 2, dimension: 0);
            colshShape.OnEntityEnterColShape += (ColShape shape, Player client) =>
            {
                try
                {
                    NAPI.Data.SetEntityData(client, "INTERACTIONCHECK", 1244);
                }
                catch (Exception ex) { Console.WriteLine("shape: " + ex.Message); }
            };

            colshShape.OnEntityExitColShape += (ColShape shape, Player client) =>
            {
                try
                {
                    NAPI.Data.SetEntityData(client, "INTERACTIONCHECK", 0);
                }
                catch (Exception ex) { Console.WriteLine("shape: " + ex.Message); }
            };
            colshShape.Position = colPosition;

            blip = NAPI.Blip.CreateBlip(blipID, blipPosition, 1, blipColor, blipName, 255, 0, shortRange: true, dimension: 0);
            NAPI.Marker.CreateMarker(1, colPosition, new Vector3(), new Vector3(), 1f, new Color(250, 250, 70));
            NAPI.Marker.CreateMarker(1, colshPosition, new Vector3(), new Vector3(), 1f, new Color(250, 250, 70));
            NAPI.TextLabel.CreateTextLabel("Оформить кредит", colPosition + new Vector3(0, 0, 1), 5F, 0.3F, 0, new Color(255, 255, 255));
            NAPI.TextLabel.CreateTextLabel("Пройти тест", colshPosition + new Vector3(0, 0, 1), 5F, 0.3F, 0, new Color(255, 255, 255));
            Log.Write("Credit loaded ", nLog.Type.Info);
        }
        #endregion

        #region [Открыть-закрыть меню]
        public static void OpenCreditMenu(Player player)
        {
            var acc = Main.Players[player];
            Trigger.ClientEvent(player, "openCreditMenu");
            Trigger.ClientEvent(player, "playerName", player.Name.ToString(), "");
            Trigger.ClientEvent(player, "debt", acc.CreditMoney.ToString(), "");
            Trigger.ClientEvent(player, "playerMoney", acc.Money.ToString(), "");
        }
        public static void CloseCreditMenu(Player player)
        {
            Trigger.ClientEvent(player, "closeCreditMenu");
        }
        public static void OpenCheckMenu(Player player)
        {
            Trigger.ClientEvent(player, "openCheckMenu");
            Trigger.ClientEvent(player, "playerName", player.Name.ToString(), "");
        }
        public static void CloseCheckMenu(Player player)
        {
            Trigger.ClientEvent(player, "closeCheckMenu");
        }
        #endregion

        #region [Уведомление при payday]
        public static void ReminderCredit(Player player)
        {
            var acc = Main.Players[player];
            if (acc.CreditMoney > 0)
            {
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Оплатите кредит! Ваша задолженность ({acc.CreditMoney}$)", 10000);
                return;
            }
        }
        #endregion

        #region [Bыдача денег и наоборот]
        [RemoteEvent("creditOperation")]
        public static void CreditOperation(Player player, params object[] args)
        {
            try
            {
                var acc = Main.Players[player];
                int operation = Convert.ToInt32(args[1]);
                int creditSum = Convert.ToInt32(args[0]);
                int sumProcent200k = (creditSum / 100) * 15;
                int sumProcent700k = (creditSum / 100) * 10;
                int lastSum200k = creditSum + sumProcent200k;
                int lastSum700k = creditSum + sumProcent700k;
                switch (operation)
                {
                    case 2://Выдача кредита и проверки
                        Trigger.ClientEvent(player, "closeCreditMenu");
                        if (creditSum <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите коректные данные.", 3000);
                            return;
                        }
                        if (acc.LVL < 4)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Оформить кредит можно только с 4 уровня.", 3000);
                            return;
                        }
                        if (creditSum >= 700001)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Максимальная сумма кредита 700.000$", 3000);
                            return;
                        }
                        if (acc.CreditMoney >= 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас уже есть неоплаченный кредит", 3000);
                            return;
                        }
                        if (creditSum <= 9999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Сумма кредита должна быть не менее 10.000$. Прочтите условия!", 3000);
                            return;
                        }
                        if (acc.AllowCredit == 1)
                        {
                            if (creditSum <= 200000)
                            {
                                if (Wallet.Change(player, +Math.Abs(creditSum)))
                                {
                                    Main.Players[player].CreditMoney += lastSum200k;
                                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вам выдан кредит в размере ({creditSum}$). Если вы не оплатите кредит в течении 14 дней, кредитное отделение заберёт у вас имущество!", 10000);
                                    Log.Write($"{player.Name} Взял кредит на сумму: ({lastSum200k})", nLog.Type.Success);
                                    Log.Write($"Сохранено. В строке creditmoney появилось число: {lastSum200k}", nLog.Type.Success);
                                    MySQL.Query($"UPDATE `characters` SET `creditmoney`= {Main.Players[player].CreditMoney}");
                                    MySQL.Query($"UPDATE `characters` SET `credittime`='{MySQL.ConvertTime(DateTime.Now.AddDays(14))}'");
                                    MySQL.Query($"UPDATE `characters` SET `allowcredit`='{acc.AllowCredit = 0}'");
                                }
                            }
                            if (creditSum >= 200001)
                            {
                                if (Wallet.Change(player, +Math.Abs(creditSum)))
                                {
                                    Main.Players[player].CreditMoney += lastSum700k;
                                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вам выдан кредит в размере ({creditSum}$). Если вы не оплатите кредит в течении 14 дней, кредитное отделение заберёт у вас имущество!", 10000);
                                    Log.Write($"{player.Name} Взял кредит на сумму: ({lastSum700k})", nLog.Type.Success);
                                    Log.Write($"Сохранено. У игока {player.Name} в столбе creditmoney появилось число: {lastSum700k}", nLog.Type.Success);
                                    MySQL.Query($"UPDATE `characters` SET `creditmoney`= {Main.Players[player].CreditMoney}");
                                    MySQL.Query($"UPDATE `characters` SET `credittime`='{MySQL.ConvertTime(DateTime.Now.AddDays(14))}'");
                                    MySQL.Query($"UPDATE `characters` SET `allowcredit`='{acc.AllowCredit = 0}'");
                                }
                            }
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нету разрешения на кредит. Пройдите тест рядом с отделением кредитов.", 3000);
                            return;
                        }
                        break;
                    case 3://Оплата кредита
                        Trigger.ClientEvent(player, "closeCreditMenu");
                        if (acc.CreditMoney < 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нету неоплаченных кредитов.", 3000);
                            return;
                        }
                        if (acc.LVL < 4)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нету неоплаченных кредитов.", 3000);
                            return;
                        }
                        if (creditSum > acc.CreditMoney)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете оплатить кредит на такую большую сумму. Максимальная сумма: {acc.CreditMoney}$", 3000);
                            return;
                        }
                        if (creditSum <= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите коректные данные.", 3000);
                            return;
                        }
                        if (Wallet.Change(player, -Math.Abs(creditSum)))
                        {
                            Main.Players[player].CreditMoney -= creditSum;
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы оплатили кредит на сумму: ({creditSum}$)", 3000);
                            Log.Write($"{player.Name} Оплатил кредит на сумму: {creditSum}", nLog.Type.Warn);
                            MySQL.Query($"UPDATE `characters` SET `creditmoney`= {Main.Players[player].CreditMoney}");
                            if (acc.CreditMoney > 0)
                            {
                                MySQL.Query($"UPDATE `characters` SET `credittime`='{MySQL.ConvertTime(DateTime.Now)}'");
                            }
                        }
                        else
                        {
                            Log.Write($"Ошибка при оплате кредита.", nLog.Type.Error);
                        }
                        break;
                }
            }
            catch (Exception e) { Log.Write("CreditOperation: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region [Проверка на имущество]
        [RemoteEvent("propertyCheck")]
        public static void PropertyCheck(Player player, params object[] args)
        {
            try
            {
                var acc = Main.Players[player];
                House house = HouseManager.GetHouse(player, true);
                int type = Convert.ToInt32(args[0]);
                var biz = Main.Players[player].BizIDs.Count;
                int property = acc.AllowCredit;
                switch (type)
                {
                    case 0:
                        Trigger.ClientEvent(player, "closeCheckMenu");
                        if (biz == 0 && house == null)
                        {
                            property = 0;
                            MySQL.Query($"UPDATE `characters` SET `allowcredit`='{property}'");
                        }
                        else
                        {
                            property = 1;
                            MySQL.Query($"UPDATE `characters` SET `allowcredit`='{property}'");
                        }
                        if (property == 1)//Разрешение на кредит
                        {
                            MySQL.Query($"UPDATE `characters` SET `allowcredit`='{acc.AllowCredit = 1}'");
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы прошли проверку, теперь можете взять кредит.", 4000);
                            return;
                        }
                        else//Запрет на кредит
                        {
                            property = 0;
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не прошли проверку, так как у вас нету имущества. Мы считаем, что вы можете обмануть кредитное отделение!", 4000);
                            return;
                        }
                }
            }
            catch (Exception e) { Log.Write("propertyCheck: " + e.Message, nLog.Type.Error); }
        }
        #endregion

        #region[Забираю имущество при неуплате кредита]

        public static void TakeMoney(Player player)//Забираю деньги, если есть
        {
            try
            {
                var acc = Main.Players[player];
                if (acc.CreditMoney > 0)
                {
                    if (acc.CreditTime <= DateTime.Now)
                    {
                        if (acc.Money >= acc.CreditMoney)
                        {
                            acc.Money = acc.Money - acc.CreditMoney;
                            acc.CreditMoney = 0;
                            MySQL.Query($"UPDATE `characters` SET `money`= {acc.Money}");
                            MySQL.Query($"UPDATE `characters` SET `creditmoney`= {acc.CreditMoney}");
                            MySQL.Query($"UPDATE `characters` SET `allowcredit`= {acc.AllowCredit = 0}");
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У вас забрали деньги за неуплату кредита.", 5000);
                            return;
                        }
                        else
                        {
                            TakeHouse(player);
                            return;
                        }
                    }
                }
            }
            catch (Exception e) { Log.Write("takeProperty: " + e.Message, nLog.Type.Error); }
        }
        public static void TakeHouse(Player player)//Забираю дом, если есть
        {
            try
            {
                var acc = Main.Players[player];
                House house = HouseManager.GetHouse(player, true);
                if (house == null)
                {
                    TakeBiz(player);
                    return;
                }
                else
                {
                    house.RemoveAllPlayers();
                    house.SetOwner(null);
                    house.PetName = "null";
                    house.Save();
                    acc.CreditMoney = 0;
                    MySQL.Query($"UPDATE `characters` SET `creditmoney`= {acc.CreditMoney}");
                    MySQL.Query($"UPDATE `characters` SET `allowcredit`= {acc.AllowCredit = 0}");
                    Trigger.ClientEvent(player, "deleteCheckpoint", 333);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У вас отобрали дом за неуплату кредита.", 3000);
                    Trigger.ClientEvent(player, "deleteGarageBlip");
                    return;
                }
            }
            catch (Exception e) { Log.Write($"takeProperty:" + e.ToString(), nLog.Type.Error); }
        }

        public static void TakeBiz(Player player)//Забираю бизнес, если есть
        {
            try
            {
                var acc = Main.Players[player];
                var biz = Main.Players[player].BizIDs;
                if (biz.Count == 0)
                {

                }
                else
                {
                    foreach (Business bizs in BusinessManager.BizList.Values)
                    {
                        string owner = bizs.Owner;
                        Player players = NAPI.Player.GetPlayerFromName(owner);
                        if (players != null && Main.Players.ContainsKey(players))
                        {
                            biz.Remove(bizs.ID);
                            bizs.Owner = "Государство";
                            bizs.UpdateLabel();
                            BusinessManager.SavingBusiness();
                            acc.CreditMoney = 0;
                            MySQL.Query($"UPDATE `characters` SET `creditmoney`= {acc.CreditMoney}");
                            MySQL.Query($"UPDATE `characters` SET `allowcredit`= {acc.AllowCredit = 0}");
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У вас отобрали бизнес за неуплату кредита.", 3000);
                            return;
                        }
                    }
                }
            }
            catch (Exception e) { Log.Write("takeProperty: " + e.Message, nLog.Type.Error); }
        }
        #endregion
    }
}