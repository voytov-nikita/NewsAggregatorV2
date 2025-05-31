using Common.API;
using CrawlerService.BLL;
using CrawlerService.DAL;
using CrawlerService.Hangfire;
using MessageQueue;
using MessageQueue.Settings;

namespace CrawlerService.API;

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

        var messageQueueSettings = new MessageQueueSettings
        {
            ServerAddress = new Uri("amqp://localhost"),
            QueueName = "news-queue",
        };
        
        builder.Services.AddNewsServiceProducers(messageQueueSettings);
        
        DatabaseSettings databaseSettings = new DatabaseSettings()
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "news",
        }; 
        builder.Services.AddDataAccessLayer(databaseSettings);

        

        HangfireSettings hangfireSettings = new HangfireSettings()
        {
            Prefix = "news-hangfire",
            ConnectionString = "mongodb://localhost:27017/news",
        };
        builder.Services.AddHangfireCustomSettings(hangfireSettings);
        
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