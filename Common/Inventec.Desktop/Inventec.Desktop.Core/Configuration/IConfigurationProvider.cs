#region License

// Created by phuongdt

#endregion

using System.Collections.Generic;

namespace Inventec.Desktop.Core.Configuration
{
	/// <summary>
	/// An interface that provides direct access to the configuration settings
	/// without having to instantiate a configuration component.
	/// </summary>
	public interface IConfigurationProvider
	{
		string SettingsClassName { get; }
		void UpdateConfiguration(Dictionary<string, string> settings);
	}
}
