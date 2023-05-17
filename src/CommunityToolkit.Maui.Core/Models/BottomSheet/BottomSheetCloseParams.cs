namespace CommunityToolkit.Maui.Core.Models;

/// <summary>
/// 
/// </summary>
public class BottomSheetCloseParams
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="result"></param>
	/// <param name="onCompletion"></param>
	public BottomSheetCloseParams(object? result, Action? onCompletion)
	{
		Result = result;
		OnCompletion = onCompletion;
	}

	/// <summary>
	/// 
	/// </summary>
	public object? Result { get; }

	/// <summary>
	/// 
	/// </summary>
	public Action? OnCompletion { get; }
}