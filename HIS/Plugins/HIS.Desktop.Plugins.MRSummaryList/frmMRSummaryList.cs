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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.MRSummaryList
{
    public partial class frmMRSummaryList : FormBase
    {
        MRSummaryDetailADO currentADO = new MRSummaryDetailADO();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_MR_CHECK_SUMMARY> lstMrCheckSummary = new List<HIS_MR_CHECK_SUMMARY>();
        HIS_MR_CHECK_SUMMARY currentMrCheckSummary = new HIS_MR_CHECK_SUMMARY();
        V_HIS_TREATMENT currentTreatment = new V_HIS_TREATMENT();
        List<HIS_MR_CHECKLIST> lstMrChecklist { get; set; }
        List<HIS_MR_CHECK_ITEM> lstMrCheckItem { get; set; }
        List<HIS_MR_CHECK_ITEM_TYPE> lstMrCheckItemType { get; set; }
        public frmMRSummaryList(Inventec.Desktop.Common.Modules.Module moduleData, MRSummaryDetailADO _currentADO)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.currentADO = _currentADO;
        }

        private void frmMRSummaryList_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void MeShow()
        {
            Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("currentADO____:", currentADO));

            GetTreatmentById(currentADO.TreatmentId);
            FillDataFormList();
        }

        private void GetTreatmentById(long treatmentId)
        {
            try
            {
                HisTreatmentViewFilter tmFilter = new HisTreatmentViewFilter();
                tmFilter.ID = treatmentId;
                this.currentTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, tmFilter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataFormList()
        {
            try
            {
                if (this.currentTreatment.ID > 0)
                {
                    HisMrCheckSummaryFilter filter = new HisMrCheckSummaryFilter();
                    filter.TREATMENT_ID = this.currentTreatment.ID;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    lstMrCheckSummary = new BackendAdapter(new CommonParam()).Get<List<HIS_MR_CHECK_SUMMARY>>("api/HisMrCheckSummary/Get", ApiConsumers.MosConsumer, filter, null);
                    if (lstMrCheckSummary != null && lstMrCheckSummary.Count() > 0)
                    {
                        grdSummary.DataSource = null;
                        grdSummary.DataSource = lstMrCheckSummary;
                    }
                    else
                    {
                        btnAdd_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvSummary_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_MR_CHECK_SUMMARY pData = (HIS_MR_CHECK_SUMMARY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }

                    else if (e.Column.FieldName == "TIME_STR")
                    {
                        var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0).ToString();
                        e.Value = time;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0).ToString();
                        e.Value = time;
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0).ToString();
                        e.Value = time;
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        e.Value = pData.IS_APPROVED == 1 ? "Đạt" : "Không đạt";
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentMrCheckSummary = (HIS_MR_CHECK_SUMMARY)grvSummary.GetFocusedRow();
                MRSummaryDetailADO ado = new MRSummaryDetailADO();
                ado.TreatmentId = currentADO.TreatmentId;
                ado.processType = currentADO.processType;
                ado.CheckSummary = currentMrCheckSummary;
                List<object> listArgs = new List<object>();
                listArgs.Add(ado);
                listArgs.Add((RefeshReference)FillDataFormList);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MRSummaryDetail", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvSummary_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    HIS_MR_CHECK_SUMMARY pData = (HIS_MR_CHECK_SUMMARY)grvSummary.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "EDIT") // sửa
                    {
                        if (currentADO.processType != null && currentADO.processType == HIS.Desktop.ADO.MRSummaryDetailADO.OpenFrom.TreatmentList)
                        {
                            e.RepositoryItem = repositoryItemButtonEditDis;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEdit;
                        }
                    }
                    else if (e.Column.FieldName == "DELETE") // xoa
                    {
                        if (currentADO.processType != null && currentADO.processType == HIS.Desktop.ADO.MRSummaryDetailADO.OpenFrom.TreatmentLatchApproveStore)
                        {
                            e.RepositoryItem = pData.CHECK_PLACE == 1 ? repositoryItemButtonDelete : repositoryItemButtonDeleteDis;
                        }
                        else if (currentADO.processType != null && currentADO.processType == HIS.Desktop.ADO.MRSummaryDetailADO.OpenFrom.MedicalStoreV2)
                        {
                            e.RepositoryItem = pData.CHECK_PLACE == 2 ? repositoryItemButtonDelete : repositoryItemButtonDeleteDis;
                        }
                        else if (currentADO.processType != null && currentADO.processType == HIS.Desktop.ADO.MRSummaryDetailADO.OpenFrom.TreatmentList)
                        {
                            e.RepositoryItem = repositoryItemButtonDeleteDis;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                MRSummaryDetailADO ado = new MRSummaryDetailADO();
                ado.TreatmentId = currentADO.TreatmentId;
                ado.CheckSummary = null;
                ado.processType = currentADO.processType;
                List<object> listArgs = new List<object>();
                listArgs.Add(ado);
                listArgs.Add((RefeshReference)FillDataFormList);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MRSummaryDetail", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000472", delegateRunPrintTemplte);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrintTemplte(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                GetMrCheckList(lstMrCheckSummary.Select(o => (long)o.ID).ToList());
                GetMrCheckItem(this.lstMrChecklist.Select(o => (long)o.MR_CHECK_ITEM_ID).Distinct().ToList());
                GetMrCheckItemType(this.lstMrCheckItem.Select(o => (long)o.CHECK_ITEM_TYPE_ID).Distinct().ToList());

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatment != null ? this.currentTreatment.TREATMENT_CODE : ""), printTypeCode, this.moduleData != null ? moduleData.RoomId : 0);

                MPS.Processor.Mps000472.PDO.Mps000472PDO rdo = new MPS.Processor.Mps000472.PDO.Mps000472PDO(
                    this.currentTreatment,
                    lstMrCheckSummary,
                    this.lstMrChecklist,
                    this.lstMrCheckItem,
                    this.lstMrCheckItemType
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void GetMrCheckItemType(List<long> lstCheckItemTypeId)
        {
            try
            {
                if (lstCheckItemTypeId != null && lstCheckItemTypeId.Count() > 0)
                {
                    HisMrCheckItemTypeFilter mrCheckItemTypeFilter = new HisMrCheckItemTypeFilter();
                    mrCheckItemTypeFilter.IDs = lstCheckItemTypeId;
                    mrCheckItemTypeFilter.ORDER_DIRECTION = "DESC";
                    mrCheckItemTypeFilter.ORDER_FIELD = "MODIFY_TIME";
                    lstMrCheckItemType = new BackendAdapter(new CommonParam()).Get<List<HIS_MR_CHECK_ITEM_TYPE>>("api/HisMrCheckItemType/Get", ApiConsumers.MosConsumer, mrCheckItemTypeFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMrCheckItem(List<long> lstCheckItemId)
        {
            try
            {
                if (lstCheckItemId != null && lstCheckItemId.Count() > 0)
                {
                    HisMrCheckItemFilter mrCheckItemFilter = new HisMrCheckItemFilter();
                    mrCheckItemFilter.IDs = lstCheckItemId;
                    mrCheckItemFilter.ORDER_DIRECTION = "DESC";
                    mrCheckItemFilter.ORDER_FIELD = "MODIFY_TIME";
                    lstMrCheckItem = new BackendAdapter(new CommonParam()).Get<List<HIS_MR_CHECK_ITEM>>("api/HisMrCheckItem/Get", ApiConsumers.MosConsumer, mrCheckItemFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetMrCheckList(List<long> mrCheckSummaryId)
        {
            try
            {
                if (currentMrCheckSummary != null)
                {
                    HisMrChecklistFilter mrCheckListFilter = new HisMrChecklistFilter();
                    mrCheckListFilter.MR_CHECK_SUMMARY_IDs = mrCheckSummaryId;
                    mrCheckListFilter.ORDER_DIRECTION = "DESC";
                    mrCheckListFilter.ORDER_FIELD = "MODIFY_TIME";
                    lstMrChecklist = new BackendAdapter(new CommonParam()).Get<List<HIS_MR_CHECKLIST>>("api/HisMrChecklist/Get", ApiConsumers.MosConsumer, mrCheckListFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvSummary_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentMrCheckSummary = (HIS_MR_CHECK_SUMMARY)grvSummary.GetFocusedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                FillDataFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (HIS_MR_CHECK_SUMMARY)grvSummary.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisMrCheckSummary/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataFormList();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
