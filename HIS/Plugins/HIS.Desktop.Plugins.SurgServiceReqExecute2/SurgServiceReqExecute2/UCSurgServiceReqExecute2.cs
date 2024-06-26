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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Security.Cryptography;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.EkipTemp;
using HIS.Desktop.ADO;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.Config;
using MOS.SDO;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.ThreadCustom;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : HIS.Desktop.Utility.UserControlBase
    {
        private long MethodIsRequired { get; set; }
        private long PriorityIsRequired { get; set; }
        private HisSurgServiceReqUpdateSDO currentHisSurgResultSDO { get; set; }
        private EpaymentDepositResultSDO epaymentDepositResultSDO { get; set; }
        private bool isAllowEditInfo { get; set; }
        private bool isStartTimeMustBeGreaterThanInstructionTime { get; set; }
        private int positionHandle { get; set; }
        private int lastRowHandle { get; set; }
        DevExpress.XtraGrid.Columns.GridColumn lastColumn { get; set; }
        DevExpress.Utils.ToolTipControlInfo lastInfo { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData { get; set; }
        List<V_HIS_SERVICE_ROOM> ServiceRoomViewList { get; set; }
        List<V_HIS_SERVICE> ServiceList { get; set; }
        List<V_HIS_SERE_SERV_1> SereServView1List { get; set; }
        List<HIS_PTTT_METHOD> PtttMethodList { get; set; }
        List<HIS_EMOTIONLESS_METHOD> EmotionLessMethodList { get; set; }
        List<HIS_PTTT_METHOD> PtttMethodRealList { get; set; }
        List<HIS_PTTT_GROUP> PtttGroupList { get; set; }
        List<HIS_EKIP_TEMP> EkipTempList { get; set; }
        List<HIS_EXECUTE_ROLE> ExecuteRoleList { get; set; }
        List<HIS_DEPARTMENT> DepartmentList { get; set; }
        List<V_HIS_ROOM> RoomViewList { get; set; }
        List<V_HIS_SERVICE> serviceSelecteds { get; set; }
        List<SereServView1ADO> lstGrid { get; set; }
        SereServView1ADO currentRow { get; set; }
        List<AcsUserADO> AcsUserADOList { get; set; }
        List<HIS_EXECUTE_ROLE_USER> executeRoleUsers { get; set; }
        public long? DepartmentId { get; private set; }
        List<HisEkipUserADO> hisEkipUserADOs { get; set; }
        V_HIS_SERVICE currentHisService {get;set;}
        public UCSurgServiceReqExecute2(Inventec.Desktop.Common.Modules.Module moduleData)
           : base(moduleData)
        {
            try
            {
                InitializeComponent();

                try
                {
                    this.moduleData = moduleData;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UCSurgServiceReqExecute2_Load(object sender, EventArgs e)
        {
            try
            {
                this.isAllowEditInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT") == "1";
                this.isStartTimeMustBeGreaterThanInstructionTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime") == "1" || HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime") == "2";
                //this.MethodIsRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.SurgServiceReqExecute.RequiredMethodOption");
                //this.PriorityIsRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.SurgServiceReqExecute.RequiredPriorityOption");
                dteFrom.DateTime = dteTo.DateTime = DateTime.Now;
                FillDataToGrid();
                CreateThreadLoadDataAll();
                AddDataToCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Save()
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Search()
        {
            try
            {
                btnSearch.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter) 
                    Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

    }
}
