namespace Tg.Bot.Domain;


public interface IReminderRepository
{
    Task<Reminder> GetReminderByIdAsync(Guid id);
    
    Task<ICollection<Reminder>> GetReminders(DateTime? dateTime);

    Task<ICollection<Reminder>> GetRemindersByIdAsync(long chatId);

    Task<Reminder> SaveReminderAsync(Reminder reminder);
    
    Task DeleteReminderAsync(Guid id);
}