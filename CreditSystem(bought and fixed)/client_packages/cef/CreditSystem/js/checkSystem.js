var checkMenu = new Vue({
    el: "#boom",
    data: {
    active: false,
    seen_content1: false,
    playerName: "Papa Mozhet",
    type: 0
    },
methods:{
        seencontent1: function(){
            this.seen_content1 = true
        },
        exit: function(){
            mp.trigger("closeCheckMenu");
            this.reset();
        },
        property: function(){
            mp.trigger('propertyCheck', this.type);
            this.reset();
        },
        reset: function(){
            seen_content1 = false;
            active = false;
        },
    }
});  