using System.Reflection;
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
using Tg.Worker;

var builder = Host.CreateApplicationBuilder(args);

var token = builder.Configuration.GetValue<string>("Token");


builder.Services.AddHostedService<BotRunnerTask>();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(x=>
    ActivatorUtilities.CreateInstance<TelegramBotClient>(x, token));
builder.Services.AddSingleton<IBotController, BotController>();
builder.Services.AddTransient<IReminderRepository, ReminderReminderRepository>();


// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();
GlobalConfiguration.Configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.Load("Tg.Bot.Core")));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient<IValidator<CreateReminderCommand>, CreateReminderValidator>();
builder.Services.AddSingleton<IBackgroundJobClient, BackgroundJobClient>(); 

RecurringJob.AddOrUpdate<ReminderJob>(x => x.SendMsg(), Cron.Minutely);//"*/5 * * * *"

var host = builder.Build();

host.Run();
