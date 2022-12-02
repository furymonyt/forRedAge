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




var Loader2 = new Vue({
    el: ".Loader2",
    data: {
        active: false,
        header: "Грузчик2",
        money: "1",
        jobid: 10,
    },
    methods: {
        set: function (money, level, currentjob) {
            this.money = money;
            this.level = level;
            this.jobid = currentjob;
        },
        setnewjob: function (jobsid) {
            this.jobid = jobsid;
        },
        exit: function () {
            this.active = false;
            mp.trigger('CloseLoader');
        },
        selectJob: function (jobid) {
            mp.trigger("selectJobLoader2", jobid);
        }
    }
});
