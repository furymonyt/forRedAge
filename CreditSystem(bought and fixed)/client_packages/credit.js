global.creditMenu = mp.browsers.new('package://cef/CreditSystem/CreditSystem.html');

mp.events.add("openCreditMenu", () => {
	if(!global.loggedin) return;
	global.menuOpen();
	global.creditMenu.active = true;
	setTimeout(function () { 
		global.creditMenu.execute(`creditMenu.active=true`);
	}, 250);	
});

mp.events.add("closeCreditMenu", () => {
	setTimeout(function () { 
		global.menuClose();
		global.creditMenu.active = false;
	}, 100);
});

mp.events.add('creditOperation', (creditSum, give) => {
    mp.events.callRemote('creditOperation', creditSum, give);
});

mp.events.add('debt', (debt) => {
	creditMenu.execute(`creditMenu.debt="${debt}"`);
});

mp.events.add('playerMoney', (playerMoney) => {
	creditMenu.execute(`creditMenu.playerMoney="${playerMoney}"`)
});

mp.events.add('playerName', (playerName) => {
	creditMenu.execute(`creditMenu.playerName="${playerName}"`)
});