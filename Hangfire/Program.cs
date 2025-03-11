using Hangfire;
using Hangfire.Hangfire;
using Hangfire.LiteDB;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(config => config.UseLiteDbStorage("hangfire.db"));
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient<ApiCallService>();
//builder.Services.AddTransient<IRecurringJobManager>(sp => sp.GetRequiredService<RecurringJobManager>());

var app = builder.Build();

//Configure schedule job
using (var scope = app.Services.CreateScope())
{
    var apiService = scope.ServiceProvider.GetRequiredService<ApiCallService>();

    //Call in every 1 minute
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate("api-call", () => apiService.CallScheduleApi(), "0/1 * * * *"); 
}

//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/hangfire-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable Hangfire Dashboard for monitoring jobs
app.UseHangfireDashboard();

app.MapControllers();
app.Run();


