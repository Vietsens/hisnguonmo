#region License

// Created by phuongdt

#endregion

using System;
using System.Drawing;

namespace Inventec.Desktop.Core
{
    /// <summary>
    /// Attribute used to mark an assembly as being a ClearCanvas Plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PluginAttribute : Attribute
    {
        private string _code;
        private string _name;
        private string _description;
        private string _icon;
        private int _imageindex;
        private string _groupName;
        private long _moduleType;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PluginAttribute()
		{
		}

        /// <summary>
        /// A friendly Code for the plugin.  
        /// </summary>
        /// <remarks>
        /// This is optional and may be supplied as a code parameter.
        /// </remarks>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

    	/// <summary>
        /// A friendly name for the plugin.  
        /// </summary>
        /// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

		/// <summary>
		/// A friendly description for the plugin.  
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

		/// <summary>
		/// The name of an icon resource to associate with the plugin.
		/// </summary>
        public string Icon
    	{
			get { return _icon; }
			set { _icon = value; }
    	}
        
        /// <summary>
        /// The name of an groupName resource to associate with the plugin.
		/// </summary>
        public string GroupName
    	{
            get { return _groupName; }
            set { _groupName = value; }
    	}

        /// <summary>
        /// The name of an ModuleType resource to associate with the plugin.
        /// </summary>
        public long ModuleType
        {
            get { return _moduleType; }
            set { _moduleType = value; }
        }
        
        /// <summary>
        /// The name of an Imageindex resource to associate with the plugin.
        /// </summary>
        public int Imageindex
        {
            get { return _imageindex; }
            set { _imageindex = value; }
        }
    }
}
