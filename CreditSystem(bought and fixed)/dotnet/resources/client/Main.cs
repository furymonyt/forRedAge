// Ищем метод public void ClientEvent_interactionPressed(Player player, params object[] arguments)
//Там же ищем switch(id) и в любом месте вставляем
      
case 1244:
    MoneySystem.CreditSystem.OpenCheckMenu(player);
    return;
case 7123:
    MoneySystem.CreditSystem.OpenCreditMenu(player);
    return;


//Ищем метод public static void payDayTrigger()
//Под ним ищем этот код:

if (Players[player].HotelID != -1)
{
    Players[player].HotelLeft--;
    if (Players[player].HotelLeft <= 0)
    {
        Hotel.MoveOutPlayer(player);
        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Вас выселили из отеля за неуплату", 3000);
    }
}

//Прямо после него пишем:

MoneySystem.CreditSystem.TakeMoney(player);
if (Players[player].CreditMoney <= 0)
{
    MoneySystem.CreditSystem.ReminderCredit(player);
}
