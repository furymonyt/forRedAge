﻿using GTANetworkAPI;
using System.Collections.Generic;
using System;
using NeptuneEvo.GUI;
using NeptuneEvo.Core;
using Redage.SDK;

namespace NeptuneEvo.Jobs
{
    class Loader : Script
    {
        private static int checkpointPayment = 100;
        private static int JobWorkId = 9;
        private static int JobsMinLVL = 1;
        private static nLog Log = new nLog("Loader");

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                NAPI.Blip.CreateBlip(615, new Vector3(1240.164, -3106.249, 4.908434), 1.5f, 46, Main.StringToU16("Грузчик"), 255, 0, true, 0, 0); // Блип на карте
                NAPI.TextLabel.CreateTextLabel("~w~Приму вас на работу", new Vector3(1240.2, -3106.788, 7.30), 30f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension); // Над головой у бота
                //NAPI.Marker.CreateMarker(1, new Vector3(1240.164, -3106.249, 4.908434) - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); //Начать рабочий день маркер
                var col = NAPI.ColShape.CreateCylinderColShape(new Vector3(1240.164, -3106.249, 4.908434), 1, 2, 0); // Меню которое открывается на 'E'

                col.OnEntityEnterColShape += (shape, player) => {
                    try
                    {
                        player.SetData("INTERACTIONCHECK", 506);
                        //Trigger.ClientEvent(player, "PressE", true);
                        Trigger.ClientEvent(player, "JobsEinfo");
                    }
                    catch (Exception ex) { Log.Write("col.OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
                };
                col.OnEntityExitColShape += (shape, player) => {
                    try
                    {
                        player.SetData("INTERACTIONCHECK", 0);
                        //Trigger.ClientEvent(player, "PressE", false);
                        Trigger.ClientEvent(player, "JobsEinfo2");
                    }
                    catch (Exception ex) { Log.Write("col.OnEntityExitColShape: " + ex.Message, nLog.Type.Error); }
                };

                int i = 0;
                foreach (var Check in Checkpoints)
                {
                    col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 1, 2, 0);
                    col.SetData("NUMBER2", i);
                    col.OnEntityEnterColShape += PlayerEnterCheckpoint;
                    i++;
                };

                int ii = 0;
                foreach (var Check in Checkpoints2)
                {
                    col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 4, 2, 0);
                    col.SetData("NUMBER3", ii);
                    col.OnEntityEnterColShape += PlayerEnterCheckpoint;
                    ii++;
                };
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        #region Чекпоинты
        private static List<Checkpoint> Checkpoints = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(1266.80, -3129.506, 4.782248), 269.0657), // Взять ящик 0
            new Checkpoint(new Vector3(1266.47, -3133.008, 4.782152), 267.7546), // Взять ящик 1
            new Checkpoint(new Vector3(1266.31, -3119.273, 4.782105), 269.8171), // Взять ящик 2
            new Checkpoint(new Vector3(1247.34, -3172.527, 4.589797), 157.6356), // Взять ящик 3
        };
        private static List<Checkpoint> Checkpoints2 = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(1224.714, -3104.72, 4.907954), 177.4974), // Поставить ящик 0
        };
        #endregion

        #region Меню которое нажимается на E
        public static void StartWorkDayLoader(Client player)
        {
            if (Main.Players[player].LVL < JobsMinLVL)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Необходим как минимум {JobsMinLVL} уровень", 3000);
                return;
            }

            //Trigger.ClientEvent(player, "PressE", false);
            Trigger.ClientEvent(player, "JobsEinfo2");
            Trigger.ClientEvent(player, "OpenLoader", checkpointPayment, Main.Players[player].LVL, Main.Players[player].WorkID, NAPI.Data.GetEntityData(player, "ON_WORK2"));

        }
        #endregion
        #region Устроться на работу
        [RemoteEvent("jobJoinLoader")]
        public static void callback_jobsSelecting(Client client, int act)
        {
            try
            {
                switch (act)
                {
                    case -1:
                        Layoff(client);
                        return;
                    default:
                        JobJoin(client);
                        return;
                }
            }
            catch (Exception e) { Log.Write("jobjoin: " + e.Message, nLog.Type.Error); }
        }
        public static void Layoff(Client player)
        {
            if (NAPI.Data.GetEntityData(player, "ON_WORK") == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны сначала закончить рабочий день", 3000);
                return;
            }
            if (Main.Players[player].WorkID != 0)
            {
                Main.Players[player].WorkID = 0;
                //Dashboard.sendStats(player);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы уволились с работы", 3000);
                var jobsid = Main.Players[player].WorkID;
                Trigger.ClientEvent(player, "secusejobLoader", jobsid);
                return;
            }
            else
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы никем не работаете", 3000);
        }
        public static void JobJoin(Client player)
        {
            if (NAPI.Data.GetEntityData(player, "ON_WORK") == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны сначала закончить рабочий день", 3000);
                return;
            }
            if (Main.Players[player].WorkID != 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже работаете: {Jobs.WorkManager.JobStats[Main.Players[player].WorkID - 1]}", 3000);
                return;
            }
            Main.Players[player].WorkID = JobWorkId;
            //Dashboard.sendStats(player);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы устроились на работу", 3000);
            var jobsid = Main.Players[player].WorkID;
            Trigger.ClientEvent(player, "secusejobLoader", jobsid);
            return;
        }
        #endregion
        #region Начать рабочий день
        [RemoteEvent("enterJobLoader")]
        public static void ClientEvent_Loader(Client client, int act)
        {
            try
            {
                switch (act)
                {
                    case -1:
                        Layoff2(client);
                        return;
                    default:
                        JobJoin2(client, act);
                        return;
                }
            }
            catch (Exception e) { Log.Write("jobjoin: " + e.Message, nLog.Type.Error); }
        }
        public static void Layoff2(Client player)
        {
            if (player.GetData("PACKAGES") == 1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Сдайте коробку прежде чем закончить рабочий день", 3000);
                return;
            }
            if (NAPI.Data.GetEntityData(player, "ON_WORK") != false)
            {
                Customization.ApplyCharacter(player);
                player.SetData("ON_WORK", false);
                player.SetData("ON_WORK2", 0);
                Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                player.SetData("PACKAGES", 0);

                //player.StopAnimation();
                //Main.OffAntiAnim(player);
                //BasicSync.DetachObject(player);
                MoneySystem.Wallet.Change(player, player.GetData("PAYMENT"));

                Trigger.ClientEvent(player, "CloseJobStatsInfo", player.GetData("PAYMENT"));
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"+ {player.GetData("PAYMENT")}$", 3000);
                player.SetData("PAYMENT", 0);
            }
            else
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже не работаете", 3000);
            }
        }
        public static void JobJoin2(Client player, int job)
        {
            if (Main.Players[player].WorkID != JobWorkId)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не работаете на этой работе.", 3000);
                return;
            }
            if (NAPI.Data.GetEntityData(player, "ON_WORK") == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны сначала закончить рабочий день", 3000);
                return;
            }

            Customization.ClearClothes(player, Main.Players[player].Gender);
            if (Main.Players[player].Gender)
            {
                //player.SetAccessories(1, 24, 2);
                //player.SetClothes(3, 2, 0);
                player.SetClothes(8, 59, 0);
                player.SetClothes(11, 1, 0);
                player.SetClothes(4, 0, 5);
                player.SetClothes(6, 48, 0);
            }
            else
            {
                //player.SetAccessories(1, 26, 2);
                //player.SetClothes(3, 11, 0);
                player.SetClothes(8, 36, 0);
                player.SetClothes(11, 0, 0);
                player.SetClothes(4, 1, 5);
                player.SetClothes(6, 49, 0);
            }
            var check = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
            player.SetData("WORKCHECK", check);
            Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[check].Position, 2, 0, 255, 0, 0);
            Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[check].Position);
            player.SetData("PACKAGES", 0);

            player.SetData("ON_WORK", true);
            player.SetData("ON_WORK2", job);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы начали рабочий день", 3000);
            Trigger.ClientEvent(player, "JobStatsInfo", player.GetData("PAYMENT"));

        }
        #endregion
        #region Когда заходишь в чекпоинт
        private static void PlayerEnterCheckpoint(ColShape shape, Client player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != JobWorkId || !player.GetData("ON_WORK")) return;

                if (player.GetData("PACKAGES") == 0)
                {
                    if (shape.GetData("NUMBER2") == player.GetData("WORKCHECK"))
                    {
                        player.SetData("PACKAGES", player.GetData("PACKAGES") + 1);

                        NAPI.Entity.SetEntityPosition(player, Checkpoints[shape.GetData("NUMBER2")].Position + new Vector3(0, 0, 1.2));
                        NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checkpoints[shape.GetData("NUMBER2")].Heading));

                        Main.OnAntiAnim(player);
                        player.PlayAnimation("anim@mp_snowball", "pickup_snowball", 47);

                        player.SetData("WORKCHECK", -1);
                        var check = WorkManager.rnd.Next(0, Checkpoints2.Count - 1);
                        player.SetData("WORKCHECK", check);
                        Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints2[check].Position, 5, 0, 255, 0, 0);
                        Trigger.ClientEvent(player, "createWorkBlip", Checkpoints2[check].Position);
                        NAPI.Task.Run(() =>
                        {
                            try
                            {
                                if (player != null && Main.Players.ContainsKey(player))
                                {
                                    player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
                                    BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("v_ret_ml_beerpis1"), 18905, new Vector3(0.1, 0.1, 0.3), new Vector3(-10, -75, -40));
                                }
                            }
                            catch { }
                        }, 250);
                    }
                }
                else
                {
                    if (shape.GetData("NUMBER3") == player.GetData("WORKCHECK"))
                    {
                        player.SetData("PACKAGES", player.GetData("PACKAGES") - 1);
                        player.PlayAnimation("anim@mp_snowball", "pickup_snowball", 47);

                        player.SetData("WORKCHECK", -1);
                        var check = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
                        player.SetData("WORKCHECK", check);
                        Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[check].Position, 2, 0, 255, 0, 0);
                        Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[check].Position);
                        NAPI.Task.Run(() =>
                        {
                            try
                            {
                                if (player != null && Main.Players.ContainsKey(player))
                                {
                                    var payment = Convert.ToInt32(checkpointPayment * Group.GroupPayAdd[Main.Accounts[player].VipLvl] * Main.oldconfig.PaydayMultiplier);
                                    player.SetData("PAYMENT", player.GetData("PAYMENT") + payment);
                                    Trigger.ClientEvent(player, "JobStatsInfo", player.GetData("PAYMENT"));

                                    BasicSync.DetachObject(player);
                                    player.StopAnimation();
                                    Main.OffAntiAnim(player);
                                }
                            }
                            catch { }
                        }, 400);
                    }
                }

            }
            catch (Exception e) { Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Если игрок умер
        public static void Event_PlayerDeath(Client player, Client entityKiller, uint weapon)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID == JobWorkId && player.GetData("ON_WORK"))
                {
                    Customization.ApplyCharacter(player);
                    player.SetData("ON_WORK", false);
                    player.SetData("ON_WORK2", 0);
                    Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                    player.SetData("PACKAGES", 0);

                    player.StopAnimation();
                    Main.OffAntiAnim(player);
                    BasicSync.DetachObject(player);
                    MoneySystem.Wallet.Change(player, player.GetData("PAYMENT"));

                    Trigger.ClientEvent(player, "CloseJobStatsInfo", player.GetData("PAYMENT"));
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"+ {player.GetData("PAYMENT")}$", 3000);
                    player.SetData("PAYMENT", 0);
                }
            }
            catch (Exception e) { Log.Write("PlayerDeath: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Если игрок вышел из игры или его кикнуло
        public static void Event_PlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            try
            {
                if (Main.Players[player].WorkID == JobWorkId && player.GetData("ON_WORK"))
                {
                    Customization.ApplyCharacter(player);
                    player.SetData("ON_WORK", false);
                    player.SetData("ON_WORK2", 0);
                    Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                    player.SetData("PACKAGES", 0);

                    player.StopAnimation();
                    Main.OffAntiAnim(player);
                    BasicSync.DetachObject(player);
                    //MoneySystem.Wallet.Change(player, player.GetData("PAYMENT"));
                    player.SetData("PAYMENT", 0);
                }
            }
            catch (Exception e) { Log.Write("PlayerDisconnected: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        internal class Checkpoint
        {
            public Vector3 Position { get; }
            public double Heading { get; }

            public Checkpoint(Vector3 pos, double rot)
            {
                Position = pos;
                Heading = rot;
            }
        }
    }
}