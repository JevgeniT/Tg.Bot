using Tg.Bot.Domain.Services;

namespace Tg.BackgroundTasks;

public class BotRunnerTask(ILogger<BotRunnerTask> logger, IBotController bot) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Bot running at: {time}", DateTimeOffset.Now);
        
        await Task.Run(() => bot.StartReceiving(stoppingToken), stoppingToken);
    }
}