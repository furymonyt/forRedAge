
{
    class RodManager : Script
    {

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Player player, Player killer, uint reason)
        {
            BasicSync.DetachObject(player);
        }

        public static Dictionary<int, ItemType> FishItems1 = new Dictionary<int, ItemType>
        {
            {1, ItemType.Lococ },
            {2, ItemType.Okyn },
            {3, ItemType.Okyn },
            {4, ItemType.Okyn },
            {5, ItemType.Okyn },
            {6, ItemType.Ocetr },
            {7, ItemType.Ygol },
            {8, ItemType.Chyka },
            {9, ItemType.Chyka },
            {10, ItemType.Chyka },
        };

        public static Dictionary<int, ItemType> FishItems2 = new Dictionary<int, ItemType>
        {
            {1, ItemType.Koroska },
            {2, ItemType.Koroska },
            {3, ItemType.Koroska },
            {4, ItemType.Lococ },
            {5, ItemType.Okyn },
            {6, ItemType.Okyn },
            {7, ItemType.Okyn },
            {8, ItemType.Ocetr },
            {9, ItemType.Skat },
            {10, ItemType.Skat },
            {12, ItemType.Ygol },
            {13, ItemType.Ygol },
            {15, ItemType.Chyka },
            {16, ItemType.Chyka },
            {17, ItemType.Chyka },
        };

        public static Dictionary<int, ItemType> FishItems3 = new Dictionary<int, ItemType>
        {
            {1, ItemType.Koroska },
            {2, ItemType.Kyndja },
            {3, ItemType.Lococ },
            {4, ItemType.Okyn },
            {5, ItemType.Okyn },
            {6, ItemType.Ocetr },
            {7, ItemType.Skat },
            {8, ItemType.Tunec },
            {9, ItemType.Ygol },
            {10, ItemType.Amyr },
            {11, ItemType.Chyka },
            {12, ItemType.Chyka },
        };

        public static Dictionary<int, ItemType> TypeRod = new Dictionary<int, ItemType>
        {
            {1, ItemType.Rod },
            {2, ItemType.RodUpgrade },
            {3, ItemType.RodMK2 },
        };

        public static ItemType GetSellingItemType(string name)
        {
            var type = ItemType.Naz;
            switch (name)
            {
                case "Корюшка":
                    type = ItemType.Koroska;
                    break;
                case "Кунджа":
                    type = ItemType.Kyndja;
                    break;
                case "Лосось":
                    type = ItemType.Lococ;
                    break;
                case "Окунь":
                    type = ItemType.Okyn;
                    break;
                case "Осётр":
                    type = ItemType.Ocetr;
                    break;
                case "Скат":
                    type = ItemType.Skat;
                    break;
                case "Тунец":
                    type = ItemType.Tunec;
                    break;
                case "Угорь":
                    type = ItemType.Ygol;
                    break;
                case "Чёрный амур":
                    type = ItemType.Amyr;
                    break;
                case "Щука":
                    type = ItemType.Chyka;
                    break;
            }
            return type;
        }

        public static Dictionary<string, int> ProductsSellPrice = new Dictionary<string, int>()
        {
            {"Корюшка",52},
            {"Кунджа",58},
            {"Лосось",50},
            {"Окунь",25},
            {"Осётр",30},
            {"Скат",63},
            {"Тунец",75},
            {"Угорь",25},
            {"Чёрный амур",53},
            {"Щука",32},
        };

        public static string GetNameByItemType(ItemType tupe)
        {
            string type = "nope";
            switch (tupe)
            {
                case ItemType.Koroska:
                    type = "Корюшка";
                    break;
                case ItemType.Kyndja:
                    type = "Кунджа";
                    break;
                case ItemType.Lococ:
                    type = "Лосось";
                    break;
                case ItemType.Okyn:
                    type = "Окунь";
                    break;
                case ItemType.Ocetr:
                    type = "Осётр";
                    break;
                case ItemType.Skat:
                    type = "Скат";
                    break;
                case ItemType.Tunec:
                    type = "Тунец";
                    break;
                case ItemType.Ygol:
                    type = "Угорь";
                    break;
                case ItemType.Amyr:
                    type = "Чёрный амур";
                    break;
                case ItemType.Chyka:
                    type = "Щука";
                    break;
            }

            return type;
        }

        private static nLog Log = new nLog("RodManager");

        private static int lastRodID = -1;

        [ServerEvent(Event.ResourceStart)]

        public void onResourceStart()
        {
            try
            {
                var result = MySQL.QueryRead($"SELECT * FROM rodings");
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB rod return null result.", nLog.Type.Warn);
                    return;
                }
                foreach (DataRow Row in result.Rows)
                {
                    Vector3 pos = JsonConvert.DeserializeObject<Vector3>(Row["pos"].ToString());

                    Roding data = new Roding(Convert.ToInt32(Row["id"]), pos, Convert.ToInt32(Row["radius"]));
                    int id = Convert.ToInt32(Row["id"]);
                    lastRodID = id;
                }
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"RODINGS\":\n" + e.ToString(), nLog.Type.Error);
            }
        }


        public static void createRodAreaCommand(Player player, float radius)
        {
            if (!Group.CanUseCmd(player, "createbusiness")) return;

            var pos = player.Position;
            pos.Z -= 1.12F;

            ++lastRodID;
            Roding biz = new Roding(lastRodID, pos, radius);

            MySQL.Query($"INSERT INTO rodings (id, pos, radius) " + $"VALUES ({lastRodID}, '{JsonConvert.SerializeObject(pos)}', {radius})");

        }

        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public static void setallow(Player player)
        {
            player.SetData("FISHING", true);
            Main.OnAntiAnim(player);
            Trigger.ClientEvent(player, "freeze", true);
            player.PlayAnimation("amb@world_human_stand_fishing@base", "base", 31);
            BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_fishing_rod_01"), 60309, new Vector3(0.03, 0, 0.02), new Vector3(0, 0, 50));
            NAPI.Task.Run(() => {
                try
                {
                    if (player != null && Main.Players.ContainsKey(player))
                    {
                        RodManager.allowfish(player);
                    }
                }
                catch { }
            }, 18000);
        }

        public static void allowfish(Player player)
        {
            player.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", 31);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Что-то клюнуло", 1000);
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("client::fishstart");

            });

            player.SetData("KD", Timers.StartOnceTask(9000, () =>
            {
                try
                {
                    if (player != null && Main.Players.ContainsKey(player))
                    {
                        if (player.HasData("FISHING") && player.GetData<bool>("FISHING"))
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Рыба сошла с крючка!", 3000);
                            NAPI.Task.Run(() => {
                                player.TriggerEvent("client::stopfish");
                            });
                            RodManager.crashpros(player);
                        }
                    }
                }
                catch { }
            }));
        }

        public static void crashpros(Player player)
        {
            NAPI.Task.Run(() => {
                try
                {
                    player.PlayAnimation("rcmcollect_paperleadinout@", "kneeling_arrest_get_up", 33);
                    player.StopAnimation(); //Вдруг если не сработает выше
                    Main.OffAntiAnim(player);
                    BasicSync.DetachObject(player);
                    player.SetData("FISHING", false);
                    Trigger.ClientEvent(player, "freeze", false);
                    string time = player.GetData<string>("KD");
                    Timers.Stop(time);
                }
                catch { }
            });
        }

        [RemoteEvent("server::givefish")]
        public static void giveRandomFish(Player player)
        {
            try
            {
                var tryAdd = nInventory.TryAdd(player, new nItem(ItemType.Ocetr));
                if (tryAdd == -1 || tryAdd > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                    RodManager.crashpros(player);
                    return;
                }
                if (player.GetData<int>("FISHLEVEL") == 1)
                {
                    var rnd = new Random();
                    int fishco = rnd.Next(1, RodManager.FishItems1.Count);
                    nInventory.Add(player, new nItem(RodManager.FishItems1[fishco], 1));
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы поймали рыбу {GetNameByItemType(RodManager.FishItems1[fishco])}", 3000);
                }
                if (player.GetData<int>("FISHLEVEL") == 2)
                {
                    var rnd = new Random();
                    int fishco = rnd.Next(1, RodManager.FishItems2.Count);
                    nInventory.Add(player, new nItem(RodManager.FishItems2[fishco], 1));
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы поймали рыбу {GetNameByItemType(RodManager.FishItems2[fishco])}", 3000);
                }
                if (player.GetData<int>("FISHLEVEL") == 3)
                {
                    var rnd = new Random();
                    int fishco = rnd.Next(1, RodManager.FishItems3.Count);
                    nInventory.Add(player, new nItem(RodManager.FishItems3[fishco], 1));
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы поймали рыбу {GetNameByItemType(RodManager.FishItems3[fishco])}", 3000);
                    /*if (FishItems3[fishco] == ItemType.Amyr)
                    {
                        Utils.QuestsManager.AddQuestProcess(player, 4);
                    }   */
                }
            }
            catch { }
            RodManager.crashpros(player);
        }

        public static void useInventory(Player player, int level)
        {
            if (player.IsInVehicle)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не должны находится в машине!", 3000);
                GUI.Dashboard.Close(player);
                return;
            }
            if (player.GetData<bool>("FISHING") == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже рыбачите!", 3000);
                return;
            }
            var aItem = nInventory.Find(Main.Players[player].UUID, ItemType.Naz);
            if (aItem == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас нет наживки", 3000);
                return;
            }
            if (!player.HasData("ALLOWFISHING") || player.GetData<bool>("ALLOWFISHING") == false)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В данном месте нельзя рыбачить", 3000);
                return;
            }
            var rndf = new Random();
            nInventory.Remove(player, ItemType.Naz, 1);
            player.SetData("FISHLEVEL", level);
            RodManager.setallow(player);
            Commands.RPChat("me", player, $"Начал(а) рыбачить");
        }

        public class Roding
        {
            public int ID { get; set; }
            public float Radius { get; set; }
            public Vector3 AreaPoint { get; set; }

            [JsonIgnore]
            private Blip blip = null;
            [JsonIgnore]
            private ColShape shape = null;

            public Roding(int id, Vector3 areapoint, float radius)
            {
                ID = id;
                AreaPoint = areapoint;
                Radius = radius;
                blip = NAPI.Blip.CreateBlip(68, AreaPoint, 0.8f, 4, "Зона для рыбалки", 255, 0, true, dimension: 0);
                shape = NAPI.ColShape.CreateCylinderColShape(AreaPoint, Radius, 6, 0);
                shape.OnEntityEnterColShape += (s, entity) =>
                {
                    try
                    {
                        entity.SetData("ALLOWFISHING", true);
                        Trigger.ClientEvent(entity, "UpdateFishZone", true);
                        Trigger.ClientEvent(entity, "safeZone", true);
                        Trigger.ClientEvent(entity, "UpdateGreenZone", true);
                    }
                    catch (Exception e) { Console.WriteLine("shape.OnEntityEnterColshape: " + e.Message); }
                };
                shape.OnEntityExitColShape += (s, entity) =>
                {
                    try
                    {
                        entity.SetData("ALLOWFISHING", false);
                        Trigger.ClientEvent(entity, "UpdateFishZone", false);
                        Trigger.ClientEvent(entity, "safeZone", false);
                        Trigger.ClientEvent(entity, "UpdateGreenZone", false);
                    }
                    catch (Exception e) { Console.WriteLine("shape.OnEntityEnterColshape: " + e.Message); }
                };
            }

        }



    }
}
