using TaskManagerApi.Proxies;
using TaskManagerApi.Services.Processes;
using TaskManagerApi.Services.Notifications;
using TaskManagerApi.SettingsModels;
using Microsoft.Extensions.Options;

namespace TaskManagerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<PerformanceCounterSettings>(builder.Configuration.GetSection("PerformanceCounters"));
            builder.Services.Configure<NotificationServiceSettings>(builder.Configuration.GetSection("NotificationSettings"));
            builder.Services.Configure<ProcessServiceSettings>(builder.Configuration.GetSection("ProcessesSettings"));

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<ISingletonNotificationWebSocketService, SingletonNotificationWebSocketService>();

            builder.Services.AddSingleton<IProcessesProxy, ProcessesProxy>();
            builder.Services.AddSingleton<ISingletonProcessesWebSocketService, SingletonProcessesWebSocketService>();
            builder.Services.AddSingleton<ISingletonProcessesStorage, SingletonProcessesStorage>();

            builder.Services.AddCors(
                options => options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyHeader().AllowAnyMethod();
                    }));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseAuthorization();

            app.UseWebSockets();

            app.MapControllers();

            app.Run();
        }
    }
}
