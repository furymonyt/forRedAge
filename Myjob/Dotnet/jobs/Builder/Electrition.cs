using System;
using System.Collections.Generic;
using Alyx.Core;
using AlyxSDK;
using GTANetworkAPI;
using static Alyx.Core.Quests;

namespace Alyx.Jobs
{
    class Electrition : Script
    {
        public static nLog Log = new nLog("Builder");
        public static int JobPayment = 160;

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                int i = 0;
                foreach (var Check in Checks1)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 2, 2, 0);
                    col.SetData("NUMBER2", i);
                    col.OnEntityEnterColShape += EnterShapePutContAndTakeNew;
                    col.OnEntityExitColShape += ExitColshape;
                    i++;
                }
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }
        public static void ExitColshape(ColShape shape, Player player)
        {
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);
        }
        public static void SetWorkId(Player player)
        {
            if (Main.Players[player].WorkID != 11)
            {
                if (Main.Players[player].WorkID != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы уже работаете: {Jobs.WorkManager.JobStats[Main.Players[player].WorkID - 1]}", 3000);
                    return;
                }
                Main.Players[player].WorkID = 11;
                Notify.Succ(player, "Вы устроились на работу электрика");
            }
            else
            {
                if (NAPI.Data.GetEntityData(player, "ON_WORK") == true)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы должны сначала закончить работу", 3000);
                    return;
                }
                if (Main.Players[player].WorkID != 0)
                {
                    if (Main.Players[player].WorkID == 11)
                    {
                        Main.Players[player].WorkID = 0;
                        Notify.Succ(player, "Вы уволились с работы электрика");
                    }
                }
            }
        }
        public static void SetWorkState(Player player)
        {
            if (Main.Players[player].WorkID != 11)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не работаете электриком");
                return;
            }
            else
            {
                if (player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK"))
                {
                    Customization.ApplyCharacter(player);
                    player.SetData("ON_WORK", false);
                    player.SetSharedData("ON_WORK", false);
                    Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы закончили рабочий день", 3000);
                    if (player.HasData("ShapeelectState") && player.GetData<bool>("ShapeelectState"))
                    {
                        BasicSync.DetachObject(player);
                    }
                    return;
                }
                else
                {
                    Customization.ClearClothes(player, Main.Players[player].Gender);
                    if (Main.Players[player].Gender)
                    {
                        player.SetClothes(3, 61, 0);
                        player.SetClothes(4, 36, 0);
                        player.SetClothes(6, 12, 0);
                        player.SetClothes(8, 59, 1);
                        player.SetClothes(11, 57, 0);
                        player.SetClothes(0, 0, 0);
                    }
                    else
                    {
                        player.SetClothes(3, 62, 0);
                        player.SetClothes(4, 35, 0);
                        player.SetClothes(6, 26, 0);
                        player.SetClothes(8, 36, 1);
                        player.SetClothes(11, 50, 0);
                        player.SetClothes(0, 0, 0);
                    }
                    var check = WorkManager.rnd.Next(0, Checks1.Count - 1);
                    player.SetData("WORKCHECK", check);
                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks1[check].Position, 1, 0, 255, 0, 0);
                    Trigger.ClientEvent(player, "createWorkBlip", Checks1[check].Position);

                    player.SetData("ON_WORK", true);
                    player.SetSharedData("ON_WORK", true);
                    Notify.Succ(player, "Вы начали рабочий день электрика. Метка установлена на GPS");
                }
            }
        }
        public static void WorkFirstCont(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");

            player.SetData("ShapeelectState", false);
            player.SetSharedData("ShapeelectState", false);
            NAPI.Entity.SetEntityPosition(player, Checks1[shape.GetData<int>("NUMBER2")].Position + new Vector3(0, 0, 1.2));
            NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checks1[shape.GetData<int>("NUMBER2")].Heading));
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);

            Main.OnAntiAnim(player);
            player.PlayAnimation("amb@prop_human_movie_studio_light@base", "base", 39);
            NAPI.Task.Run(() =>
            {
                player.StopAnimation();
                MoneySystem.Wallet.Change(player, JobPayment);
                var nextCheck = WorkManager.rnd.Next(0, Checks1.Count - 1);
                while (nextCheck == player.GetData<int>("WORKCHECK"))
                    nextCheck = WorkManager.rnd.Next(0, Checks1.Count - 1);
                player.SetData("WORKCHECK", nextCheck);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks1[nextCheck].Position, 1, 0, 255, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checks1[nextCheck].Position);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {JobPayment}$, следующая точка установлена", 3000);
                player.SetData("ShapeelectState", true);
                player.SetSharedData("ShapeelectState", true);
            }, 1500);
        }
        private static void EnterShapePutContAndTakeNew(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 11 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER2") != player.GetData<int>("WORKCHECK")) return;
                if (player.HasData("ShapeelectState") && !player.GetData<bool>("ShapeelectState")) return;

                player.SetData("INTERACTIONCHECK", 2000);
                player.SetData("WorkColshape", shape);

            }
            catch
            {

            }
        }
        private static List<Points> Checks1 = new List<Points>()
        {
            new Points(new Vector3(-145.48969, -942.96936, 268.01532), -22.98),
            new Points(new Vector3(-136.68193, -968.1325, 253.01138), -112.3),
            new Points(new Vector3(-162.80936, -1022.618, 38.21928), -149.85),
            new Points(new Vector3(-168.84093, -1015.7989, 20.1568), 158.1),
            new Points(new Vector3(-95.658745, -965.1276, 20.15684), -22),
            new Points(new Vector3(-191.75148, -1110.0709, 41.01656), 112),

        };
        
        
        internal class Points
        {
            public Vector3 Position { get; }
            public double Heading { get; }

            public Points(Vector3 pos, double rot)
            {
                Position = pos;
                Heading = rot;
            }
        }

    }
}