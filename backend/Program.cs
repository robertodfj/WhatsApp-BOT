using System.Data;
using Bot.Api.Database;
using Bot.Api.Model.Yeasy;
using Bot.Api.Repository.Yeasy;
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
builder.Services.AddHttpClient<IYeasyRepository, YeasyRepository>();
builder.Services.AddScoped<IYeasyService, YeasyService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    await EnsureUsersSchemaAsync(dbContext);
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

static async Task EnsureUsersSchemaAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
{
    var connection = dbContext.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
    {
        await connection.OpenAsync(cancellationToken);
    }

    var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    await using var pragmaCommand = connection.CreateCommand();
    pragmaCommand.CommandText = "PRAGMA table_info(users);";

    await using var reader = await pragmaCommand.ExecuteReaderAsync(cancellationToken);
    while (await reader.ReadAsync(cancellationToken))
    {
        var columnName = reader["name"]?.ToString();
        if (!string.IsNullOrWhiteSpace(columnName))
        {
            columns.Add(columnName);
        }
    }

    if (!columns.Contains("Email"))
    {
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE users ADD COLUMN Email TEXT NULL;", cancellationToken);
    }
}
