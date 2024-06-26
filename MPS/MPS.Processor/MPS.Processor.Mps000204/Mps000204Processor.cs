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
using MPS.Processor.Mps000204.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000204
{
    public class Mps000204Processor : AbstractProcessor
    {
        Mps000204PDO rdo;
        DataTable ExecuteRole;
        public Mps000204Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000204PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TDL_PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.IncludeLabel = false;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000204ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo.currentTreatment.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000204ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatmentCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                //Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                //Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                //Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();


                SetBarcodeKey();
                SetSingleKey();
                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                //store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                //singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                //objectTag.AddObjectData(store, "User", rdo.listEkipUser);

                //tự sinh key vai trò

                if (rdo.HisExecuteRoles != null && rdo.HisExecuteRoles.Count > 0)
                {
                     this.ExecuteRole = new DataTable();

                    DataRow _ravi = ExecuteRole.NewRow();//khởi tạo 1 hàng trong DataTable

                    foreach (var item in rdo.HisExecuteRoles)
                    {
                        List<V_HIS_EKIP_USER> hisEkipPlanUser = rdo.listEkipUser.Where(o => o.EXECUTE_ROLE_ID == item.ID).ToList();

                        if (hisEkipPlanUser != null && hisEkipPlanUser.Count > 0 && rdo.HisExecuteRoles.Count > 0)
                        {
                            string ColumnName = String.Format("EXECUTE_ROLE_{0}_USERNAME", item.EXECUTE_ROLE_CODE);
                            this.ExecuteRole.Columns.Add(ColumnName);

                            string RowData = "";
                            foreach (var itemU in hisEkipPlanUser)
                            {
                                if (!String.IsNullOrEmpty(RowData))
                                {
                                    RowData += "\n";
                                }
                                RowData += itemU.USERNAME;
                            }

                            _ravi[ColumnName] = RowData;
                            SetSingleKey(new KeyValue(ColumnName, RowData));

                            string ColumnName2 = String.Format("EXECUTE_ROLE_{0}_ROLE_NAME", item.EXECUTE_ROLE_CODE);
                            ExecuteRole.Columns.Add(ColumnName2);

                            _ravi[ColumnName2] = item.EXECUTE_ROLE_NAME;
                            SetSingleKey(new KeyValue(ColumnName2, item.EXECUTE_ROLE_NAME));
                        }
                    }
                    this.ExecuteRole.Rows.Add(_ravi);

                    Inventec.Common.Logging.LogSystem.Info("ProcessData DataTable 1: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ExecuteRole), ExecuteRole));
                    //objectTag.AddObjectData(store, "ExecuteRole", this.ExecuteRole);
                    Inventec.Common.Logging.LogSystem.Warn("ProcessData DataTable 2");
                }

                result = (this.templateType == ProcessorBase.PrintConfig.TemplateType.Excel) ? ProcessDataExcel() : ((this.templateType == ProcessorBase.PrintConfig.TemplateType.Word) ? ProcessDataWord():false);
                //result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private bool ProcessDataExcel()
        {
            bool success = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "User", rdo.listEkipUser);
                objectTag.AddObjectData(store, "ExecuteRole", this.ExecuteRole);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            return success;
        }

        private bool ProcessDataWord()
        {
            bool success = false;
            try
            {
                Inventec.Common.TemplaterExport.ProcessSingleTag singleTag = new Inventec.Common.TemplaterExport.ProcessSingleTag();
                Inventec.Common.TemplaterExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.TemplaterExport.ProcessBarCodeTag();
                Inventec.Common.TemplaterExport.ProcessObjectTag objectTag = new Inventec.Common.TemplaterExport.ProcessObjectTag();


                templaterExportStore.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                barCodeTag.ProcessData(templaterExportStore, dicImage);
                
                singleTag.ProcessData(templaterExportStore, singleValueDictionary);

                objectTag.AddObjectData(templaterExportStore,"User", rdo.listEkipUser);
                success = true;

            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã điều trị: " + rdo.currentServiceReq.TDL_TREATMENT_CODE;
                log += " , Mã yêu cầu: " + rdo.currentServiceReq.SERVICE_REQ_CODE;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.currentServiceReq != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo.currentServiceReq.TDL_TREATMENT_CODE;
                    string serviceReqCode = "SERVICE_REQ_CODE:" + rdo.currentServiceReq.SERVICE_REQ_CODE;
                    string serviceCode = "";
                    if (rdo.SereServs != null && rdo.SereServs.Count > 0)
                    {
                        var serviceFirst = rdo.SereServs.OrderBy(o => o.TDL_SERVICE_CODE).First();
                        serviceCode = "SERVICE_CODE:" + serviceFirst.TDL_SERVICE_CODE;
                    }

                    result = String.Format("{0} {1} {2} {3}", printTypeCode, treatmentCode, serviceReqCode, serviceCode);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<HIS_SERVICE_REQ>(rdo.currentServiceReq, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.pttt, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_1>(rdo.currentSereServ, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.currentTreatment, true);
                AddObjectKeyIntoListkey<HIS_SERE_SERV_EXT>(rdo.sereServExt, false);

                SetSingleKey(new KeyValue(Mps000204ExtendSingleKey.DESCRIPTION_EXT, rdo.sereServExt.DESCRIPTION));

                if (rdo.currentTreatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000204ExtendSingleKey.IN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.currentTreatment.IN_TIME)));
                    SetSingleKey(new KeyValue(Mps000204ExtendSingleKey.OUT_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rdo.currentTreatment.OUT_TIME ?? 0)));
                }

                if (rdo.sereServExt != null)
                {
                    SetSingleKey(new KeyValue(Mps000204ExtendSingleKey.BEGIN_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.sereServExt.BEGIN_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000204ExtendSingleKey.END_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(rdo.sereServExt.END_TIME ?? 0)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
