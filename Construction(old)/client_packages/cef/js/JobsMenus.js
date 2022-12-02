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


var Construction = new Vue({
    el: ".Construction",
    data: {
        active: false,
        header: "Строитель",
        money: "1",
        jobid: 12,
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
            mp.trigger('CloseConstruction');
        },
        setnewjob: function (jobsid) {
            this.jobid = jobsid;
        },
        enterJob: function (work) {
            mp.trigger('CloseConstruction');
            mp.trigger("enterJobConstruction", work);
        },
        selectJob: function (jobid) {
            mp.trigger("selectJobConstruction", jobid);
        }
    }
});