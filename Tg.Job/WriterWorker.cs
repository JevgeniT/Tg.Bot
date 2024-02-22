using System.Threading.Channels;
using MediatR;
using Tg.Bot.Core;

namespace Tg.Job;

public class EventDeletedHandler(ILogger<EventDeletedHandler> _logger)
    : IRequestHandler<ReminderEvent>
{
    public async Task Handle(ReminderEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling Contributed Deleted event for {EventId}", domainEvent);

        // TODO: do meaningful work here
        await Task.Delay(1);
    }
}


public class WriterWorker : BackgroundService
{
    private readonly ILogger<WriterWorker> _logger;
    private readonly ChannelWriter<string> _channelWriter;
    
    public WriterWorker(ILogger<WriterWorker> logger, ChannelWriter<string> channelWriter)
    {
        _logger = logger;
        _channelWriter = channelWriter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            _logger.LogInformation("Sending to channel");
            await _channelWriter.WriteAsync("msg");
            await Task.Delay(5000, stoppingToken);
        }
    }
}



public class ReaderWorker : BackgroundService
{
    private readonly ILogger<ReaderWorker> _logger;
    private readonly ChannelReader<string> _channelWriter;
    
    public ReaderWorker(ILogger<ReaderWorker> logger, ChannelReader<string> channelWriter)
    {
        _logger = logger;
        _channelWriter = channelWriter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            _logger.LogInformation("Sending to channel");
            var m = await _channelWriter.ReadAsync();
            _logger.LogInformation(m);
            await Task.Delay(5000, stoppingToken);
        }
    }
}