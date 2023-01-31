namespace CommunityToolkit.Maui.Core;

/// <summary>
/// BottomSheet dismissed event arguments used when a popup is dismissed.
/// </summary>
public class BottomSheetClosedEventArgs
{
	/// <summary>
	/// Initialization an instance of <see cref="BottomSheetClosedEventArgs"/>.
	/// </summary>
	/// <param name="result">
	/// The result of the popup.
	/// </param>
	/// <param name="wasDismissedByTappingOutsideOfBottomSheet">
	/// If the popup was dismissed by tapping outside of the BottomSheet.
	/// </param>
	public BottomSheetClosedEventArgs(object? result, bool wasDismissedByTappingOutsideOfBottomSheet)
	{
		Result = result;
		WasDismissedByTappingOutsideOfBottomSheet = wasDismissedByTappingOutsideOfBottomSheet;
	}

	/// <summary>
	/// The resulting object to return.
	/// </summary>
	public object? Result { get; }

	/// <summary>
	/// If true, then the user tapped outside the bounds of
	/// the popup (a light dismiss). If false, then the
	/// popup was dismissed by user action or code.
	/// </summary>
	public bool WasDismissedByTappingOutsideOfBottomSheet { get; }
}