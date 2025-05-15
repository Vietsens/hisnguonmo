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
using HIS.UC.ServiceRoom.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using System.Threading;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;

namespace HIS.UC.ServiceRoom
{
    public partial class UCRoomExamService : UserControl
    {

        public void DisposeControl()
        {
            try
            {
                tol7 = null;
                tol6 = null;
                tol5 = null;
                tol4 = null;
                tol3 = null;
                tol2 = null;
                tol1 = null;
                tol8 = null;
                tol9 = null;
                tol10 = null;
                tol11 = null;
                tol12 = null;
                col12 = null;
                col11 = null;
                col10 = null;
                col9 = null;
                col8 = null;
                col7 = null;
                col6 = null;
                col5 = null;
                col4 = null;
                col3 = null;
                col2 = null;
                col1 = null;
                col13 = null;
                col14 = null;
                col15 = null;
                col16 = null;
                col17 = null;
                isShow = false;
                isShowContainerMediMatyForChoose = false;
                isShowContainerTutorial = false;
                isShowContainerMediMaty = false;
                statecheckColumn = false;
                popupHeight = 0;
                serviceId = null;
                ucName = null;
                dicBlockByAppointment = null;
                PatientSDO = null;
                dicNumOrderBlock = null;
                currentServiceRooms = null;
                currentCulture = null;
                dicExecuteRoom = null;
                dicRoomService = null;
                dicPatientType = null;
                GetIntructionTime = null;
                changeServiceProcessPrimaryPatientType = null;
                changeRoomNotEmergency = null;
                registerPatientWithRightRouteBHYT = null;
                dlgFocusNextUserControl = null;
                dlgRemoveUC = null;
                hisRooms = null;
                sereServExam = null;
                userControlItemName = null;
                isFocusCombo = false;
                isInit = false;
                layoutExamServiceName = null;
                layoutRoomName = null;
                roomExts = null;
                roomExamServiceInitADO = null;
                hisServiceRooms = null;
                currentPatientTypes = null;
                currentPatientTypeAlter = null;
                numberNames = null;
                numOderSelected = null;
                this.beditRoom.Properties.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beditRoom_Properties_ButtonClick);
                this.beditRoom.TextChanged -= new System.EventHandler(this.beditRoom_TextChanged);
                this.beditRoom.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.beditRoom_KeyDown);
                this.popupControlContainerRoom.CloseUp -= new System.EventHandler(this.popupControlContainerRoom_CloseUp);
                this.gridControlContainerRoom.ProcessGridKey -= new System.Windows.Forms.KeyEventHandler(this.gridControlContainerRoom_ProcessGridKey);
                this.gridControlContainerRoom.Click -= new System.EventHandler(this.gridControlContainerRoom_Click);
                this.gridViewContainerRoom.RowStyle -= new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewContainerRoom_RowStyle);
                this.gridViewContainerRoom.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewContainerRoom_CustomRowCellEdit);
                this.gridViewContainerRoom.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewContainerRoom_CustomUnboundColumnData);
                this.gridViewContainerRoom.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.gridViewContainerRoom_KeyDown);
                this.gridViewContainerRoom.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.gridViewContainerRoom_MouseDown);
                this.repositoryItemBtnChooseHide.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnChooseHide_ButtonClick);
                this.repositoryItemBtnChoose.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnChooseHide_ButtonClick);
                this.txtRoomCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRoomCode_PreviewKeyDown);
                this.btnDelete.Click -= new System.EventHandler(this.btnDelete_Click);
                this.cboExamService.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExamService_Closed);
                this.cboExamService.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExamService_ButtonClick);
                this.cboExamService.EditValueChanged -= new System.EventHandler(this.cboExamService_EditValueChanged);
                this.cboExamService.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboExamService_KeyUp);
                this.txtExamServiceCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtExamServiceCode_KeyDown);
                this.txtExamServiceCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExamServiceCode_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCRoomExamService_Load);
                gridViewContainerRoom.GridControl.DataSource = null;
                gridControlContainerRoom.DataSource = null;
                gridLookUpEdit2View.GridControl.DataSource = null;
                repositoryItemBtnChoose = null;
                repositoryItemBtnChooseHide = null;
                imageCollection1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bar1 = null;
                barManager1 = null;
                lciCboRoom = null;
                beditRoom = null;
                lciRoom = null;
                gridViewContainerRoom = null;
                gridControlContainerRoom = null;
                popupControlContainerRoom = null;
                txtRoomCode = null;
                lciBtnDelete = null;
                layoutControlItem3 = null;
                lciExamService = null;
                txtExamServiceCode = null;
                gridLookUpEdit2View = null;
                cboExamService = null;
                btnDelete = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
