namespace CommunityToolkit.Maui.Core.Handlers;

public partial class BottomSheetHandler : Microsoft.Maui.Handlers.ElementHandler<IBottomSheet, object>
{
	/// <inheritdoc/>
	protected override object CreatePlatformElement() => throw new NotSupportedException();

	/// <summary>
	/// Action that's triggered when the BottomSheet is closed.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">The result that should return from this BottomSheet.</param>
	public static void MapOnClosed(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet is Opened.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">We don't need to provide the result parameter here.</param>
	public static void MapOnOpened(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet is dismissed by tapping outside of the BottomSheet.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">The result that should return from this BottomSheet.</param>
	public static void MapOnDismissedByTappingOutsideOfBottomSheet(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.Anchor"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapAnchor(BottomSheetHandler handler, IBottomSheet view)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapCanBeDismissedByTappingOutsideOfBottomSheet(BottomSheetHandler handler, IBottomSheet view)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.SwipeWillDismissBottomSheet"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapSwipeWillDismissBottomSheet(BottomSheetHandler handler, IBottomSheet view)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.AllowUserInteractionToSwitchBottomSheetSize"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapAllowUserInteractionToSwitchBottomSheetSize(BottomSheetHandler handler, IBottomSheet view)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.BackgroundColor"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapBackgroundColor(BottomSheetHandler handler, IBottomSheet view)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.BottomSheetSize"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapBottomSheetSize(BottomSheetHandler handler, IBottomSheet view)
	{
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.Size"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapSize(BottomSheetHandler handler, IBottomSheet view)
	{
	}
}