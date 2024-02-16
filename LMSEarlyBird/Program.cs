using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Repository;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore;
using Stripe;
using Stripe.Checkout;
using Azure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Settings for connecting to the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers(options =>
{
    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
// adds the identitiy framework
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IUserIdentityService,UserIdentityService>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IStudentCourseRepository, StudentCourseRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IAssignmentsRepository, AssignmentsRepository>();
builder.Services.AddScoped<ILinksRepository, LinksRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    // await Seed.SeedUsersAndRolesAsync(app);
    // Seed.SeedCourse(app);
    // Seed.SeedRooms(app);
    // Seed.SeedDepartments(app);
} else if(args.Length == 1 && args[0].ToLower() == "testdb")
{

}

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

public class StripeOptions
{
    public string option { get; set; }
}

namespace LMSEarlyBird
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
              .UseUrls("http://0.0.0.0:7243")
              .UseWebRoot("Payment")
              .UseStartup<Startup>()
              .Build()
              .Run();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // This is a public sample test API key.
            // Don’t submit any personally identifiable information in requests made with this key.
            // Sign in to see your own test API key embedded in code samples.
            StripeConfiguration.ApiKey = "sk_test_51Ogs9OA3uLdsTHsjYGPdb0TTncfQoAirAiKbDqVRhHD13TwX28i76mFpSLtHUXssoXoATUlQDGXTScu1e27UJsWp00TfVoL8CR";
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    [Route("create-checkout-session")]
    [ApiController]
    public class CheckoutApiController : Controller
    {


        [HttpPost]
        public ActionResult Create()
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = 2000,
              Currency = "usd",
              ProductData = new SessionLineItemPriceDataProductDataOptions
              {
                Name = "T-shirt",
              },
            },
            Quantity = 1,
          },
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:7243/Payment/success",
                CancelUrl = "http://localhost:7243/Payment/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = 2000,
              Currency = "usd",
              ProductData = new SessionLineItemPriceDataProductDataOptions
              {
                Name = "T-shirt",
              },
            },
            Quantity = 1,
          },
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:7243/Payment/success",
                CancelUrl = "http://localhost:7243/Payment/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}