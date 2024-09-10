#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core
{
	/// <summary>
	/// Defines possible icon schemes.
	/// </summary>
	/// <remarks>
	/// The use of icon schemes has been deprecated in favour of extensible application GUI themes.
	/// </remarks>
	[Flags]
	[Obsolete("The use of icon schemes has been deprecated in favour of extensible application GUI themes")]
	public enum IconScheme
	{
		/// <summary>
		/// Colour icons.
		/// </summary>
		Colour,

		/// <summary>
		/// Monochrome icons.
		/// </summary>
		Monochrome
	}
}