
using Microsoft.EntityFrameworkCore;
using Steal_All_The_CatsV2.Data;
using Steal_All_The_CatsV2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<CatService>();
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "StealAllTheCats",
        Version = "v1",
        Description = "An API to fetch and manage cat images from the Cats as a Service (CaaS) API."
    });
     
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "SteallAllTheCats");
    c.RoutePrefix = "swagger"; // Set Swagger UI to the root URL
});

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }



