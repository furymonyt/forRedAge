// Ищем тут {218, "Щука" },   И после вставляем

{8000, "Телефон" },

// Ищем тут { ItemType.ArmDrink3, NAPI.Util.GetHashKey("prop_bottle_cognac") }, и ниже вставляем

{ ItemType.Phone, NAPI.Util.GetHashKey("p_amb_phone_01") },

// Ищем тут { ItemType.ArmDrink3, new Vector3(0, 0, -1) }, и ниже вставляем

{ ItemType.Phone, new Vector3(0, 0, -0.8) },

// Ищем { ItemType.ArmDrink3, new Vector3() }, и ниже вставляем

{ ItemType.Phone, new Vector3() },

// Ищем { ItemType.KeyRing, 1 },  и ниже вставляем

{ ItemType.Phone, 1 },

// Ищем  public static void onUse(Player player, nItem item, int index) 
// И в нем находим 
if (nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type))
	
// И перед этим вставляем

if (item.Type == ItemType.Phone)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Что бы использовать нажмите М.", 3000);
                    Dashboard.Close(player);
                    return;
                }

// Ищем тут case ItemType.GasCan: и после break; вставляем


case ItemType.Phone:
                        break;