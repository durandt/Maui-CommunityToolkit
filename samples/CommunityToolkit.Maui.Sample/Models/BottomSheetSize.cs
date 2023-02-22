using CommunityToolkit.Maui.Core.Models;

namespace CommunityToolkit.Maui.Sample.Models;

public class BottomSheetSizeConstants
{
	public BottomSheetSizeConstants(IDeviceDisplay deviceDisplay)
	{
		var width = deviceDisplay.MainDisplayInfo.Width / deviceDisplay.MainDisplayInfo.Density;
		var tinyHeight = Math.Max(212.0, 0.25 * (deviceDisplay.MainDisplayInfo.Height / deviceDisplay.MainDisplayInfo.Density));
		var smallHeight = 0.5 * (deviceDisplay.MainDisplayInfo.Height / deviceDisplay.MainDisplayInfo.Density);
		var mediumHeight = 0.75 * (deviceDisplay.MainDisplayInfo.Height / deviceDisplay.MainDisplayInfo.Density);
		var largeHeight = 0.9 * (deviceDisplay.MainDisplayInfo.Height / deviceDisplay.MainDisplayInfo.Density);
		Collapsed = new BottomSheetSize(new(width, 0), 32, true, null) { Name = nameof(Collapsed) };
		Tiny = new BottomSheetSize(new(width, tinyHeight), 32, false, Collapsed) { Name = nameof(Tiny) };
		Small = new BottomSheetSize(new(width, smallHeight), 32, false, Tiny) { Name = nameof(Small) };
		Medium = new BottomSheetSize(new(width, mediumHeight), 32, false, Small) { Name = nameof(Medium) };
		Large = new BottomSheetSize(new(width, largeHeight), 32, false, Medium) { Name = nameof(Large) };
	}

	public BottomSheetSize Collapsed { get; }

	public BottomSheetSize Tiny { get; }

	public BottomSheetSize Small { get; }

	public BottomSheetSize Medium { get; }

	public BottomSheetSize Large { get; }
}