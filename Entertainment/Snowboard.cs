using GTANetworkAPI;
using AlyxSDK;
using System;

namespace Alyx.Core
{
    class Snowboard : Script
    {
        private static nLog Log = new nLog("Snowboard");

        [RemoteEvent("startSnowboard")]
        public void startSpecSnowMission(Player client)
        {
            try
            {
                if (client.HasSharedData("SnowboardAttached") && client.GetSharedData<bool>("SnowboardAttached") == true)
                {
                    var itemsnowboard1 = nInventory.Find(Main.Players[client].UUID, ItemType.Snowboard1);
                    itemsnowboard1.IsActive = false;
                    client.SetSharedData("SnowboardAttached", false);
                    client.SetData("SnowboardAttached", false);
                    BasicSync.DetachObject(client);
                }
                var veh = NAPI.Vehicle.CreateVehicle((VehicleHash)NAPI.Util.GetHashKey("fakeboard"), client.Position, client.Rotation.Z, 0, 0, "skate", 0, true, true, client.Dimension);
                veh.SetData("ACCESS", "SKATE");
                veh.SetData("BY", client.Name);
                veh.Transparency = 0;
                veh.SetSharedData("skate", true);
                NAPI.Task.Run(() => {
                    client.Transparency = 0;
                    client.SetIntoVehicle(veh, 0);
                    veh.Transparency = 0;
                    NAPI.Task.Run(() => {
                        client.SetSharedData("skate", true);
                    }, 500);
                }, 500);
            }
            catch (Exception e) { Log.Write($"StartSnowboard/: {e.ToString()}\n{e.StackTrace}", nLog.Type.Error); }
        }

        [RemoteEvent("StopSnowMisson")]
        public void StopSnowMIssion(Player client)
        {
            try
            {
                if (!client.IsInVehicle) return;
                Vehicle veh = client.Vehicle;
                if (veh.HasData("ACCESS") && veh.GetData<string>("ACCESS") == "SKATE") veh.Delete();
                client.SetSharedData("skate", false);
                NAPI.Task.Run(() =>
                {
                    Trigger.ClientEvent(client, "stopsnowboarddest");
                }, 1500);
                
            }
            catch (Exception e) { Log.Write($"stopSnowMission/: {e.ToString()}\n{e.StackTrace}", nLog.Type.Error); }
        }
        [ServerEvent(Event.PlayerDeath)]
        public static void DeathOnSnowboard(Player player, Player entityKiller, uint weapon)
        {
            Trigger.ClientEvent(player, "LeaveSnowboardServer");
        }
        [ServerEvent(Event.PlayerExitVehicle)]
        public static void LeaveVehOnSnow(Player player, Vehicle vehicle)
        {
            Trigger.ClientEvent(player, "LeaveSnowboardServer");
        }
    }
}
