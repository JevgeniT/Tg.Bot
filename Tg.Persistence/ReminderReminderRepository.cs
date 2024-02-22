using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Tg.Bot.Domain;

namespace Tg.Persistence;

public class ReminderReminderRepository(IConfiguration configuration) : IReminderRepository
{
    private SqlConnection Connection() => new(configuration.GetConnectionString("SqlConnectionString"));

    public async Task<IEnumerable<Reminder>> GetRemindersByIdAsync(long chatId)
    {
        await using var connection = Connection();

        var reminders = await connection.QueryAsync<Reminder>($"select remindOn, body as text from reminders (nolock) where chatId = {chatId}");

        return reminders;
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