#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Utilities
{
	/// <summary>
	/// <see cref="EventArgs"/>-derived class for raising events about a particular object of type <typeparamref name="TItem"/>.
	/// </summary>
	/// <typeparam name="TItem">Any arbitrary type for which an event is to be raised.</typeparam>
	public class ItemEventArgs<TItem> : EventArgs
	{
		private TItem _item;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The item that is the subject of the raised event.</param>
		public ItemEventArgs(TItem item)
		{
			_item = item;
		}

		/// <summary>
		/// Gets the item that is the subject of the raised event.
		/// </summary>
		public TItem Item
		{
			get { return _item; }
		}
	}
}
