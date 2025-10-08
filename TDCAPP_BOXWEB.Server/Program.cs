using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 從 appsettings.json 讀取 logSetting
var logPath = builder.Configuration["logSetting:LogPath"] ?? "logs\\log-.txt";
var rollingIntervalStr = builder.Configuration["logSetting:RollingInterval"] ?? "Day";
RollingInterval rollingInterval = RollingInterval.Day;
if (!Enum.TryParse(rollingIntervalStr, true, out rollingInterval))
{
    rollingInterval = RollingInterval.Day;
}

// Serilog 設定
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(logPath, rollingInterval: rollingInterval)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();