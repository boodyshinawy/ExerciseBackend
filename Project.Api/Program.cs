//using DotNetEnv;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
//Env.Load();

// Configure services and other configurations
//builder.Configuration.AddEnvironmentVariables();

ConfigurationManager configuration = builder.Configuration;

string connectionString = configuration.GetConnectionString("DefaultConnection");

QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;



builder.Services.AddMapper();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, option) =>
{
    UpdateAuditableEntitiesInterceptor updateAuditableEntities = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();
    option.UseApplicationServiceProvider(sp);
    option.UseNpgsql(connectionString,
         builder => builder.MigrationsAssembly(typeof(Program).Assembly.FullName))
        .UseSnakeCaseNamingConvention()
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        .EnableSensitiveDataLogging() //must remove in production
        .EnableDetailedErrors() //must remove in production
        .AddInterceptors(updateAuditableEntities);
});

builder.Services.AddMemoryCache();



builder.Services.AddCors(options =>
                        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
//Add services to the container.
builder.Services.ConfigureSecurityServices(configuration);

//to make configuration as in its domains
builder.Services.AddInstallerFromAssemblies(configuration, Assembly.GetExecutingAssembly());


// Configure supported cultures
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
            new CultureInfo("en"),
            new CultureInfo("ar")
        };

    foreach (var cluture in supportedCultures)
    {
        cluture.DateTimeFormat.Calendar = new GregorianCalendar();
        cluture.NumberFormat.NumberDecimalSeparator = ".";
        cluture.NumberFormat.CurrencyDecimalSeparator = ".";
    }

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ToDocument>();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLocalization(); // Localization

//app.UseMiddleware<ExceptionMiddleware>(); // Exception Middleware

app.UseHttpsRedirection(); // HTTPS Redirection

// app.UseStaticFiles(); // Serve Static Files

app.UseRouting();

app.UseCors(); // Apply CORS

// app.UseCors("MyCorsPolicy"); // Apply the CORS policy
// app.UseMiddleware<RateLimitingMiddleware>(); // Rate Limiting Middleware


app.UseAuthentication(); // Authentication
app.UseAuthorization(); // Authorization

app.UseMiddleware<ValidateSessionMiddleware>(); // Validate Session Middleware

app.MapControllers();
//await app.MigrateAndSeedAsync(DataSeeder.SeedAsync);

app.Run();
