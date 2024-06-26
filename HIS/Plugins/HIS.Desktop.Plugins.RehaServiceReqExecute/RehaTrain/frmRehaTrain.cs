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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using AutoMapper;
using Inventec.Core;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.RehaServiceReqExecute.ADO;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.RehaServiceReqExecute.Base;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Plugins.RehaServiceReqExecute.Delegate;
using Inventec.Desktop.Common.Message;
using MOS.Filter;

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    public partial class frmRehaTrain : DevExpress.XtraEditors.XtraForm
    {
        V_HIS_SERVICE_REQ HisServiceReqWithOrderSDO;
        RefeshData refeshDataAfterSuccess;
        List<RehaTrainADO> rehaTrainSdos = null;
        int positionHandleControl = -1;
        List<SereServRehaADO> SereServRehaADOs { get; set; }

        public frmRehaTrain(V_HIS_SERVICE_REQ HisServiceReqWithOrderSDO, List<SereServRehaADO> _sereServRehaADOs, RefeshData _refeshDataAfterSuccess)
        {
            try
            {
                InitializeComponent();
                this.HisServiceReqWithOrderSDO = HisServiceReqWithOrderSDO;
                this.refeshDataAfterSuccess = _refeshDataAfterSuccess;
                this.SereServRehaADOs = _sereServRehaADOs;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmRehaTrain_Load(object sender, EventArgs e)
        {
            try
            {
                ValidControl();
                dtTrainTime.DateTime = DateTime.Now;

                //Language_RehaTrain();
                if (this.HisServiceReqWithOrderSDO != null)
                {
                    CommonParam param = new CommonParam();
                    rehaTrainSdos = new List<RehaTrainADO>();
                    SereServRehaADOs = SereServRehaADOs != null ? SereServRehaADOs.Where(o => o.choose).ToList() : null;
                    //MOS.Filter.HisSereServRehaViewFilter hisSereServRehaFilter = new MOS.Filter.HisSereServRehaViewFilter();
                    //hisSereServRehaFilter.SERVICE_REQ_ID = HisServiceReqWithOrderSDO.ID;

                    //var currentHisSereServRehas = new BackendAdapter(param)
                    //.Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA>>(HisRequestUriStore.HIS_SERE_SERV_REHA_GETVIEW, ApiConsumers.MosConsumer, hisSereServRehaFilter, param);

                    if (SereServRehaADOs == null && SereServRehaADOs.Count == 0)
                        throw new Exception("Khong co dich vu tap");

                    //HisRehaTrainViewFilter rehaTrainFilter = new HisRehaTrainViewFilter();
                    //rehaTrainFilter.SERE_SERV_REHA_IDs = SereServRehaADOs.Select(o => o.ID).ToList();
                    //var rehaTrains = new BackendAdapter(param)
                    //.Get<List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN>>("api/HisRehaTrain/GetView", ApiConsumers.MosConsumer, rehaTrainFilter, param);
                    //LoadComboRehaTrainType(repositoryItemGridLookUpEditRehaTrainType);
                    //LoadComboRehaServiceType(repositoryItemGridLookUpEditRehaServiceType);

                    if (SereServRehaADOs != null && SereServRehaADOs.Count > 0)
                    {
                        //hisRestRetrTypes = hisRestRetrTypes.Where(o => serviceIds.Contains(o.SERVICE_ID)).Distinct().ToList();
                        int dem = 0;
                        foreach (var item in SereServRehaADOs)
                        {


                            RehaTrainADO trainSdo = new RehaTrainADO();

                            Mapper.CreateMap<SereServRehaADO, RehaTrainADO>();
                            trainSdo = Mapper.Map<SereServRehaADO, RehaTrainADO>(item);
                            if (dtTrainTime.EditValue != null && dtTrainTime.DateTime != DateTime.MinValue)
                                trainSdo.TRAIN_TIME = (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrainTime.DateTime) ?? 0);
                            trainSdo.SERE_SERV_REHA_ID = item.ID;
                            trainSdo.Action = (dem == 0 ? HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd : HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                            rehaTrainSdos.Add(trainSdo);
                            dem++;
                        }
                    }
                    else
                    {
                        btnSave.Enabled = false;
                        dtTrainTime.Enabled = false;
                    }
                }
                gridControlRehaTrain.DataSource = rehaTrainSdos;
                dtTrainTime.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Language_RehaTrain()
        {
            try
            {
                lciTrainTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_LCI_TRAIN_TIME", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_BTN_SAVE", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                gridColSTT.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_GRID_COL_STT", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                gridColRehaTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_GRID_COL_REHA_TYPE_CODE", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                gridColRehaTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_GRID_COL_REHA_TYPE_NAME", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                gridColRehaTrainTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_GRID_COL_REHA_TRAIN_TYPE_NAME", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                gridColRehaTrainUnitName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_GRID_COL_REHA_TRAIN_UNIT_NAME", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                gridColTrainTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_GRID_COL_TRAIN_TIME", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());
                gridColAmount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_REHA_TRAIN_GRID_COL_AMOUNT", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture());

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
                btnSave.Focus();
                //TODO
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                bool success = false;
                CommonParam param = new CommonParam();
                List<HIS_REHA_TRAIN> listData = new List<HIS_REHA_TRAIN>();
                var rehaTrainSdos = gridControlRehaTrain.DataSource as List<RehaTrainADO>;
                if (rehaTrainSdos != null)
                {
                    foreach (var item in rehaTrainSdos)
                    {
                        if (item.AMOUNT > 0)
                        {
                            HIS_REHA_TRAIN rehaTrain = new HIS_REHA_TRAIN();
                            Mapper.CreateMap<RehaTrainADO, HIS_REHA_TRAIN>();
                            rehaTrain = Mapper.Map<RehaTrainADO, HIS_REHA_TRAIN>(item);
                            rehaTrain.TRAIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrainTime.DateTime) ?? 0;
                            listData.Add(rehaTrain);
                        }
                    }
                    if (listData == null || listData.Count == 0)
                    {
                        param.Messages.Add("Dữ liệu không hợp lệ");
                        ResultManager.ShowMessage(param, null);
                        return;
                    }

                    WaitingManager.Show();

                    var rs = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_REHA_TRAIN>>(HisRequestUriStore.HIS_REHA_TRAIN__CREATE, ApiConsumers.MosConsumer, listData, param);
                    WaitingManager.Hide();
                    if (rs != null)
                    {
                        success = true;
                        LogSuccess(listData);
                        if (this.refeshDataAfterSuccess != null)
                            this.refeshDataAfterSuccess();
                        this.Close();
                    }

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LogSuccess(List<HIS_REHA_TRAIN> data)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (rehaTrainSdos == null || rehaTrainSdos.Count == 0)
                    rehaTrainSdos = new List<RehaTrainADO>();
                RehaTrainADO trainSdo = new RehaTrainADO();
                trainSdo.Action = ((rehaTrainSdos != null && rehaTrainSdos.Count >= 1) ? HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit : HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd);
                rehaTrainSdos.Add(trainSdo);
                gridControlRehaTrain.BeginUpdate();
                gridControlRehaTrain.DataSource = rehaTrainSdos;
                gridControlRehaTrain.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRehaTrain_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //bool IsNew = Inventec.Common.TypeConvert.Parse.ToBoolean(View.GetRowCellDisplayText(e.RowHandle, View.Columns["IsNew"]));
                    //if (id > 0 && medicineTypes != null && lstCurrentMedicineType != null)
                    //{
                    //    var mety = medicineTypes.FirstOrDefault(o => o.Id == id);
                    //    var mediType = lstCurrentMedicineType.FirstOrDefault(o => o.ID == mety.Id);
                    //    if (mediType != null && mety.TotalAmount <= (mediType.ALERT_MIN_IN_STOCK ?? 0))
                    //    {
                    //        //So luong thuoc ton kho nho hon canh bao ton kho cua thuoc thi boi do mau chu dong thuoc
                    //        e.Appearance.ForeColor = System.Drawing.Color.Red;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRehaTrain_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                RehaTrainADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (RehaTrainADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Action")
                    {
                        if (data.Action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = repositoryItemBtnAdd;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDelete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (rehaTrainSdos == null || rehaTrainSdos.Count == 0)
                    return;

                RehaTrainADO data = gridViewRehaTrain.GetFocusedRow() as RehaTrainADO;
                if (data != null)
                {
                    rehaTrainSdos.Remove(data);
                }
                if (rehaTrainSdos.Count == 0)
                {
                    RehaTrainADO trainSdo = new RehaTrainADO();
                    trainSdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                }

                gridControlRehaTrain.BeginUpdate();
                gridControlRehaTrain.DataSource = rehaTrainSdos;
                gridControlRehaTrain.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRehaTrain_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                RehaTrainADO data = view.GetFocusedRow() as RehaTrainADO;
                if (view.FocusedColumn.FieldName == "REHA_TRAIN_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        FillDataIntoPatientTypeCombo(data.REHA_SERVICE_TYPE_ID, editor);
                        //editor.EditValue = data.REHA_SERVICE_TYPE_ID;
                        //if (editor.EditValue == null)//xemlai...
                        //{
                        //    string error = GetError(gridViewCareDetail.FocusedRowHandle, gridViewCareDetail.FocusedColumn);
                        //    if (error == string.Empty) return;
                        //    gridViewCareDetail.SetColumnError(gridViewCareDetail.FocusedColumn, error);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(long rehaServiceTypeId, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo)
        {
            try
            {
                LoadDataToCombo(patientTypeCombo, rehaServiceTypeId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateTrainTime()
        {
            RehaTrain__TrainTimeValidationRule oDobDateRule = new RehaTrain__TrainTimeValidationRule();
            oDobDateRule.dtTrainTime = dtTrainTime;
            oDobDateRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
            oDobDateRule.ErrorType = ErrorType.Warning;
            this.dxValidationProvider.SetValidationRule(dtTrainTime, oDobDateRule);
        }

        private void ValidControl()
        {
            try
            {
                ValidateTrainTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderControl__ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
