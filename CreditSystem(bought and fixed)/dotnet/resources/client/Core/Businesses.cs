//���� 
Menu menu = new Menu("bizsell", false, false);
menu.Callback = callback_bizsell;

Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
menuItem.Text = "�������";
menu.Add(menuItem);

var bizID = player.GetData<int>("SELECTEDBIZ");
Business biz = BizList[bizID];
var price = biz.SellPrice / 100 * 70;
menuItem = new Menu.Item("govsell", Menu.MenuItem.Button);
menuItem.Text = $"������� ����������� (${price})";
menu.Add(menuItem);

menuItem = new Menu.Item("back", Menu.MenuItem.Button);
menuItem.Text = "�����";
menu.Add(menuItem);

menu.Open(player);
//������ ���� ���������
if (Main.Players[player].CreditMoney == 0)
{
    Menu menu = new Menu("bizsell", false, false);
    menu.Callback = callback_bizsell;

    Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
    menuItem.Text = "�������";
    menu.Add(menuItem);

    var bizID = player.GetData<int>("SELECTEDBIZ");
    Business biz = BizList[bizID];
    var price = biz.SellPrice / 100 * 70;
    menuItem = new Menu.Item("govsell", Menu.MenuItem.Button);
    menuItem.Text = $"������� ����������� (${price})";
    menu.Add(menuItem);

    menuItem = new Menu.Item("back", Menu.MenuItem.Button);
    menuItem.Text = "�����";
    menu.Add(menuItem);

    menu.Open(player);
}
else
{
    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"�� �� ������ ������� ������, �.�. �� ��������� � ������.", 3000);
    return;
}

//���� 
if (!Main.Players.ContainsKey(player) || !Main.Players.ContainsKey(target)) return;

if (player.Position.DistanceTo(target.Position) > 2)
{
    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"����� ������� ������", 3000);
    return;
}

if (Main.Players[player].BizIDs.Count == 0)
{
    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"� ��� ��� �������", 3000);
    return;
}

if (Main.Players[target].BizIDs.Count >= Group.GroupMaxBusinesses[Main.Accounts[target].VipLvl])
{
    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"����� ����� �������� ��������", 3000);
    return;
}

var biz = BizList[Main.Players[player].BizIDs[0]];
if (price < biz.SellPrice / 2 || price > biz.SellPrice * 3)
{
    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"���������� ������� ������ �� ����� ����. ������� ���� �� {biz.SellPrice / 2}$ �� {biz.SellPrice * 3}$", 3000);
    return;
}

if (Main.Players[target].Money < price)
{
    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"� ������ ������������ �����", 3000);
    return;
}

Trigger.ClientEvent(target, "openDialog", "BUSINESS_BUY", $"{player.Name} ��������� ��� ������ {BusinessTypeNames[biz.Type]} �� ${price}");
target.SetData("SELLER", player);
target.SetData("SELLPRICE", price);
target.SetData("SELLBIZID", biz.ID);

Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"�� ���������� ������ ({target.Value}) ������ ��� ������ �� {price}$", 3000);

//������ ���� ���������
if (Main.Players[player].CreditMoney == 0)
{
    if (!Main.Players.ContainsKey(player) || !Main.Players.ContainsKey(target)) return;

    if (player.Position.DistanceTo(target.Position) > 2)
    {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"����� ������� ������", 3000);
        return;
    }

    if (Main.Players[player].BizIDs.Count == 0)
    {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"� ��� ��� �������", 3000);
        return;
    }

    if (Main.Players[target].BizIDs.Count >= Group.GroupMaxBusinesses[Main.Accounts[target].VipLvl])
    {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"����� ����� �������� ��������", 3000);
        return;
    }

    var biz = BizList[Main.Players[player].BizIDs[0]];
    if (price < biz.SellPrice / 2 || price > biz.SellPrice * 3)
    {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"���������� ������� ������ �� ����� ����. ������� ���� �� {biz.SellPrice / 2}$ �� {biz.SellPrice * 3}$", 3000);
        return;
    }

    if (Main.Players[target].Money < price)
    {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"� ������ ������������ �����", 3000);
        return;
    }

    Trigger.ClientEvent(target, "openDialog", "BUSINESS_BUY", $"{player.Name} ��������� ��� ������ {BusinessTypeNames[biz.Type]} �� ${price}");
    target.SetData("SELLER", player);
    target.SetData("SELLPRICE", price);
    target.SetData("SELLBIZID", biz.ID);

    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"�� ���������� ������ ({target.Value}) ������ ��� ������ �� {price}$", 3000);
}
else
{
    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"�� �� ������ ������� ������, �.�. �� ��������� � ������.", 3000);
    return;
}