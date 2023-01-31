using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;

namespace CommunityToolkit.Maui.Views;

/// <summary>
/// Extension methods for <see cref="BottomSheet"/>.
/// </summary>
public static partial class BottomSheetExtensions
{
	/// <summary>
	/// Displays a bottom sheet.
	/// </summary>
	/// <param name="page">
	/// The current <see cref="Page"/>.
	/// </param>
	/// <param name="bottomSheet">
	/// The <see cref="BottomSheet"/> to display.
	/// </param>
	public static void ShowBottomSheet<TBottomSheet>(this Page page, TBottomSheet bottomSheet) where TBottomSheet : BottomSheet
	{
		if (page.IsPlatformEnabled)
		{
			CreateAndShowBottomSheet(page, bottomSheet);
		}
		else
		{
			void handler(object? sender, NavigatedToEventArgs args)
			{
				page.NavigatedTo -= handler;

				CreateAndShowBottomSheet(page, bottomSheet);
			}

			page.NavigatedTo += handler;
		}
	}

	/// <summary>
	/// Displays a bottom sheet and returns a result.
	/// </summary>
	/// <param name="page">
	/// The current <see cref="Page"/>.
	/// </param>
	/// <param name="bottomSheet">
	/// The <see cref="BottomSheet"/> to display.
	/// </param>
	/// <returns>
	/// A task that will complete once the <see cref="BottomSheet"/> is dismissed.
	/// </returns>
	public static Task<object?> ShowBottomSheetAsync<TBottomSheet>(this Page page, TBottomSheet bottomSheet) where TBottomSheet : BottomSheet
	{
		if (page.IsPlatformEnabled)
		{
			return CreateAndShowBottomSheetAsync(page, bottomSheet);
		}
		else
		{
			var taskCompletionSource = new TaskCompletionSource<object?>();

			async void handler(object? sender, NavigatedToEventArgs args)
			{
				page.NavigatedTo -= handler;

				try
				{
					var result = await CreateAndShowBottomSheetAsync(page, bottomSheet);

					taskCompletionSource.TrySetResult(result);
				}
				catch (Exception ex)
				{
					taskCompletionSource.TrySetException(ex);
				}
			}

			page.NavigatedTo += handler;

			return taskCompletionSource.Task;
		}
	}

	static void CreateBottomSheet(Page page, BottomSheet bottomSheet)
	{
		var mauiContext = GetMauiContext(page);
		bottomSheet.Parent = PageExtensions.GetCurrentPage(page);
		var platformBottomSheet = bottomSheet.ToHandler(mauiContext);
		platformBottomSheet.Invoke(nameof(IBottomSheet.OnOpened));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static IMauiContext GetMauiContext(Page page)
	{
		return page.Handler?.MauiContext ?? throw new InvalidOperationException("Could not locate MauiContext.");
	}

	static void CreateAndShowBottomSheet<TBottomSheet>(Page page, TBottomSheet bottomSheet) where TBottomSheet : BottomSheet
	{
#if WINDOWS
		PlatformShowBottomSheet(bottomSheet, GetMauiContext(page));
#else
		CreateBottomSheet(page, bottomSheet);
#endif
	}

	static Task<object?> CreateAndShowBottomSheetAsync<TBottomSheet>(this Page page, TBottomSheet bottomSheet) where TBottomSheet : BottomSheet
	{
#if WINDOWS
		return PlatformShowBottomSheetAsync(bottomSheet, GetMauiContext(page));
#else
		CreateBottomSheet(page, bottomSheet);

		return bottomSheet.Result;
#endif
	}
}