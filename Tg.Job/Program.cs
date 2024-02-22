using System.Threading.Channels;
using Tg.Job;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ReaderWorker>();
builder.Services.AddSingleton(Channel.CreateUnbounded<string>(new UnboundedChannelOptions()));
builder.Services.AddSingleton<ChannelReader<string>>(x => x.GetRequiredService<Channel<string>>().Reader);

var host = builder.Build();
host.Run();