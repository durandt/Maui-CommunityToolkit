using Google.Android.Material.BottomSheet;
using AColorRes = Android.Resource.Color;

namespace CommunityToolkit.Maui.Core.Views;

/// <summary>
/// Extension class where Helper methods for BottomSheet lives.
/// </summary>
public static class BottomSheetExtensions
{
	/// <summary>
	/// Method to update the <see cref="IBottomSheet.Anchor"/> view.
	/// </summary>
	/// <param name="dialog">An instance of <see cref="Dialog"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	/// <exception cref="NullReferenceException">if the <see cref="Android.Views.Window"/> is null an exception will be thrown.</exception>
	public static void SetAnchor(this Dialog dialog, in IBottomSheet bottomSheet)
	{
		var window = GetWindow(dialog);

		if (bottomSheet.Handler?.MauiContext is null)
		{
			return;
		}

		SetDialogPosition(bottomSheet, window);
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.BackgroundColor"/> property.
	/// </summary>
	/// <param name="dialog">An instance of <see cref="Dialog"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetColor(this Dialog dialog, in IBottomSheet bottomSheet)
	{
		if (bottomSheet.BackgroundColor is null)
		{
			return;
		}

		// TODO set bottom sheet background color
		//var window = GetWindow(dialog);
		//window.SetBackgroundDrawable(new ColorDrawable(bottomSheet.BackgroundColor.ToPlatform(AColorRes.BackgroundLight, dialog.Context)));
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet"/> property.
	/// </summary>
	/// <param name="dialog">An instance of <see cref="Dialog"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetCanBeDismissedByTappingOutsideOfBottomSheet(this Dialog dialog, in IBottomSheet bottomSheet)
	{
		dialog.SetCancelable(bottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet);
		dialog.SetCanceledOnTouchOutside(bottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet);
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.AllowUserInteractionToSwitchBottomSheetSize"/> property.
	/// </summary>
	/// <param name="dialog">An instance of <see cref="Dialog"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetAllowUserInteractionToSwitchBottomSheetSize(this Dialog dialog, in IBottomSheet bottomSheet)
	{
		if (dialog is BottomSheetDialog)
		{
			((BottomSheetDialog)dialog).Behavior.Draggable = bottomSheet.AllowUserInteractionToSwitchBottomSheetSize;
		}
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.SwipeWillDismissBottomSheet"/> property.
	/// </summary>
	/// <param name="dialog">An instance of <see cref="Dialog"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetSwipeWillDismissBottomSheet(this Dialog dialog, in IBottomSheet bottomSheet)
	{
		// TODO implement
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.BottomSheetSize"/> of the BottomSheet.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetBottomSheetSize(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		_ = bottomSheet.BottomSheetSize ?? throw new InvalidOperationException($"{nameof(bottomSheet.BottomSheetSize)} cannot be null.");
		if (!bottomSheet.BottomSheetSize.ContentSize.IsZero)
		{
			mauiBottomSheet.SetBottomSheetSize(bottomSheet.BottomSheetSize);
		}
		else if (bottomSheet.Content is not null)
		{
			mauiBottomSheet.Close();
		}
	}

	static void SetDialogPosition(in IBottomSheet bottomSheet, Android.Views.Window window)
	{
		var gravityFlags = Android.Views.GravityFlags.Bottom | Android.Views.GravityFlags.CenterHorizontal;
		window.SetGravity(gravityFlags);
	}

	internal static Android.Views.Window GetWindow(in Dialog dialog) =>
		dialog.Window ?? throw new NullReferenceException("Android.Views.Window is null.");
}