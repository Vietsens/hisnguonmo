#region License

// Created by phuongdt

#endregion

namespace Inventec.Desktop.Core
{
	/// <summary>
	/// An extension filter that checks for equality with the extension class name.
	/// </summary>
    public class ClassNameExtensionFilter : ExtensionFilter
    {
        private string _name;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The extension class name that will be a match for this filter.</param>
        public ClassNameExtensionFilter(string name)
        {
            _name = name;
        }

		/// <summary>
		/// Tests whether or not the input <see cref="ExtensionInfo.ExtensionClass"/>' full name matches 
		/// the name supplied to the filter constructor.
		/// </summary>
        public override bool Test(ExtensionInfo extension)
        {
            return extension.ExtensionClass.FullName.Equals(_name);
        }
    }
}
