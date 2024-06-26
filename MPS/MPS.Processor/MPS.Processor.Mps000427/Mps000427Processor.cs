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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000427.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000427
{
    public class Mps000427Processor : AbstractProcessor
    {
        Mps000427PDO rdo;
        List<Type> _Types = null;

        public Mps000427Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000427PDO)rdoBase;
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
                try
                {
                    //byte[] image = ReadFile(rdo._Stream);

                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.CHARTTem, rdo._StreamTem.ToArray()));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.CHARTPUL, rdo._StreamPul.ToArray()));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.CHARTPUL_AND_CHARTTem, rdo._StreamPul__StreamTem.ToArray()));

                    AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._HisTreatment);
                    AddObjectKeyIntoListkey<HIS_CARE>(rdo._hisCare);
                    AddObjectKeyIntoListkey<List<HIS_DHST>>(rdo._lstDHST);

                    #region setkeydate
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day1, rdo.Days[0]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day2, rdo.Days[1]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day3, rdo.Days[2]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day4, rdo.Days[3]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day5, rdo.Days[4]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day6, rdo.Days[5]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day7, rdo.Days[6]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day8, rdo.Days[7]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day9, rdo.Days[8]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day10, rdo.Days[9]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day11, rdo.Days[10]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.Day12, rdo.Days[11]));
                    #endregion

                    #region setkeySPO
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO21, rdo.SPO[0]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO22, rdo.SPO[1]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO23, rdo.SPO[2]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO24, rdo.SPO[3]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO25, rdo.SPO[4]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO26, rdo.SPO[5]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO27, rdo.SPO[6]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO28, rdo.SPO[7]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO29, rdo.SPO[8]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO210, rdo.SPO[9]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO211, rdo.SPO[10]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.SPO212, rdo.SPO[11]));
                    #endregion

                    #region setkeyTHO
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO1, rdo.THO[0]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO2, rdo.THO[1]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO3, rdo.THO[2]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO4, rdo.THO[3]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO5, rdo.THO[4]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO6, rdo.THO[5]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO7, rdo.THO[6]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO8, rdo.THO[7]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO9, rdo.THO[8]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO10, rdo.THO[9]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO11, rdo.THO[10]));
                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.THO12, rdo.THO[11]));
                    #endregion

                    #region setkeyBloodPressure
                    List<String> BloodPressure = new List<string>() { Mps000427ExtendSingleKey.BloodPressure1, Mps000427ExtendSingleKey.BloodPressure2, Mps000427ExtendSingleKey.BloodPressure3, Mps000427ExtendSingleKey.BloodPressure4, Mps000427ExtendSingleKey.BloodPressure5, Mps000427ExtendSingleKey.BloodPressure6, Mps000427ExtendSingleKey.BloodPressure7, Mps000427ExtendSingleKey.BloodPressure8, Mps000427ExtendSingleKey.BloodPressure9, Mps000427ExtendSingleKey.BloodPressure10, Mps000427ExtendSingleKey.BloodPressure11, Mps000427ExtendSingleKey.BloodPressure12 };

                    int demBloodPressure = 0;
                    for (int i = 0; i < rdo.BloodPressureMax.Count; i++)
                    {
                        if (demBloodPressure < 12)
                        {
                            string BloodPressure0 = "";

                            if (rdo.BloodPressureMax[demBloodPressure] == null && rdo.BloodPressureMin[demBloodPressure] == null)
                            {
                                BloodPressure0 = "";
                            }
                            else
                            {
                                BloodPressure0 = rdo.BloodPressureMax[demBloodPressure] + "/" + rdo.BloodPressureMin[demBloodPressure];
                            }
                            SetSingleKey(new KeyValue(BloodPressure[demBloodPressure], BloodPressure0));
                        }
                        else { break; }
                        demBloodPressure++;
                    }

                    #endregion

                    #region setkeyDichVao
                    List<String> DT = new List<string>() { Mps000427ExtendSingleKey.DT1, Mps000427ExtendSingleKey.DT2, Mps000427ExtendSingleKey.DT3, Mps000427ExtendSingleKey.DT4, Mps000427ExtendSingleKey.DT5, Mps000427ExtendSingleKey.DT6, Mps000427ExtendSingleKey.DT7, Mps000427ExtendSingleKey.DT8, Mps000427ExtendSingleKey.DT9, Mps000427ExtendSingleKey.DT10, Mps000427ExtendSingleKey.DT11, Mps000427ExtendSingleKey.DT12 };

                    List<String> EAT = new List<string>() { Mps000427ExtendSingleKey.EAT1, Mps000427ExtendSingleKey.EAT2, Mps000427ExtendSingleKey.EAT3, Mps000427ExtendSingleKey.EAT4, Mps000427ExtendSingleKey.EAT5, Mps000427ExtendSingleKey.EAT6, Mps000427ExtendSingleKey.EAT7, Mps000427ExtendSingleKey.EAT8, Mps000427ExtendSingleKey.EAT9, Mps000427ExtendSingleKey.EAT10, Mps000427ExtendSingleKey.EAT11, Mps000427ExtendSingleKey.EAT12 };

                    List<String> OTHER = new List<string>() { Mps000427ExtendSingleKey.OTHER1, Mps000427ExtendSingleKey.OTHER2, Mps000427ExtendSingleKey.OTHER3, Mps000427ExtendSingleKey.OTHER4, Mps000427ExtendSingleKey.OTHER5, Mps000427ExtendSingleKey.OTHER6, Mps000427ExtendSingleKey.OTHER7, Mps000427ExtendSingleKey.OTHER8, Mps000427ExtendSingleKey.OTHER9, Mps000427ExtendSingleKey.OTHER10, Mps000427ExtendSingleKey.OTHER11, Mps000427ExtendSingleKey.OTHER12 };

                        int demDichVao = 0;
                        foreach (var item in rdo.DichVao)
                        {
                            if (demDichVao<12)
                            {
                                if (item == 1)
                                {
                                    SetSingleKey(new KeyValue(DT[demDichVao], "X"));
                                }
                                else if (item == 2)
                                {
                                    SetSingleKey(new KeyValue(EAT[demDichVao], "X"));
                                }
                                else if (item == 3)
                                {
                                    SetSingleKey(new KeyValue(OTHER[demDichVao], "X"));
                                }
                                else 
                                {
                                    SetSingleKey(new KeyValue(DT[demDichVao], ""));
                                    SetSingleKey(new KeyValue(EAT[demDichVao], ""));
                                    SetSingleKey(new KeyValue(OTHER[demDichVao], ""));
                                }
                            }
                            else { break; }
                            demDichVao++;
                        }
                    #endregion

                    #region setkeyDichRa
                    List<String> NT = new List<string>() { Mps000427ExtendSingleKey.NT1, Mps000427ExtendSingleKey.NT2, Mps000427ExtendSingleKey.NT3, Mps000427ExtendSingleKey.NT4, Mps000427ExtendSingleKey.NT5, Mps000427ExtendSingleKey.NT6, Mps000427ExtendSingleKey.NT7, Mps000427ExtendSingleKey.NT8, Mps000427ExtendSingleKey.NT9, Mps000427ExtendSingleKey.NT10, Mps000427ExtendSingleKey.NT11, Mps000427ExtendSingleKey.NT12 };

                    List<String> DL = new List<string>() { Mps000427ExtendSingleKey.DL1, Mps000427ExtendSingleKey.DL2, Mps000427ExtendSingleKey.DL3, Mps000427ExtendSingleKey.DL4, Mps000427ExtendSingleKey.DL5, Mps000427ExtendSingleKey.DL6, Mps000427ExtendSingleKey.DL7, Mps000427ExtendSingleKey.DL8, Mps000427ExtendSingleKey.DL9, Mps000427ExtendSingleKey.DL10, Mps000427ExtendSingleKey.DL11, Mps000427ExtendSingleKey.DL12 };

                    List<String> Feces = new List<string>() { Mps000427ExtendSingleKey.Feces1, Mps000427ExtendSingleKey.Feces2, Mps000427ExtendSingleKey.Feces3, Mps000427ExtendSingleKey.Feces4, Mps000427ExtendSingleKey.Feces5, Mps000427ExtendSingleKey.Feces6, Mps000427ExtendSingleKey.Feces7, Mps000427ExtendSingleKey.Feces8, Mps000427ExtendSingleKey.Feces9, Mps000427ExtendSingleKey.Feces10, Mps000427ExtendSingleKey.Feces11, Mps000427ExtendSingleKey.Feces12 };

                        int demDichRa = 0;
                        foreach (var item in rdo.DichRa)
                        {
                            if (demDichRa < 12)
                            {
                                if (item == 1)
                                {
                                    SetSingleKey(new KeyValue(NT[demDichRa], "X"));
                                }
                                else if (item == 2)
                                {
                                    SetSingleKey(new KeyValue(DL[demDichRa], "X"));
                                }
                                else if (item == 3)
                                {
                                    SetSingleKey(new KeyValue(Feces[demDichRa], "X"));
                                }
                                else
                                {
                                    SetSingleKey(new KeyValue(NT[demDichRa], ""));
                                    SetSingleKey(new KeyValue(DL[demDichRa], ""));
                                    SetSingleKey(new KeyValue(Feces[demDichRa], ""));
                                }
                            }
                            else { break; }
                            demDichRa++;
                    }
                    #endregion

                    HIS_EXECUTE_ROOM ExecuteRoomData = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.ID == rdo._hisCare.EXECUTE_ROOM_ID).FirstOrDefault();

                    V_HIS_TREATMENT_BED_ROOM TreatmentBedRoomData = BackendDataWorker.Get<V_HIS_TREATMENT_BED_ROOM>().Where(o => o.TREATMENT_ID == rdo._hisCare.TREATMENT_ID).FirstOrDefault();
                        
                    //SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.REQUEST_DEPARTMENT_NAME, rdo._WorkPlace.DepartmentName));

                    Inventec.Common.Logging.LogSystem.Warn("EXECUTE_ROOM_ID: " + rdo._hisCare.EXECUTE_ROOM_ID);
                    Inventec.Common.Logging.LogSystem.Info("dữ liệu HIS_EXECUTE_ROOM: " +Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ExecuteRoomData), ExecuteRoomData));
                    Inventec.Common.Logging.LogSystem.Info("dữ liệu V_HIS_TREATMENT_BED_ROOM: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TreatmentBedRoomData), TreatmentBedRoomData));

                    string ExecuteRoomCode = "";
                    if (ExecuteRoomData != null)
                    {
                        ExecuteRoomCode = ExecuteRoomData.EXECUTE_ROOM_CODE;
                    }

                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.ROOM_CODE, ExecuteRoomCode));

                    string BedName = "";
                    if (TreatmentBedRoomData != null)
                    {
                        BedName = TreatmentBedRoomData.BED_CODE;
                    }

                    SetSingleKey(new KeyValue(Mps000427ExtendSingleKey.BED_CODE, BedName));
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }


    }
}
