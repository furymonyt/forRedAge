using System;
using NeptuneEvo.Core;
using System.Data;
using GTANetworkAPI;
using Redage.SDK;
using NeptuneEvo.MoneySystem;

namespace NeptuneEvo.Core
{
    class Weddings : Script
    {
        #region [blip and colshape info]
        private static nLog Log = new nLog("Weddings");
        private static float colRadius = 0.7f;
        private static Vector3 colPosition = new Vector3(-785.0517, 20.514696, 38.83411);
        private static Vector3 blipPosition = new Vector3(-785.0517, 20.514696, 38.83411);
        private static string blipName = "Бракосочетания";
        private static byte blipColor = 74;
        private static uint blipID = 171;
        private static ColShape colShape;
        private static Blip blip;
        #endregion

        #region [ResourseStart]
        [ServerEvent(Event.ResourceStart)]
        public static void standOnColshape()
        {
            colShape = NAPI.ColShape.CreateCylinderColShape(colPosition, colRadius, 2, dimension: 0);
            colShape.OnEntityEnterColShape += (ColShape shape, Player client) =>
            {
                try
                {
                    NAPI.Data.SetEntityData(client, "INTERACTIONCHECK", 3233);
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

            blip = NAPI.Blip.CreateBlip(blipID, blipPosition, 1, blipColor, blipName, 255, 0, shortRange: true, dimension: 0);
            NAPI.Marker.CreateMarker(1, colPosition, new Vector3(), new Vector3(), 1f, new Color(0, 0, 255));
            NAPI.TextLabel.CreateTextLabel("Бракосочетания", colPosition + new Vector3(0, 0, 1), 5F, 0.3F, 0, new Color(217, 35, 217));
            Log.Write("Weddings loaded...", nLog.Type.Info);
        }
        #endregion

        #region [Open and Close menu]
        public static void OpenWeddingsMenu(Player player)
        {
            Trigger.ClientEvent(player, "openWeddingsMenu");
            if(Main.Players[player].WeddingApplication == 1)
            {
                Trigger.ClientEvent(player, "applications", Main.Players[player].WeddingApplication.ToString());
                Trigger.ClientEvent(player, "applName", Main.Players[player].ApplName);
                Trigger.ClientEvent(player, "applSurname", Main.Players[player].ApplSurname);
            }
        }
        public static void CloseWeddingsMenu(Player player)
        {
            Trigger.ClientEvent(player, "closeWeddingsMenu");
        }
        #endregion

        #region[Wedding]
        [RemoteEvent("WeddingName")]
        public static void WeddingName(Player player, params object[] args)
        {
            try
            {
                var acc = Main.Players[player];
                int index = Convert.ToInt32(args[0]);
                string name = Convert.ToString(args[1]);
                DataTable nameDB = MySQL.QueryRead($"SELECT * FROM `characters` WHERE `firstname`='{name}'");
                switch (index)
                {
                    case 1:
                        if (nameDB == null || nameDB.Rows.Count == 0 && name == acc.FirstName)
                        {
                            Trigger.ClientEvent(player, "closeWeddingsMenu");
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введённое Имя не найдено.", 5000);
                            return;
                        }
                        else
                        {
                            if (acc.MarriageName == "null" && acc.MarriageSurname == "null")
                            {

                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот человек уже состоит в браке.", 5000);
                                return;
                            }
                        }
                        return;
                }
            }
            catch (Exception e) { Log.Write("WeddingName: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("WeddingSurname")]
        public static void WeddingSurname(Player player, params object[] args)
        {
            try
            {
                var acc = Main.Players[player];
                int index = Convert.ToInt32(args[0]);
                string surname = Convert.ToString(args[1]);
                DataTable surnameDB = MySQL.QueryRead($"SELECT * FROM `characters` WHERE `lastname`='{surname}'");
                switch (index)
                {
                    case 2:
                        if (surnameDB == null || surnameDB.Rows.Count == 0 && surname == acc.LastName)
                        {
                            Trigger.ClientEvent(player, "closeWeddingsMenu");
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введенная Фамилия ({surname}) не найдена.", 5000);
                            return;
                        }
                        else
                        {
                            Trigger.ClientEvent(player, "closeWeddingsMenu");
                                if (acc.MarriageName == "null" && acc.MarriageSurname == "null")
                                {
                                    if (Wallet.Change(player, -Math.Abs(50000)))
                                    {
                                        MySQL.Query($"UPDATE `characters` SET `applName`='{acc.FirstName}', `applSurname`='{acc.LastName}', `weddingappl`='{1}'  WHERE `lastname`='{surname}'");
                                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно оставили заявление. Ждите ответа.", 5000);
                                        return;
                                    }
                                    else
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Не достаточно средств.", 5000);
                                        return;
                                    }
                                }
                                else
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот человек уже состоит в браке.", 5000);
                                    return;
                                }
                        }
                }
            }
            catch (Exception e) { Log.Write("WeddingSurname: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("WeddingDivorce")]
        public static void WeddingDivorce(Player player, params object[] args)
        {
            try
            {
                var acc = Main.Players[player];
                int index = Convert.ToInt32(args[0]);
                switch (index)
                {
                    case 3:
                        Trigger.ClientEvent(player, "closeWeddingsMenu");
                        if (acc.MarriageName == "null" && acc.MarriageSurname == "null")
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в браке.", 5000);
                            return;
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы развелись с `{acc.MarriageName}_{acc.MarriageSurname}`.", 5000);
                            MySQL.Query($"UPDATE `characters` SET `mName`= '{"null"}', `mSurname`='{"null"}'  WHERE `uuid`='{acc.UUID}'");
                            MySQL.Query($"UPDATE `characters` SET `mName`= '{"null"}', `mSurname`='{"null"}'  WHERE `mName`='{acc.MarriageName}'");
                            return;
                        }
                }
            }
            catch (Exception e) { Log.Write("WeddingDivorce: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("YesOrNo")]
        public static void YesOrNo(Player player, params object[] args)
        {
            try
            {
                var acc = Main.Players[player];
                int index = Convert.ToInt32(args[0]);
                switch (index)
                {
                    case 10:
                        Trigger.ClientEvent(player, "closeWeddingsMenu");
                        if (acc.WeddingApplication == 1)
                        {
                            if (acc.MarriageName == "null" && acc.MarriageSurname == "null")
                            {
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно приняли приглашение в брак. Поздравляем молодожёнов!", 5000);
                                MySQL.Query($"UPDATE `characters` SET `mName`='{acc.ApplName}', `mSurname`='{acc.ApplSurname}', `applName`='{"null"}', `applSurname`='{"null"}', `weddingappl`='{0}' WHERE `uuid`='{acc.UUID}'");
                                MySQL.Query($"UPDATE `characters` SET `mName`='{acc.FirstName}', `mSurname`='{acc.LastName}', `applName`='{"null"}', `applSurname`='{"null"}', `weddingappl`='{0}'WHERE `firstname`='{acc.ApplName}'");
                                NAPI.Chat.SendChatMessageToAll("!{#bd1dae}" + $"{acc.FirstName}_{acc.LastName} и {acc.ApplName}_{acc.ApplSurname} вступили в брак. Поздравляем и желаем успехов в совместной жизни!"); ;
                                return;
                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже в браке с `{acc.MarriageName}_{acc.MarriageSurname}`.", 5000);
                                return;
                            }
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет заявлений.", 5000);
                            return;
                        }
                    case 11:
                        Trigger.ClientEvent(player, "closeWeddingsMenu");
                        MySQL.Query($"UPDATE `characters` SET `applName`='{"null"}', `applSurname`='{"null"}', `weddingappl`='{0}' WHERE `uuid`='{acc.UUID}'");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отклонили предложение брака.", 5000);
                        return;
                }
            }
            catch (Exception e) { Log.Write("WeddingYesOrNo: " + e.Message, nLog.Type.Error); }
        }
        #endregion
    }
}