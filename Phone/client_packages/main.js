// Ищем тут mp.keys.bind(Keys.VK_M, false, function () {       и полностью этот бинд меняем на мой

mp.keys.bind(Keys.VK_M, false, function () {

    if (!loggedin || chatActive || editing || global.menuCheck() || cuffed || localplayer.getVariable('InDeath') == true || new Date().getTime() - lastCheck < 400) return;
    if (global.phoneOpen)
    {
        mp.game.invoke ('0x3BC861DF703E5097', mp.players.local.handle, true);
        mp.events.callRemote("closePlayerMenu");
        global.phoneOpen = 0;
    }
    else
    {
        mp.events.callRemote('openPlayerMenu');
        lastCheck = new Date().getTime();
        global.phoneOpen = 1;
    }
});