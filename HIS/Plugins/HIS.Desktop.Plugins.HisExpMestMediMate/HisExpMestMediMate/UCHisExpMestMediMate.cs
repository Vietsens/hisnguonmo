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
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HIS.Desktop.Plugins.HisExpMestMediMate.HisExpMestMediMate
{
    public partial class UCHisExpMestMediMate : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare

        List<V_HIS_MEDI_STOCK> medistocks;
        List<V_HIS_EXP_MEST> expMest;//Các phiếu xuất
        //List<V_HIS_IMP_MEST> impMest;//Các phiếu nhập
        List<HIS_EXP_MEST> expMestByImp;//Các phiếu xuất theo phiếu nhập

        List<V_HIS_IMP_MEST_MEDICINE> ListImpMestMedicine;
        List<V_HIS_EXP_MEST_MEDICINE> ListExpMestmedicine;

        List<HIS_DEPARTMENT> glstDepartment = new List<HIS_DEPARTMENT>();
        List<V_HIS_MEDI_STOCK> glstMediStock = new List<V_HIS_MEDI_STOCK>();
        List<HIS_EXP_MEST_STT> glstExpMestStt = new List<HIS_EXP_MEST_STT>();
        List<HIS_IMP_MEST_STT> glstImpMestStt = new List<HIS_IMP_MEST_STT>();

        List<HisExpMestMediMateADO> listMedicineADO;

        private int MAX_REQUEST_LENGTH_PARAM = 500;

        Dictionary<long, string> dicChmsImpMest = new Dictionary<long, string>();

        Inventec.Desktop.Common.Modules.Module _Module;
        long RoomId;
        long RoomTypeId;
        #endregion

        public UCHisExpMestMediMate(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UCHisExpMestMediMate(Inventec.Desktop.Common.Modules.Module module, long roomId, long roomTypeId)
        : base(module)
        {
            InitializeComponent();
            try
            {
                WaitingManager.Show();
                this._Module = module;
                this.RoomId = roomId;
                this.RoomTypeId = roomTypeId;
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExpMestMediMate.Resources.Lang", typeof(UCHisExpMestMediMate).Assembly);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisExpMestMediMate_Load(object sender, EventArgs e)
        {
            try
            {
                dtFrom.EditValue = DateTime.Now;
                dtTo.EditValue = DateTime.Now;
                SetCaptionByLanguageKey();
                var _branchId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this._Module.RoomId).BranchId;
                var rooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => p.BRANCH_ID == _branchId).ToList();
                List<long> _RoomIds = (rooms != null && rooms.Count > 0) ? rooms.Select(p => p.ID).ToList() : null;
                List<long> _DepartmentIds = (rooms != null && rooms.Count > 0) ? rooms.Select(p => p.DEPARTMENT_ID).ToList() : null;

                glstDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
                glstMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                glstExpMestStt = BackendDataWorker.Get<HIS_EXP_MEST_STT>();
                glstImpMestStt = BackendDataWorker.Get<HIS_IMP_MEST_STT>();
                this.medistocks = glstMediStock.Where(p => p.IS_ACTIVE == 1 && _RoomIds.Contains(p.ROOM_ID)).ToList();

                //Load Combo
                InitCheck(cboSTT, SelectionGrid__Status);
                InitCombo(cboSTT, glstExpMestStt.Where(p => p.IS_ACTIVE == 1).ToList(), "EXP_MEST_STT_NAME", "ID");
                InitCheck(cboDepartment, SelectionGrid__Department);
                InitCombo(cboDepartment, glstDepartment.Where(p => p.IS_ACTIVE == 1 && _DepartmentIds.Contains(p.ID)).ToList(), "DEPARTMENT_NAME", "ID");
                InitCheck(cboExpMediStock, SelectionGrid__ExpMediStock);
                InitCombo(cboExpMediStock, medistocks.Where(p => p.IS_ACTIVE == 1 && _RoomIds.Contains(p.ROOM_ID)).ToList(), "MEDI_STOCK_NAME", "ID");
                InitCheck(cboExpMestType, SelectionGrid__ExpMestType);
                InitCombo(cboExpMestType, BackendDataWorker.Get<HIS_EXP_MEST_TYPE>().Where(p => p.IS_ACTIVE == 1).ToList(), "EXP_MEST_TYPE_NAME", "ID");
                //
                this.InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void LoadDefaultLoadForm()
        {
            try
            {
                cboSTT.Enabled = false;
                cboDepartment.Enabled = false;
                cboExpMediStock.Enabled = false;
                cboExpMestType.Enabled = false;
                cboSTT.Enabled = true;
                cboDepartment.Enabled = true;
                cboExpMediStock.Enabled = true;
                cboExpMestType.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExpMestMediMate.Resources.Lang", typeof(UCHisExpMestMediMate).Assembly);


                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHistoryMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.chkHistoryMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHistoryMedicine.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.chkHistoryMedicine.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHistoryMaterial.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.chkHistoryMaterial.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchMediMate.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.txtSearchMediMate.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExpMestType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.cboExpMestType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExpMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.cboExpMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSTT.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.cboSTT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem6.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem7.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn20.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn23.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn25.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn26.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn28.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("UCHisExpMestMediMate.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        int start = 0;
        int limit = 0;

        private void FillDataToGridControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Begin FillDataToGridControl");
                LoadDefaultLoadForm();

                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControlFormList);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void LoadGridData(object param)
        {
            try
            {
                if (this.chkHistoryMedicine.Checked)
                {
                    LoadGridDataHistoryMedicine(param);
                }
                else
                {
                    LoadGridDataHistoryMaterial(param);
                }
                //if (listAdo != null)
                //{
                //    dataTotal = listAdo.Count();
                //    var result = listAdo.Skip(startPage).Take(limit).ToList();
                //    rowCount = (result == null ? 0 : result.Count);
                //    gridControlFormList.DataSource = result;
                //}
                //else
                //{
                //    rowCount = 0;
                //    dataTotal = 0;
                //}
                //gridviewFormList.EndUpdate();
                //
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost((CommonParam)param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }


        public void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Search()
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTo.EditValue == null)
                    {
                        dtTo.Focus();
                        dtTo.ShowPopup();
                    }
                    else
                    {
                        cboSTT.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtFrom.EditValue == null)
                    {
                        dtFrom.Focus();
                        dtFrom.ShowPopup();
                    }
                    else
                    {
                        dtFrom.Focus();
                        dtFrom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_EXP_MEST_STT> _StatusSelecteds;
        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                _StatusSelecteds = new List<HIS_EXP_MEST_STT>();
                foreach (HIS_EXP_MEST_STT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        _StatusSelecteds.Add(rv);
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.EXP_MEST_STT_NAME.ToString());
                    }
                }
                this.cboSTT.Text = sb.ToString().Trim(' ', ',');

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_DEPARTMENT> _DepartmentSelecteds;
        private void SelectionGrid__Department(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                _DepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        _DepartmentSelecteds.Add(rv);
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.DEPARTMENT_NAME.ToString());
                    }
                }
                this.cboDepartment.Text = sb.ToString().Trim(' ', ',');

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_MEDI_STOCK> _ImpMediStockSelecteds;
        private void SelectionGrid__ImpMediStock(object sender, EventArgs e)
        {
            try
            {
                _ImpMediStockSelecteds = new List<V_HIS_MEDI_STOCK>();
                foreach (V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _ImpMediStockSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_MEDI_STOCK> _ExpMediStockSelecteds;
        private void SelectionGrid__ExpMediStock(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                _ExpMediStockSelecteds = new List<V_HIS_MEDI_STOCK>();
                foreach (V_HIS_MEDI_STOCK rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        _ExpMediStockSelecteds.Add(rv);
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.MEDI_STOCK_NAME.ToString());
                    }
                }
                this.cboExpMediStock.Text = sb.ToString().Trim(' ', ',');

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_EXP_MEST_TYPE> _ExpMestTypeSelecteds;
        private void SelectionGrid__ExpMestType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                _ExpMestTypeSelecteds = new List<HIS_EXP_MEST_TYPE>();
                foreach (HIS_EXP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        _ExpMestTypeSelecteds.Add(rv);
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.EXP_MEST_TYPE_NAME.ToString());
                    }
                }
                this.cboExpMestType.Text = sb.ToString().Trim(' ', ',');
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
                    if (cbo.Name == cboExpMediStock.Name)
                    {
                        var select = ((List<V_HIS_MEDI_STOCK>)data).Where(p => p.IS_ACTIVE == 1 && p.ROOM_ID == this._Module.RoomId).ToList();
                        gridCheckMark.SelectAll(select);
                    }
                    else
                    {
                        gridCheckMark.SelectAll(cbo.Properties.DataSource);
                    }
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

        private void cboSTT_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
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

                e.DisplayText = statusName.Trim(' ', ',');
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _DepartmentSelecteds)
                    {
                        statusName += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = statusName.Trim(' ', ',');
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboImpMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_ImpMediStockSelecteds != null && _ImpMediStockSelecteds.Count > 0)
                {
                    foreach (var item in _ImpMediStockSelecteds)
                    {
                        statusName += item.MEDI_STOCK_NAME + ", ";
                    }
                }

                e.DisplayText = statusName.Trim(' ', ',');
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_ExpMediStockSelecteds != null && _ExpMediStockSelecteds.Count > 0)
                {
                    foreach (var item in _ExpMediStockSelecteds)
                    {
                        statusName += item.MEDI_STOCK_NAME + ", ";
                    }
                }

                e.DisplayText = statusName.Trim(' ', ',');
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpMestType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_ExpMestTypeSelecteds != null && _ExpMestTypeSelecteds.Count > 0)
                {
                    foreach (var item in _ExpMestTypeSelecteds)
                    {
                        statusName += item.EXP_MEST_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = statusName.Trim(' ', ',');
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchClick(string keyword)
        {
            try
            {
                var columnView = gridControlFormList.FocusedView as ColumnView;
                if (columnView == null)
                    return;
                columnView.ApplyFindFilter(keyword);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSTT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //cboImpMediStock.Focus();
                    //cboImpMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboImpMediStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExpMediStock.Focus();
                    cboExpMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMediStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //cboImpMestType.Focus();
                    //cboImpMestType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboExpMestType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton_View_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var editor = sender as ButtonEdit;
                var gridControl = editor != null ? editor.Parent as GridControl : null;
                var columnView = gridControl != null ? gridControl.FocusedView as ColumnView : null;
                if (columnView == null)
                    return;
                var data = columnView.GetFocusedRow() as MediMateBaseADO;
                if (data == null)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = new Inventec.Desktop.Common.Modules.Module();
                V_HIS_EXP_MEST expMest = new V_HIS_EXP_MEST();

                moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestViewDetail")
                    .FirstOrDefault();

                if (moduleData == null)
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExpMestViewDetail");

                if (this.expMest != null && this.expMest.Count > 0)
                {
                    expMest = this.expMest.FirstOrDefault(p => p.ID == data.MEST_ID);
                }
                else
                {
                    expMest.ID = data.MEST_ID;
                    expMest.EXP_MEST_TYPE_ID = data.EXP_MEST_TYPE_ID;
                }

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    ImpMestViewDetailADO impMestADO = new ImpMestViewDetailADO(data.MEST_ID, data.IMP_MEST_TYPE_ID, data.STT_ID);

                    listArgs.Add(impMestADO);
                    listArgs.Add(expMest);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this._Module.RoomId, this._Module.RoomTypeId));

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                        PluginInstance.GetModuleWithWorkingRoom(moduleData, this._Module.RoomId, this._Module.RoomTypeId),
                        listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        public void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                //List<MedicineTypeADO> LsMedicineTypeADO = new List<MedicineTypeADO>();
                //List<string> expCode = new List<string>();

                //Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                //string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "LichSuXuatNhapThuoc.xlsx");

                ////chọn đường dẫn
                //saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                //if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    //getdata
                //    WaitingManager.Show();

                //    if (String.IsNullOrEmpty(templateFile))
                //    {
                //        store = null;
                //        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                //        return;
                //    }

                //    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                //    if (store.TemplatePath == "")
                //    {
                //        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                //        return;
                //    }

                //    if (gridControlFormList.DataSource != null)
                //        LsMedicineTypeADO = (List<MedicineTypeADO>)gridControlFormList.DataSource;
                //    foreach (var item in LsMedicineTypeADO)
                //    {
                //        if (item.IsExp)
                //        {
                //            if (expMest != null && expMest.Count > 0)
                //            {
                //                if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK)
                //                {
                //                    var mest = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                //                    if (mest != null)
                //                    {
                //                        var stock = medistocks.FirstOrDefault(p => p.ID == mest.IMP_MEDI_STOCK_ID);
                //                        item.MEDI_STOCK_NAME__STR = stock != null ? stock.MEDI_STOCK_NAME : "";
                //                    }
                //                    else
                //                        item.MEDI_STOCK_NAME__STR = "";
                //                }
                //                else if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK 
                //                    || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                //                    || item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                //                {
                //                    var expMestEdit = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                //                    item.MEDI_STOCK_NAME__STR = expMestEdit != null ? expMestEdit.TDL_TREATMENT_CODE + "-" + expMestEdit.TDL_PATIENT_NAME : "";
                //                }
                //                else if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP)
                //                {
                //                    var expMestEdit = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                //                    item.MEDI_STOCK_NAME__STR = expMestEdit != null ? expMestEdit.REQ_DEPARTMENT_NAME + "-" + expMestEdit.REQ_ROOM_NAME : "";
                //                }
                //                else if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN)
                //                {
                //                    var expMestEdit = expMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                //                    item.MEDI_STOCK_NAME__STR = expMestEdit != null ? expMestEdit.TDL_PATIENT_NAME : "";
                //                }
                //            }
                //        }
                //        else
                //        {
                //            if (impMest != null && impMest.Count > 0)
                //            {
                //                if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK)
                //                {
                //                    item.MEDI_STOCK_NAME__STR = dicChmsImpMest.ContainsKey(item.MEST_ID) ? dicChmsImpMest[item.MEST_ID] : "";
                //                }
                //                else if (item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL ||
                //                    item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL ||
                //                    item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL ||
                //                    item.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL)
                //                {
                //                    var moba = impMest.FirstOrDefault(o => o.ID == item.MEST_ID);
                //                    item.MEDI_STOCK_NAME__STR = moba != null ? moba.TDL_MOBA_EXP_MEST_CODE : "";
                //                }
                //            }
                //        }
                //    }

                //    ProcessData(LsMedicineTypeADO, ref store);
                //    WaitingManager.Hide();

                //    if (store != null)
                //    {
                //        try
                //        {
                //            if (store.OutFile(saveFileDialog1.FileName))
                //            {
                //                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                //                if (MessageBox.Show("Bạn có muốn mở file?",
                //                    "Thông báo", MessageBoxButtons.YesNo,
                //                    MessageBoxIcon.Question) == DialogResult.Yes)
                //                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            Inventec.Common.Logging.LogSystem.Warn(ex);
                //        }
                //    }
                //    else
                //    {
                //        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<MedicineTypeADO> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ExportResult", data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void txtPakageNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void chkHistoryMedicine_CheckedChanged(object sender, EventArgs e)
        {
            HistoryMode_CheckedChanged(sender, e);
        }

        private void chkHistoryMaterial_CheckedChanged(object sender, EventArgs e)
        {
            HistoryMode_CheckedChanged(sender, e);
        }

        private void HistoryMode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var modeToggle = sender as CheckEdit;
                if (modeToggle == null || !modeToggle.Checked)
                {
                    return;
                }
                this.SaveControlState();
                gridControlFormList.DataSource = null;
                bool isMedicineSelected = modeToggle == this.chkHistoryMedicine;
                gridControlFormList.MainView = isMedicineSelected ? (BaseView)gridViewHistoryMedicine : gridViewHistoryMaterial;
                if (ucPaging.pagingGrid != null)
                {
                    ucPaging.pagingGrid.FirstPage();
                }
                rowCount = 0;
                dataTotal = 0;
                Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchMediMate_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSearchMediMate_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Search();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }
    }
}
