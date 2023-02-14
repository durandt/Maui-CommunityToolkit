using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui.Core.Models;
using CoreFoundation;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace CommunityToolkit.Maui.Core.Views;

/// <summary>
/// The native implementation of BottomSheet control.
/// </summary>
public class MauiBottomSheet : UIViewController
{
	readonly IMauiContext mauiContext;
	readonly IDeviceDisplay deviceDisplay;
	readonly List<UIViewPropertyAnimator> runningAnimations = new();
	nfloat animationProgressWhenInterrupted;
	double lastCompletedFraction;
	BottomSheetSize? currentBottomSheetSize;
	BottomSheetSize? targetBottomSheetSize;

	/// <summary>
	/// Constructor of <see cref="MauiBottomSheet"/>.
	/// </summary>
	/// <param name="mauiContext">An instance of <see cref="IMauiContext"/>.</param>
	/// <param name="deviceDisplay">An instance of <see cref="IDeviceDisplay"/>.</param>
	/// <exception cref="ArgumentNullException">If <paramref name="mauiContext"/> is null an exception will be thrown. </exception>
	public MauiBottomSheet(IMauiContext mauiContext, IDeviceDisplay deviceDisplay)
	{
		this.mauiContext = mauiContext ?? throw new ArgumentNullException(nameof(mauiContext));
		this.deviceDisplay = deviceDisplay;
	}

	/// <summary>
	/// An instance of the <see cref="PageHandler"/> that holds the <see cref="IBottomSheet.Content"/>.
	/// </summary>
	public PageHandler? Control { get; private set; }

	/// <summary>
	/// An instance of the <see cref="IBottomSheet"/>.
	/// </summary>
	public IBottomSheet? VirtualView { get; private set; }

	internal UIViewController? ViewController { get; private set; }

	internal MauiBottomSheetPopover? PopoverViewController { get; set; }

	internal bool BlockUserInteractionToSwitchBottomSheetSize { get; set; }

	double AnimationDuration
	{
		get
		{
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
			_ = VirtualView.AnimationDurationMillis ?? throw new InvalidOperationException($"{nameof(VirtualView.AnimationDurationMillis)} cannot be null.");
			return VirtualView.AnimationDurationMillis.Value / 1000.0;
		}
	}

	/// <summary>
	/// Method to initialize the native implementation.
	/// </summary>
	/// <param name="element">An instance of <see cref="IBottomSheet"/>.</param>
	[MemberNotNull(nameof(VirtualView), nameof(ViewController))]
	public void SetElement(IBottomSheet element)
	{
		if (element.Parent?.Handler is not PageHandler)
		{
			throw new InvalidOperationException($"The {nameof(element.Parent)} must be of type {typeof(PageHandler)}.");
		}

		VirtualView = element;
		ModalPresentationStyle = UIModalPresentationStyle.Popover;

		_ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null.");
		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");

		var rootViewController = WindowStateManager.Default.GetCurrentUIViewController() ?? throw new InvalidOperationException($"{nameof(PageHandler.ViewController)} cannot be null.");
		ViewController ??= rootViewController;
	}

