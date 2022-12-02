using GTANetworkAPI;
using System;
using AlyxSDK;
using Alyx.Core;
using System.Collections.Generic;

namespace Alyx.Entertainment //Поменяйте на свое
{
    class Sex : Script
    {
        private static nLog Log = new nLog("Sex System");
        private static Ped Janna = null;
        private static ColShape shape = null;
        private static bool state = false;
        private static bool broken = false;
		private static int priceblowjob = 50;
		private static int pricekon = 150;
        public static List<Vector3> Listpos = new List<Vector3>() //Позиции нашей девочки
        {
            new Vector3(-1353.2756, -1176.7366, 4.4142662),
            new Vector3(-515.26917, 6271.7456, 9.848836),
            new Vector3(2746.2837, 3458.6243, 55.832784),
            new Vector3(955.33105, 124.750244, 80.99043),
            new Vector3(-236.03307, -982.2494, 29.288453),
            new Vector3(-986.7728, -438.593, 37.680658),
            new Vector3(-2189.2415, -388.90686, 13.470375),
            new Vector3(411.21606, -897.03986, 29.418618),
        };
        public static List<int> ListRot = new List<int>()
        {
            95,
            33,
            -112,
            68,
            -104,
            -150,
            -36,
            94
        };

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                JannaChangePos();
                Log.Write("Loaded", nLog.Type.Success);
            } 
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"SEX\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        [RemoteEvent("server::tososat")]
        public static void SwitchesSex(Player player, int id)
        {
			try
            {
				switch(id)
				{
					case 0:
						if (Main.Players[player].Money < priceblowjob)
						{
							Notify.Error(player, "Недостаточно денег");
							return;
						}
						MoneySystem.Wallet.Change(player, -priceblowjob);
						Trigger.ClientEvent(player, "prostitutcasosat");
						return;
					case 1:
						if (Main.Players[player].Money < pricekon)
						{
							Notify.Error(player, "Недостаточно денег");
							return;
						}
						MoneySystem.Wallet.Change(player, -pricekon);
						Trigger.ClientEvent(player, "prostitutcakon");
						return;
					case 3:
						Trigger.ClientEvent(player, "client::prostitutcaleave");
						return;
				} 
			} 
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"SEX_Tososat\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        public static void OpenMenuProstituca(Player player)
        {
			try 
			{
				if (player != null)
				{
					broken = false;
					state = true;
					Trigger.ClientEvent(player, "freezeped", Janna, false);
					Trigger.ClientEvent(player, "client::seatprostitutca", Janna);
				}
			} 
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"SEX_OpenMenuShluxa\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        public static void JannaChangePos()
        {
			try 
			{
				if (!state && broken)
				{
                    broken = false;
					var rnd = new Random().Next(0, Listpos.Count - 1);
					Janna = NAPI.Ped.CreatePed(0x14C3E407, Listpos[rnd], ListRot[rnd], true, true, false, true, 0);
					shape = NAPI.ColShape.CreateSphereColShape(Listpos[rnd], 5f);
					shape.OnEntityEnterColShape += (s, player) =>
					{
						try
						{
							if (player.IsInVehicle && !state && !broken)
							{
								player.SetData("INTERACTIONCHECK", 900);
							}
						}
						catch (Exception e) { Log.Write("EXCEPTION AT \"PROSTITUCA\":\n" + e.ToString(), nLog.Type.Error); }
					};
					shape.OnEntityExitColShape += (s, player) =>
					{
						try
						{
							if (player.IsInVehicle)
							{
								player.SetData("INTERACTIONCHECK", -1);
							}
						}
						catch (Exception e) { Log.Write("EXCEPTION AT \"PROSTITUCA\":\n" + e.ToString(), nLog.Type.Error); }
					};
				}
			} 
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"SEX_JannaMenaetPosition\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("sexspawn")]
        public static void CMD_sexspawn(Player player)
        {
            broken = true;
            JannaChangePos();
            NAPI.Chat.SendChatMessageToPlayer(player, "Coords" + Janna.Position);
        }

        [RemoteEvent("server::jannaposChange")]
        public static void JannaDelete(Player player)
        {
			try 
			{
				state = false;
				broken = true;
				shape.Delete();
				Janna.Delete();
			} 
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"SEX_JannaDelete\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
    }
}
