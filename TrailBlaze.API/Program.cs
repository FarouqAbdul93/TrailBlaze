using Microsoft.EntityFrameworkCore;
using TrailBlaze.API.Data_DbContext;
using TrailBlaze.API.Repositories;
using TrailBlaze.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TrailBlazeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorUI", policy =>
    {
        policy.WithOrigins("https://localhost:7239", "http://localhost:5158")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ITrailRepository, TrailRepository>();
builder.Services.AddControllers();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddHttpClient<IOverpassService, OverpassService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorUI");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
