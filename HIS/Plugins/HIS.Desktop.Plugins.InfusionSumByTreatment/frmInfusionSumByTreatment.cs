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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.InfusionSumByTreatment.Config;
using HIS.Desktop.Plugins.InfusionSumByTreatment.Validtion;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL;

namespace HIS.Desktop.Plugins.InfusionSumByTreatment
{
    public partial class frmInfusionSumByTreatment : HIS.Desktop.Utility.FormBase
    {
        private IcdProcessor icdProcessor = null;
        private UserControl ucIcd = null;
        private V_HIS_INFUSION_SUM currentInfusionSum = null;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private long treatmentId = 0;
        private V_HIS_TREATMENT_2 treatment = null;
        private int positionHandleControl = -1;
        private bool IsTreatmentList;

        public frmInfusionSumByTreatment(Inventec.Desktop.Common.Modules.Module module, long data, bool isTreatmentList)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;

                this.treatmentId = data;
                this.IsTreatmentList = isTreatmentList;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                InitUcIcd();
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

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = TxtIcdSubCode;
                ado.Width = 440;
                ado.Height = 24;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();

                this.ucIcd = (UserControl)icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.panelControlUcIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtIcdSubCode()
        {
            txtIcdSubCode.Focus();
            txtIcdSubCode.SelectAll();
        }

        private void frmInfusionSumByTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeyFrmLanguage();
                if (this.treatmentId > 0)
                {
                    gridViewInfusionSum.DataController.GetAllFilteredAndSortedRows();
                    FillDataToGrid();
                    LoadTreatmentById();
                    ResetControlValue(true);
                    ValidMaxLength();
                }

