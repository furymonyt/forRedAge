using System;
using System.Collections.Generic;
using Alyx;
using AlyxSDK;
using GTANetworkAPI;
class Cinema : Script
{

    public static string cinema_url = "";
    public static int cinema_time = 0;
    public static bool cinema_playing = false;
    public static bool cinema_open = true;

    private static nLog RLog = new nLog("Cinema");

    public static Vector3[] CinemaPositions = new Vector3[]
    {
        new Vector3(-1423.4061279296875, -215.29371643066406, 46.500423431396484),
        new Vector3(337.04785, 177.33636, 103.1125),
        new Vector3(393.90756, -711.7669, 29.284852),
    };


    public static List<Vector3> seats = new List<Vector3>();

    [ServerEvent(Event.ResourceStart)]
    public static void OnResourceStart()
    {
        try
        {
            var shapecinema = NAPI.ColShape.CreateCylinderColShape(CinemaPositions[0], 2, 3, 0); shapecinema.OnEntityEnterColShape += (shape, player) => {try{player.SetData("INTERACTIONCHECK", 910);}catch (Exception ex) { Console.WriteLine("shape.OnEntityEnterColShape: " + ex.Message); }};
            shapecinema.OnEntityExitColShape += (shape, player) =>{try{player.SetData("INTERACTIONCHECK", 0);}catch (Exception ex) { Console.WriteLine("shape.OnEntityExitColShape: " + ex.Message); }};

            var shapecinema2 = NAPI.ColShape.CreateCylinderColShape(CinemaPositions[1], 2, 3, 0); shapecinema2.OnEntityEnterColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 911); } catch (Exception ex) { Console.WriteLine("shape.OnEntityEnterColShape: " + ex.Message); } };
            shapecinema2.OnEntityExitColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 0); } catch (Exception ex) { Console.WriteLine("shape.OnEntityExitColShape: " + ex.Message); } };

            var shapecinema3 = NAPI.ColShape.CreateCylinderColShape(CinemaPositions[2], 2, 3, 0); shapecinema3.OnEntityEnterColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 912); } catch (Exception ex) { Console.WriteLine("shape.OnEntityEnterColShape: " + ex.Message); } };
            shapecinema3.OnEntityExitColShape += (shape, player) => { try { player.SetData("INTERACTIONCHECK", 0); } catch (Exception ex) { Console.WriteLine("shape.OnEntityExitColShape: " + ex.Message); } };

            foreach (Vector3 vec in CinemaPositions)
            {
                Blip temp_blip = null;
                temp_blip = NAPI.Blip.CreateBlip(vec);
                temp_blip.Sprite = 135;
                temp_blip.Name = "Кинотеатр";
                temp_blip.Color = 4;
                temp_blip.Scale = 0.8f;
                temp_blip.ShortRange = true;
                temp_blip.Dimension = 0;

                NAPI.Marker.CreateMarker(1, vec - new Vector3(0, 0, 1.0), new Vector3(), new Vector3(), 0.8f, new Color(67, 140, 239, 200));
            }
            RLog.Write("Loaded", nLog.Type.Info);
        }
        catch (Exception e) { RLog.Write(e.ToString(), nLog.Type.Error); }
    }
    public Cinema()
    { 
        seats.Add(new Vector3(-1418.052, -247.6875, 16.79762));
        seats.Add(new Vector3(-1419.253, -247.6236, 16.79766));
        seats.Add(new Vector3(-1421.659, -247.3815, 16.79851));
        seats.Add(new Vector3(-1423.86, -247.2797, 16.79926));
        seats.Add(new Vector3(-1427.273, -247.3671, 16.79961));
        seats.Add(new Vector3(-1430.556, -247.2636, 16.79898));
        seats.Add(new Vector3(-1432.485, -247.544, 16.79794));
        seats.Add(new Vector3(-1432.636, -249.7136, 16.7938));
        seats.Add(new Vector3(-1429.784, -249.8186, 16.79434));
        seats.Add(new Vector3(-1427.626, -249.6948, 16.7951));
        seats.Add(new Vector3(-1424.524, -249.5078, 16.79521));
        seats.Add(new Vector3(-1422.475, -249.7936, 16.79416));
        seats.Add(new Vector3(-1419.898, -249.7854, 16.79353));
        seats.Add(new Vector3(-1419.298, -251.0738, 16.79095));
        seats.Add(new Vector3(-1421.584, -250.7939, 16.79206));
        seats.Add(new Vector3(-1423.801, -250.8243, 16.79255));
        seats.Add(new Vector3(-1426.224, -250.7932, 16.7932));
        seats.Add(new Vector3(-1428.242, -250.6987, 16.79305));
        seats.Add(new Vector3(-1430.339, -250.802, 16.79233));
        seats.Add(new Vector3(-1432.28, -251.1464, 16.79117));
        seats.Add(new Vector3());
        CinemaTime();
    }

    public void CinemaTime()
    {
        Timers.StartTask("CinemaTimer", 1000, () =>
        {
            if (cinema_playing == true)
            {
                cinema_time++;
            }
        });
    }

    public static void EnterCinema(Player player, int id)
    {
        if (cinema_open == false && Main.Players[player].AdminLVL == 0)
        {
            Notify.Succ(player, "Кинотеатр закрыт", 3000);
            return;
        }
        player.SetData("CinemaNumPosition", CinemaPositions[id]);
        player.TriggerEvent("enterCinema");
        cinema_open = true;
        player.Dimension = 100;
        Main.Players[player].EnterCinema = 1;
        return;
    }

    [RemoteEvent("requestCinemaScreen")]
    public static void requestCinemaScreen(Player player)
    {

        Random rnd = new Random();
        int random_seat = rnd.Next(0, seats.Count - 1);
        player.Position = seats[random_seat];
        player.Rotation = new Vector3(0,0,180f);
        player.SetData("Cinema", true);
        player.SetData("statuscinema", true);

        if (Main.Players[player].AdminLVL > 0)
        {
            if (cinema_url == "null")
            {
                player.TriggerEvent("showCinemaScreen", Main.Players[player].FirstName, cinema_time, "null", true);

            }
            else
            {
                player.TriggerEvent("showCinemaScreen", Main.Players[player].FirstName, cinema_time, cinema_url, true);
            }
        }
        else
        {
            if (cinema_url == "null")
            {
                player.TriggerEvent("showCinemaScreen", Main.Players[player].FirstName, cinema_time, "null", false);

            }
            else
            {
                player.TriggerEvent("showCinemaScreen", Main.Players[player].FirstName, cinema_time, cinema_url, false);
            }
        }
    }

    [RemoteEvent("exitCinema")]
    public static void exitCinema(Player player)
    {
        player.Position = player.GetData<Vector3>("CinemaNumPosition");
        player.SetData("Cinema", false);
        player.SetData("statuscinema", false);
        player.Dimension = 0;
        Main.Players[player].EnterCinema = -1;
    }

    [RemoteEvent("Server:Cinema_Open")]
    public static void CinemaOpen(Player player, bool toggle)
    {
        if(toggle == true)
        {
            if(cinema_open == true)
            {
                Notify.Succ(player, "Киноетатр уже открыт", 3000);
                return;
            }
        
            cinema_open = true;
        }
        else
        {
            if (cinema_open == false)
            {
                Notify.Error(player, "Киноетатр уже закрыт", 3000);
                return;
            }

            cinema_open = false;
        }
    }

    [RemoteEvent("addCinemaVideo")]
    public static void addCinemaVideo(Player player, string url)
    {
        string url_replace = url.Replace("https://www.youtube.com/watch?v=", "");
        cinema_url = url_replace;
        cinema_time = 0;
        cinema_playing = true;
        foreach (var target in API.Shared.GetAllPlayers())
        {
            if (target.GetData<bool>("statuscinema") == true && target.GetData<bool>("Cinema") == true)
            {
                target.TriggerEvent("showCinemaScreen", Main.Players[player].FirstName, cinema_time, url_replace);
            }
        }    
    }
}

