//Ищем 
private static nLog Log = new nLog("Weddings");
//Под ним вставляем
private static readonly Random random = new Random();
//Ищем 
NAPI.Chat.SendChatMessageToAll("!{#bd1dae}" + $"{acc.FirstName}_{acc.LastName} и {acc.ApplName}_{acc.ApplSurname} вступили в брак. Поздравляем и желаем успехов в совместной жизни!"); ;
//Вместо этого кода вставляем
var rand = random.Next(0, 2);
if (rand == 0)
{
    NAPI.Chat.SendChatMessageToAll("!{#bd1dae}" + $"{acc.FirstName}_{acc.LastName} и {acc.ApplName}_{acc.ApplSurname} вступили в брак. Поздравляем и желаем успехов в совместной жизни!"); ;
}
if (rand == 1)
{
    NAPI.Chat.SendChatMessageToAll("!{#bd1dae}" + $"Радостная новость! Два любящих сердца {acc.FirstName}_{acc.LastName} и {acc.ApplName}_{acc.ApplSurname} свели свои брачные узы воедино! Поздравим новую пару штата!"); ;
}
if (rand == 2)
{
    NAPI.Chat.SendChatMessageToAll("!{#bd1dae}" + $"{acc.FirstName}_{acc.LastName} и {acc.ApplName}_{acc.ApplSurname} зарегестрировали свой брак. Поздравляем!"); ;
}