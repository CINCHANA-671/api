using WeeklyOSApi.DATAACCESS;
using WeeklyOSApi.SERVICES;
using WeeklyOSApi.BUSINESSLOGIC;

var builder = WebApplication.CreateBuilder(args);

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<WeeklyOSService>();
builder.Services.AddScoped<WeeklyOSManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();