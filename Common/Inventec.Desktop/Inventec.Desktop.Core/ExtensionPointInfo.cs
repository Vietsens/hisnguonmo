#region License

// Creater by phuongdt

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventec.Desktop.Core
{
	/// <summary>
	/// Describes an extension point.  
	/// </summary>
	/// <remarks>
	/// Instances of this class are immutable and safe for concurrent access by multiple threads.
	/// </remarks>
	[Serializable]
	public sealed class ExtensionPointInfo : IBrowsable
	{
		private readonly TypeRef _extensionPointClass;
		private readonly TypeRef _extensionInterface;
		private readonly string _name;
		private readonly string _description;

		/// <summary>
		/// Internal constructor.
		/// </summary>
		internal ExtensionPointInfo(TypeRef extensionPointClass, TypeRef extensionInterface, string name, string description)
		{
			_extensionPointClass = extensionPointClass;
			_extensionInterface = extensionInterface;
			_name = name;
			_description = description;
		}

		/// <summary>
		/// Gets the class that defines the extension point.
		/// </summary>
		public TypeRef ExtensionPointClass
		{
			get { return _extensionPointClass; }
		}

		/// <summary>
		/// Gets the interface that an extension must implement.
		/// </summary>
		public TypeRef ExtensionInterface
		{
			get { return _extensionInterface; }
		}

		/// <summary>
		/// Computes and returns a list of the installed extensions to this point,
		/// including disabled extensions.
		/// </summary>
		/// <returns></returns>
		public IList<ExtensionInfo> ListExtensions()
		{
			return Platform.PluginManager.Extensions.Where(ext => ext.PointExtended == _extensionPointClass).ToList();
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", _name ?? _extensionPointClass.Resolve().Name, _extensionPointClass.FullName);
		}

		#region IBrowsable Members

		/// <summary>
		/// Friendly name of the extension point, if one exists, otherwise null.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// A friendly description of the extension point, if one exists, otherwise null.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		/// <summary>
		/// Formal name of the extension, which is the fully qualified name of the extension point class.
		/// </summary>
		public string FormalName
		{
			get { return _extensionPointClass.FullName; }
		}

		#endregion
	}
}
