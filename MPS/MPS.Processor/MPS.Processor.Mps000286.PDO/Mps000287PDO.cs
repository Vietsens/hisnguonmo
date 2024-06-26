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

namespace MPS.Processor.Mps000287.PDO
{
    public partial class Mps000287PDO : RDOBase
    {
        public MOS.SDO.WorkPlaceSDO _WorkPlace { get; set; }
        public List<Mps000287ADO> _Mps000287ADOs { get; set; }
        public long _IntructionTime { get; set; }
        public V_HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom { get; set; }
        public MemoryStream _StreamTem { get; set; }
        public MemoryStream _StreamPul { get; set; }
        public MemoryStream _StreamPul__StreamTem { get; set; }
        public List<String> Days { get; set; }
        public List<Decimal?> HEIGHTs { get; set; }
        public List<Decimal?> Weights { get; set; }
        public List<Decimal?> BREATHs { get; set; }
        public List<String> BloodPressures { get; set; }
        public List<Decimal?> Spos { get; set; }
        public List<Decimal?> Pulses { get; set; }
        public List<Decimal?> Temperatures { get; set; }
        public List<String> Notes { get; set; }

        public Mps000287PDO(
            V_HIS_TREATMENT _Treatment,
            List<Mps000287ADO> _mps000287ADOs,
            MOS.SDO.WorkPlaceSDO _workPlace,
            V_HIS_TREATMENT_BED_ROOM _treatmentBedRoom,
            MemoryStream _streamTem,
            MemoryStream _streamPul,
             MemoryStream _streamPul_streamTem
            )
        {
            try
            {
                this._Treatment = _Treatment;
                this._Mps000287ADOs = _mps000287ADOs;
                this._WorkPlace = _workPlace;
                this._TreatmentBedRoom = _treatmentBedRoom;
                this.Days = _mps000287ADOs.Select(o => o.EXECUTE_TIME).ToList();
                this.HEIGHTs = _mps000287ADOs.Select(o => o.HEIGHT).ToList();
                this.Weights = _mps000287ADOs.Select(o => o.Weight).ToList();
                this.BREATHs = _mps000287ADOs.Select(o => o.BREATH).ToList();
                this.Spos = _mps000287ADOs.Select(o => o.SPO2).ToList();
                this.Pulses = _mps000287ADOs.Select(o => o.PULSE).ToList();
                this.Notes = _mps000287ADOs.Select(o => o.NOTE).ToList();
                this.Temperatures = _mps000287ADOs.Select(o => o.TEMPERATURE).ToList();
                this.BloodPressures = _mps000287ADOs.Select(o => o.BloodPressure).ToList();
                this._StreamTem = _streamTem;
                this._StreamPul = _streamPul;
                this._StreamPul__StreamTem = _streamPul_streamTem;
                int numDays = this.Days.Count;
                if (this.Days.Count < 10)
                {
                    for (int i = 0; i < 10 - numDays; i++)
                    {
                        this.Days.Add("");
                    }
                }

                int numHeis = this.HEIGHTs.Count;
                if (this.HEIGHTs.Count < 10)
                {
                    for (int i = 0; i < 10 - numHeis; i++)
                    {
                        this.HEIGHTs.Add(null);
                    }
                }
                int munWeis = this.Weights.Count;
                if (this.Weights.Count < 10)
                {
                    for (int i = 0; i < 10 - munWeis; i++)
                    {
                        this.Weights.Add(null);
                    }
                }
                int numBres = this.BREATHs.Count;
                if (this.BREATHs.Count < 10)
                {
                    for (int i = 0; i < 10 - numBres; i++)
                    {
                        this.BREATHs.Add(null);
                    }
                }

                int numBloodPressures = this.BloodPressures.Count;
                if (this.BloodPressures.Count < 10)
                {
                    for (int i = 0; i < 10 - numBloodPressures; i++)
                    {
                        this.BloodPressures.Add("");
                    }
                }
                int NumNote = this.Notes.Count;
                if (this.Notes.Count < 10)
                {
                    for (int i = 0; i < 10 - NumNote; i++)
                    {
                        this.Notes.Add("");
                    }
                }

                int spos = this.Spos.Count;
                if (this.Spos.Count < 10)
                {
                    for (int i = 0; i < 10 - spos; i++)
                    {
                        this.Spos.Add(null);
                    }
                }
                int pulses = this.Pulses.Count;
                if (this.Pulses.Count < 10)
                {
                    for (int i = 0; i < 10 - pulses; i++)
                    {
                        this.Pulses.Add(null);
                    }
                }
                int temperatures = this.Temperatures.Count;
                if (this.Temperatures.Count < 10)
                {
                    for (int i = 0; i < 10 - temperatures; i++)
                    {
                        this.Temperatures.Add(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class Mps000287ADO
    {
        public string EXECUTE_TIME { get; set; }
        public decimal? HEIGHT { get; set; }
        public decimal? Weight { get; set; }
        public decimal? BREATH { get; set; }
        public decimal? SPO2 { get; set; }
        public decimal? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public string NOTE { get; set; }
        public String BloodPressure { get; set; }

        public Mps000287ADO(V_HIS_DHST dhst)
        {
            if (dhst.EXECUTE_TIME != null)
            {
                this.EXECUTE_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dhst.EXECUTE_TIME.ToString());
            }
            else
            {
                this.EXECUTE_TIME = "";
            }

            if (dhst.HEIGHT != null)
            {
                this.HEIGHT = dhst.HEIGHT;
            }
            else
            {
                this.HEIGHT = null;
            }
            if (dhst.WEIGHT != null)
            {
                this.Weight = dhst.WEIGHT;
            }
            else
                this.Weight = null;
            if (dhst.BREATH_RATE != null)
            {
                this.BREATH = dhst.BREATH_RATE;
            }
            else
            {
                this.BREATH = null;
            }
            if (dhst.SPO2 != null)
            {
                this.SPO2 = dhst.SPO2;
            }
            else
            {
                this.SPO2 = null;
            }
            if (dhst.PULSE != null)
            {
                this.PULSE = dhst.PULSE;
            }
            else
            {
                this.PULSE = null;
            }
            if (dhst.TEMPERATURE != null)
            {
                this.TEMPERATURE = dhst.TEMPERATURE;
            }
            else
            {
                this.TEMPERATURE = null;
            }
            this.BloodPressure = dhst.BLOOD_PRESSURE_MAX+"/"+dhst. BLOOD_PRESSURE_MIN;
            this.NOTE = dhst.NOTE;
        }
        public Mps000287ADO() { }
    }
}
