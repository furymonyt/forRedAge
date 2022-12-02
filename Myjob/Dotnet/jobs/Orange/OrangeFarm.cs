using GTANetworkAPI;
using System.Collections.Generic;
using System;
using Alyx.GUI;
using Alyx.Core;
using AlyxSDK;
using Alyx.Fractions;

namespace Alyx.Jobs
{
    class OrangeFarm : Script
    {
        private static nLog Log = new nLog("OrangeFarm");
        private static int JobWorkId = 10;
        private static int JobPayment1 = 15;
        private static int JobPayment2 = 30;
        private static Dictionary<int, ColShape> Cols = new Dictionary<int, ColShape>();
        private void cf_onEntityEnterColShape(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", 383);
            }
            catch (Exception ex) { Log.Write("gp_onEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
        }
        private void cf_onEntityExitColShape(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", 0);
            }
            catch (Exception ex) { Log.Write("gp_onEntityExitColShape: " + ex.Message, nLog.Type.Error); }
        }
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                NAPI.Blip.CreateBlip(514, new Vector3(2416.5898, 4994.111, 45.109695), 0.8f, 5, Main.StringToU16("Апельсиновая ферма"), 255, 0, true, 0, 0);

                Cols.Add(1, NAPI.ColShape.CreateCylinderColShape(new Vector3(2416.5898, 4994.111, 45.109695), 2, 2, 0));// get clothes
                Cols[1].OnEntityEnterColShape += cf_onEntityEnterColShape;
                Cols[1].OnEntityExitColShape += cf_onEntityExitColShape;
                Cols[1].SetData("INTERACT", 383);
                //NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Роберт"), new Vector3(2416.5898, 4994.111, 47.40), 10F, 0.5F, 0, new Color(0, 180, 0));

