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
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.LocalData;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.AncillaryServicePaty.frmAncill
{
    public partial class frmAncill : DevExpress.XtraEditors.XtraForm
    {

        #region init variables
        Inventec.Desktop.Common.Modules.Module moduleData;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        List<HIS_PATIENT_TYPE> listPatientType;
        List<V_HIS_SERVICE> listService;
        List<V_HIS_SERVICE> listChildService;
        V_HIS_SERVICE selectedService = new V_HIS_SERVICE();
        V_HIS_SERVICE selectedChilService = new V_HIS_SERVICE();
        HIS_PATIENT_TYPE selectedDTTT = new HIS_PATIENT_TYPE();
        V_HIS_ANCILLARY_SERV_PATY currentData = new V_HIS_ANCILLARY_SERV_PATY();
        int actionType = -1;
        #endregion
        public frmAncill(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            this.moduleData = moduleData;
            InitializeComponent();
            this.Text = moduleData.text;
        }

        private void frmAncill_Load(object sender, EventArgs e)
        {
            try
            {
                //load data
                FillDataToControl();
                //load data to combo
                LoadDataToCombo();
                //default value
                SetDefaultValue();
                //set languages resource
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void frmAncill_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    btnSearch.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.S)
                {
                    btnEdit.PerformClick();
                } if (e.Control && e.KeyCode == Keys.N)
                {
                    btnSave.PerformClick();
                } if (e.Control && e.KeyCode == Keys.R)
                {
                    btnReset.PerformClick();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        #region load languages resource
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AncillaryServicePaty.Resources.Lang", typeof(frmAncill).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAncill.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmAncill.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAncill.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmAncill.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAncill.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmAncill.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAncill.txtSearchValue.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAncill.txtSearchValue.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbboServiceName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAncill.cbboServiceName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbboDTTT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAncill.cbboDTTT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbboChildService.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAncill.cbboChildService.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmAncill.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAncill.layoutControlItem6.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmAncill.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAncill.layoutControlItem7.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmAncill.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmAncill.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmAncill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region default value
        private void SetDefaultValue()
        {
            try
            {
                
                txtServiceCode.Text = "";
                txtChildServiceCode.Text = "";
                txtBhytCode.Text = "";

                this.currentData = null;
                this.selectedService = null;
                this.selectedChilService = null;
                this.selectedDTTT = null;

                cbboChildService.EditValue = null;
                cbboDTTT.EditValue = null;
                cbboServiceName.EditValue = null;

                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                this.actionType = GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
        #region load data for list
        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();


                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.grcListService);
                
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_ANCILLARY_SERV_PATY>> apiResult = null;
                HisAncillaryServPatyViewFilter filter = new HisAncillaryServPatyViewFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                SetFilter(ref filter);
                grcListService.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_ANCILLARY_SERV_PATY>>("/api/HisAncillaryServPaty/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    //LogSystem.Debug("____data lay được: " + LogUtil.TraceData("V_HIS_ANCILLARY_SERV_PATY: ", data));
                    if (data != null)
                    {

                        grvListService.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        grcListService.DataSource = null;
                        MessageManager.Show(this, paramCommon, false);
                    }
                }
                grcListService.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisAncillaryServPatyViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearchValue.Text.Trim()))
                {
                    filter.KEY_WORD = txtSearchValue.Text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceViewFilter filter = new HisServiceViewFilter();
                filter.IS_ACTIVE = 1;
                var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE>>("/api/HisService/GetView", ApiConsumers.MosConsumer, filter, param);
                if (data != null)
                {
                    listService = data.Where(s => s.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                                    && s.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                                    && s.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                                                    && s.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
                                                    && s.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                                                    && s.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                                                    .ToList();
                    //Load to service
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(cbboServiceName, listService, controlEditorADO);

                    //load to dich vu di kem
                    listChildService = data.Where(s => s.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                                    || s.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                                                    .ToList();
                    List<ColumnInfo> columnInfosDV = new List<ColumnInfo>();
                    columnInfosDV.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                    columnInfosDV.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorDVADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfosDV, false, 350);
                    ControlEditorLoader.Load(cbboChildService, listChildService, controlEditorDVADO);

                }
                // load doi tuong thanh toan

                listPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(s => s.IS_RATION != 1 && s.IS_ACTIVE == 1).ToList();
                if (listPatientType != null)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(cbboDTTT, listPatientType, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void FillDataToEditControl()
        {
            try
            {
                if (this.currentData == null) return;
                dxErrorProvider1.ClearErrors();
                btnEdit.Enabled = true;
                btnSave.Enabled = false;
                this.actionType = GlobalVariables.ActionEdit;
                cbboServiceName.EditValue = this.currentData.SERVICE_ID;
                cbboDTTT.EditValue = this.currentData.PATIENT_TYPE_ID;
                txtBhytCode.Text = this.currentData.PREFIX_BHYT_CODE;

                txtChildServiceCode.Text = this.currentData.CHILD_SERVICE_CODE;
                cbboChildService.EditValue = this.currentData.CHILD_SERVICE_ID;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
        #region handle value changed
        private void cbboServiceName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbboServiceName.EditValue != null)
                {
                    this.selectedService = listService.FirstOrDefault(s=>s.ID == Convert.ToInt64(cbboServiceName.EditValue));
                    txtServiceCode.Text = this.selectedService.SERVICE_CODE;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void txtServiceCode_Validated(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtServiceCode.Text.Trim()))
                {
                    var rs = listService.FirstOrDefault(s => s.SERVICE_CODE == txtServiceCode.Text || s.SERVICE_CODE == txtServiceCode.Text.ToUpper());
                    if (rs != null)
                    {
                        cbboServiceName.EditValue = rs.ID;
                        txtServiceCode.Text = rs.SERVICE_CODE;
                        cbboDTTT.Focus();
                        cbboDTTT.ShowPopup();
                    }
                    else
                    {
                        txtServiceCode.Text = "";
                        //cbboServiceName.Focus();
                        cbboServiceName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void cbboChildService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbboServiceName.EditValue == null) dxErrorProvider1.SetError(cbboServiceName, "");
                if (cbboChildService.EditValue != null)
                {
                    this.selectedChilService = listChildService.FirstOrDefault(s => s.ID == Convert.ToInt64(cbboChildService.EditValue));
                    txtChildServiceCode.Text = selectedChilService.SERVICE_CODE;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void txtChildServiceCode_Validated(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtChildServiceCode.Text.Trim()))
                {
                    var rs = listChildService.FirstOrDefault(s => s.SERVICE_CODE == txtChildServiceCode.Text || s.SERVICE_CODE == txtChildServiceCode.Text.ToUpper());
                    if (rs != null)
                    {
                        cbboChildService.EditValue = rs.ID;
                        txtChildServiceCode.Text = rs.SERVICE_CODE;
                        btnEdit.Focus();
                    }
                    else
                    {
                        txtChildServiceCode.Text = "";
                        cbboChildService.Focus();
                        cbboChildService.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        
        private void cbboDTTT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbboDTTT.EditValue != null)
                {
                    this.selectedDTTT = listPatientType.FirstOrDefault(s => s.ID == Convert.ToInt64(cbboDTTT.EditValue));
                    if (selectedDTTT != null)
                    {
                        txtBhytCode.Focus();
                        txtBhytCode.SelectAll();
                    }
                    else
                    {
                        cbboDTTT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        #endregion
        #region validate
        
        private void Validate(Control control, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    if (string.IsNullOrEmpty(control.Text))
                    {
                        dxErrorProvider1.SetError(control, "Trường dữ liệu bắt buộc", ErrorType.Warning);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        #endregion
        #region custom dataTotal
        private void grvListService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_HIS_ANCILLARY_SERV_PATY pData = (V_HIS_ANCILLARY_SERV_PATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void grvListService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_ANCILLARY_SERV_PATY data = (V_HIS_ANCILLARY_SERV_PATY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnlock);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnEDelete : btnDDelete);

                    }

                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        #endregion
        #region handle button click
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Validate(txtServiceCode, true);
                Validate(cbboDTTT, true);
                if (dxErrorProvider1.HasErrors || !btnSave.Enabled) return;
                WaitingManager.Show();
                grcListService.BeginUpdate();
                ProcessSave();
                grcListService.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Validate(txtServiceCode, true);
                Validate(cbboDTTT, true);
                if (dxErrorProvider1.HasErrors || !btnEdit.Enabled) return;
                WaitingManager.Show();
                grcListService.BeginUpdate();
                ProcessSave();
                grcListService.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void ProcessSave()
        {
            try 
	        {
                bool sucess = false;
                CommonParam param =new CommonParam();
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    HIS_ANCILLARY_SERV_PATY updateDTO = new HIS_ANCILLARY_SERV_PATY();
                    UpdateDataDTO(ref updateDTO);
                    LogSystem.Debug("____input data to CREATE: " + LogUtil.TraceData("DATA DTO:", updateDTO));
                    var data = new BackendAdapter(param).Post<HIS_ANCILLARY_SERV_PATY>("/api/HisAncillaryServPaty/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    //.Debug("______RECIVED data to UPDATE: " + LogUtil.TraceData("DATA recived:", data));
                    if (data != null)
                    {
                        FillDataToControl();
                        sucess = true;
                        SetDefaultValue();
                    }

                }
                else if(this.actionType == GlobalVariables.ActionEdit)
                {
                    HIS_ANCILLARY_SERV_PATY updateDTO = new HIS_ANCILLARY_SERV_PATY();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ANCILLARY_SERV_PATY>(updateDTO, this.currentData);
                    UpdateDataDTO(ref updateDTO);
                    LogSystem.Debug("____input data to UPDATE: " + LogUtil.TraceData("DATA DTO:", updateDTO));
                    var data = new BackendAdapter(param).Post<HIS_ANCILLARY_SERV_PATY>("/api/HisAncillaryServPaty/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    //LogSystem.Debug("______RECIVED data to UPDATE: " + LogUtil.TraceData("DATA recived:", data));
                    if (data != null)
                    {
                        FillDataToControl();
                        sucess = true;
                        SetDefaultValue();
                    }
                }

                MessageManager.Show(this, param, sucess);
               
	        }
	        catch (Exception ex)
	        {
                LogSystem.Debug(ex);
	        }
        }

        private void UpdateDataDTO(ref HIS_ANCILLARY_SERV_PATY updateDTO)
        {
            try
            {
                if (this.selectedService != null)
                {
                    updateDTO.SERVICE_ID = this.selectedService.ID;
                }
                if (this.selectedDTTT != null)
                {
                    updateDTO.PATIENT_TYPE_ID = this.selectedDTTT.ID;
                }
                if (this.selectedChilService != null)
                {
                    updateDTO.CHILD_SERVICE_ID = this.selectedChilService.ID;
                }
                updateDTO.PREFIX_BHYT_CODE = txtBhytCode.Text;
                
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        #endregion
        #region handle grid click
        private void grvListService_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_ANCILLARY_SERV_PATY)grvListService.GetFocusedRow();
                if (rowData != null)
                {
                    this.currentData = rowData;
                    FillDataToEditControl();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_ANCILLARY_SERV_PATY)grvListService.GetFocusedRow();
                bool sucess = false;
                CommonParam param = new CommonParam();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage
                    (HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                    "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    HIS_ANCILLARY_SERV_PATY tempData = new HIS_ANCILLARY_SERV_PATY();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ANCILLARY_SERV_PATY>(tempData,rowData);
                    tempData.ID = rowData.ID;
                    tempData.IS_ACTIVE = 1;
                    var res = new BackendAdapter(param).Post<HIS_ANCILLARY_SERV_PATY>("/api/HisAncillaryServPaty/ChangeLock", ApiConsumers.MosConsumer, tempData.ID, param);
                    if (res != null)
                    {
                        sucess = true;
                        FillDataToControl();
                        SetDefaultValue();
                    }
                    MessageManager.Show(this, param, sucess);
                }
                else return;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void btnGUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_ANCILLARY_SERV_PATY)grvListService.GetFocusedRow();
                bool sucess = false;
                CommonParam param = new CommonParam();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage
                    (HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                    "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    HIS_ANCILLARY_SERV_PATY tempData = new HIS_ANCILLARY_SERV_PATY();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ANCILLARY_SERV_PATY>(tempData,rowData);
                    tempData.ID = rowData.ID;
                    tempData.IS_ACTIVE = 0;
                    var res = new BackendAdapter(param).Post<HIS_ANCILLARY_SERV_PATY>("/api/HisAncillaryServPaty/ChangeLock", ApiConsumers.MosConsumer, tempData.ID, param);
                    if (res != null)
                    {
                        sucess = true;
                        FillDataToControl();
                        SetDefaultValue();
                    }
                    MessageManager.Show(this, param, sucess);
                }
                else return;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void btnEDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_ANCILLARY_SERV_PATY)grvListService.GetFocusedRow();
                bool sucess = false;
                CommonParam param = new CommonParam();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage
                    (HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                    "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    
                    sucess = new BackendAdapter(param).Post<bool>("/api/HisAncillaryServPaty/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                    if (sucess)
                    {
                        FillDataToControl();
                        SetDefaultValue();
                    }
                    MessageManager.Show(this, param, sucess);
                }
                else return;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }
        #endregion

        private void txtServiceCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    if (!string.IsNullOrEmpty(txtServiceCode.Text.Trim()))
                    {
                        var rs = listService.FirstOrDefault(s => s.SERVICE_CODE == txtServiceCode.Text || s.SERVICE_CODE == txtServiceCode.Text.ToUpper());
                        if (rs != null)
                        {
                            cbboServiceName.EditValue = rs.ID;
                            txtServiceCode.Text = rs.SERVICE_CODE;
                            cbboDTTT.Focus();
                            cbboDTTT.ShowPopup();
                        }
                        else
                        {
                            txtServiceCode.Text = "";
                            //cbboServiceName.Focus();
                            cbboServiceName.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void cbboDTTT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    if (cbboDTTT.EditValue != null)
                    {
                        this.selectedDTTT = listPatientType.FirstOrDefault(s => s.ID == Convert.ToInt64(cbboDTTT.EditValue));
                        if (selectedDTTT != null)
                        {
                            txtBhytCode.Focus();
                            txtBhytCode.SelectAll();
                        }
                        else
                        {
                            cbboDTTT.ShowPopup();
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void txtBhytCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtChildServiceCode.Focus();
                    txtChildServiceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void txtChildServiceCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    if (!string.IsNullOrEmpty(txtChildServiceCode.Text.Trim()))
                    {
                        var rs = listChildService.FirstOrDefault(s => s.SERVICE_CODE == txtChildServiceCode.Text || s.SERVICE_CODE == txtChildServiceCode.Text.ToUpper());
                        if (rs != null)
                        {
                            cbboChildService.EditValue = rs.ID;
                            txtChildServiceCode.Text = rs.SERVICE_CODE;
                            btnEdit.Focus();
                        }
                        else
                        {
                            txtChildServiceCode.Text = "";
                            cbboChildService.Focus();
                            cbboChildService.ShowPopup();
                        }
                    }
                    else
                    {
                        if (btnEdit.Enabled)
                        {
                            btnEdit.Focus();
                        }
                        else btnSave.Focus();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }

        private void txtSearchValue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter )
                {
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }
        }


        

        

        
    }
}
