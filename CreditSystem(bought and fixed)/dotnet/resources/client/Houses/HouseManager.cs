//Ищем
switch (Main.Accounts[player].VipLvl)
{
    case 0: // None
        price = Convert.ToInt32(house.Price * 0.6);
        break;
    case 1: // Bronze
        price = Convert.ToInt32(house.Price * 0.65);
        break;
    case 2: // Silver
        price = Convert.ToInt32(house.Price * 0.7);
        break;
    case 3: // Gold
        price = Convert.ToInt32(house.Price * 0.75);
        break;
    case 4: // Platinum
        price = Convert.ToInt32(house.Price * 0.8);
        break;
}

//Под ним видим
Trigger.ClientEvent(player, "openDialog", "HOUSE_SELL_TOGOV", $"Вы действительно хотите продать дом за ${price}?");
MenuManager.Close(player);
return;

//И вместо этого вставляем это
if (Main.Players[player].CreditMoney == 0)
{
    Trigger.ClientEvent(player, "openDialog", "HOUSE_SELL_TOGOV", $"Вы действительно хотите продать дом за ${price}?");
    MenuManager.Close(player);
    return;
}
else
{
    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете продать дом, т.к. он находится в залоге.", 5000);
    return;
}

//Ищем 
house.RemoveAllPlayers();
house.SetOwner(null);
house.PetName = "null";
Trigger.ClientEvent(player, "deleteCheckpoint", 333);
Trigger.ClientEvent(player, "deleteGarageBlip");
int price = 0;
switch (Main.Accounts[player].VipLvl)
{
    case 0: // None
        price = Convert.ToInt32(house.Price * 0.6);
        break;
    case 1: // Bronze
        price = Convert.ToInt32(house.Price * 0.65);
        break;
    case 2: // Silver
        price = Convert.ToInt32(house.Price * 0.7);
        break;
    case 3: // Gold
        price = Convert.ToInt32(house.Price * 0.75);
        break;
    case 4: // Platinum
        price = Convert.ToInt32(house.Price * 0.8);
        break;
}
MoneySystem.Wallet.Change(player, price);
GameLog.Money($"server", $"player({Main.Players[player].UUID})", Convert.ToInt32(house.Price * 0.6), $"houseSell({house.ID})");
Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали свой дом государству за {price}$", 3000);

//Вместо него вставляем
if (Main.Players[player].CreditMoney == 0)
{
    house.RemoveAllPlayers();
    house.SetOwner(null);
    house.PetName = "null";
    Trigger.ClientEvent(player, "deleteCheckpoint", 333);
    Trigger.ClientEvent(player, "deleteGarageBlip");
    int price = 0;
    switch (Main.Accounts[player].VipLvl)
    {
        case 0: // None
            price = Convert.ToInt32(house.Price * 0.6);
            break;
        case 1: // Bronze
            price = Convert.ToInt32(house.Price * 0.65);
            break;
        case 2: // Silver
            price = Convert.ToInt32(house.Price * 0.7);
            break;
        case 3: // Gold
            price = Convert.ToInt32(house.Price * 0.75);
            break;
        case 4: // Platinum
            price = Convert.ToInt32(house.Price * 0.8);
            break;
    }
}
else
{
    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете продать дом, т.к. он находится в залоге.", 5000);
    return;
}