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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Core.TestDeviceSample;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Worker
{
    /// <summary>
    /// Port -> logged-in account sessions, kept in the shared in-memory list that the TestDeviceSample form also reads.
    /// TeminalCode holds the COM port name. Only use from the UI thread (the shared list has no real lock).
    /// </summary>
    internal static class SessionStore
    {
        internal static TestDeviceSampleADO GetByPort(string portName)
        {
            try
            {
                return TestDeviceSampleDataWorker.TestDeviceSamples
                    .FirstOrDefault(o => String.Equals(o.TeminalCode, portName, StringComparison.OrdinalIgnoreCase)
                        && !String.IsNullOrEmpty(o.Loginname));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        internal static List<TestDeviceSampleADO> GetByLoginname(string loginname)
        {
            try
            {
                return TestDeviceSampleDataWorker.TestDeviceSamples
                    .Where(o => String.Equals(o.Loginname, loginname, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<TestDeviceSampleADO>();
            }
        }

        internal static TestDeviceSampleADO Bind(string portName, ACS_USER user, DateTime loginTime)
        {
            TestDeviceSampleADO result = null;
            try
            {
                RemoveByPort(portName);
                result = new TestDeviceSampleADO();
                result.TeminalCode = portName;
                result.Loginname = user.LOGINNAME;
                result.Username = user.USERNAME;
                result.SessionTimeStart = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(loginTime) ?? 0;
                TestDeviceSampleDataWorker.TestDeviceSamples.Add(result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static void Remove(TestDeviceSampleADO session)
        {
            try
            {
                if (session != null)
                    TestDeviceSampleDataWorker.TestDeviceSamples.Remove(session);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Removes every session of the port and returns the removed ones (for the event log).
        /// </summary>
        internal static List<TestDeviceSampleADO> RemoveByPort(string portName)
        {
            List<TestDeviceSampleADO> removed = new List<TestDeviceSampleADO>();
            try
            {
                removed = TestDeviceSampleDataWorker.TestDeviceSamples
                    .Where(o => String.Equals(o.TeminalCode, portName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach (TestDeviceSampleADO item in removed)
                    TestDeviceSampleDataWorker.TestDeviceSamples.Remove(item);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return removed;
        }
    }
}
