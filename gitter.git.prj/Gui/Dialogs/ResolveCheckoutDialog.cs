﻿namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class ResolveCheckoutDialog : GitDialogBase
	{
		private Branch _selectedBranch;
		private bool _checkoutCommit = true;

		public ResolveCheckoutDialog()
		{
			InitializeComponent();

			Text = Resources.StrCheckout;
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Cancel; }
		}

		public void SetAvailableBranches(IEnumerable<Branch> branches)
		{
			if(branches == null) throw new ArgumentNullException("branches");
			_references.BeginUpdate();
			_references.Items.Clear();
			Branch first = null;
			foreach(var b in branches)
			{
				if(first == null) first = b;
				_references.Items.Add(new BranchListItem(b));
			}
			if(first != null)
			{
				_selectedBranch = first;
				UpdateButton();
			}
			_references.EndUpdate();
			if(_references.Items.Count <= 1)
			{
				_references.Visible = false;
				_lblSelectOther.Visible = false;
				Height -= _references.Height + _lblSelectOther.Height;
			}
		}

		public bool CheckoutCommit
		{
			get { return _checkoutCommit; }
		}

		public Branch SelectedBranch
		{
			get { return _selectedBranch; }
		}

		private void UpdateButton()
		{
			_btnCheckoutBranch.Text = string.Format("{0} '{1}'", Resources.StrCheckout, _selectedBranch.Name);
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var b = ((BranchListItem)e.Item).Data;
			_selectedBranch = b;
			UpdateButton();
		}

		private void _btnCheckoutCommit_Click(object sender, EventArgs e)
		{
			_checkoutCommit = true;
			GlobalBehavior.AskOnCommitCheckouts = !_chkDontShowAgain.Checked;
			ClickOk();
		}

		private void _btnCheckoutBranch_Click(object sender, EventArgs e)
		{
			_checkoutCommit = false;
			GlobalBehavior.AskOnCommitCheckouts = !_chkDontShowAgain.Checked;
			ClickOk();
		}
	}
}