namespace Tg.Bot.Core.Extensions;

public static class MessageParser
{
    public static string ParseMessageBody(string msgBody)
    {
        var arr = msgBody.Split(" ");
        return arr.Length != 3 ? string.Empty : arr[1]; //todo parse sentence
    }
        
    public static DateTime ParseDate(string msgBody)
    {
        var arr = msgBody.Split(" ");
        if (arr.Length != 3) return default;
            
        var dateStr = arr[2];
        var date = dateStr.Split('.');
            
        var (day, month) = (date[0], date[1]); // todo  hh:mm ?
        return new DateTime(2024, int.Parse(month), int.Parse(day));
    }
}