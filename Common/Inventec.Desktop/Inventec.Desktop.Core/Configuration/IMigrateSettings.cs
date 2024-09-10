#region License

// Created by phuongdt

#endregion

using System;

namespace Inventec.Desktop.Core.Configuration
{
	public enum MigrationScope
	{
		User,
		Shared
	}
	
	public class UserSettingsMigrationDisabledAttribute : Attribute
    {}

    public class SharedSettingsMigrationDisabledAttribute : Attribute
    {}

    public class SettingsPropertyMigrationValues
    {
		public SettingsPropertyMigrationValues(string propertyName, MigrationScope migrationScope, object currentValue, object previousValue)
        {
			PropertyName = propertyName;
			MigrationScope = migrationScope;
            CurrentValue = currentValue;
            PreviousValue = previousValue;
        }

		public MigrationScope MigrationScope { get; private set; }
		public string PropertyName { get; private set; }
        public object PreviousValue { get; private set; }
        public object CurrentValue { get; set; }
    }

	public interface IMigrateSettings
    {
        void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues);
    }
}