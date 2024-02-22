namespace Tg.Bot.Domain;

public class Reminder
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public long ChatId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime RemindOn { get; set; }
}