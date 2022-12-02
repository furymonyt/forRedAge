var wedding = new Vue({
    el: '#Wedding',
    data: {
      active: false,
      index: 0,
      name: "",
      surname: "",
      WeddingInputMenu: false,
      WeddingInputMenu2: false,
      application: "",
      applicationMenu: false,
      applName: "Obeme",
      applSurname: "Bolshoi"
    },
    methods:{
      WeddingInputBTN: function(){
        this.WeddingInputMenu = true;
      },

      WeddingInputMenuClose: function(){
        this.WeddingInputMenu = false;
        this.WeddingInputMenu2 = false;
      },

      WeddingName: function(){
      mp.trigger('WeddingName', this.index = 1, this.name);
      this.WeddingInputMenu = false;
      this.WeddingInputMenu2 = true;
      this.name = "";
      this.reset();
      },

      WeddingSurname: function(){
      mp.trigger('WeddingSurname', this.index = 2,  this.surname);
      this.surname = "";
      this.WeddingInputMenu2 = false;
      this.reset();
      },

      divorce: function(){
        mp.trigger('WeddingDivorce', this.index = 3);
        this.reset();
      },
      
      openAppMenu: function(){
        this.applicationMenu = true
      },

      Yes: function(){
        mp.trigger('Yes', this.index = 10);
        this.applicationMenu = false;
      },

      No: function(){
        mp.trigger('No', this.index = 11);
        this.applicationMenu = false;
      },

      closeMenu: function(){
        mp.trigger("closeWeddingsMenu");
        this.WeddingInputMenu = false
        this.WeddingInputMenu2 = false;
        this.reset();
      }
    }
})