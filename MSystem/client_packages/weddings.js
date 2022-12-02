global.wedding = mp.browsers.new('package://cef/WeddingSystem/wedding.html');

mp.events.add("openWeddingsMenu", () => {
	if(!global.loggedin) return;
	global.menuOpen();
	global.wedding.active = true;
	setTimeout(function () { 
		global.wedding.execute(`wedding.active=true`);
	}, 250);	
});

mp.events.add("closeWeddingsMenu", () => {
	setTimeout(function () { 
		global.menuClose();
		global.wedding.active = false;
	}, 100);
});

mp.events.add('WeddingName', (index, name) => {
    mp.events.callRemote('WeddingName', index, name);
});

mp.events.add('WeddingSurname', (index, surname) => {
    mp.events.callRemote('WeddingSurname', index, surname);
});

mp.events.add('WeddingDivorce', (index) => {
    mp.events.callRemote('WeddingDivorce', index);
});

mp.events.add('Yes', (index) => {
    mp.events.callRemote('YesOrNo', index);
});

mp.events.add('No', (index) => {
    mp.events.callRemote('YesOrNo', index);
});

mp.events.add('applications', (application) => {
	wedding.execute(`wedding.application="${application}"`)
});

mp.events.add('applName', (applName) => {
	wedding.execute(`wedding.applName="${applName}"`)
});

mp.events.add('applSurname', (applSurname) => {
	wedding.execute(`wedding.applSurname="${applSurname}"`)
});