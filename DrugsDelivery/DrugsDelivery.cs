using GTANetworkAPI;
using RealLife.SDK;
using RealLife.GUI;
using SpaceMode.MoneySystem;
using System;
using System.Collections.Generic;

namespace RealLife.Core
{
    class DrugsDelivery : Script
    {
        private static ColShape shapes;
        private static Marker intmarker;
        private static Random rnd = new Random();
        private static Dictionary<int, ColShape> Cols = new Dictionary<int, ColShape>();
        private static nLog Log = new nLog("DrugsDelivery");
        #region ColShape
        private void DrugsDelivery_onEntityEnterColShape(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", shape.GetData<int>("INTERACT"));
            }
            catch (Exception ex) { Log.Write("DrugsDelivery_onEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
        }
        private void DrugsDelivery_onEntityExitColShape(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", 0);
            }
            catch (Exception ex) { Log.Write("DrugsDelivery_onEntityExitColShape: " + ex.Message, nLog.Type.Error); }
        }
        #endregion
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                #region #Creating Marker
                NAPI.Blip.CreateBlip(351, new Vector3(-891.38763, -2040.3411, 8.17914), 0.8f, 2, Main.StringToU16("Работа Закладчиком"), 255, 0, true, 0, 0);

                Cols.Add(0, NAPI.ColShape.CreateCylinderColShape(new Vector3(-891.38763, -2040.3411, 8.17914), 1, 2, 0));
                Cols[0].OnEntityEnterColShape += DrugsDelivery_onEntityEnterColShape;
                Cols[0].OnEntityExitColShape += DrugsDelivery_onEntityExitColShape;
                Cols[0].SetData("INTERACT", 1569);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Начать работу."), new Vector3(-891.38763, -2040.3411, 8.17914) + new Vector3(0, 0, 2), 10F, 0.6F, 0, new Color(0, 180, 0));
                NAPI.Marker.CreateMarker(1, new Vector3(-891.38763, -2040.3411, 8.17914) - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 0.6f, new Color(255, 255, 0));

                Cols.Add(1, NAPI.ColShape.CreateCylinderColShape(new Vector3(-887.5527, -2043.9156, 8.179147), 3, 4, 0));
                Cols[1].OnEntityEnterColShape += DrugsDelivery_onEntityEnterColShape;
                Cols[1].OnEntityExitColShape += DrugsDelivery_onEntityExitColShape;
                Cols[1].SetData("INTERACT", 1570);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Сдать закладки."), new Vector3(-887.5527, -2043.9156, 8.179147) + new Vector3(0, 0, 2), 10F, 0.6F, 0, new Color(0, 180, 0));
                NAPI.Marker.CreateMarker(1, new Vector3(-887.5527, -2043.9156, 8.179147) - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 0.6f, new Color(255, 255, 0));

