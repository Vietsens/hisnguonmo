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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using DevExpress.Data;
using HIS.Desktop.ADO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.TreatmentLog.Resources;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.TreatmentLog.Config;
using HIS.Desktop.Plugins.TreatmentLog.Popup.CoTreatment;

namespace HIS.Desktop.Plugins.TreatmentLog
{
    public partial class UCTreatmentProcessPartial : HIS.Desktop.Utility.UserControlBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                currentModule = null;
                currentTreatmentId = 0;
                currentRoomId = 0;
                logTypeId__DepartmentTran = 0;
                logTypeId__patientTypeAlter = 0;
                currentRoom = null;
                currentTreatment = null;
                resultDepartmentTran = null;
                resultPatientTypeAlter = null;
                resultCoTreatment = null;
                apiResult = null;
                loginName = null;
                controlAcs = null;
                diction = null;
                this.treeListTreatmentLog.CustomNodeCellEdit -= new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.treeListTreatmentLog_CustomNodeCellEdit);
                this.treeListTreatmentLog.CustomUnboundColumnData -= new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeListTreatmentLog_CustomUnboundColumnData);
                this.Btn_Edit_Enable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnEdit_ButtonClick);
                this.Btn_Delete_Enable.Click -= new System.EventHandler(this.btnDelete_Click);
                this.btnDepartmentTran.Click -= new System.EventHandler(this.btnDepartmentTran_Click);
                this.btnPatientTypeAlter.Click -= new System.EventHandler(this.btnPatientTypeAlter_Click);
                this.Load -= new System.EventHandler(this.UCTreatmentProcessPartial_Load);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
