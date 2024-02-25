using System.Data.SqlClient;
using Dapper;
using Tg.Bot.Domain;

namespace Tg.Persistence;

public class ReminderRepository(string connectionString) : IReminderRepository
{
    private SqlConnection Connection() => new(connectionString);

    public async Task<ICollection<Reminder>> GetReminders(DateTime? dateTime = null)
    {
        await using var connection = Connection();
        
        var query = "select remindOn, body as text, chatId from reminders (nolock) where remindOn BETWEEN @fromDate AND @toDate";
        var from = DateTime.Now;
        var to = DateTime.Now.AddMinutes(15);
        var reminders = connection.Query<Reminder>(query, new { fromDate = from, toDate = to });
   
        return reminders.ToList();
    }

    public async Task<ICollection<Reminder>> GetRemindersByIdAsync(long chatId)
    {
        await using var connection = Connection();

        var reminders = await connection.QueryAsync<Reminder>($"select remindOn, body as text from reminders (nolock) where chatId = {chatId}");

        return reminders.ToList();
    }

    public async Task<Reminder> GetReminderByIdAsync(Guid id)
    {
        await using var connection = Connection();
        
        var reminder = await connection.QuerySingleAsync<Reminder>($"select * from reminders (nolock) where id = {id}");

        return reminder;
    }
    
    public async Task<Reminder> SaveReminderAsync(Reminder reminder)
    {
        await using var connection = Connection(); 
        await connection.ExecuteAsync(
            $"insert into reminders (chatId, body, remindOn) values (@chatId, @text, @remindOn)", reminder);
        return reminder; // todo ???
    }

    public async Task DeleteReminderAsync(Guid id)
    {
        await using var connection = Connection();

        await connection.ExecuteAsync($"delete from reminders where Id = {id}");
    }
}