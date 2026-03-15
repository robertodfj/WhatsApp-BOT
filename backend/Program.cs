using Bot.Api.Database;
using Bot.Api.Helper.Auth;
using Bot.Api.Model.Yeasy;
using Bot.Api.Repository.Auth;
using Bot.Api.Repository.Yeasy;
using Bot.Api.Service.Auth;
using Bot.Api.Service.Yeasy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=bot.db"));
builder.Services.Configure<YeasyOptions>(builder.Configuration.GetSection(YeasyOptions.SectionName));
builder.Services.AddScoped<IUserVerificationHelper, UserVerificationHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserOnboardingService, UserOnboardingService>();
builder.Services.AddHttpClient<IYeasyRepository, YeasyRepository>();
builder.Services.AddScoped<IYeasyService, YeasyService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();
