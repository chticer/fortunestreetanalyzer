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
    InitialCatalog = "fortunestreet",
    Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault,
    CommandTimeout = (int) TimeSpan.FromMinutes(5).TotalSeconds
}.ConnectionString;

#if DEBUG
    fortuneStreetSQLConnectionString = new SqlConnectionStringBuilder
    {
        DataSource = "tcp:analyzerprojects-secretply.database.windows.net,1433",
        InitialCatalog = "fortunestreet",
        UserID = builder.Configuration["AZURE-SQLSERVER-ANALYZERPROJECTS-SECRETPLY-FORTUNESTREET-USERNAME"],
        Password = builder.Configuration["AZURE-SQLSERVER-ANALYZERPROJECTS-SECRETPLY-FORTUNESTREET-PASSWORD"],
        CommandTimeout = (int) TimeSpan.FromMinutes(5).TotalSeconds
    }.ConnectionString;
#endif

builder.Services.AddDbContext<FortuneStreetAppContext>(options => options.UseSqlServer(new SqlConnection(fortuneStreetSQLConnectionString)));
builder.Services.AddDbContext<FortuneStreetSavePreRollContext>(options => options.UseSqlServer(new SqlConnection(fortuneStreetSQLConnectionString)));
builder.Services.AddDbContext<FortuneStreetSavePostRollContext>(options => options.UseSqlServer(new SqlConnection(fortuneStreetSQLConnectionString)));

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
