using System;
using GTANetworkAPI;
using AlyxSDK;
using Alyx.Core;
using System.Collections.Generic;

namespace Alyx.GYM
{
    class GYM : Script
    {
        private static nLog Log = new nLog("Gym");
        private static List<bool> states = new List<bool>() //todo Licences Names
        {
            false,   //0
            false,   //1
            false,   //2
            false,   //3
            false,   //4
            false,   //5
            false,   //6
            false,   //7
            false,   //8
            false,   //9
            false,   //10
            false,   //11
            false,   //12
            false,   //13
        };
        public static Vector3[] seatmusculebench = new Vector3[]
        {
            new Vector3(1640.52, 2522.35, 45.06),
            new Vector3(1635.75, 2526.75, 45.06),
            new Vector3(1638.15, 2529.75, 45.06),
            new Vector3(1640.75, 2532.75, 45.06),
            new Vector3(1643.05, 2535.35, 45.06),
        };
        public static Vector3[] CHINUP = new Vector3[]
        {
            new Vector3(1643.39, 2527.75, 45.56),
            new Vector3(1649.16, 2529.57, 45.56),
        };
        public static Vector3[] MUSCLEFREEWEIGHTS = new Vector3[]
        {
            new Vector3(1646.75, 2536.32, 45.56),
            new Vector3(1643.98, 2523.17, 45.56),
        };
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                /*var CHINUPS = NAPI.ColShape.CreateCylinderColShape(CHINUP[0], 1f, 2);
                CHINUPS.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 945); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; CHINUPS.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                var CHINUPS2 = NAPI.ColShape.CreateCylinderColShape(CHINUP[1], 1f, 2);
                CHINUPS2.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 944); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; CHINUPS2.OnEntityExitColShape += OnEntityExitCasinoMainShape;


                var BENCH = NAPI.ColShape.CreateCylinderColShape(seatmusculebench[0], 1f, 2);
                BENCH.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 946); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; BENCH.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                var BENCH2 = NAPI.ColShape.CreateCylinderColShape(seatmusculebench[1], 1f, 2);
                BENCH2.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 947); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; BENCH2.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                var BENCH3 = NAPI.ColShape.CreateCylinderColShape(seatmusculebench[2], 1f, 2);
                BENCH3.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 948); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; BENCH3.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                var BENCH4 = NAPI.ColShape.CreateCylinderColShape(seatmusculebench[3], 1f, 2);
                BENCH4.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 949); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; BENCH4.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                var BENCH5 = NAPI.ColShape.CreateCylinderColShape(seatmusculebench[4], 1f, 2);
                BENCH5.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 950); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; BENCH5.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                var MUSCULEFRENCH = NAPI.ColShape.CreateCylinderColShape(MUSCLEFREEWEIGHTS[0], 1f, 2);
                MUSCULEFRENCH.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 951); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; MUSCULEFRENCH.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                var MUSCULEFRENCH2 = NAPI.ColShape.CreateCylinderColShape(MUSCLEFREEWEIGHTS[1], 1f, 2);
                MUSCULEFRENCH2.OnEntityEnterColShape += (s, e) => { try { if (!e.IsInVehicle) { NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 952); } } catch (Exception ex) { Log.Write("ExitCayoPerico_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); } }; MUSCULEFRENCH2.OnEntityExitColShape += OnEntityExitCasinoMainShape;
                */
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }
        public static void OnEntityExitCasinoMainShape(ColShape shape, Player player)
        {
            NAPI.Data.SetEntityData(player, "INTERACTIONCHECK", 0);
        }
        public static void CallBackShape(Player player, int id, int rot)
        {
            if (player.HasData("CHINUP") && player.GetData<bool>("CHINUP") == true) return;
            if (states[id + 6] == true)
            {
                Notify.Error(player, "Это место занято");
                return;
            }
            states[id + 6] = true;
            player.SetData("CHINUP", true);
            NAPI.Entity.SetEntityPosition(player, CHINUP[id]);
            NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, rot));
            player.PlayAnimation("amb@prop_human_muscle_chin_ups@male@base", "base", 1);
            Trigger.ClientEvent(player, "freeze", true);
            NAPI.Data.SetEntityData(player, "INTERACTIONCHECK", 0);
            NAPI.Task.Run(() =>
            {
                player.StopAnimation();
                Trigger.ClientEvent(player, "freeze", false);
                player.SetData("CHINUP", false);
                states[id + 6] = false;
            }, 10000);
        }
        public static void CallBackShapeMusculeHand(Player player, int id, int rot)
        {
            if (player.HasData("MUSCLEFREEWEIGHTS") && player.GetData<bool>("MUSCLEFREEWEIGHTS") == true) return;
            if (states[id + 9] == true)
            {
                Notify.Error(player, "Это место занято");
                return;
            }
            states[id + 9] = true;
            player.SetData("MUSCLEFREEWEIGHTS", true);
            NAPI.Entity.SetEntityPosition(player, MUSCLEFREEWEIGHTS[id]);
            NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, rot));
            player.PlayAnimation("amb@world_human_muscle_free_weights@male@barbell@base", "base", 1);
            Trigger.ClientEvent(player, "freeze", true);
            BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_barbell_01"), 4185, new Vector3(.025, -.03, 0), new Vector3(0, 90, 0));
            NAPI.Data.SetEntityData(player, "INTERACTIONCHECK", 0);
            NAPI.Task.Run(() =>
            {
                player.StopAnimation();
                BasicSync.DetachObject(player);
                BasicSync.DetachObject(player);
                Trigger.ClientEvent(player, "freeze", false);
                player.SetData("MUSCLEFREEWEIGHTS", false);
                states[id + 9] = false;                                              
            }, 10000);
        }
        public static void CallBackShapeBench(Player player, int id, int rot)
        {
            if (player.HasData("BENCHSEAT") && player.GetData<bool>("BENCHSEAT") == true) return;
            if (states[id] == true)
            {
                Notify.Error(player, "Это место занято");
                return;
            }
            states[id] = true;
            player.SetData("BENCHSEAT", true);
            NAPI.Entity.SetEntityPosition(player, seatmusculebench[id]);
            NAPI.Entity.SetEntityRotation(player, new Vector3(0,0,rot));
            Trigger.ClientEvent(player, "freeze", true);
            player.PlayAnimation("amb@prop_human_seat_muscle_bench_press@idle_a", "idle_a", 1);
            BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_barbell_20kg"), 28422, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            NAPI.Data.SetEntityData(player, "INTERACTIONCHECK", 0);
            NAPI.Task.Run(() =>
            {
                player.StopAnimation();
                BasicSync.DetachObject(player);
                states[id] = false;
                Trigger.ClientEvent(player, "freeze", false);
                player.SetData("BENCHSEAT", false);
            }, 10000);
        }
    }
}
