mp.peds.new(0x81F74DE7, new mp.Vector3(-5.401763, -2541.6501, -9.93951), -11.5896, 0); //Meo

mp.events.add("DrugOpenMenu2", (json) => {
	if (!loggedin || chatActive || editing || cuffed) return;
	global.menuOpen();
	global.menuOrange = mp.browsers.new('http://package/browser/modules/Jobs/Drugs/index.html');
	global.menuOrange.active = true;
	global.menuOrange.execute(`init()`);
});

mp.events.add("closeOpenMenu2", (count) => {
	global.menuClose();
	global.menuOrange.active = false;
	global.menuOrange.destroy();
	mp.events.callRemote("DrugStopWork", count);
});