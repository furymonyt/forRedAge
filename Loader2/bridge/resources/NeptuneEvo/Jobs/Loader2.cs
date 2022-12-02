using GTANetworkAPI;
using System.Collections.Generic;
using System;
using NeptuneEvo.GUI;
using NeptuneEvo.Core;
using Redage.SDK;

namespace NeptuneEvo.Jobs
{
    class Loader2 : Script
    {
        private static int checkpointPayment = 157;
        private static int JobWorkId = 10;
        private static int JobsMinLVL = 2;
        private static int RentCarMoney = 1;
        private static nLog Log = new nLog("Loader2");

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                NAPI.Blip.CreateBlip(615, new Vector3(1196.187, -3253.669, 5.975182), 1.5f, 46, Main.StringToU16("Грузчик2"), 255, 0, true, 0, 0); // Блип на карте
                NAPI.TextLabel.CreateTextLabel("~w~Приму вас на работу", new Vector3(1196.747, -3253.617, 8.30), 30f, 0.3f, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension); // Над головой у бота
                NAPI.Marker.CreateMarker(1, new Vector3(1196.187, -3253.669, 5.975182) - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220)); //Начать рабочий день маркер
                var col = NAPI.ColShape.CreateCylinderColShape(new Vector3(1196.187, -3253.669, 5.975182), 1, 2, 0); // Меню которое открывается на 'E'

