// Ищешь  case "mayor_put": и после return; вставляешь

case "bitcoin_sell":
                        amount = 0;
                        try
                        {
                            amount = Convert.ToInt32(text);
                            if (amount <= 0) return;
                        }
                        catch { return; }
                        MoneySystem.Bitcoin.SellBitcoin(player);
                        return;
                    case "bitcoin_buy":
                        amount = 0;
                        try
                        {
                            amount = Convert.ToInt32(text);
                            if (amount <= 0) return;
                        }
                        catch { return; }
                        MoneySystem.Bitcoin.BuyBitcoin(player);
                        return;
						
// Ищешь #region SMS и над ним вставляешь

 #region BitcoinMenuPhone
        public static void OpenBitcoinMenu(Player player)
        {
            Menu menu = new Menu("citymanage", false, false);
            menu.Callback = callback_bitcoinmenu;

            Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
            menuItem.Text = "Биткоин";
            menu.Add(menuItem);

            MoneySystem.Bank.Data bankAcc = MoneySystem.Bank.Accounts.FirstOrDefault(a => a.Value.Holder == player.Name).Value;
            int bankMoney = 0;
            if (bankAcc != null) bankMoney = (int)bankAcc.Balance;

            menuItem = new Menu.Item("info", Menu.MenuItem.Card);
            menuItem.Text = $"Кол-во биткоинов: {Players[player].Bitcoin}";
            menu.Add(menuItem);

            menuItem = new Menu.Item("sell", Menu.MenuItem.Button);
            menuItem.Text = "Продать биткоин";
            menu.Add(menuItem);

            menuItem = new Menu.Item("buy", Menu.MenuItem.Button);
            menuItem.Text = "Купить биткоин";
            menu.Add(menuItem);

            menuItem = new Menu.Item("back", Menu.MenuItem.Button);
            menuItem.Text = "Назад";
            menu.Add(menuItem);

            menu.Open(player);
        }
        private static void callback_bitcoinmenu(Player player, Menu menu, Menu.Item item, string eventName, dynamic data)
        {
            switch (item.ID)
            {
                case "sell":
                    MenuManager.Close(player);
                    Trigger.ClientEvent(player, "openInput", "Продать биткоин", "Количество", 5, "bitcoin_sell");
                    return;
                case "buy":
                    MenuManager.Close(player);
                    Trigger.ClientEvent(player, "openInput", "Купить биткоин", "Количество", 5, "bitcoin_buy");
                    return;
                case "back":
                    Task pmenu = OpenPlayerMenu(player);
                    return;
            }
        }
        #endregion
		
		
// Ищешь private static void callback_mainmenu(Player player, Menu menu, Menu.Item item, string eventName, dynamic data)

// В нем находишь 
case "biz":
                    BusinessManager.OpenBizListMenu(player);
                    return;
// И после вставляешь


                case "bitcoin":
                    OpenBitcoinMenu(player);
                    return;
					
// Ищешь public static async Task OpenPlayerMenu(Player player) 

// В нем находишь

Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
            menuItem.Text = "Меню";
            menu.Add(menuItem);
			
// Ниже вставляешь

menuItem = new Menu.Item("bitcoin", Menu.MenuItem.Button);
            menuItem.Text = "Биткоин";
            menu.Add(menuItem);
			
// Ищешь 
case "CARWASH_PAY":
                            BusinessManager.Carwash_Pay(player);
                            return;
							
// И под ним вставляешь


                        case "BUY_BITCOIN":
                            MoneySystem.Bitcoin.BuyBitcoin(player);
                            return;