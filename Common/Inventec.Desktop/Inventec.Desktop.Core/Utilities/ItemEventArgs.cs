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

namespace Inventec.Desktop.Core.Utilities
{
	/// <summary>
	/// <see cref="EventArgs"/>-derived class for raising events about a particular object of type <typeparamref name="TItem"/>.
	/// </summary>
	/// <typeparam name="TItem">Any arbitrary type for which an event is to be raised.</typeparam>
	public class ItemEventArgs<TItem> : EventArgs
	{
		private TItem _item;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The item that is the subject of the raised event.</param>
		public ItemEventArgs(TItem item)
		{
			_item = item;
		}

		/// <summary>
		/// Gets the item that is the subject of the raised event.
		/// </summary>
		public TItem Item
		{
			get { return _item; }
		}
	}
}
