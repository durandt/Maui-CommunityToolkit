namespace CommunityToolkit.Maui.Core.Handlers;

/// <summary>
/// Handler BottomSheet control
/// </summary>
public partial class BottomSheetHandler
{
	/// <summary>
	/// PropertyMapper for BottomSheet Control
	/// </summary>
	public static IPropertyMapper<IBottomSheet, BottomSheetHandler> BottomSheetMapper = new PropertyMapper<IBottomSheet, BottomSheetHandler>(ElementMapper)
	{
		[nameof(IBottomSheet.Anchor)] = MapAnchor,
		[nameof(IBottomSheet.BackgroundColor)] = MapBackgroundColor,
		// TODO map properties
		//[nameof(IBottomSheet.CornerRadius)] = MapCornerRadius,
		//[nameof(IBottomSheet.HandleColor)] = MapHandleColor,
		//[nameof(IBottomSheet.HandleHeight)] = MapHandleHeight,
		//[nameof(IBottomSheet.HandleWidth)] = MapHandleWidth,
		//[nameof(IBottomSheet.HandleSpacingToBottomSheet)] = MapHandleSpacingToBottomSheet,
		//[nameof(IBottomSheet.HandleSwipeAreaHeight)] = MapHandleSwipeAreaHeight,
		//[nameof(IBottomSheet.AnimationDurationMillis)] = MapAnimationDurationMillis,
		[nameof(IBottomSheet.BottomSheetSize)] = MapBottomSheetSize,
		[nameof(IBottomSheet.Size)] = MapSize,
		[nameof(IBottomSheet.VerticalOptions)] = MapSize,
		[nameof(IBottomSheet.HorizontalOptions)] = MapSize,
		[nameof(IBottomSheet.CanBeDismissedByTappingOutsideOfBottomSheet)] = MapCanBeDismissedByTappingOutsideOfBottomSheet,
		[nameof(IBottomSheet.AllowUserInteractionToSwitchBottomSheetSize)] = MapAllowUserInteractionToSwitchBottomSheetSize
	};

	/// <summary>
	/// <see cref ="CommandMapper"/> for BottomSheet Control.
	/// </summary>
	public static CommandMapper<IBottomSheet, BottomSheetHandler> BottomSheetCommandMapper = new(ElementCommandMapper)
	{
		[nameof(IBottomSheet.OnClosed)] = MapOnClosed,
#if !(IOS || MACCATALYST)
		[nameof(IBottomSheet.OnOpened)] = MapOnOpened,
#endif
		[nameof(IBottomSheet.OnDismissedByTappingOutsideOfBottomSheet)] = MapOnDismissedByTappingOutsideOfBottomSheet
	};

	/// <summary>
	/// Constructor for <see cref="BottomSheetHandler"/>.
	/// </summary>
	/// <param name="mapper">Custom instance of <see cref="PropertyMapper"/>, if it's null the <see cref="BottomSheetMapper"/> will be used</param>
	/// <param name="commandMapper">Custom instance of <see cref="CommandMapper"/>, if it's null the <see cref="BottomSheetCommandMapper"/> will be used</param>
	public BottomSheetHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
		: base(mapper ?? BottomSheetMapper, commandMapper ?? BottomSheetCommandMapper)
	{
	}

	/// <summary>
	/// Default Constructor for <see cref="BottomSheetHandler"/>.
	/// </summary>
	public BottomSheetHandler()
		: base(BottomSheetMapper, BottomSheetCommandMapper)
	{
	}
}