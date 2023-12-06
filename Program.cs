using Azure.Identity;
using Barunson.BBarunsonWeb.Config;
using Barunson.BBarunsonWeb.Services;
using Barunson.DbContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Barunson.BBarunsonWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsProduction())
            {
                //� ȯ�濡���� ���
                builder.Configuration.AddAzureKeyVault(
                    new Uri($"https://barunsecret.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
            else
            {
                //���߿� ȯ�濡���� ���
                builder.Configuration.AddAzureKeyVault(
                    new Uri($"https://dev-barunsecret.vault.azure.net/"),
                    new DefaultAzureCredential());
            }

            // DB Context 
            builder.Services.AddDbContext<BarunsonContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BarunsonDBConn")));
            builder.Services.AddDbContext<BarShopContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BarShopDBConn")));

            var pginfostr = builder.Configuration.GetSection("PgMertInfos").Get<string>(); //Keyvault���� �о��� ���.
            if (!string.IsNullOrEmpty(pginfostr))
                builder.Services.AddSingleton<List<PgMertInfo>>(JsonSerializer.Deserialize<List<PgMertInfo>>(pginfostr)!);
            else
                builder.Services.AddSingleton<List<PgMertInfo>>(builder.Configuration.GetSection("PgMertInfos").Get<List<PgMertInfo>>());

            builder.Services.AddHttpClient();

            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
			
			builder.Services.AddHttpClient();
            builder.Services.AddScoped<ITossPaymentService, TossPaymentService>();


            // ������(cshtml)�� ������ ���� ���α׷��� ��������� �ʾƵ� �ٷ� �ٲ� ������ Ȯ���� �� �ְ� ��
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}