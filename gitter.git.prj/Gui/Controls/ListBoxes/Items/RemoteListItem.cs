﻿namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary><see cref="gitter.Framework.Controls.CustomListBoxItem"/> representing <see cref="gitter.Git.Remote"/>.</summary>
	public sealed class RemoteListItem : CustomListBoxItem<Remote>
	{
		private static readonly Bitmap ImgRemote = CachedResources.Bitmaps["ImgRemote"];

		#region Comparers

		public static int CompareByName(RemoteListItem item1, RemoteListItem item2)
		{
			var data1 = item1.Data.Name;
			var data2 = item2.Data.Name;
			return string.Compare(data1, data2);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as RemoteListItem;
			if(i1 == null) return 0;
			var i2 = item2 as RemoteListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByName(i1, i2);
			}
			catch
			{
				return 0;
			}
		}

		public static int CompareByFetchUrl(RemoteListItem item1, RemoteListItem item2)
		{
			var data1 = item1.Data.FetchUrl;
			var data2 = item2.Data.FetchUrl;
			return string.Compare(data1, data2);
		}

		public static int CompareByFetchUrl(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as RemoteListItem;
			if(i1 == null) return 0;
			var i2 = item2 as RemoteListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByFetchUrl(i1, i2);
			}
			catch
			{
				return 0;
			}
		}

		public static int CompareByPushUrl(RemoteListItem item1, RemoteListItem item2)
		{
			var data1 = item1.Data.PushUrl;
			var data2 = item2.Data.PushUrl;
			return string.Compare(data1, data2);
		}

		public static int CompareByPushUrl(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as RemoteListItem;
			if(i1 == null) return 0;
			var i2 = item2 as RemoteListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByPushUrl(i1, i2);
			}
			catch
			{
				return 0;
			}
		}

		#endregion

		#region .ctor

		/// <summary>Create <see cref="RemoteListItem"/>.</summary>
		/// <param name="remote">Related <see cref="Remote"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="remote"/> == <c>null</c>.</exception>
		public RemoteListItem(Remote remote)
			: base(remote)
		{
			if(remote == null) throw new ArgumentNullException("remote");
		}

		#endregion

		protected override void OnListBoxAttached()
		{
			Data.Deleted += OnRemoteDeleted;
			Data.Renamed += OnRenamed;
			base.OnListBoxAttached();
		}

		protected override void OnListBoxDetached()
		{
			Data.Deleted -= OnRemoteDeleted;
			Data.Renamed -= OnRenamed;
			base.OnListBoxDetached();
		}

		private void OnRemoteDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		private void OnRenamed(object sender, EventArgs e)
		{
			if(EnsureSortOrderSafe())
			{
				InvalidateSubItemSafe((int)ColumnId.Name);
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgRemote, Data.Name);
				case ColumnId.Url:
				case ColumnId.FetchUrl:
					return measureEventArgs.MeasureText(Data.FetchUrl);
				case ColumnId.PushUrl:
					return measureEventArgs.MeasureText(Data.PushUrl);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgRemote, Data.Name);
					break;
				case ColumnId.Url:
				case ColumnId.FetchUrl:
					paintEventArgs.PaintText(Data.FetchUrl);
					break;
				case ColumnId.PushUrl:
					paintEventArgs.PaintText(Data.PushUrl);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var mnu = new RemoteMenu(Data);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}
	}
}