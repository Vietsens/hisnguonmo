#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Scripting
{
	/// <summary>
	/// Used to specify that a class (for example, an <see cref="IScriptEngine"/>) 
	/// supports a certain scripting language.
	/// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class LanguageSupportAttribute : Attribute
    {
        private string _language;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="language">A string describing the language.</param>
		public LanguageSupportAttribute(string language)
        {
            _language = language;
        }

		/// <summary>
		/// Gets a string describing the language.
		/// </summary>
        public string Language
        {
            get { return _language; }
        }

		/// <summary>
		/// Determines whether or not this instance is the same as <paramref name="obj"/>, which is itself an <see cref="Attribute"/>.
		/// </summary>
		public override bool Match(object obj)
        {
            LanguageSupportAttribute that = obj as LanguageSupportAttribute;
            return that != null && that.Language.ToLower().Equals(this.Language.ToLower());
        }
    }
}
