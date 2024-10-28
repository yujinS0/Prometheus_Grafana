using MatchServer.Repository;
using MatchServer;
using MatchServer.Services.Interfaces;
using MatchServer.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// CORS ��å �߰� - blazor���� ȣ���� ����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.Configure<DbConfig>(builder.Configuration.GetSection("ConnectionStrings")); // DbConfig ���� �ε�

builder.Services.AddSingleton<IMemoryDb, MemoryDb>(); // Game Redis
builder.Services.AddSingleton<IRequestMatchingService, RequestMatchingService>();
builder.Services.AddSingleton<MatchWorker>(); // MatchWorker �̱���?

builder.Services.AddHttpClient(); // HttpClientFactory �߰�

// �α� ����
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddControllers();

var app = builder.Build();

// Prometheus �̵���� �߰�
app.UseMetricServer();   // /metrics ��������Ʈ ����
app.UseHttpMetrics();     // HTTP ��û�� ���� ��Ʈ�� ����

// CORS �̵���� �߰�
app.UseCors("AllowAllOrigins");

app.MapControllers();

app.Run();