                if (treatment.IS_PAUSE == 1 || IsTreatmentList)
                {
                    long keyLockingTreatment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY_UPDATING_AFTER_LOCKING_TREATMENT));
                    if (keyLockingTreatment == 1)
                    {
                        btnNew.Enabled = true;
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnNew.Enabled = false;
                        btnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentById()
        {
            try
            {
                HisTreatmentView2Filter treatFilter = new HisTreatmentView2Filter();
                treatFilter.ID = treatmentId;
                var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, treatFilter, null);
                if (listTreatment != null && listTreatment.Count == 1)
                {
                    this.treatment = listTreatment.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                HisInfusionSumViewFilter infuFilter = new HisInfusionSumViewFilter();
                infuFilter.TREATMENT_ID = this.treatmentId;
                infuFilter.ORDER_DIRECTION = "DESC";
                infuFilter.ORDER_FIELD = "MODIFY_TIME";
                var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_INFUSION_SUM>>("api/HisInfusionSum/GetView", ApiConsumers.MosConsumer, infuFilter, null);
                gridControlInfusionSum.BeginUpdate();
                gridControlInfusionSum.DataSource = listData;
                gridControlInfusionSum.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValue(bool IsInit)
        {
            try
            {
                this.currentInfusionSum = null;
                this.SetControlValueByInfusionSum(IsInit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlValueByInfusionSum(bool isInit)
        {
            try
            {
                icdProcessor.Reload(ucIcd, null);
                txtIcdText.Text = "";
                txtIcdSubCode.Text = "";
                txtNote.Text = "";
                if (this.currentInfusionSum != null)
                {
                    HIS.UC.Icd.ADO.IcdInputADO inputIcd = new HIS.UC.Icd.ADO.IcdInputADO();
                    //inputIcd.ICD_ID = this.currentInfusionSum.ICD_ID;
                    inputIcd.ICD_CODE = this.currentInfusionSum.ICD_CODE;
                    inputIcd.ICD_NAME = this.currentInfusionSum.ICD_NAME;
                    icdProcessor.Reload(ucIcd, inputIcd);
                    txtIcdText.Text = this.currentInfusionSum.ICD_TEXT;
                    txtIcdSubCode.Text = this.currentInfusionSum.ICD_SUB_CODE;
                    txtNote.Text = this.currentInfusionSum.NOTE;
                }
                else if (isInit)
                {
                    HIS.UC.Icd.ADO.IcdInputADO inputIcd = new HIS.UC.Icd.ADO.IcdInputADO();
                    //inputIcd.ICD_ID = this.treatment.ICD_ID;
                    inputIcd.ICD_CODE = this.treatment.ICD_CODE;
                    inputIcd.ICD_NAME = this.treatment.ICD_NAME;
                    icdProcessor.Reload(ucIcd, inputIcd);
                    txtIcdText.Text = this.treatment.ICD_TEXT;
                    txtIcdSubCode.Text = this.treatment.ICD_SUB_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //txtIcdExtraCode.Focus();
                    //txtIcdExtraCode.SelectAll();
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
                else if (e.KeyCode == Keys.F1)
                {
                    //hien thi popup chon icd
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SecondaryIcd");
                        MessageManager.Show(Base.ResourceMessageLang.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(stringIcds, txtIcdSubCode.Text, txtIcdText.Text);

                        List<object> listArgs = new List<object>();
                        listArgs.Add(secondaryIcdADO);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        WaitingManager.Hide();
                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(Base.ResourceMessageLang.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        txtIcdSubCode.Focus();
            //        txtIcdSubCode.SelectAll();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        SendKeys.Send("{TAB}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void gridViewInfusionSum_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_INFUSION_SUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "ICD_MAIN")
                        {
                            try
                            {
                                e.Value = (!String.IsNullOrEmpty(data.ICD_NAME)) ? data.ICD_NAME : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
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

        private void gridViewInfusionSum_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (V_HIS_INFUSION_SUM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = repositoryItemBtnDelete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteDisable;
                        }
                    }
                    else if (e.Column.FieldName == "VIEW_DETAIL")
                    {
                        if (treatment.IS_PAUSE == 1 || IsTreatmentList)
                        {
                            long keyLockingTreatment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY_UPDATING_AFTER_LOCKING_TREATMENT));
                            if (keyLockingTreatment == 1)
                            {
                                e.RepositoryItem = repositoryItemBtnViewDetail;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnViewDetailDisable;
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

        private void gridViewInfusionSum_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                this.currentInfusionSum = null;
                if (e.RowHandle >= 0)
                {
                    this.currentInfusionSum = (V_HIS_INFUSION_SUM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }

                SetControlValueByInfusionSum(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewInfusionSum_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                    return;
                this.currentInfusionSum = null;
                if (gridViewInfusionSum.FocusedRowHandle >= 0)
                {
                    this.currentInfusionSum = (V_HIS_INFUSION_SUM)((IList)((BaseView)sender).DataSource)[gridViewInfusionSum.FocusedRowHandle];
                }
                SetControlValueByInfusionSum(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidMaxLength()
        {
            ValidMaxlengthIcdeSubCode();
            ValidMaxlengthIcdText();
            ValidMaxlengthNote();
        }

        private void ValidMaxlengthIcdeSubCode()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtIcdSubCode;
                validateMaxLength.maxLength = 500;
                dxValidationProvider1.SetValidationRule(txtIcdSubCode, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidMaxlengthIcdText()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtIcdText;
                validateMaxLength.maxLength = 500;
                dxValidationProvider1.SetValidationRule(txtIcdText, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidMaxlengthNote()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtNote;
                validateMaxLength.maxLength = 1000;
                dxValidationProvider1.SetValidationRule(txtNote, validateMaxLength);
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

                if (!btnSave.Enabled || this.treatmentId <= 0)
                    return;
                bool icdValid = (bool)icdProcessor.ValidationIcd(ucIcd);
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate() || !icdValid)
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_INFUSION_SUM data = new HIS_INFUSION_SUM();
                data.TREATMENT_ID = this.treatmentId;
                if (this.currentInfusionSum != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_INFUSION_SUM>(data, this.currentInfusionSum);
                }
                var icdData = this.icdProcessor.GetValue(this.ucIcd);
                if (icdData != null && icdData is HIS.UC.Icd.ADO.IcdInputADO)
                {
                    data.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdData).ICD_CODE;
                    data.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdData).ICD_NAME;
                }
                data.ICD_TEXT = txtIcdText.Text;
                data.ICD_SUB_CODE = txtIcdSubCode.Text;
                data.NOTE = txtNote.Text.Trim();
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId && o.ROOM_TYPE_ID == this.currentModule.RoomTypeId);
                if (room != null)
                {
                    data.ROOM_ID = room.ID;
                    data.DEPARTMENT_ID = room.DEPARTMENT_ID;
                }

                if (this.currentInfusionSum != null)
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Post<HIS_INFUSION_SUM>("api/HisInfusionSum/Update", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                }
                else
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Post<HIS_INFUSION_SUM>("api/HisInfusionSum/Create", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                }
                if (success)
                {
                    FillDataToGrid();
                    ResetControlValue(false);
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                this.ResetControlValue(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void bbtnRCNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

        private void bbtnRCChoiceIcd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    SecondaryIcdADO ado = new SecondaryIcdADO(RefreshChanDoanPhu, txtIcdSubCode.Text, txtIcdText.Text);
                    listArgs.Add(ado);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("extenceInstance is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.PhienBanPhanMemHienTaiChuaHoTroChucNangNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshChanDoanPhu(string icdCodes, string icdNames)
        {
            try
            {
                if (!String.IsNullOrEmpty(icdNames))
                {
                    txtIcdText.Text = icdNames;
                }
                if (!String.IsNullOrEmpty(icdCodes))
                {
                    txtIcdSubCode.Text = icdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_INFUSION_SUM)gridViewInfusionSum.GetFocusedRow();
                if (data != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.BanCoMuonXoaDuLieu, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        return;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    var success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisInfusionSum/Delete", ApiConsumers.MosConsumer, data.ID, param);
                    if (success)
                    {
                        FillDataToGrid();
                        ResetControlValue(false);
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuTruyenDich_MPS000146, delegateRunPrintTemplte);
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

        private void repositoryItemBtnViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewInfusionSum.FocusedRowHandle < 0)
                    return;
                var data = (V_HIS_INFUSION_SUM)gridViewInfusionSum.GetFocusedRow();
                if (data == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfusionCreate").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfusionCreate'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.InfusionCreate' is not plugins");

                List<object> listArgs = new List<object>();
                listArgs.Add(data.ID);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                ((Form)extenceInstance).ShowDialog();

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
                var data = (V_HIS_INFUSION_SUM)gridViewInfusionSum.GetFocusedRow();
                if (data == null)
                    return result;
                WaitingManager.Show();
                HisInfusionViewFilter filter = new HisInfusionViewFilter();
                filter.INFUSION_SUM_ID = data.ID;
                var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_INFUSION>>("api/HisInfusion/GetView", ApiConsumers.MosConsumer, filter, null);

                MOS.Filter.HisTreatmentBedRoomViewFilter filterBedRoom = new HisTreatmentBedRoomViewFilter();
                filterBedRoom.TREATMENT_ID = this.treatmentId;
                V_HIS_TREATMENT_BED_ROOM _TreatmetnbedRoom = new V_HIS_TREATMENT_BED_ROOM();
                var TreatmetnbedRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filterBedRoom, null);
                if (TreatmetnbedRooms != null && TreatmetnbedRooms.Count > 0)
                {
                    _TreatmetnbedRoom = TreatmetnbedRooms.OrderByDescending(o => o.OUT_TIME.HasValue).OrderByDescending(o => o.ID).FirstOrDefault(o => o.BED_ID.HasValue);
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                //long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.CONFIG_KEY__HIS_DESKTOP_PLUGINS_EMR_DOCUMENT_IS_PRINT_MERGE));
                //if (keyPrintMerge == 1)
                //{
                //    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, "", (this.treatment != null ? this.treatment.TREATMENT_CODE : ""));
                //}
                List<HIS_MIXED_MEDICINE> lstMixedMedicine = new List<HIS_MIXED_MEDICINE>();

                if (listData != null && listData.Count() > 0)
                {
                    foreach (var item in listData)
                    {
                        HisMixedMedicineFilter medicineFilter = new HisMixedMedicineFilter();
                        medicineFilter.INFUSION_ID = item.ID;
                        var lstHisMixedMedicine = new BackendAdapter(new CommonParam()).Get<List<HIS_MIXED_MEDICINE>>("api/HisMixedMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, null);
                        if (lstHisMixedMedicine != null && lstHisMixedMedicine.Count() > 0)
                        {
                            lstMixedMedicine.AddRange(lstHisMixedMedicine);
                        }
                    }
                }

                MPS.Processor.Mps000146.PDO.Mps000146PDO rdo = new MPS.Processor.Mps000146.PDO.Mps000146PDO(
                    data,
                    this.treatment,
                    listData,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    _TreatmetnbedRoom,
                    lstMixedMedicine
                    );
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null) { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var lang = Base.ResourceLangManager.LanguageFrmInfusionSumByTreatment;
                //Button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__BTN_SAVE", lang, cul);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__BTN_NEW", lang, cul);
                //layout
                this.layoutIcdSubCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__LAYOUT_ICD_SUB_CODE", lang, cul);
                this.layoutIcdText.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__LAYOUT_ICD_TEXT", lang, cul);
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__LAYOUT_NOTE", lang, cul);

                //grid Expense
                this.gridColumn_InfusionSum_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_CREATE_TIME", lang, cul);
                this.gridColumn_InfusionSum_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_CREATOR", lang, cul);
                this.gridColumn_InfusionSum_Department.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_DEPARTMENT", lang, cul);
                this.gridColumn_InfusionSum_IcdMain.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_ICD_MAIN", lang, cul);
                this.gridColumn_InfusionSum_IcdText.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_ICD_TEXT", lang, cul);
                this.gridColumn_InfusionSum_Modifier.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_MODIFIER", lang, cul);
                this.gridColumn_InfusionSum_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_MODIFY_TIME", lang, cul);
                this.gridColumn_InfusionSum_Room.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_ROOM", lang, cul);
                this.gridColumn_InfusionSum_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_STT", lang, cul);
                this.gridColumn_InfusionSum_Note.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__GRID_INFUSION_SUM__COLUMN_NOTE", lang, cul);

                //Repository
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__REPOSITORY__BTN_DELETE", lang, cul);
                this.repositoryItemBtnDeleteDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__REPOSITORY__BTN_DELETE", lang, cul);
                this.repositoryItemBtnPrint.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__REPOSITORY__BTN_PRINT", lang, cul);
                this.repositoryItemBtnViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INFUSION_SUM_BY_TREATMENT__REPOSITORY__BTN_CREATE_INFUSION", lang, cul);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string seperate = ";";
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                string[] periodSeparators = new string[1];
                periodSeparators[0] = seperate;
                string[] arrIcdExtraCodes = txtIcdSubCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                {
                    var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                    foreach (var itemCode in arrIcdExtraCodes)
                    {
                        var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                        if (icdByCode != null && icdByCode.ID > 0)
                        {
                            strIcdNames += (seperate + icdByCode.ICD_NAME);
                        }
                        else
                        {
                            strWrongIcdCodes += (seperate + itemCode);
                        }
                    }
                    strIcdNames += seperate;
                    if (!String.IsNullOrEmpty(strWrongIcdCodes))
                    {
                        MessageManager.Show(String.Format(Base.ResourceMessageLang.KhongTimThayIcdTuongUng, strWrongIcdCodes));
                        //txtIcdExtraCode.Focus();
                        //txtIcdExtraCode.SelectAll();
                    }
                }
                txtIcdText.Text = strIcdNames;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_Edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.currentInfusionSum = (V_HIS_INFUSION_SUM)gridViewInfusionSum.GetFocusedRow();
                if (this.currentInfusionSum != null)
                {
                    SetControlValueByInfusionSum(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
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
