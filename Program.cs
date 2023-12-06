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
                //운영 환경에서만 사용
                builder.Configuration.AddAzureKeyVault(
                    new Uri($"https://barunsecret.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
            else
            {
                //개발용 환경에서만 사용
                builder.Configuration.AddAzureKeyVault(
                    new Uri($"https://dev-barunsecret.vault.azure.net/"),
                    new DefaultAzureCredential());
            }

            // DB Context 
            builder.Services.AddDbContext<BarunsonContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BarunsonDBConn")));
            builder.Services.AddDbContext<BarShopContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BarShopDBConn")));

            var pginfostr = builder.Configuration.GetSection("PgMertInfos").Get<string>(); //Keyvault에서 읽었을 경우.
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


            // 뷰파일(cshtml)을 수정한 다음 프로그램을 재시작하지 않아도 바로 바뀐 내용을 확인할 수 있게 함
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