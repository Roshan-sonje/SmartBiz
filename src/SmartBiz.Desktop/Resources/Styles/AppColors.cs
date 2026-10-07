namespace SmartBiz.Desktop.Resources.Styles;

public static class AppColors
{
    public static void Register(ResourceDictionary resources)
    {
        // Brand
        resources["BrandPrimary"] = Color.FromArgb("#1E3A8A");
        resources["BrandPrimaryDark"] = Color.FromArgb("#1E40AF");
        resources["BrandPrimaryLight"] = Color.FromArgb("#3B82F6");
        resources["BrandAccent"] = Color.FromArgb("#0D9488");
        resources["BrandAccentDark"] = Color.FromArgb("#0F766E");
        resources["BrandAccentLight"] = Color.FromArgb("#14B8A6");

        // Background & Surface
        resources["BackgroundPrimary"] = Color.FromArgb("#F9FAFB");
        resources["BackgroundSecondary"] = Color.FromArgb("#F3F4F6");
        resources["SurfacePrimary"] = Color.FromArgb("#FFFFFF");
        resources["SurfaceSecondary"] = Color.FromArgb("#F9FAFB");
        resources["SurfaceElevated"] = Color.FromArgb("#FFFFFF");

        // Text
        resources["TextPrimary"] = Color.FromArgb("#111827");
        resources["TextSecondary"] = Color.FromArgb("#6B7280");
        resources["TextTertiary"] = Color.FromArgb("#9CA3AF");
        resources["TextInverse"] = Color.FromArgb("#FFFFFF");
        resources["TextDisabled"] = Color.FromArgb("#D1D5DB");

        // Border
        resources["BorderDefault"] = Color.FromArgb("#E5E7EB");
        resources["BorderStrong"] = Color.FromArgb("#D1D5DB");
        resources["BorderFocus"] = Color.FromArgb("#3B82F6");

        // Status
        resources["Success"] = Color.FromArgb("#059669");
        resources["SuccessLight"] = Color.FromArgb("#D1FAE5");
        resources["Warning"] = Color.FromArgb("#D97706");
        resources["WarningLight"] = Color.FromArgb("#FEF3C7");
        resources["Danger"] = Color.FromArgb("#DC2626");
        resources["DangerLight"] = Color.FromArgb("#FEE2E2");
        resources["Info"] = Color.FromArgb("#2563EB");
        resources["InfoLight"] = Color.FromArgb("#DBEAFE");
        resources["Neutral"] = Color.FromArgb("#6B7280");
        resources["NeutralLight"] = Color.FromArgb("#F3F4F6");

        // Sidebar
        resources["SidebarBackground"] = Color.FromArgb("#1F2937");
        resources["SidebarBackgroundHover"] = Color.FromArgb("#374151");
        resources["SidebarBackgroundActive"] = Color.FromArgb("#0D9488");
        resources["SidebarText"] = Color.FromArgb("#D1D5DB");
        resources["SidebarTextActive"] = Color.FromArgb("#FFFFFF");
        resources["SidebarTextMuted"] = Color.FromArgb("#9CA3AF");

        // Header
        resources["HeaderBackground"] = Color.FromArgb("#FFFFFF");
        resources["HeaderBorder"] = Color.FromArgb("#E5E7EB");

        // Shadows / overlay
        resources["OverlayBackground"] = Color.FromRgba(0, 0, 0, 128);
        resources["ShadowSoft"] = Color.FromRgba(0, 0, 0, 10);
        resources["ShadowMedium"] = Color.FromRgba(0, 0, 0, 26);
        resources["ShadowStrong"] = Color.FromRgba(0, 0, 0, 51);
    }
}