using Common.API;
using Logger;
using MessageQueue;
using MessageQueue.Settings;
using NewsService.API.Services;
using NewsService.API.Settings;
using NewsService.BLL;
using NewsService.DAL.PostgreSql;
using Serilog;

namespace NewsService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        GlobalSettings globalSettings = builder.Configuration.Get<GlobalSettings>();
        
        builder.ConfigureCommonApiSettings();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddOpenApi(); // Move to Swagger project
        builder.Services.AddProblemDetails();

        builder.Services.AddBusinessLayer();
        builder.Services.AddDataAccessLayer(globalSettings.ConnectionStrings.PostgreSql);
        
        var messageQueueSettings = new MessageQueueSettings
        {
            ServerAddress = new Uri("amqp://localhost"),
            QueueName = "news-queue",
        };
        
        builder.Services.AddNewsServiceConsumers(messageQueueSettings);

        var webhooksQueueSettings = new MessageQueueSettings
        {
            ServerAddress = new Uri("amqp://localhost"),
            QueueName = "webhooks-queue",
        };
        builder.Services.AddWebhookDispatcher(webhooksQueueSettings);

        builder.Services.AddHostedService<NewsProcessor>();
        
        var app = builder.Build();
        
        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "My API V1"); }); // Move to Swagger project
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