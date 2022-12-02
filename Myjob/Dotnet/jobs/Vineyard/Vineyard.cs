using GTANetworkAPI;
using System.Collections.Generic;
using System;
using Alyx.GUI;
using Alyx.Core;
using AlyxSDK;
using Alyx.Fractions;

namespace Alyx.Jobs
{
    class VineYard : Script
    {
        private static nLog Log = new nLog("VineYard");

        private static Dictionary<int, ColShape> Cols2 = new Dictionary<int, ColShape>();

        private void cf2_onEntityEnterColShape1(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", shape.GetData<int>("INTERACT"));
            }
            catch (Exception ex) { Log.Write("gp_onEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
        }
        private void cf2_onEntityExitColShape1(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", 0);
            }
            catch (Exception ex) { Log.Write("gp_onEntityExitColShape: " + ex.Message, nLog.Type.Error); }
        }
        public TextLabel label = null;
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                NAPI.Blip.CreateBlip(285, new Vector3(-1874.2358, 2070.4126, 139.87767), 0.9f, 2, Main.StringToU16("Виноградник"), 255, 0, true, 0, 0);


                Cols2.Add(1, NAPI.ColShape.CreateCylinderColShape(new Vector3(-1874.2358, 2070.4126, 140.17767), 2, 2, 0)); // get clothes
                Cols2[1].OnEntityEnterColShape += cf2_onEntityEnterColShape1;
                Cols2[1].OnEntityExitColShape += cf2_onEntityExitColShape1;
                Cols2[1].SetData("INTERACT", 2005);
                //NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Прораб Глеб"), new Vector3(-5.401763, -2541.6501, -9.63951) + new Vector3(0, 0, 1), 10F, 10F, 4, new Color(0, 180, 0));

                int i = 0;
                foreach (var Check in Checkpoints5)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 1, 2, 0);
                    col.SetData("NUMBER", i);
                    col.OnEntityEnterColShape += PlayerEnterCheckpoint3;
                    i++;
                }

            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        public static void StartWorkDay2(Player player)
        {

            if (player.GetData<bool>("ON_WORK"))
            {
                Customization.ApplyCharacter(player);
                player.SetData("ON_WORK", false);

                Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                Trigger.ClientEvent(player, "deleteWorkBlip");
                int UUID = Main.Players[player].UUID;
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы закончили рабочий день", 3000);
                return;
            }
            else
            {
                Customization.ClearClothes(player, Main.Players[player].Gender);
                if (Main.Players[player].Gender)
                {
                    player.SetClothes(3, 0, 0);
                    player.SetClothes(11, 0, 0);
                    player.SetClothes(4, 90, 0);
                    player.SetClothes(6, 7, 0);
                }
                else
                {
                    player.SetClothes(3, 2, 0);
                    player.SetClothes(11, 0, 3);
                    player.SetClothes(4, 25, 7);
                    player.SetClothes(6, 5, 0);
                }

                var check = WorkManager.rnd.Next(0, Checkpoints5.Count - 1);
                player.SetData("WORKCHECK", check);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints5[check].Position, 107, 107, 250, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checkpoints5[check].Position);

                player.SetData("ON_WORK", true);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы начали рабочий день", 3000);

                return;
            }
        }
        public static void interactPressed2(Player client, int id)
        {
            if (client.IsInVehicle) return;
            switch (id)
            {
                case 2005:
                    try
                    {
                        if (!Main.Players.ContainsKey(client)) return;
                        StartWorkDay2(client);
                    }
                    catch (Exception e) { Log.Write("PlayerExitVehicle: " + e.Message, nLog.Type.Error); }
                    return;
            }

        }
        private static void PlayerEnterCheckpoint3(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!player.GetData<bool>("ON_WORK") ||
                    shape.GetData<int>("NUMBER") != player.GetData<int>("WORKCHECK")) return;

                if (Checkpoints5[(int)shape.GetData<int>("NUMBER")].Position.DistanceTo(player.Position) > 3) return;

                NAPI.Entity.SetEntityPosition(player,
                Checkpoints5[shape.GetData<int>("NUMBER")].Position + new Vector3(0, 0, 1.2));
                NAPI.Entity.SetEntityRotation(player,
                new Vector3(0, 0, Checkpoints5[shape.GetData<int>("NUMBER")].Heading));
                Main.OnAntiAnim(player);
                Trigger.ClientEvent(player, "VineOpenMenu2");
            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }

        [RemoteEvent("VineStopWork")]
        private static void VineStopWork(Player player, int count)
        {
            try
            {
                if (player != null && Main.Players.ContainsKey(player))
                {
                    player.StopAnimation();
                    Main.OffAntiAnim(player);
                    MoneySystem.Wallet.Change(player, 25);
                    var nextCheck = WorkManager.rnd.Next(0, Checkpoints5.Count - 1);
                    while (nextCheck == player.GetData<int>("WORKCHECK"))
                        nextCheck = WorkManager.rnd.Next(0, Checkpoints5.Count - 1);
                    player.SetData("WORKCHECK", nextCheck);
                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints5[nextCheck].Position, 1,
                        0, 255, 0, 0);
                    Trigger.ClientEvent(player, "createWorkBlip", Checkpoints5[nextCheck].Position);
                }
            }
            catch
            {
            }
        }

        private static List<Checkpoint1> Checkpoints5 = new List<Checkpoint1>()
        {
            new Checkpoint1(new Vector3(-1874.2904, 2098.728, 138.29985), -6.445362),
            new Checkpoint1(new Vector3(-1839.6919, 2111.5408, 134.07326), -4.262393),
            new Checkpoint1(new Vector3(-1808.171, 2126.512, 127.84249), 5.5008154),
            new Checkpoint1(new Vector3(-1777.2478, 2141.0803, 125.60736), -2.6065366),
            new Checkpoint1(new Vector3(-1759.1143, 2149.5188, 123.25698), 2.4354813),
            new Checkpoint1(new Vector3(-1766.8276, 2161.9795, 119.033516), 13.310221),
            new Checkpoint1(new Vector3(-1779.2219, 2157.8633, 118.75731), 7.798368),
            new Checkpoint1(new Vector3(-1811.8777, 2157.8958, 114.44343), 12.863322),
            new Checkpoint1(new Vector3(-1823.5317, 2144.5112, 117.9331), 26.259071),
            new Checkpoint1(new Vector3(-1855.8667, 2137.0884, 124.14029), 3.9538188),
            new Checkpoint1(new Vector3(-1889.1804, 2139.365, 122.877846), 19.234604),
            new Checkpoint1(new Vector3(-1889.5322, 2121.9312, 129.28151), -5.259341),
            new Checkpoint1(new Vector3(-1836.0989, 2067.7449, 136.14632), -132.86276),
            new Checkpoint1(new Vector3(-1832.0442, 2080.9233, 135.53561), -136.48958),
            new Checkpoint1(new Vector3(-1850.7804, 2090.5178, 138.6975), -151.42659),

        };
        internal class Checkpoint1
        {
            public Vector3 Position { get; }
            public double Heading { get; }

            public Checkpoint1(Vector3 pos, double rot)
            {
                Position = pos;
                Heading = rot;
            }
        }
    }
}
