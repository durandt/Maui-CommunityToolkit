using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Handlers;

namespace CommunityToolkit.Maui.Views;

public partial class BottomSheet
{
	/// <summary>
	/// 
	/// </summary>
	public static CommandMapper<IBottomSheet, BottomSheetHandler> ControlBottomSheetCommandMapper = new(BottomSheetHandler.BottomSheetCommandMapper)
	{
#if IOS || MACCATALYST
		[nameof(IBottomSheet.OnOpened)] = MapOnOpened
#endif
	};

	internal static void RemapForControls()
	{
		BottomSheetHandler.BottomSheetCommandMapper = ControlBottomSheetCommandMapper;
	}
}