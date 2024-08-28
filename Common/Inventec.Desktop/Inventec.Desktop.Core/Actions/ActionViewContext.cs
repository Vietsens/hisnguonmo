#region License

// Created by phuongdt

#endregion

using System;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Utilities;

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Interface for an <see cref="IActionView"/>'s context.
	/// </summary>
	public interface IActionViewContext
	{
		/// <summary>
		/// Gets the associated <see cref="IAction"/>.
		/// </summary>
		IAction Action { get; }

		/// <summary>
		/// Gets or sets the <see cref="IconSize"/> to be shown by the <see cref="IActionView"/>.
		/// </summary>
		IconSize IconSize { get; set; }

		/// <summary>
		/// Fires when the <see cref="IconSize"/> has changed.
		/// </summary>
		event EventHandler IconSizeChanged;
	}

	/// <summary>
	/// Simple implementation of an <see cref="IActionViewContext"/>.
	/// </summary>
	public class ActionViewContext : IActionViewContext
	{
		private readonly IAction _action;
		private IconSize _iconSize;
		private event EventHandler _iconSizeChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The associated <see cref="IAction"/>.</param>
		public ActionViewContext(IAction action)
			: this(action, default(IconSize))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The associated <see cref="IAction"/>.</param>
		/// <param name="iconSize">The initial icon size.</param>
		public ActionViewContext(IAction action, IconSize iconSize)
		{
			Platform.CheckForNullReference(action, "action");
			_action = action;
			_iconSize = iconSize;
		}

		#region IActionViewContext Members

		/// <summary>
		/// Gets the associated <see cref="IAction"/>.
		/// </summary>
		public IAction Action
		{
			get { return _action; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IActionViewContext.IconSize"/> to be shown by the <see cref="IActionView"/>.
		/// </summary>
		public IconSize IconSize
		{
			get
			{
				return _iconSize;
			}
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					EventsHelper.Fire(_iconSizeChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Fires when the <see cref="IActionViewContext.IconSize"/> has changed.
		/// </summary>
		public event EventHandler IconSizeChanged
		{
			add { _iconSizeChanged += value; }
			remove { _iconSizeChanged -= value; }
		}

		#endregion
	}
}