/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core
{
	internal class LocalTimeProvider : ITimeProvider
	{
		#region ITimeProvider Members

		public DateTime GetCurrentTime(DateTimeKind kind)
		{
			switch (kind)
			{
				case DateTimeKind.Utc:
					return DateTime.UtcNow;
				case DateTimeKind.Local:
					return DateTime.Now;
				default:
					throw new ArgumentOutOfRangeException("kind");
			}
		}

		#endregion
	}
}
