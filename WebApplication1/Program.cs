using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication1.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

// Регистрация обработчиков авторизации
builder.Services.AddScoped<IAuthorizationHandler, IsRecipeOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViewRecipeHandler>();
builder.Services.AddScoped<IAuthorizationHandler, EditRecipeHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DeleteRecipeHandler>();
builder.Services.AddScoped<IAuthorizationHandler, HideRecipeHandler>();

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();

builder.Services.AddScoped<EmailHelper>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddAuthorization(options => {

    options.AddPolicy("CanManageRecipe", policyBuilder =>

    policyBuilder.AddRequirements(new IsRecipeOwnerRequirement()));

    // политики для работы с рецептами
    options.AddPolicy(RecipeOperations.View, policyBuilder =>
        policyBuilder.AddRequirements(new ViewRecipeRequirement()));

    options.AddPolicy(RecipeOperations.Edit, policyBuilder =>
        policyBuilder.AddRequirements(new EditRecipeRequirement()));

    options.AddPolicy(RecipeOperations.Delete, policyBuilder =>
        policyBuilder.AddRequirements(new DeleteRecipeRequirement()));

    options.AddPolicy(RecipeOperations.Hide, policyBuilder =>
        policyBuilder.AddRequirements(new HideRecipeRequirement()));

});


IConfigurationRoot _confString = new ConfigurationBuilder().

    SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

//builder.Services.AddTransient<IPasswordValidator<User>,

//        CustomPasswordValidator>(serv => new CustomPasswordValidator(6));

builder.Services.AddDbContext<ApplicationContext>(options =>

               options.UseSqlServer(_confString.GetConnectionString("DefaultConnection")));


builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));


builder.Services.AddIdentity<User, IdentityRole>(opts =>

{

    opts.Password.RequiredLength = 5;

    opts.Password.RequireNonAlphanumeric = false;

    opts.Password.RequireLowercase = false;

    opts.Password.RequireUppercase = false;

    opts.Password.RequireDigit = false;
    opts.SignIn.RequireConfirmedEmail = true;

}).AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();


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


        applicationContext.Database.EnsureDeleted();

        applicationContext.Database.EnsureCreated();


        var userManager = services.GetRequiredService<UserManager<User>>();

        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await RoleInitializer.InitializeAsync(userManager, rolesManager);

        await RecipeVisibilitiesInitializer.InitializeAsync(applicationContext);

        await RecipeInitializer.InitializeAsync(applicationContext);

    }

    catch (Exception ex)

    {

        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while seeding the database.");

    }

}


app.Run();

/*
 Используя ASP.NET Core Identity, реализуйте приложения с возможностями:
1) Регистрация пользователя.
2) Авторизация пользователя.
3) Редактирования личных данных.
4) Смена пароля.
5) Хранение и управление личными заметками. 
После прохождения авторизации, выполняйте редирект пользователя на страницу с его персональными заметками. Реализуйте весь набор CRUD операций по отношению к заметкам.
 */