                col.OnEntityEnterColShape += (shape, player) => {
                    try
                    {
                        player.SetData("INTERACTIONCHECK", 507);
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
                    col = NAPI.ColShape.CreateCylinderColShape(Check.Position, 1, 2, 0);
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
            new Checkpoint(new Vector3(1081.916, -3267.611, 4.780363), 271.3802), // Загрузить машину 1
            new Checkpoint(new Vector3(1041.326, -3280.458, 4.767561), 279.2969), // Загрузить машину 2
            new Checkpoint(new Vector3(1084.706, -3293.911, 4.768104), 279.0769), // Загрузить машину 3
            new Checkpoint(new Vector3(1029.333, -3263.089, 4.776977), 187.9965), // Загрузить машину 4
            new Checkpoint(new Vector3(1038.317, -3244.529, 4.774917), 229.1119), // Загрузить машину 5
        };
        private static List<Checkpoint> Checkpoints2 = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(1179.362, -3263.06, 4.538577), 268.4699), // Разгрузить машину 1
            new Checkpoint(new Vector3(1219.559, -3203.649, 4.447934), 348.4007), // Разгрузить машину 2
            new Checkpoint(new Vector3(1233.267, -3231.412, 4.505898), 2.297199), // Разгрузить машину 3
            new Checkpoint(new Vector3(1179.837, -3297.958, 4.50528), 263.9995), // Разгрузить машину 4
        };
        #endregion
        
        #region Спавн машины и респавн машины
        public static List<CarInfo> CarInfos = new List<CarInfo>();
        public static void loader2CarsSpawner()
        {
            for (int a = 0; a < CarInfos.Count; a++)
            {
                var veh = NAPI.Vehicle.CreateVehicle(CarInfos[a].Model, CarInfos[a].Position, CarInfos[a].Rotation.Z, CarInfos[a].Color1, CarInfos[a].Color2, CarInfos[a].Number);
                NAPI.Data.SetEntityData(veh, "ACCESS", "WORK");
                NAPI.Data.SetEntityData(veh, "WORK", 10);
                NAPI.Data.SetEntityData(veh, "TYPE", "LOADER2");
                NAPI.Data.SetEntityData(veh, "NUMBER", a);
                NAPI.Data.SetEntityData(veh, "ON_WORK", false);
                NAPI.Data.SetEntityData(veh, "DRIVER", null);
                veh.SetSharedData("PETROL", VehicleManager.VehicleTank[veh.Class]);
                Core.VehicleStreaming.SetEngineState(veh, false);
                Core.VehicleStreaming.SetLockStatus(veh, false);
            }
        }
        public static void respawnCar(Vehicle veh)
        {
            try
            {
                int i = NAPI.Data.GetEntityData(veh, "NUMBER");
                NAPI.Entity.SetEntityPosition(veh, CarInfos[i].Position);
                NAPI.Entity.SetEntityRotation(veh, CarInfos[i].Rotation);
                VehicleManager.RepairCar(veh);
                NAPI.Data.SetEntityData(veh, "ACCESS", "WORK");
                NAPI.Data.SetEntityData(veh, "WORK", 10);
                NAPI.Data.SetEntityData(veh, "TYPE", "LOADER2");
                NAPI.Data.SetEntityData(veh, "NUMBER", i);
                NAPI.Data.SetEntityData(veh, "ON_WORK", false);
                NAPI.Data.SetEntityData(veh, "DRIVER", null);
                Core.VehicleStreaming.SetEngineState(veh, false);
                Core.VehicleStreaming.SetLockStatus(veh, false);
                veh.SetSharedData("PETROL", VehicleManager.VehicleTank[veh.Class]);
            }
            catch (Exception e) { Log.Write("respawnCar: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Когда содишься в машину
        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicleHandler(Client player, Vehicle vehicle, sbyte seatid)
        {
            try
            {
                if (NAPI.Data.GetEntityData(vehicle, "TYPE") != "LOADER2" || player.VehicleSeat != -1) return;
                if (Main.Players[player].WorkID == JobWorkId)
                {
                    if (player.HasData("WORKOBJECT"))
                    {
                        BasicSync.DetachObject(player);
                        player.ResetData("WORKOBJECT");
                    }
                    if (!NAPI.Data.GetEntityData(vehicle, "ON_WORK"))
                    {
                        if (NAPI.Data.GetEntityData(player, "WORK") == null)
                        {
                            if (Main.Players[player].Money >= 100) Trigger.ClientEvent(player, "openDialog", "LOADER2_RENT", $"Аренда: {RentCarMoney}$");
                            else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас не хватает " + (100 - Main.Players[player].Money) + "$ на аренду машины", 3000);
                                VehicleManager.WarpPlayerOutOfVehicle(player);
                            }
                        }
                        else if (NAPI.Data.GetEntityData(player, "WORK") == vehicle)
                            NAPI.Data.SetEntityData(player, "IN_WORK_CAR", true);
                    }
                    else
                    {
                        if (NAPI.Data.GetEntityData(player, "WORK") != vehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Эта машина занята", 3000);
                            VehicleManager.WarpPlayerOutOfVehicle(player);
                        }
                        else NAPI.Data.SetEntityData(player, "IN_WORK_CAR", true);
                    }
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не работаете грузчиком.", 3000);
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                }
            }
            catch (Exception e) { Log.Write("PlayerEnterVehicle: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Когда выходишь с машины
        [ServerEvent(Event.PlayerExitVehicle)]
        public void onPlayerExitVehicleHandler(Client player, Vehicle vehicle)
        {
            try
            {
                if (NAPI.Data.GetEntityData(vehicle, "TYPE") == "LOADER2" &&
                Main.Players[player].WorkID == JobWorkId &&
                NAPI.Data.GetEntityData(player, "ON_WORK") &&
                NAPI.Data.GetEntityData(player, "WORK") == vehicle)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Если Вы не сядете в транспорт через 30 секунд, то рабочий день закончится", 3000);
                    NAPI.Data.SetEntityData(player, "IN_WORK_CAR", false);
                    if (player.HasData("WORK_CAR_EXIT_TIMER"))
                        //Main.StopT(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"), "timer_13");
                        Timers.Stop(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"));
                    NAPI.Data.SetEntityData(player, "CAR_EXIT_TIMER_COUNT", 0);
                    //NAPI.Data.SetEntityData(player, "WORK_CAR_EXIT_TIMER", Main.StartT(1000, 1000, (o) => timer_playerExitWorkVehicle(player, vehicle), "COL_EXIT_CAR_TIMER"));
                    NAPI.Data.SetEntityData(player, "WORK_CAR_EXIT_TIMER", Timers.StartTask(1000, () => timer_playerExitWorkVehicle(player, vehicle)));
                }
            }
            catch (Exception e) { Log.Write("PlayerExitVehicle: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Если ты умираешь то идёт следующее
        public static void Event_PlayerDeath(Client player, Client entityKiller, uint weapon)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID == JobWorkId && player.GetData("ON_WORK"))
                {
                    player.StopAnimation();
                    var vehicle = player.GetData("WORK");

                    respawnCar(vehicle);

                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы закончили рабочий день", 3000);
                    Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                    Trigger.ClientEvent(player, "deleteWorkBlip");
                    Trigger.ClientEvent(player, "CloseJobStatsInfo");

                    MoneySystem.Wallet.Change(player, player.GetData("PAYMENT"));
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"+ {player.GetData("PAYMENT")}$", 3000);
                    NAPI.Data.SetEntityData(player, "PAYMENT", 0);

                    NAPI.Data.SetEntityData(player, "ON_WORK", false);
                    NAPI.Data.SetEntityData(player, "WORK", null);
                    Customization.ApplyCharacter(player);
                    if (player.HasData("WORK_CAR_EXIT_TIMER"))
                    {
                        //Main.StopT(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"), "timer_14");
                        Timers.Stop(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"));
                        NAPI.Data.ResetEntityData(player, "WORK_CAR_EXIT_TIMER");
                    }
                }
                if (player.HasData("WORKOBJECT"))
                {
                    BasicSync.DetachObject(player);
                    player.ResetData("WORKOBJECT");
                }
            }
            catch (Exception e) { Log.Write("PlayerDeath: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Если выкинуло из игры или игрок вышел
        public static void Event_PlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            try
            {
                if (Main.Players[player].WorkID == JobWorkId && player.GetData("ON_WORK"))
                {
                    player.StopAnimation();
                    var vehicle = player.GetData("WORK");

                    respawnCar(vehicle);
                }
                if (player.HasData("WORKOBJECT"))
                {
                    BasicSync.DetachObject(player);
                    player.ResetData("WORKOBJECT");
                }
            }
            catch (Exception e) { Log.Write("PlayerDisconnected: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Когда вышел из машины и время закончилось срабатывает конец рабочего дня
        private void timer_playerExitWorkVehicle(Client player, Vehicle vehicle)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    if (!player.HasData("WORK_CAR_EXIT_TIMER")) return;
                    if (NAPI.Data.GetEntityData(player, "IN_WORK_CAR"))
                    {
                        //                    Main.StopT(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"), "timer_16");
                        Timers.Stop(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"));
                        NAPI.Data.ResetEntityData(player, "WORK_CAR_EXIT_TIMER");
                        Log.Debug("Player exit work vehicle timer was stoped");
                        return;
                    }
                    if (NAPI.Data.GetEntityData(player, "CAR_EXIT_TIMER_COUNT") > 30)
                    {
                        respawnCar(vehicle);

                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы закончили рабочий день", 3000);
                        Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                        Trigger.ClientEvent(player, "deleteWorkBlip");
                        Trigger.ClientEvent(player, "CloseJobStatsInfo", player.GetData("PAYMENT"));
                        MoneySystem.Wallet.Change(player, player.GetData("PAYMENT"));
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"+ {player.GetData("PAYMENT")}$", 3000);
                        NAPI.Data.SetEntityData(player, "PAYMENT", 0);

                        NAPI.Data.SetEntityData(player, "ON_WORK", false);
                        NAPI.Data.SetEntityData(player, "WORK", null);
                        //Main.StopT(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"), "timer_17");
                        Timers.Stop(NAPI.Data.GetEntityData(player, "WORK_CAR_EXIT_TIMER"));
                        NAPI.Data.ResetEntityData(player, "WORK_CAR_EXIT_TIMER");
                        Customization.ApplyCharacter(player);

                        if (player.HasData("WORKOBJECT"))
                        {
                            BasicSync.DetachObject(player);
                            player.ResetData("WORKOBJECT");
                        }
                        return;
                    }
                    NAPI.Data.SetEntityData(player, "CAR_EXIT_TIMER_COUNT", NAPI.Data.GetEntityData(player, "CAR_EXIT_TIMER_COUNT") + 1);

                }
                catch (Exception e)
                {
                    Log.Write("Timer_PlayerExitWorkVehicle_Collector:\n" + e.ToString(), nLog.Type.Error);
                }
            });
        }
        #endregion

        #region Открытие главного меню устройства на работу
        public static void StartWorkDayLoader(Client player)
        {
            if (Main.Players[player].LVL < JobsMinLVL)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Необходим как минимум {JobsMinLVL} уровень", 3000);
                return;
            }

            //Trigger.ClientEvent(player, "PressE", false);
            Trigger.ClientEvent(player, "JobsEinfo2");
            Trigger.ClientEvent(player, "OpenLoader2", checkpointPayment, Main.Players[player].LVL, Main.Players[player].WorkID);

        }
        #endregion
        #region Аренда машины и начало рабочего дня
        public static void rentCar(Client player)
        {
            if (!NAPI.Player.IsPlayerInAnyVehicle(player) || player.VehicleSeat != -1 || player.Vehicle.GetData("TYPE") != "LOADER2") return;

            MoneySystem.Wallet.Change(player, -RentCarMoney);
            GameLog.Money($"player({Main.Players[player].UUID})", $"server", RentCarMoney, $"loaderRent");

            var vehicle = player.Vehicle;
            NAPI.Data.SetEntityData(player, "WORK", vehicle);
            player.SetData("ON_WORK", true);
            Core.VehicleStreaming.SetEngineState(vehicle, false);
            NAPI.Data.SetEntityData(player, "IN_WORK_CAR", true);
            NAPI.Data.SetEntityData(vehicle, "DRIVER", player);

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
            player.SetData("PACKAGES", 1);
            //player.SetData("WORKCHECK", 1);
            //Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[1].Position, 3, 0, 255, 0, 0);
            //Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[1].Position);

            var check = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
            player.SetData("WORKCHECK", check);
            Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[check].Position, 2, 0, 255, 0, 0);
            Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[check].Position);

            Trigger.ClientEvent(player, "JobStatsInfo", player.GetData("PAYMENT"));
        }
        #endregion
        #region Когда машина заезжает на чекпоинт
        private static void PlayerEnterCheckpoint(ColShape shape, Client player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].WorkID != JobWorkId || !player.GetData("ON_WORK")) return;
                if (player.GetData("PACKAGES") == 1)
                {
                    if (shape.GetData("NUMBER2") == player.GetData("WORKCHECK"))
                    {
                        player.SetData("PACKAGES", player.GetData("PACKAGES") - 1);

                        player.SetData("WORKCHECK", -1);
                        NAPI.Task.Run(() =>
                        {
                            try
                            {
                                if (player != null && Main.Players.ContainsKey(player))
                                {
                                    var check = WorkManager.rnd.Next(0, Checkpoints2.Count - 1);
                                    player.SetData("WORKCHECK", check);
                                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints2[check].Position, 3, 0, 255, 0, 0);
                                    Trigger.ClientEvent(player, "createWorkBlip", Checkpoints2[check].Position);
                                }
                            }
                            catch { }
                        }, 500);
                    }
                }
                else
                {
                    if (shape.GetData("NUMBER3") == player.GetData("WORKCHECK"))
                    {
                        player.SetData("PACKAGES", +1);

                        var payment = Convert.ToInt32(checkpointPayment * Group.GroupPayAdd[Main.Accounts[player].VipLvl] * Main.oldconfig.PaydayMultiplier);
                        player.SetData("PAYMENT", player.GetData("PAYMENT") + payment);
                        Trigger.ClientEvent(player, "JobStatsInfo", player.GetData("PAYMENT"));
                        //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваша зарплата составляет: {player.GetData("PAYMENT")}$", 1000);

                        player.SetData("WORKCHECK", -1);
                        NAPI.Task.Run(() =>
                        {
                            try
                            {
                                if (player != null && Main.Players.ContainsKey(player))
                                {

                                //player.SetData("WORKCHECK", 1);
                                //Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[1].Position, 4, 0, 255, 0, 0);
                                //Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[1].Position);

                                var check = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
                                    player.SetData("WORKCHECK", check);
                                    Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[check].Position, 2, 0, 255, 0, 0);
                                    Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[check].Position);
                                }
                            }
                            catch { }
                        }, 500);
                    }
                }

            }
            catch (Exception e) { Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Меню устроиться на работу
        [RemoteEvent("jobJoinLoader2")]
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
                Trigger.ClientEvent(player, "secusejobLoader2", jobsid);
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
            Trigger.ClientEvent(player, "secusejobLoader2", jobsid);
            return;
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
