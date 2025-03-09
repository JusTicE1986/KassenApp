using KassenApp.Components;
using KassenApp.Data;
using KassenApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KassenApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Datenbankverbindung setzen:
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<KassenDbContext>(options =>
        options.UseSqlServer(connectionString));

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        //Initial Konten einfügen
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<KassenDbContext>();

            if (!dbContext.Konten.Any()) // Nur wenn noch keine Konten existieren
            {
                dbContext.Konten.AddRange(new List<Konto>
        {
            new Konto { KontoName = "Girokonto", Saldo = 0 },
            new Konto { KontoName = "Sparbuch", Saldo = 0 },
            new Konto { KontoName = "Barkasse", Saldo = 0 }
        });

                dbContext.SaveChanges();
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
