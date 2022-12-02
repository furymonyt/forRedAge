// Ищем:
public static void CMD_sheriffAccept(Player player, int id)
{
    try
    {
        if (Main.GetPlayerByID(id) == null)
        {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
            return;
        }
        Fractions.Sheriff.acceptCall(player, Main.GetPlayerByID(id));
    }
    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
}

//После, вставляем:

[Command("removecredit")] //Команда для полной очистки чела от кредитов
public static void CMD_removedebt(Player player, int id)
{
    try
    {
        if (!Group.CanUseCmd(player, "removecredit")) return;
        var target = Main.GetPlayerByID(id);
        if (target == null)
        {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден.", 3000);
            return;
        }

        if (!Main.Players.ContainsKey(target)) return;

        if (Main.Players[target].CreditMoney <= 0)
        {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет кредитов.", 3000);
            return;
        }

        Main.Players[target].CreditMoney = 0;
        Dashboard.sendStats(target);

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли с игрока {target.Name} кредиты.", 3000);
        Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"С вас сняли все кредиты!", 3000);
        GameLog.Admin($"{player.Name}", $"removecredit", $"{target.Name}");
        MySQL.Query($"UPDATE `characters` SET `creditmoney`= {Main.Players[player].CreditMoney}");
        MySQL.Query($"UPDATE `characters` SET `allowcredit`= {Main.Players[player].AllowCredit = 0}");
        MySQL.Query($"UPDATE `characters` SET `credittime`='{MySQL.ConvertTime(DateTime.Now)}'");
        Log.Write($"Администратор {player.Name} снял все кредиты с игрока {target.Name}", nLog.Type.Warn);
    }
    catch (Exception e) { Log.Write("removecredit: " + e.Message, nLog.Type.Error); }
}
