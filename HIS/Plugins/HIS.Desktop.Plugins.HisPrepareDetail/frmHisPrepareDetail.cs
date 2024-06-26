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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisPrepareApprove.ADO;
using HIS.Desktop.Plugins.HisPrepareApprove.Validtion;
using TYT.EFMODEL.DataModels;
using TYT.Filter;
using MOS.Filter;
using MOS.SDO;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisPrepareDetail
{
    public partial class frmHisPrepareDetail : HIS.Desktop.Utility.FormBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        long prepareId;
        int positionHandleControl = -1;
        V_HIS_PREPARE currentPrepare = null;
        List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY> currentPrepareMetyList = new List<V_HIS_PREPARE_METY>();
        List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_MATY> currentPrepareMatyList = new List<V_HIS_PREPARE_MATY>();

        public frmHisPrepareDetail()
        {
            InitializeComponent();
        }

        public frmHisPrepareDetail(Inventec.Desktop.Common.Modules.Module _currentModule, long _prepareId)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.prepareId = _prepareId;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisPrepareDetail_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                GetPrepare();

                FillDataToGridControlView();

                ValidateControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPrepare()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPrepareViewFilter filter = new HisPrepareViewFilter();
                filter.ID = this.prepareId;
                var results = new BackendAdapter(param).Get<List<V_HIS_PREPARE>>("api/HisPrepare/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (results != null && results.Count() > 0)
                {
                    currentPrepare = results.FirstOrDefault();
                    lblApprovalLogginName.Text = currentPrepare.CREATOR;
                    lblDescription.Text = currentPrepare.DESCRIPTION;
                    lblUseFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentPrepare.FROM_TIME ?? 0);
                    lblUseTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentPrepare.TO_TIME ?? 0);
                }
                else
                {
                    lblUseTo.Text = "";
                    lblUseFrom.Text = "";
                    lblDescription.Text = "";
                    lblApprovalLogginName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FillDataToGridControlView()
        {
            try
            {
                WaitingManager.Show();

                List<MetyMatyADO> medicineADOList = new List<MetyMatyADO>();
                List<MetyMatyADO> materialADOList = new List<MetyMatyADO>();
                CommonParam paramCommon = new CommonParam();

                // get mety
                MOS.Filter.HisPrepareMetyViewFilter filterMety = new HisPrepareMetyViewFilter();
                filterMety.PREPARE_ID = this.currentPrepare.ID;
                filterMety.ORDER_DIRECTION = "ASC";
                filterMety.ORDER_FIELD = "MEDICINE_TYPE_NAME";
                var prepareMetyList = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY>>("api/HisPrepareMety/GetView", ApiConsumers.MosConsumer, filterMety, paramCommon);

                if (prepareMetyList != null && prepareMetyList.Count() > 0)
                {
                    foreach (var item in prepareMetyList)
                    {
                        MetyMatyADO MetyMatyADO = new MetyMatyADO(item);
                        medicineADOList.Add(MetyMatyADO);
                    }
                }

                // get maty
                MOS.Filter.HisPrepareMatyViewFilter filterMaty = new HisPrepareMatyViewFilter();
                filterMaty.PREPARE_ID = this.prepareId;
                filterMaty.ORDER_DIRECTION = "ASC";
                filterMaty.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                var prepareMatyList = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_MATY>>("api/HisPrepareMaty/GetView", ApiConsumers.MosConsumer, filterMaty, paramCommon);

                if (prepareMatyList != null && prepareMatyList.Count() > 0)
                {
                    foreach (var item in prepareMatyList)
                    {
                        MetyMatyADO MetyMatyADO = new MetyMatyADO(item);
                        materialADOList.Add(MetyMatyADO);
                    }
                }

                gridViewMedicine.BeginUpdate();
                if (medicineADOList != null)
                {
                    gridViewMedicine.GridControl.DataSource = medicineADOList;
                }
                gridViewMedicine.EndUpdate();

                gridViewMaterial.BeginUpdate();
                if (materialADOList != null)
                {
                    gridViewMaterial.GridControl.DataSource = materialADOList;
                }
                gridViewMaterial.EndUpdate();


                if (medicineADOList != null && medicineADOList.Count() > 0)
                {
                    xtraTabControl.SelectedTabPage = xtraTabPageMedicine;
                }
                else
                {
                    xtraTabControl.SelectedTabPage = xtraTabPageMaterial;
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnTuChoiDuyet_Click(object sender, EventArgs e)
        {
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Refesh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidateControl()
        {
            ValidMaxlengthTxtTick();
            ValidMaxlengthTxtGhiChu();
        }

        void ValidMaxlengthTxtTick()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.maxLength = 20;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtGhiChu()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.maxLength = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void gridLookUpEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void gridViewView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MetyMatyADO pData = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "REQ_NAME_STR")
                    {
                        e.Value = pData.REQ_LOGINNAME + " - " + pData.REQ_USERNAME;
                    }
                    else if (e.Column.FieldName == "APPROVAL_NAME_STR")
                    {
                        e.Value = pData.APPROVAL_LOGINNAME + " - " + pData.APPROVAL_USERNAME;
                    }

                    gridControlMedicine.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterial_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MetyMatyADO pData = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "REQ_NAME_STR")
                    {
                        e.Value = pData.REQ_LOGINNAME + " - " + pData.REQ_USERNAME;
                    }
                    else if (e.Column.FieldName == "APPROVAL_NAME_STR")
                    {
                        e.Value = pData.APPROVAL_LOGINNAME + " - " + pData.APPROVAL_USERNAME;
                    }

                    gridControlMedicine.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
