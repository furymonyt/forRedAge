using System;
using GTANetworkAPI;
using AlyxSDK;

namespace Alyx.Core
{
    class Arcade : Script
    {
        private static nLog Log = new nLog("Aracde");
        private static Vector3 _entrancePosition = new Vector3(-601.1239, 279.51154, 82.037274);
        private static Vector3 _exitPosition = new Vector3(2737.893, -374.50977, -47.993014);
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                var colShapeEnter = NAPI.ColShape.CreateCylinderColShape(_entrancePosition, 1f, 2, 0);
                var colShapeExit = NAPI.ColShape.CreateCylinderColShape(_exitPosition, 1f, 2, 0);

                NAPI.Blip.CreateBlip(740, _entrancePosition, 0.7f, 4, Main.StringToU16("Игровой зал"), 255, 0, true, 0, 0);

                NAPI.Marker.CreateMarker(30, _entrancePosition - new Vector3(0, 0, 0), new Vector3(), new Vector3(), 0.8f, new Color(107, 107, 250), false, 0);
                NAPI.Marker.CreateMarker(30, _exitPosition - new Vector3(0, 0, 0), new Vector3(), new Vector3(), 0.8f, new Color(107, 107, 250), false, 0);

                colShapeEnter.OnEntityEnterColShape += (s, e) =>
                {
                    try
                    {
                        if (!e.IsInVehicle)
                        {
                            NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 668);
                            NAPI.Data.SetEntityData(e, "ARCADE_MAIN_SHAPE", "ENTER");
                        }
                    }
                    catch (Exception ex) { Log.Write("EnterCasino_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
                };
                colShapeEnter.OnEntityExitColShape += OnEntityExitCasinoMainShape;

                colShapeExit.OnEntityEnterColShape += (s, e) =>
                {
                    try
                    {
                        if (!e.IsInVehicle)
                        {
                            NAPI.Data.SetEntityData(e, "INTERACTIONCHECK", 668);
                            NAPI.Data.SetEntityData(e, "ARCADE_MAIN_SHAPE", "EXIT");
                        }
                    }
                    catch (Exception ex) { Log.Write("ExitCasino_OnEntityEnterColShape: " + ex.Message, nLog.Type.Error); }
                };
                colShapeExit.OnEntityExitColShape += OnEntityExitCasinoMainShape;
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }
        public static void OnEntityExitCasinoMainShape(ColShape shape, Player player)
        {
            NAPI.Data.SetEntityData(player, "INTERACTIONCHECK", 0);
            NAPI.Data.ResetEntityData(player, "ARCADE_MAIN_SHAPE");
        }
        public static void CallBackShape(Player player)
        {
            if (!player.HasData("ARCADE_MAIN_SHAPE")) return;
            string data = player.GetData<string>("ARCADE_MAIN_SHAPE");
            if (data == "ENTER")
            {
                Trigger.ClientEvent(player, "showHUD", false);
                NAPI.Task.Run(() => {
                    try
                    {
                        if (player != null)
                        {
                            Trigger.ClientEvent(player, "screenFadeOut", 1000);
                        }
                    }
                    catch { }
                }, 100);
                NAPI.Task.Run(() => {
                    try
                    {
                        if (player != null)
                        {
                            if (player.IsInVehicle)
                            {
                                return;
                            }
                            else
                            {
                                NAPI.Entity.SetEntityPosition(player, _exitPosition);
                                NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, -27.5));
                                Trigger.ClientEvent(player, "screenFadeIn", 1000);
                                Trigger.ClientEvent(player, "showHUD", true);
                            }
                        }
                    }
                    catch { }
                }, 1600);
                return;
            }
            if (data == "EXIT")
            {
                Trigger.ClientEvent(player, "showHUD", false);
                NAPI.Task.Run(() => {
                    try
                    {
                        if (player != null)
                        {
                            Trigger.ClientEvent(player, "screenFadeOut", 1000);
                            Trigger.ClientEvent(player, "showHUD", false);
                        }
                    }
                    catch { }
                }, 100);
                NAPI.Task.Run(() => {
                    try
                    {
                        if (player != null)
                        {
                            if (player.IsInVehicle)
                            {
                                return;
                            }
                            else
                            {
                                NAPI.Entity.SetEntityPosition(player, _entrancePosition);
                                NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, -27.5));
                                Trigger.ClientEvent(player, "screenFadeIn", 1000);
                                Trigger.ClientEvent(player, "showHUD", true);
                            }
                        }
                    }
                    catch { }
                }, 1600);
            }
        }
    }
}
