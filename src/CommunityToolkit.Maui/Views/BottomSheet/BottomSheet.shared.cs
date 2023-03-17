using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Models;
using Microsoft.Maui.Controls.Internals;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace CommunityToolkit.Maui.Views;

/// <summary>
/// Represents a small View that pops up at front the Page. Implements <see cref="IBottomSheet"/>.
/// </summary>
[ContentProperty(nameof(Content))]
public partial class BottomSheet : Element, IBottomSheet, IWindowController, IPropertyPropagationController
{
	/// <summary>
	///  Backing BindableProperty for the <see cref="Content"/> property.
	/// </summary>
	public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(BottomSheet), propertyChanged: OnContentChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="BackgroundColor"/> property.
	/// </summary>
	public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(BottomSheet), Colors.LightGray, propertyChanged: OnBackgroundColorChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="CornerRadius"/> property.
	/// </summary>
	public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(double?), typeof(BottomSheet), 28.0, propertyChanged: OnCornerRadiusChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="HandleColor"/> property.
	/// </summary>
	public static readonly BindableProperty HandleColorProperty = BindableProperty.Create(nameof(HandleColor), typeof(Color), typeof(BottomSheet), Colors.LightGray, propertyChanged: OnHandleColorChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="HandleHeight"/> property.
	/// </summary>
	public static readonly BindableProperty HandleHeightProperty = BindableProperty.Create(nameof(HandleHeight), typeof(double?), typeof(BottomSheet), 4.0, propertyChanged: OnHandleHeightChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="HandleWidth"/> property.
	/// </summary>
	public static readonly BindableProperty HandleWidthProperty = BindableProperty.Create(nameof(HandleWidth), typeof(double?), typeof(BottomSheet), 32.0, propertyChanged: OnHandleWidthChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="HandleSpacingToBottomSheet"/> property.
	/// </summary>
	public static readonly BindableProperty HandleSpacingToBottomSheetProperty = BindableProperty.Create(nameof(HandleSpacingToBottomSheet), typeof(double?), typeof(BottomSheet), 8.0, propertyChanged: OnHandleSpacingToBottomSheetChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="HandleSwipeAreaHeight"/> property.
	/// </summary>
	public static readonly BindableProperty HandleSwipeAreaHeightProperty = BindableProperty.Create(nameof(HandleSwipeAreaHeight), typeof(double?), typeof(BottomSheet), 32.0, propertyChanged: OnHandleSwipeAreaHeightChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="AnimationDurationMillis"/> property.
	/// </summary>
	public static readonly BindableProperty AnimationDurationMillisProperty = BindableProperty.Create(nameof(AnimationDurationMillis), typeof(int?), typeof(BottomSheet), 300, propertyChanged: OnAnimationDurationMillisChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="Size"/> property.
	/// </summary>
	public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(Size), typeof(BottomSheet), default(Size));

	/// <summary>
	///  Backing BindableProperty for the <see cref="BottomSheetSize"/> property.
	/// </summary>
	public static readonly BindableProperty BottomSheetSizeProperty = BindableProperty.Create(nameof(BottomSheetSize), typeof(BottomSheetSize), typeof(BottomSheet), default(BottomSheetSize));

	/// <summary>
	///  Backing BindableProperty for the <see cref="CanBeDismissedByTappingOutsideOfBottomSheet"/> property.
	/// </summary>
	public static readonly BindableProperty CanBeDismissedByTappingOutsideOfBottomSheetProperty = BindableProperty.Create(nameof(CanBeDismissedByTappingOutsideOfBottomSheet), typeof(bool), typeof(BottomSheet), true);

	/// <summary>
	///  Backing BindableProperty for the <see cref="SwipeWillDismissBottomSheet"/> property.
	/// </summary>
	public static readonly BindableProperty SwipeWillDismissBottomSheetProperty = BindableProperty.Create(nameof(SwipeWillDismissBottomSheet), typeof(bool), typeof(BottomSheet), true);

	/// <summary>
	///  Backing BindableProperty for the <see cref="AllowUserInteractionToSwitchBottomSheetSize"/> property.
	/// </summary>
	public static readonly BindableProperty AllowUserInteractionToSwitchBottomSheetSizeProperty = BindableProperty.Create(nameof(AllowUserInteractionToSwitchBottomSheetSize), typeof(bool), typeof(BottomSheet), true);

	/// <summary>
	///  Backing BindableProperty for the <see cref="VerticalOptions"/> property.
	/// </summary>
	public static readonly BindableProperty VerticalOptionsProperty = BindableProperty.Create(nameof(VerticalOptions), typeof(LayoutAlignment), typeof(BottomSheet), LayoutAlignment.Center);

	/// <summary>
	///  Backing BindableProperty for the <see cref="HorizontalOptions"/> property.
	/// </summary>
	public static readonly BindableProperty HorizontalOptionsProperty = BindableProperty.Create(nameof(HorizontalOptions), typeof(LayoutAlignment), typeof(BottomSheet), LayoutAlignment.Center);

	readonly WeakEventManager dismissWeakEventManager = new();
	readonly WeakEventManager openedWeakEventManager = new();
	readonly WeakEventManager appearedWeakEventManager = new();
	readonly WeakEventManager disappearingWeakEventManager = new();
	readonly Lazy<PlatformConfigurationRegistry<BottomSheet>> platformConfigurationRegistry;

	TaskCompletionSource<object?> taskCompletionSource = new();
	Window? window;

	/// <summary>
	/// Instantiates a new instance of <see cref="BottomSheet"/>.
	/// </summary>
	public BottomSheet(IDeviceDisplay deviceDisplay)
	{
		DeviceDisplay = deviceDisplay;
		platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<BottomSheet>>(() => new(this));

		VerticalOptions = HorizontalOptions = LayoutAlignment.Center;
#if WINDOWS
		this.HandlerChanged += OnBottomSheetHandlerChanged;
#endif
	}

	/// <summary>
	/// Dismissed event is invoked when the bottom sheet is closed.
	/// </summary>
	public event EventHandler<BottomSheetClosedEventArgs> Closed
	{
		add => dismissWeakEventManager.AddEventHandler(value);
		remove => dismissWeakEventManager.RemoveEventHandler(value);
	}

	/// <summary>
	/// Opened event is invoked when the bottom sheet is opened.
	/// </summary>
	public event EventHandler<BottomSheetOpenedEventArgs> Opened
	{
		add => openedWeakEventManager.AddEventHandler(value);
		remove => openedWeakEventManager.RemoveEventHandler(value);
	}

	/// <inheritdoc/>
	public event EventHandler<EventArgs> Appeared
	{
		add => appearedWeakEventManager.AddEventHandler(value);
		remove => appearedWeakEventManager.RemoveEventHandler(value);
	}

	/// <inheritdoc/>
	public event EventHandler<EventArgs> Disappearing
	{
		add => disappearingWeakEventManager.AddEventHandler(value);
		remove => disappearingWeakEventManager.RemoveEventHandler(value);
	}

	/// <summary>
	/// Gets the final result of the dismissed bottom sheet.
	/// </summary>
	public Task<object?> Result => taskCompletionSource.Task;

	/// <summary>
	/// Gets or sets the <see cref="View"/> content to render in the BottomSheet.
	/// </summary>
	/// <remarks>
	/// The View can be or type: <see cref="View"/>, <see cref="ContentPage"/> or <see cref="NavigationPage"/>
	/// </remarks>
	public virtual View? Content
	{
		get => (View?)GetValue(ContentProperty);
		set => SetValue(ContentProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="BackgroundColor"/> of the BottomSheet.
	/// </summary>
	/// <remarks>
	/// This color sets the native background color of the <see cref="BottomSheet"/>, which is
	/// independent of any background color configured in the actual View.
	/// </remarks>
	public Color BackgroundColor
	{
		get => (Color)GetValue(BackgroundColorProperty);
		set => SetValue(BackgroundColorProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="CornerRadius"/> of the BottomSheet.
	/// </summary>
	/// <remarks>
	/// Gets or sets the <see cref="BottomSheet"/>'s corner radius.
	/// </remarks>
	public double? CornerRadius
	{
		get => (double?)GetValue(CornerRadiusProperty);
		set => SetValue(CornerRadiusProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="HandleColor"/> of the BottomSheet.
	/// </summary>
	/// <remarks>
	/// This color sets the native background color of handle above the
	/// <see cref="BottomSheet"/>.
	/// </remarks>
	public Color HandleColor
	{
		get => (Color)GetValue(HandleColorProperty);
		set => SetValue(HandleColorProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="HandleHeight"/> of the BottomSheet's handle.
	/// </summary>
	/// <remarks>
	/// Gets or sets the <see cref="BottomSheet"/> handle's height.
	/// </remarks>
	public double? HandleHeight
	{
		get => (double?)GetValue(HandleHeightProperty);
		set => SetValue(HandleHeightProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="HandleWidth"/> of the BottomSheet's handle.
	/// </summary>
	/// <remarks>
	/// Gets or sets the <see cref="BottomSheet"/> handle's width.
	/// </remarks>
	public double? HandleWidth
	{
		get => (double?)GetValue(HandleWidthProperty);
		set => SetValue(HandleWidthProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="HandleSpacingToBottomSheet"/>, the spacing between
	/// the BottomSheet and its handle.
	/// </summary>
	/// <remarks>
	/// Gets or sets the spacing between the <see cref="BottomSheet"/> and its handle.
	/// </remarks>
	public double? HandleSpacingToBottomSheet
	{
		get => (double?)GetValue(HandleSpacingToBottomSheetProperty);
		set => SetValue(HandleSpacingToBottomSheetProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="HandleSwipeAreaHeight"/> of the swipeable area
	/// around the BottomSheet's handle.
	/// </summary>
	/// <remarks>
	/// Gets or sets the height of the area that allows swiping around the
	/// <see cref="BottomSheet"/>'s handle's.
	/// </remarks>
	public double? HandleSwipeAreaHeight
	{
		get => (double?)GetValue(HandleSwipeAreaHeightProperty);
		set => SetValue(HandleSwipeAreaHeightProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="AnimationDurationMillis"/> of the BottomSheet.
	/// </summary>
	/// <remarks>
	/// Gets or sets the <see cref="BottomSheet"/> animations durations.
	/// </remarks>
	public int? AnimationDurationMillis
	{
		get => (int?)GetValue(AnimationDurationMillisProperty);
		set => SetValue(AnimationDurationMillisProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="LayoutOptions"/> for positioning the <see cref="BottomSheet"/> vertically on the screen.
	/// </summary>
	public LayoutAlignment VerticalOptions
	{
		get => (LayoutAlignment)GetValue(VerticalOptionsProperty);
		set => SetValue(VerticalOptionsProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="LayoutOptions"/> for positioning the <see cref="BottomSheet"/> horizontally on the screen.
	/// </summary>
	public LayoutAlignment HorizontalOptions
	{
		get => (LayoutAlignment)GetValue(HorizontalOptionsProperty);
		set => SetValue(HorizontalOptionsProperty, value);
	}

	/// <summary>
	/// Gets or sets the <see cref="BottomSheetSize"/> of the BottomSheet Display.
	/// </summary>
	/// <remarks>
	/// The BottomSheet will always try to constrain the actual size of the <see cref="BottomSheet" />
	/// to the <see cref="BottomSheet" /> of the View unless a <see cref="BottomSheetSize"/> is specified.
	/// If the <see cref="BottomSheet" /> contains <see cref="LayoutOptions"/> a <see cref="BottomSheetSize"/>
	/// will be required. This will allow the View to have a concept of <see cref="BottomSheetSize"/>
	/// that varies from the actual <see cref="BottomSheetSize"/> of the <see cref="BottomSheet" />
	/// </remarks>
	public BottomSheetSize BottomSheetSize
	{
		get => (BottomSheetSize)GetValue(BottomSheetSizeProperty);
		set {
			SetValue(BottomSheetSizeProperty, value);
			if (value != null)
			{
				Size = value.ContentSize;
			}
		}
	}

	/// <summary>
	/// Gets or sets the <see cref="Size"/> of the BottomSheet Display.
	/// </summary>
	/// <remarks>
	/// The BottomSheet will always try to constrain the actual size of the <see cref="BottomSheet" />
	/// to the <see cref="BottomSheet" /> of the View unless a <see cref="Size"/> is specified.
	/// If the <see cref="BottomSheet" /> contains <see cref="LayoutOptions"/> a <see cref="Size"/>
	/// will be required. This will allow the View to have a concept of <see cref="Size"/>
	/// that varies from the actual <see cref="Size"/> of the <see cref="BottomSheet" />
	/// </remarks>
	public Size Size
	{
		get => (Size)GetValue(SizeProperty);
		set => SetValue(SizeProperty, value);
	}

	/// <summary>
	/// Gets or sets a value indicating whether the bottom sheet can be dismissed by tapping outside of the BottomSheet.
	/// </summary>
	/// <remarks>
	/// When true and the user taps outside of the bottom sheet it will dismiss.
	/// On Android - when false the hardware back button is disabled.
	/// </remarks>
	public bool CanBeDismissedByTappingOutsideOfBottomSheet
	{
		get => (bool)GetValue(CanBeDismissedByTappingOutsideOfBottomSheetProperty);
		set => SetValue(CanBeDismissedByTappingOutsideOfBottomSheetProperty, value);
	}

	/// <summary>
	/// Gets or sets a value indicating whether swiping down the bottom sheet will dismiss it.
	/// </summary>
	/// <remarks>
	/// When true and the user swipes the bottom sheet down it will dismiss.
	/// On Android - when false the hardware back button is disabled.
	/// </remarks>
	public bool SwipeWillDismissBottomSheet
	{
		get => (bool)GetValue(SwipeWillDismissBottomSheetProperty);
		set => SetValue(SwipeWillDismissBottomSheetProperty, value);
	}

	/// <summary>
	/// Gets or sets a value indicating whether the use can change the bottom sheet size by interacting with it.
	/// </summary>
	/// <remarks>
	/// When true and the user swipes the view up or down, the bottom sheet will animate to a bigger or smaller size.
	/// </remarks>
	public bool AllowUserInteractionToSwitchBottomSheetSize
	{
		get => (bool)GetValue(AllowUserInteractionToSwitchBottomSheetSizeProperty);
		set => SetValue(AllowUserInteractionToSwitchBottomSheetSizeProperty, value);
	}

	/// <summary>
	/// Gets the instance of the <see cref="IDeviceDisplay"/>.
	/// </summary>
	public IDeviceDisplay DeviceDisplay { get; }

	/// <summary>
	/// Gets or sets the <see cref="View"/> anchor.
	/// </summary>
	/// <remarks>
	/// The Anchor is where the BottomSheet will render closest to. When an Anchor is configured
	/// the bottom sheet will appear centered over that control or as close as possible.
	/// </remarks>
	public View? Anchor { get; set; }

	/// <summary>
	/// Property that represents the Window that's showing the BottomSheet.
	/// </summary>
	public Window? Window
	{
		get => window;
		set
		{
			window = value;

			if (Content is IWindowController controller)
			{
				controller.Window = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the result that will return when user taps outside of the BottomSheet.
	/// </summary>
	protected object? ResultWhenUserTapsOutsideOfBottomSheet { get; set; }

	/// <inheritdoc/>
	IView? IBottomSheet.Anchor => Anchor;

	/// <inheritdoc/>
	IView? IBottomSheet.Content => Content;


	/// <summary>
	/// Resets the BottomSheet.
	/// </summary>
	public void Reset() => taskCompletionSource = new();

	/// <summary>
	/// Close the current bottom sheet.
	/// </summary>
	/// <param name="result">
	/// The result to return.
	/// </param>
	public void Close(object? result = null)
	{
		taskCompletionSource.TrySetResult(result);
		OnClosed(result, false);
	}

	/// <summary>
	/// Invokes the <see cref="Opened"/> event.
	/// </summary>
	public virtual void OnOpened() =>
		openedWeakEventManager.HandleEvent(this, BottomSheetOpenedEventArgs.Empty, nameof(Opened));

	/// <summary>
	/// Invokes the <see cref="Closed"/> event.
	/// </summary>
	/// <param name="result">
	/// Sets the <see cref="BottomSheetClosedEventArgs"/> Property of <see cref="BottomSheetClosedEventArgs.Result"/>.
	/// </param>
	/// /// <param name="wasDismissedByTappingOutsideOfBottomSheet">
	/// Sets the <see cref="BottomSheetClosedEventArgs"/> Property of <see cref="BottomSheetClosedEventArgs.WasDismissedByTappingOutsideOfBottomSheet"/>/>.
	/// </param>
	public virtual void OnClosed(object? result, bool wasDismissedByTappingOutsideOfBottomSheet)
	{
		((IBottomSheet)this).OnClosed(result);
		dismissWeakEventManager.HandleEvent(this, new BottomSheetClosedEventArgs(result, wasDismissedByTappingOutsideOfBottomSheet), nameof(Closed));
	}

	/// <summary>
	/// Invokes the <see cref="Appeared"/> event.
	/// </summary>
	public virtual void OnAppeared() =>
		appearedWeakEventManager.HandleEvent(this, EventArgs.Empty, nameof(Appeared));

	/// <summary>
	/// Invokes the <see cref="Disappearing"/> event.
	/// </summary>
	public virtual void OnDisappearing() =>
		disappearingWeakEventManager.HandleEvent(this, EventArgs.Empty, nameof(Disappearing));

	/// <summary>
	/// Invoked when the bottom sheet is dismissed by tapping outside of the bottom sheet.
	/// </summary>
	protected internal virtual void OnDismissedByTappingOutsideOfBottomSheet()
	{
		taskCompletionSource.TrySetResult(ResultWhenUserTapsOutsideOfBottomSheet);
		OnClosed(ResultWhenUserTapsOutsideOfBottomSheet, true);
	}

	/// <summary>
	/// Invoked when the bottom sheet is dismissed by swiping down.
	/// </summary>
	protected internal virtual void OnDismissedBySwipingDown()
	{
		taskCompletionSource.TrySetResult(ResultWhenUserTapsOutsideOfBottomSheet);
	}

	/// <summary>
	///<inheritdoc/>
	/// </summary>
	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();

		if (Content is not null)
		{
			SetInheritedBindingContext(Content, BindingContext);
			Content.Parent = this;
		}
	}

	static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var bottomSheet = (BottomSheet)bindable;
		bottomSheet.OnBindingContextChanged();
	}

	static void OnBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	static void OnCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	static void OnHandleColorChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	static void OnHandleHeightChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	static void OnHandleWidthChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	static void OnHandleSpacingToBottomSheetChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	static void OnHandleSwipeAreaHeightChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	static void OnAnimationDurationMillisChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	void IBottomSheet.OnClosed(object? result) => Handler.Invoke(nameof(IBottomSheet.OnClosed), result);

	void IBottomSheet.OnOpened() => OnOpened();

	void IBottomSheet.OnDismissedByTappingOutsideOfBottomSheet() => OnDismissedByTappingOutsideOfBottomSheet();

	void IBottomSheet.OnDismissedBySwipingDown() => OnDismissedBySwipingDown();

	void IPropertyPropagationController.PropagatePropertyChanged(string propertyName) =>
		PropertyPropagationExtensions.PropagatePropertyChanged(propertyName, this, ((IVisualTreeElement)this).GetVisualChildren());

	IReadOnlyList<IVisualTreeElement> IVisualTreeElement.GetVisualChildren() =>
		Content is null
			? Enumerable.Empty<IVisualTreeElement>().ToList()
			: new List<IVisualTreeElement> { Content };
}