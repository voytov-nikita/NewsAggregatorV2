using NewsService.BLL;
using NewsService.DAL;

namespace NewsService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddProblemDetails();
        
        builder.Services.AddCustomControllers();
        builder.Services.AddBusinessLayer();
        builder.Services.AddDataAccessLayer();

        var app = builder.Build();

        app.UseExceptionHandler();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
            });
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