using GTANetworkAPI;
using Newtonsoft.Json;

        public static List<HouseType> HouseTypeList = new List<HouseType>
        {
            new HouseType("Trailer", new Vector3(1973.124, 3816.065, 32.30873), new Vector3(), 0.0f, "trevorstrailer"),
            new HouseType("Slums", new Vector3(151.2052, -1008.007, -100.12), new Vector3(), 0.0f,"hei_hw1_blimp_interior_v_motel_mp_milo_"),
            new HouseType("Dwelling House", new Vector3(265.9691, -1007.078, -102.0758), new Vector3(), 0.0f,"hei_hw1_blimp_interior_v_studio_lo_milo_"),
            new HouseType("Flat", new Vector3(346.6991, -1013.023, -100.3162), new Vector3(349.5223, -994.5601, -99.7562), 264.0f, "hei_hw1_blimp_interior_v_apart_midspaz_milo_"),
            new HouseType("Pillbox Hills, Apt 7", new Vector3(-31.35483, -594.9686, 78.9109),  new Vector3(-25.42115, -581.4933, 79.12776), 159.84f, "hei_hw1_blimp_interior_32_dlc_apart_high2_new_milo_"),
            new HouseType("Pillbox Hills, Apt 12", new Vector3(-17.85757, -589.0983, 88.99482), new Vector3(-38.84652, -578.466, 88.58952), 50.8f, "hei_hw1_blimp_interior_10_dlc_apart_high_new_milo_"),
            new HouseType("Dell Perro Heights, Apt 7", new Vector3(-1451.6641, -523.7244, 56.30898), new Vector3(-1449.982, -525.79346, 56.30898), 50.8f, "Dell Perro Heights, Apt 7"), // Dell Perro Heights, Apt 7
            new HouseType("Dell Perro Heights, Apt 4", new Vector3(-1452.4906, -540.3255, 73.44433), new Vector3(-1452.4906, -540.3255, 73.44433), 40.0f, "Dell Perro Heights, Apt 4"), // Dell Perro Heights, Apt 4
            new HouseType("Richard Majestic, Apt 2", new Vector3(-912.84326, -365.1879, 113.654724), new Vector3(-912.84326, -365.1879, 113.654724), 40.0f, "Richard Majestic, Apt 2"), // Richard Majestic, Apt 2
            new HouseType("Tinsel Towers, Apt 42", new Vector3(-603.32, 58.962257, 97.68024), new Vector3(-603.32, 58.962257, 97.68024), 40.0f, "Tinsel Towers, Apt 42"), //  Tinsel Towers, Apt 42
            new HouseType("Eclipse Towers, Apt 3", new Vector3(-784.96423, 323.73303, 211.37701), new Vector3(-784.96423, 323.73303, 211.37701), 40.0f, "Eclipse Towers, Apt 3"), // Eclipse Towers, Apt 3
            new HouseType("3655 Wild Oats Drive", new Vector3(-174.36844, 497.4991, 137.14688), new Vector3(-174.36844, 497.4991, 137.14688), 40.0f, "3655 Wild Oats Drive"), // 3655 Wild Oats Drive // 1 дом
            new HouseType("2044 North Conker Avenue", new Vector3(341.9316, 437.71017, 148.474), new Vector3(341.9316, 437.71017, 148.474), 40.0f, "2044 North Conker Avenue"), //  2044 North Conker Avenue
            new HouseType("2045 North Conker Avenue", new Vector3(373.4818, 423.5782, 145.38781), new Vector3(373.4818, 423.5782, 145.38781), 40.0f, "2045 North Conker Avenue"), // 2045 North Conker Avenue	
            new HouseType("2862 Hillcrest Avenue", new Vector3(-682.3555, 592.55286, 144.7635), new Vector3(-682.3555, 592.55286, 144.7635), 40.0f, "2862 Hillcrest Avenue"), // 2862 Hillcrest Avenue
            new HouseType("2868 Hillcrest Avenue", new Vector3(-758.5874, 619.2068, 143.53381), new Vector3(-758.5874, 619.2068, 143.53381), 40.0f, "2868 Hillcrest Avenue"), //  2868 Hillcrest Avenue
            new HouseType("2874 Hillcrest Avenue", new Vector3(-859.8462, 691.2875, 152.24066), new Vector3(-859.8462, 691.2875, 152.24066), 40.0f, "2874 Hillcrest Avenue"), // 2874 Hillcrest Avenue	
            new HouseType("2677 Whispymound Drive", new Vector3(117.345406, 560.10535, 183.7048), new Vector3(117.345406, 560.10535, 183.7048), 40.0f, "2677 Whispymound Drive"), // 2677 Whispymound Drive
            new HouseType("2133 Mad Wayne Thunder", new Vector3(-1289.9935, 449.86682, 97.30248), new Vector3(-1289.9935, 449.86682, 97.30248), 40.0f, "2133 Mad Wayne Thunder"), //  2133 Mad Wayne Thunder
            new HouseType("Eclipse Towers, Apt 4", new Vector3(-786.9563, 315.6229, 187.9136), new Vector3(-786.9563, 315.6229, 187.9136), 40.0f, "apa_v_mp_h_01_c"), //  Eclipse Towers, Apt 4
            new HouseType("Eclipse Towers, Apt 5", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "apa_v_mp_h_01_b"), //  Eclipse Towers, Apt 5
           /* Здесь кастомные интерьеры на будущее 
           
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_11_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_12_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_13_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_14_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_15_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_16_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_17_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_18_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_19_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_20_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_21_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_22_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_23_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_24_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_25_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_26_dlc_apart_high_new_milo_"), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_27_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_28_dlc_apart_high2_new_milo_ "), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_29_dlc_apart_high2_new_milo_ "), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_30_dlc_apart_high2_new_milo_ "), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_31_dlc_apart_high2_new_milo_ "), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_33_dlc_apart_high2_new_milo_ "), //  test
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_8_dlc_apart_high_new_milo_"), //  Eclipse Towers, Apt 5
            new HouseType("Penthouse1", new Vector3(-774.0126, 342.0428, 196.6864), new Vector3(-774.0126, 342.0428, 196.6864), 40.0f, "hei_hw1_blimp_interior_9_dlc_apart_high_new_milo_"), //  test
            new HouseType("Middle-Class House", new Vector3(-1647.6609, -528.7176, 65.98673), new Vector3(-1647.6609, -528.7176, 65.98673), 40.0f, ""), //  Middle-Class House
            new HouseType("High-Class House", new Vector3(72.40017, -824.6635, 69.37323), new Vector3(72.40017, -824.6635, 69.37323), 40.0f, ""), //  High-Class House
        */
            };
        private static List<int> MaxRoommates = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };

        
