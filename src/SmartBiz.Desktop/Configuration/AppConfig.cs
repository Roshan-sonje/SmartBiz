namespace SmartBiz.Desktop.Configuration;

public static class AppConfig
{
#if ANDROID
    public const string ApiBaseUrl = "http://10.0.2.2:5266";
#else
    public const string ApiBaseUrl = "http://localhost:5266";
#endif

    public const string AppName = "SmartBiz";
    public const string ApiVersion = "v1";
}