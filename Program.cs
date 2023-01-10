using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#if DEBUG
    Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", builder.Configuration["AZURE_CLIENT_ID"]);
    Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", builder.Configuration["AZURE_CLIENT_SECRET"]);
    Environment.SetEnvironmentVariable("AZURE_TENANT_ID", builder.Configuration["AZURE_TENANT_ID"]);
#endif

// Add services to the container.
builder.Services.AddRazorPages();

string fortuneStreetSQLConnectionString = new SqlConnectionStringBuilder
{
    DataSource = "tcp:analyzerprojects-secretply.database.windows.net,1433",
    InitialCatalog = "fortunestreet"
}.ConnectionString;

builder.Services.AddDbContext<FortuneStreetAppContext>
(
    options => options.UseSqlServer
    (
        new SqlConnection(fortuneStreetSQLConnectionString),
        options => options.CommandTimeout((int) new TimeSpan(0, 3, 0).TotalSeconds)
    )
);

builder.Services.AddDbContext<FortuneStreetSaveAnalyzerInstanceLogContext>
(
    options => options.UseSqlServer
    (
        new SqlConnection(fortuneStreetSQLConnectionString),
        options => options.CommandTimeout((int) new TimeSpan(0, 3, 0).TotalSeconds)
    )
);

builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});

builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseFileServer();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
