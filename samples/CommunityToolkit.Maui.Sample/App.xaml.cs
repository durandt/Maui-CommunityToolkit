using Application = Microsoft.Maui.Controls.Application;

namespace CommunityToolkit.Maui.Sample;

public partial class App : Application
{
	readonly TabAppShell appShell;

	public App(TabAppShell appShell)
	{
		InitializeComponent();

		this.appShell = appShell;
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(appShell);
}