                int i = 0;
                foreach (var Check2 in Checkpoints2LVL)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check2.Position, 2, 2, 0);
                    col.SetData("NUMBER2", i);
                    col.OnEntityEnterColShape += PlayerEnterCheckpoint2LVL;
                    i++;
                }
                int i2 = 0;
                foreach (var Check in Checkpoints1LVL)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 2, 2, 0);
                    col.SetData("NUMBER", i2);
                    col.OnEntityEnterColShape += PlayerEnterCheckpoint1LVL;
                    i2++;
                }
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("ChangeWorkStateOrange")]
        public static void ChangeWorkStateOrange(Player player, int act)
        {
            try
            {
                switch (act)
                {
                    case -1:
                        JobOrangeLeave(player);
                        return;
                    default:
                        JobOrangeJoin(player);
                        return;
                }
            }
            catch (Exception e) { Log.Write("WorkStateOrange: " + e.Message, nLog.Type.Error); }
        }
        private static void JobOrangeJoin(Player player)
        {
            if (Main.Players[player].WorkID != 10)
            {
                if (Main.Players[player].WorkID != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы уже работаете: {Jobs.WorkManager.JobStats[Main.Players[player].WorkID - 1]}", 3000);
                    return;
                }
                Main.Players[player].WorkID = JobWorkId;
                Dashboard.sendStats(player);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы устроились работать сборщиком апельсинов");
                return;
            }
        }
        private static void JobOrangeLeave(Player player)
        {
            if (NAPI.Data.GetEntityData(player, "ON_WORK") == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы должны сначала закончить работу сборщиком апельсинов", 3000);
                return;
            }
            if (Main.Players[player].WorkID != 0)
            {
                if (Main.Players[player].WorkID == 10)
                {
                    if (player.GetData<bool>("ON_WORK2"))
                    {
                        Customization.ApplyCharacter(player);
                        player.ResetData("ON_WORK2");
                        player.ResetSharedData("ON_WORK2");
                        Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                        Trigger.ClientEvent(player, "deleteWorkBlip");
                        //Main.Players[player].WorkID = 0;
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, $"Вы закончили рабочий день", 3000);
                        return;
                    }
                    if (player.GetData<bool>("ON_WORK"))
                    {
                        Customization.ApplyCharacter(player);
                        player.ResetData("ON_WORK");
                        player.ResetSharedData("ON_WORK");
                        Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                        Trigger.ClientEvent(player, "deleteWorkBlip");
                        //Main.Players[player].WorkID = 0;
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, $"Вы закончили рабочий день", 3000);
                        return;
                    }
                    Main.Players[player].WorkID = 0;
                    Dashboard.sendStats(player);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы уволились с работы сборщика апельсинов");
                    return;
                }
            }
        }
        [RemoteEvent("server::startOrangeWork")]
        private static void SelectOrangeWork(Player player, int act)
        {
            try
            {
                switch (act)
                {
                    case 0:
                        OrangeWorkStateDay1(player);
                        return;
                    case 1:
                        OrangeWorkStateDay2(player);
                        return;
                }
            }
            catch (Exception e) { Log.Write("workOrangeSelect: " + e.Message, nLog.Type.Error); }
        }
        private static void OrangeWorkStateDay1(Player player)
        {
            if (Main.Players[player].WorkID != 10)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не работаете собрщиком апельсинов");
                return;
            }
            if (player.GetData<bool>("ON_WORK2"))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, $"Нельзя начать вторую работу, закончите первую", 3000);
                return;
            }
            if (player.GetData<bool>("ON_WORK"))
            {
                Customization.ApplyCharacter(player);
                player.ResetData("ON_WORK");
                player.ResetSharedData("ON_WORK");
                Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                //Main.Players[player].WorkID = 0;
                int UUID = Main.Players[player].UUID;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, $"Вы закончили рабочий день", 3000);
                return;
            }
            else
            {
                Customization.ClearClothes(player, Main.Players[player].Gender);
                if (Main.Players[player].Gender)
                {
                    player.SetClothes(3, 0, 0);
                    player.SetClothes(11, 1, 5);
                    player.SetClothes(4, 6, 0);
                    player.SetClothes(6, 5, 0);
                }
                else
                {
                    player.SetClothes(3, 2, 0);
                    player.SetClothes(11, 40, 0);
                    player.SetClothes(4, 25, 7);
                    player.SetClothes(6, 5, 0);
                }

                var check = WorkManager.rnd.Next(0, Checkpoints1LVL.Count - 1);
                player.SetData("WORKCHECK", check);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints1LVL[check].Position, 107, 107, 250, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checkpoints1LVL[check].Position);

                player.SetData("ON_WORK", true);
                //Main.Players[player].WorkID = JobWorkId;
                player.SetSharedData("ON_WORK", true);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, "Вы начали рабочий день, на вашем GPS указана точка сбора апельсинов", 3000);

                return;
            }
        }
        private static void OrangeWorkStateDay2(Player player)
        {
            if (Main.Players[player].WorkID != 10)
            {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы не работаете собрщиком апельсинов");
                return;
            }
            if (player.GetData<bool>("ON_WORK"))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, $"Нельзя начать вторую работу, закончите первую", 3000);
                return;
            }
            if (player.GetData<bool>("ON_WORK2"))
            {
                Customization.ApplyCharacter(player);
                player.ResetData("ON_WORK2");
                player.ResetSharedData("ON_WORK2");
                Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                //Main.Players[player].WorkID = 0;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, $"Вы закончили рабочий день", 3000);
                return;
            }
            else
            {
                Customization.ClearClothes(player, Main.Players[player].Gender);
                if (Main.Players[player].Gender)
                {
                    player.SetClothes(3, 0, 0);
                    player.SetClothes(11, 1, 5);
                    player.SetClothes(4, 6, 0);
                    player.SetClothes(6, 5, 0);
                }
                else
                {
                    player.SetClothes(3, 2, 0);
                    player.SetClothes(11, 40, 0);
                    player.SetClothes(4, 25, 7);
                    player.SetClothes(6, 5, 0);
                }

                var check = WorkManager.rnd.Next(0, Checkpoints2LVL.Count - 1);
                player.SetData("WORKCHECK2", check);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints2LVL[check].Position, 107, 107, 250, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checkpoints2LVL[check].Position);

                player.SetData("ON_WORK2", true);
                //Main.Players[player].WorkID = JobWorkId;
                player.SetSharedData("ON_WORK2", true);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomLeft, "Вы начали рабочий день, на вашем GPS указана точка сбора апельсинов", 3000);

                return;
            }
        }
        public static void interactPressed(Player player, int id)
        {
            switch (id)
            {
                case 383:
                    try
                    {
                        if (!Main.Players.ContainsKey(player)) return;
                        //StartWorkDay(client);
                        Trigger.ClientEvent(player, "OpenWorkMenuOrange", Main.Players[player].LVL, JobPayment1, JobPayment2, Main.Players[player].WorkID, NAPI.Data.GetEntityData(player, "ON_WORK"), NAPI.Data.GetEntityData(player, "ON_WORK2"));
                    }
                    catch (Exception e) { Log.Write("IneractionOrange: " + e.Message, nLog.Type.Error); }
                    return;
            }

        }

        private static void PlayerEnterCheckpoint1LVL(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!player.GetData<bool>("ON_WORK") ||
                    shape.GetData<int>("NUMBER") != player.GetData<int>("WORKCHECK")) return;

                if (Checkpoints1LVL[(int)shape.GetData<int>("NUMBER")].Position.DistanceTo(player.Position) > 3) return;

                NAPI.Entity.SetEntityPosition(player,
                    Checkpoints1LVL[shape.GetData<int>("NUMBER")].Position + new Vector3(0, 0, 1.2));
                NAPI.Entity.SetEntityRotation(player,
                    new Vector3(0, 0, Checkpoints1LVL[shape.GetData<int>("NUMBER")].Heading));
                Main.OnAntiAnim(player);
                Trigger.ClientEvent(player, "orangeOpenMenu");
                Trigger.ClientEvent(player, "FreezePlayerFix", true);
                player.PlayAnimation("amb@prop_human_movie_studio_light@base", "base", 1);
                //player.SetData("WORKCHECK", -1);

            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }

        private static void PlayerEnterCheckpoint2LVL(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!player.GetData<bool>("ON_WORK2") ||
                    shape.GetData<int>("NUMBER2") != player.GetData<int>("WORKCHECK2")) return;

                if (Checkpoints2LVL[(int)shape.GetData<int>("NUMBER2")].Position.DistanceTo(player.Position) > 3) return;

                NAPI.Entity.SetEntityPosition(player, Checkpoints2LVL[shape.GetData<int>("NUMBER2")].Position + new Vector3(0, 0, 1.2));
                NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checkpoints2LVL[shape.GetData<int>("NUMBER2")].Heading));
                Main.OnAntiAnim(player);
                Trigger.ClientEvent(player, "orangeOpenMenu");
                Trigger.ClientEvent(player, "FreezePlayerFix", true);
                player.PlayAnimation("amb@prop_human_movie_studio_light@base", "base", 1);
                //player.SetData("WORKCHECK", -1);

            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }

        [RemoteEvent("orangeStopWork")]
        private static void orangeStopWork(Player player, int count)
        {
            try
            {
                if (player.HasData("ON_WORK"))
                {
                    if (player != null && Main.Players.ContainsKey(player))
                    {
                        
                        Main.OffAntiAnim(player);
                        player.StopAnimation();
                        Trigger.ClientEvent(player, "FreezePlayerFix", false);
                        MoneySystem.Wallet.Change(player, JobPayment1);
                        var nextCheck = WorkManager.rnd.Next(0, Checkpoints1LVL.Count - 1);
                        while (nextCheck == player.GetData<int>("WORKCHECK"))
                            nextCheck = WorkManager.rnd.Next(0, Checkpoints1LVL.Count - 1);
                        player.SetData("WORKCHECK", nextCheck);
                        Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints1LVL[nextCheck].Position, 107, 107, 250, 0, 0);
                        Trigger.ClientEvent(player, "createWorkBlip", Checkpoints1LVL[nextCheck].Position);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomLeft, $"Вы получили {JobPayment1}$, следующая точка установлена", 3000);
                        return;
                    }
                }
                if (player.HasData("ON_WORK2"))
                {
                    if (player != null && Main.Players.ContainsKey(player))
                    {
                        
                        Main.OffAntiAnim(player);
                        player.StopAnimation();
                        Trigger.ClientEvent(player, "FreezePlayerFix", false);
                        MoneySystem.Wallet.Change(player, JobPayment2);
                        var nextCheck = WorkManager.rnd.Next(0, Checkpoints2LVL.Count - 1);
                        while (nextCheck == player.GetData<int>("WORKCHECK"))
                            nextCheck = WorkManager.rnd.Next(0, Checkpoints2LVL.Count - 1);
                        player.SetData("WORKCHECK", nextCheck);
                        Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints2LVL[nextCheck].Position, 107, 107, 250, 0, 0);
                        Trigger.ClientEvent(player, "createWorkBlip", Checkpoints2LVL[nextCheck].Position);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomLeft, $"Вы получили {JobPayment2}$, следующая точка установлена", 3000);
                        return;
                    }
                }
            }
            catch
            {
            }
        }

        private static List<Checkpoint> Checkpoints1LVL = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(2424.753, 4658.507, 32.33183), 77),
            new Checkpoint(new Vector3(2434.1553, 4678.153, 32.26446), -15),
            new Checkpoint(new Vector3(2418.5845, 4673.539, 32.802357), -72),
            new Checkpoint(new Vector3(2421.9434, 4685.8477, 32.61524), -15),
            new Checkpoint(new Vector3(2407.0925, 4677.393, 32.845867), 148),
            new Checkpoint(new Vector3(2402.039, 4687.878, 32.574944), 26),
            new Checkpoint(new Vector3(2412.2708, 4706.7095, 31.881617), -19),
            new Checkpoint(new Vector3(2383.0913, 4701.0283, 32.83377), 102),
            new Checkpoint(new Vector3(2386.673, 4723.9834, 32.53565), -25),
            new Checkpoint(new Vector3(2375.1538, 4734.855, 32.57244), 56),
            new Checkpoint(new Vector3(2365.7358, 4729.6445, 33.010292), 82),
            new Checkpoint(new Vector3(2365.7358, 4729.6445, 33.010292), 53),
            new Checkpoint(new Vector3(2343.9202, 4754.851, 33.66181), 22),
            new Checkpoint(new Vector3(2323.8083, 4746.949, 34.978252), -110),
        };

        private static List<Checkpoint> Checkpoints2LVL = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(2390.2268, 5004.7188, 44.641857), 116),
            new Checkpoint(new Vector3(2377.9182, 5004.853, 43.5815), 157),
            new Checkpoint(new Vector3(2374.3965, 4989.9917, 42.92537), 160),
            new Checkpoint(new Vector3(2362.3428, 4989.2188, 42.250416), 106),
            new Checkpoint(new Vector3(2360.8499, 5003.2134, 42.341255), 178),
            new Checkpoint(new Vector3(2369.312, 5012.1167, 43.293255), -179),
        };
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
