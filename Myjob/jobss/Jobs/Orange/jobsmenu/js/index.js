var JobMenuOrange = new Vue({
    el: ".job",
    data: {
	active: true,
	style: 0,
	workid: 10,
	workstate2: false,
	workstate3: false,
	jobpayment: 15,
	jobpayment2: 30,
	lvl: 1,
    },
    methods:{
        gostyle: function(index) {
            this.style = index;
        },
		btnchangeworkstate: function(act) {
			mp.trigger("ChangeWorkStateOrange", act);
		},
		btnselectjob: function(act) {
			mp.trigger("client::startOrangeWork", act);
		},
    }
});