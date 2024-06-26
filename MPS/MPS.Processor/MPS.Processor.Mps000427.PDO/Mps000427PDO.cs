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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000427.PDO
{
    public partial class Mps000427PDO : RDOBase
    {
        public HIS_CARE _hisCare { get; set; }
        public HIS_TREATMENT _HisTreatment { get; set; }
        public List<HIS_DHST> _lstDHST { get; set; }
        public MemoryStream _StreamTem { get; set; }
        public MemoryStream _StreamPul { get; set; }
        public MemoryStream _StreamPul__StreamTem { get; set; }


        public List<long?> Days { get; set; }
        public List<long?> BloodPressureMin { get; set; }
        public List<long?> BloodPressureMax { get; set; }
        public List<Decimal?> SPO { get; set; }
        public List<Decimal?> THO { get; set; }
        public List<short?> DichVao { get; set; }
        public List<short?> DichRa { get; set; }


        public Mps000427PDO(
            HIS_CARE _hisCare,
            HIS_TREATMENT _HisTreatment,
            List<HIS_DHST> _lstDHST,
            MemoryStream _streamTem,
            MemoryStream _streamPul,
             MemoryStream _streamPul_streamTem
            ) 
        {
            try
            {
                this._hisCare = _hisCare;
                this._HisTreatment = _HisTreatment;
                this._lstDHST = _lstDHST;
                this._StreamTem = _streamTem;
                this._StreamPul = _streamPul;
                this._StreamPul__StreamTem = _streamPul_streamTem;

                this.Days = this._lstDHST.Select(o => o.EXECUTE_TIME).ToList();
                this.BloodPressureMin = this._lstDHST.Select(o => o.BLOOD_PRESSURE_MIN).ToList();
                this.BloodPressureMax = this._lstDHST.Select(o => o.BLOOD_PRESSURE_MAX).ToList();
                this.SPO = this._lstDHST.Select(o => o.SPO2).ToList();
                this.THO = this._lstDHST.Select(o => o.BREATH_RATE).ToList();
                this.DichVao = this._lstDHST.Select(o => o.INFUTION_INTO).ToList();
                this.DichRa = this._lstDHST.Select(o => o.INFUTION_OUT).ToList();

                int numDays = this.Days.Count;
                if (this.Days.Count < 12)
                {
                    for (int i = 0; i < 12 - numDays; i++)
                    {
                        this.Days.Add(null);
                    }
                }

                int numBloodPressureMin = this.BloodPressureMin.Count;
                if (this.BloodPressureMin.Count < 12)
                {
                    for (int i = 0; i < 12 - numBloodPressureMin; i++)
                    {
                        this.BloodPressureMin.Add(null);
                    }
                }

                int numBloodPressureMax = this.BloodPressureMax.Count;
                if (this.BloodPressureMax.Count < 12)
                {
                    for (int i = 0; i < 12 - numBloodPressureMax; i++)
                    {
                        this.BloodPressureMax.Add(null);
                    }
                }

                int numSPO = this.SPO.Count;
                if (this.SPO.Count < 12)
                {
                    for (int i = 0; i < 12 - numSPO; i++)
                    {
                        this.SPO.Add(null);
                    }
                }

                int numTHO = this.THO.Count;
                if (this.THO.Count < 12)
                {
                    for (int i = 0; i < 12 - numTHO; i++)
                    {
                        this.THO.Add(null);
                    }
                }

                int numDichVao = this.DichVao.Count;
                if (this.DichVao.Count < 12)
                {
                    for (int i = 0; i < 12 - numDichVao; i++)
                    {
                        this.DichVao.Add(null);
                    }
                }

                int numDichRa = this.DichRa.Count;
                if (this.DichRa.Count < 12)
                {
                    for (int i = 0; i < 12 - numDichRa; i++)
                    {
                        this.DichRa.Add(null);
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("dữ liệu _hisCare: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _hisCare), _hisCare));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu _HisTreatment: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _HisTreatment), _HisTreatment));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu _lstDHST: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _lstDHST), _lstDHST));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu Days: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Days), Days));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu BloodPressureMin: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => BloodPressureMin), BloodPressureMin));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu BloodPressureMax: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => BloodPressureMax), BloodPressureMax));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu SPO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SPO), SPO));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu THO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => THO), THO));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu DichVao: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DichVao), DichVao));
                Inventec.Common.Logging.LogSystem.Info("dữ liệu DichRa: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DichRa), DichRa));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }
    }
}