                int i = 0;
                foreach (var Check in Checkpoints1)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 1, 2, 0);
                    col.SetData("NUMBER:DRUGS", i);
                    col.OnEntityEnterColShape += EnterCheckpoint;
                    i++;
                }
                #endregion
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }
        public static void interactPressed(Player player, int index)
        {
            switch (index)
            {
                case 1569:
                    try
                    {
                        if (!Main.Players.ContainsKey(player)) return;
                        if (Fractions.Manager.FractionTypes[Main.Players[player].FractionID] == 0 || Fractions.Manager.FractionTypes[Main.Players[player].FractionID] == 1)
                        {
                            if (player.HasData("WORK:DRUGSDELIVERY") && player.HasData("ON_WORK"))
                            {
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы закончили на работу.", 2000);
                                player.ResetData("WORK:DRUGSDELIVERY");
                                player.ResetData("ON_WORK");
                                Trigger.ClientEvent(player, "ARREA:DRUGS:DELIVERY", false, new Vector3());
                                int UUID = Main.Players[player].UUID;
                                var item = nInventory.Find(Main.Players[player].UUID, ItemType.Drugs);
                                if (item == null)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету закладок в инвентаре.", 2000);
                                    return;
                                }
                                else
                                {
                                    nInventory.Remove(player, ItemType.Drugs, item.Count);
                                    Dashboard.sendItems(player);
                                    int payment1 = 2500 * item.Count;
                                    Wallet.Change(player, +payment1);
                                    player.SendChatMessage($"~y~Из вашего инвентаря было продано - {item.Count} закладок на сумму {payment1}$");
                                    Log.Debug($"{Main.Players[player].PersonID} ({player.Name}) Продал из инвентаря {item.Count} закладок и получил за это {payment1}$.");
                                }
                                return;
                            }
                            player.SetData("WORK:DRUGSDELIVERY", true);
                            player.SetData("ON_WORK", true);
                            var check = rnd.Next(0, Checkpoints1.Count - 1);
                            player.SetData("WORKCHECKS", check);
                            var rand = rnd.Next(0, 5);
                            var rand2 = rnd.Next(0, 13);
                            Trigger.ClientEvent(player, "ARREA:DRUGS:DELIVERY", true, Checkpoints1[check].Position + new Vector3(rand2, rand, rand2));
                            intmarker = NAPI.Marker.CreateMarker(1, Checkpoints1[check].Position + new Vector3(0, 0, 0.1), new Vector3(), new Vector3(), 0.5f, new Color(255, 225, 64), false, 0);
                            shapes = NAPI.ColShape.CreateCylinderColShape(Checkpoints1[check].Position, 1, 2, 0);
                            shapes.OnEntityEnterColShape += EnterCheckpoint;
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы устроились на работу.", 2000);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны состоять в крим.организации.", 2000);
                            return;
                        }
                    }
                    catch (Exception e) { Log.Write("interactPressed: " + e.Message, nLog.Type.Error); }
                    return;
                case 1570:
                    try
                    {
                        if (!Main.Players.ContainsKey(player)) return;
                        int UUID = Main.Players[player].UUID;
                        var item = nInventory.Find(Main.Players[player].UUID, ItemType.Drugs);
                        if (item == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету закладок в инвентаре.", 2000);
                            return;
                        }
                        else
                        {
                            nInventory.Remove(player, ItemType.Drugs, item.Count);
                            Dashboard.sendItems(player);
                            int payment1 = 2500 * item.Count;
                            Wallet.Change(player, +payment1);
                            player.SendChatMessage($"~y~Из вашего инвентаря было продано - {item.Count} закладок на сумму {payment1}$");
                            Log.Debug($"{Main.Players[player].PersonID} ({player.Name}) Сдал из инвентаря {item.Count} закладок и получил за это {payment1}$.");
                        }
                    }
                    catch (Exception e) { Log.Write("interactPressed: " + e.Message, nLog.Type.Error); }
                    return;
            }
        }
        public static void EnterCheckpoint(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER:DRUGS") != player.GetData<int>("WORKCHECKS")) return;
                if (Checkpoints1[shape.GetData<int>("NUMBER:DRUGS")].Position.DistanceTo(player.Position) > 3) return;
                NAPI.Entity.SetEntityPosition(player, Checkpoints1[shape.GetData<int>("NUMBER:DRUGS")].Position + new Vector3(0, 0, 1.2));
                NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checkpoints1[shape.GetData<int>("NUMBER:DRUGS")].Heading));
                Main.OnAntiAnim(player);
                player.PlayAnimation("anim@heists@money_grab@briefcase", "put_down_case", 39);
                player.SetData("WORKCHECKS", -1);
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player != null && Main.Players.ContainsKey(player))
                        {
                            var tryAdd = nInventory.TryAdd(player, new nItem(ItemType.Drugs, 1));
                            if (tryAdd == -1 || tryAdd > 49)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
                                return;
                            }
                            else nInventory.Add(player, new nItem(ItemType.Drugs, 1, ""));
                            player.StopAnimation();
                            Main.OffAntiAnim(player);
                            var nextCheck = rnd.Next(0, Checkpoints1.Count - 1);
                            while (nextCheck == shape.GetData<int>("NUMBER:DRUGS")) nextCheck = rnd.Next(0, Checkpoints1.Count - 1);
                            player.SetData("WORKCHECKS", nextCheck);
                            Trigger.ClientEvent(player, "ARREA:DRUGS:DELIVERY", false, new Vector3());
                            var rand = rnd.Next(0, 5);
                            var rand2 = rnd.Next(3, 17);
                            if (intmarker != null) intmarker.Delete();
                            if (shapes != null) shapes.Delete();
                            Trigger.ClientEvent(player, "ARREA:DRUGS:DELIVERY", true, Checkpoints1[nextCheck].Position + new Vector3(rand2, rand, rand2));
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно нашли 1 закладку.", 3000);
                            intmarker = NAPI.Marker.CreateMarker(1, Checkpoints1[nextCheck].Position + new Vector3(0, 0, 0.1), new Vector3(), new Vector3(), 0.5f, new Color(255, 225, 64), false, 0);
                            shapes = NAPI.ColShape.CreateCylinderColShape(Checkpoints1[nextCheck].Position, 1, 2, 0);
                            shapes.OnEntityEnterColShape += EnterCheckpoint;
                        }
                    }
                    catch { }
                }, 3200);
                return;
            }
            catch (Exception e) { Log.Write("EnterCheckpoint: " + e.Message, nLog.Type.Error); }
        }
        #region Checks
        private static List<Checkpoint> Checkpoints1 = new List<Checkpoint>()
        {
           new Checkpoint(new Vector3(-19.876028, 6387.78, 30.232035), 47.18349),
           new Checkpoint(new Vector3(-52.412647, 6355.2227, 30.262545), 132.54846),
           new Checkpoint(new Vector3(-318.6114, 6082.0415, 30.192053), 109.96247),
           new Checkpoint(new Vector3(-397.97745, 6096.118, 30.334599), 25.010021),
           new Checkpoint(new Vector3(1686.3024, 4972.575, 41.571663), 44.58799),
           new Checkpoint(new Vector3(1720.7338, 4932.097, 40.972546), -115.59381),
           new Checkpoint(new Vector3(1703.2983, 4875.156, 40.912178), -94.296974),
           new Checkpoint(new Vector3(1659.9683, 4821.947, 40.880928), -169.29143),
           new Checkpoint(new Vector3(1946.9996, 4624.287, 39.41132), -55.444714),
           new Checkpoint(new Vector3(2151.219, 4781.273, 39.888306), 28.264427),
           new Checkpoint(new Vector3(2522.5957, 4210.6895, 38.81433), 61.942886),
           new Checkpoint(new Vector3(-28.051764, -1352.201, 28.197823), 3.885066),
           new Checkpoint(new Vector3(1925.4324, 3744.4744, 31.401514), -59.283524),
           new Checkpoint(new Vector3(1691.9681, 3750.086, 33.160534), -45.887775),
           new Checkpoint(new Vector3(1681.6857, 3571.1743, 34.3469), 26.740353),
           new Checkpoint(new Vector3(1386.4711, 3601.7134, 33.77481), -65.9241),
           new Checkpoint(new Vector3(1370.5839, 3609.4333, 33.773182), 103.74014),
           new Checkpoint(new Vector3(1176.5055, 2729.646, 36.884125), 0.12646778),
           new Checkpoint(new Vector3(272.2567, -1501.9948, 28.105417), 141.6012),
           new Checkpoint(new Vector3(158.01193, -1654.719, 28.171665), -152.3032),
           new Checkpoint(new Vector3(146.00888, -1676.9181, 28.514145), 132.46825),
           new Checkpoint(new Vector3(148.95178, -1728.771, 28.138906), -42.272408),
           new Checkpoint(new Vector3(229.92873, -1777.8984, 27.78912), 143.21695),
           new Checkpoint(new Vector3(241.461, -1770.0433, 27.659779), -37.37362),
           new Checkpoint(new Vector3(192.043, -1812.5264, 27.597878), -50.167763),
           new Checkpoint(new Vector3(230.87396, -1847.7163, 25.731964), 134.94916),
           new Checkpoint(new Vector3(355.11154, -1809.9098, 27.803137), 139.92815),
           new Checkpoint(new Vector3(453.0896, -1908.7472, 23.706032), -54.871742),
           new Checkpoint(new Vector3(375.00232, -1984.167, 23.024424), -12.427036),
           new Checkpoint(new Vector3(340.0521, -1974.0336, 23.156988), 143.9102),
           new Checkpoint(new Vector3(307.33585, -2002.1932, 19.735432), 41.44818),
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
        #endregion
    }
}