
{
    class ElectroStations : Script
    {
        private static nLog Log = new nLog("Electro Stations");
        private static ColShape colShape;
        private static int price = 50;
        public static Vector3[] electroStationsPositions = new Vector3[]
        {
            new Vector3(-707.15686, -939.0491, 17.89403),
            new Vector3(-335.34085, -1457.3112, 29.421846),
            new Vector3(-82.43239, -1757.5712, 28.514603),
            new Vector3(182.04198, -1545.7596, 28.029219),
            new Vector3(284.87747, -1263.7346, 28.148338),
            new Vector3(808.5983, -1045.5682, 25.545921),
            new Vector3(-1419.8214, -286.83978, 45.160587),
            new Vector3(-2111.0132, -318.95578, 11.901008),
            new Vector3(-2554.1655, 2321.7563, 31.940409),
            new Vector3(1190.3724, 2659.853, 36.702656),
            new Vector3(2683.1477, 3293.6196, 54.120632),
            new Vector3(1991.9292, 3761.193, 31.060698),
            new Vector3(202.4303, 6607.031, 30.52927),
            new Vector3(-1130.5604, -1977.0101, 12.042612),
            new Vector3(602.95605, 268.55856, 102.27629),
            new Vector3(2564.5715, 362.4265, 107.34258),
            new Vector3(-3050.6572, 609.88196, 6.060239),
        };
        #region [ResourseStart]
        [ServerEvent(Event.ResourceStart)]
        public static void standOnColshape()
        {
            foreach (Vector3 vec in electroStationsPositions)
            {
                colShape = NAPI.ColShape.CreateCylinderColShape(vec, 2, 2, dimension: 0);
                colShape.OnEntityEnterColShape += (ColShape shape, Player client) =>
                {
                    try
                    {
                        NAPI.Data.SetEntityData(client, "INTERACTIONCHECK", 909);
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
            }
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-705.6441, -939.0361, 17.893934), new Vector3(0, 0, 0), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-335.841, -1455.0388, 29.436937), new Vector3(0, 0, 270), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-84.18107, -1757.0314, 28.488735), new Vector3(0, 0, 155), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(183.36285, -1544.042, 28.021866), new Vector3(0, 0, 40), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(286.88303, -1263.6915, 28.17071), new Vector3(0, 0, 265), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(808.6441, -1047.4987, 25.657053), new Vector3(0, 0, 270), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-1418.2333, -288.89948, 45.19959), new Vector3(0, 0, 307), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-2112.7378, -318.6783, 11.898869), new Vector3(0, 0, 350), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-2554.1118, 2319.522, 31.9510759), new Vector3(0, 0, 290), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(1192.1095, 2658.096, 36.725666), new Vector3(0, 0, 215), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(2682.07, 3291.5884, 54.129246), new Vector3(0, 0, 150), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(1993.1398, 3759.0713, 31.060701), new Vector3(0, 0, 201), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(205.37161, 6607.294, 30.486553), new Vector3(0, 0, 10), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-1130.5709, -1974.1855, 12.041295), new Vector3(0, 0, 100), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(600.2108, 268.45932, 102.625916), new Vector3(0, 0, 180), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(2562.5745, 362.5738, 107.34803), new Vector3(0, 0, 175), 255);
            NAPI.Object.CreateObject(NAPI.Util.GetHashKey("prop_teslastation"), new Vector3(-3051.7927, 611.9502, 6.0547686), new Vector3(0, 0, 125), 255);

            Log.Write("Electrostations activated", nLog.Type.Info);
        }

        public static void OpenElectroPetrolMenu(Player player)
        {
            if (!player.IsInVehicle)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в транспортном средстве", 3000);
                return;
            }
            else
            {
                Vehicle vehicle = player.Vehicle;
                int fuel = vehicle.GetSharedData<int>("PETROL");
                Trigger.ClientEvent(player, "openElectroPetrol", price, fuel);
                return;
            }
        }

        [RemoteEvent("electropetrol")]   //for cef
        public static void fillelectrocar(Player player, int lvl)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                Vehicle vehicle = player.Vehicle;
                if (vehicle == null) return;
                if (player.VehicleSeat != 0) return;
                var isGov = false;
                int fuel = vehicle.GetSharedData<int>("PETROL");

                if (lvl == 9999)
                    lvl = VehicleManager.VehicleTank[vehicle.Class] - fuel;
                else if (lvl == 99999)
                {
                    isGov = true;
                    lvl = VehicleManager.VehicleTank[vehicle.Class] - fuel;
                }

                int tfuel = fuel + lvl;
                if (!vehicle.HasSharedData("PETROL"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно заправить эту машину", 3000);
                    return;
                }
                if (tfuel > VehicleManager.VehicleTank[vehicle.Class])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
                    return;
                }
                if (!VehicleManager.ElectroVehicles.Contains((VehicleHash)vehicle.Model))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Электричеством нельзя заправлять данное авто", 3000);
                    return;
                }

                if (isGov)
                {
                    int frac = Main.Players[player].FractionID;
                    if (Fractions.Manager.FractionTypes[frac] != 2)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Чтобы заправить транспорт за гос. счет, Вы должны состоять в гос. организации", 3000);
                        return;
                    }
                    if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "FRACTION" || vehicle.GetData<int>("FRACTION") != frac)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете заправить за государственный счет не государственный транспорт", 3000);
                        return;
                    }
                    if (Fractions.Stocks.fracStocks[frac].FuelLeft < lvl * price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Лимит на заправку гос. транспорта за день исчерпан", 3000);
                        return;
                    }
                }
                else
                {
                    if (Main.Players[player].Money < lvl * price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств", 3000);
                        return;
                    }
                }
                if (isGov)
                {
                    GameLog.Money($"frac(6)", $"ELECTROSTATION", lvl * price, "buyElectroPetrol");
                    Fractions.Stocks.fracStocks[6].Money -= lvl * price;
                    Fractions.Stocks.fracStocks[Main.Players[player].FractionID].FuelLeft -= lvl * price;
                }
                else
                {
                    Trigger.ClientEvent(player, "VEHICLE::FREEZE", vehicle, true);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Транспорт заряжается", 3000);
                    MoneySystem.Wallet.Change(player, -lvl * price);
                    Trigger.ClientEvent(player, "open_electropetroltimer");
                    NAPI.Task.Run(() =>
                    {
                        if (NAPI.Data.GetEntityData(vehicle, "ACCESS") == "PERSONAL")
                        {
                            var number = NAPI.Vehicle.GetVehicleNumberPlate(vehicle);
                            VehicleManager.Vehicles[number].Fuel += lvl;
                        }
                        vehicle.SetSharedData("PETROL", tfuel);
                        Trigger.ClientEvent(player, "VEHICLE::FREEZE", vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Транспорт заряжен", 3000);
                        Trigger.ClientEvent(player, "close_electropetroltimer");
                    }, 10000);
                }
            }
            catch (Exception e) { Log.Write("Petrol: " + e.Message, nLog.Type.Error); }
        }
        #endregion
    }
}