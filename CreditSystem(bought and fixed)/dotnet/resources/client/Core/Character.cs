// Ищем Money = Convert.ToInt64(Row["money"]);
//Сразу после него вставляем:

CreditMoney = Convert.ToInt64(Row["creditmoney"]);
//Там же ищем Unwarn = ((DateTime)Row["unwarn"]);
//После него вставляем:
AllowCredit = Convert.ToInt32(Row["allowcredit"]);
CreditTime = ((DateTime)Row["credittime"]);

//Ищем await MySQL.QueryAsync($"UPDATE `characters` SET `pos`='{pos}',`gender`={Gender},

//После `money`={ Money}, вставляем:
`creditmoney`= {CreditMoney},

//Там же ищем `licenses`='{JsonConvert.SerializeObject(Licenses)}',
//Около него вставляем:
`credittime`= '{MySQL.ConvertTime(CreditTime)}',`allowcredit`={AllowCredit},

//Идём дальше.
//Ищем  await MySQL.QueryAsync($"INSERT INTO `characters`(`uuid`,
//После `money`, вставляем:
`creditmoney`,

//Там же ищем `unwarn`,
//После него вставляем:
`credittime`,`allowcredit`,

//В этом же месте ищем {Money},
//Вставляем: 
{CreditMoney},

//Ищем '{JsonConvert.SerializeObject(Licenses)}',
//Вставляем:
'{MySQL.ConvertTime(CreditTime)}','{AllowCredit}',