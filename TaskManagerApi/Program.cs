using Microsoft.AspNetCore.Hosting;
using TaskManagerApi.Services;
using TaskManagerApi.Services.Notifications;
using TaskManagerApi.SettingsModels;

namespace TaskManagerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<PerformanceCounterSettings>(builder.Configuration.GetSection("PerformanceCounters"));
            builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection("NotificationSettings"));
            builder.Services.Configure<ProcessesSettings>(builder.Configuration.GetSection("ProcessesSettings"));

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<INotificationWebSocketService, NotificationWebSocketService>();
            builder.Services.AddScoped<IProcessesWebSocketService, ProcessesWebSocketService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.UseWebSockets();

            app.MapControllers();

            app.Run();
        }
    }
}
