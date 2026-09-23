using Microsoft.EntityFrameworkCore;
using TennisMatchmaker.Data;
// using TennisMatchmaker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TennisDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddScoped<SessionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => 
        policy.WithOrigins("http://localhost:4200") // Angular dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Program.cs runs outside any HTTP request, so there's no ambient scoped
    // TennisDbContext waiting to be injected like there is in a controller
    // constructor. CreateScope() manually creates one "fake request" scope
    // just long enough to resolve a DbContext instance and use it.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TennisDbContext>();
    await DbSeeder.SeedAsync(db);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();