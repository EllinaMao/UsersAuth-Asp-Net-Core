using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAuthorizationHandler, IsRecipeOwnerHandler>();

builder.Services.AddAuthorization(options => {

    options.AddPolicy("CanManageRecipe", policyBuilder =>

    policyBuilder.AddRequirements(new IsRecipeOwnerRequirement()));

});

IConfigurationRoot _confString = new ConfigurationBuilder().

    SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

//builder.Services.AddTransient<IPasswordValidator<User>,

//        CustomPasswordValidator>(serv => new CustomPasswordValidator(6));

builder.Services.AddDbContext<ApplicationContext>(options =>

               options.UseSqlServer(_confString.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(opts =>

{

    opts.Password.RequiredLength = 5;

    opts.Password.RequireNonAlphanumeric = false;

    opts.Password.RequireLowercase = false;

    opts.Password.RequireUppercase = false;

    opts.Password.RequireDigit = false;

}).AddEntityFrameworkStores<ApplicationContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())

{

    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

    app.UseHsts();

}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(

    name: "default",

    pattern: "{controller=Home}/{action=Index}/{id?}")

    .WithStaticAssets();

using (var scope = app.Services.CreateScope())

{

    var services = scope.ServiceProvider;

    try

    {

        var applicationContext = services.GetRequiredService<ApplicationContext>();

        //applicationContext.Database.EnsureDeleted();

        applicationContext.Database.EnsureCreated();

        var userManager = services.GetRequiredService<UserManager<User>>();

        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await RoleInitializer.InitializeAsync(userManager, rolesManager);

        await RecipeInitializer.InitializeAsync(applicationContext);

    }

    catch (Exception ex)

    {

        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while seeding the database.");

    }

}


app.Run();

