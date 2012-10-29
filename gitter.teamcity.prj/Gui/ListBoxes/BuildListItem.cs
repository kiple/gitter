﻿namespace gitter.TeamCity.Gui
{
	using System;
	using System.Globalization;
	using System.Drawing;

	using gitter.Framework.Controls;

	sealed class BuildListItem : CustomListBoxItem<Build>
	{
		public BuildListItem(Build buildType)
			: base(buildType)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();

			DataContext.PropertyChanged += OnBuildPropertyChanged;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.PropertyChanged -= OnBuildPropertyChanged;

			base.OnListBoxDetached();
		}

		private Image StatusImage
		{
			get
			{
				switch(DataContext.Status)
				{
					case BuildStatus.Success:
						return CachedResources.Bitmaps["ImgStatusSuccess"];
					case BuildStatus.Failure:
						return CachedResources.Bitmaps["ImgStatusFailure"];
					case BuildStatus.Error:
						return CachedResources.Bitmaps["ImgStatusError"];
					default:
						return null;
				}
			}
		}

		private void OnBuildPropertyChanged(object sender, TeamCityObjectPropertyChangedEventArgs e)
		{
			if(e.Property == Build.IdProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Id);
			}
			else if(e.Property == Build.StatusProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Status);
			}
			else if(e.Property == Build.BuildTypeProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.BuildType);
			}
			else if(e.Property == Build.StartDateProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.StartDate);
			}
			else if(e.Property == Build.NumberProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Number);
			}
			else if(e.Property == Build.WebUrlProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.WebUrl);
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Id:
					return measureEventArgs.MeasureText(DataContext.Id);
				case ColumnId.Status:
					return measureEventArgs.MeasureImageAndText(StatusImage, DataContext.Status.ToString());
				case ColumnId.Number:
					return measureEventArgs.MeasureText(DataContext.Number);
				case ColumnId.StartDate:
					return DateColumn.OnMeasureSubItem(measureEventArgs, DataContext.StartDate);
				case ColumnId.WebUrl:
					return measureEventArgs.MeasureText(DataContext.WebUrl);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Id:
					paintEventArgs.PaintText(DataContext.Id.ToString(CultureInfo.InvariantCulture));
					break;
				case ColumnId.Status:
					paintEventArgs.PaintImageAndText(StatusImage, DataContext.Status.ToString());
					break;
				case ColumnId.Number:
					paintEventArgs.PaintText(DataContext.Number);
					break;
				case ColumnId.StartDate:
					DateColumn.OnPaintSubItem(paintEventArgs, DataContext.StartDate);
					break;
				case ColumnId.WebUrl:
					paintEventArgs.PaintText(DataContext.WebUrl);
					break;
			}
		}
	}
}
