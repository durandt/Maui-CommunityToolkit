using CommunityToolkit.Maui.Sample.Models;
using CommunityToolkit.Maui.Views;

namespace CommunityToolkit.Maui.Sample;

public partial class ToggleSizeBottomSheet : BottomSheet
{
	readonly Size originalSize;

	public ToggleSizeBottomSheet(BottomSheetSizeConstants bottomSheetSizeConstants)
	{
		InitializeComponent();

		Size = originalSize = bottomSheetSizeConstants.Medium;
	}

	void Button_Clicked(object? sender, EventArgs e)
	{
		if (originalSize == Size)
		{
			Size = new Size(originalSize.Width * 1.25, originalSize.Height * 1.25);
		}
		else
		{
			Size = originalSize;
		}
	}
}