	/// <summary>
	/// Method to CleanUp the resources of the <see cref="MauiBottomSheet"/>.
	/// </summary>
	public void CleanUp()
	{
		if (VirtualView is null)
		{
			return;
		}

		if (PopoverViewController != null)
		{
			PopoverViewController.VirtualView = null;
			PopoverViewController = null;
		}

		VirtualView = null;

		if (PresentationController is UIPopoverPresentationController presentationController)
		{
			presentationController.Delegate = null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="func"></param>
	/// <param name="virtualView"></param>
	/// <returns></returns>
	[MemberNotNull(nameof(Control), nameof(ViewController))]
	public void CreateControl(Func<IBottomSheet, PageHandler> func, in IBottomSheet virtualView)
	{
		Control = func(virtualView);

		SetPresentationController();

		_ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null.");
		SetView(View, Control);

		_ = ViewController ?? throw new InvalidOperationException($"{nameof(ViewController)} cannot be null.");
		AddToCurrentPageViewController(ViewController);

		this.SetSize(virtualView);
		this.SetLayout(virtualView);
	}

	void SetView(UIView view, PageHandler control)
	{
		_ = control.ViewController ?? throw new InvalidOperationException($"{nameof(control.ViewController)} cannot be null.");
		_ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null.");

		SetControlBackgroundColor(control, Colors.Transparent);
		var contentsViewController = control.ViewController;
		view.BackgroundColor = UIColor.Clear;
		view.Bounds = new(0, 0, deviceDisplay.MainDisplayInfo.Width, deviceDisplay.MainDisplayInfo.Height);

		control.PlatformView.BackgroundColor = UIColor.Clear;
		if (VirtualView is not null)
		{
			this.SetBackgroundColor(VirtualView);

			var popoverViewController = new MauiBottomSheetPopover();
			popoverViewController.VirtualView = VirtualView;
			popoverViewController.ContentsViewController = contentsViewController;
			var visualEffectView = new UIVisualEffectView();
			visualEffectView.Frame = View.Frame;
			View.AddSubview(visualEffectView);

			_ = popoverViewController.View ?? throw new InvalidOperationException($"{nameof(popoverViewController.View)} cannot be null.");
			View.AddSubview(popoverViewController.View);
			//popoverViewController.View.Bounds = new(0, 0, PreferredContentSize.Width, PreferredContentSize.Height);
			AddChildViewController(popoverViewController);

			var handleSwipeAreaHeight = (nfloat)(VirtualView.HandleSwipeAreaHeight ?? throw new InvalidOperationException($"{nameof(VirtualView.HandleSwipeAreaHeight)} cannot be null."));
			popoverViewController.View.Frame = new CGRect(x:0, y:View.Frame.Height - handleSwipeAreaHeight, width:View.Bounds.Width, height:View.Frame.Height);
			popoverViewController.View.ClipsToBounds = true;

			var panGestureRecognizer = new UIPanGestureRecognizer(HandlePopoverSlide);
			popoverViewController.View.AddGestureRecognizer(panGestureRecognizer);

			var backgroundPanGestureRecognizer = new UIPanGestureRecognizer(HandleBackgroundSlide);
			view.AddGestureRecognizer(backgroundPanGestureRecognizer);

			var backgroundTapGestureRecognizer = new UITapGestureRecognizer(recognizer =>
			{
				if (recognizer.State == UIGestureRecognizerState.Ended)
				{
					var point = recognizer.LocationInView(View);
					if (!popoverViewController.View.Frame.Contains(point))
					{
						if (!ModalInPresentation)
						{
							AnimateTransitionIfNeeded(VirtualView.BottomSheetSize, AnimationDuration, true);
						}
					}
				}
			});
			view.AddGestureRecognizer(backgroundTapGestureRecognizer);
			PopoverViewController = popoverViewController;
		}
	}

	void SetControlBackgroundColor(PageHandler control, Color? color)
	{
		var uiColor = color?.ToPlatform();
		control.PlatformView.BackgroundColor = uiColor;
		if (control.ViewController?.View is UIView view)
		{
			view.BackgroundColor = uiColor;
		}
	}

	void SetPresentationController()
	{
		var popOverDelegate = new PopoverDelegate();
		popOverDelegate.PopoverDismissedEvent += HandlePopoverDelegateDismissed;

		UIPopoverPresentationController presentationController = (UIPopoverPresentationController)(PresentationController ?? throw new InvalidOperationException($"{nameof(PresentationController)} cannot be null."));
		presentationController.SourceView = ViewController?.View ?? throw new InvalidOperationException($"{nameof(ViewController.View)} cannot be null.");

		presentationController.Delegate = popOverDelegate;
	}

	/// <inheritdoc/>
	public override void ViewDidAppear(bool animated)
	{
		base.ViewDidAppear(animated);
		if (animated)
		{
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");

			AnimateTransitionIfNeeded(VirtualView.BottomSheetSize, AnimationDuration);
		}
	}

	internal void SetBottomSheetSize(BottomSheetSize size)
	{
		if (PopoverViewController != null)
		{
			AnimateTransitionIfNeeded(size, AnimationDuration, size.IsCollapsed);
		}
	}

	void AnimateTransitionIfNeeded(BottomSheetSize size, double duration, bool collapsed = false, Action? onCompletion = null)
	{
		_ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null.");
		_ = PopoverViewController ?? throw new InvalidOperationException($"{nameof(PopoverViewController)} cannot be null.");
		_ = PopoverViewController.View ?? throw new InvalidOperationException($"{nameof(PopoverViewController.View)} cannot be null.");

		if (currentBottomSheetSize == null)
		{
			currentBottomSheetSize = new BottomSheetSize(new Size(View.Frame.Width, 0),
				VirtualView?.HandleSwipeAreaHeight ?? 32, true, null);
		}

		if (!runningAnimations.Any())
		{
			var frameAnimator = new UIViewPropertyAnimator(duration:duration, ratio:1, () =>
			{
				var newY = !collapsed ? View.Frame.Height - (nfloat)size.TotalHeight : View.Frame.Height;
				var frame = PopoverViewController.View.Frame;
				PopoverViewController.View.Frame = new CGRect(x:frame.X, y:newY, width:frame.Width, height:frame.Height);
			});
			frameAnimator.AddCompletion(_ =>
			{
				if (frameAnimator.Reversed)
				{
					runningAnimations.Clear();
					return;
				}

				if (collapsed)
				{
					DismissViewController(false, null);
				}

				runningAnimations.Remove(frameAnimator);
				onCompletion?.Invoke();
				if (VirtualView != null)
				{
					VirtualView.BottomSheetSize = size;
					currentBottomSheetSize = size;
				}
			});
			frameAnimator.StartAnimation();
			runningAnimations.Add(frameAnimator);

			var popoverBackgroundAnimator = new UIViewPropertyAnimator(duration: duration,
				UIViewAnimationCurve.Linear,
				() =>
				{
					View.BackgroundColor = !collapsed ? new UIColor(0, 0, 0, 0.15f) : UIColor.Clear;
				});
			popoverBackgroundAnimator.AddCompletion(_ =>
			{
				runningAnimations.Remove(popoverBackgroundAnimator);
			});
			popoverBackgroundAnimator.StartAnimation();
			runningAnimations.Add(popoverBackgroundAnimator);
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

	/// <summary>
	/// Close the <see cref="IBottomSheet"/>.
	/// </summary>
	/// <returns></returns>
	public Task CloseAsync()
	{
		var tcs = new TaskCompletionSource();
		if (VirtualView != null)
		{
			AnimateTransitionIfNeeded(VirtualView.BottomSheetSize, AnimationDuration, true, () =>
			{
				tcs.TrySetResult();
			});
		}
		else
		{
			tcs.TrySetResult();
		}

		return tcs.Task;
	}

	void HandleBackgroundSlide(UIPanGestureRecognizer recognizer)
	{
		if (BlockUserInteractionToSwitchBottomSheetSize)
		{
			return;
		}

		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
		_ = PopoverViewController ?? throw new InvalidOperationException($"{nameof(PopoverViewController)} cannot be null.");
		_ = PopoverViewController.View ?? throw new InvalidOperationException($"{nameof(PopoverViewController.View)} cannot be null.");
		switch (recognizer.State)
		{
			case UIGestureRecognizerState.Began:
				var velocity = recognizer.VelocityInView(View).Y / 60;
				// If background-swipe should only allow decrease, not increase
				//if (velocity < 0)
				//{
				//	targetBottomSheetSize = VirtualView.BottomSheetSize.Next;
				//}
				if (velocity < 0)
				{
					targetBottomSheetSize = VirtualView.BottomSheetSize.Next;
				}
				else
				{
					targetBottomSheetSize = VirtualView.BottomSheetSize.Previous;
				}

				break;
			default:
				break;
		}

		HandlePopoverSlideInternal(recognizer);
	}

	void HandlePopoverSlide(UIPanGestureRecognizer recognizer)
	{
		if (BlockUserInteractionToSwitchBottomSheetSize)
		{
			return;
		}

		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
		_ = PopoverViewController ?? throw new InvalidOperationException($"{nameof(PopoverViewController)} cannot be null.");
		_ = PopoverViewController.View ?? throw new InvalidOperationException($"{nameof(PopoverViewController.View)} cannot be null.");
		switch (recognizer.State)
		{
			case UIGestureRecognizerState.Began:
				var velocity = recognizer.VelocityInView(View).Y / 60;
				if (velocity < 0)
				{
					targetBottomSheetSize = VirtualView.BottomSheetSize.Next;
				}
				else
				{
					targetBottomSheetSize = VirtualView.BottomSheetSize.Previous;
				}
				break;
			default:
				break;
		}

		HandlePopoverSlideInternal(recognizer);
	}

    void HandlePopoverSlideInternal(UIPanGestureRecognizer recognizer)
    {
	    _ = PopoverViewController ?? throw new InvalidOperationException($"{nameof(PopoverViewController)} cannot be null.");
	    _ = PopoverViewController.View ?? throw new InvalidOperationException($"{nameof(PopoverViewController.View)} cannot be null.");
        switch (recognizer.State)
        {
	        case UIGestureRecognizerState.Began:
		        if (targetBottomSheetSize != null
		            && !targetBottomSheetSize.ContentSize.IsZero)
		        {
			        StartInteractiveTransition(targetBottomSheetSize, AnimationDuration);
		        }
		        break;
			case UIGestureRecognizerState.Changed:
				var translation = recognizer.TranslationInView(PopoverViewController.View);
				var translationY = -translation.Y; // > 0 is UP, < 0 is DOWN
				var startY = currentBottomSheetSize?.TotalHeight ?? deviceDisplay.MainDisplayInfo.Height;
				var endY = targetBottomSheetSize?.TotalHeight ?? 0.0;
				var distance =  endY - startY; // > 0 is UP, < 0 is DOWN
				var fractionComplete = distance != 0 ? translationY / distance : 0.0;
				lastCompletedFraction = fractionComplete;
				UpdateInteractiveTransition(fractionComplete);
				break;
	        case UIGestureRecognizerState.Ended:
		        var velocity = recognizer.VelocityInView(View).Y / 60;
		        var duration = CalculateAnimationTimeLeft(lastCompletedFraction);
		        DisableViewControllerGestures(duration);

		        // If running fast enough and in the right direction, complete started transition
		        if (Math.Abs(velocity) > 5 && lastCompletedFraction > 0)
		        {
			        ContinueInteractiveTransition();
			        return;
		        }

				// If view swiped halfway or less, reverse the started transition
	            if (lastCompletedFraction < 0.5)
		        {
			        runningAnimations.ForEach(a => a.Reversed = true);
				}
				ContinueInteractiveTransition();
		        break;
        default:
	        break;
        }
    }

    void DisableViewControllerGestures(double duration)
    {
	    if (PopoverViewController != null)
	    {
		    if (PopoverViewController.HandleSwipeAreaView?.GestureRecognizers?.FirstOrDefault() != null)
		    {
			    PopoverViewController.HandleSwipeAreaView.GestureRecognizers.First().Enabled = false;
		    }
		    if (PopoverViewController.View?.GestureRecognizers?.FirstOrDefault() != null)
		    {
			    PopoverViewController.View.GestureRecognizers.First().Enabled = false;
		    }

		    var when = new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(duration));
		    DispatchQueue.MainQueue.DispatchAfter(when, () =>
		    {
			    if (PopoverViewController.HandleSwipeAreaView?.GestureRecognizers?.FirstOrDefault() != null)
			    {
				    PopoverViewController.HandleSwipeAreaView.GestureRecognizers.First().Enabled = true;
			    }
			    if (PopoverViewController.View?.GestureRecognizers?.FirstOrDefault() != null)
			    {
				    PopoverViewController.View.GestureRecognizers.First().Enabled = true;
			    }
		    });
	    }
    }

    double CalculateAnimationTimeLeft(double fraction)
    {
	    double normalizedFraction;
	    if (fraction > 1)
	    {
		    normalizedFraction = 1;
	    }
	    else if (fraction < 0)
	    {
		    normalizedFraction = 0.001;
	    } 
	    else
	    {
		    normalizedFraction = fraction;
	    }

	    var fractionDuration = AnimationDuration * normalizedFraction;
	    var timeLeft = AnimationDuration - fractionDuration;
	    return timeLeft;
    }

    /// <summary>
    /// Pause the current transition toward the provided <see cref="BottomSheetSize"/>.
    /// If no current transition exists, start a new transition
    /// </summary>
    /// <param name="size">The transition's target <see cref="BottomSheetSize"/></param>
    /// <param name="duration">The animation duration</param>
    void StartInteractiveTransition(BottomSheetSize size, double duration)
    {
	    if (!runningAnimations.Any())
	    {
		    AnimateTransitionIfNeeded(size, duration:duration, collapsed:size.IsCollapsed);
	    }

	    foreach (var animator in runningAnimations)
	    {
		    animator.PauseAnimation();
		    animationProgressWhenInterrupted = animator.FractionComplete;
	    }
    }

    /// <summary>
    /// Sets the current transition animation progress, between 0.0 and 1.0.
    /// Setting the animation to a value greater or equal to 1.0 will complete
    /// the animation and trigger the animation completion callback.
    /// </summary>
    /// <param name="factionCompleted">The completion ratio of the animation</param>
    void UpdateInteractiveTransition(double factionCompleted)
    {
	    var fractionComplete = (float)Math.Min(factionCompleted + animationProgressWhenInterrupted, 0.99f);
	    foreach (var animator in runningAnimations)
	    {
		    animator.FractionComplete = fractionComplete;
	    }
    }

    /// <summary>
    /// Resume the paused transition
    /// </summary>
    void ContinueInteractiveTransition()
    {
	    foreach (var animator in runningAnimations)
	    {
		    animator.ContinueAnimation(null, 0);
	    }
    }

	[MemberNotNull(nameof(VirtualView))]
	void HandlePopoverDelegateDismissed(object? sender, UIPresentationController e)
	{
		_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
		VirtualView.Handler?.Invoke(nameof(IBottomSheet.OnDismissedByTappingOutsideOfBottomSheet));
	}

	void AddToCurrentPageViewController(UIViewController viewController)
	{
		viewController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
		viewController.PresentViewController(this, false, null);
	}

	sealed class PopoverDelegate : UIPopoverPresentationControllerDelegate
	{
		readonly WeakEventManager popoverDismissedEventManager = new();

		public event EventHandler<UIPresentationController> PopoverDismissedEvent
		{
			add => popoverDismissedEventManager.AddEventHandler(value);
			remove => popoverDismissedEventManager.RemoveEventHandler(value);
		}

		public override UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController) =>
			UIModalPresentationStyle.OverFullScreen;

		public override void DidDismiss(UIPresentationController presentationController) =>
			popoverDismissedEventManager.HandleEvent(this, presentationController, nameof(PopoverDismissedEvent));
	}

	internal sealed class MauiBottomSheetPopover : UIViewController
	{
		public IBottomSheet? VirtualView { get; set; }

		public UIViewController? ContentsViewController { get; set; }

		UIView? HandleView { get; set; }
		internal UIView? HandleSwipeAreaView { get; set; }

		public UIColor? BackgroundColor
		{
			set
			{
				if (ContentsViewController?.View != null)
				{
					ContentsViewController.View.BackgroundColor = value;
				}
			}
		}

		UIView CreateHandleView()
		{
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");

			var handleView = new UIView();
			handleView.TranslatesAutoresizingMaskIntoConstraints = false;
			handleView.Layer.CornerRadius = (nfloat)(VirtualView.HandleHeight / 2.0 ?? throw new InvalidOperationException($"{nameof(VirtualView.HandleHeight)} cannot be null."));
			handleView.BackgroundColor = (VirtualView.HandleColor ?? throw new InvalidOperationException($"{nameof(VirtualView.HandleColor)} cannot be null."))?.ToPlatform();

			return handleView;
		}

		UIView CreateHandleSwipeAreaView()
		{
			var handleSwipeAreaView = new UIView();
			handleSwipeAreaView.TranslatesAutoresizingMaskIntoConstraints = false;
			handleSwipeAreaView.BackgroundColor = UIColor.Clear;

			return handleSwipeAreaView;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			SetupViews();
			SetupAutoLayout();
		}

		void SetupViews()
		{
			HandleView = CreateHandleView();
			HandleSwipeAreaView = CreateHandleSwipeAreaView();

			if (ContentsViewController == null)
			{
				return;
			}
			if (ContentsViewController.View is null)
			{
				throw new NullReferenceException($"{nameof(ContentsViewController.View)} cannot be null");
			}
			AddChildViewController(ContentsViewController);
			
			if (View is null)
			{
				throw new NullReferenceException($"{nameof(View)} cannot be null");
			}
			View.AddSubview(ContentsViewController.View);

			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");

			ContentsViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentsViewController.DidMoveToParentViewController(this);

			View.AddSubview(HandleSwipeAreaView);
			HandleSwipeAreaView.AddSubview(HandleView);

			ContentsViewController.View.BackgroundColor = (VirtualView.BackgroundColor ?? throw new InvalidOperationException($"{nameof(VirtualView.BackgroundColor)} cannot be null.")).ToPlatform();
			ContentsViewController.View.Layer.CornerRadius = (nfloat)(VirtualView.CornerRadius ?? throw new InvalidOperationException($"{nameof(VirtualView.CornerRadius)} cannot be null."));
			// TODO parameterize Shadow
			ContentsViewController.View.Layer.ShadowColor = UIColor.Black.CGColor;
			ContentsViewController.View.Layer.ShadowOffset = new CGSize(width:0f, height:4f);
			ContentsViewController.View.Layer.ShadowOpacity = 0.15f;
			ContentsViewController.View.Layer.ShadowRadius = 8f;
			ContentsViewController.View.Layer.LayoutSublayers();
		}

		void SetupAutoLayout()
		{
			if (ContentsViewController == null)
			{
				return;
			}

			_ = ContentsViewController.View ?? throw new InvalidOperationException($"{nameof(ContentsViewController.View)} cannot be null.");
			_ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");
			_ = HandleView ?? throw new InvalidOperationException($"{nameof(HandleView)} cannot be null.");
			_ = HandleSwipeAreaView ?? throw new InvalidOperationException($"{nameof(HandleSwipeAreaView)} cannot be null.");
			var handleSwipeAreaHeight = (nfloat)(VirtualView.HandleSwipeAreaHeight ?? throw new InvalidOperationException($"{nameof(VirtualView.HandleSwipeAreaHeight)} cannot be null."));
			var handleHeight = (nfloat)(VirtualView.HandleHeight ?? throw new InvalidOperationException($"{nameof(VirtualView.HandleHeight)} cannot be null."));
			var handleWidth = (nfloat)(VirtualView.HandleWidth ?? throw new InvalidOperationException($"{nameof(VirtualView.HandleWidth)} cannot be null."));
			var handleSpacingToBottomSheet = (nfloat)(VirtualView.HandleSpacingToBottomSheet ?? throw new InvalidOperationException($"{nameof(VirtualView.HandleSpacingToBottomSheet)} cannot be null."));

			NSLayoutConstraint.ActivateConstraints(new[]
			{
				ContentsViewController.View.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
				ContentsViewController.View.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
				ContentsViewController.View.TopAnchor.ConstraintEqualTo(HandleSwipeAreaView.BottomAnchor),
				ContentsViewController.View.BottomAnchor.ConstraintEqualTo(View.BottomAnchor)
			});
			NSLayoutConstraint.ActivateConstraints(new[]
			{
				HandleSwipeAreaView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
				HandleSwipeAreaView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
				HandleSwipeAreaView.TopAnchor.ConstraintEqualTo(View.TopAnchor),
				HandleSwipeAreaView.HeightAnchor.ConstraintEqualTo(handleSwipeAreaHeight)
			});
			NSLayoutConstraint.ActivateConstraints(new[]
			{
				HandleView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
				HandleView.BottomAnchor.ConstraintEqualTo(HandleSwipeAreaView.BottomAnchor, constant: -handleSpacingToBottomSheet),
				HandleView.HeightAnchor.ConstraintEqualTo(handleHeight),
				HandleView.WidthAnchor.ConstraintEqualTo(handleWidth)
			});
		}
	}
}