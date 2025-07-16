var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddPolicy("vue", p =>
    p.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseCors("vue");

app.MapGet("/api/ping", () => "pong");  // 헬스 체크

app.Run();
