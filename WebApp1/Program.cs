using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using CartItemService.Services;
using Iemailsender.Services;
using Emailsender.Services;
namespace WebApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages(); // For Razor Pages
            builder.Services.AddControllersWithViews();

            // Configure Entity Framework
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // Add Identity services
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict; // Adjust as needed
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are sent only over HTTPS
                options.LoginPath = "/Account/Login"; // Path to the login page
                options.LogoutPath = "/Account/Logout"; // Path to the logout page
                options.AccessDeniedPath = "/Account/AccessDenied"; // Path for access denied
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Adjust session expiration time as needed
            });

            // Configure Role-Based Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });
            builder.Services.AddScoped<CartService>();
            builder.Services.AddTransient<ICustomEmailSender, CustomEmailSender>();



            // Add session services
            builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; // Ensure the session cookie is used
            });
           
            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Add session middleware
            app.UseSession();
            // Configure endpoints
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.MapControllerRoute(
                name: "home",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "register",
                pattern: "{controller=Account}/{action=Register}/{id?}");

            app.MapRazorPages();

            // Seed roles and admin user
            await SeedDatabase(app);

            app.Run();
        }

        private static async Task SeedDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Get the role manager and user manager
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            // Seed roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed admin user
            var adminUser = await userManager.FindByEmailAsync("harinisree2023@gmail.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "harinisree2023@gmail.com", Email = "harinisree2023@gmail.com" };
                var result = await userManager.CreateAsync(adminUser, "Harini@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            
        }
    }
}
