﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.Gui.Views
{
	public class DiffViewModel
	{
		#region .ctor

		public DiffViewModel(IDiffSource diffSource, DiffOptions diffOptions)
		{
			DiffSource  = diffSource;
			DiffOptions = diffOptions;
		}

		#endregion

		#region Properties

		public IDiffSource DiffSource { get; }

		public DiffOptions DiffOptions { get; }

		#endregion

		#region Methods

		public override int GetHashCode()
		{
			var hashCode = 0;
			if(DiffSource != null)
			{
				hashCode = DiffSource.GetHashCode();
			}
			if(DiffOptions != null)
			{
				hashCode ^= DiffOptions.GetHashCode();
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if(!(obj is DiffViewModel other))
			{
				return false;
			}
			return
				object.Equals(DiffSource,  other.DiffSource) &&
				object.Equals(DiffOptions, other.DiffOptions);
		}

		#endregion
	}
}
