// Program.cs  (Minimal API 스타일 예시, .NET 8)
using System.Text.Json;
using Npgsql;
using System.Data;
using JHAsset.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1) CORS 정책 등록
const string CorsPolicy = "DevCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// 2) 컨트롤러 · DI
builder.Services.AddControllers()
        .AddJsonOptions(o =>
            o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddScoped<IDbConnection>(_ =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("AssetDb")));
builder.Services.AddScoped<AssetRepository>();

var app = builder.Build();

app.Urls.Clear();

// 3) 미들웨어 순서
app.Urls.Add("http://localhost:5000");   // HTTP만
// app.UseHttpsRedirection();            // 주의: https 안 쓸 거면 제거

app.UseRouting();
app.UseCors(CorsPolicy);                 // ← UseRouting 다음, MapControllers 이전
app.MapControllers();

app.Run();
