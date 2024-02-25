using System.Globalization;
using System.Text.RegularExpressions;

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
        var dateTime = Regex.Replace(msgBody, "[a-zA-Z]*\\s","");
        try
        {
            return DateTime.ParseExact(dateTime, "dd.MM/HH.mm", CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            return default;
        }
        
    }
}