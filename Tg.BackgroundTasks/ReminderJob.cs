using System.Threading.Channels;
using Tg.Bot.Domain;

namespace Tg.BackgroundTasks;

public class ReminderJob(IReminderRepository repository, Channel<Reminder> channel)
{
    public async Task ReminderNotifierJob()
    {
        var reminders = (await repository.GetReminders(DateTime.Now)).ToList();
        if(reminders.Count == 0) return;
        
        //todo delete inactive, fail case
        reminders.ForEach(x=> channel.Writer.TryWrite(new Reminder()
        {
            ChatId = x.ChatId,
            Text = x.Text
        }));
    }
}