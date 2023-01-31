namespace CommunityToolkit.Maui.Sample.ViewModels.Views;

public sealed partial class CsharpBindingBottomSheetViewModel : BaseViewModel
{
	public string Title { get; } = "C# Binding BottomSheet";

	public string Message { get; } = "This is a platform specific bottom sheet with a .NET MAUI View being rendered. The behaviors of the bottom sheet will confirm to 100% this platform look and feel, but still allows you to use your .NET MAUI Controls.";
}