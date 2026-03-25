using Calculadora.Components;
using CalculadoraDebitos.Data;
using CalculadoraDebitos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Calculadora
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://0.0.0.0:5275","https://0.0.0.0:7260");
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080); // só HTTP
            });
            // Add services to the container.
            builder.Services.AddRazorComponents().AddInteractiveServerComponents();
            builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite("Data Source=database.db"));

            builder.Services.AddQuickGridEntityFrameworkAdapter();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddScoped<CalculadoraService>();
            builder.Services.AddScoped<SelicService>();
            builder.Services.AddScoped<UffiService>();


            builder.Services.AddFluentUIComponents();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
    app.UseMigrationsEndPoint();
            }

       //     app.UseHttpsRedirection();
            app.UseAntiforgery();
            app.MapStaticAssets();
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
