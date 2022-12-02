var JobStatsInfo = new Vue({
    el: ".JobStatsInfo",
    data: {
        active: false,
        money: "1",
    },
    methods: {
        set: function (money) {
            this.money = money;
        }
    }
});

var JobsEinfo = new Vue({
    el: ".JobsEinfo",
    data: {
        active: false,
    }
});



var Loader = new Vue({
    el: ".Loader",
    data: {
        active: false,
        header: "Грузчик",
        money: "1",
        jobid: 9,
        work: 0,
    },
    methods: {
        set: function (money, level, currentjob, work) {
            this.money = money;
            this.level = level;
            this.jobid = currentjob;
            this.work = work;
        },
        exit: function () {
            this.active = false;
            mp.trigger('CloseLoader');
        },
        setnewjob: function (jobsid) {
            this.jobid = jobsid;
        },
        enterJob: function (work) {
            mp.trigger('CloseLoader');
            mp.trigger("enterJobLoader", work);
        },
        selectJob: function (jobid) {
            mp.trigger("selectJobLoader", jobid);
        }
    }
});
