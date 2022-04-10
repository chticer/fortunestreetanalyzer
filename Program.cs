using fortunestreetanalyzer.DatabaseModels.fortunestreet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
        options => options.CommandTimeout(int.MaxValue)
    )
);

builder.Services.AddDbContext<FortuneStreetSaveAnalyzerInstanceLogContext>
(
    options => options.UseSqlServer
    (
        new SqlConnection(fortuneStreetSQLConnectionString),
        options => options.CommandTimeout(int.MaxValue)
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
