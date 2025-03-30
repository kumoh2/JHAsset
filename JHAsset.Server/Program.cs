var builder = WebApplication.CreateBuilder(args);

// 서비스 등록: Razor Pages와 Blazor Server 서비스를 추가합니다.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// 개발/운영 환경에 따른 에러 처리 및 보안 설정
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// SignalR 기반 Blazor Hub와 Fallback 페이지 설정
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();