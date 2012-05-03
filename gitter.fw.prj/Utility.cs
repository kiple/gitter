﻿namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Drawing.Text;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Security.Principal;
	using System.Windows.Forms;
	using Microsoft.Win32;

	public static class Utility
	{
		private static readonly Version _osVersion = Environment.OSVersion.Version;

		private static readonly Image _dummyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		private static readonly Graphics _measurementGraphics = Graphics.FromImage(_dummyImage);
		/// <summary>1 Jan 1970</summary>
		private static readonly DateTime UnixEraStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static Graphics MeasurementGraphics
		{
			get { return _measurementGraphics; }
		}

		public static bool IsOSVistaOrNewer
		{
			get { return _osVersion >= new Version(6, 0); }
		}

		public static bool IsOSWindows7OrNewer
		{
			get { return _osVersion >= new Version(6, 1); }
		}

		private static readonly string[] SizePostfix = new[]
		{
			"b ", "Kb", "Mb", "Gb", "Tb", "Pb"
		};

		public static string FormatSize(long size)
		{
			double s = size;
			var sizeId = 0;
			while(s > 1024)
			{
				s /= 1024;
				++sizeId;
			}
			if(sizeId >= SizePostfix.Length) return size.ToString();
			return ((int)(s + .5)).ToString() + " " + SizePostfix[sizeId];
		}

		public static string FormatDate(DateTime date, DateFormat format)
		{
			switch(format)
			{
				case DateFormat.SystemDefault:
					return date.ToString("dd.MM.yyyy HH:mm:ss");
				case DateFormat.UnixTimestamp:
					return ((int)(date - UnixEraStart).TotalSeconds).ToString();
				case DateFormat.Relative:
					{
						var span = DateTime.Now - date;
						if(span.TotalDays >= 365)
						{
							var years = (int)(span.TotalDays / 365);
							return (years == 1) ? "1 year ago" : years.ToString() + " years ago";
						}
						if(span.TotalDays >= 30)
						{
							var months = (int)(span.TotalDays / 30);
							return (months == 1) ? "1 month ago" : months.ToString() + " months ago";
						}
						if(span.TotalDays >= 7)
						{
							var weeks = (int)(span.TotalDays / 7);
							return (weeks == 1) ? "1 week ago" : weeks.ToString() + " weeks ago";
						}
						if(span.TotalDays >= 1)
						{
							var days = (int)span.TotalDays;
							return (days == 1) ? "1 day ago" : days.ToString() + " days ago";
						}
						if(span.TotalHours >= 1)
						{
							var hours = (int)span.TotalHours;
							return (hours == 1) ? "1 hour ago" : hours.ToString() + " hours ago";
						}
						if(span.TotalMinutes >= 1)
						{
							var minutes = (int)span.TotalMinutes;
							return (minutes == 1) ? "1 minute ago" : minutes.ToString() + " minutes ago";
						}
						var seconds = (int)span.TotalSeconds;
						return (seconds == 1) ? "1 second ago" : seconds.ToString() + " seconds ago";
					}
				case DateFormat.ISO8601:
					return date.FormatISO8601();
				case DateFormat.RFC2822:
					return date.FormatRFC2822();
				default:
					throw new ArgumentException("format");
			}
		}

		public static Bitmap QueryIcon(string fileName)
		{
			return IconCache.GetIcon(fileName);
		}

		public static string GetFileType(string fileName, bool dir, bool useExtensionOnly)
		{
			const int SHGFI_USEFILEATTRIBUTES = 0x10;
			const int SHGFI_TYPENAME = 0x400;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;
			const int FILE_ATTRIBUTE_DIR = 0x10;

			var attr = dir ? FILE_ATTRIBUTE_DIR | FILE_ATTRIBUTE_NORMAL : FILE_ATTRIBUTE_NORMAL;
			var req = useExtensionOnly ? SHGFI_USEFILEATTRIBUTES | SHGFI_TYPENAME : SHGFI_TYPENAME;

			var info = new NativeMethods.SHFILEINFO();
			var ret = NativeMethods.SHGetFileInfo(fileName, attr, ref info, Marshal.SizeOf(info), req);
			return info.szTypeName;
		}

		public static Control GetParentControl(ToolStripItem item)
		{
			if(item == null) throw new ArgumentNullException("item");
			var cms = item.Owner as ContextMenuStrip;
			if(cms != null)
			{
				return cms.SourceControl;
			}
			var tsdd = item.Owner as ToolStripDropDown;
			if(tsdd != null)
			{
				return tsdd.Parent;
			}
			return item.Owner;
		}

		private static readonly LinkedList<IDisposable> LazyDisposables = new LinkedList<IDisposable>();

		public static void MarkDropDownForAutoDispose(ToolStripDropDown menu)
		{
			if(menu == null) throw new ArgumentNullException("menu");
			menu.Closed += (sender, e) => DisposeOnIdle((IDisposable)sender);
		}

		private static void DisposeOnIdle(IDisposable obj)
		{
			LazyDisposables.AddLast(obj);
			if(LazyDisposables.Count == 1)
			{
				Application.Idle += DisposeRegisteredObjects;
			}
		}

		private static void DisposeRegisteredObjects(object sender, EventArgs e)
		{
			foreach(var obj in LazyDisposables)
			{
				obj.Dispose();
			}
			LazyDisposables.Clear();
			Application.Idle -= DisposeRegisteredObjects;
		}

		public static readonly StringFormat DefaultStringFormat =
			new StringFormat(StringFormat.GenericTypographic)
			{
				FormatFlags =
					StringFormatFlags.LineLimit |
					StringFormatFlags.DisplayFormatControl |
					StringFormatFlags.MeasureTrailingSpaces |
					StringFormatFlags.FitBlackBox |
					StringFormatFlags.NoWrap,
				HotkeyPrefix = HotkeyPrefix.None,
				LineAlignment = StringAlignment.Near,
				Trimming = StringTrimming.None,
			};

		public static readonly StringFormat DefaultStringFormatLeftAlign =
			new StringFormat(DefaultStringFormat)
			{
				Alignment = StringAlignment.Near,
			};

		public static readonly StringFormat DefaultStringFormatRightAlign =
			new StringFormat(DefaultStringFormat)
			{
				Alignment = StringAlignment.Far,
			};

		public static readonly StringFormat DefaultStringFormatCenterAlign =
			new StringFormat(DefaultStringFormat)
			{
				Alignment = StringAlignment.Center,
			};

		public const TextRenderingHint TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

		public const int TextContrast = 0;

		public static GraphicsPath GetRoundedRectangle(RectangleF rect, float arcRadius)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;
			var d = 2 * arcRadius;

			var gp = new GraphicsPath();
			if(arcRadius == 0)
			{
				gp.AddRectangle(rect);
			}
			else
			{
				gp.AddArc(x, y, d, d, 180, 90);
				gp.AddLine(x + arcRadius, y, x + w - arcRadius - 1, y);
				gp.AddArc(x + w - d - 1, y, d, d, 270, 90);
				gp.AddLine(x + w - 1, y + arcRadius, x + w - 1, y + h - arcRadius - 1);
				gp.AddArc(x + w - d - 1, y + h - d - 1, d, d, 0, 90);
				gp.AddLine(x + w - arcRadius - 1, y + h - 1, x + arcRadius, y + h - 1);
				gp.AddArc(x, y + h - d - 1, d, d, 90, 90);
				gp.AddLine(x, y + h - arcRadius - 1, x, y + arcRadius);
			}
			gp.CloseFigure();
			return gp;
		}

		public static Region GetRoundedRegion(RectangleF rect, float arcRadius)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;
			var d = 2 * arcRadius;

			using(var gp = new GraphicsPath())
			{
				if(arcRadius == 0)
				{
					gp.AddRectangle(rect);
				}
				else
				{
					gp.AddArc(x, y, d, d, 180, 90);
					gp.AddLine(x + arcRadius, y, x + w - arcRadius + 1, y);
					gp.AddArc(x + w - d, y, d - 1, d, 270, 90);
					gp.AddLine(x + w, y + arcRadius, x + w, y + h - arcRadius);
					gp.AddArc(x + w - d, y + h - d - 1, d - 1, d, 0, 90);
					gp.AddLine(x + w - arcRadius - 1, y + h, x + arcRadius, y + h);
					gp.AddArc(x, y + h - d - 1, d, d, 90, 90);
					gp.AddLine(x, y + h - arcRadius, x, y + arcRadius);
				}
				gp.CloseFigure();
				return new Region(gp);
			}
		}

		public static GraphicsPath GetRoundedRectangle(RectangleF rect, float topLeftCorner, float topRightCorner, float bottomLeftCorner, float bottomRightCorner)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;

			var gp = new GraphicsPath();
			if(topLeftCorner != 0)
			{
				gp.AddArc(x, y,
					2 * topLeftCorner, 2 * topLeftCorner, 180, 90);
			}
			gp.AddLine(x, y, x + w - topRightCorner - 1, y);
			if(topRightCorner != 0)
			{
				gp.AddArc(
					x + w - 2 * topRightCorner - 1,
					y,
					2 * topRightCorner, 2 * topRightCorner, 270, 90);
			}
			gp.AddLine(x + w - 1, y, x + w - 1, y + h - bottomRightCorner - 1);
			if(bottomRightCorner != 0)
			{
				gp.AddArc(
					x + w - 2 * bottomRightCorner - 1,
					y + h - 2 * bottomRightCorner - 1,
					2 * bottomRightCorner, 2 * bottomRightCorner, 0, 90);
			}
			gp.AddLine(x + w, y + h - 1, x + bottomLeftCorner, y + h - 1);
			if(bottomLeftCorner != 0)
			{
				gp.AddArc(
					x,
					y + h - 2 * bottomLeftCorner - 1,
					2 * bottomLeftCorner, 2 * bottomLeftCorner, 90, 90);
			}
			gp.AddLine(x, y + h - bottomLeftCorner - 1, x, y + topLeftCorner);
			gp.CloseFigure();
			return gp;
		}

		private static ITaskbarList _taskBarList;
		internal static int WM_TASKBAR_BUTTON_CREATED;

		internal static ITaskbarList TaskBarList
		{
			get { return _taskBarList; }
		}

		public static void EnableWin7TaskbarSupport()
		{
			if(_taskBarList != null) return;
			if(IsOSWindows7OrNewer)
			{
				WM_TASKBAR_BUTTON_CREATED = NativeMethods.RegisterWindowMessage("TaskbarButtonCreated");
				_taskBarList = (ITaskbarList)Activator.CreateInstance<TaskbarList>();
			}
		}

		public static void DisableWin7TaskbarSupport()
		{
			if(_taskBarList == null) return;
			Marshal.ReleaseComObject(_taskBarList);
			_taskBarList = null;
		}

		public static void OpenUrl(string url)
		{
			var psi = new System.Diagnostics.ProcessStartInfo()
				{
					FileName = url,
				};
			if(Directory.Exists(url))
			{
				psi.WorkingDirectory = url;
			}
			else if(File.Exists(url))
			{
				psi.WorkingDirectory = Path.GetDirectoryName(url);
			}
			var p = new System.Diagnostics.Process()
			{
				StartInfo = psi,
			};
			p.Start();
		}

		public static void ShowOpenWithDialog(string fileName)
		{
			var psi = new System.Diagnostics.ProcessStartInfo()
				{
					FileName = "rundll32.exe",
					Arguments = "shell32.dll,OpenAs_RunDLL \"" + fileName + "\"",
				};
			if(Directory.Exists(fileName))
			{
				psi.WorkingDirectory = fileName;
			}
			else if(File.Exists(fileName))
			{
				psi.WorkingDirectory = Path.GetDirectoryName(fileName);
			}
			var p = new System.Diagnostics.Process()
			{
				StartInfo = psi,
			};
			p.Start();
		}

		/// <summary>Checks if process is running with administrator privileges.</summary>
		public static bool IsRunningWithAdministratorRights
		{
			get
			{
				var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				return pricipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}


		[StructLayout(LayoutKind.Sequential)]
		private struct PROCESS_BASIC_INFORMATION
		{
			public int ExitStatus;
			public int PebBaseAddress;
			public int AffinityMask;
			public int BasePriority;
			public int UniqueProcessId;
			public int InheritedFromUniqueProcessId;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool TerminateProcess(IntPtr hProcess, int exitCode);

		[DllImport("ntdll.dll")]

		static extern int NtQueryInformationProcess(
		   IntPtr hProcess,
		   int processInformationClass /* 0 */,
		   ref PROCESS_BASIC_INFORMATION processBasicInformation,
		   uint processInformationLength,
		   out uint returnLength
		);

		/// <summary>
		/// Terminate a process tree
		/// </summary>
		/// <param name="hProcess">The handle of the process</param>
		/// <param name="processID">The ID of the process</param>
		/// <param name="exitCode">The exit code of the process</param>
		public static void TerminateProcessTree(IntPtr hProcess, int processID, int exitCode)
		{
			foreach(var process in Process.GetProcesses())
			{
				var pbi = new PROCESS_BASIC_INFORMATION();
				try
				{
					uint bytesWritten;
					if(NtQueryInformationProcess(
						process.Handle,
						0, ref pbi, (uint)Marshal.SizeOf(pbi),
						out bytesWritten) == 0)
					{
						if(pbi.InheritedFromUniqueProcessId == processID)
						{
							TerminateProcessTree(process.Handle, pbi.UniqueProcessId, exitCode);
						}
					}
				}
				catch
				{
				}
			}
			TerminateProcess(hProcess, exitCode);
		}

		public static string ShowPickFolderDialog(IWin32Window parent)
		{
			if(IsOSVistaOrNewer)
			{
				try
				{
					return VistaPickFolderDialog.Show(parent);
				}
				catch { }
			}
			using(var dlg = new FolderBrowserDialog())
			{
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					return dlg.SelectedPath;
				}
			}
			return null;
		}
	}
}
