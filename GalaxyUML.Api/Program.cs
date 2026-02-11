using GalaxyUML.Data;
using GalaxyUML.Data.Repositories;
using GalaxyUML.Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 1. Konfiguracija baze podataka
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registracija svih repozitorijuma (Dependency Injection)
builder.Services.AddScoped<IMethodRepo, MethodRepo>();
builder.Services.AddScoped<IAttributeRepo, AttributeRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IBannedUserRepo, BannedUserRepo>();
builder.Services.AddScoped<IChatRepo, ChatRepo>();
builder.Services.AddScoped<IDiagramRepo, DiagramRepo>();
builder.Services.AddScoped<IDrawableRepo, DrawableRepo>();
builder.Services.AddScoped<IMeetingRepo, MeetingRepo>();
builder.Services.AddScoped<IMeetingParticipantRepo, MeetingParticipantRepo>();
builder.Services.AddScoped<IMessageRepo, MessageRepo>();

// DODATO: Registracija IUserRepo je neophodna jer AuthRepo zavisi od njega!
builder.Services.AddScoped<IUserRepo, UserRepo>(); 

// 3. Dodavanje kontrolera
builder.Services.AddControllers();

// 4. Konfiguracija Swagger-a
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GalaxyUML API",
        Version = "v1",
        Description = "API za upravljanje UML dijagramima, sastancima i korisnicima."
    });
});

var app = builder.Build();

// 5. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GalaxyUML API v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthorization();
app.MapControllers();

app.Run();