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
using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Inventec.DrawTools
{
	//[Serializable]
	/// <summary>
	/// Each <see cref="Layer"/> contains a list of Graphic Objects <see cref="GraphicsList"/>.
	/// Each <see cref="Layer"/> is contained in the <see cref="Layers"/> collection.
	/// Properties:
	///	LayerName		User-defined name of the Layer
	///	Graphics			Collection of <see cref="DrawObject"/>s in the Layer
	///	IsVisible			True if the <see cref="Layer"/> is visible
	///	IsActive			True if the <see cref="Layer"/> is the active <see cref="Layer"/> (only one <see cref="Layer"/> may be active at a time)
	///	Dirty				True if any object in the <see cref="Layer"/> is dirty
	/// </summary>
	public class Layer
	{
		private string _name;
		private bool _isDirty;
		private bool _visible;
		private bool _active;
		private GraphicsList _graphicsList;

		/// <summary>
		/// <see cref="Layer"/> Name (User-defined)
		/// </summary>
		public string LayerName
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// List of Graphic objects (derived from <see cref="DrawObject"/>) contained by this <see cref="Layer"/>
		/// </summary>
		public GraphicsList Graphics
		{
			get { return _graphicsList; }
			set { _graphicsList = value; }
		}

		/// <summary>
		/// Returns True if this <see cref="Layer"/> is visible, else False
		/// </summary>
		public bool IsVisible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		/// <summary>
		/// Returns True if this is the active <see cref="Layer"/>, else False
		/// </summary>
		public bool IsActive
		{
			get { return _active; }
			set { _active = value; }
		}

		/// <summary>
		/// Dirty is True if any elements in the contained <see cref="GraphicsList"/> are dirty, else False
		/// </summary>
		public bool Dirty
		{
			get
			{
				if (_isDirty == false)
					_isDirty = _graphicsList.Dirty;
				return _isDirty;
			}
			set
			{
				_graphicsList.Dirty = false;
				_isDirty = false;
			}
		}

		private const string entryLayerName = "LayerName";
		private const string entryLayerVisible = "LayerVisible";
		private const string entryLayerActive = "LayerActive";
		private const string entryObjectType = "ObjectType";
		private const string entryGraphicsCount = "GraphicsCount";

		public void SaveToStream(SerializationInfo info, int orderNumber)
		{
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryLayerName, orderNumber),
				_name);

			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryLayerVisible, orderNumber),
				_visible);

			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryLayerActive, orderNumber),
				_active);

			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryGraphicsCount, orderNumber),
				_graphicsList.Count);

			for (int i = 0; i < _graphicsList.Count; i++)
			{
				object o = _graphicsList[i];
				info.AddValue(
					String.Format(CultureInfo.InvariantCulture,
					              "{0}{1}-{2}",
					              entryObjectType, orderNumber, i),
					o.GetType().FullName);

				((DrawObject)o).SaveToStream(info, orderNumber, i);
			}
		}

		public void LoadFromStream(SerializationInfo info, int orderNumber)
		{
			_graphicsList = new GraphicsList();

			_name = info.GetString(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryLayerName, orderNumber));

			_visible = info.GetBoolean(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryLayerVisible, orderNumber));

			_active = info.GetBoolean(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryLayerActive, orderNumber));

			int n = info.GetInt32(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}",
				              entryGraphicsCount, orderNumber));

			for (int i = 0; i < n; i++)
			{
				string typeName;
				typeName = info.GetString(
					String.Format(CultureInfo.InvariantCulture,
					              "{0}{1}-{2}",
					              entryObjectType, orderNumber, i));

				object drawObject;
				drawObject = Assembly.GetExecutingAssembly().CreateInstance(typeName);

				((DrawObject)drawObject).LoadFromStream(info, orderNumber, i);

                // Thanks to Member 3272353 for this fix to object ordering problem.
				// _graphicsList.Add((DrawObject)drawObject);
                _graphicsList.Append((DrawObject) drawObject);
			}
		}

		internal void Draw(Graphics g)
		{
			_graphicsList.Draw(g);
		}
	}
}
