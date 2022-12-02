var creditMenu = new Vue({
    el: "#boom",
    data: {
    active: false,
    playerMoney:  "100.435",
    playerName: "Papa Mozhet",
    debt: "0",
    seen_content1: false,
    seen_content2: false,
    seen_content3: false,
    give: 2,
    pay: 3,
    creditSum: ""
    },
methods:{
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
            mp.trigger("closeCreditMenu");
            this.reset();
        },
        giveCredit: function () {
            mp.trigger('creditOperation', this.creditSum, this.give);
            this.reset();
        },
        payCredit: function () {
            mp.trigger('creditOperation', this.creditSum, this.pay);
            this.reset();
        },
        reset(){
            this.seen_content1 = false
            this.seen_content2 = false
            this.seen_content2 = false
            this.seen_content1 = false
        },
    }
})
if(active = true){
}