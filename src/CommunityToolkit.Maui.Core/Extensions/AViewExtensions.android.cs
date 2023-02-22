using Android.Graphics.Drawables;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.Shape;
using AView = Android.Views.View;

namespace CommunityToolkit.Maui.Core.Extensions;

static class AViewExtensions
{
	internal static void DebugView(this AView view, string prefix = "")
	{
		if (view is ViewGroup)
		{
			var gr = (ViewGroup)view;
			Console.WriteLine($"{prefix}GROUP type={gr.GetType().Name} " +
			                  $"layout={LayoutParametersToString(gr.LayoutParameters)} " +
			                  $"width={gr.Width} " +
			                  $"height={gr.Height} " +
			                  $"color={DrawableToString(gr.Background)}");
			for (var i = 0; i < gr.ChildCount; i++)
			{
				var c = gr.GetChildAt(i);
				if (c != null)
				{
					DebugView(c, "  " + prefix);
				}
			}
		}
		else
		{
			Console.WriteLine($"{prefix}CHILD type={view.GetType().Name} " +
			                  $"layout={LayoutParametersToString(view.LayoutParameters)} " +
			                  $"width={view.Width} " +
			                  $"height={view.Height} " +
			                  $"color={DrawableToString(view.Background)}");
		}
	}

	static string LayoutParametersToString(ViewGroup.LayoutParams? layoutParams)
	{
		return "";
		if (layoutParams is WindowManagerLayoutParams)
		{
			var wmlp = (WindowManagerLayoutParams)layoutParams;
			return $"WindowManagerLayoutParams[Gravity={GravityToString(wmlp.Gravity)}]";
		}

		if (layoutParams is FrameLayout.LayoutParams)
		{
			var fllp = (FrameLayout.LayoutParams)layoutParams;
			return $"FrameLayout.LayoutParams[Gravity={GravityToString(fllp.Gravity)}]";
		}

		if (layoutParams is LinearLayout.LayoutParams)
		{
			var lllp = (LinearLayout.LayoutParams)layoutParams;
			return $"LinearLayout.LayoutParams[Gravity={GravityToString(lllp.Gravity)}]";
		}

		if (layoutParams is CoordinatorLayout.LayoutParams)
		{
			var cllp = (CoordinatorLayout.LayoutParams)layoutParams;
			return $"CoordinatorLayout.LayoutParams[Gravity={GravityToString((GravityFlags)cllp.Gravity)}]";
		}

		return layoutParams?.GetType().FullName ?? "";
	}

	static string GravityToString(GravityFlags gravityFlags)
	{
		var res = Enum.GetValues<GravityFlags>()
			.Where(v => gravityFlags.HasFlag(v))
			.Select(v => v.ToString())
			.Where(s => !s.StartsWith("Axis"));
		return string.Join("|", res);
	}

	static string DrawableToString(Drawable? drawable)
	{
		if (drawable is ColorDrawable)
		{
			var color = (ColorDrawable)drawable;
			return color.Color.ToString();
		}

		if (drawable is MaterialShapeDrawable)
		{
			var msd = (MaterialShapeDrawable)drawable;
			return $"MaterialShapeDrawable[FillColor={msd.FillColor}, StrokeColor={msd.StrokeColor}]";
		}

		return drawable?.GetType().FullName ?? "";
	}
}