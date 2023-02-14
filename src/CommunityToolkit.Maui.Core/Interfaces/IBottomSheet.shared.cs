using CommunityToolkit.Maui.Core.Models;
using IElement = Microsoft.Maui.IElement;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace CommunityToolkit.Maui.Core;

/// <summary>
/// Represents a small View that pops up at front the Page.
/// </summary>
public interface IBottomSheet : IElement, IVisualTreeElement
{
	/// <summary>
	/// Gets the instance of the <see cref="IDeviceDisplay"/>.
	/// </summary>
	IDeviceDisplay? DeviceDisplay { get; }

	/// <summary>
	/// Gets the View that BottomSheet will be anchored.
	/// </summary>
	IView? Anchor { get; }

	/// <summary>
	/// Gets the BottomSheet's color.
	/// </summary>
	Color? BackgroundColor { get; }

	/// <summary>
	/// Gets the BottomSheet's corner radius.
	/// </summary>
	double? CornerRadius { get; }

	/// <summary>
	/// Gets the BottomSheet's handle color.
	/// </summary>
	Color? HandleColor { get; }

	/// <summary>
	/// Gets the BottomSheet's handle height.
	/// </summary>
	double? HandleHeight { get; }

	/// <summary>
	/// Gets the BottomSheet's handle width.
	/// </summary>
	double? HandleWidth { get; }

	/// <summary>
	/// Gets the spacing between the BottomSheet and its handle.
	/// </summary>
	double? HandleSpacingToBottomSheet { get; }

	/// <summary>
	/// Gets the BottomSheet's handle swipe area height.
	/// </summary>
	double? HandleSwipeAreaHeight { get; }

	/// <summary>
	/// Gets the BottomSheet's animations duration (milliseconds).
	/// </summary>
	int? AnimationDurationMillis { get; }

	/// <summary>
	/// Gets the BottomSheet's Content.
	/// </summary>
	IView? Content { get; }

	/// <summary>
	/// Gets the horizontal aspect of this element's arrangement in a container.
	/// </summary>
	LayoutAlignment HorizontalOptions { get; }

	/// <summary>
	/// Gets the CanBeDismissedByTappingOutsideOfBottomSheet property.
	/// </summary>
	bool CanBeDismissedByTappingOutsideOfBottomSheet { get; }

	/// <summary>
	/// Gets the AllowUserInteractionToSwitchBottomSheetSize property.
	/// </summary>
	bool AllowUserInteractionToSwitchBottomSheetSize { get; }

	/// <summary>
	/// Gets the BottomSheet's <see cref="BottomSheetSize"/>.
	/// </summary>
	BottomSheetSize BottomSheetSize { get; set; }

	/// <summary>
	/// Gets the BottomSheet's size.
	/// </summary>
	Size Size { get; }

	/// <summary>
	/// Gets the vertical aspect of this element's arrangement in a container.
	/// </summary>
	LayoutAlignment VerticalOptions { get; }

	/// <summary>
	/// Occurs when the BottomSheet is closed.
	/// </summary>
	/// <param name="result">Return value from the BottomSheet.</param>
	void OnClosed(object? result = null);

	/// <summary>
	/// Occurs when the BottomSheet is opened.
	/// </summary>
	void OnOpened();

	/// <summary>
	/// Occurs when the BottomSheet is dismissed by a user tapping outside of the BottomSheet.
	/// </summary>
	void OnDismissedByTappingOutsideOfBottomSheet();
}