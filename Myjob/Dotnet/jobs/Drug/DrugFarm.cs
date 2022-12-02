using GTANetworkAPI;
using System.Collections.Generic;
using System;
using Alyx.GUI;
using Alyx.Core;
using AlyxSDK;
using Alyx.Fractions;

namespace Alyx.Jobs
{
    class DrugFarm : Script
    {
        private static nLog Log = new nLog("DrugFarm");
        private static Random rnd = new Random();

        private static Dictionary<int, ColShape> Cols2 = new Dictionary<int, ColShape>();
        private static int _drugsPayment = 1;  //статичная цена
        private static int _drugsMultiplier;    //коэффициент, который умножается на _drugsPayment
        private static int _minMultiplier = 1; // минимальный коеффициент
        private static int _maxMultiplier = 2; // максимальный коеффициеннт

        private void cf2_onEntityEnterColShape(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", shape.GetData<int>("INTERACT"));
                UpdateLabel();
            }
            catch (Exception ex) { Log.Write("gp_onEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
        }
        private void cf2_onEntityExitColShape(ColShape shape, Player entity)
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
                NAPI.Blip.CreateBlip(469, new Vector3(-5.401763, -2541.6501, -11.05951), 0.9f, 2, Main.StringToU16("Сбор марихуаны"), 255, 0, true, 0, 0);

                NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_weed_01"), new Vector3(1063.7826, -3194.3408, -40.246037), new Vector3(0, 0, 92.086174), 255, 0);

                Cols2.Add(1, NAPI.ColShape.CreateCylinderColShape(new Vector3(-5.401763, -2541.6501, -11.05951) , 2, 2, 0)); // get clothes
                Cols2[1].OnEntityEnterColShape += cf2_onEntityEnterColShape;
                Cols2[1].OnEntityExitColShape += cf2_onEntityExitColShape;
                Cols2[1].SetData("INTERACT", 381);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Приступить собирать"), new Vector3(2931.998, 4624.349, 49) + new Vector3(0, 0, 1), 10F, 0.6F, 0, new Color(0, 0, 0, 0));
                label = NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"~w~Курс еще не известен."), new Vector3(2931.998, 4624.349, 48.6) + new Vector3(0, 0, 1), 10F, 0.6F, 0, new Color(0, 0, 0, 0));
                //NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Прораб Глеб"), new Vector3(-5.401763, -2541.6501, -9.63951) + new Vector3(0, 0, 1), 10F, 10F, 4, new Color(0, 180, 0));
                UpdateMultiplier();
                UpdateLabel();


