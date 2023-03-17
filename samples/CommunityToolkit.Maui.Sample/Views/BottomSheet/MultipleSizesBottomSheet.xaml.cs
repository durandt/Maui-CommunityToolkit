using CommunityToolkit.Maui.Sample.Models;
using CommunityToolkit.Maui.Views;

namespace CommunityToolkit.Maui.Sample;

public partial class MultipleSizesBottomSheet : BottomSheet
{
	public MultipleSizesBottomSheet(BottomSheetSizeConstants bottomSheetSizeConstants, IDeviceDisplay deviceDisplay)
		: base(deviceDisplay)
	{
		InitializeComponent();
		BottomSheetSize = bottomSheetSizeConstants.Small;
		BackgroundColor = Colors.White;
		HandleColor = Color.FromArgb("#F0F2F4");
		CanBeDismissedByTappingOutsideOfBottomSheet = true;
		SwipeWillDismissBottomSheet = true;
		AllowUserInteractionToSwitchBottomSheetSize = false;
		_Content.Loaded += ContentOnLoaded;
		_Content.Unloaded += ContentOnUnloaded;
	}

	void ContentOnLoaded(object? sender, EventArgs e)
	{
		Console.WriteLine("Content Loaded");
	}

	void ContentOnUnloaded(object? sender, EventArgs e)
	{
		Console.WriteLine("Content Unloaded");
	}

	public override void OnAppeared()
	{
		base.OnAppeared();
		Console.WriteLine("OnAppeared");
	}

	public override void OnDisappearing()
	{
		base.OnDisappearing();
		Console.WriteLine("OnDisappearing");
	}

	public override void OnOpened()
	{
		base.OnOpened();
		Console.WriteLine("OnOpened");
	}

	public override void OnClosed(object? result, bool wasDismissedByTappingOutsideOfBottomSheet)
	{
		base.OnClosed(result, wasDismissedByTappingOutsideOfBottomSheet);
		Console.WriteLine("OnClosed");
	}

	void PreviousSizeButton_Clicked(object? sender, EventArgs e)
	{
		if (BottomSheetSize.Previous != null)
		{
			BottomSheetSize = BottomSheetSize.Previous;
		}
	}

	void NextSizeButton_Clicked(object? sender, EventArgs e)
	{
		if (BottomSheetSize.Next != null)
		{
			BottomSheetSize = BottomSheetSize.Next;
		}
	}

	async void CloseButton_Clicked(object? sender, EventArgs e)
	{
		Close();
	}

	async void ToggleCanBeDismissedByTappingOutsideOfBottomSheetButton_Clicked(object? sender, EventArgs e)
	{
		CanBeDismissedByTappingOutsideOfBottomSheet = !CanBeDismissedByTappingOutsideOfBottomSheet;
	}

	async void ToggleSwipeWillDismissBottomSheetButton_Clicked(object? sender, EventArgs e)
	{
		SwipeWillDismissBottomSheet = !SwipeWillDismissBottomSheet;
	}

	async void ToggleAllowUserInteractionToSwitchBottomSheetSizeButton_Clicked(object? sender, EventArgs e)
	{
		AllowUserInteractionToSwitchBottomSheetSize = !AllowUserInteractionToSwitchBottomSheetSize;
	}
}