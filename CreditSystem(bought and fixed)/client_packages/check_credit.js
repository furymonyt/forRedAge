global.checkMenu = mp.browsers.new('package://cef/CreditSystem/checkSystem.html');

mp.events.add("openCheckMenu", () => {
	if(!global.loggedin) return;
	global.menuOpen();
	global.checkMenu.active = true;
	global.checkMenu.seen_content1 = false;
	setTimeout(function () { 
		global.checkMenu.execute(`checkMenu.active=true`);
	}, 250);	
});

mp.events.add("closeCheckMenu", () => {
	setTimeout(function () { 
		global.menuClose();
		global.checkMenu.active = false;
	}, 100);
});

mp.events.add('playerName', (playerName) => {
	global.checkMenu.execute(`checkMenu.playerName="${playerName}"`);
});

mp.events.add('propertyCheck', (type) => {
    mp.events.callRemote('propertyCheck', type);
});