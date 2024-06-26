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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Exemptions.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Exemptions
{
    public partial class frmExemptions : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        long? treatmentId = null;
        HIS_TREATMENT currentTreatment = null;
        V_HIS_PATIENT_TYPE_ALTER resultPatientType;
        List<V_HIS_SERE_SERV_5> ListSereServ = new List<V_HIS_SERE_SERV_5>();
        List<V_HIS_SERE_SERV_5> currentSereServs = null;
        //List<V_HIS_SERE_SERV_5> ListSereServTranfer;
        Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill = null;
        HIS_SERE_SERV _currentServSer = null;
        bool isReloadTree = false;

        public frmExemptions(Inventec.Desktop.Common.Modules.Module module, HIS_SERE_SERV _sereServ)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.currentModule = module;
                this.spinEditTyLe.EditValue = 100;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                this._currentServSer = _sereServ;
                this.treatmentId = _sereServ.TDL_TREATMENT_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmExemptions(Inventec.Desktop.Common.Modules.Module module, long _treatmentId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.currentModule = module;
                this.spinEditTyLe.EditValue = 100;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                this.treatmentId = _treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmExemptions(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.currentModule = module;
                this.spinEditTyLe.EditValue = 100;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExemptions_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                if (this.treatmentId > 0)
                {
                    LoadSearch();
                }

                this.spinEditMienGiamTreatmnet.EditValue = 100;

                FillInfoPatient(currentTreatment);

                LoadDataToTreeSereServ();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillInfoPatient(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    this.chkMienGiamTreatment.Checked = data.IS_AUTO_DISCOUNT == 1 ? true : false;
                    this.spinEditMienGiamTreatmnet.EditValue = data.AUTO_DISCOUNT_RATIO > 0 ? data.AUTO_DISCOUNT_RATIO * 100 : 100;

                    txtPatient.Text = data.TDL_PATIENT_CODE;
                    txtPatientName.Text = data.TDL_PATIENT_NAME;
                    txtDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    txtGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    txtAddress.Text = data.TDL_PATIENT_ADDRESS;
                    if (data.TDL_PATIENT_TYPE_ID != null)
                    {
                        var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == data.TDL_PATIENT_TYPE_ID);
                        txtPatienType.Text = patientType.PATIENT_TYPE_NAME;
                    }
                    //string InfoPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TransactionBillInfoPatient"); //Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>("HIS.Desktop.Plugins.TransactionBillInfoPatient");
                    //if (InfoPatient == "1")
                    //{
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = currentTreatment.ID;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    resultPatientType = new BackendAdapter(new CommonParam()).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, null);
                    //this.resultPatientType = new BackendAdapter(new CommonParam())
                    //.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, currentTreatment.ID, null);
                    if (resultPatientType != null)
                    {
                        txtHeinCard.Text = TrimHeinCardNumber(resultPatientType.HEIN_CARD_NUMBER);
                        txtHeinFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        txtHeinTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_TO_TIME ?? 0);
                        txtMediOrg.Text = resultPatientType.HEIN_MEDI_ORG_NAME;
                    }
                    //  }
                    else
                    {
                        txtHeinCard.Text = "";
                        txtHeinFrom.Text = "";
                        txtHeinTo.Text = "";
                        txtMediOrg.Text = "";
                    }
                }
                else
                {
                    txtPatient.Text = "";
                    txtPatientName.Text = "";
                    txtDOB.Text = "";
                    txtGender.Text = "";
                    txtAddress.Text = "";
                    txtPatienType.Text = "";
                    txtHeinCard.Text = "";
                    txtHeinFrom.Text = "";
                    txtHeinTo.Text = "";
                    txtMediOrg.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {
                LogSystem.Error("Không thể tách thẻ BHYT");
            }
            return result;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSearch();
                LoadDataToTreeSereServ();
                FillInfoPatient(currentTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSearch()
        {
            try
            {
                //treatment = new V_HIS_TREATMENT_FEE();
                CommonParam param = new CommonParam();
                //HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                //if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                //{
                //    string code = txtFindTreatmentCode.Text.Trim();
                //    if (code.Length < 12)
                //    {
                //        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                //        txtFindTreatmentCode.Text = code;
                //    }
                //    filter.TREATMENT_CODE__EXACT = code;

                //}
                //var listTreatment = new BackendAdapter(param)
                //        .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                //if (listTreatment != null && listTreatment.Count > 0)
                //{
                //    currentTreatment = listTreatment.FirstOrDefault();
                //    treatmentId = currentTreatment.ID;
                //}
                //else
                //{
                //    param.Messages.Add(ResourceMessage.KhongTimThayMaDieuTri);
                //    return;
                //}
                this.isReloadTree = true;
                HisTreatmentFilter filter = new HisTreatmentFilter();
                if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                {
                    string code = txtFindTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    filter.TREATMENT_CODE__EXACT = code;

                }
                else if (this.treatmentId > 0)
                {
                    filter.ID = this.treatmentId;
                }
                var listTreatment = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    currentTreatment = listTreatment.FirstOrDefault();
                    treatmentId = currentTreatment.ID;
                    txtFindTreatmentCode.Text = currentTreatment.TREATMENT_CODE;
                }
                else
                {
                    param.Messages.Add(ResourceMessage.KhongTimThayMaDieuTri);
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFindTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToTreeSereServ()
        {
            try
            {
                ListSereServ = new List<V_HIS_SERE_SERV_5>();
                currentSereServs = new List<V_HIS_SERE_SERV_5>();
                dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                if (this.treatmentId.HasValue)
                {
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = this.treatmentId.Value;
                    var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        foreach (var item in listSSBill)
                        {
                            if (item.IS_CANCEL == (short)1)
                                continue;
                            if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                            dicSereServBill[item.SERE_SERV_ID].Add(item);
                        }
                    }
                    //if (ListSereServTranfer != null && ListSereServTranfer.Count > 0)
                    //{
                    //    currentSereServs = ListSereServTranfer;
                    //    foreach (var item in ListSereServTranfer)
                    //    {
                    //        if (dicSereServBill.ContainsKey(item.ID))
                    //            continue;
                    //        if (item.IS_NO_PAY == 1 || item.VIR_TOTAL_PATIENT_PRICE == 0 || item.IS_NO_EXECUTE == 1)
                    //            continue;
                    //        ListSereServ.Add(item);
                    //    }
                    //}
                    //else
                    //{
                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    ssFilter.TDL_TREATMENT_ID = this.treatmentId;
                    var hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                    if (hisSereServs != null && hisSereServs.Count > 0)
                    {
                        currentSereServs = hisSereServs;
                        foreach (var item in hisSereServs)
                        {
                            if (dicSereServBill.ContainsKey(item.ID))
                                continue;
                            if (item.IS_NO_PAY == 1 || item.IS_NO_EXECUTE == 1 || (item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && item.VIR_TOTAL_PATIENT_PRICE_NO_DC == 0))// #17152 
                                continue;
                            ListSereServ.Add(item);
                        }
                    }
                    //  }

                }
                this.BindTreePlus(ListSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToTreeSereServ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                this.isReloadTree = false;
                if (trvService.Nodes != null)
                {
                    foreach (TreeListNode node in trvService.Nodes)
                    {
                        ProcessTyLe(node);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_AfterCheckNode(object sender, NodeEventArgs e)
        {
            try
            {
                var row = (SereServADO)trvService.GetDataRecordByNode(e.Node);
                if (row != null)
                {
                    this.treeSereServ_AfterCheckNode(e.Node, row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                this.treeSereServ_BeforeCheckNode(e.Node, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    var rowData = (SereServADO)trvService.GetDataRecordByNode(e.Node);

                    if (rowData != null)
                    {
                        this.treeSereServ_CustomDrawNodeCell(rowData, e);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    var data = (SereServADO)trvService.GetDataRecordByNode(e.Node);
                    if (data != null)
                    {
                        this.treeSereServ_CustomDrawNodeCheckBox(data, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is SereServADO)
                {
                    var rowData = data as SereServADO;

                    if (rowData.IsLeaf.HasValue && rowData.IsLeaf.Value)
                    {
                        if (e.Column.FieldName == "IsExpend")
                        {
                            if (1 == 2)//this.updateSingleRow != null)
                            {
                                e.RepositoryItem = repositoryItemchkIsExpend__Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemchkIsExpend__Disable;
                            }

                            if (rowData != null && rowData.IS_EXPEND == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                rowData.IsExpend = true;
                            }
                            else
                            {
                                rowData.IsExpend = false;
                            }
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_DISCOUNT")
                        {
                            e.RepositoryItem = repositoryItemSpinEdit__Discount;
                        }
                    }
                    else
                    {
                        if (e.Column.FieldName == "VIR_TOTAL_DISCOUNT")
                        {
                            e.RepositoryItem = repositoryItemTextEdit__D;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    SereServADO currentRow = e.Row as SereServADO;
                    if (currentRow == null) return;
                    this.treeSereServ_CustomUnboundColumnData(currentRow, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                trvService.PostEditor();
                btnSave.Focus();
                MOS.SDO.HisSereServDiscountSDO sdo = new HisSereServDiscountSDO();
                sdo.TreatmentId = this.treatmentId ?? 0;
                if (chkMienGiamTreatment.Checked)
                {
                    sdo.AutoDiscountRatio = this.spinEditMienGiamTreatmnet.Value > 0 ? this.spinEditMienGiamTreatmnet.Value / 100 : 0;
                }
                sdo.IsAutoDiscount = chkMienGiamTreatment.Checked;

                List<HIS_SERE_SERV> listUpdate = new List<HIS_SERE_SERV>();
                if (this.currentTreatment.IS_LOCK_FEE != 1)
                {
                    var dataChecks = this.GetListCheck();
                    if (dataChecks != null && dataChecks.Count > 0)
                    {
                        string mess = "";
                        foreach (var item in dataChecks)
                        {
                            HIS_SERE_SERV ado = new HIS_SERE_SERV();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(ado, item);
                            ado.DISCOUNT = item.VIR_TOTAL_DISCOUNT;
                            if (item.VIR_TOTAL_DISCOUNT > item.VIR_TOTAL_PATIENT_PRICE_NO_DC)
                            {
                                mess += item.TDL_SERVICE_NAME + "; ";
                            }
                            listUpdate.Add(ado);
                        }
                        if (!string.IsNullOrEmpty(mess))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Các dịch vụ có miễn giảm vượt quá giới hạn Bệnh nhân phải trả: " + mess, "Thông báo");
                            return;
                        }
                        sdo.HisSereServs = listUpdate;
                        //var data = new BackendAdapter(param).Post<HisSereServDiscountSDO>("api/HisSereServ/UpdateDiscountList", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);
                        //if (data != null)
                        //{
                        //    success = true;
                        //}
                        //MessageManager.Show(this, param, success);
                    }
                    //else if (!chkMienGiamTreatment.Checked)
                    //{
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    //    return;
                    //}
                    Inventec.Common.Logging.LogSystem.Debug("INPUT____"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    var data = new BackendAdapter(param).Post<HisSereServDiscountSDO>("api/HisSereServ/UpdateDiscountList", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);
                    if (data != null)
                    {
                        success = true;
                        LoadDataToTreeSereServ();
                    }
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Hồ sơ đã khóa viện phí", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (!e.Node.HasChildren && e.Node.Checked)
                {
                    //TOD

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void barButtonItem__New_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

    }
}
