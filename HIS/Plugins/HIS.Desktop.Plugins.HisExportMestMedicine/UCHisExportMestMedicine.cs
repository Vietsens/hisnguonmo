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
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.IO;
using Inventec.Common.RichEditor.Base;
using System.Threading;
using FlexCel.Report;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.HisExportMestMedicine.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using DevExpress.XtraBars;
using HIS.Desktop.ADO;
using AutoMapper;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.HisExportMestMedicine.popup;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using Inventec.Common.DocumentViewer;

namespace HIS.Desktop.Plugins.HisExportMestMedicine
{
    public partial class UCHisExportMestMedicine : UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        private string LoggingName = "";
        List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2> listExpMest;
        bool IsType = true;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<HIS_EXP_MEST_TYPE> expMestTypes;
        List<V_HIS_MEDI_STOCK> medistocks;
        List<V_HIS_ROOM> rooms;
        V_HIS_EXP_MEST rightClickData;
        //ACS.SDO.AcsAuthorizeSDO acsAuthorizeSDO;

        List<HIS_EXP_MEST_STT> _StatusSelecteds;
        List<HIS_EXP_MEST_TYPE> _TypeSelecteds;
        List<HIS_PATIENT_TYPE> _PatientTypeSelecteds;

        List<HIS_IMP_MEST> listImpMest;

        ToolTip toolTip = new ToolTip();
        V_HIS_ROOM room;

        Inventec.Desktop.Common.Modules.Module currentModule;

        List<V_HIS_MEST_ROOM> _RoomSelecteds;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.HisExportMestMedicine";
        V_HIS_EXP_MEST_2 rowFocusGrid;
        #endregion

        #region Construct
        public UCHisExportMestMedicine()
        {
            InitializeComponent();
            try
            {
                gridControl.ToolTipController = this.toolTipController;
                //FillDataToNavBarStatus();
                //FillDataToNavBarTypes();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisExportMestMedicine(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                gridControl.ToolTipController = this.toolTipController;
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void VisibleFilterBloodCode()
        {
            try
            {
                var check = medistocks.FirstOrDefault(o => o.ROOM_ID == roomId);
                if (check != null && check.IS_BLOOD == 1)
                {
                    lciBloodCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                }
                else
                    lciBloodCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisExportMestMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisConfigCFG.LoadConfig();

                medistocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList();
                rooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).ToList();
                expMestTypes = BackendDataWorker.Get<HIS_EXP_MEST_TYPE>();

                medistock = medistocks.FirstOrDefault(o => o.ROOM_ID == this.roomId && o.ROOM_TYPE_ID == this.roomTypeId);
                room = rooms.FirstOrDefault(o => o.ID == this.roomId);

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                //Gan ngon ngu
                LoadKeysFromlanguage();

                InitControlState();

                //Load Combo
                InitCheck(cboStatus, SelectionGrid__Status);
                InitCombo(cboStatus, BackendDataWorker.Get<HIS_EXP_MEST_STT>(), "EXP_MEST_STT_NAME", "ID");

                InitCheck(cboType, SelectionGrid__Type);

                InitCheck(cboPatientType, SelectionGrid__PatientType);
                InitCombo(cboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "PATIENT_TYPE_NAME", "ID");

                ResetComboPatientType(cboPatientType);

                if (expMestTypes != null && expMestTypes.Count > 0)
                {
                    if (this.medistock != null && this.medistock.IS_SHOW_DDT == 1)
                    {
                        InitCombo(cboType, expMestTypes.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL).ToList(), "EXP_MEST_TYPE_NAME", "ID");
                    }
                    else
                    {
                        InitCombo(cboType, expMestTypes.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL && o.ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).ToList(), "EXP_MEST_TYPE_NAME", "ID");
                    }
                }

                LciRequestRoomId.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (medistock.IS_BUSINESS != 1)
                {
                    LciRequestRoomId.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    InitCheck(CboRequestRoomIds, SelectionGrid__RequestRoom);
                    var reqRoom = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == medistock.ID).ToList();

                    //xếp theo thứ tự phòng khám, buồng bệnh,...
                    List<V_HIS_MEST_ROOM> mestRoom = new List<V_HIS_MEST_ROOM>();
                    if (reqRoom != null && reqRoom.Count > 0)
                    {
                        reqRoom = reqRoom.OrderBy(o => o.ROOM_TYPE_ID).ThenBy(o => o.ROOM_NAME).ToList();
                        var executeRoom = reqRoom.Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                        if (executeRoom != null && executeRoom.Count > 0)
                        {
                            mestRoom.AddRange(executeRoom);
                        }

                        var bbRoom = reqRoom.Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG).ToList();
                        if (bbRoom != null && bbRoom.Count > 0)
                        {
                            mestRoom.AddRange(bbRoom);
                        }

                        var otherRoom = reqRoom.Where(o => o.ROOM_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG && o.ROOM_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                        if (otherRoom != null && otherRoom.Count > 0)
                        {
                            mestRoom.AddRange(otherRoom);
                        }
                    }

                    InitCombo(CboRequestRoomIds, mestRoom, "ROOM_NAME", "ROOM_ID");
                }

                ResetComboPatientType(CboRequestRoomIds);

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                RefreshData();

                //focus truong du lieu dau tien
                txtExpMestCode.Focus();

                VisibleFilterBloodCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

        private void GetImpMest(List<long> chmsId)
        {
            try
            {
                listImpMest = new List<HIS_IMP_MEST>();
                CommonParam param = new CommonParam();
                HisImpMestFilter filter = new HisImpMestFilter();
                filter.CHMS_EXP_MEST_IDs = chmsId;
                listImpMest = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void RefreshData()
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExportMestMedicine.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExportMestMedicine.UCHisExportMestMedicine).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxChiTiet.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.groupBoxChiTiet.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnListDepa.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.btnListDepa.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportCodeList.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.btnExportCodeList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkInStock.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.chkInStock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtExpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.txtExpMestCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtBloodCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.txtBloodCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciExpTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciExpTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExpMestSubCode.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumnExpMestSubCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestTypeName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcExpMestTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcExpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcFinishTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcFinishTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcReqDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcReqDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcNationalExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcNationalExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ButtonAssignTestDisable.Buttons[0].ToolTip = this.ButtonAssignTest.Buttons[0].ToolTip;

                this.lciBhytNumber.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciBhytNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientType.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciPatientType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_PatientType.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn_PatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_PatientType.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn_PatientType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem36.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.layoutControlItem36.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.chkInHDSD.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.chkInHDSD.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciTotalOfPatient.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciTotalOfPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalOfMedicine.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciTotalOfMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalOfPrice.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciTotalOfPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAveragePerMedicine.Text = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciAveragePerMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAveragePerMedicine.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExportMestMedicine.lciAveragePerMedicine.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

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

        private void ResetComboPatientType(GridLookUpEdit cbo)
        {
            try
            {
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

        private void SetDefaultValueControl()
        {
            try
            {
                radioGroupStatus.SelectedIndex = 1;
                radioGroupStatus.Enabled = false;
                radioGroupExportBill.SelectedIndex = 0;
                radioGroupExportBill.Enabled = true;
                cboStatus.Focus();
                cboType.Focus();
                cboPatientType.Focus();
                CboRequestRoomIds.Focus();

                txtExpMestCode.Text = "";
                txtBloodCode.Text = "";
                txtSearchTreatmentCode.Text = "";
                txtServiceReqCode.Text = "";
                txtPresNumber.Text = "";
                txtSearchPatientCode.Text = "";
                txtKeyWord.Text = "";
                var now = DateTime.Today;
                // var nowfrom = 
                // dtFromTime.DateTime.ToString("yyyyMMdd") + "000000"
                dtCreateTimeFrom.EditValue = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                dtCreateTimeTo.EditValue = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                dtExpTimeFrom.EditValue = null;
                dtExpTimeTo.EditValue = null;
                //SetDefaultValueStatus();
                //SetDefaultValueType();
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();

                if (this.medistock != null)
                {
                    chkInStock.Enabled = true;
                    chkInStock.Checked = true;
                }
                else
                {
                    chkInStock.Enabled = false;
                    chkInStock.Checked = false;
                }

                long showbtn = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(Base.GlobalStore.showButton));

                if (showbtn == 1)
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciBtnListDepa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciBtnListDepa.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                popupControlExpMestDetail.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pagingSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControl);
                gridControl.RefreshDataSource();

                RefreshDisplaySummaryLabel();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void RefreshDisplaySummaryLabel()
        {
            try
            {
                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(FillDataToSummaryLabel));
                thread.Priority = ThreadPriority.Normal;
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
        private void FillDataToSummaryLabel()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestView2Filter filter = new MOS.Filter.HisExpMestView2Filter();
                SetFilter(ref filter);
                filter.HAS_CHMS_TYPE_ID = false;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<HisExpMestSummarySDO>("api/HisExpMest/GetSummary", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        Invoke(new Action(() =>
                        {
                            lblTotalOfMedicine.Text = data.TotalOfMedicine.ToString();
                            lblTotalOfPatient.Text = data.TotalOfPatient.ToString();
                            string totalOfPrice = string.Format("{0:0,000}", Convert.ToDecimal(data.TotalOfPrice));
                            totalOfPrice = totalOfPrice.Replace(",", ".");
                            lblTotalOfPrice.Text = totalOfPrice;
                            string averagePerMedicine = string.Format("{0:0,000}", Convert.ToDecimal(data.TotalOfPrice / data.TotalOfMedicine));
                            averagePerMedicine = averagePerMedicine.Replace(",", ".");
                            lblAveragePerMedicine.Text = averagePerMedicine;
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            lblTotalOfMedicine.Text = "0";
                            lblTotalOfPatient.Text = "0";
                            lblTotalOfPrice.Text = "0.000";
                            lblAveragePerMedicine.Text = "0.000";
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>> apiResult = null;
                MOS.Filter.HisExpMestView2Filter filter = new MOS.Filter.HisExpMestView2Filter();
                SetFilter(ref filter);
                filter.HAS_CHMS_TYPE_ID = false;
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>>("api/HisExpMest/GetView2", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    listExpMest = apiResult.Data;
                    if (listExpMest != null && listExpMest.Count > 0)
                    {
                        GetImpMest(listExpMest.Select(o => o.ID).ToList());
                        gridControl.DataSource = listExpMest;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        LoadInfoClick(listExpMest[0]);
                        LoadVisibility(listExpMest[0]);
                        if (chkAutoCallPatient.Checked && (filter.EXP_MEST_CODE__EXACT != null || filter.TDL_SERVICE_REQ_CODE__EXACT != null || filter.TDL_TREATMENT_CODE__EXACT != null))
                        {
                            var listCall = listExpMest.Where(o => o.IS_NOT_TAKEN != 1 && o.TDL_PATIENT_NAME != null);
                            if (listCall != null && listCall.ToList().Count > 0)
                                LoadCallPatientByThread(listCall.ToList()[0]);
                        }
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisExpMestView2Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!String.IsNullOrEmpty(txtBloodCode.Text))
                {
                    string code = txtBloodCode.Text.Trim();

                    filter.TDL_BLOOD_CODE__EXACT = code;
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.HAS_AGGR = false;
                }
                else if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.HAS_AGGR = false;
                }

                else if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    filter.TDL_SERVICE_REQ_CODE__EXACT = code;
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.EXP_MEST_TYPE_IDs = new List<long>();
                    filter.EXP_MEST_TYPE_IDs.AddRange(new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK
                });
                    filter.HAS_AGGR = false;
                }
                else
                {
                    filter.HAS_AGGR = false;
                    if (!String.IsNullOrEmpty(txtSearchTreatmentCode.Text))
                    {
                        string code = txtSearchTreatmentCode.Text.Trim();
                        if (code.Length < 12 && checkDigit(code))
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtSearchTreatmentCode.Text = code;
                        }
                        filter.TDL_TREATMENT_CODE__EXACT = code;
                    }

                    else if (!String.IsNullOrEmpty(txtSearchPatientCode.Text))
                    {
                        string code = txtSearchPatientCode.Text.Trim();
                        if (code.Length < 10 && checkDigit(code))
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                            txtSearchPatientCode.Text = code;
                        }
                        filter.TDL_PATIENT_CODE__EXACT = code;
                    }

                    if (!String.IsNullOrEmpty(txtHeinCard.Text))
                    {
                        string code = txtHeinCard.Text.Trim();
                        if (code.Length > 0)
                        {
                            code = code.ToUpper();
                            txtHeinCard.Text = code;
                            if (code.Length == 2 && checkLetter(code))
                            {
                                filter.VIR_HEIN_CARD_PREFIX__EXACT = code;
                            }
                            else if (code.Length == 15)
                            {
                                filter.TDL_HEIN_CARD_NUMBER__EXACT = code;
                            }
                            else
                            {
                                filter.TDL_HEIN_CARD_NUMBER = code;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(txtPresNumber.Text))
                    {
                        filter.PRES_NUMBER = long.Parse(txtPresNumber.Text);
                    }

                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    if (medistock != null && medistock.IS_SHOW_DDT == 1)
                    {
                        if (_TypeSelecteds != null && _TypeSelecteds.Count > 0)
                        {
                            filter.EXP_MEST_TYPE_IDs = _TypeSelecteds.Select(o => o.ID).ToList();
                        }
                    }
                    else
                    {
                        if (_TypeSelecteds != null && _TypeSelecteds.Count > 0)
                        {
                            filter.EXP_MEST_TYPE_IDs = _TypeSelecteds.Where(p => p.ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT).Select(o => o.ID).ToList();
                        }
                    }

                    if (_TypeSelecteds != null && _TypeSelecteds.Count == 1 && _TypeSelecteds.FirstOrDefault().ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK && radioGroupStatus.Enabled)
                    {
                        if (radioGroupStatus.SelectedIndex == 0)
                            filter.TREATMENT_IS_ACTIVE = null;
                        else if (radioGroupStatus.SelectedIndex == 1)
                            filter.TREATMENT_IS_ACTIVE = false;
                        else if (radioGroupStatus.SelectedIndex == 2)
                            filter.TREATMENT_IS_ACTIVE = true;
                    }


                    if (radioGroupExportBill.SelectedIndex == 0)
                        filter.HAS_BILL_ID = null;
                    else if (radioGroupExportBill.SelectedIndex == 1)
                        filter.HAS_BILL_ID = true;
                    else if (radioGroupExportBill.SelectedIndex == 2)
                        filter.HAS_BILL_ID = false;


                    if (chkInStock.Checked && this.medistock != null)
                    {
                        filter.IMP_OR_EXP_MEDI_STOCK_ID = this.medistock.ID;
                    }
                    else if (!chkInStock.Checked && this.medistock != null)
                    {
                        filter.DATA_DOMAIN_FILTER = true;
                        filter.WORKING_ROOM_ID = roomId;
                    }
                    else if (this.medistock == null && this.room != null)
                    {
                        filter.REQ_DEPARTMENT_ID = this.room.DEPARTMENT_ID;
                    }

                    //    if (filter.EXP_MEST_TYPE_IDs == null || filter.EXP_MEST_TYPE_IDs.Count == 0)
                    //    {
                    //        filter.EXP_MEST_TYPE_IDs = new List<long>();
                    //        filter.EXP_MEST_TYPE_IDs.AddRange(new List<long>
                    //{
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC,
                    //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC
                    //});
                    //    }

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        //filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        //    Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue) filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMddHHmmss"));

                    if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue) filter.FINISH_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));

                    if (dteIntructionTimeFrom.EditValue != null && dteIntructionTimeFrom.DateTime != DateTime.MinValue) 
                        filter.TDL_INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteIntructionTimeFrom.EditValue).ToString("yyyyMMddHHmm00"));

                    if (dteIntructionTimeTo.EditValue != null && dteIntructionTimeTo.DateTime != DateTime.MinValue) 
                        filter.TDL_INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteIntructionTimeTo.EditValue).ToString("yyyyMMddHHmm59"));

