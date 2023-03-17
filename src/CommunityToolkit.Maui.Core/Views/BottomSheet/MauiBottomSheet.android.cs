using System.Diagnostics.CodeAnalysis;
using Android.Animation;
using Android.Content;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using CommunityToolkit.Maui.Core.Models;
using Google.Android.Material.BottomSheet;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Primitives;
using AView = Android.Views.View;

namespace CommunityToolkit.Maui.Core.Views;

/// <summary>
/// The native implementation of BottomSheet control.
/// </summary>
public class MauiBottomSheet : BottomSheetDialog, IDialogInterfaceOnCancelListener, IDialogInterfaceOnShowListener
{
	readonly IMauiContext mauiContext;
	AView? platformViewContainer { get; set; }
	BottomSheetSize? currentBottomSheetSize;
	readonly List<ValueAnimator> runningAnimations = new();

	int AnimationDuration
	{
		get
		{
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
			_ = VirtualView.AnimationDurationMillis ?? throw new InvalidOperationException($"{nameof(VirtualView.AnimationDurationMillis)} cannot be null.");
			return VirtualView.AnimationDurationMillis.Value;
		}
	}

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

	/// <inheritdoc/>
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		SetOnShowListener(this);
	}

	/// <inheritdoc/>
	public void OnShow(IDialogInterface? dialog)
	{
		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
		VirtualView.OnAppeared();
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

		platformViewContainer = container;
		return container;
	}

	/// <summary>
	/// Method to show the BottomSheet.
	/// </summary>
	public override void Show()
	{
		Behavior.State = BottomSheetBehavior.StateExpanded;
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

	internal void SetBottomSheetSize(BottomSheetSize size)
	{
		AnimateTransitionIfNeeded(size, AnimationDuration, size.IsCollapsed);
	}

	/// <summary>
	/// Method to update the <see cref="IBottomSheet.Size"/> property.
	/// </summary>
	/// <param name="container">The native representation of <see cref="IBottomSheet.Content"/>.</param>
	/// <exception cref="NullReferenceException">if the <see cref="Android.Views.Window"/> is null an exception will be thrown. If the <paramref name="container"/> is null an exception will be thrown.</exception>
	public void SetSize(in AView container)
	{
		ArgumentNullException.ThrowIfNull(VirtualView);
		ArgumentNullException.ThrowIfNull(VirtualView.Content);

		var window = GetWindow(this);

		var decorView = (ViewGroup)window.DecorView;
		var child = decorView.GetChildAt(0) ?? throw new NullReferenceException();

		int realWidth = 0,
			realHeight = 0,
			realContentWidth = 0,
			realContentHeight = 0;

		CalculateSizes(VirtualView, Context, ref realWidth, ref realHeight, ref realContentWidth, ref realContentHeight);

		var childLayoutParams = (FrameLayout.LayoutParams)(child.LayoutParameters ?? throw new NullReferenceException());
		_ = VirtualView.DeviceDisplay ?? throw new NullReferenceException(nameof(VirtualView.DeviceDisplay));
		childLayoutParams.Width = (int)VirtualView.DeviceDisplay.MainDisplayInfo.Width;
		childLayoutParams.Height = (int)VirtualView.DeviceDisplay.MainDisplayInfo.Height;
		child.LayoutParameters = childLayoutParams;

		var containerLayoutParams = new FrameLayout.LayoutParams(realWidth, realHeight);
		containerLayoutParams.Gravity = GravityFlags.Bottom;

		switch (VirtualView.Content.HorizontalLayoutAlignment)
		{
			case LayoutAlignment.Start:
				containerLayoutParams.Gravity |= GravityFlags.Left;
				break;
			case LayoutAlignment.Center:
			case LayoutAlignment.Fill:
				containerLayoutParams.Gravity |= GravityFlags.FillHorizontal;
				containerLayoutParams.Width = realWidth;
				break;
			case LayoutAlignment.End:
				containerLayoutParams.Gravity |= GravityFlags.Right;
				break;
			default:
				throw new NotSupportedException();
		}

		AnimateTransitionIfNeeded(VirtualView.BottomSheetSize, AnimationDuration,
			VirtualView.BottomSheetSize.IsCollapsed);

		//AViewExtensions.DebugView(window.DecorView.RootView!);
		
		//EdgeToEdgeUtils.ApplyEdgeToEdge(window, true);

		static void CalculateSizes(IBottomSheet bottomSheet, Context context, ref int realWidth, ref int realHeight, ref int realContentWidth, ref int realContentHeight)
		{
			ArgumentNullException.ThrowIfNull(bottomSheet.Content);

			if (!bottomSheet.Size.IsZero)
			{
				realWidth = (int)context.ToPixels(bottomSheet.Size.Width);
				realHeight = (int)context.ToPixels(bottomSheet.Size.Height);
			}
			if (double.IsNaN(bottomSheet.Content.Width) || double.IsNaN(bottomSheet.Content.Height))
			{
				var size = bottomSheet.Content.Measure(double.PositiveInfinity, double.PositiveInfinity);
				realContentWidth = (int)context.ToPixels(size.Width);
				realContentHeight = (int)context.ToPixels(size.Height);
			}
			else
			{
				realContentWidth = (int)context.ToPixels(bottomSheet.Content.Width);
				realContentHeight = (int)context.ToPixels(bottomSheet.Content.Height);
			}

			realWidth = realWidth is 0 ? realContentWidth : realWidth;
			realHeight = realHeight is 0 ? realContentHeight : realHeight;

			if (realHeight is 0 || realWidth is 0)
			{
				realWidth = (int?)(context.Resources?.DisplayMetrics?.WidthPixels * 0.8) ?? throw new NullReferenceException();
				realHeight = (int?)(context.Resources?.DisplayMetrics?.HeightPixels * 0.6) ?? throw new NullReferenceException();
			}
		}
	}

	void AnimateTransitionIfNeeded(BottomSheetSize size, int duration, bool collapsed = false,
		Action? onCompletion = null)
	{
		if (collapsed || size.ContentSize.Height <= 0)
		{
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
			VirtualView.OnClosed();
			return;
		}

		var container = platformViewContainer ?? throw new InvalidOperationException($"{nameof(platformViewContainer)} cannot be null.");

		if (size == currentBottomSheetSize)
		{
			return;
		}

		AView? viewToAnimate;
		if (container.Parent is ViewGroup && container.Parent.Parent is CoordinatorLayout)
		{
			var parent = (ViewGroup)container.Parent;
			viewToAnimate = parent;
		}
		else
		{
			// Fallback
			viewToAnimate = container;
		}

		if (currentBottomSheetSize == null)
		{
			// First size, don't animate as native BottomSheetDialog will animate
			var layoutParams = viewToAnimate.LayoutParameters;
			if (layoutParams != null)
			{
				layoutParams.Height = (int)Context.ToPixels(size.ContentSize.Height);
				viewToAnimate.LayoutParameters = layoutParams;
			}
			currentBottomSheetSize = size;
			return;
		}

		if (!runningAnimations.Any())
		{
			
			
			var animation = ValueAnimator.OfInt(viewToAnimate.MeasuredHeight, (int)Context.ToPixels(size.ContentSize.Height)) ?? throw new InvalidOperationException($"animation cannot be null.");
			var animationListener = new AnimatorListener(viewToAnimate,
				completionAction: _ =>
				{
					runningAnimations.Remove(animation);
					onCompletion?.Invoke();
					if (VirtualView != null)
					{
						VirtualView.BottomSheetSize = size;
						currentBottomSheetSize = size;
					}
				},
				cancelAction: _ =>
				{
					runningAnimations.Clear();
				});
			animation.AddUpdateListener(animationListener);
			animation.AddListener(animationListener);
			animation.SetDuration(duration);
			animation.Start(); 
			runningAnimations.Add(animation);
		}
	}

	/// <summary>
	/// Close the <see cref="IBottomSheet"/>.
	/// </summary>
	/// <returns></returns>
	public void Close()
	{
		if (VirtualView != null)
		{
			AnimateTransitionIfNeeded(VirtualView.BottomSheetSize, AnimationDuration, true);
		}
	}

	static Window GetWindow(in Dialog dialog) =>
		dialog.Window ?? throw new NullReferenceException("Android.Views.Window is null.");

	class AnimatorListener : Java.Lang.Object, Animator.IAnimatorListener, ValueAnimator.IAnimatorUpdateListener
	{
		View ViewToAnimate { get; }
		Action<Animator>? CompletionAction { get; }
		Action<Animator>? CancelAction { get; }

		public AnimatorListener(AView viewToAnimate,
			Action<Animator>? completionAction = null,
			Action<Animator>? cancelAction = null)
		{
			ViewToAnimate = viewToAnimate;
			CompletionAction = completionAction;
			CancelAction = cancelAction;
		}

		public void OnAnimationUpdate(ValueAnimator valueAnimator)
		{
			var val = (int)(valueAnimator.AnimatedValue ?? 0);
			var layoutParams = ViewToAnimate.LayoutParameters;
			if (layoutParams != null)
			{
				layoutParams!.Height = val;
				ViewToAnimate.LayoutParameters = layoutParams;
			}
		}

		public void OnAnimationCancel(Animator animation)
		{
			CancelAction?.Invoke(animation);
		}

		public void OnAnimationEnd(Animator animation)
		{
			CompletionAction?.Invoke(animation);
		}

		public void OnAnimationRepeat(Animator animation)
		{
		}

		public void OnAnimationStart(Animator animation)
		{
		}
	}
}