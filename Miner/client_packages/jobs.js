mp.peds.new(0x49EA5685, new mp.Vector3(2946.686, 2746.836, 43.40), 288.2411); // ped Miner
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



// Job Miner //
mp.events.add('OpenMiner', (money, level, currentjob, work, money2, money3) => {
    if (global.menuCheck()) return;
    jobs.execute(`Miner.set('${money}', '${level}', '${currentjob}', '${work}', '${money2}', '${money3}')`);
    jobs.execute('Miner.active=1');
    global.menuOpen();
});
mp.events.add('CloseMiner', () => {
    jobs.execute('Miner.active=0');
    global.menuClose();
});
mp.events.add("selectJobMiner", (jobid) => {
    if (new Date().getTime() - global.lastCheck < 1000) return;
    global.lastCheck = new Date().getTime();
    mp.events.callRemote("jobJoinMiner", jobid);
});
mp.events.add('secusejob', (jobsid) => {
    jobs.execute(`Miner.setnewjob('${jobsid}')`);
});
mp.events.add('enterJobMiner', (work) => {
    if (new Date().getTime() - global.lastCheck < 1000) return;
    global.lastCheck = new Date().getTime();
    mp.events.callRemote('enterJobMiner', work);
    global.menuClose();
    jobs.execute('Miner.active=0');
});