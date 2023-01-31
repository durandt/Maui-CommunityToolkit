using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace CommunityToolkit.Maui.Core.Views;

/// <summary>
/// The native implementation of BottomSheet control.
/// </summary>
public class MauiBottomSheet : Dialog, IDialogInterfaceOnCancelListener
{
	readonly IMauiContext mauiContext;

	/// <summary>
	/// Constructor of <see cref="MauiBottomSheet"/>.
	/// </summary>
	/// <param name="context">An instance of <see cref="Context"/>.</param>
	/// <param name="mauiContext">An instance of <see cref="IMauiContext"/>.</param>
	/// <exception cref="ArgumentNullException">If <paramref name="mauiContext"/> is null an exception will be thrown. </exception>
	public MauiBottomSheet(Context context, IMauiContext mauiContext)
		: base(context)
	{
		this.mauiContext = mauiContext ?? throw new ArgumentNullException(nameof(mauiContext));
	}

	/// <summary>
	/// An instance of the <see cref="IBottomSheet"/>.
	/// </summary>
	public IBottomSheet? VirtualView { get; private set; }

	/// <summary>
	/// Method to initialize the native implementation.
	/// </summary>
	/// <param name="element">An instance of <see cref="IBottomSheet"/>.</param>
	public AView? SetElement(IBottomSheet? element)
	{
		ArgumentNullException.ThrowIfNull(element);

		VirtualView = element;

		if (TryCreateContainer(VirtualView, out var container))
		{
			SubscribeEvents();
		}

		return container;
	}

	/// <summary>
	/// Method to show the BottomSheet.
	/// </summary>
	public override void Show()
	{
		base.Show();

		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null");

		VirtualView.OnOpened();
	}

	/// <summary>
	/// Method triggered when the BottomSheet is dismissed by tapping outside of the BottomSheet.
	/// </summary>
	/// <param name="dialog">An instance of the <see cref="IDialogInterface"/>.</param>
	public void OnDismissedByTappingOutsideOfBottomSheet(IDialogInterface dialog)
	{
		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null");
		_ = VirtualView.Handler ?? throw new InvalidOperationException($"{nameof(VirtualView.Handler)} cannot be null");

		VirtualView.Handler.Invoke(nameof(IBottomSheet.OnDismissedByTappingOutsideOfBottomSheet));
	}

	/// <summary>
	/// Method to CleanUp the resources of the <see cref="MauiBottomSheet"/>.
	/// </summary>
	public void CleanUp()
	{
		VirtualView = null;
	}

	bool TryCreateContainer(in IBottomSheet bottomSheet, [NotNullWhen(true)] out AView? container)
	{
		container = null;

		if (bottomSheet.Content is null)
		{
			return false;
		}

		container = bottomSheet.Content.ToPlatform(mauiContext);
		SetContentView(container);

		return true;
	}

	void SubscribeEvents()
	{
		SetOnCancelListener(this);
	}

	void IDialogInterfaceOnCancelListener.OnCancel(IDialogInterface? dialog) => OnDismissedByTappingOutsideOfBottomSheet(this);
}