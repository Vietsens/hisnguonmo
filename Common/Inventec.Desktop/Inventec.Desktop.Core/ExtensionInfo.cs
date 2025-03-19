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

// Creater by phuongdt

#endregion

using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Describes an extension.  
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable and safe for concurrent access by multiple threads.
    /// </remarks>
    [Serializable]
    public sealed class ExtensionInfo : IBrowsable
    {
        private readonly TypeRef _extensionClass;
        private readonly TypeRef _pointExtended;
        private readonly string _code;
        private readonly string _name;
        private readonly string _description;
        private int _imageindex;
        private string _icon;
        private string _groupName;
        private long _moduleType;
        private readonly bool _enabled;
        private readonly string _featureToken;
        private ToolSet _toolSet;

        public ExtensionInfo(TypeRef extensionClass, TypeRef pointExtended, string code, string name, string keyLanguage, string description, int imageindex, string groupName, long moduleType, bool enabled, ToolSet toolSet)
            : this(extensionClass, pointExtended, code, name, description, imageindex, null, groupName, moduleType, enabled, toolSet, null) { }

        public ExtensionInfo(TypeRef extensionClass, TypeRef pointExtended, string code, string name, string keyLanguage, string description, int imageindex, string icon, string groupName, long moduleType, bool enabled, ToolSet toolSet)
            : this(extensionClass, pointExtended, code, name, description, imageindex, icon, groupName, moduleType, enabled, toolSet, null) { }

        public ExtensionInfo(TypeRef extensionClass, TypeRef pointExtended, string code, string name, string description, int imageindex, string icon, string groupName, long moduleType, bool enabled, ToolSet toolSet, string featureToken)
        {
            _extensionClass = extensionClass;
            _pointExtended = pointExtended;
            _code = code;
            _name = name;
            _description = description;
            _enabled = enabled;
            _featureToken = featureToken;
            _toolSet = toolSet;
            _imageindex = imageindex;
            _icon = icon;
            _groupName = groupName;
            _moduleType = moduleType;
        }

        /// <summary>
        /// Gets the type that implements the extension.
        /// </summary>
        public TypeRef ExtensionClass
        {
            get { return _extensionClass; }
        }

        /// <summary>
        /// Gets the extension point type which this extension extends.
        /// </summary>
        public TypeRef PointExtended
        {
            get { return _pointExtended; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this extension is enabled by application configuration.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
        }

        /// <summary>
        /// Gets value indicating whether or not this extension is KeyboardActions by application configuration.
        /// </summary>
        public ToolSet ToolSet
        {
            get { return _toolSet; }
            set { _toolSet = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this extension is authorized by application licensing.
        /// </summary>
        public bool Authorized
        {
            get { return true; }
            //get { return string.IsNullOrEmpty(_featureToken) || LicenseInformation.IsFeatureAuthorized(_featureToken); }
        }

        /// <summary>
        /// Gets the feature identification token to be checked against application licensing.
        /// </summary>
        public string FeatureToken
        {
            get { return _featureToken; }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", _name ?? _extensionClass.Resolve().Name, _extensionClass.FullName);
        }

        #region IBrowsable Members

        /// <summary>
        /// Gets a friendly name of this extension, if one exists, otherwise null.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        public string Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Gets a friendly description of this extension, if one exists, otherwise null.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the formal name of this extension, which is the fully qualified name of the extension class.
        /// </summary>
        public string FormalName
        {
            get { return _extensionClass.FullName; }
        }

        /// <summary>
        /// The name of an groupName resource to associate with the plugin.
        /// </summary>
        public string GroupName
        {
            get { return _groupName; }
        }

        /// <summary>
        /// The name of an ModuleType resource to associate with the plugin.
        /// </summary>
        public long ModuleType
        {
            get { return _moduleType; }
        }

        /// <summary>
        /// The name of an Imageindex resource to associate with the plugin.
        /// </summary>
        public int Imageindex
        {
            get { return _imageindex; }
        }
        
        /// <summary>
        /// The name of an Icon resource to associate with the plugin.
        /// </summary>
        public string Icon
        {
            get { return _icon; }
        }
        #endregion
    }
}
