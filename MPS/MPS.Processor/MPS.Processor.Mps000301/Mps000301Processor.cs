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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000301.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000301
{
    class Mps000301Processor : AbstractProcessor
    {
        Mps000301PDO rdo;

        public Mps000301Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000301PDO)rdoBase;
        }
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetBarcodeKey();
                SetSingleKey();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "ServiceCDHA", _ADO_CDHAs);
                objectTag.AddObjectData(store, "ServiceG", _ADO_Gs);
                objectTag.AddObjectData(store, "ServiceGPBL", _ADO_GPBLs);
                objectTag.AddObjectData(store, "ServiceKH", _ADO_KHs);
                objectTag.AddObjectData(store, "ServiceKHAC", _ADO_KHACs);
                objectTag.AddObjectData(store, "ServiceMAU", _ADO_MAUs);
                objectTag.AddObjectData(store, "ServiceNS", _ADO_NSs);
                objectTag.AddObjectData(store, "ServicePHCN", _ADO_PHCNs);
                objectTag.AddObjectData(store, "ServicePT", _ADO_PTs);
                objectTag.AddObjectData(store, "ServiceSA", _ADO_SAs);
                objectTag.AddObjectData(store, "ServiceTDCN", _ADO_TDCNs);
                objectTag.AddObjectData(store, "ServiceTHUOC", _ADO_THUOCs);
                objectTag.AddObjectData(store, "ServiceTT", _ADO_TTs);
                objectTag.AddObjectData(store, "ServiceVT", _ADO_VTs);
                objectTag.AddObjectData(store, "ServiceXN", _ADO_XNs);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        List<Mps000301BySereServ> _ADO_CDHAs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_Gs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_GPBLs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_KHs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_KHACs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_MAUs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_NSs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_PHCNs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_PTs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_SAs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_TDCNs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_THUOCs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_TTs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_VTs = new List<Mps000301BySereServ>();
        List<Mps000301BySereServ> _ADO_XNs = new List<Mps000301BySereServ>();

        private void SetSingleKey()
        {
            try
            {
                #region ---- SetSingleKeyDateTime
                if (rdo._Mps000301ADOs != null && rdo._Mps000301ADOs.Count > 0)
                {
                    foreach (var item in rdo._Mps000301ADOs)
                    {
                        #region ---Day---
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day1, item.Day1));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day2, item.Day2));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day3, item.Day3));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day4, item.Day4));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day5, item.Day5));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day6, item.Day6));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day7, item.Day7));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day8, item.Day8));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day9, item.Day9));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day10, item.Day10));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day11, item.Day11));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day12, item.Day12));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day13, item.Day13));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day14, item.Day14));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day15, item.Day15));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day16, item.Day16));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day17, item.Day17));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day18, item.Day18));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day19, item.Day19));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day20, item.Day20));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day21, item.Day21));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day22, item.Day22));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day23, item.Day23));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.Day24, item.Day24));
                        #endregion

                        #region ---Day and year---
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear1, item.DayAndYear1));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear2, item.DayAndYear2));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear3, item.DayAndYear3));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear4, item.DayAndYear4));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear5, item.DayAndYear5));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear6, item.DayAndYear6));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear7, item.DayAndYear7));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear8, item.DayAndYear8));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear9, item.DayAndYear9));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear10, item.DayAndYear10));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear11, item.DayAndYear11));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear12, item.DayAndYear12));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear13, item.DayAndYear13));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear14, item.DayAndYear14));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear15, item.DayAndYear15));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear16, item.DayAndYear16));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear17, item.DayAndYear17));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear18, item.DayAndYear18));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear19, item.DayAndYear19));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear20, item.DayAndYear20));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear21, item.DayAndYear21));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear22, item.DayAndYear22));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear23, item.DayAndYear23));
                        SetSingleKey(new KeyValue(Mps000301ExtendSingleKey.DayAndYear24, item.DayAndYear24));

                        #endregion
                    }
                }
                #endregion

                _ADO_CDHAs = new List<Mps000301BySereServ>();
                _ADO_Gs = new List<Mps000301BySereServ>();
                _ADO_GPBLs = new List<Mps000301BySereServ>();
                _ADO_KHs = new List<Mps000301BySereServ>();
                _ADO_KHACs = new List<Mps000301BySereServ>();
                _ADO_MAUs = new List<Mps000301BySereServ>();
                _ADO_NSs = new List<Mps000301BySereServ>();
                _ADO_PHCNs = new List<Mps000301BySereServ>();
                _ADO_PTs = new List<Mps000301BySereServ>();
                _ADO_SAs = new List<Mps000301BySereServ>();
                _ADO_TDCNs = new List<Mps000301BySereServ>();
                _ADO_THUOCs = new List<Mps000301BySereServ>();
                _ADO_TTs = new List<Mps000301BySereServ>();
                _ADO_VTs = new List<Mps000301BySereServ>();
                _ADO_XNs = new List<Mps000301BySereServ>();

                if (rdo._Mps000301BySereServs != null && rdo._Mps000301BySereServs.Count > 0)
                {
                    foreach (var item in rdo._Mps000301BySereServs)
                    {
                        if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                        {
                            _ADO_CDHAs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                        {
                            _ADO_Gs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL)
                        {
                            _ADO_GPBLs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                        {
                            _ADO_KHs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
                        {
                            _ADO_KHACs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                        {
                            _ADO_MAUs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS)
                        {
                            _ADO_NSs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN)
                        {
                            _ADO_PHCNs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                        {
                            _ADO_PTs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                        {
                            _ADO_SAs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                        {
                            _ADO_TDCNs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            _ADO_THUOCs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                        {
                            _ADO_TTs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            _ADO_VTs.Add(item);
                        }
                        else if (item.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                        {
                            _ADO_XNs.Add(item);
                        }
                    }
                }

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey((new KeyValue(Mps000301ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.IN_TIME))));
                    if (rdo.currentTreatment.OUT_TIME != null)
                        SetSingleKey((new KeyValue(Mps000301ExtendSingleKey.OUT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.OUT_TIME ?? 0))));
                    if (rdo.currentTreatment.CLINICAL_IN_TIME != null)
                        SetSingleKey((new KeyValue(Mps000301ExtendSingleKey.CLINICAL_IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentTreatment.CLINICAL_IN_TIME ?? 0))));
                    SetSingleKey((new KeyValue(Mps000301ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.currentTreatment.TDL_PATIENT_DOB))));
                }
                AddObjectKeyIntoListkey<SingleKeys>(rdo._SingleKeys);

                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000301ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