                Cols2.Add(2, NAPI.ColShape.CreateCylinderColShape(new Vector3(-5.401763, -2541.6501, -15.05951), 2, 2, 0)); // drug seller
                Cols2[2].OnEntityEnterColShape += cf2_onEntityEnterColShape;
                Cols2[2].OnEntityExitColShape += cf2_onEntityExitColShape;
                Cols2[2].SetData("INTERACT", 382);
                int i = 0;
                foreach (var Check in Checkpoints2)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 1, 2, 0);
                    col.SetData("NUMBER", i);
                    col.OnEntityEnterColShape += PlayerEnterCheckpoint2;
                    i++;
                }

            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        public void UpdateLabel()
        {
            string text = $"~w~Курс {_drugsPayment * _drugsMultiplier} за 1 травку"; // если надо, тут меняем цену в TextLable
            label.Text = Main.StringToU16(text);

        }
        public static void UpdateMultiplier()
        {
            _drugsMultiplier = rnd.Next(_minMultiplier, _maxMultiplier);
            Log.Write($"Обновлен коэффициент на: {_drugsMultiplier}");
            
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
                var drugs = nInventory.Items[UUID].Find(t => t.Type == ItemType.Drugs);
                if (drugs != null)
                {
                    if (Main.Players[player].FractionID != 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вам нельзя сдавать травку", 3000);
                        return;
                    }
                    nInventory.Remove(player, drugs.Type, drugs.Count);
                    GUI.Dashboard.sendItems(player);
                    int payment = Convert.ToInt32((drugs.Count * _drugsPayment * _drugsMultiplier)); // количество * fix-price * коеффициент
                    MoneySystem.Wallet.Change(player, payment);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали {drugs.Count} травы  за {payment}$", 3000);
                }

                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы закончили рабочий день", 3000);
                return;
            }
            else
            {
                Customization.ClearClothes(player, Main.Players[player].Gender);
                if (Main.Players[player].Gender)
                {
                    player.SetClothes(1, 36, 0);
                    player.SetClothes(3, 0, 0);
                    player.SetClothes(11, 0, 0);
                    player.SetClothes(4, 90, 0);
                    player.SetClothes(6, 7, 0);
                }
                else
                {
                    player.SetClothes(1, 36, 0);
                    player.SetClothes(3, 2, 0);
                    player.SetClothes(11, 0, 3);
                    player.SetClothes(4, 25, 7);
                    player.SetClothes(6, 5, 0);
                }

                var check = WorkManager.rnd.Next(0, Checkpoints2.Count - 1);
                player.SetData("WORKCHECK", check);
                Trigger.ClientEvent (player, "createCheckpoint", 15, 1, Checkpoints2[check].Position, 107, 107, 250, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checkpoints2[check].Position);

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
                case 381:
                    try
                    {
                        if (!Main.Players.ContainsKey(client)) return;
                        StartWorkDay2(client);
                    }
                    catch (Exception e) { Log.Write("PlayerExitVehicle: " + e.Message, nLog.Type.Error); }
                    return;
                case 382:
                    try
                    {

                        if (!Main.Players.ContainsKey(client)) return;
                        int UUID = Main.Players[client].UUID;
                        var drugs= nInventory.Items[UUID].Find(t => t.Type == ItemType.Drugs);                      
                        if (drugs == null)
                        {
                            Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет травы", 3000);
                            return;
                        }
                        if (Main.Players[client].FractionID != 0)
                        {
                            Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, "Вам нельзя сдавать травку", 3000);
                            return;
                        }
                        nInventory.Remove(client, drugs.Type, drugs.Count);
                        GUI.Dashboard.sendItems(client);
                        int payment = Convert.ToInt32((drugs.Count * _drugsPayment * _drugsMultiplier)); // количество * fix-price * коеффициент
                        MoneySystem.Wallet.Change(client, payment);
                        Notify.Send(client, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали {drugs.Count} травы  за {payment}$", 3000);

                    }
                    catch (Exception e) { Log.Write("PlayerExitVehicle: " + e.Message, nLog.Type.Error); }
                    return;
            }

        }
        private static void PlayerEnterCheckpoint2(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!player.GetData<bool>("ON_WORK") ||
                    shape.GetData<int>("NUMBER") != player.GetData<int>("WORKCHECK")) return;

                if (Checkpoints2[(int)shape.GetData<int>("NUMBER")].Position.DistanceTo(player.Position) > 3) return;

                NAPI.Entity.SetEntityPosition(player,
                Checkpoints2[shape.GetData<int>("NUMBER")].Position + new Vector3(0, 0, 1.2));
                NAPI.Entity.SetEntityRotation(player,
                new Vector3(0, 0, Checkpoints2[shape.GetData<int>("NUMBER")].Heading));
                Main.OnAntiAnim(player);
                Trigger.ClientEvent(player, "DrugOpenMenu2");
            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }

        [RemoteEvent("DrugStopWork")]
        private static void DrugStopWork(Player player, int count)
        {
            try
            {
                if (player != null && Main.Players.ContainsKey(player))
                {
                    player.StopAnimation();
                    Main.OffAntiAnim(player);
                    MoneySystem.Wallet.Change(player, 16);
                    var nextCheck = WorkManager.rnd.Next(0, Checkpoints2.Count - 1);
                    while (nextCheck == player.GetData<int>("WORKCHECK"))
                        nextCheck = WorkManager.rnd.Next(0, Checkpoints2.Count - 1);
                    player.SetData("WORKCHECK", nextCheck);
                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints2[nextCheck].Position, 1,
                        0, 255, 0, 0);
                    Trigger.ClientEvent(player, "createWorkBlip", Checkpoints2[nextCheck].Position);
                }
            }
            catch
            {
            }
        }

        private static List<Checkpoint> Checkpoints2 = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(-5.8046355, -2519.5022, -11.059496), 38.674236),
            new Checkpoint(new Vector3(-4.9421096, -2524.9802, -11.059498), -129),
            new Checkpoint(new Vector3(-8.467393, -2526.4443, -11.058531), 137),
            new Checkpoint(new Vector3(-14.619127, -2524.8005, -11.059508), -132),
            new Checkpoint(new Vector3(-3.1400568, -2521.1826, -11.0595), 18),
            new Checkpoint(new Vector3(-3.9972138, -2528.3853, -11.05881), -35),
            new Checkpoint(new Vector3(-7.479072, -2530.8167, -11.059499), 46),
            new Checkpoint(new Vector3(-11.398244, -2524.2947, -11.059502), 136),
            new Checkpoint(new Vector3(-7.479072, -2530.8167, -11.059499), 46),
            new Checkpoint(new Vector3(-3.9972138, -2528.3853, -11.05881), -35),
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
