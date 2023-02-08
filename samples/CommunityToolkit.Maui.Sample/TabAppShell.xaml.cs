using CommunityToolkit.Maui.Sample.Pages;
using CommunityToolkit.Maui.Sample.Pages.Alerts;
using CommunityToolkit.Maui.Sample.Pages.Behaviors;
using CommunityToolkit.Maui.Sample.Pages.Converters;
using CommunityToolkit.Maui.Sample.Pages.Essentials;
using CommunityToolkit.Maui.Sample.Pages.Extensions;
using CommunityToolkit.Maui.Sample.Pages.ImageSources;
using CommunityToolkit.Maui.Sample.Pages.Layouts;
using CommunityToolkit.Maui.Sample.Pages.Views;
using CommunityToolkit.Maui.Sample.ViewModels;
using CommunityToolkit.Maui.Sample.ViewModels.Alerts;
using CommunityToolkit.Maui.Sample.ViewModels.Behaviors;
using CommunityToolkit.Maui.Sample.ViewModels.Converters;
using CommunityToolkit.Maui.Sample.ViewModels.Essentials;
using CommunityToolkit.Maui.Sample.ViewModels.ImageSources;
using CommunityToolkit.Maui.Sample.ViewModels.Layouts;
using CommunityToolkit.Maui.Sample.ViewModels.Views;
using CommunityToolkit.Maui.Sample.ViewModels.Views.AvatarView;

namespace CommunityToolkit.Maui.Sample;

public partial class TabAppShell : Shell
{
	static readonly IReadOnlyDictionary<Type, (Type GalleryPageType, Type ContentPageType)> viewModelMappings = new Dictionary<Type, (Type, Type)>(new[]
	{
		CreateViewModelMapping<MultipleBottomSheetPage, MultipleBottomSheetViewModel, ViewsGalleryPage, ViewsGalleryViewModel>()
	});

	public TabAppShell() => InitializeComponent();

	public static string GetPageRoute<TViewModel>() where TViewModel : BaseViewModel
	{
		return GetPageRoute(typeof(TViewModel));
	}

	public static string GetPageRoute(Type viewModelType)
	{
		if (!viewModelType.IsAssignableTo(typeof(BaseViewModel)))
		{
			throw new ArgumentException($"{nameof(viewModelType)} must implement {nameof(BaseViewModel)}", nameof(viewModelType));
		}

		if (!viewModelMappings.TryGetValue(viewModelType, out (Type GalleryPageType, Type ContentPageType) mapping))
		{
			throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings. Please register your ViewModel in {nameof(TabAppShell)}.{nameof(viewModelMappings)}");
		}

		var uri = new UriBuilder("", GetPageRoute(mapping.GalleryPageType, mapping.ContentPageType));
		return uri.Uri.OriginalString[..^1];
	}

	static string GetPageRoute(Type galleryPageType, Type contentPageType) => $"//{galleryPageType.Name}/{contentPageType.Name}";

	static KeyValuePair<Type, (Type GalleryPageType, Type ContentPageType)> CreateViewModelMapping<TPage, TViewModel, TGalleryPage, TGalleryViewModel>() where TPage : BasePage<TViewModel>
																																							where TViewModel : BaseViewModel
																																							where TGalleryPage : BaseGalleryPage<TGalleryViewModel>
																																							where TGalleryViewModel : BaseGalleryViewModel
	{
		return new KeyValuePair<Type, (Type GalleryPageType, Type ContentPageType)>(typeof(TViewModel), (typeof(TGalleryPage), typeof(TPage)));
	}
}