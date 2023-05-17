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
		while (Navigation.ModalStack.Any())
		{
			await Navigation.PopModalAsync();
		}

		var toggleSizeBottomSheet = new MultipleSizesBottomSheet(bottomSheetSizeConstants, deviceDisplay);
		await this.ShowBottomSheetAsync(toggleSizeBottomSheet);
		if (_ShowModalSwitch.IsToggled)
		{
			await toggleSizeBottomSheet.ShowingResult;
			await ShowTestModalAsync();
		}
	}

	async Task ShowTestModalAsync()
	{
		var button = new Button { Text = "Close" };
		button.Clicked += async (sender, e) => { await Navigation.PopModalAsync(); };
		await Navigation.PushModalAsync(new NavigationPage(new ContentPage()
		{
			Content = new StackLayout
			{
				Children =
				{
					new Label { Text = "Modal shown" },
					button
				}
			}
		}));
	}
}