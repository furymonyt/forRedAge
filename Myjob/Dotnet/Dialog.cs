
                case 25:
                    Trigger.ClientEvent(player, "client::closedialog2");
                    Jobs.WorkManager.openGoPostalStart(player);
                    return;
                case 21:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.WorkManager.SetWorkId(player);
                    return;
                case 22:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.WorkManager.callback_gpStartMenu(player, 0);
                    return;
                case 23:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.WorkManager.callback_gpStartMenu(player, 1);
                    return;
                case 24:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.WorkManager.callback_gpStartMenu(player, 2);
                    return;
                case 50:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.Builder.SetWorkId(player);
                    return;
                case 51:
                    Trigger.ClientEvent(player, "client::closedialog2");
                    Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Дело простое, нужно всего-лишь таскать мешки по точкам и за это будешь получать немного зеленых", (new QuestAnswer("Понял", 53), 0));
                    return;
                case 52:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.Builder.SetWorkState(player);
                    return;
                case 53:
                    Trigger.ClientEvent(player, "client::closedialog2");
                    Jobs.Builder.OpenMenu(player);
                    return;
                
               case 500:
                    Trigger.ClientEvent(player, "client::closedialog");
Jobs.Electrition.SetWorkId(player);
return;
                case 501:
                    Trigger.ClientEvent(player, "client::closedialog");
Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Дело простое, нужно всего-лишь чинить электронику по точкам и за это будешь получать немного зеленых", (new QuestAnswer("Понял", 53), 0));
return;
                case 502:
                    Trigger.ClientEvent(player, "client::closedialog");
Jobs.Electrition.SetWorkState(player);
return;
                case 503:
                    if (Main.Players[player].WorkID != 9)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 51), new QuestAnswer("Устроиться", 50), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    else if (Main.Players[player].WorkID == 9 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == false)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 51), new QuestAnswer("Уволиться", 50), new QuestAnswer("Начать рабочий день", 52), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    else if (Main.Players[player].WorkID == 9 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == true)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 51), new QuestAnswer("Уволиться", 50), new QuestAnswer("Закончить день", 52), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    return;
                case 504:
                    if (Main.Players[player].WorkID != 11)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 501), new QuestAnswer("Устроиться", 500), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    else if (Main.Players[player].WorkID == 11 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == true)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 501), new QuestAnswer("Уволиться", 500), new QuestAnswer("Закончить день", 502), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    else if (Main.Players[player].WorkID == 11 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == false)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 501), new QuestAnswer("Уволиться", 500), new QuestAnswer("Начать рабочий день", 502), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    return;
                case 505:
                    if (Main.Players[player].WorkID != 12)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 508), new QuestAnswer("Устроиться", 506), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    else if (Main.Players[player].WorkID == 12 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == true)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 508), new QuestAnswer("Уволиться", 506), new QuestAnswer("Закончить день", 507), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    else if (Main.Players[player].WorkID == 12 && player.HasData("ON_WORK") && player.GetData<bool>("ON_WORK") == false)
                    {
                        Trigger.ClientEvent(player, "client::closedialog2");
                        Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Привет, не хочешь заработать немного зеленых, работая на стройке?", (new QuestAnswer("Как тут работать?", 508), new QuestAnswer("Уволиться", 506), new QuestAnswer("Начать рабочий день", 507), new QuestAnswer("В следующий раз", 2)));
                        return;
                    }
                    return;
                case 506:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.Crow.SetWorkId(player);
                    return;
                case 508:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Trigger.ClientEvent(player, "client::opendialogmenu", true, "Рон", "Прораб", "Дело простое, нужно всего-лишь чинить крышу с помощью молотка по точкам и за это будешь получать немного зеленых", (new QuestAnswer("Понял", 53), 0));
                    return;
                case 507:
                    Trigger.ClientEvent(player, "client::closedialog");
                    Jobs.Crow.SetWorkState(player);
                    return;
                case 103:
                    Trigger.ClientEvent(player, "client::closedialog");
                    item = nInventory.Find(Main.Players[player].UUID, ItemType.MoneyHeist);
                    if (item != null)
                    {
                        nInventory.Remove(player, ItemType.MoneyHeist, item.Count);
                        Dashboard.sendItems(player);
                        var payment = item.Count * 0.85;
                        var payment2 = item.Count * 0.15;

                        Fractions.Stocks.fracStocks[Main.Players[player].FractionID].Materials += Convert.ToInt32(payment2);
                        MoneySystem.Wallet.Change(player, Convert.ToInt32(payment));
                        Notify.Succ(player, $"Вы получили {payment}$");
                        return;
                    }
                    else
                    {
                        Notify.Error(player, "У Вас нет пачек с деньгами");
                    }
                    return;
            }
        }
    }
}
