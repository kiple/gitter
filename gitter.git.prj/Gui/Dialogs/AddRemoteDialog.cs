﻿namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class AddRemoteDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Repository _repository;

		public AddRemoteDialog(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			InitializeComponent();

			SetupReferenceNameInputBox(_txtName, ReferenceType.Remote);

			Text = Resources.StrAddRemote;

			_lblName.Text			= Resources.StrName.AddColon();
			_lblUrl.Text			= Resources.StrUrl.AddColon();
			
			_grpOptions.Text		= Resources.StrOptions;
			_chkFetch.Text			= Resources.StrFetchRemote;
			_chkMirror.Text			= Resources.StrMirror;

			_grpBranches.Text		= Resources.StrTrackingBranches;
			_trackAllBranches.Text	= Resources.StrlTrackAllBranches;
			_trackSpecified.Text	= Resources.StrlTrackSpecifiedBranches.AddColon();

			_grpTagImport.Text		= Resources.StrTagFetchMode;
			_tagFetchDefault.Text	= Resources.StrDefault;
			_tagFetchAll.Text		= Resources.StrFetchAll;
			_tagFetchNone.Text		= Resources.StrFetchNone;

			if(_repository.Remotes.Count == 0)
			{
				_txtName.Text = GitConstants.DefaultRemoteName;
			}

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtUrl);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public string RemoteName
		{
			get { return _txtName.Text; }
			set { _txtName.Text = value; }
		}

		public string Url
		{
			get { return _txtUrl.Text; }
			set { _txtUrl.Text = value; }
		}

		public TagFetchMode TagFetchMode
		{
			get
			{
				if(_tagFetchAll.Checked)
				{
					return TagFetchMode.AllTags;
				}
				if(_tagFetchNone.Checked)
				{
					return TagFetchMode.NoTags;
				}
				return TagFetchMode.Default;
			}
			set
			{
				switch(value)
				{
					case TagFetchMode.AllTags:
						_tagFetchAll.Checked = true;
						break;
					case TagFetchMode.NoTags:
						_tagFetchNone.Checked = true;
						break;
					case TagFetchMode.Default:
						_tagFetchDefault.Checked = true;
						break;
					default:
						throw new ArgumentException("value");
				}
			}
		}

		public bool Execute()
		{
			var name = _txtName.Text.Trim();
			if(!ValidateNewRemoteName(name, _txtName, _repository))
			{
				return false;
			}
			var url = _txtUrl.Text.Trim();
			if(!ValidateUrl(url, _txtUrl))
			{
				return false;
			}
			bool fetch = _chkFetch.Checked;
			bool mirror = _chkMirror.Checked;
			var tagFetchMode = TagFetchMode;
			Cursor = Cursors.WaitCursor;
			try
			{
				_repository.Remotes.AddRemote(name, url, fetch, mirror, tagFetchMode);
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToAddRemote,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}