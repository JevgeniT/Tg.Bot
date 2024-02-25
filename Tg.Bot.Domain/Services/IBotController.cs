namespace Tg.Bot.Domain.Services;

public interface IBotController
{
    void StartReceiving(CancellationToken cts);
    Task RespondAsync(long chatId, string msg, CancellationToken ct);
}
