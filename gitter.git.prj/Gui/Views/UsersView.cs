﻿namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	partial class UsersView : GitViewBase
	{
		public UsersView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.UsersViewGuid, parameters, gui)
		{
			InitializeComponent();

			Text = Resources.StrUsers;

			_lstUsers.Text = Resources.StrsNoUsersToDisplay;
			_lstUsers.PreviewKeyDown += OnKeyDown;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgUser"]; }
		}

		protected override void AttachToRepository(Repository repository)
		{
			_lstUsers.Load(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstUsers.Load(null);
		}

		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			else
			{
				if(Repository != null)
				{
					Cursor = Cursors.WaitCursor;
					Repository.Users.Refresh();
					Cursor = Cursors.Default;
				}
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F5:
					RefreshContent();
					e.IsInputKey = true;
					break;
			}
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("UsersList");
			_lstUsers.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("UsersList");
			if(listNode != null)
				_lstUsers.LoadViewFrom(listNode);
		}
	}
}