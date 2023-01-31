namespace CommunityToolkit.Maui.Views;

public static partial class BottomSheetExtensions
{
	static void PlatformShowBottomSheet(BottomSheet bottomSheet, IMauiContext mauiContext) =>
		throw new NotSupportedException($"The current platform '{DeviceInfo.Platform}' does not support CommunityToolkit.Maui.Core.BottomSheet.");

	static Task<object?> PlatformShowBottomSheetAsync(BottomSheet bottomSheet, IMauiContext mauiContext) =>
		throw new NotSupportedException($"The current platform '{DeviceInfo.Platform}' does not support CommunityToolkit.Maui.Core.BottomSheet.");
}