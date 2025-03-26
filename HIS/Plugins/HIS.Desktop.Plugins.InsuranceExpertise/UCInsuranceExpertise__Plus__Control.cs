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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.InsuranceExpertise.Config;
using HIS.UC.SereServTree;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InsuranceExpertise
{
    public partial class UCInsuranceExpertise
    {

        private void dtFeeLockTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtFeeLockTimeTo.Focus();
                    dtFeeLockTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtFeeLockTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboStatus.Focus();
                    cboStatus.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboTimeFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                dtFeeLockTimeFrom.Focus();
                ToolTip toolTip1 = new ToolTip();
                toolTip1.AutoPopDelay = 0;
                toolTip1.InitialDelay = 0;
                toolTip1.ReshowDelay = 0;
                toolTip1.ShowAlways = true;
                toolTip1.SetToolTip(this.cboTimeFrom, cboTimeFrom.Items[cboTimeFrom.SelectedIndex].ToString());
                //dtFeeLockTimeFrom.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtFindTreatmentCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFindTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                    {
                        //positionHandleControl = -1;
                        //if (!dxValidationProvider1.Validate())
                        //    return;
                        WaitingManager.Show();
                        FillDataToGridTreatment();
                        if (listTreatment != null && listTreatment.Count == 1)
                        {
                            this.currentTreatment = listTreatment.First();
                            ClickTreatment(currentTreatment);
                            FillDataToGridHeinCardAndHeinApproval();
                        }
                        txtFindTreatmentCode.SelectAll();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    FillDataToGridTreatment();
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        this.currentTreatment = listTreatment.First();
                        ClickTreatment(currentTreatment);
                        FillDataToGridHeinCardAndHeinApproval();
                    }
                    gridControlTreatment.Focus();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TREATMENT_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        else if (e.Column.FieldName == "STORE_BORDEREAU_CODE_STR")
                        {
                            try
                            {
                                e.Value = data.IS_LOCK_HEIN == 1 ? data.STORE_BORDEREAU_CODE : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "IMAGE_STATUS")
                    {
                        try
                        {
                            decimal? CountHeinApproval = (long?)view.GetRowCellValue(e.RowHandle, "COUNT_HEIN_APPROVAL");
                            if (CountHeinApproval.HasValue && CountHeinApproval.Value > 0)
                            {
                                e.RepositoryItem = repositoryItemImgApprovaled;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemImgNotApproval;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "LOCK_HEIN")
                    {
                        try
                        {
                            short? IsLockHein = (short?)view.GetRowCellValue(e.RowHandle, "IS_LOCK_HEIN");
                            if (IsLockHein == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = repositoryItemImgIsLockHein;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemImgIsUnlockHein;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "DOWNLOAD_XML")
                    {
                        string XML4210_URL = (view.GetRowCellValue(e.RowHandle, "XML4210_URL") ?? "").ToString();
                        if (!String.IsNullOrWhiteSpace(XML4210_URL))
                        {
                            e.RepositoryItem = repositoryItem_Dowload;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItem_DowloadDisable;
                        }
                    }
                    else if (e.Column.FieldName == "VIEW_XML")
                    {
                        string XML4210_URL = (view.GetRowCellValue(e.RowHandle, "XML4210_URL") ?? "").ToString();
                        if (!String.IsNullOrWhiteSpace(XML4210_URL))
                        {
                            e.RepositoryItem = repositoryItem_XMLView;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItem_XMlViewDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {

                GridHitInfo info = gridViewTreatment.CalcHitInfo(e.Location);
                if (info != null && info.Column.FieldName == "DX$CheckboxSelectorColumn")
                {
                    return;
                }
                this.currentPatientTypeAlter = null;
                this.currentTreatment = null;
                //this.currentTreatment = (V_HIS_TREATMENT_1)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                this.currentTreatment = (V_HIS_TREATMENT_1)gridViewTreatment.GetFocusedRow();

                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFillDataToGridHeinCardAndHeinApproval));
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadFillDataToGridHeinCardAndHeinApproval()
        {
            try
            {
                System.Threading.Thread.Sleep(5);
                Invoke(new Action(() =>
                {
                    WaitingManager.Show();
                    FillDataToGridHeinCardAndHeinApproval();
                    WaitingManager.Hide();
                }));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var data = (V_HIS_TREATMENT_1)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null && data.IS_LOCK_HEIN != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    GridGroupRowInfo info = e.Info as GridGroupRowInfo;
                    info.SelectorInfo.State = DevExpress.Utils.Drawing.ObjectState.Disabled;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                //listSelectTreatment.Clear();
                //int[] listIndex = gridViewTreatment.GetSelectedRows();
                //foreach (var index in listIndex)
                //{
                //    var treatment = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(index);
                //    if (treatment != null && treatment.IS_LOCK_HEIN != IMSys.DbConfig.HIS_RS.HIS_TREATMENT.IS_LOCK_HEIN__TRUE)
                //    {
                //        listSelectTreatment.Add(treatment);
                //    }
                //}
                //if (listSelectTreatment.Count > 0)
                //{
                //    btnApproval.Enabled = true;
                //}
                //else
                //{
                //    btnApproval.Enabled = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHeinCard_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HIS_PATIENT_TYPE_ALTER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "HEIN_CARD_FROM_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_FROM_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "HEIN_CARD_TO_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.HEIN_CARD_TO_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHeinCard_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentPatientTypeAlter = null;
                this.currentPatientTypeAlter = (HIS_PATIENT_TYPE_ALTER)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                List<V_HIS_SERE_SERV_5> hisSereServs = new List<V_HIS_SERE_SERV_5>();
                if (this.currentPatientTypeAlter != null && listSereServ != null && listSereServ.Count > 0)
                {
                    foreach (var sereServ in listSereServ)
                    {
                        BhytServiceRequestData bhytService = new BhytServiceRequestData(sereServ.JSON_PATIENT_TYPE_ALTER, null);
                        if (bhytService != null && this.IsProperPatientTypeData(this.currentPatientTypeAlter, bhytService.PatientTypeData))
                        {
                            hisSereServs.Add(sereServ);
                        }
                    }
                }
                FillDataToSereServTree(hisSereServs);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHeinApproval_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (HIS_HEIN_APPROVAL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHeinApproval_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var data = (HIS_HEIN_APPROVAL)gridViewHeinApproval.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "CANCEL")
                    {
                        if (this.currentTreatment.IS_LOCK_HEIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = repositoryItemBtnCancelApprovalDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnCancelApproval;
                        }
                    }
                    else if (e.Column.FieldName == "DOWNLOAD_XML")
                    {
                        if (String.IsNullOrEmpty(data.XML_URL))
                        {
                            e.RepositoryItem = repositoryItemBtnDownXmlDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDownXml;
                        }
                    }
                    else if (e.Column.FieldName == "VIEW_XML")
                    {
                        if (!String.IsNullOrEmpty(data.XML_URL))
                        {
                            e.RepositoryItem = repositoryItemViewXmlEnable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemViewXmlDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHeinApproval_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentPatientTypeAlter = null;
                var data = (HIS_HEIN_APPROVAL)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                List<V_HIS_SERE_SERV_5> hisSereServs = new List<V_HIS_SERE_SERV_5>();
                if (data != null)
                {
                    if (this.listSereServ != null && this.listSereServ.Count > 0)
                    {
                        hisSereServs = listSereServ.Where(o => o.HEIN_APPROVAL_ID.HasValue && o.HEIN_APPROVAL_ID.Value == data.ID).ToList();
                    }
                }
                FillDataToSereServTree(hisSereServs);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.VIR_TOTAL_PATIENT_PRICE.HasValue && data.VIR_TOTAL_PATIENT_PRICE.Value > 0)
                    {
                        //if (data.BILL_ID.HasValue)
                        //    e.Appearance.ForeColor = Color.Blue;
                        //else if (e.Node.Checked)
                        //{
                        //    e.Appearance.ForeColor = Color.Blue;
                        //}
                        //else
                        //{
                        //    e.Appearance.ForeColor = Color.Black;
                        //}
                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridHeinCardAndHeinApproval()
        {
            try
            {
                listSereServ = new List<V_HIS_SERE_SERV_5>();
                listPatientTypeAlter = new List<HIS_PATIENT_TYPE_ALTER>();
                listHeinApproval = new List<HIS_HEIN_APPROVAL>();
                if (this.currentTreatment != null)
                {
                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    ssFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                    if (this.patientTypeIdBhyt <= 0)
                    {
                        var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisPatientTypeCFG.PATIENT_TYPE_CODE__BHYT);
                        if (patientType != null)
                        {
                            patientTypeIdBhyt = patientType.ID;
                        }
                    }
                    ssFilter.PATIENT_TYPE_ID = this.patientTypeIdBhyt;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);

                    listPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetDistinct", ApiConsumers.MosConsumer, this.currentTreatment.ID, null);

                    if (listPatientTypeAlter != null)
                    {
                        listPatientTypeAlter = listPatientTypeAlter.Where(o => !String.IsNullOrWhiteSpace(o.HEIN_CARD_NUMBER)).ToList();
                    }

                    HisHeinApprovalFilter appFilter = new HisHeinApprovalFilter();
                    appFilter.TREATMENT_ID = this.currentTreatment.ID;
                    appFilter.IS_DELETE = false;
                    listHeinApproval = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_HEIN_APPROVAL>>(HisRequestUriStore.HIS_HEIN_APPROVAL_GET, ApiConsumers.MosConsumer, appFilter, null);
                    btnLockHein.Enabled = false;
                    btnUnLockHein.Enabled = false;
                    if (this.currentTreatment.IS_LOCK_HEIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        btnUnLockHein.Enabled = true;
                        btnLockHein.Enabled = false;
                    }
                    else
                    {
                        btnUnLockHein.Enabled = false;
                        btnLockHein.Enabled = true;
                    }
                    if (btnLuuTru.Visible)
                        btnLuuTru.Enabled = this.currentTreatment.HEIN_LOCK_TIME != null && string.IsNullOrEmpty(this.currentTreatment.STORE_BORDEREAU_CODE);
                    if (string.IsNullOrEmpty(this.currentTreatment.STORE_BORDEREAU_CODE))
                        GetNextStoreBordereauCode();
                    if (txtStoreBordereauCode.Visible)
                        txtStoreBordereauCodeOption2.Text = this.currentTreatment.STORE_BORDEREAU_CODE ?? (nextStoreBordereauCode != null ? nextStoreBordereauCode.StoreBordereauCode : null);
                    if (Config.HisConfigCFG.OptionStoreBordereauCode == "2")
                    {
                        txtStoreBordereauCodeOption2.Text = this.currentTreatment.STORE_BORDEREAU_CODE ?? (nextStoreBordereauCode != null ? nextStoreBordereauCode.StoreBordereauCode : null);
                        ChangeEnableButtonOption2();
                    }
                }
                else
                {
                    txtStoreBordereauCode.Text = null;
                    btnLockHein.Enabled = false;
                    btnUnLockHein.Enabled = false;
                    if (Config.HisConfigCFG.OptionStoreBordereauCode == "2")
                    {
                        dteStoreTime.Enabled = true;
                        btnLuuTruOption2.Text = "Lưu trữ (F5)";
                        btnLuuTruOption2.ToolTip = null;
                        btnLuuTruOption2.Enabled = true;
                        btnLockHein.Enabled = true;
                        btnUnLockHein.Enabled = true;
                        txtStoreBordereauCodeOption2.Enabled = true;
                        txtStoreBordereauCodeOption2.Text = null;
                    }
                }
                gridControlHeinCard.BeginUpdate();
                gridControlHeinCard.DataSource = listPatientTypeAlter;
                gridControlHeinCard.EndUpdate();

                gridControlHeinApproval.BeginUpdate();
                gridControlHeinApproval.DataSource = listHeinApproval;
                gridControlHeinApproval.EndUpdate();

                FillDataToSereServTree(listSereServ);

                //gridControlTreatment.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ChangeEnableButtonOption2()
        {

            try
            {
                dteStoreTime.Enabled = string.IsNullOrEmpty(this.currentTreatment.STORE_BORDEREAU_CODE);

                txtStoreBordereauCodeOption2.Enabled = string.IsNullOrEmpty(this.currentTreatment.STORE_BORDEREAU_CODE);
                btnLockHein.Enabled = this.currentTreatment.IS_LOCK_HEIN != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && string.IsNullOrEmpty(this.currentTreatment.STORE_BORDEREAU_CODE);
                btnUnLockHein.Enabled = this.currentTreatment.IS_LOCK_HEIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                if (string.IsNullOrEmpty(this.currentTreatment.STORE_BORDEREAU_CODE))
                {
                    btnLuuTruOption2.Text = "Lưu trữ (F5)";
                    if (btnLockHein.Enabled)
                        btnLuuTruOption2.Enabled = false;
                }
                else
                {
                    btnLuuTruOption2.Text = "Hủy LT (F5)";
                    btnLuuTruOption2.ToolTip = "Hủy lưu trữ bảng kê";
                    btnLuuTruOption2.Enabled = true;
                    btnUnLockHein.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void FillDataToSereServTree(List<V_HIS_SERE_SERV_5> hisSereServs)
        {
            try
            {
                ssTreeProcessor.Reload(ucSereServTree, hisSereServs);
                decimal totalPrice = 0;
                decimal totalHeinPrice = 0;
                decimal totalPatientPrice = 0;
                if (hisSereServs != null && hisSereServs.Count > 0)
                {
                    totalPrice = hisSereServs.Sum(s => s.VIR_TOTAL_PRICE ?? 0);
                    totalHeinPrice = hisSereServs.Sum(s => s.VIR_TOTAL_HEIN_PRICE ?? 0);
                    totalPatientPrice = hisSereServs.Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                }
                lblVirTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                lblVirTotalHeinPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalHeinPrice, ConfigApplications.NumberSeperator);
                lblVirTotalPatientPrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPatientPrice, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsProperPatientTypeData(HIS_PATIENT_TYPE_ALTER bhyt, BhytPatientTypeData patientData)
        {
            try
            {
                return bhyt.HEIN_CARD_NUMBER == patientData.HEIN_CARD_NUMBER
                && bhyt.HEIN_MEDI_ORG_CODE == patientData.HEIN_MEDI_ORG_CODE
                && (bhyt.LEVEL_CODE ?? "") == (patientData.LEVEL_CODE ?? "")
                && (bhyt.RIGHT_ROUTE_CODE ?? "") == (patientData.RIGHT_ROUTE_CODE ?? "")
                && (bhyt.JOIN_5_YEAR ?? "") == (patientData.JOIN_5_YEAR ?? "")
                && (bhyt.PAID_6_MONTH ?? "") == (patientData.PAID_6_MONTH ?? "")
                && (bhyt.LIVE_AREA_CODE ?? "") == (patientData.LIVE_AREA_CODE ?? "")
                && bhyt.HEIN_CARD_TO_TIME == patientData.HEIN_CARD_TO_TIME
                && bhyt.HEIN_CARD_FROM_TIME == patientData.HEIN_CARD_FROM_TIME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
