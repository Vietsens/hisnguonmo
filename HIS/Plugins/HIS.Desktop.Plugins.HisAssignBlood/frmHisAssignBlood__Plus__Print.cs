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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisAssignBlood.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAssignBlood
{
    public partial class frmHisAssignBlood : HIS.Desktop.Utility.FormBase
    {
        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menuMedicine = new DXPopupMenu();
                DXMenuItem itemDonThuocVatTu = new DXMenuItem(Inventec.Common.Resource.Get.Value("frmHisAssignBlood.btnPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), new EventHandler(OnClickInChiDinhDichVu));
                itemDonThuocVatTu.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108;
                menuMedicine.Items.Add(itemDonThuocVatTu);

                dropDownPrintBlood.DropDownControl = menuMedicine;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInChiDinhDichVu(object sender, EventArgs e)
        {
            try
            {              
                var btnItem = sender as DXMenuItem;
                string type = (string)(btnItem.Tag);
                switch (type)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108:
                        SetUpToPrint108(false);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108:
                        this.currentTypeCode108 = printTypeCode;
                        this.currentFileName108 = fileName;
                        InPhieuYeuCauChiDinhMau108(printTypeCode, fileName, ref result, this.HisPrescriptionSDOResultPrint);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InPhieuYeuCauChiDinhMau108(string printTypeCode, string fileName, ref bool result, PatientBloodPresResultSDO currentPrint)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                var treatment = LoadDataToCurrentTreatmentData(treatmentId);

                if (currentPrint.ServiceReq != null)
                {
                    MOS.Filter.HisServiceReqViewFilter serviceReqViewFilter = new HisServiceReqViewFilter();
                    serviceReqViewFilter.ID = currentPrint.ServiceReq.ID;
                    hisServiceReqPrint = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceReqViewFilter, param).FirstOrDefault();
                }


                MOS.Filter.HisExpMestBltyReqView1Filter expMestMetyFilter = new MOS.Filter.HisExpMestBltyReqView1Filter();
                expMestMetyFilter.EXP_MEST_ID = currentPrint.ExpMest.ID;
                var expMestMeties = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLTY_REQ_1>>(RequestUriStore.HIS_EXP_MEST_BLTY_REQ__GETVIEW1, ApiConsumers.MosConsumer, expMestMetyFilter, ProcessLostToken, param);

                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_ID = currentPrint.ExpMest.ID;
                var _ExpMestBloods_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);

                HisTreatmentBedRoomViewFilter tbrFilter = new HisTreatmentBedRoomViewFilter();
                tbrFilter.TREATMENT_ID = treatmentId;
                tbrFilter.IS_IN_ROOM = true;
                var TreatmentBerRoomPrint = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, tbrFilter, param);

                HisSereServView1Filter ss1Filter = new HisSereServView1Filter();
                ss1Filter.SERVICE_REQ_PARENT_ID = hisServiceReqPrint.ID;
                var SereServPrint = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>("api/HisSereServ/GetView1", ApiConsumers.MosConsumer, ss1Filter, param);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, currentModule.RoomId);

                EmrDataStore.treatmentCode = treatment != null ? treatment.TREATMENT_CODE : "";

                MPS.Processor.Mps000108.PDO.Mps000108PDO mps000108RDO = new MPS.Processor.Mps000108.PDO.Mps000108PDO(
                    currentPrint.ExpMest,
                    expMestMeties,
                    treatment,
                    hisServiceReqPrint,
                    _ExpMestBloods_Print,
                    TreatmentBerRoomPrint,
                    SereServPrint
                );
                if (this.isSaveAndPrint)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, currentFileName108, mps000108RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName){ EmrInputADO = inputADO });
                }
                else
                {                  
                    this.total108 += SereServPrint.Count();
                    Print.PrintData(printTypeCode, fileName, mps000108RDO, SereServPrint.Count(), currentModule.RoomId, SetDataGroup);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void CancelChooseTemplate(string printTypeCode)
        {
            try
            {
                this.CancelPrint = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataGroup(int count, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            try
            {
                this.CountSereServPrinted += count;
                if (data != null)
                {
                    if (this.GroupStreamPrint == null)
                    {
                        this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    }

                    this.GroupStreamPrint.Add(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintMerge()
        {
            try
            {
                int countTimeOut = 0;
                //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
                while (this.TotalPrint != this.CountSereServPrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
                {
                    Thread.Sleep(50);
                    countTimeOut++;
                }

                if (countTimeOut > TIME_OUT_PRINT_MERGE)
                {
                    throw new Exception("TimeOut");
                }
                if (CancelPrint)
                {
                    throw new Exception("Cancel Print");
                }

                Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = this.GroupStreamPrint.First();

                Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                    adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                printProcess.SetPartialFile(this.GroupStreamPrint);
                if (!isSaveAndPrint)
                    printProcess.PrintPreviewShow();
                else
                    printProcess.Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
