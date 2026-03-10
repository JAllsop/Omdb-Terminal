using Microsoft.EntityFrameworkCore;
using OmdbTerminal.ApiService.Data;
using OmdbTerminal.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IOmdbClient, OmdbClient>(client =>
{
    client.BaseAddress = new Uri("https://www.omdbapi.com/");
});

builder.AddMySqlDbContext<OmdbDbContext>("OmdbCacheDb");
builder.Services.AddScoped<IMovieService, MovieService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OmdbDbContext>();
    await db.Database.MigrateAsync();
}

app.MapDefaultEndpoints();

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
