namespace CommunityToolkit.Maui.Core.Models;

/// <summary>
/// Represents the size of a BottomSheet.
/// </summary>
public class BottomSheetSize
{
	/// <summary>
	/// Creates a representation of the size of a BottomSheet
	/// </summary>
	/// <param name="contentSize">The size of the content</param>
	/// <param name="handleTapAreaHeight">The height of the BottomSheet's handle tap area</param>
	/// <param name="isCollapsed">This is the collapsed size</param>
	/// <param name="previous">The previous size, if many sizes chained</param>
	public BottomSheetSize(Size contentSize, double handleTapAreaHeight, bool isCollapsed, BottomSheetSize? previous)
	{
		ContentSize = contentSize;
		HandleTapAreaHeight = handleTapAreaHeight;
		IsCollapsed = isCollapsed;
		Previous = previous;
		if (previous != null)
		{
			previous.Next = this;
		}
	}

	/// <summary>
	/// The BottomSheet's content size
	/// </summary>
	public Size ContentSize { get; }

	/// <summary>
	/// The height the area around the handle that user can interact with
	/// </summary>
	public double HandleTapAreaHeight { get; }

	/// <summary>
	/// The BottomSheet's total size
	/// </summary>
	public Size TotalSize => new(ContentSize.Width, TotalHeight);

	/// <summary>
	/// The BottomSheet's total height
	/// </summary>
	public double TotalHeight => ContentSize.Height + HandleTapAreaHeight;

	/// <summary>
	/// If this size represents the collapsed state of the BottomSheet
	/// </summary>
	public bool IsCollapsed { get; }

	/// <summary>
	/// Ths previous (smaller) size
	/// </summary>
	public BottomSheetSize? Previous { get; }

	/// <summary>
	/// The next (bigger) size
	/// </summary>
	public BottomSheetSize? Next { get; private set; }

	/// <summary>
	/// Name to help debugging
	/// </summary>
	public string? Name { get; set; }
}