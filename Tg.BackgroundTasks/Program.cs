using System.Reflection;
using System.Threading.Channels;
using FluentValidation;
using Hangfire;
using MediatR;
using Telegram.Bot;
using Tg.BackgroundTasks;
using Tg.Bot;
using Tg.Bot.Core.Reminder.CreateReminder;
using Tg.Bot.Domain;
using Tg.Bot.Domain.Services;
using Tg.Persistence;

var builder = Host.CreateApplicationBuilder(args);

var apiToken = builder.Configuration.GetValue<string>("Token") 
            ?? throw new ArgumentNullException($"Bot api token required");

var dbConnectionString = builder.Configuration.GetConnectionString("SqlConnectionString") 
                         ?? throw new ArgumentNullException($"Db connection string required");

var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnectionString") 
                               ?? throw new ArgumentNullException($"Hangfire db connection string required");

builder.Services.AddHostedService<BotRunnerTask>();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(x=>
    ActivatorUtilities.CreateInstance<TelegramBotClient>(x, apiToken));
builder.Services.AddTransient<IReminderRepository, ReminderRepository>(x=>
    ActivatorUtilities.CreateInstance<ReminderRepository>(x, dbConnectionString));
builder.Services.AddSingleton<IBotController, BotController>();


// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(hangfireConnectionString));
builder.Services.AddHangfireServer();
GlobalConfiguration.Configuration.UseSqlServerStorage(hangfireConnectionString);


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.Load("Tg.Bot.Core")));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient<IValidator<CreateReminderCommand>, CreateReminderValidator>();
builder.Services.AddSingleton<IBackgroundJobClient, BackgroundJobClient>(); 
builder.Services.AddSingleton(Channel.CreateBounded<Reminder>(new BoundedChannelOptions(10)
{
    AllowSynchronousContinuations = false,
    SingleReader = false,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.Wait
}));

RecurringJob.AddOrUpdate<ReminderJob>(x => x.ReminderNotifierJob(), Cron.Minutely);//"*/15 9-23 * * *

var host = builder.Build();

host.Run();
