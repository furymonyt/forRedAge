using System;
using System.Collections.Generic;
using Alyx.Core;
using AlyxSDK;
using GTANetworkAPI;
using static Alyx.Core.Quests;

namespace Alyx.Jobs
{
    class Builder : Script
    {
        private static Vector3 pos = new Vector3(-169.25494, -1026.9185, 26.172077);
        public static nLog Log = new nLog("Builder");
        public static int JobPayment = 160;
        private static Vector3 _posElevator = new Vector3(-162.2582, -939.01935, 28.561915);
        private static Vector3 _posRoof = new Vector3(-154.2388, -942.58405, 269.13498);

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                NAPI.Blip.CreateBlip(566, pos, 0.8f, 4, Main.StringToU16("Строитель"), 255, 0, true, 0, 0);
                var shape = NAPI.ColShape.CreateCylinderColShape(pos, 1.2f, 2, 0); shape.OnEntityEnterColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 1010); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } }; shape.OnEntityExitColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 0); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } };
                var _shapeElevator = NAPI.ColShape.CreateCylinderColShape(_posElevator, 1.2f, 2, 0); _shapeElevator.OnEntityEnterColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 1011); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } }; _shapeElevator.OnEntityExitColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 0); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } };
                var _shapeRoof = NAPI.ColShape.CreateCylinderColShape(_posRoof, 1.2f, 2, 0); _shapeRoof.OnEntityEnterColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 1012); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } }; _shapeRoof.OnEntityExitColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 0); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } };

                NAPI.Marker.CreateMarker(0, _posElevator, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0.8f, new Color(67, 140, 239, 200), false, 0);
                NAPI.Marker.CreateMarker(0, _posRoof, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0.8f, new Color(67, 140, 239, 200), false, 0);
                int i = 0;
                foreach (var CheckStop in ChecksStop)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(CheckStop.Position, 2, 2, 0);
                    col.SetData("NUMBER2", i);
                    col.OnEntityEnterColShape += EnterShapePutContAndTakeNew;
                    col.OnEntityExitColShape += ExitColshape;
                    i++;
                }
                int to = 0;
                foreach (var Check in Checks)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 2, 2, 0);
                    col.SetData("NUMBER", to);
                    col.OnEntityEnterColShape += EnterShapeTakeNewCont;
                    col.OnEntityExitColShape += ExitColshape;
                    to++;
                }
                //int b = 0;
                /*foreach (var Check in Checks1)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 2, 2, 0);
                    col.SetData("NUMBER2", b);
                    col.OnEntityEnterColShape += EnterShapePutContAndTakeNew2;
                    col.OnEntityExitColShape += ExitColshape;
                    b++;
                }*/
               /* int d = 0;
                foreach (var Check in Checks2)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 2, 2, 0);
                    col.SetData("NUMBER2", d);
                    col.OnEntityEnterColShape += EnterShapePutContAndTakeNew3;
                    col.OnEntityExitColShape += ExitColshape;
                    d++;
                }*/
                /*  int d = 0;
                  foreach (var Check in Checks1)
                  {
                      var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 2, 2, 0);
                      col.SetData("NUMBER", d);
                      col.OnEntityEnterColShape += EnterShapeTakeNewCont;
                      col.OnEntityExitColShape += ExitColshape;
                      d++;
                  }*/
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
            if (Main.Players[player].WorkID != 9)
            {
                if (Main.Players[player].WorkID != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы уже работаете: {Jobs.WorkManager.JobStats[Main.Players[player].WorkID - 1]}", 3000);
                    return;
                }
                Main.Players[player].WorkID = 9;
                Notify.Succ(player, "Вы устроились на работу строителем");
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
                    if (Main.Players[player].WorkID == 9)
                    {
                        Main.Players[player].WorkID = 0;
                        Notify.Succ(player, "Вы уволились с работы строителя");
                    }
                }
            }
        }
        public static void OpenMenu(Player player)
        {
            if (player.IsInVehicle) return;
            {
                
                Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Грузчик", 503), new QuestAnswer("Электрик", 504), new QuestAnswer("Кровельщик", 505), new QuestAnswer("В следующий раз", 2)));
                return;
            }
        }
        public static void SetWorkState(Player player)
        {
            if (Main.Players[player].WorkID != 9)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не работаете строителем");
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
                    if (player.HasData("ShapeBuilderState") && player.GetData<bool>("ShapeBuilderState"))
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
                    var check = WorkManager.rnd.Next(0, Checks.Count - 1);
                    player.SetData("WORKCHECK", check);
                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks[check].Position, 1, 0, 255, 0, 0);
                    Trigger.ClientEvent(player, "createWorkBlip", Checks[check].Position);

                    player.SetData("ON_WORK", true);
                    player.SetSharedData("ON_WORK", true);
                    Notify.Succ(player, "Вы начали рабочий день строителя. Метка установлена на GPS");
                }
            }
        }
        public static void WorkFirstCont(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");
            player.SetData("ShapeBuilderState", true);
            player.SetSharedData("ShapeBuilderState", true);
            NAPI.Entity.SetEntityPosition(player, Checks[shape.GetData<int>("NUMBER")].Position + new Vector3(0, 0, 1.2));
            NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checks[shape.GetData<int>("NUMBER")].Heading));
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);

            Main.OnAntiAnim(player);

            player.PlayAnimation("anim@mp_snowball", "pickup_snowball", 1);
            player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
            NAPI.Task.Run(() =>
            {
                player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
                BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_feed_sack_01"), 26611, new Vector3(0, -0.3, 0.075), new Vector3(-45, 20, 120));
                NAPI.Task.Run(() =>
                {
                    player.StopAnimation();
                    player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
                    var nextCheck = WorkManager.rnd.Next(0, ChecksStop.Count - 1);
                    while (nextCheck == player.GetData<int>("WORKCHECK"))
                        nextCheck = WorkManager.rnd.Next(0, ChecksStop.Count - 1);
                    player.SetData("WORKCHECK", nextCheck);
                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, ChecksStop[nextCheck].Position, 1, 0, 255, 0, 0);
                    Trigger.ClientEvent(player, "createWorkBlip", ChecksStop[nextCheck].Position);
                }, 500);
            }, 1500);
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);
        }
        private static void EnterShapeTakeNewCont(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 9 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER") != player.GetData<int>("WORKCHECK")) return;
                if (player.HasData("ShapeBuilderState") && player.GetData<bool>("ShapeBuilderState")) return;

                player.SetData("INTERACTIONCHECK", 931);
                player.SetData("WorkColshape", shape);

            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }
        public static void WorkPutCont(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");

            player.SetData("ShapeBuilderState", false);
            player.SetSharedData("ShapeBuilderState", false);
            NAPI.Entity.SetEntityPosition(player, ChecksStop[shape.GetData<int>("NUMBER2")].Position + new Vector3(0, 0, 1.2));
            NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, ChecksStop[shape.GetData<int>("NUMBER2")].Heading));
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);
            Main.OnAntiAnim(player);

            player.PlayAnimation("anim@mp_snowball", "pickup_snowball", 1);
            player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
            NAPI.Task.Run(() =>
            {
                player.StopAnimation();
                BasicSync.DetachObject(player);
                MoneySystem.Wallet.Change(player, JobPayment);
                var nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                while (nextCheck == player.GetData<int>("WORKCHECK"))
                    nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                player.SetData("WORKCHECK", nextCheck);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks[nextCheck].Position, 1, 0, 255, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checks[nextCheck].Position);
                if (Main.Players[player].Achievements[2] == true && Main.Players[player].Achievements[3] == false)
                {
                    player.SetData("JobsBuilderQuestCount", player.GetData<int>("JobsBuilderQuestCount") + JobPayment);
                    Trigger.ClientEvent(player, "client::addToMissionsOnHud", true, "Первые деньги", 100, "Заработай на стройке 200 000$. Заработано: " + player.GetData<int>("JobsBuilderQuestCount") + "$ / 200 000$");
                    if (player.GetData<int>("JobsBuilderQuestCount") > 200000)
                    {
                        Main.Players[player].Achievements[3] = true;
                        Trigger.ClientEvent(player, "client::addToMissionsOnHud", true, "Первые деньги", 100, "Вернитесь к Гарри и поговорите с ним");
                    }
                }
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {JobPayment}$, следующая точка установлена", 3000);
            }, 2000);
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);
        }
        private static void EnterShapePutContAndTakeNew(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 9 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER2") != player.GetData<int>("WORKCHECK")) return;
                if (player.HasData("ShapeBuilderState") && !player.GetData<bool>("ShapeBuilderState")) return;

                player.SetData("INTERACTIONCHECK", 932);
                player.SetData("WorkColshape", shape);

            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }
        #region Elevator
        public static void TeleportToRoof(Player player)
        {
            if (player.IsInVehicle) return;
            if (player == null) return;
            Trigger.ClientEvent(player, "showHUD", false);
            NAPI.Task.Run(() => {
                try
                {
                    Trigger.ClientEvent(player, "screenFadeOut", 1000);
                }
                catch { }
            }, 100);
            NAPI.Task.Run(() => {
                try
                {
                    NAPI.Entity.SetEntityPosition(player, _posRoof);
                    NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, -115));
                    Trigger.ClientEvent(player, "screenFadeIn", 1000);
                    Trigger.ClientEvent(player, "showHUD", true);
                }
                catch { }
            }, 1600);
        }
        public static void TeleportToGround(Player player)
        {
            if (player.IsInVehicle) return;
            if (player == null) return;
            Trigger.ClientEvent(player, "showHUD", false);
            NAPI.Task.Run(() => {
                try
                {
                    Trigger.ClientEvent(player, "screenFadeOut", 1000);
                }
                catch { }
            }, 100);
            NAPI.Task.Run(() => {
                try
                {
                    NAPI.Entity.SetEntityPosition(player, _posElevator);
                    Trigger.ClientEvent(player, "screenFadeIn", 1000);
                    Trigger.ClientEvent(player, "showHUD", true);
                }
                catch { }
            }, 1600);
        }
        #endregion
        #region ClientEvents
        [RemoteEvent("serverplayerPlayAnimBuilder")]
        public static void serverplayerPlayAnimBuilder(Player player)
        {
            player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
        }
        [RemoteEvent("serverplayerstopboxBuilder")]
        public static void playerstopboxBuilder(Player player)
        {
            player.SetData("ShapeBuilderState", false);
            player.SetSharedData("ShapeBuilderState", false);
            BasicSync.DetachObject(player);
            player.PlayAnimation("rcmcollect_paperleadinout@", "kneeling_arrest_get_up", 33);
            var nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
            while (nextCheck == player.GetData<int>("WORKCHECK"))
                nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
            player.SetData("WORKCHECK", nextCheck);
            Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks[nextCheck].Position, 1, 0, 255, 0, 0);
            Trigger.ClientEvent(player, "createWorkBlip", Checks[nextCheck].Position);
            Notify.Error(player, "Вы уронили мешок, следующая точка установлена");
        }
        #endregion
        #region Constructor
        private static List<Points> Checks = new List<Points>()
        {
            new Points(new Vector3(-161.01372, -993.15546, 20.156858), -21.7),
            new Points(new Vector3(-145.89914, -957.91187, 20.156858), 67.7),
            new Points(new Vector3(-142.1013, -1006.4746, 26.15522), -24.9),
            new Points(new Vector3(-96.925735, -982.6265, 20.156841), 67.8),
            new Points(new Vector3(-114.475876, -1016.848, 25.368534), 74.6),
            new Points(new Vector3(-138.91292, -1033.9711, 38.219265), 70.5),
            new Points(new Vector3(-144.37712, -962.9675, 268.01532), 80),
            new Points(new Vector3(-155.00078, -953.5132, 268.01532), 0.2),
            new Points(new Vector3(-153.53087, -971.67285, 253.01138), 63.2),
        };
        private static List<Points> ChecksStop = new List<Points>()
        {
            new Points(new Vector3(-157.96039, -1081.371, 41.019257), 69.6),
            new Points(new Vector3(-160.6684, -1070.7599, 35.01933), 66.3),
            new Points(new Vector3(-157.95349, -1070.8453, 29.0194), 69.6),
            new Points(new Vector3(-161.37169, -1016.9021, 253.01138), 53.8),
            new Points(new Vector3(-145.91096, -1071.0165, 20.56524), 60.5),
            new Points(new Vector3(-138.88321, -1034.092, 38.219265), 65.7),
            new Points(new Vector3(-153.53748, -974.9936, 268.01532), 65.8),
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
        #endregion

     /*   #region electrik
        public static void SetWorkId2(Player player)
        {
            if (Main.Players[player].WorkID != 30)
            {
                if (Main.Players[player].WorkID != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы уже работаете: {Jobs.WorkManager.JobStats[Main.Players[player].WorkID - 1]}", 3000);
                    return;
                }
                Main.Players[player].WorkID = 30;
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
                    if (Main.Players[player].WorkID == 30)
                    {
                        Main.Players[player].WorkID = 0;
                        Notify.Succ(player, "Вы уволились с работы электрика");
                    }
                }
            }
        }
        public static void SetWorkState2(Player player)
        {
            if (Main.Players[player].WorkID != 30)
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
        public static void WorkFirstCont2(Player player)
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
                var nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                while (nextCheck == player.GetData<int>("WORKCHECK"))
                    nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                player.SetData("WORKCHECK", nextCheck);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks1[nextCheck].Position, 1, 0, 255, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checks1[nextCheck].Position);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {JobPayment}$, следующая точка установлена", 3000);
                player.SetData("ShapeelectState", true);
                player.SetSharedData("ShapeelectState", true);
            }, 1500);
        }
      
       private static void EnterShapePutContAndTakeNew2(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 30 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER2") != player.GetData<int>("WORKCHECK")) return;
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
        #endregion*/
       /* #region crov
        public static void SetWorkId3(Player player)
        {
            if (Main.Players[player].WorkID != 31)
            {
                if (Main.Players[player].WorkID != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы уже работаете: {Jobs.WorkManager.JobStats[Main.Players[player].WorkID - 1]}", 3000);
                    return;
                }
                Main.Players[player].WorkID = 31;
                Notify.Succ(player, "Вы устроились на работу кровельщика");
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
                    if (Main.Players[player].WorkID == 31)
                    {
                        Main.Players[player].WorkID = 0;
                        Notify.Succ(player, "Вы уволились с работы кровельщика");
                    }
                }
            }
        }
        public static void SetWorkState3(Player player)
        {
            if (Main.Players[player].WorkID != 31)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не работаете кровельщика");
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
                    if (player.HasData("ShapecrovState") && player.GetData<bool>("ShapecrovState"))
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
                    var check = WorkManager.rnd.Next(0, Checks2.Count - 1);
                    player.SetData("WORKCHECK", check);
                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks2[check].Position, 1, 0, 255, 0, 0);
                    Trigger.ClientEvent(player, "createWorkBlip", Checks2[check].Position);

                    player.SetData("ON_WORK", true);
                    player.SetSharedData("ON_WORK", true);
                    Notify.Succ(player, "Вы начали рабочий день кровельщика. Метка установлена на GPS");
                }
            }
        }
        public static void WorkFirstCont3(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");

            player.SetData("ShapecrovState", false);
            player.SetSharedData("ShapecrovState", false);
            NAPI.Entity.SetEntityPosition(player, Checks2[shape.GetData<int>("NUMBER2")].Position + new Vector3(0, 0, 1.2));
            NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checks2[shape.GetData<int>("NUMBER2")].Heading));
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);

            Main.OnAntiAnim(player);
            player.PlayAnimation("amb@world_human_hammering@male@base", "base", 39);
            BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_tool_hammer"), 57005, new Vector3(0.1, 0.0, 0.0), new Vector3(-90, 0, 0));
            NAPI.Task.Run(() =>
            {
                player.StopAnimation();
                BasicSync.DetachObject(player);
                MoneySystem.Wallet.Change(player, JobPayment + 4);
                var nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                while (nextCheck == player.GetData<int>("WORKCHECK"))
                    nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                player.SetData("WORKCHECK", nextCheck);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks2[nextCheck].Position, 1, 0, 255, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checks2[nextCheck].Position);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {JobPayment + 4}$, следующая точка установлена", 3000);
                player.SetData("ShapecrovState", true);
                player.SetSharedData("ShapecrovState", true);
            }, 1500);
        }
        /* private static void EnterShapeTakeNewCont2(ColShape shape, Player player)
         {
             try
             {
                 if (!Main.Players.ContainsKey(player)) return;
                 if (Main.Players[player].WorkID != 30 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER") != player.GetData<int>("WORKCHECK")) return;
                 if (player.HasData("ShapecrovState") && player.GetData<bool>("ShapecrovState")) return;

                 player.SetData("INTERACTIONCHECK", 2000);
                 player.SetData("WorkColshape", shape);

             }
             catch (Exception e)
             {
                 Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
             }
         }*/
     /*   private static void EnterShapePutContAndTakeNew3(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 31 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER2") != player.GetData<int>("WORKCHECK")) return;
                if (player.HasData("ShapecrovState") && !player.GetData<bool>("ShapecrovState")) return;

                player.SetData("INTERACTIONCHECK", 2001);
                player.SetData("WorkColshape", shape);

            }
            catch
            {
              
            }
        }
        private static List<Points> Checks2 = new List<Points>()
        {
            new Points(new Vector3(-145.48969, -942.96936, 268.01532), -22.98),
            new Points(new Vector3(-136.68193, -968.1325, 253.01138), -112.3),
            new Points(new Vector3(-162.80936, -1022.618, 38.21928), -149.85),
            new Points(new Vector3(-168.84093, -1015.7989, 20.1568), 158.1),
            new Points(new Vector3(-95.658745, -965.1276, 20.15684), -22),
            new Points(new Vector3(-191.75148, -1110.0709, 41.01656), 112),

        };
        #endregion*/
    }
}