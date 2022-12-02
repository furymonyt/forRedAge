//Там же ищем Unwarn = ((DateTime)Row["unwarn"]);
//После него вставляем:
MarriageName = Convert.ToString(Row["mName"]);
MarriageSurname = Convert.ToString(Row["mSurname"]);
WeddingApplication = Convert.ToInt32(Row["weddingappl"]);
ApplName = Convert.ToString(Row["applName"]);
ApplSurname = Convert.ToString(Row["applSurname"]);

//Ищем await MySQL.QueryAsync($"UPDATE `characters` SET `pos`='{pos}',`gender`={Gender},

//Там же ищем `licenses`='{JsonConvert.SerializeObject(Licenses)}',
//Около него вставляем:
`mName`= '{MarriageName}',`mSurname`= '{MarriageSurname}',`weddingappl`= '{WeddingApplication}',`applSurname`= '{ApplSurname}',`applName`= '{ApplName}',

//Идём дальше.
//Ищем  await MySQL.QueryAsync($"INSERT INTO `characters`(`uuid`,

//Там же ищем `unwarn`,
//После него вставляем:
`mName`,`mSurname`,`weddingappl`,`applName`,`applSurname`,

//Ищем '{JsonConvert.SerializeObject(Licenses)}',
//Вставляем:
'{MarriageName}','{MarriageSurname}','{WeddingApplication}','{ApplName}','{ApplSurname}',