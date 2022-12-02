/*using GTANetworkAPI;
using System.Collections.Generic;
using System;
using Alyx.GUI;
using Alyx.Core;
using AlyxSDK;
using Alyx.Houses;

namespace Alyx.Jobs
{
    class OFD : Script
    {
        private static nLog Log = new nLog("OFD");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Cols.Add(0, NAPI.ColShape.CreateCylinderColShape(Coords[0], 1, 2, 0)); // start work
                Cols[0].OnEntityEnterColShape += gp_onEntityEnterColShape;
                Cols[0].OnEntityExitColShape += gp_onEntityExitColShape;
                Cols[0].SetData("INTERACT", 1050);
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        private static int checkpointPayment = 110;

        public static List<Vector3> Coords = new List<Vector3>()
        {
            new Vector3(2310.567, 4884.895, 41.588263), // start work
        };
        private static Dictionary<int, ColShape> Cols = new Dictionary<int, ColShape>();
        private static Dictionary<int, ColShape> gCols = new Dictionary<int, ColShape>();
        // OFD items (objects) //
        public static List<uint> OFDObjects = new List<uint>
        {
            NAPI.Util.GetHashKey("prop_drug_package_02"),
        };

        [ServerEvent(Event.PlayerExitVehicle)]
        public void onPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 20) return;
                if (NAPI.Data.GetEntityData(player, "ON_WORK") && NAPI.Data.GetEntityData(player, "PACKAGES") != 0)
                {
                    int x = WorkManager.rnd.Next(0, OFDObjects.Count);
                    BasicSync.AttachObjectToPlayer(player, OFDObjects[x], 60309, new Vector3(0.03, 0, 0.02), new Vector3(0, 0, 50));
                }
            }
            catch (Exception e) { Log.Write("PlayerExitVehicle: " + e.Message, nLog.Type.Error); }
        }

        public static void Event_PlayerDeath(Player player, Player entityKiller, uint weapon)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID == 20 && NAPI.Data.GetEntityData(player, "ON_WORK"))
                {
                    player.SetData("ON_WORK", false);
                    player.SetData("PAYMENT", 0);
                    player.SetData("PACKAGES", 0);

                    Customization.ApplyCharacter(player);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                    Trigger.ClientEvent(player, "deleteCheckpoint", 1, 0);
                    NAPI.Task.Run(() => { BasicSync.DetachObject(player); }, 1000);
                    if (player.GetData<Vehicle>("WORK") != null)
                    {
                        NAPI.Entity.DeleteEntity(player.GetData<Vehicle>("WORK"));
                        player.SetData<Vehicle>("WORK", null);
                    }
                    Notify.Info(player, $"Вы закончили рабочий день", 3000);
                }
            }
            catch (Exception e) { Log.Write("PlayerDeath: " + e.Message, nLog.Type.Error); }
        }

        public static void OFD_onEntityEnterColShape(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 20 || !NAPI.Data.GetEntityData(player, "ON_WORK")) return;
                if (player.HasData("NEXTHOUSE") && player.HasData("HOUSEID") && NAPI.Data.GetEntityData(player, "NEXTHOUSE") == player.GetData<int>("HOUSEID"))
                {
                    if (NAPI.Player.IsPlayerInAnyVehicle(player))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Покиньте транспортное средство", 3000);
                        return;
                    }
                    if (player.GetData<int>("PACKAGES") == 0) return;
                    else if (player.GetData<int>("PACKAGES") > 1)
                    {
                        var coef = Convert.ToInt32(player.Position.DistanceTo2D(player.GetData<Vector3>("W_LASTPOS")) / 100);
                        DateTime lastTime = player.GetData<DateTime>("W_LASTTIME");
                        if (DateTime.Now < lastTime.AddSeconds(coef * 2))
                        {
                            Notify.Error(player, "Хозяина нет дома, попробуйте позже", 3000);
                            return;
                        }
                        player.SetData("PACKAGES", player.GetData<int>("PACKAGES") - 1);
                        Notify.Info(player, $"У Вас осталось {player.GetData<int>("PACKAGES")} ящиков", 3000);

                        var payment = Convert.ToInt32(coef * checkpointPayment * Group.GroupPayAdd[Main.Accounts[player].VipLvl] * Main.oldconfig.PaydayMultiplier);
                        //надбавка с учетом уровня игрока на данной работе
                        //добавление опыта
                        if (Main.Players[player].AddExpForWork(Main.oldconfig.PaydayMultiplier))
                            Notify.Alert(player, $"Поздравляем с повышением уровня! Текущий уровень теперь: {Main.Players[player].GetLevelAtThisWork()}");
                        MoneySystem.Wallet.Change(player, payment);
                        GameLog.Money($"server", $"player({Main.Players[player].UUID})", payment, $"OFDCheck");

                        BasicSync.DetachObject(player);

                        var nextHouse = player.GetData<object>("NEXTHOUSE");
                        var next = -1;
                        do
                        {
                            next = WorkManager.rnd.Next(0, HouseManager.Houses.Count - 1);
                        }
                        while (Houses.HouseManager.Houses[next].Position.DistanceTo2D(player.Position) < 200);
                        player.SetData("W_LASTPOS", player.Position);
                        player.SetData("W_LASTTIME", DateTime.Now);
                        player.SetData("NEXTHOUSE", HouseManager.Houses[next].ID);

                        Trigger.ClientEvent(player, "createCheckpoint", 1, 1, HouseManager.Houses[next].Position, 1, 0, 255, 0, 0);
                        Trigger.ClientEvent(player, "createWaypoint", HouseManager.Houses[next].Position.X, HouseManager.Houses[next].Position.Y);
                        Trigger.ClientEvent(player, "createWorkBlip", HouseManager.Houses[next].Position);
                        NAPI.Player.PlayPlayerAnimation(player, -1, "anim@heists@narcotics@trash", "drop_side");
                    }
                    else
                    {
                        var coef = Convert.ToInt32(player.Position.DistanceTo2D(player.GetData<Vector3>("W_LASTPOS")) / 100);
                        DateTime lastTime = player.GetData<DateTime>("W_LASTTIME");
                        if (DateTime.Now < lastTime.AddSeconds(coef * 2))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Хозяина нет дома, попробуйте позже", 3000);
                            return;
                        }

                        var payment = Convert.ToInt32(coef * checkpointPayment * Group.GroupPayAdd[Main.Accounts[player].VipLvl]);
                        //надбавка с учетом уровня игрока на данной работе
                        payment += Jobs.WorkManager.PaymentIncreaseAmount[Main.Players[player].WorkID] * Main.Players[player].GetLevelAtThisWork();
                        //добавление опыта
                        if (Main.Players[player].AddExpForWork(Main.oldconfig.PaydayMultiplier))
                            Notify.Alert(player, $"Поздравляем с повышением уровня! Текущий уровень теперь: {Main.Players[player].GetLevelAtThisWork()}");
                        MoneySystem.Wallet.Change(player, payment);
                        GameLog.Money($"server", $"player({Main.Players[player].UUID})", payment, $"OFDCheck");

                        Trigger.ClientEvent(player, "deleteWorkBlip");
                        Trigger.ClientEvent(player, "createWaypoint", 105.4633f, -1568.843f);

                        BasicSync.DetachObject(player);

                        Trigger.ClientEvent(player, "deleteCheckpoint", 1, 0);
                        NAPI.Player.PlayPlayerAnimation(player, -1, "anim@heists@narcotics@trash", "drop_side");
                        player.SetData("PACKAGES", 0);
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У Вас не осталось посылок, возьмите новые", 3000);
                    }
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"OFD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        private void gp_onEntityEnterColShape(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", shape.GetData<int>("INTERACT"));
            }
            catch (Exception ex) { Log.Write("gp_onEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
        }
        private void gp_onEntityExitColShape(ColShape shape, Player entity)
        {
            try
            {
                NAPI.Data.SetEntityData(entity, "INTERACTIONCHECK", 0);
            }
            catch (Exception ex) { Log.Write("gp_onEntityExitColShape: " + ex.Message, nLog.Type.Error); }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatid)
        {
            try
            {
                BasicSync.DetachObject(player);
            }
            catch (Exception e) { Log.Write("PlayerEnterVehicle: " + e.Message, nLog.Type.Error); }
        }
        public static Vector3[] OFDPosition = new Vector3[]
        {
            new Vector3(2296.7666, 4897.756, 40.072327),  //1
            new Vector3(2301.985, 4903.155, 40.189915),     //2
            new Vector3(2309.2634, 4910.146, 40.15761),          //3
            new Vector3(2314.5815, 4916.5405, 40.18334),            //4
            new Vector3(2321.1204, 4923.359, 40.312008),               //6
        };
        public static Vector3[] OFDRostation = new Vector3[]
        {
            new Vector3(0, 0, -128.14732),  //1
            new Vector3(0, 0, -139.9789),     //2
            new Vector3(0, 0, -130.84595),          //3
            new Vector3(0, 0, -127.580086),            //4
            new Vector3(0, 0, -127.992615),               //6
       };
        public static void getOrangeCar(Player player)
        {
            if (Main.Players[player].WorkID != 20)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не работаете доставщиком фруктов", 3000);
                return;
            }
            if (!player.GetData<bool>("ON_WORK"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны начать рабочий день", 3000);
                return;
            }
            if (player.GetData<Vehicle>("WORK") != null)
            {
                NAPI.Entity.DeleteEntity(player.GetData<Vehicle>("WORK"));
                player.SetData<Vehicle>("WORK", null);
                return;
            }
            var rnd = new Random().Next(0, OFDPosition.Length);
            var veh = API.Shared.CreateVehicle(VehicleHash.Tiptruck, OFDPosition[rnd], OFDRostation[rnd], 134, 77, "Orange");
            player.SetData("WORK", veh);
            player.SetIntoVehicle(veh, 0);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили рабочий транспорт", 3000);
            veh.SetData("ACCESS", "WORK");
            Core.VehicleStreaming.SetEngineState(veh, true);
        }
        #region BOFD
        public static void WorkFirstCont(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");
            player.SetData("ShapeOFDState", true);
            player.SetSharedData("ShapeOFDState", true);
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
                BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_cardbordbox_04a"), 26611, new Vector3(0, -0.3, 0.075), new Vector3(-45, 20, 120));
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
        public static void WorkPutCont(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");

            player.SetData("ShapeOFDState", false);
            player.SetSharedData("ShapeOFDState", false);
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
                MoneySystem.Wallet.Change(player, checkpointPayment);
                var nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                while (nextCheck == player.GetData<int>("WORKCHECK"))
                    nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
                player.SetData("WORKCHECK", nextCheck);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks[nextCheck].Position, 1, 0, 255, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checks[nextCheck].Position);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {checkpointPayment}$, следующая точка установлена", 3000);
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
                if (player.HasData("ShapeOFDState") && !player.GetData<bool>("ShapeOFDState")) return;

                player.SetData("INTERACTIONCHECK", 932);
                player.SetData("WorkColshape", shape);

            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }
        #endregion
        #region Serverevent
        [RemoteEvent("serverplayerPlayAnimOFD")]
        public static void serverplayerPlayAnimOFD(Player player)
        {
            player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
        }
        [RemoteEvent("serverplayerstopboxOFD")]
        public static void playerstopboxOFD(Player player)
        {
            player.SetData("ShapeOFDState", false);
            player.SetSharedData("ShapeOFDState", false);
            BasicSync.DetachObject(player);
            player.PlayAnimation("rcmcollect_paperleadinout@", "kneeling_arrest_get_up", 33);
            var nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
            while (nextCheck == player.GetData<int>("WORKCHECK"))
                nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
            player.SetData("WORKCHECK", nextCheck);
            Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks[nextCheck].Position, 1, 0, 255, 0, 0);
            Trigger.ClientEvent(player, "createWorkBlip", Checks[nextCheck].Position);
            Notify.Error(player, "Вы уронили ящик, следующая точка установлена");
        }
#endregion
        #region Constructor
        private static List<Points> Checks = new List<Points>()
        {
            new Points(new Vector3(-154.47127, -1081.0221, 20.56526), -102),
            new Points(new Vector3(-154.85541, -1080.6384, 29.019403), -103),
            new Points(new Vector3(-169.78923, -1064.0565, 29.019417), -109),
            new Points(new Vector3(-176.9737, -1073.2231, 29.019411), 175),
            new Points(new Vector3(-162.01611, -1075.9753, 35.019356), 66),
            new Points(new Vector3(-176.87253, -1073.3542, 35.019363), 173),
            new Points(new Vector3(-155.9621, -1081.8391, 41.01925), -107),
            new Points(new Vector3(-169.38747, -1049.082, 41.01925), 72),
            new Points(new Vector3(-97.54516, -965.6552, 20.156845), 79),
        };
        private static List<Points> ChecksStop = new List<Points>()
        {
            new Points(new Vector3(-141.82927, -1066.7698, 20.565266), -23),
            new Points(new Vector3(-170.1911, -1071.4585, 29.019405), 157),
            new Points(new Vector3(-178.41185, -1099.4235, 29.019419), -1),
            new Points(new Vector3(-154.96272, -1080.6786, 35.019363), -117),
            new Points(new Vector3(-169.83168, -1064.1018, 35.01936), -109),
            new Points(new Vector3(-184.85513, -1072.8644, 41.049744), 74),
            new Points(new Vector3(-160.83797, -1051.9121, 41.01927), 151),
            new Points(new Vector3(-167.72087, -1099.1355, 41.019363), -14),
            new Points(new Vector3(-163.58441, -985.6222, 20.156843), 56),
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
    }
}
*/
////////////////////////////////////////////////////////using System;
/*using System.Collections.Generic;
using Alyx.Core;
using AlyxSDK;
using GTANetworkAPI;
using static Alyx.Core.Quests;

namespace Alyx.Jobs
{
    class OFD : Script
    {
        private static Vector3 pos = new Vector3(2310.3381, 4884.656, 41.688232);
        public static nLog Log = new nLog("OFD");
        public static int JobPayment = 23;

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                NAPI.Blip.CreateBlip(568, pos, 0.8f, 4, Main.StringToU16("Грузчик Апельсинов"), 255, 0, true, 0, 0);
                var shape = NAPI.ColShape.CreateCylinderColShape(pos, 1.2f, 2, 0); shape.OnEntityEnterColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 1050); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } }; shape.OnEntityExitColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 0); } catch (Exception e) { Log.Write(e.ToString(), nLog.Type.Error); } };

                int i = 0;
                foreach (var CheckStop in ChecksStop)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(CheckStop.Position, 2, 2, 0);
                    col.SetData("NUMBER2", i);
                    col.OnEntityEnterColShape += EnterShapePutContAndTakeNew1;
                    col.OnEntityExitColShape += ExitColshape1;
                    i++;
                }
                int to = 0;
                foreach (var Check in Checks)
                {
                    var col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 2, 2, 0);
                    col.SetData("NUMBER", to);
                    col.OnEntityEnterColShape += EnterShapeTakeNewCont1;
                    col.OnEntityExitColShape += ExitColshape1;
                    to++;
                }
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }
        public static void ExitColshape1(ColShape shape, Player player)
        {
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);
        }
        public static void SetWorkId(Player player)
        {
            if (Main.Players[player].WorkID != 20)
            {
                if (Main.Players[player].WorkID != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomLeft, $"Вы уже работаете: {Jobs.WorkManager.JobStats[Main.Players[player].WorkID - 1]}", 3000);
                    return;
                }
                Main.Players[player].WorkID = 20;
                Notify.Succ(player, "Вы устроились на работу грузчика");
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
                    if (Main.Players[player].WorkID == 20)
                    {
                        Main.Players[player].WorkID = 0;
                        Notify.Succ(player, "Вы уволились с работы грузчика");
                    }
                }
            }
        }
        public static void OpenMenu(Player player)
        {
            if (Main.Players[player].WorkID != 20)
            {
                Trigger.ClientEvent(player, "client::opendialogmenu", true, "Антонио", "Прораб", "Привет, не хочешь заработать немного зеленых, нужно всего лишь таскать ящики с фруктами", (new QuestAnswer("Как тут работать?", 151), new QuestAnswer("Устроиться", 150), new QuestAnswer("В следующий раз", 2)));
                return;
            }
            else if (Main.Players[player].WorkID == 20 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == false)
            {
                Trigger.ClientEvent(player, "client::opendialogmenu", true, "Антонио", "Прораб", "Привет, не хочешь заработать немного зеленых, нужно всего лишь таскать ящики с фруктами", (new QuestAnswer("Как тут работать?", 151), new QuestAnswer("Уволиться", 150), new QuestAnswer("Начать рабочий день", 152), new QuestAnswer("В следующий раз", 2)));
                return;
            }
            else if (Main.Players[player].WorkID == 20 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == true)
            {
                Trigger.ClientEvent(player, "client::opendialogmenu", true, "Антонио", "Прораб", "Привет, не хочешь заработать немного зеленых, нужно всего лишь таскать ящики с фруктами", (new QuestAnswer("Как тут работать?", 151), new QuestAnswer("Уволиться", 150), new QuestAnswer("Закончить день", 152), new QuestAnswer("В следующий раз", 2)));
                return;
            }
        }
        public static void SetWorkState1(Player player)
        {
            if (Main.Players[player].WorkID != 20)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не работаете грузчиком");
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
                    if (player.HasData("ShapeOFDState") && player.GetData<bool>("ShapeOFDState"))
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
                        player.SetClothes(8, 59, 0);
                        player.SetClothes(11, 1, 0);
                        player.SetClothes(4, 0, 5);
                        player.SetClothes(6, 48, 0);
                    }
                    else
                    {
                        player.SetClothes(8, 36, 0);
                        player.SetClothes(11, 0, 0);
                        player.SetClothes(4, 1, 5);
                        player.SetClothes(6, 49, 0);
                    }
                    var check = WorkManager.rnd.Next(0, Checks.Count - 1);
                    player.SetData("WORKCHECK", check);
                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks[check].Position, 1, 0, 255, 0, 0);
                    Trigger.ClientEvent(player, "createWorkBlip", Checks[check].Position);

                    player.SetData("ON_WORK", true);
                    player.SetSharedData("ON_WORK", true);
                    Notify.Succ(player, "Вы начали рабочий день грузчика. Метка установлена на GPS");
                }
            }
        }
        public static void WorkFirstCont1(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");
            player.SetData("ShapeOFDState", true);
            player.SetSharedData("ShapeOFDState", true);
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
                BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_paper_box_03"), 26611, new Vector3(0, -0.3, 0.075), new Vector3(-45, 20, 120));
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
        private static void EnterShapeTakeNewCont1(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 20 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER") != player.GetData<int>("WORKCHECK")) return;
                if (player.HasData("ShapeOFDState") && player.GetData<bool>("ShapeOFDState")) return;

                player.SetData("INTERACTIONCHECK", 1051);
                player.SetData("WorkColshape", shape);

            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }
        public static void WorkPutCont1(Player player)
        {
            var shape = player.GetData<ColShape>("WorkColshape");

            player.SetData("ShapeOFDState", false);
            player.SetSharedData("ShapeOFDState", false);
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
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {JobPayment}$, следующая точка установлена", 3000);
            }, 2000);
            player.SetData("INTERACTIONCHECK", -1);
            player.SetData("WorkColshape", -1);
        }
        private static void EnterShapePutContAndTakeNew1(ColShape shape, Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != 20 || !player.GetData<bool>("ON_WORK") || shape.GetData<int>("NUMBER2") != player.GetData<int>("WORKCHECK")) return;
                if (player.HasData("ShapeOFDState") && !player.GetData<bool>("ShapeOFDState")) return;

                player.SetData("INTERACTIONCHECK", 1052);
                player.SetData("WorkColshape", shape);

            }
            catch (Exception e)
            {
                Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error);
            }
        }
        #region ClientEvents
        [RemoteEvent("serverplayerPlayAnimOFD")]
        public static void serverplayerPlayAnimOFD(Player player)
        {
            player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
        }
        [RemoteEvent("serverplayerstopboxOFD")]
        public static void playerstopboxOFD(Player player)
        {
            player.SetData("ShapeOFDState", false);
            player.SetSharedData("ShapeOFDState", false);
            BasicSync.DetachObject(player);
            player.PlayAnimation("rcmcollect_paperleadinout@", "kneeling_arrest_get_up", 33);
            var nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
            while (nextCheck == player.GetData<int>("WORKCHECK"))
                nextCheck = WorkManager.rnd.Next(0, Checks.Count - 1);
            player.SetData("WORKCHECK", nextCheck);
            Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checks[nextCheck].Position, 1, 0, 255, 0, 0);
            Trigger.ClientEvent(player, "createWorkBlip", Checks[nextCheck].Position);
            Notify.Error(player, "Вы уронили ящик, следующая точка установлена");
        }
        #endregion
        #region Constructor
        private static List<Points> Checks = new List<Points>()
        {
            new Points(new Vector3(2294.4856, 4878.0835, 40.888232), 150),
            new Points(new Vector3(2294.9932, 4863.723, 40.888232), -8),
            new Points(new Vector3(2348.7117, 4869.079, 40.888194), 70),
            new Points(new Vector3(2337.843, 4889.992, 40.902667), -26),
        };
        private static List<Points> ChecksStop = new List<Points>()
        {
            new Points(new Vector3(2287.008, 4961.832, 40.447585), 34.5),
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
    }
}*/