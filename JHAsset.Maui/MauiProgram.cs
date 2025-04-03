using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.EntityFrameworkCore;
using JHAsset.Shared.Data;
using JHAsset.Shared.Services;
using System.IO;

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

        return builder.Build();
    }
}