using GalaxyUML.Core.Services;
using GalaxyUML.Data;
using GalaxyUML.Data.Repositories;
using GalaxyUML.Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null)));

// Repositories
builder.Services.AddScoped<ITeamRepo, TeamRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IMeetingRepo, MeetingRepo>();
builder.Services.AddScoped<IDiagramRepo, DiagramRepo>();

// Services
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MeetingService>();
builder.Services.AddScoped<DiagramService>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GalaxyUML API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GalaxyUML API v1");
        c.RoutePrefix = string.Empty; // swagger na rootu
    });
}

app.UseHttpsRedirection();
app.UseCors(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthorization();
app.MapControllers();
app.Run();
