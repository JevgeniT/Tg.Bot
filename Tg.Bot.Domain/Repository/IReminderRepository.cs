namespace Tg.Bot.Domain;


public interface IReminderRepository
{
    Task<Reminder> GetReminderByIdAsync(Guid id);

    Task<IEnumerable<Reminder>> GetRemindersByIdAsync(long chatId);

    Task<Reminder> SaveReminderAsync(Reminder reminder);
    
    Task DeleteReminderAsync(Guid id);
}