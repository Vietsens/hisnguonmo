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
using FlexCel.Report;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000287.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000287
{
    public class Mps000287Processor : AbstractProcessor
    {
        Mps000287PDO rdo;
        List<Type> _Types = null;

        public Mps000287Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000287PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                //Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                //byte[] image = ReadFile(rdo._Stream);
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.CHARTTem, rdo._StreamTem.ToArray()));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.CHARTPUL, rdo._StreamPul.ToArray()));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.CHARTPUL_AND_CHARTTem, rdo._StreamPul__StreamTem.ToArray()));
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo._Treatment);

                #region setkeydate
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day1, rdo.Days[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day2, rdo.Days[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day3, rdo.Days[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day4, rdo.Days[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day5, rdo.Days[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day6, rdo.Days[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day7, rdo.Days[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day8, rdo.Days[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day9, rdo.Days[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.Day10, rdo.Days[9]));
                #endregion

                #region setkeyHeight
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT1, rdo.HEIGHTs[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT2, rdo.HEIGHTs[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT3, rdo.HEIGHTs[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT4, rdo.HEIGHTs[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT5, rdo.HEIGHTs[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT6, rdo.HEIGHTs[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT7, rdo.HEIGHTs[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT8, rdo.HEIGHTs[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT9, rdo.HEIGHTs[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.HEIGHT10, rdo.HEIGHTs[9]));
                #endregion

                #region setkeyWeight
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT1, rdo.Weights[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT2, rdo.Weights[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT3, rdo.Weights[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT4, rdo.Weights[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT5, rdo.Weights[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT6, rdo.Weights[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT7, rdo.Weights[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT8, rdo.Weights[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT9, rdo.Weights[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.WEIGHT10, rdo.Weights[9]));
                #endregion
                #region setkeyBreath
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH1, rdo.BREATHs[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH2, rdo.BREATHs[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH3, rdo.BREATHs[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH4, rdo.BREATHs[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH5, rdo.BREATHs[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH6, rdo.BREATHs[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH7, rdo.BREATHs[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH8, rdo.BREATHs[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH9, rdo.BREATHs[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BREATH10, rdo.BREATHs[9]));
                #endregion

                #region setkeyBreath
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure1, rdo.BloodPressures[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure2, rdo.BloodPressures[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure3, rdo.BloodPressures[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure4, rdo.BloodPressures[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure5, rdo.BloodPressures[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure6, rdo.BloodPressures[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure7, rdo.BloodPressures[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure8, rdo.BloodPressures[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure9, rdo.BloodPressures[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BloodPressure10, rdo.BloodPressures[9]));
                #endregion
                #region SPO
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO21, rdo.Spos[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO22, rdo.Spos[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO23, rdo.Spos[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO24, rdo.Spos[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO25, rdo.Spos[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO26, rdo.Spos[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO27, rdo.Spos[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO28, rdo.Spos[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO29, rdo.Spos[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.SPO210, rdo.Spos[9]));
                #endregion
                #region PULSE
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE1, rdo.Pulses[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE2, rdo.Pulses[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE3, rdo.Pulses[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE4, rdo.Pulses[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE5, rdo.Pulses[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE6, rdo.Pulses[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE7, rdo.Pulses[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE8, rdo.Pulses[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE9, rdo.Pulses[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.PULSE10, rdo.Pulses[9]));
                #endregion

                #region NOTE
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE1, rdo.Notes[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE2, rdo.Notes[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE3, rdo.Notes[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE4, rdo.Notes[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE5, rdo.Notes[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE6, rdo.Notes[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE7, rdo.Notes[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE8, rdo.Notes[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE9, rdo.Notes[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.NOTE10, rdo.Notes[9]));
                #endregion
                #region TEMPERATURE
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE1, rdo.Temperatures[0]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE2, rdo.Temperatures[1]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE3, rdo.Temperatures[2]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE4, rdo.Temperatures[3]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE5, rdo.Temperatures[4]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE6, rdo.Temperatures[5]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE7, rdo.Temperatures[6]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE8, rdo.Temperatures[7]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE9, rdo.Temperatures[8]));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.TEMPERATURE10, rdo.Temperatures[9]));
                #endregion

                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.REQUEST_DEPARTMENT_NAME, rdo._WorkPlace.DepartmentName));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.ROOM_NAME, rdo._WorkPlace.RoomName));
                SetSingleKey(new KeyValue(Mps000287ExtendSingleKey.BED_NAME, rdo._TreatmentBedRoom.BED_NAME));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    class Type
    {
        public string SERVICE_TYPE_NAME { get; set; }
        public long ID { get; set; }
    }

    class CalculateMergerData : TFlexCelUserFunction
    {
        long typeId = 0;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                long servicetypeId = Convert.ToInt64(parameters[0]);

                if (servicetypeId > 0)
                {
                    if (this.typeId == servicetypeId)
                    {
                        return true;
                    }
                    else
                    {
                        this.typeId = servicetypeId;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
