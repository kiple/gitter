﻿namespace gitter.Git.Gui.Views
{
	partial class DiffView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_source != null)
				{
					_source.Updated -= OnSourceUpdated;
					_source.Dispose();
				}
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._diffViewer = new gitter.Git.Gui.Controls.DiffViewer();
			this.SuspendLayout();
			// 
			// _diffViewer
			// 
			this._diffViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._diffViewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._diffViewer.Location = new System.Drawing.Point(0, 0);
			this._diffViewer.Name = "_diffViewer";
			this._diffViewer.Size = new System.Drawing.Size(555, 362);
			this._diffViewer.TabIndex = 0;
			this._diffViewer.Text = "diffViewer1";
			// 
			// DiffTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._diffViewer);
			this.Name = "DiffTool";
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.DiffViewer _diffViewer;
	}
}