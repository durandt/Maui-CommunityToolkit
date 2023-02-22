using CommunityToolkit.Maui.Core.Views;
using Microsoft.Maui.Handlers;
using AView = Android.Views.View;

namespace CommunityToolkit.Maui.Core.Handlers;

public partial class BottomSheetHandler : ElementHandler<IBottomSheet, MauiBottomSheet>
{
	internal AView? Container { get; set; }

	/// <summary>
	/// Action that's triggered when the BottomSheet is closed
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">The result that should return from this BottomSheet.</param>
	public static void MapOnClosed(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
		var bottomSheet = handler.PlatformView;

		if (bottomSheet.IsShowing)
		{
			bottomSheet.Dismiss();
		}

		handler.DisconnectHandler(bottomSheet);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet is Opened.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">We don't need to provide the result parameter here.</param>
	public static void MapOnOpened(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
		handler.PlatformView.Show();
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet is dismissed by tapping outside of the bottom sheet.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">The result that should return from this BottomSheet.</param>
	public static void MapOnDismissedByTappingOutsideOfBottomSheet(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
		if (view.CanBeDismissedByTappingOutsideOfBottomSheet)
		{
			view.OnDismissedByTappingOutsideOfBottomSheet();
		}
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.Anchor"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapAnchor(BottomSheetHandler handler, IBottomSheet view)
	{
		handler.PlatformView.SetAnchor(view);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapCanBeDismissedByTappingOutsideOfBottomSheet(BottomSheetHandler handler, IBottomSheet view)
	{
		handler.PlatformView.SetCanBeDismissedByTappingOutsideOfBottomSheet(view);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.SwipeWillDismissBottomSheet"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapSwipeWillDismissBottomSheet(BottomSheetHandler handler, IBottomSheet view)
	{
		handler.PlatformView.SetSwipeWillDismissBottomSheet(view);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.AllowUserInteractionToSwitchBottomSheetSize"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapAllowUserInteractionToSwitchBottomSheetSize(BottomSheetHandler handler, IBottomSheet view)
	{
		handler.PlatformView.SetAllowUserInteractionToSwitchBottomSheetSize(view);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.BackgroundColor"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapBackgroundColor(BottomSheetHandler handler, IBottomSheet view)
	{
		handler.PlatformView.SetColor(view);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.BottomSheetSize"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapBottomSheetSize(BottomSheetHandler handler, IBottomSheet view)
	{
		handler.PlatformView.SetBottomSheetSize(view);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet <see cref="IBottomSheet.Size"/> property changes.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	public static void MapSize(BottomSheetHandler handler, IBottomSheet view)
	{
		ArgumentNullException.ThrowIfNull(handler.Container);

		//handler.PlatformView.SetSize(handler.Container);
		//handler.PlatformView.SetAnchor(view);
	}

	/// <inheritdoc/>
	protected override MauiBottomSheet CreatePlatformElement()
	{
		_ = MauiContext ?? throw new InvalidOperationException("MauiContext is null, please check your MauiApplication.");
		_ = MauiContext.Context ?? throw new InvalidOperationException("Android Context is null, please check your MauiApplication.");

		return new MauiBottomSheet(MauiContext.Context, MauiContext);
	}

	/// <inheritdoc/>
	protected override void ConnectHandler(MauiBottomSheet platformView)
	{
		Container = platformView.SetElement(VirtualView);
	}

	/// <inheritdoc/>
	protected override void DisconnectHandler(MauiBottomSheet platformView)
	{
		platformView.Dispose();
	}

	void OnShowed(object? sender, EventArgs args)
	{
		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null");

		VirtualView.OnOpened();
	}
}