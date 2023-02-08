using CommunityToolkit.Maui.Sample.Models;
using CommunityToolkit.Maui.Sample.ViewModels.Views;
using CommunityToolkit.Maui.Views;

namespace CommunityToolkit.Maui.Sample.Pages.Views;

public partial class MultipleBottomSheetPage : BasePage<MultipleBottomSheetViewModel>
{
	readonly IDeviceDisplay deviceDisplay;
	readonly BottomSheetSizeConstants bottomSheetSizeConstants;
	readonly CsharpBindingBottomSheetViewModel csharpBindingBottomSheetViewModel;

	public MultipleBottomSheetPage(IDeviceInfo deviceInfo,
		                        IDeviceDisplay deviceDisplay,
								BottomSheetSizeConstants bottomSheetSizeConstants,
								MultipleBottomSheetViewModel multipleBottomSheetViewModel,
								CsharpBindingBottomSheetViewModel csharpBindingBottomSheetViewModel)
		: base(multipleBottomSheetViewModel)
	{
		InitializeComponent();

		this.deviceDisplay = deviceDisplay;
		this.bottomSheetSizeConstants = bottomSheetSizeConstants;
		this.csharpBindingBottomSheetViewModel = csharpBindingBottomSheetViewModel;
	}

	async void HandleToggleSizeBottomSheetButtonClicked(object sender, EventArgs e)
	{
		var toggleSizeBottomSheet = new MultipleSizesBottomSheet(bottomSheetSizeConstants, deviceDisplay);
		await this.ShowBottomSheetAsync(toggleSizeBottomSheet);
	}
}