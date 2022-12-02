mp.peds.new(0x76284640, new mp.Vector3(-1874.2358, 2070.4126, 140.97767), -16.63, 0); //Meo

mp.events.add("VineOpenMenu2", (json) => {
	if (!loggedin || chatActive || editing || cuffed) return;
	global.menuOpen();
	global.menuOrange = mp.browsers.new('http://package/browser/modules/Jobs/Vine/index.html');
	global.menuOrange.active = true;
	global.menuOrange.execute(`init()`);
});

mp.events.add("closeOpenMenu3", (count) => {
	global.menuClose();
	global.menuOrange.active = false;
	global.menuOrange.destroy();
	mp.events.callRemote("VineStopWork", count);
});