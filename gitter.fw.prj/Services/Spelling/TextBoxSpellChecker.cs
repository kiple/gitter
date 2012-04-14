﻿namespace gitter.Framework.Services
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Collections.Generic;
	using System.Text;
	using System.Windows.Forms;

	/// <summary>Highlights all spelling errors in specified <see cref="TextBox"/>.</summary>
	public sealed class TextBoxSpellChecker : NativeWindow, IDisposable
	{
		#region Data

		private readonly Dictionary<string, bool> _cache;
		private readonly TextBox _textBox;
		private readonly List<Substring> _errors;
		private bool _enabled;
		private Bitmap _bitmap;
		private Graphics _textBoxGraphics;  
		private Graphics _bufferGraphics;  

		#endregion

		/// <summary>Create <see cref="TextBoxSpellChecker"/>.</summary>
		/// <param name="textBox"><see cref="TextBox"/> for spell checking.</param>
		/// <param name="enable">Enable spell checking.</param>
		public TextBoxSpellChecker(TextBox textBox, bool enable)
		{
			if(textBox == null)
				throw new ArgumentNullException("textBox");
			_textBox = textBox;
			_cache = new Dictionary<string, bool>();
			_errors = new List<Substring>();
			Enabled = enable;
		}

		/// <summary>Enable spell checking.</summary>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if(_enabled != value)
				{
					_enabled = value;
					if(value)
					{
						_textBoxGraphics = Graphics.FromHwnd(_textBox.Handle);
						_textBox.TextChanged += OnTextChanged;
						_textBox.SizeChanged += OnSizeChanged;
						AssignHandle(_textBox.Handle);
						Revalidate();
					}
					else
					{
						_textBox.TextChanged -= OnTextChanged;
						_textBox.SizeChanged -= OnSizeChanged;
						ReleaseHandle();
						if(_textBoxGraphics != null)
						{
							_textBoxGraphics.Dispose();
							_textBoxGraphics = null;
						}
						if(_bufferGraphics != null)
						{
							_bufferGraphics.Dispose();
							_bufferGraphics = null;
						}
						if(_bitmap != null)
						{
							_bitmap.Dispose();
							_bitmap = null;
						}
					}
				}
			}
		}

		protected override void WndProc(ref Message m)  
		{  
			switch (m.Msg)  
			{  
				case 15:
					_textBox.Invalidate();  
					base.WndProc(ref m);  
					PaintErrors();  
					break;  
				default:  
					base.WndProc(ref m);  
					break;  
			}  
		}

		private void PaintErrors()
		{
			if(SpellingService.Enabled && _errors.Count != 0)
			{
				if(_bitmap != null && _bitmap.Size != _textBox.Size)
				{
					if(_bufferGraphics != null)
					{
						_bufferGraphics.Dispose();
						_bufferGraphics = null;
					}
					_bitmap.Dispose();
					_bitmap = null;
				}
				if(_bitmap == null)
				{
					if(_bufferGraphics != null)
					{
						_bufferGraphics.Dispose();
						_bufferGraphics = null;
					}
					_bitmap = new Bitmap(_textBox.Width, _textBox.Height);
				}
				if(_bufferGraphics == null)
				{
					_bufferGraphics = Graphics.FromImage(_bitmap);
					_bufferGraphics.Clip = new Region(_textBox.ClientRectangle);
				}
				_bufferGraphics.Clear(Color.Transparent);
				using(var pen = new Pen(Color.Red)
					{
						Width = 2,
						DashStyle = DashStyle.Dash,
					})
				{
					for(int i = 0; i < _errors.Count; ++i)
					{
						var err = _errors[i];
						int line1 = _textBox.GetLineFromCharIndex(err.Start);
						int line2 = _textBox.GetLineFromCharIndex(err.End);
						var pos1 = _textBox.GetPositionFromCharIndex(err.Start);
						var pos2 = _textBox.GetPositionFromCharIndex(err.End);
						if(line1 != line2)
						{
							pos2.X += 6;
							pos1.Y += 15;
							pos2.Y = pos1.Y;
							pos2.X = _textBox.ClientSize.Width;
						}
						else
						{
							pos2.X += 6;
							pos1.Y += 15;
							pos2.Y += 15;
						}
						_bufferGraphics.DrawLine(pen, pos1, pos2);
					}
				}
				_textBoxGraphics.DrawImageUnscaled(_bitmap, 0, 0);
			}
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			if(_textBoxGraphics != null)
				_textBoxGraphics.Dispose();
			_textBoxGraphics = Graphics.FromHwnd(_textBox.Handle);
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			if(SpellingService.Enabled)
			{
				Revalidate();
			}
		}

		private bool SpellCheck(string word)
		{
			bool res;
			if(!_cache.TryGetValue(word, out res))
			{
				res = SpellingService.Spell(word);
				_cache.Add(word, res);
			}
			return res;
		}

		private void Revalidate()
		{
			var text = _textBox.Text;
			int start = -1;
			_errors.Clear();
			for(int i = 0; i < text.Length; ++i)
			{
				var c = text[i];
				if(char.IsLetter(c) || c == '\'')
				{
					if(start == -1)
						start = i;
				}
				else
				{
					if(start != -1)
					{
						var word = text.GetSubstring(start, i - start);
						start = -1;
						if(!SpellCheck(word))
							_errors.Add(word);
					}
				}
			}
			if(start != -1)
			{
				var word = text.GetSubstring(start, text.Length - start);
				if(!SpellCheck(word))
					_errors.Add(word);
			}
			_textBox.Invalidate();
		}

		public void Dispose()
		{
			if(_enabled)
			{
				_textBox.TextChanged -= OnTextChanged;
				_textBox.SizeChanged -= OnSizeChanged;
			}
			_cache.Clear();
			ReleaseHandle();
			if(_textBoxGraphics != null)
			{
				_textBoxGraphics.Dispose();
				_textBoxGraphics = null;
			}
			if(_bufferGraphics != null)
			{
				_bufferGraphics.Dispose();
				_bufferGraphics = null;
			}
			if(_bitmap != null)
			{
				_bitmap.Dispose();
				_bitmap = null;
			}
		}
	}
}