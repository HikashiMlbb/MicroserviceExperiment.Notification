using Application.Ports.In;
using Application.Ports.Out;
using Application.Services;
using Infrastructure.Adapters.In;
using Infrastructure.Adapters.Out;
using Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

#region Application Layer

builder.Services.AddScoped<IncomingMessageHandler>();

#endregion

#region Application Settings

// RabbitMQ Settings
var rabbitMqSettings = new RabbitMQSettings();
var rabbitMqSection = builder.Configuration.GetSection("RABBITMQ");
rabbitMqSettings.ConnectionString = rabbitMqSection.GetValue<string>("CONNECTION_STRING") ?? throw new ArgumentNullException($"{nameof(RabbitMQSettings)}.{nameof(RabbitMQSettings.ConnectionString)}");
rabbitMqSettings.QueueName = rabbitMqSection.GetValue<string>("QUEUE_NAME") ?? throw new ArgumentNullException($"{nameof(RabbitMQSettings)}.{nameof(RabbitMQSettings.QueueName)}");
builder.Services.AddSingleton<RabbitMQSettings>(rabbitMqSettings);

// SMTP Settings
var mailSettings = new MailSettings();
var mailSection = builder.Configuration.GetSection("SMTP");
mailSettings.EmailName = mailSection.GetValue<string>("EMAIL_NAME") ?? throw new ArgumentNullException($"{nameof(MailSettings)}.{nameof(MailSettings.EmailName)}");
mailSettings.EmailAddress = mailSection.GetValue<string>("EMAIL_ADDRESS") ?? throw new ArgumentNullException($"{nameof(MailSettings)}.{nameof(MailSettings.EmailAddress)}");
mailSettings.SmtpHost = mailSection.GetValue<string>("HOST") ?? throw new ArgumentNullException($"{nameof(MailSettings)}.{nameof(MailSettings.SmtpHost)}");
mailSettings.SmtpPort = mailSection.GetValue<int>("PORT");
mailSettings.UseSsl = mailSection.GetValue<bool>("USE_SSL");
mailSettings.SmtpLogin = mailSection.GetValue<string>("LOGIN") ?? throw new ArgumentNullException($"{nameof(MailSettings)}.{nameof(MailSettings.SmtpLogin)}");
mailSettings.SmtpPassword = mailSection.GetValue<string>("PASSWORD") ?? throw new ArgumentNullException($"{nameof(MailSettings)}.{nameof(MailSettings.SmtpPassword)}");
builder.Services.AddSingleton<MailSettings>(mailSettings);

#endregion

#region Infrastructure Adapters

// In-Adapters
builder.Services.AddSingleton<IMessageListener, RabbitMessageListener>();

// Out-Adapters
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
builder.Services.AddScoped<ISmtpService, SmtpService>();

#endregion

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var messageListener = scope.ServiceProvider.GetRequiredService<IMessageListener>();
    var handler = scope.ServiceProvider.GetRequiredService<IncomingMessageHandler>();
    messageListener.OnMessageReceived += handler.Handle;
    await messageListener.StartListeningAsync(CancellationToken.None);    
}

app.Run();