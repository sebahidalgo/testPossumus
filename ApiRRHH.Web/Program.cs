using ApiRRHH.Services;
using ApiRRHH.Services.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ApiRRHH.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
;
builder.Services.AddDbContext<ApiRRHHContext>(opt => opt.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:RRHHDB").Value));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IApiRRHHService, ApiRRHHService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseLogging();

var app = builder.Build();
app.UseActivityLog();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiRRHHContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
