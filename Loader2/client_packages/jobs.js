mp.peds.new(0x49EA5685, new mp.Vector3(1196.747, -3253.617, 7.00), 92.806); // ped Loader2
global.jobs = mp.browsers.new('package://cef/jobs.html'); //статистика

// Job JobsEinfo //
mp.events.add('JobsEinfo', () => {
    jobs.execute('JobsEinfo.active=1');
});
mp.events.add('JobsEinfo2', () => {
    jobs.execute('JobsEinfo.active=0');
});

// Job StatsInfo //
mp.events.add('JobStatsInfo', (money) => {
    jobs.execute('JobStatsInfo.active=1');
    jobs.execute(`JobStatsInfo.set('${money}')`);
});
mp.events.add('CloseJobStatsInfo', () => {
    jobs.execute('JobStatsInfo.active=0');
});
// Улучшенные блипы
var JobMenusBlip = [];
mp.events.add('JobMenusBlip', function (uid, type, position, names, dir) {
    if (typeof JobMenusBlip[uid] != "undefined") {
        JobMenusBlip[uid].destroy();
        JobMenusBlip[uid] = undefined;
    }
    if (dir != undefined) {
        JobMenusBlip[uid] = mp.blips.new(type, position,
            {
                name: names,
                scale: 1,
                color: 4,
                alpha: 255,
                drawDistance: 100,
                shortRange: false,
                rotation: 0,
                dimension: 0
            });
    }

});
mp.events.add('deleteJobMenusBlip', function (uid) {
    if (typeof JobMenusBlip[uid] == "undefined") return;
    JobMenusBlip[uid].destroy();
    JobMenusBlip[uid] = undefined;
});




// Job Loader2 //
mp.events.add('OpenLoader2', (money, level, currentjob) => {
    if (global.menuCheck()) return;
    jobs.execute(`Loader2.set('${money}', '${level}', '${currentjob}')`);
    jobs.execute('Loader2.active=1');
    global.menuOpen();
});
mp.events.add('CloseLoader', () => {
    jobs.execute('Loader2.active=0');
    global.menuClose();
});
mp.events.add("selectJobLoader2", (jobid) => {
    if (new Date().getTime() - global.lastCheck < 1000) return;
    global.lastCheck = new Date().getTime();
    mp.events.callRemote("jobJoinLoader2", jobid);
});
mp.events.add('secusejobLoader2', (jobsid) => {
    jobs.execute(`Loader2.setnewjob('${jobsid}')`);
});
