//Ищем 

mp.events.add("executePersonInfo",

//Если вы ничего не меняли для себя, просто вставляем это

mp.events.add("executePersonInfo", (name, lastname, uuid, gender, wedding,wantedlvl, lic) => {
    global.menu.execute(`pc.openPerson("${name}","${lastname}","${uuid}","${gender}","${wedding}","${wantedlvl}","${lic}")`);
});

//А если меняли, в этом методе после gender, вставляем
wedding,

//Так же после "${gender}", вставляем

"${wedding}",