using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using OmdbTerminal.ApiService.Data;
using OmdbTerminal.ApiService.Services;
using OmdbTerminal.ApiService.Middleware;
using OmdbTerminal.ApiService.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    })
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .SetMaxTop(100)
        .Count()
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<ODataOperationFilter>();
});

builder.Services.AddHttpClient<IOmdbClient, OmdbClient>(client =>
{
    client.BaseAddress = new Uri("https://www.omdbapi.com/");
});

builder.AddMySqlDbContext<OmdbDbContext>("OmdbCacheDb");
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICachedEntriesService, CachedEntriesService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Note applying migrations at startup can lead to issues in production environments, but for this use case, we can assume it's acceptable.
    // In a real-world scenario, we would use a more robust migration strategy e.g. applying migrations as part of the deployment process, using a migration tool or modifying the migration script to be idempotent
    var db = scope.ServiceProvider.GetRequiredService<OmdbDbContext>();
    //await db.Database.EnsureDeletedAsync();
    await db.Database.MigrateAsync();
}

app.MapDefaultEndpoints();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.Run();
