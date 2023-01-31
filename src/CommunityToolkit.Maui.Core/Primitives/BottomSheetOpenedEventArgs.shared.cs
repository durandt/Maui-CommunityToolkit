namespace CommunityToolkit.Maui.Core;

/// <summary>
/// BottomSheet opened event arguments used when a popup is opened.
/// </summary>
public class BottomSheetOpenedEventArgs : EventArgs
{
	/// <summary>
	/// Empty version of <see cref= "BottomSheetOpenedEventArgs"/>.
	/// </summary>
	public static new BottomSheetOpenedEventArgs Empty { get; } = new();
}