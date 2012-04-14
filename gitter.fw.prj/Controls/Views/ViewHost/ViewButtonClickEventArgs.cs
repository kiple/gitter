﻿namespace gitter.Framework.Controls
{
	using System;

	internal sealed class ViewButtonClickEventArgs : EventArgs
	{
		private readonly ViewButtonType _button;

		public ViewButtonClickEventArgs(ViewButtonType button)
		{
			_button = button;
		}

		public ViewButtonType Button
		{
			get { return _button; }
		}
	}
}