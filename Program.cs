using Azure.Core;
using Azure.Identity;
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

string azureCredentialSQLToken = new DefaultAzureCredential().GetToken(new TokenRequestContext(new[]
{
    "https://database.windows.net/.default"
})).Token;

SqlConnection fortuneStreetAppContextSQLConnection = new SqlConnection(fortuneStreetSQLConnectionString);

fortuneStreetAppContextSQLConnection.AccessToken = azureCredentialSQLToken;

builder.Services.AddDbContext<FortuneStreetAppContext>
(
    options => options.UseSqlServer
    (
        fortuneStreetAppContextSQLConnection,
        options => options.CommandTimeout(int.MaxValue)
    )
);

SqlConnection fortuneStreetSaveAnalyzerInstanceLogContextSQLConnection = new SqlConnection(fortuneStreetSQLConnectionString);

fortuneStreetSaveAnalyzerInstanceLogContextSQLConnection.AccessToken = azureCredentialSQLToken;

builder.Services.AddDbContext<FortuneStreetSaveAnalyzerInstanceLogContext>
(
    options => options.UseSqlServer
    (
        fortuneStreetSaveAnalyzerInstanceLogContextSQLConnection,
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
