using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Handlers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace CommunityToolkit.Maui.Views;

public partial class BottomSheet
{
	/// <summary>
	/// Action that's triggered when the BottomSheet is Opened.
	/// </summary>
	/// <param name="handler">An instance of <see cref="BottomSheetHandler"/>.</param>
	/// <param name="view">An instance of <see cref="IBottomSheet"/>.</param>
	/// <param name="result">We don't need to provide the result parameter here.</param>
	public static void MapOnOpened(BottomSheetHandler handler, IBottomSheet view, object? result)
	{
		handler.PlatformView?.CreateControl(CreatePageHandler, view);
		view.OnOpened();


		static PageHandler CreatePageHandler(IBottomSheet virtualView)
		{
			var mauiContext = virtualView.Handler?.MauiContext ?? throw new NullReferenceException(nameof(IMauiContext));
			var view = (View?)virtualView.Content ?? throw new InvalidOperationException($"{nameof(IBottomSheet.Content)} can't be null here.");
			var contentPage = new ContentPage
			{
				Parent = virtualView.Parent as Element,
				Content = view
			};

			contentPage.SetBinding(BindingContextProperty, new Binding { Source = virtualView, Path = BindingContextProperty.PropertyName });

			return (PageHandler)contentPage.ToHandler(mauiContext);
		}
	}
}