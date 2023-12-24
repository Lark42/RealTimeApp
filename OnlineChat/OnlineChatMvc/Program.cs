using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineChatMvc.Data;
using OnlineChatMvc.Hubs;
using OnlineChatMvc.Services;

namespace OnlineChatMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("ChatConnectionString");

            builder.Services.AddDbContext<ChatContext>(options =>options.UseSqlite(connectionString));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                 .AddCookie(options => {
                     options.LoginPath = "/login";
                 });

            builder.Services.AddAuthorization();

            builder.Services.AddSignalR();

            builder.Services.AddScoped<ChatService>();

            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseInMemoryStorage());
            builder.Services.AddHangfireServer();

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

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {

            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<ChatHub>("/chat");

            var scope = app.Services.CreateScope();
            var chatServices = scope.ServiceProvider.GetService<ChatService>();

            RecurringJob.AddOrUpdate("CheckYmlFiles", () => chatServices.DeleteOldMessages(), "* * * * *");

            app.Run();
        }
    }
}