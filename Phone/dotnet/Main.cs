// Ищем тут
[RemoteEvent("openPlayerMenu")]
        public async Task ClientEvent_openPlayerMenu(Player player, params object[] arguments)
        {
			
// И весь этот ивент заменяем на мой

[RemoteEvent("openPlayerMenu")]
        public async Task ClientEvent_openPlayerMenu(Player player, params object[] arguments)
        {
            try
            {
                var aItem = nInventory.Find(Players[player].UUID, ItemType.Phone);
                if (aItem == null || aItem.Count <= -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету телефона. Купите его в магазине 24/7.", 2000);
                    return;
                }
                if (aItem.Count >= 0)
                {
                    await OpenPlayerMenu(player);
                    uint phoneHash = NAPI.Util.GetHashKey("prop_amb_phone");
                    player.PlayAnimation("anim@cellphone@in_car@ds", "cellphone_text_read_base", 30);
                    if (!player.IsInVehicle)
                    {
                        BasicSync.AttachObjectToPlayer(player, phoneHash, 6286, new Vector3(0.11, 0.03, -0.01), new Vector3(85, -15, 120));
                    }
                }
            }
            catch (Exception e) { Log.Write("openPlayerMenu: " + e.Message, nLog.Type.Error); }
        }
		
// Затем ниже видим [RemoteEvent("closePlayerMenu")] его тоже полностью заменяем на мой

 [RemoteEvent("closePlayerMenu")]
        public void ClientEvent_closePlayerMenu(Player player, params object[] arguments)
        {
            try
            {
                MenuManager.Close(player);
                if (player == null) return;
                if (!player.IsInVehicle) player.StopAnimation();
                else player.SetData("ToResetAnimPhone", true);
                OffAntiAnim(player);
                Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                BasicSync.DetachObject(player);
                return;
            }
            catch (Exception e)
            {
                Log.Write("closePlayerMenu: " + e.Message, nLog.Type.Error);
            }
        }