                    if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue) filter.FINISH_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtExpTimeTo
.EditValue).ToString("yyyyMMddHHmmss"));

                    SetFilterStatus(ref filter);
                    SetFilterPatientType(ref filter);

                    if (medistock.IS_BUSINESS != 1 && _RoomSelecteds != null && _RoomSelecteds.Count > 0)
                    {
                        filter.REQ_ROOM_IDs = _RoomSelecteds.Select(o => o.ROOM_ID).ToList();
                    }
                    //filter.TDL_BLOOD_CODE__EXACT = txtBloodCode.Text;
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

        private bool checkLetter(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsLetter(s[i]) == true)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilterStatus(ref MOS.Filter.HisExpMestView2Filter filter)
        {
            try
            {
                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    filter.EXP_MEST_STT_IDs = _StatusSelecteds.Select(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void SetFilterType(ref MOS.Filter.HisExpMestViewFilter filter)
        {
            try
            {

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetCombo(cboStatus);
                ResetCombo(cboPatientType);
                ResetCombo(cboType);
                ResetComboPatientType(CboRequestRoomIds);
                ResetComboPatientType(cboPatientType);
                SetDefaultValueControl();
                RefreshData();
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
                if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        RefreshData();
                        txtExpMestCode.SelectAll();
                    }
                    else
                    {
                        txtBloodCode.Focus();
                        txtBloodCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtBloodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtBloodCode.Text))
                    {
                        RefreshData();
                        txtBloodCode.SelectAll();
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

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
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
                        else if (e.Column.FieldName == "EXP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT) //tam thoi
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST) //yeu cau
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            //else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT) // tu choi duyet
                            //{
                            //    e.Value = imageListStatus.Images[2];
                            //}
                            else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE) // duyet
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE) // da xuat
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                            else if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL) //Tong hop
                            {
                                e.Value = imageListStatus.Images[5];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "FINISH_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.TDL_INTRUCTION_TIME ?? 0);
                        }
                        // else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        //{
                        //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        // }
                        //else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        // {
                        //string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                        //string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                        //e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        //}
                        //else if (e.Column.FieldName == "EXP_LOGINNAME_DISPLAY")
                        //{
                        //string IMP_LOGINNAME = data.EXP_LOGINNAME;
                        //string IMP_USERNAME = data.EXP_USERNAME;
                        //e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        // }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                        if (e.Column.FieldName == "EXP_MEST_REASON_DISPLAY")
                        {
                            //if (data.EXP_MEST_SUB_CODE_2 != null) e.Value = string.Format("{0}-{1}", data.EXP_MEST_REASON_CODE, data.EXP_MEST_REASON_NAME);
                            if (data.EXP_MEST_SUB_CODE_2 != null) e.Value = data.EXP_MEST_REASON_NAME;
                        }
                        // else if (e.Column.FieldName == "EXP_MEST_TYPE_NAME_DISPLAY")
                        //{
                        //Review
                        ////if (data.EXP_MEST_TYPE_ID == Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__PRES)
                        ////{
                        //if (data.BLOOD_TYPE_COUNT > 0)
                        //{
                        //    e.Value = data.EXP_MEST_TYPE_NAME + Inventec.Common.Resource.Get.Value(
                        //        "IVT_LANGUAGE_KEY__UC_HIS_EXP_MEST_LIST__GRID_CELL_EXP_MEST_TYPE",
                        //        Resources.ResourceLanguageManager.LanguageUCHisExportMestMedicine,
                        //        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        //}
                        //else
                        //    e.Value = data.EXP_MEST_TYPE_NAME;
                        ////}
                        ////else
                        ////    e.Value = data.EXP_MEST_TYPE_NAME;
                        //}
                        if (e.Column.FieldName == "EXP_MEST_SUB_CODE_2")
                        {
                            if (data.EXP_MEST_SUB_CODE_2 != null)
                            {
                                e.Value = data.EXP_MEST_SUB_CODE_2;

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

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_EXP_MEST_2 pData = (V_HIS_EXP_MEST_2)gridView.GetRow(e.RowHandle);
                    long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    long impMediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "IMP_MEDI_STOCK_ID") ?? "").ToString());
                    long expMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    long currentDepartment = WorkPlace.GetDepartmentId();
                    if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT ||
                                statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                                )
                            {
                                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN && (pData.BILL_ID.HasValue || pData.DEBT_ID.HasValue))
                                {
                                    e.RepositoryItem = ButtonDisableEdit;
                                }
                                else
                                    e.RepositoryItem = ButtonEnableEdit;
                            }
                            else
                                e.RepositoryItem = ButtonDisableEdit;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableEdit;
                        }
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
                    {
                        if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                            {
                                if ((HisConfigCFG.EnableButtonDelete != "2" && (creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName) || (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnDelete) != null))) || (HisConfigCFG.EnableButtonDelete == "2" && (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnDelete) != null)))
                                {
                                    e.RepositoryItem = ButtonEnableDiscard;
                                }
                                else
                                    e.RepositoryItem = ButtonDisableDiscard;
                            }
                            else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT) && medistock == null)
                                    e.RepositoryItem = ButtonEnableDiscard;
                                else
                                    e.RepositoryItem = ButtonDisableDiscard;
                            }
                            else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                            {
                                e.RepositoryItem = ButtonDisableDiscard;
                            }
                            else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                if (this.medistock == null && currentDepartment == pData.REQ_DEPARTMENT_ID)
                                {
                                    e.RepositoryItem = ButtonEnableDiscard;
                                }
                                else
                                {
                                    e.RepositoryItem = ButtonDisableDiscard;
                                }
                            }
                            else if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))                                  
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                                    )
                            {
                                e.RepositoryItem = ButtonEnableDiscard;
                            }
                            else
                                e.RepositoryItem = ButtonDisableDiscard;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableDiscard;
                        }
                    }
                    else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (pData.IS_NOT_TAKEN == 1)
                            {
                                e.RepositoryItem = ButtonDisableApproval;
                            }
                            else
                            {
                                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)
                                {
                                    if (medistock != null && medistock.ID == mediStockId &&
                                        (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                                        && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                                         )
                                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                                    {
                                        e.RepositoryItem = ButtonEnableApproval;
                                    }
                                    else if (medistock != null && medistock.ID == mediStockId &&
                                        (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                        && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                                        e.RepositoryItem = ButtonEnableApproval;
                                    else
                                        e.RepositoryItem = ButtonDisableApproval;
                                }
                                else
                                    e.RepositoryItem = ButtonDisableApproval;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableApproval;
                        }
                        if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                        {
                            if (HisConfigCFG.MUST_CONFIRM_BEFORE_APPROVE == "1" && pData.IS_CONFIRM != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.RepositoryItem = ButtonDisableApproval;
                            }
                        }
                    }
                    //else if (e.Column.FieldName == "DIS_APPROVAL")// Từ chối duyệt duyệt
                    //{
                    //    if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                    //    {
                    //        if (pData.IS_NOT_TAKEN == 1)
                    //        {
                    //            e.RepositoryItem = ButtonDisableDisApproval;
                    //        }
                    //        else
                    //        {
                    //            if (medistock != null && medistock.ID == mediStockId &&
                    //                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                    //                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                    //                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT
                    //                )
                    //            {
                    //                if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                    //                {
                    //                    e.RepositoryItem = Btn_HuyTuChoiDuyet_Enable;
                    //                }
                    //                else if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                    //                {
                    //                    e.RepositoryItem = ButtonEnableDisApproval;
                    //                }
                    //                else
                    //                {
                    //                    e.RepositoryItem = ButtonDisableDisApproval;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                e.RepositoryItem = ButtonDisableDisApproval;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        e.RepositoryItem = ButtonDisableDisApproval;
                    //    }
                    //}
                    else if (e.Column.FieldName == "EXPORT_DISPLAY")// thực xuất
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (pData.IS_NOT_TAKEN == 1)
                            {
                                e.RepositoryItem = ButtonDisableActualExport;
                            }
                            else
                            {
                                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN
                                    && HisConfigCFG.EXPORT_SALE__MUST_BILL && !pData.BILL_ID.HasValue && !pData.DEBT_ID.HasValue
                                    && statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                                {
                                    e.RepositoryItem = ButtonDisableActualExport;
                                }
                                else if (medistock != null && medistock.ID == mediStockId &&
                                    statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                                {
                                    if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnExport) != null)
                                    {
                                        e.RepositoryItem = ButtonEnableActualExport;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = ButtonDisableActualExport;
                                    }
                                }
                                else if (
                                    statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                    && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                                {
                                    if ((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnCancelExport) != null) || (HisConfigCFG.APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION == "1" && pData.LAST_EXP_LOGINNAME == LoggingName))
                                    {
                                        if (medistock != null && medistock.ID == mediStockId)
                                            e.RepositoryItem = Btn_HuyThucXuat_Enable;
                                        else
                                            e.RepositoryItem = Btn_HuyThucXuat_Disable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_HuyThucXuat_Disable;
                                    }
                                }
                                else
                                    e.RepositoryItem = ButtonDisableActualExport;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableActualExport;
                        }

                    }
                    else if (e.Column.FieldName == "MOBA_IMP_MEST_CREATE")// Tạo yêu cầu nhập thu hồi
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE && (
                                expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP || expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM || expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                            {
                                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                                    ButtonEnableMobaImpCreate.Buttons[0].ToolTip = "Tạo nhập hao phí trả lại";
                                else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                                    ButtonEnableMobaImpCreate.Buttons[0].ToolTip = "Tạo nhập thu hồi máu";
                                e.RepositoryItem = ButtonEnableMobaImpCreate;
                                //if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DNT)
                                //{
                                //    //decimal BloodTypeCount = (decimal)(gridView.GetRowCellValue(e.RowHandle, "BLOOD_TYPE_COUNT") ?? 0);
                                //    //if (BloodTypeCount == 0)
                                //    //{
                                //    e.RepositoryItem = ButtonEnableMobaImpCreate;
                                //    //}
                                //    //else
                                //    //    e.RepositoryItem = ButtonDisableMobaImpCreate;
                                //}
                                //else
                                //    e.RepositoryItem = ButtonDisableMobaImpCreate;
                            }
                            //else if (
                            //    expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            //{
                            //    ButtonEnableMobaImpCreate.Buttons[0].ToolTip = "Tạo phiếu nhập thu hồi";
                            //    e.RepositoryItem = ButtonEnableMobaImpCreate;
                            //}
                            else
                                e.RepositoryItem = ButtonDisableMobaImpCreate;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableMobaImpCreate;
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (pData.IS_NOT_TAKEN == 1 || !((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnHuyDuyet) != null) || (HisConfigCFG.APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION == "1" && pData.LAST_APPROVAL_LOGINNAME == LoggingName)))
                            {
                                e.RepositoryItem = ButtonRequestDisable;
                            }
                            else
                            {
                                if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                                {
                                    if (medistock != null && medistock.ID == mediStockId && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                                    {
                                        e.RepositoryItem = ButtonRequest;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = ButtonRequestDisable;
                                    }
                                }
                                else
                                {
                                    e.RepositoryItem = ButtonRequestDisable;
                                }
                            }
                        }
                        else
                        {
                            e.RepositoryItem = ButtonRequestDisable;
                        }

                    }
                    else if (e.Column.FieldName == "ExportEqualApprove")// Trạng thái duyệt
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                            {
                                if (pData.IS_EXPORT_EQUAL_APPROVE == 1)
                                {
                                    e.RepositoryItem = NotApprove;
                                }
                                else
                                    e.RepositoryItem = Approve;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = NotApprove;
                        }
                    }
                    else if (e.Column.FieldName == "ASSIGN_TEST")//chỉ định xét nghiệm
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (
                                expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM && (
                                statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                            {
                                //decimal BloodTypeCount = (decimal)(gridView.GetRowCellValue(e.RowHandle, "BLOOD_TYPE_COUNT") ?? 0);
                                //if (BloodTypeCount == 0)
                                //{
                                //    e.RepositoryItem = ButtonAssignTestDisable;
                                //}
                                //else
                                e.RepositoryItem = ButtonAssignTest;
                            }
                            else
                                e.RepositoryItem = ButtonAssignTestDisable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonAssignTestDisable;
                        }
                    }

                    else if (e.Column.FieldName == "MobaDepaCreate")//Tạo hoa phí trả lại
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL
                                && expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                            {
                                e.RepositoryItem = Btn_MobaDepaCreate_Enable;
                            }
                            else
                                e.RepositoryItem = Btn_MobaDepaCreate_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = Btn_MobaDepaCreate_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "DONE")//Tạo hoa phí trả lại
                    {
                        if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                            && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            && pData.IS_EXPORT_EQUAL_APPROVE == 1
                            && medistock != null
                            && pData.MEDI_STOCK_ID == medistock.ID)
                        {

                            e.RepositoryItem = Btn_HoanThanh_Enable;
                        }
                        else
                            e.RepositoryItem = Btn_HoanThanh_Disable;
                    }
                    else if (e.Column.FieldName == "CHECK_TRANBLOOD")
                    {
                        if ((new List<long>() { IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE }).Contains(pData.EXP_MEST_STT_ID) && pData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                        {
                            e.RepositoryItem = Btn_Check_TruyenMau_Enable;
                        }
                        else
                            e.RepositoryItem = Btn_Check_TruyenMau_Disable;
                    }
                    else if (e.Column.FieldName == "ImpMestCreate")//Tạo phiếu nhập chuyển kho
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                        {
                            //SetToolTipButton(Btn_ImpMestCreate_Enable, expMestTypeId);
                            //SetToolTipButton(Btn_ImpMestCreate_Disable, expMestTypeId);

                            if (medistock != null && medistock.ID == impMediStockId && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE) && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS))
                            {
                                SetToolTipButton(Btn_ImpMestCreate_Enable, expMestTypeId);
                                e.RepositoryItem = Btn_ImpMestCreate_Enable;

                            }
                            else if (medistock != null && medistock.ID == impMediStockId && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE) && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL))
                            {
                                SetToolTipButton(Btn_ImpMestCreate_Enable, expMestTypeId);
                                e.RepositoryItem = Btn_ImpMestCreate_Enable;

                            }
                            else if (medistock != null && medistock.ID == impMediStockId && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE && (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK))
                            {
                                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                                {
                                    if (listImpMest != null && listImpMest.Count > 0)
                                    {
                                        var impMests = listImpMest.Where(o => o.CHMS_EXP_MEST_ID == pData.ID).ToList();
                                        if (impMests != null && impMests.Count > 0)
                                        {
                                            SetToolTipButton(Btn_ImpMestCreate_Disable, expMestTypeId);
                                            e.RepositoryItem = Btn_ImpMestCreate_Disable;
                                        }
                                        else
                                        {
                                            SetToolTipButton(Btn_ImpMestCreate_Enable, expMestTypeId);
                                            e.RepositoryItem = Btn_ImpMestCreate_Enable;
                                        }
                                    }
                                    else
                                    {
                                        SetToolTipButton(Btn_ImpMestCreate_Enable, expMestTypeId);
                                        e.RepositoryItem = Btn_ImpMestCreate_Enable;
                                    }
                                }
                            }
                            else
                            {
                                SetToolTipButton(Btn_ImpMestCreate_Disable, expMestTypeId);
                                e.RepositoryItem = Btn_ImpMestCreate_Disable;
                            }
                        }
                        else
                        {
                            SetToolTipButton(Btn_ImpMestCreate_Disable, expMestTypeId);
                            e.RepositoryItem = Btn_ImpMestCreate_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "CallPatient")//Gọi bệnh nhân
                    {
                        if (pData.IS_NOT_TAKEN != 1 && pData.TDL_PATIENT_NAME != null)
                        {
                            e.RepositoryItem = btnCallPatient;
                        }
                        else
                        {
                            e.RepositoryItem = btnCallPatientDisable;
                        }
                    }
                    else if (e.Column.FieldName == "BILL")
                    {
                        if (expMestTypeId != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                        {
                            if (statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                            {
                                e.RepositoryItem = Btn_Bill_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = Btn_CancelBill_Disable;
                            }
                        }
                        else
                        {
                            if (HisConfigCFG.EXPORT_SALE__MUST_BILL)
                            {
                                if (pData.DEBT_ID.HasValue && !pData.BILL_ID.HasValue)
                                {
                                    e.RepositoryItem = Btn_Bill_Disable;
                                }
                                else if (pData.BILL_ID.HasValue)
                                {
                                    if (statusIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                                        && (pData.CASHIER_LOGINNAME == LoggingName
                                        ||
                                        (HisConfigCFG.CANCEL_ALLOW_OTHER_LOGINNAME
                                        && HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(LoggingName))))
                                    {
                                        e.RepositoryItem = Btn_CancelBill_Enable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_CancelBill_Disable;
                                    }
                                }
                                else
                                {
                                    e.RepositoryItem = Btn_Bill_Enable;
                                }
                            }
                            else
                            {
                                if (pData.DEBT_ID.HasValue && !pData.BILL_ID.HasValue)
                                {
                                    e.RepositoryItem = Btn_Bill_Disable;
                                }
                                else if (pData.BILL_ID.HasValue
                                    && (pData.CASHIER_LOGINNAME == LoggingName
                                    ||
                                    (HisConfigCFG.CANCEL_ALLOW_OTHER_LOGINNAME
                                    && HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(LoggingName))))
                                {
                                    e.RepositoryItem = Btn_CancelBill_Enable;
                                }
                                else if (pData.BILL_ID.HasValue)
                                {
                                    e.RepositoryItem = Btn_CancelBill_Disable;
                                }
                                else
                                {
                                    e.RepositoryItem = Btn_Bill_Enable;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetToolTipButton(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn, long expMestTypeId)
        {
            try
            {
                //if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HCS)
                //    btn.Buttons[0].ToolTip = "Tạo phiếu nhập hoàn cơ số";
                if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                    btn.Buttons[0].ToolTip = "Tạo phiếu nhập bù cơ số";
                else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                    btn.Buttons[0].ToolTip = "Tạo phiếu nhập chuyển kho";
                else if (expMestTypeId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                    btn.Buttons[0].ToolTip = "Tạo phiếu nhập bù lẻ";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "EXP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else
                            {
                                var isNotTaken = (short?)view.GetRowCellValue(lastRowHandle, "IS_NOT_TAKEN");
                                if (isNotTaken == 1)
                                {
                                    text = "Bệnh nhân không lấy";
                                }
                                var stt = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_ID") ?? "").ToString();
                                if (stt != "5")
                                {
                                    var bloodLevel = (view.GetRowCellValue(lastRowHandle, "BLOOD_LEVEL") ?? "").ToString();
                                    if (bloodLevel == "1")
                                    {
                                        text = "Mức độ tối cấp cấp máu trong 5 phút";
                                    }
                                    else if (bloodLevel == "2")
                                    {
                                        text = "Mức độ cấp cứu cấp máu trong 1 giờ";
                                    }
                                    else if (bloodLevel == "3")
                                    {
                                        text = "Mức độ thường quy";
                                    }
                                }
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

        private void LoadInfoClick(V_HIS_EXP_MEST_2 data)
        {
            try
            {
                lblBloodCode.Text = data.TDL_PATIENT_BLOOD_ABO_CODE;
                lblDepartmentReq.Text = data.REQ_DEPARTMENT_NAME;
                lblDiaChi.Text = data.TDL_PATIENT_ADDRESS;
                lblDoiTuong.Text = data.PATIENT_TYPE_NAME;
                lblBhytNumber.Text = HIS.Desktop.Utility.HeinCardHelper.SetHeinCardNumberDisplayByNumber(data.TDL_HEIN_CARD_NUMBER);
                var supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == data.SUPPLIER_ID);
                lblNhaCungCap.Text = supplier != null ? supplier.SUPPLIER_NAME : "";

                lblGiaGiam.Text = data.DISCOUNT != null ? data.DISCOUNT.ToString() : "";
                lblGioiTinh.Text = data.TDL_PATIENT_GENDER_NAME;

                var medistock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == data.IMP_MEDI_STOCK_ID);
                lblKho.Text = medistock != null ? medistock.MEDI_STOCK_NAME : "";

                var reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().FirstOrDefault(o => o.ID == data.EXP_MEST_REASON_ID);
                lblLyDoXuat.Text = reason != null ? reason.EXP_MEST_REASON_NAME : "";

                lblMaDieuTri.Text = data.TDL_TREATMENT_CODE;
                lblMaYeuCau.Text = data.TDL_SERVICE_REQ_CODE;
                lblPatientCode.Text = data.TDL_PATIENT_CODE;
                lblNgaySinh.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                lblTenBenhNhan.Text = data.TDL_PATIENT_NAME;
                lblExpTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.LAST_EXP_TIME ?? 0);
                lblExpUserName.Text = data.LAST_EXP_LOGINNAME + (!string.IsNullOrEmpty(data.LAST_EXP_USERNAME) ? ("- " + data.LAST_EXP_USERNAME) : "");
                lblPrescriptionReqUserName.Text = data.TDL_PRESCRIPTION_REQ_LOGINNAME + (!string.IsNullOrEmpty(data.TDL_PRESCRIPTION_REQ_USERNAME) ? ("- " + data.TDL_PRESCRIPTION_REQ_USERNAME) : "");
                lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(data.TDL_TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                {
                    lblCreateTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.CREATE_TIME ?? 0);
                    lblReqUserName.Text = data.REQ_LOGINNAME + (!string.IsNullOrEmpty(data.REQ_USERNAME) ? ("- " + data.REQ_USERNAME) : "");
                }
                else
                {
                    lblCreateTime.Text = "";
                    lblReqUserName.Text = "";
                }

                LblReqRoomName.Text = data.REQ_ROOM_NAME;

                toolTip.RemoveAll();
                if (!string.IsNullOrEmpty(lblDoiTuong.Text))
                    toolTip.SetToolTip(lblDoiTuong, lblDoiTuong.Text);
                if (!string.IsNullOrEmpty(lblNhaCungCap.Text))
                    toolTip.SetToolTip(lblNhaCungCap, lblNhaCungCap.Text);
                if (!string.IsNullOrEmpty(lblGioiTinh.Text))
                    toolTip.SetToolTip(lblGioiTinh, lblGioiTinh.Text);
                if (!string.IsNullOrEmpty(lblKho.Text))
                    toolTip.SetToolTip(lblKho, lblKho.Text);
                if (!string.IsNullOrEmpty(lblLyDoXuat.Text))
                    toolTip.SetToolTip(lblLyDoXuat, lblLyDoXuat.Text);
                if (!string.IsNullOrEmpty(lblMaDieuTri.Text))
                    toolTip.SetToolTip(lblMaDieuTri, lblMaDieuTri.Text);
                if (!string.IsNullOrEmpty(lblPatientCode.Text))
                    toolTip.SetToolTip(lblPatientCode, lblPatientCode.Text);
                if (!string.IsNullOrEmpty(lblMaYeuCau.Text))
                    toolTip.SetToolTip(lblMaYeuCau, lblMaYeuCau.Text);
                if (!string.IsNullOrEmpty(lblNgaySinh.Text))
                    toolTip.SetToolTip(lblNgaySinh, lblNgaySinh.Text);
                if (!string.IsNullOrEmpty(lblTenBenhNhan.Text))
                    toolTip.SetToolTip(lblTenBenhNhan, lblTenBenhNhan.Text);
                if (!string.IsNullOrEmpty(lblCreateTime.Text))
                    toolTip.SetToolTip(lblCreateTime, lblCreateTime.Text);
                if (!string.IsNullOrEmpty(lblReqUserName.Text))
                    toolTip.SetToolTip(lblReqUserName, lblReqUserName.Text);
                if (!string.IsNullOrEmpty(lblExpTime.Text))
                    toolTip.SetToolTip(lblExpTime, lblExpTime.Text);
                if (!string.IsNullOrEmpty(lblExpUserName.Text))
                    toolTip.SetToolTip(lblExpUserName, lblExpUserName.Text);
                if (!string.IsNullOrEmpty(lblPrescriptionReqUserName.Text))
                    toolTip.SetToolTip(lblPrescriptionReqUserName, lblPrescriptionReqUserName.Text);
                if (!string.IsNullOrEmpty(lblTotalPrice.Text))
                    toolTip.SetToolTip(lblTotalPrice, lblTotalPrice.Text);
                if (!string.IsNullOrEmpty(LblReqRoomName.Text))
                    toolTip.SetToolTip(LblReqRoomName, LblReqRoomName.Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadVisibility(V_HIS_EXP_MEST_2 row)
        {
            try
            {
                if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT ||
                    row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM ||
                    row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK ||
                    row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                {
                    layoutControlItem29.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem32.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    layoutControlItem29.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem25.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem32.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }

                HideControlInControlProcess hideControlInControlProcess = new Utility.HideControlInControlProcess();
                hideControlInControlProcess.Run(this, "HIS.Desktop.Plugins.HisExportMestMedicine");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__RequestRoom(object sender, EventArgs e)
        {
            try
            {
                _RoomSelecteds = new List<V_HIS_MEST_ROOM>();
                foreach (V_HIS_MEST_ROOM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _RoomSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _StatusSelecteds = new List<HIS_EXP_MEST_STT>();
                foreach (HIS_EXP_MEST_STT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _StatusSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Type(object sender, EventArgs e)
        {
            try
            {
                _TypeSelecteds = new List<HIS_EXP_MEST_TYPE>();
                foreach (HIS_EXP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _TypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PatientType(object sender, EventArgs e)
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

        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
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
                if (btnRefresh.Enabled)
                {
                    btnRefresh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusExpCode()
        {
            try
            {
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Export()
        {
            try
            {
                if (btnExportCodeList.Enabled)
                {
                    btnExportCodeList.Focus();
                    btnExportCodeList_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region repot
        private void btnExportCodeList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportCodeList.Enabled) return;

                if (dtExpTimeFrom.EditValue == null || dtExpTimeTo.EditValue == null)
                {
                    MessageBox.Show(Resources.ResourceMessage.BanChuaChonThoiGianThucXuat);
                    if (dtExpTimeFrom.EditValue == null)
                    {
                        dtExpTimeFrom.Focus();
                        dtExpTimeFrom.SelectAll();
                    }
                    else if (dtExpTimeTo.EditValue == null)
                    {
                        dtExpTimeTo.Focus();
                        dtExpTimeTo.SelectAll();
                    }
                    return;
                }

                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CreateReport));
                thread.Priority = ThreadPriority.Normal;
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

        private void CreateReport()
        {
            try
            {
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachCacMaPhieuXuat.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayBieuMauIn, templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    //getdata
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
                singleTag.AddSingleKey(store, "TYPE", "THỰC XUẤT");
                singleTag.AddSingleKey(store, "MEDI_STOCK_NAME", medistock.MEDI_STOCK_NAME.ToUpper());
                singleTag.AddSingleKey(store, "EXP_TIME_FROM", dtExpTimeFrom.DateTime.ToString("dd/MM/yyyy"));
                singleTag.AddSingleKey(store, "EXP_TIME_TO", dtExpTimeTo.DateTime.ToString("dd/MM/yyyy"));
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
                MOS.Filter.HisExpMestViewFilter expFilter = new MOS.Filter.HisExpMestViewFilter();
                expFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                expFilter.MEDI_STOCK_ID = medistock.ID;
                expFilter.EXP_MEST_TYPE_IDs = new List<long>();
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS);
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP);
                //Review
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__DEPA);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__EXPE);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__LIQU);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__LOST);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__MANU);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__OTHER);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__PRES);
                //expFilter.EXP_MEST_TYPE_IDs.Add(Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__SALE);

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue) expFilter.FINISH_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtExpTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue) expFilter.FINISH_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtExpTimeTo.DateTime.ToString("yyyyMMdd") + "000000");

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
                if (exportList != null && exportList.Count > 0)
                {
                    //exportList = exportList.Where(w => w.EXP_MEST_TYPE_ID != Base.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__AGGR).ToList();
                    expCode = exportList.Select(s => s.EXP_MEST_CODE).OrderBy(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                expCode = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnListDepa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnListDepa.Enabled) return;
                if (lciBtnListDepa.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;

                if (dtExpTimeFrom.EditValue == null || dtExpTimeTo.EditValue == null)
                {
                    MessageBox.Show(Resources.ResourceMessage.BanChuaChonThoiGianThucXuat);
                    if (dtExpTimeFrom.EditValue == null)
                    {
                        dtExpTimeFrom.Focus();
                        dtExpTimeFrom.SelectAll();
                    }
                    else if (dtExpTimeTo.EditValue == null)
                    {
                        dtExpTimeTo.Focus();
                        dtExpTimeTo.SelectAll();
                    }
                    return;
                }

                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DepaReport));
                thread.Priority = ThreadPriority.Normal;
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

        private void DepaReport()
        {
            try
            {
                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);
                List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2> listDepa = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>();
                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachCacMaPhieuXuatSuDung.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayBieuMauIn, templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    //getdata
                    //Review
                    GetDataDepaProcessor(ref listDepa);

                    ProcessDataDepa(listDepa, ref store);
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
                WaitingManager.Hide();
            }
        }

        //Reiew
        private void ProcessDataDepa(List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2> listDepa, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                List<Base.ExportListCodeRDO> listRdo = new List<Base.ExportListCodeRDO>();
                List<Base.ExportListCodeRDO> listDepartment = new List<Base.ExportListCodeRDO>();
                Dictionary<string, object> singleValueDictionary = new Dictionary<string, object>();

                if (listDepa != null && listDepa.Count > 0)
                {
                    listDepa = listDepa.OrderBy(o => o.EXP_MEST_CODE).ToList();
                    var groups = listDepa.GroupBy(g => new { g.REQ_DEPARTMENT_ID }).ToList();
                    foreach (var depa in groups)
                    {
                        Base.ExportListCodeRDO department = new Base.ExportListCodeRDO();
                        department.DEPARTMENT_NAME = depa.First().REQ_DEPARTMENT_NAME;
                        listDepartment.Add(department);
                        var roomGroups = depa.ToList().GroupBy(g => new { g.REQ_ROOM_ID }).ToList();
                        foreach (var rooms in roomGroups)
                        {

                            var expCode = rooms.ToList();
                            Dictionary<int, List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>> dicExpCode = new Dictionary<int, List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>>();

                            int count = expCode.Count;
                            int max = count / 6;
                            int size = count % 6;
                            MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 emty = new MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2();

                            if (count > 31)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    if (i != 5)
                                    {
                                        dicExpCode[i] = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>();
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
                                        dicExpCode[i] = new List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>();
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
                                a.EXPORT_CODE1 = dicExpCode[0][i].EXP_MEST_CODE;
                                a.EXPORT_CODE2 = dicExpCode[1][i].EXP_MEST_CODE;
                                a.EXPORT_CODE3 = dicExpCode[2][i].EXP_MEST_CODE;
                                a.EXPORT_CODE4 = dicExpCode[3][i].EXP_MEST_CODE;
                                a.EXPORT_CODE5 = dicExpCode[4][i].EXP_MEST_CODE;
                                a.EXPORT_CODE6 = dicExpCode[5][i].EXP_MEST_CODE;
                                a.ROOM_NAME = rooms.First().REQ_ROOM_NAME;
                                a.ROOM_ID = rooms.First().REQ_ROOM_ID;
                                a.DEPARTMENT_NAME = rooms.First().REQ_DEPARTMENT_NAME;
                                listRdo.Add(a);
                            }
                        }
                    }
                }

                //sắp xếp để merge dòng
                listDepartment = listDepartment.OrderBy(o => o.DEPARTMENT_NAME).ToList();
                listRdo = listRdo.OrderBy(o => o.DEPARTMENT_NAME).ThenBy(t => t.ROOM_ID).ToList();

                singleTag.AddSingleKey(store, "TYPE", "THỰC XUẤT");
                singleTag.AddSingleKey(store, "MEDI_STOCK_NAME", medistock.MEDI_STOCK_NAME.ToUpper());
                singleTag.AddSingleKey(store, "EXP_TIME_FROM", dtExpTimeFrom.DateTime.ToString("dd/MM/yyyy"));
                singleTag.AddSingleKey(store, "EXP_TIME_TO", dtExpTimeTo.DateTime.ToString("dd/MM/yyyy"));
                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(singleValueDictionary);
                singleTag.ProcessData(store, singleValueDictionary);
                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "List", listRdo);
                objectTag.AddObjectData(store, "department", listDepartment);
                objectTag.AddRelationship(store, "department", "List", "DEPARTMENT_NAME", "DEPARTMENT_NAME");
                objectTag.SetUserFunction(store, "FuncSameTitleColRemedy", new CustomerFuncMergeSameData(listRdo));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataDepaProcessor(ref List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2> listDepa)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter expFilter = new MOS.Filter.HisExpMestViewFilter();
                expFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                expFilter.MEDI_STOCK_ID = medistock.ID;
                expFilter.EXP_MEST_TYPE_IDs = new List<long>();
                expFilter.EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP);

                //expFilter.MEDI_STOCK_ID = medistock.ID;

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    expFilter.FINISH_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMdd") + "000000");

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2>>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
                if (exportList != null && exportList.Count > 0)
                {
                    listDepa = exportList;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridView_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (row != null)
                {
                    LoadInfoClick(row);
                    LoadVisibility(row);
                    ShowPopupDetail(row, e);
                    if (e.Column.FieldName == "CallPatient")
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridView_RowCellClick. 1");

                        LoadCallPatientByThread(row);
                    }
                    //gridView.FocusedColumn = gridView.Columns[1];
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboStatus_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    foreach (var item in _StatusSelecteds)
                    {
                        statusName += item.EXP_MEST_STT_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string typeName = "";
                if (_TypeSelecteds != null && _TypeSelecteds.Count > 0)
                {
                    foreach (var item in _TypeSelecteds)
                    {
                        typeName += item.EXP_MEST_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //cboType.Focus();
                //cboType.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboPatientType.Focus();
                cboPatientType.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCreateTimeFrom.Focus();
                    dtCreateTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                // dtCreateTimeTo.Focus();
                //dtCreateTimeTo.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //dtExpTimeFrom.Focus();
                //dtExpTimeFrom.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //dtExpTimeTo.Focus();
                //dtExpTimeTo.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //cboStatus.Focus();
                //cboStatus.ShowPopup();
                var now = dtExpTimeTo.DateTime;
                dtExpTimeTo.EditValue = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_MobaDepaCreate_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.ID);
                    CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_ImpMestCreate_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (row != null)
                {
                    var expMest = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMest, row);
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(expMest);
                    listArgs.Add(expMest.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCreateTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCreateTimeTo.Focus();
                    dtCreateTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtCreateTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpTimeFrom.Focus();
                    dtExpTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpTimeTo.Focus();
                    dtExpTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboStatus.Focus();
                    cboStatus.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboType.Focus();
                    cboType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    //cboType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_Popup(object sender, EventArgs e)
        {

        }

        private void gridViewStatus_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                PhimTatCombo(e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void PhimTatCombo(KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Control | e.KeyCode == Keys.F)
                {
                    Search();
                }
                else if (e.KeyCode == Keys.Control | e.KeyCode == Keys.R)
                {
                    Refresh();
                }
                else if (e.KeyCode == Keys.Control | e.KeyCode == Keys.E)
                {
                    Export();
                }
                else if (e.KeyCode == Keys.F2)
                {
                    FocusExpCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                PhimTatCombo(e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_HuyThucXuat_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy thực xuất không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                    V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                    if (row != null)
                    {
                        if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var data = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrExamUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);

                                if (rs != null)
                                {
                                    foreach (var item in data)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    data = data.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = data;
                                    gridView.EndUpdate();

                                }

                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                        else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var data = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/InPresUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                if (rs != null)
                                {
                                    foreach (var item in data)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    data = data.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = data;
                                    gridView.EndUpdate();

                                }
                            }

                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion

                        }
                        else
                        {
                            WaitingManager.Show();
                            bool success = false;

                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
                            {
                                MessageBox.Show(Resources.ResourceMessage.PhieuTongHopKhongDuocHuyThucXuat);
                                return;
                            }

                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;

                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                            if (rs != null)
                            {
                                if (rs != null)
                                {
                                    success = true;
                                    RefreshData();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }
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
                WaitingManager.Show();
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST expMestData = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMestData, rowDataExpMest);
                if (expMestData != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "EXP_MEST_CODE: " + expMestData.EXP_MEST_CODE);
                    listArgs.Add(dataInit3);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    CallModule callModule = new CallModule(CallModule.EventLog, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_HuyTuChoiDuyet_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                    hisExpMestApproveSDO.ExpMestId = row.ID;
                    //hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestApproveSDO.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var data = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;

                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                   "api/HisExpMest/Undecline", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                        if (rs != null)
                        {
                            foreach (var item in data)
                            {
                                if (item.ID == rs.ID)
                                {
                                    var ExpMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                    item.EXP_MEST_STT_ID = ExpMestSTT.ID;
                                    item.EXP_MEST_STT_NAME = ExpMestSTT.EXP_MEST_STT_NAME;
                                    item.EXP_MEST_STT_CODE = ExpMestSTT.EXP_MEST_STT_CODE;
                                    item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                    break;
                                }
                            }
                            success = true;
                            gridView.BeginUpdate();
                            data = data.OrderByDescending(p => p.MODIFY_TIME).ToList();
                            gridControl.DataSource = data;
                            gridView.EndUpdate();
                        }
                    }
                    WaitingManager.Hide();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSearchTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtSearchTreatmentCode.Text))
                    {
                        RefreshData();
                        txtSearchTreatmentCode.SelectAll();
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

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                    {
                        RefreshData();
                        txtServiceReqCode.SelectAll();
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

        private void txtSearchPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtSearchPatientCode.Text))
                    {
                        RefreshData();
                        txtSearchPatientCode.SelectAll();
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

        private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var data = (V_HIS_EXP_MEST_2)gridView.GetRow(e.RowHandle);
                if (data != null && data.IS_NOT_TAKEN == 1)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);

                }
                if (data.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    if(data.BLOOD_LEVEL == 1)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }else if (data.BLOOD_LEVEL == 2)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                    }
                    else if (data.BLOOD_LEVEL == 3)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Regular);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_HoanThanh_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisExpMestSDO hisExpMestFinishSDO = new MOS.SDO.HisExpMestSDO();

                    hisExpMestFinishSDO.ExpMestId = row.ID;
                    //hisExpMestApproveSDO.IsFinish = true;
                    hisExpMestFinishSDO.ReqRoomId = this.roomId;
                    if (gridControl.DataSource != null)
                    {
                        var data = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                   "api/HisExpMest/Finish", ApiConsumers.MosConsumer, hisExpMestFinishSDO, param);
                        if (rs != null)
                        {
                            if (rs != null)
                            {
                                foreach (var item in data)
                                {
                                    if (item.ID == rs.ID)
                                    {
                                        var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                        item.EXP_MEST_STT_ID = expMestSTT.ID;
                                        item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                        item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                        item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                        break;
                                    }
                                }
                                success = true;
                                gridView.BeginUpdate();
                                data = data.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                gridControl.DataSource = data;
                                gridView.EndUpdate();
                            }
                        }
                    }
                    WaitingManager.Hide();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_Check_TruyenMau_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowDataExpMest = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowDataExpMest);
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                this.rightClickData = null;
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridView.GetVisibleRowHandle(hi.RowHandle);

                    rowFocusGrid = (V_HIS_EXP_MEST_2)gridView.GetRow(rowHandle);
                    V_HIS_EXP_MEST row = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(row, rowFocusGrid);
                    if (row != null)
                    {
                        this.rightClickData = row;

                        gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
                        gridView.OptionsSelection.EnableAppearanceFocusedRow = true;

                        BarManager barManager = new BarManager();
                        barManager.Form = this.ParentForm;

                        ExpMestADO expMestAdo = new ExpMestADO();
                        expMestAdo.LoginName = LoggingName;
                        expMestAdo.MediStock = medistock;
                        expMestAdo.listImpMest = listImpMest;
                        expMestAdo.controlAcs = controlAcs;

                        PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(expMestAdo, row, rowFocusGrid, barManager, MouseRight_Click);
                        popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.rightClickData != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        #region -----ThaoTac

                        case PopupMenuProcessor.ItemType.Sua:
                            Sua(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.Xoa:
                            Xoa(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.Duyet:
                            Duyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HuyDuyet:
                            HuyDuyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TuChoiDuyet:
                            TuChoiDuyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HuyTuChoiDuyet:
                            HuyTuChoiDuyet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.ThucXuat:
                            ThucXuat(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HuyThucXuat:
                            HuyThucXuat(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.HoanThanh:
                            HoanThanh(this.rightClickData);
                            break;

                        #endregion

                        case PopupMenuProcessor.ItemType.XemChiTiet:
                            XemChiTiet(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapHaoPhiTraLai:
                            TaoPhieuNhapHaoPhiTraLai(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapThuHoiMau:
                            TaoPhieuNhapThuHoiMau(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapThuHoi:
                            TaoPhieuNhapHaoPhiTraLai(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapBuCoSo:
                            TaoPhieuNhapBuCoSo(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapChuyenKho:
                            TaoPhieuNhapChuyenKho(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.TaoPhieuNhapBuLe:
                            TaoPhieuNhapBuLe(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.ChiDinhXetNghiem:
                            ChiDinhXetNghiem(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.LichSuTacDong:
                            LichSuTacDong(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.ThuHoiDonPhongKham:
                            ThuHoiDonPhongKham(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.KhongLay:
                            KhongLayXuatBan(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.PhucHoiDonKhongLay:
                            PhucHoiDonKhongLay(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.XacNhanNo:
                            XacNhanNo(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.LyDoXuat:
                            LyDoXuat(this.rightClickData);
                            break;
                        case PopupMenuProcessor.ItemType.InHoaDonDt:
                            InHoaDonDt(this.rowFocusGrid);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InHoaDonDt(V_HIS_EXP_MEST_2 rowFocusGrid)
        {
            try
            {
                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(rowFocusGrid.INVOICE_CODE);

                dataInput.InvoiceCode = rowFocusGrid.INVOICE_CODE;
                dataInput.NumOrder = rowFocusGrid.NUM_ORDER ?? -1;
                dataInput.SymbolCode = rowFocusGrid.SYMBOL_CODE;
                dataInput.TemplateCode = rowFocusGrid.TEMPLATE_CODE;
                dataInput.TransactionTime = rowFocusGrid.EINVOICE_TIME ?? rowFocusGrid.TRANSACTION_TIME ?? 0;
                dataInput.ENumOrder = rowFocusGrid.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = rowFocusGrid.EINVOICE_TYPE_ID;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                tran.TRANSACTION_CODE = rowFocusGrid.TRANSACTION_CODE;
                dataInput.Transaction = tran;
                V_HIS_TREATMENT_FEE treatment2 = new V_HIS_TREATMENT_FEE();
                if (rightClickData.TDL_TREATMENT_ID.HasValue)
                {
                    MOS.Filter.HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                    filter.ID = rightClickData.TDL_TREATMENT_ID;
                    treatment2 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                }
                else
                {
                    treatment2.TDL_PATIENT_ACCOUNT_NUMBER = rightClickData.TDL_PATIENT_ACCOUNT_NUMBER;
                    treatment2.TDL_PATIENT_ADDRESS = rightClickData.TDL_PATIENT_ADDRESS;
                    treatment2.TDL_PATIENT_PHONE = rightClickData.TDL_PATIENT_PHONE;
                    treatment2.TDL_PATIENT_TAX_CODE = rightClickData.TDL_PATIENT_TAX_CODE;
                    treatment2.TDL_PATIENT_WORK_PLACE = rightClickData.TDL_PATIENT_WORK_PLACE;
                    treatment2.TDL_PATIENT_NAME = rightClickData.TDL_PATIENT_NAME;
                    treatment2.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                    treatment2.PATIENT_ID = -1;
                }

                string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.CONFIG");

                if ((serviceConfig.Contains(ProviderType.VIETTEL) || rowFocusGrid.INVOICE_SYS == ProviderType.VIETTEL) &&
                    HisConfigCFG.autoPrintType != "1")
                {
                    if (XtraMessageBox.Show("Bạn có muốn lấy thông tin người chuyển đổi theo thông tin người lưu ký không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    {
                        dataInput.Converter = rightClickData.CASHIER_USERNAME;
                    }
                }
                dataInput.Treatment = treatment2;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    if (electronicBillResult != null && electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        MessageBox.Show("Tải hóa đơn điện tử thất bại. " + string.Join(". ", electronicBillResult.Messages));
                    }
                    else
                        MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    return;
                }

                Inventec.Common.DocumentViewer.DocumentViewerManager viewManager = new Inventec.Common.DocumentViewer.DocumentViewerManager(ViewType.ENUM.Pdf);
                InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.NumberOfCopy = HisConfigCFG.E_BILL__PRINT_NUM_COPY;
                ado.URL = electronicBillResult.InvoiceLink;
                ViewType.Platform type = ViewType.Platform.Telerik;
                if (HisConfigCFG.PlatformOption > 0)
                {
                    type = (ViewType.Platform)(HisConfigCFG.PlatformOption - 1);
                }
                viewManager.Run(ado, type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LyDoXuat(V_HIS_EXP_MEST data)
        {
            try
            {
                frmReasonExp frm = new frmReasonExp(this.currentModule, data, (DelegateRefeshData)FillDataToControl);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Click

        private void XemChiTiet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
                        {
                            HIS.Desktop.ADO.ApproveAggrExpMestSDO exeMestView = new HIS.Desktop.ADO.ApproveAggrExpMestSDO(ExpMestData.ID, ExpMestData.EXP_MEST_STT_ID);
                            List<object> listArgs = new List<object>();
                            listArgs.Add(exeMestView);
                            CallModule callModule = new CallModule(CallModule.ApproveAggrExpMest, this.roomId, this.roomTypeId, listArgs);

                            WaitingManager.Hide();
                        }
                        else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(ExpMestData);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.AggrExpMestDetail, this.roomId, this.roomTypeId, listArgs);

                            WaitingManager.Hide();
                        }
                        else
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(ExpMestData);
                            CallModule callModule = new CallModule(CallModule.ExpMestViewDetail, this.roomId, this.roomTypeId, listArgs);

                            WaitingManager.Hide();
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Xoa(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    try
                    {
                        bool success = false;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                            Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
                            Resources.ResourceMessage.ThongBao,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (row != null)
                            {
                                WaitingManager.Show();

                                if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                                {
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<bool>
                                        ("api/HisExpMest/AggrExamDelete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                                else
                                {
                                    HisExpMestSDO sdo = new HisExpMestSDO();
                                    sdo.ExpMestId = row.ID;
                                    sdo.ReqRoomId = this.roomId;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<bool>
                                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresul)
                                    {
                                        success = true;
                                        RefreshData();
                                    }
                                }
                                WaitingManager.Hide();
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
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Duyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK
                                     || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP
                                     || row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(row.ID);
                                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                                CallModule callModule = new CallModule(CallModule.BrowseExportTicket, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterSave(true);
                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(row.ID);
                                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                                CallModule callModule = new CallModule(CallModule.ApprovalExpMestBcs, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterSave(true);
                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                               "api/HisExpMest/InPresApprove", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ExpMest.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.ExpMest.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestSDO.ExpMestId = row.ID;
                                hisExpMestSDO.ReqRoomId = this.roomId;

                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_EXP_MEST>>(
                           "api/HisExpMest/AggrExamApprove", ApiConsumers.MosConsumer, hisExpMestSDO, param);
                                if (rs != null && rs.Count > 0)
                                {
                                    success = true;
                                    RefreshData();
                                }

                                WaitingManager.Hide();
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestApproveSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestApproveSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                               "api/HisExpMest/Approve", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ExpMest.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.ExpMest.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
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
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HuyDuyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();

                                HisExpMestSDO data = new HisExpMestSDO();
                                data.ExpMestId = row.ID;
                                data.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/AggrExamUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                    if (apiresul != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresul.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion
                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();

                                HisExpMestSDO data = new HisExpMestSDO();
                                data.ExpMestId = row.ID;
                                data.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/InPresUnapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                    if (apiresul != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresul.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();

                                HisExpMestSDO data = new HisExpMestSDO();
                                data.ExpMestId = row.ID;
                                data.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, data, param);
                                    if (apiresul != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresul.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TuChoiDuyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO ado = new HisExpMestSDO();
                            ado.ExpMestId = row.ID;
                            ado.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var apiresul = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/Decline", ApiConsumer.ApiConsumers.MosConsumer, ado, param);
                                if (apiresul != null)
                                {
                                    foreach (var item in griddata)
                                    {
                                        if (item.ID == apiresul.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresul.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = griddata;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HuyTuChoiDuyet(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestApproveSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestApproveSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                           "api/HisExpMest/Undecline", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                if (rs != null)
                                {
                                    foreach (var item in griddata)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = griddata;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThucXuat(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                            {
                                bool IsFinish = false;
                                if (row.IS_EXPORT_EQUAL_APPROVE == 1)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Đã xuất hết số lượng duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                else if (row.IS_EXPORT_EQUAL_APPROVE == null || row.IS_EXPORT_EQUAL_APPROVE != 1)
                                {
                                    HisExpMestMetyReqFilter expMestMetyReqFilter = new HisExpMestMetyReqFilter();
                                    expMestMetyReqFilter.EXP_MEST_ID = row.ID;

                                    var listExpMestMetyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, expMestMetyReqFilter, param);

                                    HisExpMestMatyReqFilter expMestMatyReqFilter = new HisExpMestMatyReqFilter();
                                    expMestMatyReqFilter.EXP_MEST_ID = row.ID;

                                    var listExpMestMatyReq = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, expMestMatyReqFilter, param);

                                    List<AmountADO> amountAdo = new List<AmountADO>();

                                    if (listExpMestMetyReq != null && listExpMestMetyReq.Count > 0)
                                    {
                                        foreach (var item in listExpMestMetyReq)
                                        {
                                            var ado = new AmountADO(item);
                                            amountAdo.Add(ado);
                                        }
                                    }

                                    if (listExpMestMatyReq != null && listExpMestMatyReq.Count > 0)
                                    {
                                        foreach (var item in listExpMestMatyReq)
                                        {
                                            var ado = new AmountADO(item);
                                            amountAdo.Add(ado);
                                        }
                                    }

                                    if (amountAdo != null && amountAdo.Count > 0)
                                    {
                                        var dataAdo = amountAdo.Where(o => o.Amount > o.Dd_Amount || o.Dd_Amount == null).ToList();
                                        //if (dataAdo != null && dataAdo.Count > 0)
                                        //{
                                        //    if (XtraMessageBox.Show("Phiếu chưa duyệt đủ số lượng yêu cầu. Bạn có muốn hoàn thành phiếu xuất?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        //    {
                                        //        IsFinish = true;
                                        //    }
                                        //}
                                        //else
                                        IsFinish = true;
                                    }

                                }

                                HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                sdo.IsFinish = IsFinish;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                HisExpMestSDO sdo = new HisExpMestSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                //sdo.IsFinish = true;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        ("api/HisExpMest/InPresExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                HisExpMestSDO sdo = new HisExpMestSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                //sdo.IsFinish = true;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        ("api/HisExpMest/AggrExamExport", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else
                            {
                                HisExpMestExportSDO sdo = new HisExpMestExportSDO();
                                sdo.ExpMestId = row.ID;
                                sdo.ReqRoomId = this.roomId;
                                sdo.IsFinish = true;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                                        (param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>
                                        (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_EXPORT, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                                    if (apiresult != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == apiresult.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == apiresult.EXP_MEST_STT_ID);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
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
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HuyThucXuat(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                //hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/AggrExamUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion
                            }
                            else if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                //hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/InPresUnexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
                                #region Show message
                                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                                #endregion

                                #region Process has exception
                                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            else
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();
                                HisExpMestSDO hisExpMestApproveSDO = new MOS.SDO.HisExpMestSDO();

                                hisExpMestApproveSDO.ExpMestId = row.ID;
                                //hisExpMestApproveSDO.IsFinish = true;
                                hisExpMestApproveSDO.ReqRoomId = this.roomId;
                                if (gridControl.DataSource != null)
                                {
                                    var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/Unexport", ApiConsumers.MosConsumer, hisExpMestApproveSDO, param);
                                    if (rs != null)
                                    {
                                        foreach (var item in griddata)
                                        {
                                            if (item.ID == rs.ID)
                                            {
                                                var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE);
                                                item.EXP_MEST_STT_ID = expMestSTT.ID;
                                                item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                                item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                                item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                                break;
                                            }
                                        }
                                        success = true;
                                        gridView.BeginUpdate();
                                        griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                        gridControl.DataSource = griddata;
                                        gridView.EndUpdate();
                                    }
                                }
                                WaitingManager.Hide();
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
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HoanThanh(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        if (row != null)
                        {
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            HisExpMestSDO hisExpMestFinishSDO = new MOS.SDO.HisExpMestSDO();

                            hisExpMestFinishSDO.ExpMestId = row.ID;
                            //hisExpMestApproveSDO.IsFinish = true;
                            hisExpMestFinishSDO.ReqRoomId = this.roomId;
                            if (gridControl.DataSource != null)
                            {
                                var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                           "api/HisExpMest/Finish", ApiConsumers.MosConsumer, hisExpMestFinishSDO, param);
                                if (rs != null)
                                {
                                    foreach (var item in griddata)
                                    {
                                        if (item.ID == rs.ID)
                                        {
                                            var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                            item.EXP_MEST_STT_ID = expMestSTT.ID;
                                            item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                            item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                            item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                            break;
                                        }
                                    }
                                    success = true;
                                    gridView.BeginUpdate();
                                    griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                    gridControl.DataSource = griddata;
                                    gridView.EndUpdate();
                                }
                            }
                            WaitingManager.Hide();
                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapHaoPhiTraLai(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData != null)
                        {
                            if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaBloodCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                            {
                                WaitingManager.Hide();
                                if (HisConfigCFG.DO_NOT_ALLOW_MOBA_HAS_TRANSACTION_SALE_EXP_MEST == "1" && row.BILL_ID != null)
                                {
                                    HisTransactionFilter ft = new HisTransactionFilter();
                                    ft.ID = row.BILL_ID;
                                    var transaction = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();
                                    if (transaction != null)
                                        XtraMessageBox.Show(string.Format("Phiếu xuất {0} đã được thanh toán (mã thanh toán: {1})", row.EXP_MEST_CODE, transaction.TRANSACTION_CODE));
                                }
                                else
                                {
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(ExpMestData.ID);
                                    CallModule callModule = new CallModule(CallModule.MobaSaleCreate, this.roomId, this.roomTypeId, listArgs);

                                    FillDataApterClose(ExpMestData);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void TaoPhieuNhapThuHoiMau(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData != null)
                        {
                            if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaDepaCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaBloodCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                            else if (ExpMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                            {
                                List<object> listArgs = new List<object>();
                                listArgs.Add(ExpMestData.ID);
                                CallModule callModule = new CallModule(CallModule.MobaSaleCreate, this.roomId, this.roomTypeId, listArgs);

                                WaitingManager.Hide();
                                FillDataApterClose(ExpMestData);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapBuCoSo(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add(row.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapChuyenKho(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add(row.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TaoPhieuNhapBuLe(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add(row.EXP_MEST_TYPE_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    CallModule callModule = new CallModule(CallModule.ImpMestChmsCreate, this.roomId, this.roomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChiDinhXetNghiem(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();
                    var expMestData = row;
                    if (expMestData != null)
                    {
                        List<object> listArgs = new List<object>();
                        AssignServiceTestADO assignBloodADO = new AssignServiceTestADO(0, 0, 0, null);
                        GetTreatmentIdFromResultData(expMestData, ref assignBloodADO);
                        listArgs.Add(assignBloodADO);
                        long keyconfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.AssignServiceTestSelect");
                        if (keyconfig == 1)
                        {
                            CallModule callModule = new CallModule(CallModule.AssignServiceTest, this.roomId, this.roomTypeId, listArgs);
                        }
                        else
                        {
                            CallModule callModule = new CallModule(CallModule.AssignServiceTestMulti, this.roomId, this.roomTypeId, listArgs);
                        }

                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LichSuTacDong(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit dataInit = new Inventec.UC.EventLogControl.Data.DataInit(ConfigApplications.NumPageSize, "", "", "", "", row.EXP_MEST_CODE, "");
                    KeyCodeADO ado = new KeyCodeADO();
                    ado.expMestCode = row.EXP_MEST_CODE;
                    listArgs.Add(ado);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                    var moduleWK = PluginInstance.GetModuleWithWorkingRoom(this.currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EventLog", moduleWK.RoomId, moduleWK.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Sua(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    var expMestData = row;
                    if (expMestData != null)
                    {
                        if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__KHAC)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.ExpMestOtherExport, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData.ID);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                            CallModule callModule = new CallModule(CallModule.ExpMestSaleCreate, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData);
                            CallModule callModule = new CallModule(CallModule.ExpMestChmsUpdate, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData);
                            CallModule callModule = new CallModule(CallModule.ExpMestDepaUpdate, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else if (expMestData.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__TNCC)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMestData);
                            CallModule callModule = new CallModule(CallModule.ManuExpMestCreate, this.roomId, this.roomTypeId, listArgs);

                            RefreshData();
                        }
                        else
                            MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                    }
                    else
                        MessageManager.Show(Resources.ResourceMessage.ChucNangDangPhatTrienVuiLongThuLaiSau);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThuHoiDonPhongKham(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    try
                    {
                        WaitingManager.Show();
                        var ExpMestData = row;

                        if (ExpMestData != null)
                        {
                            WaitingManager.Show();

                            List<object> listObj = new List<object>();
                            listObj.Add(ExpMestData.SERVICE_REQ_ID);
                            CallModule callModule = new CallModule(CallModule.MobaExamPresCreate, this.roomId, this.roomTypeId, listObj);

                            WaitingManager.Hide();
                        }

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void KhongLayXuatBan(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    if (row.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK)
                    {
                        MOS.Filter.HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.ID = row.TDL_TREATMENT_ID;
                        var treatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                        if (treatment != null && ((treatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && XtraMessageBox.Show("Bạn có muốn chắc muốn đánh dấu đơn bệnh nhân không lấy không?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes) || (treatment.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !CheckLoginAdmin.IsAdmin(LoggingName) && XtraMessageBox.Show("Hồ sơ điều trị đã khóa viện phí", "Thông báo") == DialogResult.OK) || (treatment.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && CheckLoginAdmin.IsAdmin(LoggingName) && XtraMessageBox.Show("Hồ sơ điều trị đã khóa viện phí. Bạn có muốn mở khóa hồ sơ và đánh dấu đơn bệnh nhân không lấy không?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)))
                            return;
                    }
                    else if (XtraMessageBox.Show("Bạn có muốn chắc muốn đánh dấu đơn bệnh nhân không lấy không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True) != DialogResult.Yes)
                    {
                        return;
                    }
                    try
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        
                        if (gridControl.DataSource != null)
                        {
                            MOS.SDO.HisExpMestNotTakenSDO sdo = new HisExpMestNotTakenSDO();
                            sdo.ExpMestId = row.ID;
                            sdo.RequestRoomId = this.medistock.ROOM_ID;
                            var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/NotTaken", ApiConsumers.MosConsumer, sdo, param);
                            if (rs != null)
                            {
                                foreach (var item in griddata)
                                {
                                    if (item.ID == rs.ID)
                                    {
                                        var expMestSTT = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(o => o.ID == rs.EXP_MEST_STT_ID);
                                        item.EXP_MEST_STT_ID = expMestSTT.ID;
                                        item.EXP_MEST_STT_NAME = expMestSTT.EXP_MEST_STT_NAME;
                                        item.EXP_MEST_STT_CODE = expMestSTT.EXP_MEST_STT_CODE;
                                        item.MODIFY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                                        break;
                                    }
                                }
                                success = true;
                                gridView.BeginUpdate();
                                griddata = griddata.OrderByDescending(p => p.MODIFY_TIME).ToList();
                                gridControl.DataSource = griddata;
                                gridView.EndUpdate();
                            }
                        }
                        WaitingManager.Hide();

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PhucHoiDonKhongLay(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    if (XtraMessageBox.Show("Bạn có muốn phục hồi đơn không lấy không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True) != DialogResult.Yes)
                    {
                        return;
                    }
                    try
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = false;
                        HisExpMestSDO ExpMestData = new HisExpMestSDO();
                        ExpMestData.ExpMestId = row.ID;
                        ExpMestData.ReqRoomId = this.medistock.ROOM_ID;
                        if (gridControl.DataSource != null)
                        {
                            var griddata = (List<V_HIS_EXP_MEST_2>)gridControl.DataSource;
                            var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                               "api/HisExpMest/RecoverNotTaken", ApiConsumers.MosConsumer, ExpMestData, param);
                            if (rs != null)
                            {
                                success = true;
                                btnSearch_Click(null, null);
                            }
                        }
                        WaitingManager.Hide();

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void XacNhanNo(V_HIS_EXP_MEST row)
        {
            try
            {
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(new List<long>() { row.ID });
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataApterSave);
                    CallModule callModule = new CallModule(CallModule.DrugStoreDebt, this.roomId, this.roomTypeId, listArgs);

                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void Btn_Check_TruyenMau_Disable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void cboPatientType_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string patientTypeName = "";
                if (_PatientTypeSelecteds != null && _PatientTypeSelecteds.Count > 0)
                {
                    foreach (var item in _PatientTypeSelecteds)
                    {
                        patientTypeName += item.PATIENT_TYPE_NAME + ", ";
                    }
                }
                e.DisplayText = patientTypeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        class CustomerFuncMergeSameData : TFlexCelUserFunction
        {
            List<Base.ExportListCodeRDO> rdo;

            public CustomerFuncMergeSameData(List<Base.ExportListCodeRDO> rdo)
            {
                this.rdo = rdo;
            }

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    int row = Convert.ToInt32(parameters[0]);
                    long id = Convert.ToInt64(parameters[1]);
                    if (row > 0)
                    {
                        if (rdo[row - 1].ROOM_ID == id)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                return result;
            }
        }

        private void gridPatientType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.Column != null && info.HitTest == GridHitTest.Column && info.Column.FieldName == "CheckMarkSelection")
                {
                    string patientTypeConn = "";
                    if (view != null)
                    {
                        int[] selectRow = view.GetSelectedRows();
                        foreach (var item in selectRow)
                        {
                            var selectTex = (HIS_PATIENT_TYPE)view.GetRow(item);
                            patientTypeConn += selectTex.PATIENT_TYPE_NAME + ", ";
                        }
                    }
                    cboPatientType.Text = patientTypeConn;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridPatientType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    string patientTypeConn = "";
            //    GridView grd = sender as GridView;
            //    if (grd != null)
            //    {
            //        int[] selectRow = grd.GetSelectedRows();
            //        foreach (var item in selectRow)
            //        {
            //            var selectTex = (HIS_PATIENT_TYPE)grd.GetRow(item);
            //            patientTypeConn += selectTex.PATIENT_TYPE_NAME + ", ";
            //        }
            //    }
            //    cboPatientType.Text = patientTypeConn;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void gridViewStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string status = "";
                GridView grd = sender as GridView;
                if (grd != null)
                {
                    int[] selectRow = grd.GetSelectedRows();
                    foreach (var item in selectRow)
                    {
                        var selectTex = (HIS_EXP_MEST_STT)grd.GetRow(item);
                        status += selectTex.EXP_MEST_STT_NAME + ", ";
                    }
                }
                cboStatus.Text = status;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewStatus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.Column != null && info.HitTest == GridHitTest.Column && info.Column.FieldName == "CheckMarkSelection")
                {
                    string status = "";
                    if (view != null)
                    {
                        int[] selectRow = view.GetSelectedRows();
                        foreach (var item in selectRow)
                        {
                            var selectTex = (HIS_EXP_MEST_STT)view.GetRow(item);
                            status += selectTex.EXP_MEST_STT_NAME + ", ";
                        }
                    }
                    cboStatus.Text = status;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.Column != null && info.HitTest == GridHitTest.Column && info.Column.FieldName == "CheckMarkSelection")
                {
                    string Type = "";
                    if (view != null)
                    {
                        int[] selectRow = view.GetSelectedRows();
                        foreach (var item in selectRow)
                        {
                            var selectTex = (HIS_EXP_MEST_TYPE)view.GetRow(item);
                            Type += selectTex.EXP_MEST_TYPE_NAME + ", ";
                        }
                    }
                    cboType.Text = Type;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string Type = "";
                GridView grd = sender as GridView;
                if (grd != null)
                {
                    int[] selectRow = grd.GetSelectedRows();
                    foreach (var item in selectRow)
                    {
                        var selectTex = (HIS_EXP_MEST_TYPE)grd.GetRow(item);
                        Type += selectTex.EXP_MEST_TYPE_NAME + ", ";
                    }
                }
                cboType.Text = Type;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridPatientType_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                string patientTypeConn = "";
                GridView grd = sender as GridView;
                if (grd != null)
                {
                    int[] selectRow = grd.GetSelectedRows();
                    foreach (var item in selectRow)
                    {
                        var selectTex = (HIS_PATIENT_TYPE)grd.GetRow(item);
                        patientTypeConn += selectTex.PATIENT_TYPE_NAME + ", ";
                    }
                }
                cboPatientType.Text = patientTypeConn;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboRequestRoomIds_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (_RoomSelecteds != null && _RoomSelecteds.Count > 0)
                {
                    roomName = string.Join(",", _RoomSelecteds.Select(s => s.ROOM_NAME).ToList());
                }

                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboRequestRoomIds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                btnSearch.Focus();
                if (_TypeSelecteds != null && _TypeSelecteds.Count == 1 && _TypeSelecteds.FirstOrDefault().ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK)
                {
                    radioGroupStatus.SelectedIndex = 1;
                    radioGroupStatus.Enabled = true;
                }
                else
                {
                    radioGroupStatus.SelectedIndex = 1;
                    radioGroupStatus.Enabled = false;
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (_RoomSelecteds != null && _RoomSelecteds.Count > 0)
                {
                    roomName = string.Join(",", _RoomSelecteds.Select(s => s.ROOM_NAME).ToList());
                }

                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridRequestRoomView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.Location);
                if (info.Column != null && info.HitTest == GridHitTest.Column && info.Column.FieldName == "CheckMarkSelection")
                {
                    string patientTypeConn = "";
                    if (view != null)
                    {
                        int[] selectRow = view.GetSelectedRows();
                        foreach (var item in selectRow)
                        {
                            var selectTex = (V_HIS_MEST_ROOM)view.GetRow(item);
                            patientTypeConn += selectTex.ROOM_NAME + ", ";
                        }
                    }
                    cboPatientType.Text = patientTypeConn;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridRequestRoomView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string status = "";
                GridView grd = sender as GridView;
                if (grd != null)
                {
                    int[] selectRow = grd.GetSelectedRows();
                    foreach (var item in selectRow)
                    {
                        var selectTex = (V_HIS_MEST_ROOM)grd.GetRow(item);
                        status += selectTex.ROOM_NAME + ", ";
                    }
                }
                CboRequestRoomIds.Text = status;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowPopupDetail(V_HIS_EXP_MEST_2 row, RowCellClickEventArgs e)
        {
            try
            {
                popupControlExpMestDetail.HidePopup();
                if (row != null)
                {
                    long showdetail = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(Base.GlobalStore.ShowDetailClick));
                    if (showdetail == 1)
                    {
                        Gc_ExpMestDetail.DataSource = null;
                        GetDetailExpMest data = new GetDetailExpMest(row);
                        var dataDetail = data.Get();
                        Gc_ExpMestDetail.DataSource = dataDetail;
                        Gc_ExpMestDetail.RefreshDataSource();

                        //int rowcount = 10;
                        //if (dataDetail.Count < 9)
                        //{
                        //    rowcount = dataDetail.Count + 1;
                        //}
                        //popupControlExpMestDetail.Height = rowcount * 24;

                        //Point pShow = new Point(e.Location.X, e.Location.Y + 175);

                        //2 màn hình vẫn hiển thị đúng màn hình 
                        var screen = Screen.GetWorkingArea(ucPaging);
                        var screenchk = Screen.GetWorkingArea(chkAutoCallPatient);
                        int x = ucPaging.Width + chkAutoCallPatient.Width - popupControlExpMestDetail.Width;
                        Point pShow = new Point(ucPaging.Location.X + x + screen.X + screenchk.X, ucPaging.Location.Y);
                        popupControlExpMestDetail.ShowPopup(pShow);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_ExpMestDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPresNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPresNumber.Text))
                    {
                        RefreshData();
                        txtPresNumber.SelectAll();
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

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkInHDSD.Name)
                        {
                            chkInHDSD.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAutoCallPatient.Name)
                        {
                            chkAutoCallPatient.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkInHDSD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInHDSD.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInHDSD.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInHDSD.Name;
                    csAddOrUpdate.VALUE = (chkInHDSD.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHeinCard_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtHeinCard.Text))
                    {
                        RefreshData();
                        txtHeinCard.SelectAll();
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

        private void chkAutoCallPatient_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoCallPatient.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoCallPatient.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoCallPatient.Name;
                    csAddOrUpdate.VALUE = (chkAutoCallPatient.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
