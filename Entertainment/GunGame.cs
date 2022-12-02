using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using AlyxSDK;
using Alyx;
using System.Linq;

namespace Alyx.Entertainment
{

    #region Ядро
    public class GunGame : Script
    {
        static nLog Log = new nLog("GunGame");
        static int LastID = -1;
        
        #region Список арен
        public static Dictionary<int, Arena> Arenas = new Dictionary<int, Arena>();
        #endregion
        #region Список карт
        public static List<Map> Maps = new List<Map>
        {
            new Map("Cayo Perico", new Vector3(4970.987, -5170.7935, 1.1226937)  ,150f, new List<Vector3> {
                new Vector3(4990.1753, -5177.8555, 2.502152),
                new Vector3(4956.223, -5199.5005, 2.657428),
                new Vector3(4915.347, -5195.8066, 2.4969137),
                new Vector3(4921.111, -5238.712, 2.3991603),
                new Vector3(4891.423, -5207.5493, 2.671087),
                new Vector3(4878.8457, -5173.3286, 2.4922018),
                new Vector3(4993.0845, -5131.439, 2.5395458),
                new Vector3(4983.568, -5105.7725, 2.6758234),
                new Vector3(4962.831, -5087.9565, 3.2302433),
                new Vector3(4941.809, -5096.5103, 3.0433516),
                new Vector3(5006.4766, -5196.441, 2.4148175),
                new Vector3(5001.3955, -5215.7524, 2.5045671),
                new Vector3(4972.844, -5219.563, 2.5201626),
                new Vector3(4956.678, -5217.8613, 2.5133545),
                new Vector3(4963.489, -5181.6255, 2.474004),
                new Vector3(4962.6304, -5145.4595, 2.5392888),
                new Vector3(4999.4165, -5165.216, 2.7543433),
                new Vector3(4948.6846, -5180.341, 2.4740424),
                new Vector3(4872.2505, -5194.4937, 2.8042593),
                new Vector3(4934.1523, -5224.2593, 2.5528138)
            }),  
            new Map("Kortz Center", new Vector3(-2273.0842, 294.21057, 172.9755)  ,100f, new List<Vector3> {
                new Vector3(-2273.0842, 294.21057, 174.5855),
                new Vector3(-2258.3982, 263.30414, 174.6052),
                new Vector3(-2202.859, 232.6656, 174.6052),                                                              
                new Vector3(-2273.9731, 239.34167, 169.6019),
                new Vector3(-2283.17, 308.43863, 184.59548),
                new Vector3(-2215.8987, 284.46646, 184.59905),
                new Vector3(-2207.5752, 242.46092, 184.59946),
                new Vector3(-2299.396, 336.37485, 184.59548),
                new Vector3(-2269.7603, 269.49936, 184.58204),
                new Vector3(-2273.585, 288.37473, 184.58204),
                new Vector3(-2242.3865, 359.28708, 174.6019),
                new Vector3(-2205.5637, 187.27766, 174.60192),
                new Vector3(-2255.2312, 248.78629, 174.60181),
                new Vector3(-2249.7175, 272.48126, 174.60181),
            }), 
            new Map("Airport", new Vector3(2382.5273, 3103.752, 44.186993), 100f, new List<Vector3> {
                new Vector3(2426.59, 3099.3062, 52.16454),
                new Vector3(2432.4983, 3056.0637, 48.152665),
                new Vector3(2336.485, 3040.8838, 48.15147),
                new Vector3(2338.8523, 3073.74, 48.15234),
                new Vector3(2359.9797, 3125.1172, 49.008715),
                new Vector3(2342.6562, 3146.2068, 49.008745),
                new Vector3(2379.9146, 3162.5645, 48.23826),
                new Vector3(2404.91, 3138.2354, 48.155408),
                new Vector3(2363.8503, 3089.122, 48.103923),
                new Vector3(2329.086, 3082.7205, 48.100167),
            }),      
            new Map("Camp", new Vector3(-1103.2756, 4918.5215, 215.0162), 80f, new List<Vector3> {
                new Vector3(-1103.5801, 4891.9966, 215.55326),
                new Vector3(-1134.9147, 4897.7393, 219.19684),
                new Vector3(-1145.0941, 4908.88, 221.00882),
                new Vector3(-1152.2267, 4935.9326, 220.88429),
                new Vector3(-1131.6537, 4951.7354, 222.28873),
                new Vector3(-1103.1078, 4931.8374, 218.35415),
                new Vector3(-1092.698, 4951.2827, 218.35433),
                new Vector3(-1064.6888, 4943.9136, 211.59214),
                new Vector3(-1051.9773, 4911.2593, 210.25527),
                new Vector3(-1068.9941, 4901.5513, 212.85018),
                new Vector3(-1083.3558, 4936.0166, 229.35168),
                new Vector3(-1172.3362, 4894.2007, 215.3874),
            }),  
            new Map("Sawmill", new Vector3(-527.77136, 5314.657, 78.36611), 90f, new List<Vector3> {
                new Vector3(-536.6048, 5378.1074, 70.48061),
                new Vector3(-573.2181, 5365.3037, 70.30544),
                new Vector3(-590.68616, 5351.2983, 70.44476),
                new Vector3(-605.68353, 5337.431, 70.553745),
                new Vector3(-595.5823, 5313.588, 70.2945),
                new Vector3(-559.94336, 5316.4395, 73.67968),
                new Vector3(-544.5256, 5282.1816, 77.832486),
                new Vector3(-495.08246, 5262.9604, 80.801274),
                new Vector3(-492.26422, 5296.063, 80.79012),
                new Vector3(-500.89334, 5322.3315, 80.41986),
                new Vector3(-469.04266, 5357.593, 80.95851),
            }), 
            new Map("Port", new Vector3(978.58984, -3033.4688, 4.7807117), 90f, new List<Vector3> {
                new Vector3(1000.53033, -3034.1304, 5.98104),
                new Vector3(994.6945, -2996.3423, 5.980768),
                new Vector3(962.4691, -2981.872, 5.981157),
                new Vector3(1006.2914, -2970.342, 5.9808228),
                new Vector3(1062.1798, -2959.6013, 5.976683),
                new Vector3(1058.4336, -2997.4495, 5.981038),
                new Vector3(1039.8617, -3032.7622, 5.9810345),
                new Vector3(1033.2126, -3061.207, 5.9810435),
                new Vector3(1017.8787, -3088.3093, 5.9810574),
                new Vector3(985.62067, -3110.4158, 5.9808385),
                new Vector3(1076.1263, -3115.755, 5.964134),
                new Vector3(884.9869, -3116.6296, 5.9732396),
                new Vector3(911.706, -3087.967, 5.958922),
                new Vector3(933.2633, -3063.1968, 5.968984),
                new Vector3(963.4168, -3047.573, 17.227843),
            })

        };
        #endregion
        #region Создание главного шейпа
        [ServerEvent(Event.ResourceStart)]
        public void CreateGunGame()
        {
            try
            {

                Vector3 pos = new Vector3(-268.23242, -2031.9342, 29.025599);
                NAPI.Blip.CreateBlip(647, new Vector3(-254.79948, -2026.8092, 29.021372), 0.9f, 4, Main.StringToU16("Maze Bank Arena"), 255, 0, true, 0, 0);
                ColShape shape = NAPI.ColShape.CreateSphereColShape(pos, 3f);
                NAPI.TextLabel.CreateTextLabel("~w~Deathmatch", new Vector3(pos.X, pos.Y, pos.Z + 2f), 10F, 5F, 4, new Color(255, 255, 255), true, 0);
                NAPI.Marker.CreateMarker(1, pos - new Vector3(0, 0, 0), new Vector3(), new Vector3(), 2.965f, new Color(107, 107, 250, 220), false, 0);
                NAPI.Marker.CreateMarker(27, pos + new Vector3(0, 0, 0.14f), new Vector3(), new Vector3(), 3f, new Color(107, 107, 250, 220), false, 0);
                shape.OnEntityEnterColShape += (s, player) =>
                {
                    try
                    {
                        player.SetData("INTERACTIONCHECK", 522);
                    }
                    catch (Exception e) { Log.Write("CREATE GUN GAME SHAPE" + e.ToString(), nLog.Type.Error); }
                };
                shape.OnEntityExitColShape += (s, player) =>
                {
                    try
                    {
                        player.SetData("INTERACTIONCHECK", 0);
                    }
                    catch (Exception e) { Log.Write("CREATE GUN GAME SHAPE" + e.ToString(), nLog.Type.Error); }
                };
            }
            catch (Exception e) { Log.Write("CREATE GUN GAME" + e.ToString(), nLog.Type.Error); }
        }
        #endregion
        #region Когда игрок умирает
        [ServerEvent(Event.PlayerDeath)]
        public void onPlayerDeathHandler(Player player, Player entityKiller, uint weapon)
        {
            try
            {
                if (!player.HasData("ARENA")) return;
                if (player.GetData<Arena>("ARENA").Active)
                {
                    if (entityKiller != null && player != entityKiller)
                    {
                        entityKiller.SetData("KILLS", entityKiller.GetData<int>("KILLS") + 1);
                        NAPI.ClientEvent.TriggerClientEvent(entityKiller, "client::setkills", entityKiller.GetData<int>("KILLS"));
                    }
                    player.SetData("DEATHS", player.GetData<int>("DEATHS") + 1);

                    NAPI.ClientEvent.TriggerClientEvent(player, "client::setdeaths", player.GetData<int>("DEATHS"));
                    player.GetData<Arena>("ARENA").SpawnPlayer(player);
                    player.SetData("DEATH", true);
                }
                else
                {
                    NAPI.Player.SpawnPlayer(player, new Vector3(-265.5906, -2017.5664, 30.025599));
                    player.Dimension = (uint)(player.GetData<Arena>("ARENA").ID + 2000);
                }
            }
            catch (Exception e) { Log.Write("PlayerDeath: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Когда игрок выходит
        [ServerEvent(Event.PlayerDisconnected)]
        public static void onPlayerDissonnectedHandler(Player player, DisconnectionType type, string reason)
        {
            try
            {
                if (!player.HasData("ARENA")) return;

                Arena arena = player.GetData<Arena>("ARENA");

                if (arena.Players.Contains(player))
                    arena.Players.Remove(player);

                arena.RefreshPlayers();
            }
            catch (Exception e) { Log.Write("PlayerDisconnected: " + e.Message, nLog.Type.Error); }
        }
        #endregion
        #region Ивенты с клиента

        [RemoteEvent("server::getlobbylist")]
        static void RE_getlobbylist(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;

                List<object> LobbyList = new List<object>();

                foreach(Arena arena in Arenas.Values)
                {
                    if (arena.Active) continue;
                    List<object> Lobby = new List<object> {
                        arena.ID,
                        !string.IsNullOrEmpty(arena.Pass),
                        arena.Players.Count + " / " + arena.MaxPlayers,
                        arena.Money,
                        arena.Weapon,
                        arena.Map.Name
                    };
                    LobbyList.Add(Lobby);
                }
                NAPI.ClientEvent.TriggerClientEvent(player, "client::setlobbylist", JsonConvert.SerializeObject(LobbyList));

            }
            catch (Exception e) { Log.Write("RE_getlobbylist: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("server::kickplayer")]
        static void RE_kickplayer(Player player, string nick)
        {
            try 
            {
                if (!player.HasData("ARENA")) return;
                Arena arena = player.GetData<Arena>("ARENA");
                if (arena.Owner != player) return;
                Player findnick = null;
                
                foreach (Player ply in arena.Players)
                    if (ply.Name == nick)
                        findnick = ply;

                if (findnick == arena.Owner) return;

                arena.Players.Remove(findnick);
                arena.RefreshPlayers();

                NAPI.Entity.SetEntityPosition(findnick, new Vector3(-265.5906, -2017.5664, 30.025599));
                Main.Players[findnick].ExteriorPos = new Vector3();

                NAPI.ClientEvent.TriggerClientEvent(findnick, "client::closemenu");

            }
            catch (Exception e) { Log.Write("RE_kickplayer: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("server::startmatch")]
        static void RE_startmatch(Player player)
        {
            try
            {
                if (!player.HasData("ARENA")) return;
                Arena arena = player.GetData<Arena>("ARENA");

                if (arena.Active && arena.Owner != player) return;

                /*if (arena.Players.Count < 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя запустить матч. Минимум 2 игрока!", 3000);
                    return;
                }  */

                arena.Start();

            }
            catch (Exception e) { Log.Write("RE_startmatch: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("server::connectlobby")]
        static void RE_connectlobby(Player player, int id)
        {
            try
            {
                if (player.HasData("ARENA")) return;
                if (!Arenas.ContainsKey(id)) return;

                Arena arena = Arenas[id];

                if (arena.Players.Count >= arena.MaxPlayers)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данное лобби переполнено!", 3000);
                    return;
                }
                if (!string.IsNullOrEmpty(arena.Pass))
                {
                    player.SetData("ARENAID", id);
                    NAPI.ClientEvent.TriggerClientEvent(player, "client::closemenu");
                    NAPI.ClientEvent.TriggerClientEvent(player, "openInput", $"Присоединение к Лобби №{id} ", "Введите пароль чтоб войти", 8, "gun_enterpass") ;
                    return;
                }

                Main.Players[player].ExteriorPos = new Vector3(-265.5906, -2017.5664, 29.025599);

                arena.Players.Add(player);

                arena.SetLobby(player);
                arena.RefreshPlayers();

                player.SetData("ARENA", arena);
               // List<string> LobbyInfo = JsonConvert.DeserializeObject<List<string>>(LobbyInfoT);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы вошли в лобби!", 3000);
                MoneySystem.Wallet.Change(player, -Arenas[id].Money);
            }
            catch (Exception e) { Log.Write("RE_connectlobby: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("server::disconnectlobby")]
        static void RE_disconnectlobby(Player player)
        {
            try
            {
                if (!player.HasData("ARENA")) return;

                Arena arena = player.GetData<Arena>("ARENA");
                arena.Players.Remove(player);

                Trigger.ClientEvent(player, "client::removeweapon", GunGame.WeaponOnName[arena.Weapon]);

                if (arena.Owner == player)
                {

                    foreach (Player ply in arena.Players)
                    {
                        if (!ply.HasData("ARENA")) continue;
                        ply.ResetData("ARENA");
                        NAPI.ClientEvent.TriggerClientEvent(ply, "client::closemenu");
                        Trigger.ClientEvent(ply, "client::removeweapon", GunGame.WeaponOnName[arena.Weapon]);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Владелец покинул лобби", 3000);
                    }
                    if (Arenas.ContainsKey(arena.ID))
                        Arenas.Remove(arena.ID);
                    arena.Destroy();
                }

                NAPI.Entity.SetEntityPosition(player, new Vector3(-265.5906, -2017.5664, 30.025599));
                Main.Players[player].ExteriorPos = new Vector3();

                NAPI.Entity.SetEntityDimension(player, 0);

                

                player.ResetData("ARENA");

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы вышли из лобби!", 3000);
               // List<string> LobbyInfo = JsonConvert.DeserializeObject<List<string>>(LobbyInfoT);

               // MoneySystem.Wallet.Change(player, Convert.ToInt32(LobbyInfo[2]));
            }
            catch (Exception e) { Log.Write("RE_disconnectlobby: " + e.Message, nLog.Type.Error); }
        }

        static List<string> WeaponOnIndex = new List<string> { "Revolver", "SMG", "Heavy Shotgun", "Rifle" };
        public static Dictionary<string, int> WeaponOnName = new Dictionary<string, int> { { "Revolver", -1045183535 }, { "SMG", 736523883 }, { "Heavy Shotgun", -1074790547 }, { "Rifle", 984333226 } };

        [RemoteEvent("server::sendlobby")]
        static void RE_sendlobby(Player player, string LobbyInfoT)
        {
            try
            {
                if (player.HasData("ARENA")) return;

                List<string> LobbyInfo = JsonConvert.DeserializeObject<List<string>>(LobbyInfoT);

                if (Convert.ToInt32( LobbyInfo[1] ) > 16)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Максимально 16 игроков!", 3000);
                    return;
                }
                if (Convert.ToInt32(LobbyInfo[1]) < 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Минимально 2 игрока!", 3000);
                    return;
                }
                if (Convert.ToInt32(LobbyInfo[2]) > 100000 || Convert.ToInt32(LobbyInfo[2]) < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Максимально 25000$", 3000);
                    return;
                }
                if (Main.Players[player].Money < Convert.ToInt32(LobbyInfo[2]))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                    return;
                }
                if (Arenas.Count >= 16)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В данный момент список матчей переполнен, подождите", 3000);
                    return;
                }
                LastID++;

                Arena arena = new Arena(player, LastID, WeaponOnIndex[Convert.ToInt32(LobbyInfo[3])], Convert.ToInt32(LobbyInfo[2]), Maps[Convert.ToInt32(LobbyInfo[4])], LobbyInfo[0], Convert.ToInt32(LobbyInfo[1])) ;
                arena.Players.Add(player);
                Arenas.Add(LastID, arena);

                List<object> Lobby = new List<object>
                {
                    "1 / " + Convert.ToInt32(LobbyInfo[1]),
                    arena.Weapon,
                    arena.Money,
                    arena.ID,
                    arena.Map.Name
                };

                Main.Players[player].ExteriorPos = new Vector3(-265.5906, -2017.5664, 30.025599);

                player.SetData("ARENA", arena);

                NAPI.Entity.SetEntityDimension(player, (uint)(2000 + arena.ID));

                MoneySystem.Wallet.Change(player, -arena.Money);

                NAPI.ClientEvent.TriggerClientEvent(player, "client::createlobby", JsonConvert.SerializeObject(new List<string> { player.Name }), JsonConvert.SerializeObject(Lobby));

            }
            catch  { }
        }


        #endregion
    }
    #endregion
    #region Конструктор карт
    public class Map
    {
        public string Name;
        public Vector3 Center;
        public float Radius;
        public List<Vector3> SpawnPos;

        public Map(string name, Vector3 center, float radius, List<Vector3> spawns)
        {
            Name = name; Center = center; Radius = radius; SpawnPos = spawns;
        }

    }
    #endregion
    #region Конструктор арены
    public class Arena
    {
        public Player Owner;
        public int ID;
        public bool Active;
        public int MaxPlayers;
        public string Weapon = "none";
        public int Money;
        public string Pass = "";
        public Map Map;
        public List<Player> Players;
        string Timerid;
        int Time;
        int TimeStart;
        int Dimension;
        ColShape CenterColShape;

        public Arena( Player owner, int id, string weapon, int money, Map map, string pass, int maxplayers )
        {
            Owner = owner; ID = id; Active = false; Weapon = weapon; Players = new List<Player>(); Money = money; Map = map; Pass = pass; MaxPlayers = maxplayers;
        }
        #region Когда матч начинается
        public void Start()
        {
            try
            {
                Dimension = 2300 + ID;
                int i = 0;
                foreach (Player player in Players)
                    if (player.HasData("ARENA") && player.GetData<Arena>("ARENA").ID == ID)
                    {

                            NAPI.Entity.SetEntityPosition(player, Map.SpawnPos[i]);

                        NAPI.ClientEvent.TriggerClientEvent(player, "client::closemenuno");
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::sethud", true);
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::setkills", 0);
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::setdeaths", 0);
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::setarena", Map.Name);
                        player.SetData("KILLS", 0);
                        player.SetData("DEATHS", 0);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Начинается матч!", 3000);
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::setweapon", GunGame.WeaponOnName[Weapon]);
                       /* Trigger.ClientEvent(player, "wgive", GunGame.WeaponOnName[Weapon], 9000, false, true);   */

                        NAPI.Entity.SetEntityDimension(player, (uint)Dimension);
                        i++;
                    }
                CenterColShape = NAPI.ColShape.CreateSphereColShape(Map.Center, Map.Radius, (uint)Dimension);
                CenterColShape.OnEntityExitColShape += (s, entity) =>
                {
                    try
                    {
                        NAPI.Entity.SetEntityPosition(entity, GetSpawnPos());
                        Notify.Send(entity, NotifyType.Error, NotifyPosition.BottomCenter, "Вы покинули зону боя, возвращение назад!", 3000);
                    }
                    catch { }
                };
                Time = 600;
                TimeStart = 10;
                Timerid = Timers.StartTask(1000, () => { TimerStart(); });
            }
            catch (Exception e) { Console.WriteLine("start: " + e.ToString()); }
        }
        #endregion
        #region Спавн игрока когда он умер
        public void SpawnPlayer(Player player)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    if (!player.HasData("DEATH")) return;
                    NAPI.Player.SpawnPlayer(player, GetSpawnPos());
                    NAPI.Entity.SetEntityDimension(player, (uint)Dimension);
                    NAPI.ClientEvent.TriggerClientEvent(player, "client::setweapon", GunGame.WeaponOnName[Weapon]);
                }
                catch (Exception e) { Console.WriteLine("respawn: " + e.ToString()); }
            }, 5000);
        }
        #endregion
        #region Отчёт времени старта
        public void TimerStart()
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    if (Players == null)
                    {
                        Timers.Stop(Timerid);
                        return;
                    }

                    string sec = TimeStart % 60 < 10 ? $"0{TimeStart % 60}" : (TimeStart % 60).ToString();
                    string time = $"{TimeStart / 60}:{sec}";

                    foreach (Player player in Players)
                    {
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::settime", time);
                    }

                    TimeStart -= 1;
                    if (TimeStart <= 0)
                    {
                        Timers.Stop(Timerid);
                        Timerid = Timers.StartTask(1000, () => { Timer(); });
                    }
                    Active = true;
                }
                catch (Exception e) { Console.WriteLine("timerstart: " + e.ToString()); }
            });

        }
        #endregion
        #region Таймер
        public void Timer()
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    string sec = Time % 60 < 10 ? $"0{Time % 60}" : (Time % 60).ToString();
                    string time = $"{Time / 60}:{sec}";
                    int time2 = Time;

                    foreach (Player player in Players)
                    {
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::settime", time);
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::settime2", time2);
                    }

                    Time -= 1;
                    if (Time <= 0)
                        Stop();
                }
                catch (Exception e) { Console.WriteLine("timer: " + e.ToString()); }
            });
        }
        #endregion
        #region Уничтожение арены
        public void Destroy()
        {
            try
            {
                Owner = null; ID = -1; Weapon = ""; Players = null; Money = 0; Map = null; Pass = null; MaxPlayers = 0; Time = 0; Timerid = null; NAPI.Task.Run(() => { try { CenterColShape.Delete(); } catch { } });
            }
            catch (Exception e) { Console.WriteLine("destroy: " + e.ToString()); }
        }

        #endregion

        #region Когда матч заканчивается
        public void Stop()
        {
            try
            {
                List<object> Winners = new List<object> { };

                Dictionary<Player, int> ForSort = new Dictionary<Player, int>();


                foreach (Player player in Players)
                    ForSort.Add(player, player.GetData<int>("KILLS"));

                ForSort = ForSort.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

                int i = 0;

                foreach (KeyValuePair<Player, int> pair in ForSort.Reverse().ToDictionary(x => x.Key, x => x.Value))
                {
                    i++;
                    List<object> Winner = new List<object> { pair.Key.Name, pair.Value, pair.Key.GetData<int>("DEATHS"), i == 1 ? Money : 0 };
                    if (i == 1)
                        MoneySystem.Wallet.Change(pair.Key, Money*Players.Count);
                    Winners.Add(Winner);
                }

                foreach (Player player in Players)
                    if (player.HasData("ARENA") && player.GetData<Arena>("ARENA").ID == ID)
                    {
                        if (player.HasData("DEATH"))
                        {
                            NAPI.Player.SpawnPlayer(player, new Vector3(-265.5906, -2017.5664, 30.025599));
                            player.ResetData("DEATH");
                        }
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::sethud", false);
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::sendwinners", JsonConvert.SerializeObject(Winners));
                        NAPI.Entity.SetEntityPosition(player, new Vector3(-265.5906, -2017.5664, 30.025599));
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::removeweapon", GunGame.WeaponOnName[Weapon]);
                        NAPI.Entity.SetEntityDimension(player, 0);
                        Main.Players[player].ExteriorPos = new Vector3();
                        player.ResetData("KILLS");
                        player.ResetData("DEATHS");
                        player.ResetData("ARENA");
                    }
                Active = false;
                if (GunGame.Arenas.ContainsKey(ID))
                    GunGame.Arenas.Remove(ID);

                Timers.Stop(Timerid);

                Destroy();
            }
            catch (Exception e) { Console.WriteLine("stop: " + e.ToString()); }
        }
        #endregion
        #region Функция для изображения лобби для игрока
        public void SetLobby(Player player)
        {
            try
            {
                List<object> LobbyInfo = new List<object> 
                {
                    Players.Count + " / " + MaxPlayers,
                    Weapon,
                    Money,
                    ID,
                    Map.Name
                };

                List<string> NamePlayers = new List<string>();

                foreach (Player players in Players)
                    NamePlayers.Add(players.Name);

                NAPI.Entity.SetEntityDimension(player, (uint)(2000 + ID));

                NAPI.ClientEvent.TriggerClientEvent(player, "client::setlobby", JsonConvert.SerializeObject(NamePlayers), JsonConvert.SerializeObject(LobbyInfo));
            }
            catch (Exception e) { Console.WriteLine("setlobby: " + e.ToString()); }
        }
        #endregion
        #region Обновление данных о лобби, а именно игроков
        public void RefreshPlayers()
        {
            try
            {
                List<string> NamePlayers = new List<string>();

                foreach (Player player in Players)
                    NamePlayers.Add(player.Name);

                foreach (Player player in Players)
                    if (player.HasData("ARENA") && player.GetData<Arena>("ARENA").ID == ID)
                    {
                        NAPI.ClientEvent.TriggerClientEvent(player, "client::refreshlobby", JsonConvert.SerializeObject(NamePlayers));
                    }
            }
            catch (Exception e) { Console.WriteLine("refreshplayers: " + e.ToString()); }
        }
        #endregion

        #region Поиск свободной позиций спавна
        public Vector3 GetSpawnPos()
        {
            try
            {
                return Map.SpawnPos[new Random().Next(0, Map.SpawnPos.Count)];
            }
            catch (Exception e) { Console.WriteLine("getspawnpos: " + e.ToString()); return new Vector3(); }
        }
        #endregion
    }
#endregion
}
