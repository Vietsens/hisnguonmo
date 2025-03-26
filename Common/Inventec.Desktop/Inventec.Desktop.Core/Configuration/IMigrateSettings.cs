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
