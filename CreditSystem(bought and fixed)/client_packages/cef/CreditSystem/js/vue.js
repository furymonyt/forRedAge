var creditMenu = new Vue({
    el: "#boom",
    data: {
    active: false,
    playerMoney:  "100.435",
    seen_content1: false,
    seen_content2: false,
    seen_content3: false,
    debt: "100.000",
    playerName: "Papa Mozhet",
    give: 2,
    pay: 3,
    creditSum: ""
    },
methods:{
        open: function (json) {
            this.reset();
            this.give = json[2];
            this.pay = json[3]
        },
        seenContent1: function(){
            this.seen_content1 = true
            this.seen_content2 = false
            this.seen_content3 = false 
        },
        seenContent2: function(){
            this.seen_content2 = true
            this.seen_content1 = false
            this.seen_content3 = false
        },
        seenContent3: function(){
            this.seen_content3 = true
            this.seen_content2 = false
            this.seen_content1 = false
        },
        exit: function(){
            (mp.trigger("closeCreditMenu"));
            this.reset();
        },
        giveCredit: function () {
            mp.trigger('creditOperation', this.creditSum, this.give);
        },
        payCredit: function () {
            mp.trigger('creditOperation', this.creditSum, this.pay);
        },
    }
})


