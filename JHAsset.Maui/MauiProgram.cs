using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.EntityFrameworkCore;
using JHAsset.Shared.Data;
using JHAsset.Shared.Services;
using System.IO;
using Microsoft.FluentUI.AspNetCore.Components; // AddFluentUIComponents() 확장 메서드
using Microsoft.FluentUI.AspNetCore.Components.DataGrid.EntityFrameworkAdapter; // AddDataGridEntityFrameworkAdapter() 확장 메서드

namespace JHAsset.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
               });

#if DEBUG
        // 개발 모드일 때 BlazorWebView 개발자 도구 서비스 등록
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        // 꼭 필요한 BlazorWebView 서비스 등록
        builder.Services.AddMauiBlazorWebView();

        // EF Core 및 기타 서비스 등록
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "JHAsset.db");
        builder.Services.AddDbContextFactory<JHAssetContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IAssetService, AssetService>();

        // ★ Fluent UI 서비스 등록
        builder.Services.AddFluentUIComponents(options =>
        {
            // Tailwind 등의 특수 클래스명 쓸 때는
            // options.ValidateClassNames = false;
        });

        // FluentDataGrid + EFCore를 Async
        builder.Services.AddDataGridEntityFrameworkAdapter();
        
        // ★ 여기서부터 DB 생성 보장 로직 추가
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            // DbContextFactory에서 컨텍스트를 생성
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<JHAssetContext>>();
            using var dbContext = dbContextFactory.CreateDbContext();
            dbContext.Database.EnsureCreated();
        }

        return app;
    }
}