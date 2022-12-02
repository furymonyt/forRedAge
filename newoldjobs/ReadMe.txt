Копируем содержимое папки client_packages в вашу папку с таким же именем. Заменяем файлы при необходимости.
В файле client_packages/index.js находим список где много раз пишется require('');
под ними, в конце добавляем одну новую:
JavaScript:
require('./Jobs.js');

В файле dotnet\resources/NeptuneEvo/Main.cs находим строчку
[RemoteEvent("interactionPressed")]
в ней находим switch и добавляем в конец:
C#:
#region CustomJobs
case 510:
    Jobs.Diver.StartWorkDayDiver(player);
    return;
case 508:
    Jobs.Miner.StartWorkDayMiner(player);
    return;
case 509:
    Jobs.Construction.StartWorkDayConstruction(player);
    return;
case 5060:
    Jobs.Loader.StartWorkDayLoader(player);
    return;
#endregion

В том же файле находим строчку
Jobs.AutoMechanic.onPlayerDissconnectedHandler(player, type, reason);
и после нее добавляем несколько новых:
C#:
// new jobs
Jobs.Miner.Event_PlayerDisconnected(player, type, reason);
Jobs.Diver.Event_PlayerDisconnected(player, type, reason);
Jobs.Loader.Event_PlayerDisconnected(player, type, reason);
Jobs.Construction.Event_PlayerDisconnected(player, type, reason);

В файле dotnet\resources\NeptuneEvo\Fractions\Ems.cs находим строчку
Jobs.Gopostal.Event_PlayerDeath(player, entityKiller, weapon);
и после нее добавляем несколько новых:
C#:
// new jobs
Jobs.Construction.Event_PlayerDeath(player, entityKiller, weapon);
Jobs.Loader.Event_PlayerDeath(player, entityKiller, weapon);
Jobs.Diver.Event_PlayerDeath(player, entityKiller, weapon);
Jobs.Miner.Event_PlayerDeath(player, entityKiller, weapon);

В файле dotnet\resources\client\Jobs\WorkManager.cs находим строчку
public static List<string> JobStats = new List<string>
и добавляем туда названия новых работ:
C#:
public static List<string> JobStats = new List<string>
{
    "Электрик",
    "Почтальон",
    "Таксист",
    "Водитель автобуса",
    "Газонокосильщик",
    "Дальнобойщик",
    "Инкассатор",
    "Автомеханик",
    "Грузчик", // Добавили это
    "Пусто", // и это
    "Каменщик", // и это
    "Строитель", // и это
    "Дайвер", // и это
};