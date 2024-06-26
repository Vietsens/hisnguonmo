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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.Paging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using System.Threading;
using System.IO;
using Inventec.Common.RichEditor.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraBars;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.ListPresBloodConfirm.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.SDO;
using DevExpress.Utils.Menu;

namespace HIS.Desktop.Plugins.ListPresBloodConfirm
{
    public partial class UCListPresBloodConfirm : UserControl
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        long impMestTypeId = 0;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        private string LoggingName = "";
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        public MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 ImpMest, ImpMestEdit;
        internal MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 ViewImportMest;
        public Base.HisCommonImpMestTypeInfo hisCommonImpMestTypeInfo;
        internal V_HIS_EXP_MEST_2 currentImpMestRightClick { get; set; }

        List<V_HIS_BID> listBid;
        ToolTip toolTip = new ToolTip();
        V_HIS_ROOM room;

        List<HIS_PATIENT_TYPE> _PatientTypeSelecteds;

        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        Inventec.Desktop.Common.Modules.Module currentModule;
        //HIS_TREATMENT treatment = null;
        internal string typeCodeFind__KeyWork_InDate = "Ngày";
        internal string typeCodeFind_InDate = "Ngày";
        internal string typeCodeFind__InMonth = "Tháng";
        #endregion

        #region Construct
        public UCListPresBloodConfirm()
        {
            InitializeComponent();
            try
            {
                //FillDataNavStatus();
                //FillDataNavType();
                gridControlExpMestList.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCListPresBloodConfirm(Inventec.Desktop.Common.Modules.Module _module)
            : this()
        {
            try
            {
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;

                medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == this.roomId && o.ROOM_TYPE_ID == this.roomTypeId);
                room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCListPresBloodConfirm(Inventec.Desktop.Common.Modules.Module _module, long impMestTypeId, MobaImpMestListADO mobaImpMestListADO)
            : this()
        {
            try
            {
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;

                medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == this.roomId && o.ROOM_TYPE_ID == this.roomTypeId);
                room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                this.impMestTypeId = impMestTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisImportMestMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                //GetBid();
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }

                //Gan ngon ngu
                LoadKeysFromlanguage();

                InitTypeFind();

                InitCheck(cboPatientType, SelectionGrid__Status);
                InitCombo(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), "PATIENT_TYPE_NAME", "ID");
                //Gan gia tri mac dinh
                SetDefaultValueControl();

                InitComboPayStt();

                //if (mobaImpMestListADO != null)
                //{
                //    txtKeyWord.Text = mobaImpMestListADO.TreatmentCode;                    
                //}
                //Load du lieu
                FillDataExpMestList();
                //Ản hiện layout search
                txtExpCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _PatientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                foreach (HIS_PATIENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _PatientTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFilterPatientType(ref MOS.Filter.HisExpMestView2Filter filter)
        {
            try
            {
                if (_PatientTypeSelecteds != null && _PatientTypeSelecteds.Count > 0)
                {
                    filter.TDL_PATIENT_TYPE_IDs = _PatientTypeSelecteds.Select(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPayStt()
        {
            try
            {
                List<Base.PaySttADO> data = new List<PaySttADO>()
                {
                    new Base.PaySttADO(1,"Thanh toán"),
                    //new Base.PaySttADO(2,"Đang thanh toán"),
                    new Base.PaySttADO(3,"Chưa thanh toán")
                };

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ListPresBloodConfirm.Resources.Lang", typeof(HIS.Desktop.Plugins.ListPresBloodConfirm.UCListPresBloodConfirm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtExpCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.txtImpCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCImpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCImpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCReqLoginName.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCReqLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCReqDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCReqDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCCreator.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCModifier.Caption = Inventec.Common.Resource.Get.Value("UCListPresBloodConfirm.gCModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                btnCodeFind.Text = typeCodeFind_InDate;
                this.typeCodeFind__KeyWork_InDate = btnCodeFind.Text;
                FormatDtIntructionDate();

                dtIntructionDate.DateTime = DateTime.Now;

                cboIsConform.SelectedIndex = 0;

                cboPatientType.Enabled = false;
                cboPatientType.Enabled = true;

                txtKeyWord.Text = "";
                txtExpCode.Text = "";
                txtServiceReqCode.Text = "";
                txtExpCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBid()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidViewFilter bidFilter = new HisBidViewFilter();
                bidFilter.IS_ACTIVE = 1;
                listBid = new BackendAdapter(param).Get<List<V_HIS_BID>>("api/HisBid/GetView", ApiConsumers.MosConsumer, bidFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataExpMestList()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                ExpMestPaging(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(ExpMestPaging, param, numPageSize, this.gridControlExpMestList);
                WaitingManager.Hide();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ExpMestPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>> apiResult = null;
                MOS.Filter.HisExpMestView2Filter filter = new MOS.Filter.HisExpMestView2Filter();
                SetFilterExpMest(ref filter);
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM;

                gridViewExpMestList.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Info("Filter HisExpMest/GetView2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>>
                    ("api/HisExpMest/GetView2", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlExpMestList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlExpMestList.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewExpMestList.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInDateCode = new DXMenuItem(typeCodeFind__KeyWork_InDate, new EventHandler(btnCodeFind_Click));
                itemInDateCode.Tag = "InDate";
                menu.Items.Add(itemInDateCode);

                DXMenuItem itemInMonth = new DXMenuItem(typeCodeFind__InMonth, new EventHandler(btnCodeFind_Click));
                itemInMonth.Tag = "InMonth";
                menu.Items.Add(itemInMonth);

                btnCodeFind.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_InDate = btnMenuCodeFind.Caption;

                FormatDtIntructionDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FormatDtIntructionDate()
        {
            try
            {
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                {
                    dtIntructionDate.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtIntructionDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtIntructionDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtIntructionDate.Properties.EditMask = "dd/MM/yyyy";
                    dtIntructionDate.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else
                {
                    dtIntructionDate.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtIntructionDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtIntructionDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtIntructionDate.Properties.EditMask = "MM/yyyy";
                    dtIntructionDate.Properties.Mask.EditMask = "MM/yyyy";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void GetTreatment(string treatmentCode)
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        HisTreatmentFilter filter = new HisTreatmentFilter();
        //        filter.TREATMENT_CODE__EXACT = treatmentCode;

        //        var listTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
        //        if (listTreatment != null && listTreatment.Count > 0)
        //        {
        //            this.treatment = listTreatment.FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void SetFilterExpMest(ref MOS.Filter.HisExpMestView2Filter filter)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtExpCode.Text))
                {
                    string code = txtExpCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpCode.Text = code;
                    }
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.EXP_MEST_CODE__EXACT = code;
                }
                else if (!String.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.TDL_PATIENT_CODE__EXACT = code;
                    filter.WORKING_ROOM_ID = roomId;
                }
                else if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.TDL_TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.TDL_SERVICE_REQ_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate
                   && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InMonth
                        && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_MONTH__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMM") + "00000000");
                    }

                    if (cboIsConform.SelectedIndex == 0)
                    {
                        filter.IS_CONFIRM = null;
                    }
                    else if (cboIsConform.SelectedIndex == 1)
                    {
                        filter.IS_CONFIRM = false;
                    }
                    else
                    {
                        filter.IS_CONFIRM = true;
                    }

                    SetFilterPatientType(ref filter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilterDocumentDate(ref MOS.Filter.HisExpMestView2Filter filter)
        {
            try
            {
                //if (layoutControlGroupType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                //{
                //    if (!string.IsNullOrEmpty(txtDocumentNumber.Text))
                //    {
                //        filter.DOCUMENT_NUMBER__EXACT = txtDocumentNumber.Text;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataExpMestList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetCombo(cboPatientType);
                SetDefaultValueControl();
                FillDataExpMestList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtImpCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpCode.Text))
                    {
                        FillDataExpMestList();
                    }
                    else
                    {

                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExpMestList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlExpMestList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "IMP_MEST_STT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "IMP_MEST_STT_ICON")
                            {
                                short? isConfirm = null;
                                if (!String.IsNullOrWhiteSpace((view.GetRowCellValue(lastRowHandle, "IS_CONFIRM") ?? "").ToString()))
                                {
                                    isConfirm = Convert.ToInt16((view.GetRowCellValue(lastRowHandle, "IS_CONFIRM") ?? "").ToString());
                                }
                                text = isConfirm == 1 ? "Đã chốt" : "Chưa chốt";
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImportMestList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "IMP_MEST_STT_ICON")// Trạng thái: Đã chốt (IS_CONFIRM = 1) Màu xanh, Chưa chốt (IS_CONFIRM <> 1) Màu vàng.
                        {
                            if (data.IS_CONFIRM == (short)1)
                            {
                                e.Value = imageListStatus.Images[6];
                            }
                            else
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CONFIRM_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CONFIRM_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                        else if (e.Column.FieldName == "CONFIRM_DISPLAY")
                        {
                            string Req_loginName = data.CONFIRM_LOGINNAME;
                            string Req_UserName = data.CONFIRM_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImportMestList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_EXP_MEST_2)gridViewExpMestList.GetRow(e.RowHandle);
                    long statusIdCheckForButtonEdit = long.Parse((gridViewExpMestList.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    long typeIdCheckForButtonEdit = long.Parse((gridViewExpMestList.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewExpMestList.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    string creator = (gridViewExpMestList.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (e.Column.FieldName == "APPROVAL_DISPLAY")//duyệt
                    {
                        if (data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)
                            {
                                if (medistock != null && medistock.ID == mediStockId &&
                                    (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST))
                                {
                                    //if (data != null && data.AGGR_IMP_MEST_ID != null && data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL && data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                                    //{
                                    //    e.RepositoryItem = repositoryItemButtonApprovalDisable;
                                    //}
                                    //else
                                    //{
                                    //    e.RepositoryItem = repositoryItemButtonApprovalEnable;
                                    //}
                                }
                                else
                                    e.RepositoryItem = repositoryItemButtonApprovalDisable;
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonApprovalDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY")//hủy
                    {
                        if (data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT) && typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL
                                && data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    //11770 cho phép xóa đơn pk trả lại ở kho
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                    }
                                }
                                else
                                {
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                    }
                                }
                                //    {
                                //if (typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH)
                                //{
                                //    if (medistock != null && medistock.ID == mediStockId)
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                //    }
                                //    else
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                //    }
                                //}
                                //else
                                //{
                                //    if (medistock != null && medistock.ID == mediStockId)
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardEnable;
                                //    }
                                //    else
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonDiscardDisable;
                                //    }
                                //}
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonDiscardDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDiscardDisable;
                        }
                    }

                    else if (e.Column.FieldName == "EditNCC")//Sửa thông tin nhập NCC
                    {
                        if (data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                                (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT))
                            {
                                e.RepositoryItem = Btn_EditInfoImpMestNCC_Enable;
                            }
                            else
                                e.RepositoryItem = Btn_EditInfoImpMestNCC_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_EditInfoImpMestNCC_Disable;
                        }
                    }

                    else if (e.Column.FieldName == "CreateExpNCC")//Xuất trả ncc
                    {
                        if (data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE && data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                e.RepositoryItem = Btn_ExportNCC_Enable;
                            }
                            else
                                e.RepositoryItem = Btn_ExportNCC_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_ExportNCC_Disable;
                        }
                    }

                    else if (e.Column.FieldName == "IMPORT_DISPLAY")// thực nhập
                    {
                        if (data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                               statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                               && impMestTypeId != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT && data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                //if (data != null && data.AGGR_IMP_MEST_ID != null && data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                //{
                                //    if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnImport) != null)
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonActualImportDisable;
                                //    }
                                //    else
                                //    {
                                //        e.RepositoryItem = repositoryItemButtonActualImportEnable;
                                //    }
                                //}
                                //else
                                //{
                                //    e.RepositoryItem = repositoryItemButtonActualImportEnable;
                                //}

                            }
                            else if (
                                statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                                && impMestTypeId != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT && data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    //if ((data != null && data.AGGR_IMP_MEST_ID != null && data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL) && !CheckLoginAdmin.IsAdmin(LoggingName))
                                    //{
                                    //    e.RepositoryItem = Btn_Cancel_Import_Disable;
                                    //}
                                    //else
                                    //{
                                    //    e.RepositoryItem = Btn_Cancel_Import_Enable;
                                    //}
                                }
                                else
                                    e.RepositoryItem = Btn_Cancel_Import_Disable;
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonActualImportDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonActualImportDisable;
                        }
                    }
                    else if (e.Column.FieldName == "EDIT")// sửa
                    {
                        if (data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                                 && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                                 &&
                                (typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH &&
                                typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK && data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT))
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditEnable;
                                    }
                                }
                                else
                                {
                                    if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                                        || typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonEditEnable;
                                    }
                                }
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonEditDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// chốt/Hủy chốt
                    {
                        if (data.IS_CONFIRM == (short)1)
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_HuyChot;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_Chot;
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                    {
                        if (data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE) && data.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                //if (data != null && data.AGGR_IMP_MEST_ID != null && data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                //{
                                //    e.RepositoryItem = repositoryItemButtonRequestDisable;
                                //}
                                //else
                                //{
                                //    e.RepositoryItem = repositoryItemButtonRequest;
                                //}
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonRequestDisable;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonRequestDisable;
                        }
                    }
                    else if (e.Column.FieldName == "PRINT")// In
                    {
                        if (data.IS_CONFIRM == (short)1)
                            e.RepositoryItem = repositoryItemButtonEdit_Print_Enable;
                        else
                            e.RepositoryItem = repositoryItemButtonEdit_Print_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string DisplayName(string loginname, string username)
        {
            string value = "";
            try
            {
                if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
                {
                    value = "";
                }
                else if (loginname != "" && username == "")
                {
                    value = loginname;
                }
                else if (loginname == "" && username != "")
                {
                    value = username;
                }
                else if (loginname != "" && username != "")
                {
                    value = string.Format("{0} - {1}", loginname, username);
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return value;
            }
        }
        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusCode()
        {
            try
            {
                txtExpCode.Focus();
                txtExpCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region report
        private void CreateReport()
        {
            try
            {
                List<string> expCode = new List<string>();
                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();
                    string fileName = "";

                    string direct = System.IO.Path.Combine(FileLocalStore.Rootpath, "ExportListExportCode");

                    string[] fileEntries = Directory.EnumerateFiles(direct, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx")).ToArray();

                    foreach (string file in fileEntries)
                    {
                        fileName = file;
                    }

                    if (String.IsNullOrEmpty(fileName))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayBieuMauIn, System.IO.Path.Combine(FileLocalStore.Rootpath, "ExportListExportCode")));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BieuMauDangMo);
                        return;
                    }

                    GetDataProcessor(ref expCode);

                    ProcessData(expCode, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.TaiThanhCong);

                                if (MessageBox.Show(Resources.ResourceMessage.BanCoMuonMoFile,
                                    Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.XuLyThatBai);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<string> expCode, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                List<Base.ExportListCodeRDO> listRdo = new List<Base.ExportListCodeRDO>();
                Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();

                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);

                if (expCode != null && expCode.Count > 0)
                {
                    Dictionary<int, List<string>> dicExpCode = new Dictionary<int, List<string>>();

                    int count = expCode.Count;
                    int max = count / 6;
                    int size = count % 6;
                    string emty = "";

                    if (count > 31)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (i != 5)
                            {
                                dicExpCode[i] = new List<string>();
                                dicExpCode[i].AddRange(expCode.GetRange(0, (size <= 0 ? max : max + 1)));
                                expCode.RemoveRange(0, (size <= 0 ? max : max + 1));
                            }
                            else
                                dicExpCode.Add(i, expCode);

                            if (dicExpCode[i].Count < dicExpCode[0].Count)
                            {
                                int loop = dicExpCode[0].Count - dicExpCode[i].Count;
                                for (int j = 0; j < loop; j++)
                                {
                                    dicExpCode[i].Add(emty);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (i != 5)
                            {
                                dicExpCode[i] = new List<string>();
                                dicExpCode[i].AddRange(expCode.GetRange(0, (size <= 0 ? max : max + 1)));
                                expCode.RemoveRange(0, (size <= 0 ? max : max + 1));
                                size--;
                            }
                            else
                                dicExpCode.Add(i, expCode);

                            if (dicExpCode[i].Count < dicExpCode[0].Count)
                            {
                                dicExpCode[i].Add(emty);
                            }
                        }
                    }

                    for (int i = 0; i < dicExpCode[0].Count; i++)
                    {
                        Base.ExportListCodeRDO a = new Base.ExportListCodeRDO();
                        a.EXPORT_CODE1 = dicExpCode[0][i];
                        a.EXPORT_CODE2 = dicExpCode[1][i];
                        a.EXPORT_CODE3 = dicExpCode[2][i];
                        a.EXPORT_CODE4 = dicExpCode[3][i];
                        a.EXPORT_CODE5 = dicExpCode[4][i];
                        a.EXPORT_CODE6 = dicExpCode[5][i];

                        listRdo.Add(a);
                    }
                }
                var mediStockName = medistock != null ? medistock.MEDI_STOCK_NAME : room.ROOM_NAME;
                singleTag.AddSingleKey(store, "TYPE", "THỰC NHẬP");
                singleTag.AddSingleKey(store, "MEDI_STOCK_NAME", mediStockName.ToUpper());
                //singleTag.AddSingleKey(store, "EXP_TIME_FROM", dtImpTimeFrom.DateTime.ToString("dd/MM/yyyy"));
                //singleTag.AddSingleKey(store, "EXP_TIME_TO", dtImpTimeTo.DateTime.ToString("dd/MM/yyyy"));
                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(singleValueDictionary);
                singleTag.ProcessData(store, singleValueDictionary);

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "List", listRdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void GetDataProcessor(ref List<string> expCode)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestView2Filter impFilter = new MOS.Filter.HisExpMestView2Filter();

                impFilter.EXP_MEST_TYPE_IDs = new List<long>();
                impFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE);

                impFilter.EXP_MEST_TYPE_IDs = new List<long>();
                impFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS);
                //impFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL);
                ////impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DQH);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL);
                ////impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HCS);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT);

                if (medistock != null)
                {
                    impFilter.MEDI_STOCK_ID = medistock.ID;
                }

                //if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                //    expFilter.IMP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                //        dtImpTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                //if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                //    expFilter.IMP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                //        dtImpTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>>(ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (exportList != null && exportList.Count > 0)
                {
                    expCode = exportList.Select(s => s.EXP_MEST_CODE).OrderBy(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                expCode = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewImportMestList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            //long servicePatyId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServicePaty.GetRowCellValue(hi.RowHandle, "ServicePatyId") ?? "").ToString());

                            //bool checkDontSell = Inventec.Common.TypeConvert.Parse.ToBoolean((gridViewServicePaty.GetRowCellValue(hi.RowHandle, "CheckDontSell") ?? "").ToString());
                            //if (servicePatyId > 0)
                            //{
                            //    return;
                            //}

                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImportMestList_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var rowFocus = (V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                if (rowFocus != null)
                {
                    LoadInfoClick(rowFocus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadInfoClick(V_HIS_EXP_MEST_2 data)
        {
            try
            {
                if (data.SUPPLIER_ID != null)
                {
                    //var supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == data.SUPPLIER_ID);
                }
                //if (data.BID_ID != null)
                //{
                //    if (listBid != null && listBid.Count > 0)
                //    {
                //        var bid = listBid.FirstOrDefault(o => o.ID == data.BID_ID);
                //        if (bid != null)
                //        {
                //            lblGoiThau.Text = bid.BID_NUMBER;
                //        }
                //        else
                //        {
                //            lblGoiThau.Text = "";
                //        }
                //    }
                //}
                //else
                //{
                //    lblGoiThau.Text = "";
                //}

                toolTip.RemoveAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_EditInfoImpMestNCC_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                if (ViewImportMest != null)
                {
                    //hien thi popup chi tiet
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ViewImportMest);
                    CallModule callModule = new CallModule(CallModule.ManuImpMestEdit, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void RightMouse_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.currentImpMestRightClick != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    RightMouseClickProcessor.ModuleType type = (RightMouseClickProcessor.ModuleType)(e.Item.Tag);

                    switch (type)
                    {
                        case RightMouseClickProcessor.ModuleType.ManuExpMestCreate:
                            ManuExpMestCreateClick(this.currentImpMestRightClick);
                            break;
                        case RightMouseClickProcessor.ModuleType.ManuImpMestEdit:
                            EditInfoImportNCC(this.currentImpMestRightClick);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ManuExpMestCreateClick(V_HIS_EXP_MEST_2 impMest)
        {
            try
            {
                WaitingManager.Show();

                List<object> listArgs = new List<object>();
                Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                listArgs.Add(impMest);
                CallModule callModule = new CallModule(CallModule.ManuExpMestCreate, this.roomId, this.roomTypeId, listArgs);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void EditInfoImportNCC(V_HIS_EXP_MEST_2 impMest)
        {
            try
            {
                //hien thi popup chi tiet
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add(impMest);
                CallModule callModule = new CallModule(CallModule.ManuImpMestEdit, this.roomId, this.roomTypeId, listArgs);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewImportMestList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                //GridHitInfo hi = e.HitInfo;
                //if (hi.InRowCell)
                //{
                //    int rowHandle = gridViewImportMestList.GetVisibleRowHandle(hi.RowHandle);
                //    this.currentImpMestRightClick = (V_HIS_EXP_MEST_2)gridViewImportMestList.GetRow(rowHandle);
                //    gridViewImportMestList.OptionsSelection.EnableAppearanceFocusedCell = true;
                //    gridViewImportMestList.OptionsSelection.EnableAppearanceFocusedRow = true;
                //    if (barManager1 == null)
                //    {
                //        barManager1 = new BarManager();
                //        barManager1.Form = this;
                //    }

                //    rightMouseClickProcessor = new RightMouseClickProcessor(this.currentImpMestRightClick, RightMouse_Click, barManager1, roomId, this.LoggingName);
                //    rightMouseClickProcessor.InitMenu();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_ExportNCC_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var impMest = (V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                if (impMest != null)
                {
                    ManuExpMestCreateClick(impMest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Done_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Cancel_Import_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 VImportMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    ("api/HisImpMest/CancelImport", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataExpMestList();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_EvenLog_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var impMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                if (impMest != null)
                {
                    //hien thi popup chi tiet
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "EXP_MEST_CODE: " + impMest.EXP_MEST_CODE);
                    listArgs.Add(dataInit3);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    CallModule callModule = new CallModule(CallModule.EventLog, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_HuyTuChoiDuyet_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 VImportMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_EXP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataExpMestList();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        FillDataExpMestList();
                    }
                    else
                    {

                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtExpMestCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        FillDataExpMestList();
                    }
                    else
                    {

                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDataImportMestDetailList()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                ImportMestPagingDetail(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(ImportMestPagingDetail, param, numPageSize, this.gridControlExpMestList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ImportMestPagingDetail(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>> apiResult = null;
                MOS.Filter.HisImpMestViewDetailFilter filter = new MOS.Filter.HisImpMestViewDetailFilter();
                SetFilterImpMestDetail(ref filter);

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                gridViewExpMestList.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>>
                    ("api/HisImpMest/GetViewByDetail", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlExpMestList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlExpMestList.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewExpMestList.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterImpMestDetail(ref MOS.Filter.HisImpMestViewDetailFilter filter)
        {
            try
            {
                //if (!String.IsNullOrEmpty(txtMedicineType.Text))
                //{
                //    string code = txtMedicineType.Text.Trim();
                //    //if (code.Length < 12 && checkDigit(code))
                //    //{
                //    //    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                //    //    txtMedicineType.Text = code;
                //    //}
                //    filter.MEDICINE_TYPE_CODE__EXACT = code;
                //}
                //filter.DOCUMENT_NUMBER__EXACT = txtDocumentNumber.Text.Trim();

                ////filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;

                //if (checkEditMediStock.Checked && this.medistock != null)
                //{
                //    filter.MEDI_STOCK_ID = this.medistock.ID;
                //}
                //else if (this.medistock == null && this.room != null)
                //{
                //    filter.REQ_DEPARTMENT_ID = this.room.DEPARTMENT_ID;
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImportMestList_RowStyle(object sender, RowStyleEventArgs e)
        {
        }

        private void cboPatientType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_PatientTypeSelecteds != null && _PatientTypeSelecteds.Count > 0)
                {
                    foreach (var item in _PatientTypeSelecteds)
                    {
                        statusName += item.PATIENT_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Print_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                _CurrentExpMest = (V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                PrintProcess(PrintType.Mps000422_PHIEU_IN_TEM_DON_MAU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        V_HIS_EXP_MEST_2 _CurrentExpMest = null;
        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
        private void InPhieuTemDonMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();
                ProcessPrint(printTypeCode);

                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this._CurrentExpMest.ID;
                var lstExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, new CommonParam()).FirstOrDefault();

                HisExpMestBltyReqViewFilter expMestBltyReqfilter = new HisExpMestBltyReqViewFilter();
                expMestBltyReqfilter.EXP_MEST_ID = _CurrentExpMest.ID;

                var lstExpBltyService = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestBltyReqfilter, new CommonParam());

                WaitingManager.Hide();
                string txt = "";
                if (lstExpBltyService != null)
                {
                    foreach (var item in lstExpBltyService)
                    {
                        int amount = (int)item.AMOUNT;
                        for (int i = 0; i < amount; i++)
                        {
                            GenText(lstExpMest, item, i + 1, ref txt);
                            txt += "\n";
                        }
                        GenText(lstExpMest, item, null, ref txt);
                    }
                }
                string resultPrint = new Bartender.PrintTemBlood.PrintTemBlood().StartPrintTemBlood(txt);

                if (!string.IsNullOrWhiteSpace(resultPrint))
                {
                    Inventec.Common.Logging.LogSystem.Warn(resultPrint);
                }
                if (!string.IsNullOrWhiteSpace(resultPrint) && resultPrint.StartsWith("[ERROR]:"))
                {
                    Inventec.Common.Logging.LogSystem.Warn(resultPrint);
                    MPS.Processor.Mps000422.PDO.Mps000422PDO pdo = new MPS.Processor.Mps000422.PDO.Mps000422PDO(
                     lstExpMest,
                     lstExpBltyService
                     );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void GenText(V_HIS_EXP_MEST expMest, V_HIS_EXP_MEST_BLTY_REQ mestBLTYReq, int? index, ref string txt)
        {
            txt += expMest.TDL_PATIENT_CODE;
            txt += "," + expMest.TDL_PATIENT_DOB;
            txt += "," + (expMest.TDL_PATIENT_DOB != null && expMest.TDL_PATIENT_DOB > 10000000000000 ? expMest.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "");
            txt += "," + expMest.TDL_PATIENT_GENDER_NAME;
            txt += "," + expMest.TDL_PATIENT_NAME;
            txt += "," + mestBLTYReq.BLOOD_ABO_CODE;
            txt += "," + mestBLTYReq.BLOOD_RH_CODE;
            txt += "," + expMest.REQ_DEPARTMENT_NAME;
            txt += "," + expMest.TDL_TREATMENT_CODE;
            txt += "," + mestBLTYReq.BLOOD_TYPE_NAME;
            txt += "," + mestBLTYReq.AMOUNT;
            if (index.HasValue)
            {
                txt += "," + String.Format("{0}/{1}", index, mestBLTYReq.AMOUNT);
            }
        }

        string printerName = "";
        private void ProcessPrint(String printTypeCode)
        {
            try
            {
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._CurrentExpMest != null ? this._CurrentExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
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
                    case RequestUri.PRINT_TYPE_CODE__PHIEU_IN_TEM_DON_MAU_Mps000422:
                        InPhieuTemDonMau(printTypeCode, fileName, ref result);
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

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.Mps000422_PHIEU_IN_TEM_DON_MAU:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__PHIEU_IN_TEM_DON_MAU_Mps000422, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        internal enum PrintType
        {
            Mps000422_PHIEU_IN_TEM_DON_MAU
        }

        private void repositoryItemButtonEdit_Chot_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this._CurrentExpMest = (V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                if (this._CurrentExpMest != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ConfirmPresBlood").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ConfirmPresBlood");
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_CurrentExpMest);
                        listArgs.Add(this.currentModule);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataAfterSave);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        WaitingManager.Hide();
                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataAfterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_HuyChot_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this._CurrentExpMest = (V_HIS_EXP_MEST_2)gridViewExpMestList.GetFocusedRow();
                if (_CurrentExpMest != null)
                {
                    bool success = false;
                    HisExpMestSDO sdo = new HisExpMestSDO();
                    sdo.ExpMestId = this._CurrentExpMest.ID;
                    sdo.ReqRoomId = this.roomId;
                    CommonParam param = new CommonParam();
                    if (MessageBox.Show("Bạn có muốn hủy chốt đơn máu ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var apiresul = new BackendAdapter(param).Post<HIS_EXP_MEST>("/api/HisExpMest/PresBloodUnconfirm", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiresul != null)
                        {
                            success = true;
                            FillDataAfterSave(apiresul);
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                    {
                        FillDataExpMestList();
                    }
                    else
                    {

                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPreviewIntructionDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtIntructionDate.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtIntructionDate.EditValue = currentdate.AddDays(-1);
                    else
                        dtIntructionDate.EditValue = currentdate.AddMonths(-1);

                    btnSearch_Click(null, null);
                }
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
                if (dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtIntructionDate.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtIntructionDate.EditValue = currentdate.AddDays(1);
                    else
                        dtIntructionDate.EditValue = currentdate.AddMonths(1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
