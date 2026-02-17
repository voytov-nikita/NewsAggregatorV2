using Common.API;
using MessageQueue;
using MessageQueue.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.BLL;
using NotificationService.DAL;
using NotificationService.Services;

namespace NotificationService;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

       // GlobalSettings globalSettings = builder.Configuration.Get<GlobalSettings>();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.ConfigureCommonApiSettings();
        
        builder.Services.AddOpenApi();
        builder.Services.AddProblemDetails();
        
        builder.Services.AddCustomControllers();
        builder.Services.AddBusinessLayer();
        
        builder.Services.AddHttpClient();

        builder.Services.AddTransient<IWebhookDispatcher, WebhookDispatcher>();
        
        //Todo: move to appsettings
        DatabaseSettings databaseSettings = new DatabaseSettings()
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "notificationService",
        }; 
        builder.Services.AddDataAccessLayer(databaseSettings);

        //Todo: move to appsettings
        var webhooksQueueSettings = new MessageQueueSettings
        {
            ServerAddress = new Uri("amqp://localhost"),
            QueueName = "webhooks-queue",
        };
        builder.Services.AddWebhooksConsumer(webhooksQueueSettings);
        builder.Services.AddHostedService<WebhooksProcessor>();

        var app = builder.Build();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "My API V1"); });
        }

        app
            .UseHttpsRedirection()
            .UseCors("AllowAll")
            .UseRouting()
            .UseResponseCompression();

        app.MapControllers();


        app.Run();
    }
}