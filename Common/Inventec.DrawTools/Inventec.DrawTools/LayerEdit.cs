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
namespace Inventec.DrawTools
{
	public class LayerEdit
	{
		private string _name;
		private bool _visible;
		private bool _active;
		private bool _new;
		private bool _deleted;

		/// <summary>
		/// Layer Name
		/// </summary>
		public string LayerName
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// IsVisible is True if this Layer is visible, else False
		/// </summary>
		public bool LayerVisible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		/// <summary>
		/// IsActive is True if this is the active Layer, else False
		/// </summary>
		public bool LayerActive
		{
			get { return _active; }
			set { _active = value; }
		}

		/// <summary>
		/// True if Layer was added in the dialog
		/// </summary>
		public bool LayerNew
		{
			get { return _new; }
			set { _new = value; }
		}

		/// <summary>
		/// True if Layer was deleted in the dialog
		/// </summary>
		public bool LayerDeleted
		{
			get { return _deleted; }
			set { _deleted = value; }
		}
	}
}
