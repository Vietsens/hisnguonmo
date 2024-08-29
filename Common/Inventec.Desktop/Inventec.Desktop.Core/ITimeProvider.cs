#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core
{
	/// <summary>
	/// Defines the interface to a time provider.
	/// </summary>
	/// <remarks>
	/// Applications should call <see cref="Platform.Time"/> rather than making direct use of a time provider.
	/// </remarks>
	public interface ITimeProvider
	{
		/// <summary>
		/// Gets the current date and time, in either local or UTC.
		/// </summary>
		/// <param name="kind"> </param>
		DateTime GetCurrentTime(DateTimeKind kind);
	}
}
