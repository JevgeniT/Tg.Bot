namespace Tg.Bot.Contracts.Reminder;


public class CreateReminderResponse
{
    public string Text { get; set; }

    public DateTime RemindOn { get; set; }
}