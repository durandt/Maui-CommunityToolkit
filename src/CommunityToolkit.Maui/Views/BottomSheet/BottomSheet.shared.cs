using CommunityToolkit.Maui.Core;
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
	///  Backing BindableProperty for the <see cref="Color"/> property.
	/// </summary>
	public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(BottomSheet), Colors.LightGray, propertyChanged: OnColorChanged);

	/// <summary>
	///  Backing BindableProperty for the <see cref="Size"/> property.
	/// </summary>
	public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(Size), typeof(BottomSheet), default(Size));

	/// <summary>
	///  Backing BindableProperty for the <see cref="CanBeDismissedByTappingOutsideOfBottomSheet"/> property.
	/// </summary>
	public static readonly BindableProperty CanBeDismissedByTappingOutsideOfBottomSheetProperty = BindableProperty.Create(nameof(CanBeDismissedByTappingOutsideOfBottomSheet), typeof(bool), typeof(BottomSheet), true);

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
	readonly Lazy<PlatformConfigurationRegistry<BottomSheet>> platformConfigurationRegistry;

	TaskCompletionSource<object?> taskCompletionSource = new();
	Window? window;

	/// <summary>
	/// Instantiates a new instance of <see cref="BottomSheet"/>.
	/// </summary>
	public BottomSheet()
	{
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
	/// Gets or sets the <see cref="Color"/> of the BottomSheet.
	/// </summary>
	/// <remarks>
	/// This color sets the native background color of the <see cref="BottomSheet"/>, which is
	/// independent of any background color configured in the actual View.
	/// </remarks>
	public Color Color
	{
		get => (Color)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
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
	internal virtual void OnOpened() =>
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
	protected void OnClosed(object? result, bool wasDismissedByTappingOutsideOfBottomSheet)
	{
		((IBottomSheet)this).OnClosed(result);
		dismissWeakEventManager.HandleEvent(this, new BottomSheetClosedEventArgs(result, wasDismissedByTappingOutsideOfBottomSheet), nameof(Closed));
	}

	/// <summary>
	/// Invoked when the bottom sheet is dismissed by tapping outside of the bottom sheet.
	/// </summary>
	protected internal virtual void OnDismissedByTappingOutsideOfBottomSheet()
	{
		taskCompletionSource.TrySetResult(ResultWhenUserTapsOutsideOfBottomSheet);
		OnClosed(ResultWhenUserTapsOutsideOfBottomSheet, true);
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

	static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
	{
		ArgumentNullException.ThrowIfNull(newValue);
	}

	void IBottomSheet.OnClosed(object? result) => Handler.Invoke(nameof(IBottomSheet.OnClosed), result);

	void IBottomSheet.OnOpened() => OnOpened();

	void IBottomSheet.OnDismissedByTappingOutsideOfBottomSheet() => OnDismissedByTappingOutsideOfBottomSheet();

	void IPropertyPropagationController.PropagatePropertyChanged(string propertyName) =>
		PropertyPropagationExtensions.PropagatePropertyChanged(propertyName, this, ((IVisualTreeElement)this).GetVisualChildren());

	IReadOnlyList<IVisualTreeElement> IVisualTreeElement.GetVisualChildren() =>
		Content is null
			? Enumerable.Empty<IVisualTreeElement>().ToList()
			: new List<IVisualTreeElement> { Content };
}