using Microsoft.AspNetCore.Mvc;           // 컨트롤러 특성 등
using System.Data;
using Npgsql;
using JHAsset.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. 필수 서비스 등록 -----------------------------
builder.Services.AddControllers();

// 2. DI ------------------------------------------
builder.Services.AddScoped<IDbConnection>(_ =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("AssetDb")));
builder.Services.AddScoped<AssetRepository>();

// 3. CORS ----------------------------------------
builder.Services.AddCors(o => o.AddPolicy("vue",
    p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors("vue");

// 4. 라우팅 --------------------------------------
app.MapControllers();                       // 컨트롤러 라우트 매핑

app.Run();
