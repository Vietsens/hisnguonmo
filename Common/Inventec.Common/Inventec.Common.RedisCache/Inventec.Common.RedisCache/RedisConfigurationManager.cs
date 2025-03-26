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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.RedisCache
{
    public class RedisConfigurationSection : ConfigurationSection
    {
        #region Constants

        private const string HostAttributeName = "host";
        private const string PortAttributeName = "port";
        private const string PasswordAttributeName = "password";
        private const string DatabaseIDAttributeName = "databaseID";

        #endregion

        #region Properties

        [ConfigurationProperty(HostAttributeName, IsRequired = true)]
        public string Host
        {
            get { return this[HostAttributeName].ToString(); }
        }

        [ConfigurationProperty(PortAttributeName, IsRequired = true)]
        public int Port
        {
            get { return (int)this[PortAttributeName]; }
        }

        [ConfigurationProperty(PasswordAttributeName, IsRequired = false)]
        public string Password
        {
            get { return this[PasswordAttributeName].ToString(); }
        }

        [ConfigurationProperty(DatabaseIDAttributeName, IsRequired = false)]
        public long DatabaseID
        {
            get { return (long)this[DatabaseIDAttributeName]; }
        }

        #endregion
    }
}
