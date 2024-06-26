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
using LIS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000360.PDO
{
    public class Mps000360PDO : RDOBase
    {
        public V_LIS_SAMPLE _Sample { get; set; }
        public V_LIS_SAMPLE_SERVICE _SampleService { get; set; }
        public LIS_MACHINE _Machine { get; set; }
        public List<V_HIS_SERE_SERV_TEIN> _sereServTeins { get; set; }
        public HIS_SERE_SERV_EXT _sereServExt { get; set; }

        public Mps000360PDO(V_LIS_SAMPLE sample, V_LIS_SAMPLE_SERVICE service, LIS_MACHINE machine, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_TEIN> results, HIS_SERE_SERV_EXT sereServExt)
        {
            this._Sample = sample;
            this._SampleService = service;
            this._Machine = machine;
            this._sereServTeins = results;
            this._sereServExt = sereServExt;
        }
    }
}
