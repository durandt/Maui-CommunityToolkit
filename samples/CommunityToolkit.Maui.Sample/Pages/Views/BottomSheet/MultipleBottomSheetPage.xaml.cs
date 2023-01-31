using CommunityToolkit.Maui.Sample.Models;
using CommunityToolkit.Maui.Sample.ViewModels.Views;
using CommunityToolkit.Maui.Views;

namespace CommunityToolkit.Maui.Sample.Pages.Views;

public partial class MultipleBottomSheetPage : BasePage<MultipleBottomSheetViewModel>
{
	readonly BottomSheetSizeConstants bottomSheetSizeConstants;
	readonly CsharpBindingBottomSheetViewModel csharpBindingBottomSheetViewModel;

	public MultipleBottomSheetPage(IDeviceInfo deviceInfo,
								BottomSheetSizeConstants bottomSheetSizeConstants,
								MultipleBottomSheetViewModel multipleBottomSheetViewModel,
								CsharpBindingBottomSheetViewModel csharpBindingBottomSheetViewModel)
		: base(multipleBottomSheetViewModel)
	{
		InitializeComponent();

		this.bottomSheetSizeConstants = bottomSheetSizeConstants;
		this.csharpBindingBottomSheetViewModel = csharpBindingBottomSheetViewModel;
	}

	async void HandleToggleSizeBottomSheetButtonClicked(object sender, EventArgs e)
	{
		var toggleSizeBottomSheet = new ToggleSizeBottomSheet(bottomSheetSizeConstants);
		await this.ShowBottomSheetAsync(toggleSizeBottomSheet);
	}
}