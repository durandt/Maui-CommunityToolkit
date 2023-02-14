using CommunityToolkit.Maui.Core.Views;
using Microsoft.Maui.Handlers;

namespace CommunityToolkit.Maui.Core.Handlers;

public partial class BottomSheetHandler : ElementHandler<IBottomSheet, MauiBottomSheet>
{
	/// <summary>
	/// Action that's triggered when the BottomSheet is Dismissed.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">The result that should return from this BottomSheet.</param>
	public static async void MapOnClosed(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
		var vc = handler.PlatformView.ViewController;
		if (vc is not null)
		{
			//await vc.DismissViewControllerAsync(true);
			await handler.PlatformView.CloseAsync();
		}

		handler.DisconnectHandler(handler.PlatformView);
	}

	/// <summary>
	/// Action that's triggered when the BottomSheet is dismissed by tapping outside of the BottomSheet.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">The result that should return from this BottomSheet.</param>
	public static void MapOnDismissedByTappingOutsideOfBottomSheet(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
		if (handler.PlatformView is not MauiBottomSheet bottomSheetRenderer)
		{
			throw new InvalidOperationException($"{nameof(handler.PlatformView)} must be of type {typeof(BottomSheetHandler)}.");
		}

		if (bottomSheetRenderer.IsViewLoaded && view.CanBeDismissedByTappingOutsideOfBottomSheet)
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
		handler.PlatformView.SetSize(view);
		handler.PlatformView.SetLayout(view);
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
		handler.PlatformView.SetBackgroundColor(view);
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
		handler.PlatformView.SetSize(view);
		handler.PlatformView.SetLayout(view);
	}

	/// <inheritdoc/>
	protected override void ConnectHandler(MauiBottomSheet platformView)
	{
		base.ConnectHandler(platformView);
		platformView.SetElement(VirtualView);
	}

	/// <inheritdoc/>
	protected override MauiBottomSheet CreatePlatformElement()
	{
		return new MauiBottomSheet(MauiContext ?? throw new NullReferenceException(nameof(MauiContext)),
			VirtualView.DeviceDisplay ?? throw new NullReferenceException(nameof(VirtualView.DeviceDisplay)));
	}

	/// <inheritdoc/>
	protected override void DisconnectHandler(MauiBottomSheet platformView)
	{
		base.DisconnectHandler(platformView);
		platformView.CleanUp();
	}
}