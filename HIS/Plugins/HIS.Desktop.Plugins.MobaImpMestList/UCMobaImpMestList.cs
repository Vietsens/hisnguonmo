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
using HIS.Desktop.Plugins.MobaImpMestList.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.MobaImpMestList
{
    public partial class UCMobaImpMestList : UserControl
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        long impMestTypeId = 0;
        MobaImpMestListADO mobaImpMestListADO = null;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        private string LoggingName = "";
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        public MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 ImpMest, ImpMestEdit;
        internal MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 ViewImportMest;
        public Base.HisCommonImpMestTypeInfo hisCommonImpMestTypeInfo;
        internal V_HIS_IMP_MEST_2 currentImpMestRightClick { get; set; }
        RightMouseClickProcessor rightMouseClickProcessor;

        List<V_HIS_BID> listBid;
        List<HIS_IMP_MEST_STT> _StatusSelecteds;
        List<HIS_IMP_MEST_TYPE> _TypeSelecteds;
        ToolTip toolTip = new ToolTip();
        V_HIS_ROOM room;

        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;

        Inventec.Desktop.Common.Modules.Module currentModule;
        //HIS_TREATMENT treatment = null;
        #endregion

        #region Construct
        public UCMobaImpMestList()
        {
            InitializeComponent();
            try
            {
                //FillDataNavStatus();
                //FillDataNavType();
                gridControlImportMestList.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCMobaImpMestList(Inventec.Desktop.Common.Modules.Module _module)
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

        public UCMobaImpMestList(Inventec.Desktop.Common.Modules.Module _module, long impMestTypeId, MobaImpMestListADO mobaImpMestListADO)
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
                this.mobaImpMestListADO = mobaImpMestListADO;
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

                if (medistock != null)
                {
                    checkEditMediStock.Checked = true;
                    checkEditMediStock.Enabled = true;
                }
                else
                {
                    checkEditMediStock.Checked = false;
                    checkEditMediStock.Enabled = false;
                }
                //Gan ngon ngu
                LoadKeysFromlanguage();
                //Load Combo
                InitCheck(cboStatus, SelectionGrid__Status);
                InitCombo(cboStatus, BackendDataWorker.Get<HIS_IMP_MEST_STT>(), "IMP_MEST_STT_NAME", "ID");

                InitCheck(cboType, SelectionGrid__Type);

                if (medistock != null && medistock.IS_ODD == 1)
                {
                    InitCombo(cboType, BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o =>
                        o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL
                        ).ToList(), "IMP_MEST_TYPE_NAME", "ID");
                }
                else
                {
                    InitCombo(cboType, BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o =>
                        o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL
                        ).ToList(), "IMP_MEST_TYPE_NAME", "ID");
                }

                if (impMestTypeId > 0)
                {
                    cboType.Enabled = false;
                }
                else
                {
                    cboType.Enabled = true;
                }

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                if (this.mobaImpMestListADO != null)
                {
                    txtExpMestCode.Text = this.mobaImpMestListADO.ExpMestCode;
                    txtTreatmentCode.Text = this.mobaImpMestListADO.TreatmentCode;
                    dtCreateTimeFrom.EditValue = null;
                    dtCreateTimeTo.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        var data = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o =>
                            o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH
                           || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL).ToList();
                        gridCheckMark.SelectAll(data);
                    }
                }

                InitComboPayStt();

                //if (mobaImpMestListADO != null)
                //{
                //    txtKeyWord.Text = mobaImpMestListADO.TreatmentCode;                    
                //}
                //Load du lieu
                FillDataImportMestList();
                //Ản hiện layout search
                layoutControlGroupType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                txtImpCode.Focus();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MobaImpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.MobaImpMestList.UCMobaImpMestList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxInfo.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.groupBoxInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem41.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem46.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExport.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.btnExport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEditMediStock.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.checkEditMediStock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxSoChungTu.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.groupBoxSoChungTu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEditNoDocumentNumber.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.checkEditNoDocumentNumber.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEditHasDocumentNumber.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.checkEditHasDocumentNumber.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxNgayChungTu.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.groupBoxNgayChungTu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtImpCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.txtImpCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCImpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCImpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCImpMestTypeName.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCImpMestTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCReqLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCReqLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCApprovalLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCApprovalLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCImpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCImpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.EVENT_LOG_TYPE_ID.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.EVENT_LOG_TYPE_ID.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCReqDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCReqDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gCModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisImportMestMedicine.gCModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void FillDataNavStatus()
        //{
        //    try
        //    {
        //        navBarControlFilter.BeginUpdate();
        //        int d = 0;
        //        foreach (var item in Base.GlobalStore.HisImpMestStts)
        //        {
        //            navBarGroupStatus.GroupClientHeight += 25;
        //            DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
        //            layoutControlContainerStatus.Controls.Add(checkEdit);
        //            checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
        //            checkEdit.Name = item.ID.ToString();
        //            checkEdit.Properties.Caption = item.IMP_MEST_STT_NAME;
        //            checkEdit.Size = new System.Drawing.Size(150, 19);
        //            checkEdit.StyleController = this.layoutControlContainerStatus;
        //            checkEdit.TabIndex = 4 + d;
        //            checkEdit.EnterMoveNextControl = false;
        //            d++;
        //        }
        //        navBarControlFilter.EndUpdate();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void FillDataNavType()
        //{
        //    try
        //    {
        //        navBarControlFilter.BeginUpdate();
        //        int d = 0;
        //        foreach (var item in Base.GlobalStore.HisImpMestTypes)
        //        {
        //            navBarGroupType.GroupClientHeight += 25;
        //            DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
        //            layoutControlContainerType.Controls.Add(checkEdit);
        //            checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
        //            checkEdit.Name = item.ID.ToString();
        //            checkEdit.Properties.Caption = item.IMP_MEST_TYPE_NAME;
        //            checkEdit.Size = new System.Drawing.Size(150, 19);
        //            checkEdit.StyleController = this.layoutControlContainerType;
        //            checkEdit.TabIndex = 4 + d;
        //            checkEdit.EnterMoveNextControl = false;
        //            d++;
        //        }
        //        navBarControlFilter.EndUpdate();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void SetDefaultValueControl()
        {
            try
            {
                cboStatus.Enabled = false;
                cboType.Enabled = false;
                cboStatus.Enabled = true;
                cboType.Enabled = true;

                txtKeyWord.Text = "";
                txtImpCode.Text = "";
                dtDocumentDateFrom.EditValue = null;
                dtDocumentDateTo.EditValue = null;
                checkEditHasDocumentNumber.Checked = false;
                checkEditNoDocumentNumber.Checked = false;
                dtImpTimeFrom.EditValue = null;
                dtImpTimeTo.EditValue = null;
                txtMedicineType.Text = "";
                txtDocumentNumber.Text = "";
                if (this.mobaImpMestListADO != null)
                {
                    txtExpMestCode.Text = this.mobaImpMestListADO.ExpMestCode;
                    txtTreatmentCode.Text = this.mobaImpMestListADO.TreatmentCode;
                    dtCreateTimeFrom.EditValue = null;
                    dtCreateTimeTo.EditValue = null;
                }
                else
                {
                    txtExpMestCode.Text = "";
                    txtTreatmentCode.Text = "";
                    DateTime? TimeNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                    dtCreateTimeFrom.EditValue = TimeNow;
                    dtCreateTimeTo.EditValue = TimeNow;
                }

                //dtImpTimeFrom.EditValue = null;
                //dtImpTimeTo.EditValue = null;
                txtImpCode.Focus();
                //SetDefaultStatus();
                //SetDefaultType();

                long showbtn = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(Base.GlobalStore.showButton));

                if (showbtn == 1)
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciBtnExport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                layoutControlGroupType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

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
        //private void SetDefaultType()
        //{
        //    try
        //    {
        //        if (layoutControlContainerType.Controls.Count > 0)
        //        {
        //            for (int i = 0; i < layoutControlContainerType.Controls.Count; i++)
        //            {
        //                if (layoutControlContainerType.Controls[i] is DevExpress.XtraEditors.CheckEdit)
        //                {
        //                    var checkEdit = layoutControlContainerType.Controls[i] as DevExpress.XtraEditors.CheckEdit;
        //                    checkEdit.Checked = false;
        //                }
        //            }
        //        }
        //        navBarGroupType.Expanded = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void SetDefaultStatus()
        //{
        //    try
        //    {
        //        if (layoutControlContainerStatus.Controls.Count > 0)
        //        {
        //            for (int i = 0; i < layoutControlContainerStatus.Controls.Count; i++)
        //            {
        //                if (layoutControlContainerStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
        //                {
        //                    var checkEdit = layoutControlContainerStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
        //                    checkEdit.Checked = false;
        //                }
        //            }
        //        }
        //        navBarGroupStatus.Expanded = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

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

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _StatusSelecteds = new List<HIS_IMP_MEST_STT>();
                foreach (HIS_IMP_MEST_STT rv in (sender as GridCheckMarksSelection).Selection)
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
                _TypeSelecteds = new List<HIS_IMP_MEST_TYPE>();
                foreach (HIS_IMP_MEST_TYPE rv in (sender as GridCheckMarksSelection).Selection)
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

        private void FillDataImportMestList()
        {
            try
            {
                if (layoutControlGroupType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && !string.IsNullOrEmpty(txtMedicineType.Text))
                {
                    FillDataImportMestDetailList();
                }
                else
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
                    ImportMestPaging(new CommonParam(0, numPageSize));
                    CommonParam param = new CommonParam();
                    param.Limit = rowCount;
                    param.Count = dataTotal;
                    ucPaging.Init(ImportMestPaging, param, numPageSize, this.gridControlImportMestList);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void ImportMestPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>> apiResult = null;
                MOS.Filter.HisImpMestView2Filter filter = new MOS.Filter.HisImpMestView2Filter();
                SetFilterImpMest(ref filter);
                //if (mobaImpMestListADO != null && !string.IsNullOrEmpty(mobaImpMestListADO.ExpMestCode))
                //    filter.TDL_MOBA_EXP_MEST_CODE__EXACT = mobaImpMestListADO.ExpMestCode;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                gridViewImportMestList.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Info("Filter HisImpMest/GetView2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>>
                    ("api/HisImpMest/GetView2", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlImportMestList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlImportMestList.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewImportMestList.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void SetFilterImpMest(ref MOS.Filter.HisImpMestView2Filter filter)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtImpCode.Text))
                {
                    string code = txtImpCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtImpCode.Text = code;
                    }
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.IMP_MEST_CODE__EXACT = code;
                    filter.IMP_MEST_TYPE_IDs = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL).Select(o => o.ID).ToList();
                }
                else if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                    filter.TDL_MOBA_EXP_MEST_CODE__EXACT = code;
                    filter.IMP_MEST_TYPE_IDs = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL).Select(o => o.ID).ToList();
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                        filter.IMP_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                        filter.IMP_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeTo.EditValue).ToString("yyyyMMdd") + "000000");
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
                    filter.IMP_MEST_TYPE_IDs = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL
                        || o.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL).Select(o => o.ID).ToList();
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                        filter.IMP_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                        filter.IMP_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeTo.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();

                    if (checkEditMediStock.Checked && this.medistock != null)
                    {
                        filter.MEDI_STOCK_ID = this.medistock.ID;
                    }
                    else if (!checkEditMediStock.Checked && this.medistock != null)
                    {
                        filter.DATA_DOMAIN_FILTER = true;
                        filter.WORKING_ROOM_ID = roomId;
                    }
                    else if (this.medistock == null && this.room != null)
                    {
                        filter.REQ_DEPARTMENT_ID = this.room.DEPARTMENT_ID;
                    }

                    //if (medistock != null)
                    //{
                    //    filter.IS_ODD_OR_NOT_DNTTL = true;
                    //}
                    //filter.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL;          
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                        filter.IMP_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                        filter.IMP_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeTo.EditValue).ToString("yyyyMMdd") + "000000");

                    SetFilterStatus(ref filter);

                    //if (impMestTypeId == 0)
                    //{
                        SetFilterType(ref filter);
                    //}
                    //else
                    //{
                    //    filter.IMP_MEST_TYPE_IDs = new List<long>();
                    //    filter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH);
                    //    filter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL);
                    //    filter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL);
                    //    filter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL);
                    //    filter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL);
                    //    filter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BTL);
                    //    filter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL);
                    //}
                    SetFilterDocumentDate(ref filter);

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

        private void SetFilterDocumentDate(ref MOS.Filter.HisImpMestView2Filter filter)
        {
            try
            {
                if (layoutControlGroupType.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (dtDocumentDateFrom.EditValue != null && dtDocumentDateFrom.DateTime != DateTime.MinValue)
                        filter.DOCUMENT_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtDocumentDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtDocumentDateTo.EditValue != null && dtDocumentDateTo.DateTime != DateTime.MinValue)
                        filter.DOCUMENT_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtDocumentDateTo.EditValue).ToString("yyyyMMdd") + "000000");

                    if (checkEditHasDocumentNumber.Checked)
                        filter.HAS_DOCUMENT_NUMBER = true;
                    if (checkEditNoDocumentNumber.Checked)
                        filter.HAS_DOCUMENT_NUMBER = false;

                    if (!string.IsNullOrEmpty(txtDocumentNumber.Text))
                    {
                        filter.DOCUMENT_NUMBER__EXACT = txtDocumentNumber.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetFilterStatus(ref MOS.Filter.HisImpMestView2Filter filter)
        {
            try
            {
                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    filter.IMP_MEST_STT_IDs = _StatusSelecteds.Select(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterType(ref MOS.Filter.HisImpMestView2Filter filter)
        {
            try
            {
                if (_TypeSelecteds != null && _TypeSelecteds.Count > 0)
                {
                    filter.IMP_MEST_TYPE_IDs = _TypeSelecteds.Select(o => o.ID).ToList();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataImportMestList();
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
                ResetCombo(cboStatus);
                ResetCombo(cboType);
                SetDefaultValueControl();
                FillDataImportMestList();
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
                    if (!String.IsNullOrEmpty(txtImpCode.Text))
                    {
                        FillDataImportMestList();
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

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeFrom.EditValue != null)
                    {
                        dtCreateTimeTo.Focus();
                        dtCreateTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeTo.EditValue != null)
                    {
                        cboStatus.Focus();
                        cboStatus.ShowPopup();
                    }
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
                if (e.Info == null && e.SelectedControl == gridControlImportMestList)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlImportMestList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
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
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "PAY_DISPLAY")
                            {
                                var documentPrice = (view.GetRowCellValue(lastRowHandle, "DOCUMENT_PRICE") ?? "").ToString();
                                var sumPay = (view.GetRowCellValue(lastRowHandle, "SUM_PAY") ?? "").ToString();
                                if (!String.IsNullOrWhiteSpace(documentPrice))
                                {
                                    if (!String.IsNullOrWhiteSpace(sumPay))
                                    {
                                        if (Inventec.Common.TypeConvert.Parse.ToDecimal(sumPay) == Inventec.Common.TypeConvert.Parse.ToDecimal(documentPrice))
                                        {
                                            text = "Đã thanh toán";//xanh lam đã thanh toán
                                        }
                                        else
                                        {
                                            text = "Đang thanh toán";//xanh lục đang tt
                                        }
                                    }
                                    else
                                    {
                                        text = "Chưa thanh toán";//đen chưa tt
                                    }
                                }
                                else
                                {
                                    text = "Chưa thanh toán";//đen chưa tt
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

        private void gridViewImportMestList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 data = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "IMP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT) //tam thoi
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)//yêu cầu
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT)//// tu choi
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL) // duyet
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT) // da nhap
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        {
                            string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                            string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                            e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "IMP_LOGINNAME_DISPLAY")
                        {
                            string IMP_LOGINNAME = data.IMP_LOGINNAME;
                            string IMP_USERNAME = data.IMP_USERNAME;
                            e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                        //else if (e.Column.FieldName == "PAY_DISPLAY")
                        //{
                        //    if (data.DOCUMENT_PRICE.HasValue)
                        //    {
                        //        if (data.SUM_PAY.HasValue)
                        //        {
                        //            if (data.SUM_PAY == data.DOCUMENT_PRICE)
                        //            {
                        //                e.Value = imageListStatus.Images[6];//xanh lam đã thanh toán
                        //            }
                        //            else
                        //            {
                        //                e.Value = imageListStatus.Images[3];//xanh lục đang tt
                        //            }
                        //        }
                        //        else
                        //        {
                        //            e.Value = imageListStatus.Images[2];//đen chưa tt
                        //        }
                        //    }
                        //    else
                        //    {
                        //        e.Value = imageListStatus.Images[2];//đen chưa tt
                        //    }
                        //}
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
                    var data = (V_HIS_IMP_MEST_2)gridViewImportMestList.GetRow(e.RowHandle);
                    long statusIdCheckForButtonEdit = long.Parse((gridViewImportMestList.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                    long typeIdCheckForButtonEdit = long.Parse((gridViewImportMestList.GetRowCellValue(e.RowHandle, "IMP_MEST_TYPE_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewImportMestList.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    string creator = (gridViewImportMestList.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (e.Column.FieldName == "APPROVAL_DISPLAY")//duyệt
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnApprove) != null)
                            {
                                if (medistock != null && medistock.ID == mediStockId &&
                                    (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST))
                                {
                                    if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                                    {
                                        e.RepositoryItem = repositoryItemButtonApprovalDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonApprovalEnable;
                                    }
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
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT) && typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL
                                && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
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
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName)) &&
                                (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT))
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
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC && statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
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
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                               statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL
                               && impMestTypeId != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                {
                                    if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnImport) != null)
                                    {
                                        e.RepositoryItem = repositoryItemButtonActualImportDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonActualImportEnable;
                                    }
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonActualImportEnable;
                                }

                            }
                            else if (
                                statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT
                                && impMestTypeId != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (medistock != null && medistock.ID == mediStockId)
                                {
                                    if ((data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL) && !CheckLoginAdmin.IsAdmin(LoggingName))
                                    {
                                        e.RepositoryItem = Btn_Cancel_Import_Disable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_Cancel_Import_Enable;
                                    }
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
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if ((creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                                 && (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                                 &&
                                (typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH &&
                                typeIdCheckForButtonEdit != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT))
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
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Không duyệt duyệt
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST || statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT) && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT)
                                {
                                    if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDisApprovalEnable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = Btn_HuyTuChoiDuyet_Enable;
                                    }
                                }
                                else
                                {
                                    if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                    {
                                        e.RepositoryItem = repositoryItemButtonDisApprovalDisable;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonDisApprovalEnable;
                                    }
                                }
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonDisApprovalDisable;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonDisApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_DISPLAY")// Hủy duyệt
                    {
                        if (data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                        {
                            if (medistock != null && medistock.ID == mediStockId &&
                                (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL) && data.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCT)
                            {
                                if (data != null && data.AGGR_IMP_MEST_ID != null && data.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL)
                                {
                                    e.RepositoryItem = repositoryItemButtonRequestDisable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonRequest;
                                }
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
                    //else if (e.Column.FieldName == "DONE")// Hủy duyệt
                    //{
                    //    if (medistock != null && medistock.ID == mediStockId &&
                    //        (statusIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)&&
                    //       (typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK||
                    //        typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS ||
                    //        typeIdCheckForButtonEdit == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HCS
                    //        ))
                    //    {
                    //        e.RepositoryItem = Btn_Done_Enable;
                    //    }
                    //    else
                    //    {
                    //        e.RepositoryItem = repositoryItemButtonRequestDisable;
                    //    }
                    //}
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
                txtImpCode.Focus();
                txtImpCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region report
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExport.Enabled) return;
                if (lciBtnExport.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;

                //if (dtImpTimeFrom.EditValue == null || dtImpTimeTo.EditValue == null)
                //{
                //    MessageBox.Show(Resources.ResourceMessage.BanChuaChonThoiGianThucXuat);
                //    if (dtImpTimeFrom.EditValue == null)
                //    {
                //        dtImpTimeFrom.Focus();
                //        dtImpTimeFrom.SelectAll();
                //    }
                //    else if (dtImpTimeTo.EditValue == null)
                //    {
                //        dtImpTimeTo.Focus();
                //        dtImpTimeTo.SelectAll();
                //    }
                //    return;
                //}

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
                MOS.Filter.HisImpMestView2Filter impFilter = new MOS.Filter.HisImpMestView2Filter();

                impFilter.IMP_MEST_STT_IDs = new List<long>();
                impFilter.IMP_MEST_STT_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT);

                impFilter.IMP_MEST_TYPE_IDs = new List<long>();
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BCS);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DQH);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL);
                //impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HCS);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HPTL);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__TH);
                impFilter.IMP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__THT);

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

                var exportList = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>>(ApiConsumer.HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (exportList != null && exportList.Count > 0)
                {
                    expCode = exportList.Select(s => s.IMP_MEST_CODE).OrderBy(o => o).ToList();
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
                        statusName += item.IMP_MEST_STT_NAME + ", ";
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
                        typeName += item.IMP_MEST_TYPE_NAME + ", ";
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
                if (cboType.Enabled)
                {
                    cboType.Focus();
                    cboType.ShowPopup();
                }
                else
                    btnSearch.Focus();
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
                if (_TypeSelecteds != null && _TypeSelecteds.Count == 1 && _TypeSelecteds.FirstOrDefault().ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    layoutControlGroupType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    //dtDocumentDateFrom.Focus();
                    //dtDocumentDateFrom.ShowPopup();
                }
                else
                {
                    layoutControlGroupType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    btnSearch.Focus();
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
                var rowFocus = (V_HIS_IMP_MEST_2)gridViewImportMestList.GetFocusedRow();
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

        private void LoadInfoClick(V_HIS_IMP_MEST_2 data)
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

                lblChietKhau.Text = data.DISCOUNT_RATIO != null ? (data.DISCOUNT_RATIO * 100).ToString() : "";
                lblDiaChi.Text = data.TDL_PATIENT_ADDRESS;
                lblGioiTinh.Text = data.TDL_PATIENT_GENDER_NAME;
                lblMaDieuTri.Text = data.TDL_TREATMENT_CODE;

                lblNgaySinh.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString((data.TDL_PATIENT_DOB ?? 0)); ;
                lblTenBenhNhan.Text = data.TDL_PATIENT_NAME;

                toolTip.RemoveAll();
                if (!string.IsNullOrEmpty(lblChietKhau.Text))
                    toolTip.SetToolTip(lblChietKhau, lblChietKhau.Text);
                if (!string.IsNullOrEmpty(lblDiaChi.Text))
                    toolTip.SetToolTip(lblDiaChi, lblDiaChi.Text);
                if (!string.IsNullOrEmpty(lblGioiTinh.Text))
                    toolTip.SetToolTip(lblGioiTinh, lblGioiTinh.Text);
                if (!string.IsNullOrEmpty(lblMaDieuTri.Text))
                    toolTip.SetToolTip(lblMaDieuTri, lblMaDieuTri.Text);
                if (!string.IsNullOrEmpty(lblNgaySinh.Text))
                    toolTip.SetToolTip(lblNgaySinh, lblNgaySinh.Text);
                if (!string.IsNullOrEmpty(lblTenBenhNhan.Text))
                    toolTip.SetToolTip(lblTenBenhNhan, lblTenBenhNhan.Text);
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
                ViewImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewImportMestList.GetFocusedRow();
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

        private void ManuExpMestCreateClick(V_HIS_IMP_MEST_2 impMest)
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

        private void EditInfoImportNCC(V_HIS_IMP_MEST_2 impMest)
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
                //    this.currentImpMestRightClick = (V_HIS_IMP_MEST_2)gridViewImportMestList.GetRow(rowHandle);
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
                var impMest = (V_HIS_IMP_MEST_2)gridViewImportMestList.GetFocusedRow();
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
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewImportMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/CancelImport", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataImportMestList();
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
                var impMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewImportMestList.GetFocusedRow();
                if (impMest != null)
                {
                    //hien thi popup chi tiet
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "IMP_MEST_CODE: " + impMest.IMP_MEST_CODE);
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
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 VImportMest = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewImportMestList.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
                Inventec.Common.Mapper.DataObjectMapper.Map
                    <MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    (EVImportMest, VImportMest);

                EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
                    ("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresul != null)
                {
                    success = true;
                    FillDataImportMestList();
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
                        FillDataImportMestList();
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
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        FillDataImportMestList();
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

        private void txtDocumentNumber_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtDocumentNumber.Text))
                    {
                        FillDataImportMestList();
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

        private void txtMedicineType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    FillDataImportMestDetailList();
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
                ucPaging.Init(ImportMestPagingDetail, param, numPageSize, this.gridControlImportMestList);
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
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>> apiResult = null;
                MOS.Filter.HisImpMestViewDetailFilter filter = new MOS.Filter.HisImpMestViewDetailFilter();
                SetFilterImpMestDetail(ref filter);

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                gridViewImportMestList.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2>>
                    ("api/HisImpMest/GetViewByDetail", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlImportMestList.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlImportMestList.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewImportMestList.EndUpdate();

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
                if (!String.IsNullOrEmpty(txtMedicineType.Text))
                {
                    string code = txtMedicineType.Text.Trim();
                    //if (code.Length < 12 && checkDigit(code))
                    //{
                    //    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    //    txtMedicineType.Text = code;
                    //}
                    filter.MEDICINE_TYPE_CODE__EXACT = code;
                }
                filter.DOCUMENT_NUMBER__EXACT = txtDocumentNumber.Text.Trim();

                filter.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC;

                if (checkEditMediStock.Checked && this.medistock != null)
                {
                    filter.MEDI_STOCK_ID = this.medistock.ID;
                }
                else if (this.medistock == null && this.room != null)
                {
                    filter.REQ_DEPARTMENT_ID = this.room.DEPARTMENT_ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImportMestList_RowStyle(object sender, RowStyleEventArgs e)
        {
        }

    }
}
