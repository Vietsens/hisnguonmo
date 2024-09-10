#region License

// Created by phuongdt

#endregion

using System;
using System.ComponentModel;

namespace Inventec.Desktop.Core.Actions
{
	/// <summary>
	/// Models a toolbar item that displays a text box into which the user can type.
	/// </summary>
	public interface ITextBoxAction : IAction
	{
		/// <summary>
		/// Occurs when the value of <see cref="TextValue"/> changes.
		/// </summary>
		event EventHandler TextValueChanged;

		/// <summary>
		/// Gets or sets the value of the text displayed in the text box.
		/// </summary>
		string TextValue { get; set; }

		/// <summary>
		/// Occurs when the value of <see cref="CueText"/> changes.
		/// </summary>
		event EventHandler CueTextChanged;

		/// <summary>
		/// Gets or sets the cue text displayed in the text box when it does not have focus.
		/// </summary>
		[Localizable(true)]
		string CueText { get; set; }
	}
}