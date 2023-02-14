using System.Runtime.InteropServices;
using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Maui;
using Microsoft.Maui.Platform;

namespace CommunityToolkit.Maui.Core.Views;
/// <summary>
/// Extension class where Helper methods for BottomSheet lives.
/// </summary>
public static class BottomSheetExtensions
{
	/// <summary>
	/// Method to update the <see cref="IBottomSheet.BottomSheetSize"/> of the BottomSheet.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetBottomSheetSize(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		if (!bottomSheet.BottomSheetSize.ContentSize.IsZero)
		{
			mauiBottomSheet.SetBottomSheetSize(bottomSheet.BottomSheetSize);
		}
		else if (bottomSheet.Content is not null)
		{
			mauiBottomSheet.Close();
		}
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.Size"/> of the BottomSheet.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetSize(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		if (!bottomSheet.Size.IsZero)
		{
			mauiBottomSheet.PreferredContentSize = new CGSize(bottomSheet.Size.Width, bottomSheet.Size.Height);
		}
		else if (bottomSheet.Content is not null)
		{
			var content = bottomSheet.Content;
			if (!content.Width.IsZeroOrNaN() || !content.Height.IsZeroOrNaN())
			{
				mauiBottomSheet.PreferredContentSize = new CGSize(content.Width, content.Height);
			}
			else
			{
				var measure = bottomSheet.Content.Measure(double.PositiveInfinity, double.PositiveInfinity);
				mauiBottomSheet.PreferredContentSize = new CGSize(measure.Width, measure.Height);
			}
		}
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.BackgroundColor"/> of the BottomSheet.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetBackgroundColor(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		if (mauiBottomSheet.PopoverPresentationController is not null && bottomSheet.BackgroundColor == Colors.Transparent)
		{
			mauiBottomSheet.PopoverPresentationController.PopoverBackgroundViewType = typeof(TransparentPopoverBackgroundView);
		}

		if (mauiBottomSheet.PopoverViewController is null)
		{
			return;
		}

		var color = bottomSheet.BackgroundColor?.ToPlatform();
		mauiBottomSheet.PopoverViewController.BackgroundColor = color;
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet"/> property of the BottomSheet.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetCanBeDismissedByTappingOutsideOfBottomSheet(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		if (OperatingSystem.IsIOSVersionAtLeast(13))
		{
			mauiBottomSheet.ModalInPresentation = !bottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet;
		}
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.SwipeWillDismissBottomSheet"/> property of the BottomSheet.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetSwipeWillDismissBottomSheet(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		mauiBottomSheet.SwipeWillDismissBottomSheet = bottomSheet.SwipeWillDismissBottomSheet;
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.AllowUserInteractionToSwitchBottomSheetSize"/> property of the BottomSheet.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetAllowUserInteractionToSwitchBottomSheetSize(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		mauiBottomSheet.BlockUserInteractionToSwitchBottomSheetSize = !bottomSheet.AllowUserInteractionToSwitchBottomSheetSize;
	}

	/// <summary>
	/// Method to update the layout of the BottomSheet and <see cref="IBottomSheet.Content"/>.
	/// </summary>
	/// <param name="mauiBottomSheet">An instance of <see cref="MauiBottomSheet"/>.</param>
	/// <param name="bottomSheet">An instance of <see cref="IBottomSheet"/>.</param>
	public static void SetLayout(this MauiBottomSheet mauiBottomSheet, in IBottomSheet bottomSheet)
	{
		if (mauiBottomSheet.View is null)
		{
			return;
		}

		CGRect frame;

		if (mauiBottomSheet.ViewController?.View?.Window is UIWindow window)
		{
			frame = window.Frame;
		}
		else
		{
			frame = UIScreen.MainScreen.Bounds;
		}

		if (mauiBottomSheet.PopoverPresentationController is null)
		{
			throw new InvalidOperationException("PopoverPresentationController cannot be null");
		}

		if (bottomSheet.Anchor is null)
		{
			var originY = bottomSheet.VerticalOptions switch
			{
				Microsoft.Maui.Primitives.LayoutAlignment.End => frame.Height,
				Microsoft.Maui.Primitives.LayoutAlignment.Center => frame.GetMidY(),
				_ => 0f
			};

			var originX = bottomSheet.HorizontalOptions switch
			{
				Microsoft.Maui.Primitives.LayoutAlignment.End => frame.Width,
				Microsoft.Maui.Primitives.LayoutAlignment.Center => frame.GetMidX(),
				_ => 0f
			};

			if (DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
			{
				if (bottomSheet.VerticalOptions == Microsoft.Maui.Primitives.LayoutAlignment.End)
				{
					originY -= (mauiBottomSheet.PreferredContentSize.Height / 2);
				}

				if (bottomSheet.HorizontalOptions == Microsoft.Maui.Primitives.LayoutAlignment.End)
				{
					originX -= (mauiBottomSheet.PreferredContentSize.Width);
				}

				if (bottomSheet.HorizontalOptions == Microsoft.Maui.Primitives.LayoutAlignment.Center)
				{
					originX -= (mauiBottomSheet.PreferredContentSize.Width / 2);
				}
			}

			mauiBottomSheet.PopoverPresentationController.SourceRect = new CGRect(originX, originY, 0, 0);
			mauiBottomSheet.PopoverPresentationController.PermittedArrowDirections = 0;
		}
		else
		{
			var view = bottomSheet.Anchor.ToPlatform(bottomSheet.Handler?.MauiContext ?? throw new InvalidOperationException($"{nameof(bottomSheet.Handler.MauiContext)} Cannot Be Null"));
			mauiBottomSheet.PopoverPresentationController.SourceView = view;
			mauiBottomSheet.PopoverPresentationController.SourceRect = view.Bounds;
		}
	}

	class TransparentPopoverBackgroundView : UIPopoverBackgroundView
	{
		public TransparentPopoverBackgroundView(IntPtr handle) : base(handle)
		{
			BackgroundColor = Colors.Transparent.ToPlatform();
			Alpha = 0.0f;
		}

		public override NFloat ArrowOffset { get; set; }

		public override UIPopoverArrowDirection ArrowDirection { get; set; }

		[Export("arrowHeight")]
		static new float GetArrowHeight()
		{
			return 0f;
		}

		[Export("arrowBase")]
		static new float GetArrowBase()
		{
			return 0f;
		}

		[Export("contentViewInsets")]
		static new UIEdgeInsets GetContentViewInsets()
		{
			return UIEdgeInsets.Zero;
		}
	}
}