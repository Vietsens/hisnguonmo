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
using System.Linq;

namespace Inventec.Desktop.Core.Utilities
{
	/// <summary>
	/// Helper class for raising events.
	/// </summary>
	public static class EventsHelper
	{
	    /// <summary>
	    /// Helper method for raising events safely.
	    /// </summary>
	    /// <param name="del">Delegate to invoke.</param>
	    /// <param name="sender">The sender of the event.</param>
	    /// <param name="e">The <see cref="EventArgs"/>.</param>
	    /// <remarks>
	    /// Use this method to invoke user code via delegates.
	    /// This method will log any exceptions thrown in user code and immediately rethrow it.
	    /// The typical usage is shown below.
	    /// </remarks>
	    /// <example>
	    /// <code>
	    /// [C#]
	    /// public class PresentationImage
	    /// {
	    ///    private event EventHandler _imageDrawingEvent;
	    ///    
	    ///    public void Draw()
	    ///    {
	    ///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
	    ///    }
	    /// }
	    /// </code>
	    /// </example>
	    public static void Fire(EventHandler del, object sender, EventArgs e)
	    {
            if (del == null)
                return;

	        var delegates = del.GetInvocationList().Cast<EventHandler>();
            foreach (var sink in delegates)
            {
                try
                {
                    sink.Invoke(sender, e);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                    throw;
                }
            }
        }

        	    /// <summary>
	    /// Helper method for raising events safely.
	    /// </summary>
	    /// <param name="del">Delegate to invoke.</param>
	    /// <param name="sender">The sender of the event.</param>
	    /// <param name="e">The <see cref="EventArgs"/>.</param>
	    /// <remarks>
	    /// Use this method to invoke user code via delegates.
	    /// This method will log any exceptions thrown in user code and immediately rethrow it.
	    /// The typical usage is shown below.
	    /// </remarks>
	    /// <example>
	    /// <code>
	    /// [C#]
	    /// public class PresentationImage
	    /// {
	    ///    private event EventHandler _imageDrawingEvent;
	    ///    
	    ///    public void Draw()
	    ///    {
	    ///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
	    ///    }
	    /// }
	    /// </code>
	    /// </example>
	    public static void Fire<T>(EventHandler<T> del, object sender, T e) where T : EventArgs
        {
            if (del == null)
                return;

            var delegates = del.GetInvocationList().Cast<EventHandler<T>>();
            foreach (var sink in delegates)
            {
                try
                {
                    sink.Invoke(sender, e);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Helper method for raising events safely.
        /// </summary>
        /// <param name="del">Delegate to invoke.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        /// <remarks>
        /// Use this method to invoke user code via delegates.
        /// This method will log any exceptions thrown in user code and immediately rethrow it.
        /// The typical usage is shown below.
        /// </remarks>
        /// <example>
        /// <code>
        /// [C#]
        /// public class PresentationImage
        /// {
        ///    private event EventHandler _imageDrawingEvent;
        ///    
        ///    public void Draw()
        ///    {
        ///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
        ///    }
        /// }
        /// </code>
        /// </example>
	    public static void Fire(Delegate del, object sender, EventArgs e)
		{
			// TODO CR (Apr 13): del should really be EventHandler (and an overload added for EventHandler<T>) since this method won't work with any other kind of delegate
			//                   and a strongly typed API would help programmers from putting in the wrong arguments
			if (del == null)
				return;

			var delegates = del.GetInvocationList();

			foreach (var sink in delegates)
			{
				try
				{
					sink.DynamicInvoke(sender, e);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex);
					throw;
				}
			}
		}
	}
}
