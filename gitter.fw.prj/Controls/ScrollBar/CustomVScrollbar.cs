﻿namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public class CustomVScrollbar : CustomScrollBar
	{
		#region Data

		private Rectangle _decreaseButtonBounds;
		private Rectangle _decreaseTrackBarBounds;
		private Rectangle _thumbBounds;
		private Rectangle _increaseTrackBarBounds;
		private Rectangle _increaseButtonBounds;
		private int _initialScrollY;
		private bool _isArranged;

		#endregion

		#region .ctor

		public CustomVScrollbar()
		{
			Width = SystemInformation.VerticalScrollBarWidth;
		}

		#endregion

		#region Methods

		private void Arrange()
		{
			const int MinThumbSize = 17;

			var size = Size;
			if(size.Width <= 0 || size.Height <= 0) return;
			var buttonHeight	= SystemInformation.VerticalScrollBarArrowHeight;
			var physicalRange	= Maximum - Minimum;
			var trackBarSize	= size.Height - buttonHeight * 2;
			int thumbSize;
			int thumbOffset;
			if(physicalRange <= LargeChange)
			{
				thumbSize = 0;
				thumbOffset = 0;
			}
			else
			{
				thumbSize = trackBarSize * LargeChange / physicalRange;
				if(thumbSize < MinThumbSize) thumbSize = MinThumbSize;
				var freeTrackBarSize = trackBarSize - thumbSize;
				thumbOffset = freeTrackBarSize * ClampValue(Value) / (physicalRange - LargeChange + 1);
			}

			_decreaseButtonBounds	= new Rectangle(0, 0, size.Width, buttonHeight);
			_decreaseTrackBarBounds	= new Rectangle(0, buttonHeight, size.Width, thumbOffset);
			_thumbBounds			= new Rectangle(0, buttonHeight + thumbOffset, size.Width, thumbSize);
			_increaseTrackBarBounds	= new Rectangle(0, buttonHeight + thumbOffset + thumbSize, size.Width, trackBarSize - thumbSize - thumbOffset);
			_increaseButtonBounds	= new Rectangle(0, size.Height - buttonHeight, size.Width, buttonHeight);
			_isArranged = true;
		}

		#endregion

		#region Event Handlers

		private void OnScrollHereClick(object sender, EventArgs e)
		{
		}

		private void OnTopClick(object sender, EventArgs e)
		{
			Value = ClampValue(Minimum);
		}

		private void OnBottomClick(object sender, EventArgs e)
		{
			Value = ClampValue(Maximum);
		}

		private void OnScrollUpClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value - SmallChange);
		}

		private void OnScrollDownClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value + SmallChange);
		}

		private void OnPageUpClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value - LargeChange);
		}

		private void OnPageDownClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value + LargeChange);
		}

		#endregion

		#region CustomScrollBar Overrides

		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			switch(e.Button)
			{
				case MouseButtons.Right:
					var menu = new ContextMenuStrip();
					menu.Items.Add(new ToolStripMenuItem(Resources.StrScrollHere, null, OnScrollHereClick));
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(new ToolStripMenuItem(Resources.StrTop, null, OnTopClick));
					menu.Items.Add(new ToolStripMenuItem(Resources.StrBottom, null, OnBottomClick));
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(new ToolStripMenuItem(Resources.StrPageUp, null, OnPageUpClick));
					menu.Items.Add(new ToolStripMenuItem(Resources.StrPageDown, null, OnPageDownClick));
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(new ToolStripMenuItem(Resources.StrScrollUp, null, OnScrollUpClick));
					menu.Items.Add(new ToolStripMenuItem(Resources.StrScrollDown, null, OnScrollDownClick));
					Utility.MarkDropDownForAutoDispose(menu);
					menu.Show(this, e.X, e.Y);
					break;
			}
			base.OnMouseDown(e);
		}

		protected override Rectangle DecreaseButtonBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _decreaseButtonBounds;
			}
		}

		protected override Rectangle DecreaseTrackBarBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _decreaseTrackBarBounds;
			}
		}

		protected override Rectangle ThumbBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _thumbBounds;
			}
		}

		protected override Rectangle IncreaseTrackBarBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _increaseTrackBarBounds;
			}
		}

		protected override Rectangle IncreaseButtonBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _increaseButtonBounds;
			}
		}

		protected override void ArrangeInvalidate()
		{
			_isArranged = false;
		}

		protected override void BeginScroll(Point from)
		{
			if(!_isArranged)
			{
				Arrange();
			}
			_initialScrollY = _thumbBounds.Y;
		}

		protected override bool PerformScroll(Point from, Point to)
		{
			var dy = to.Y - from.Y;
			if(dy == 0) return false;
			int y = _initialScrollY + dy;
			int maxY = Height - (_decreaseButtonBounds.Height + _thumbBounds.Height);
			if(y > maxY)
			{
				y = maxY;
			}
			if(y < _decreaseButtonBounds.Height)
			{
				y = _decreaseButtonBounds.Height;
			}
			if(_thumbBounds.Y != y)
			{
				dy = y - _thumbBounds.Y;
				_thumbBounds.Y = y;
				_decreaseTrackBarBounds.Height += dy;
				_increaseTrackBarBounds.Y += dy;
				_increaseTrackBarBounds.Height -= dy;
				return true;
			}
			else
			{
				return false;
			}
		}

		protected override bool EndScroll(Point from, Point to)
		{
			return PerformScroll(from, to);
		}

		protected override int ThumbPositionToValue()
		{
			int visualRange = _decreaseTrackBarBounds.Height + _increaseTrackBarBounds.Height;
			int visualPosition = _decreaseTrackBarBounds.Height;
			int physicalRange = Maximum - Minimum - LargeChange + 1;
			return physicalRange * visualPosition / visualRange;
		}

		#endregion
	}
}