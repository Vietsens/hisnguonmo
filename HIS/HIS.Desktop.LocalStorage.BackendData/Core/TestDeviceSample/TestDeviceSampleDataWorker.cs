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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.Core.TestDeviceSample
{
    public class TestDeviceSampleDataWorker
    {
        private static List<TestDeviceSampleADO> testDeviceSamples { get; set; }

        public static List<TestDeviceSampleADO> TestDeviceSamples
        {
            get
            {
                if (testDeviceSamples == null)
                {
                    testDeviceSamples = new List<TestDeviceSampleADO>();
                }
                lock (testDeviceSamples) ;
                return testDeviceSamples;
            }
            set
            {
                lock (testDeviceSamples) ;
                testDeviceSamples = value;
            }
        }
    }
}
