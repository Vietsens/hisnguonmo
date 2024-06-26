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
using AutoMapper;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicServices_NT.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicServices_NT
{
    public partial class frmPublicServices_NT : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {

                currentControlStateRDO = null;
                controlStateWorker = null;
                isNotLoadWhileChangeControlStateInFirst = false;
                dicServiceReq = null;
                dicExpMest = null;
                department = null;
                treatmentBedRoomList = null;
                isShowPatientList = false;
                congKhaiDichVu_DaySize = 0;
                patientTypeHasSelecteds = null;
                patientTypeSelectADOs = null;
                currentModule = null;
                _TreatmentBedRoom = null;
                _treatmentId = 0;
                _Datas = null;
                dayCountData = 0;
                _Mps000116ADOs = null;
                this.chkLayCaThuocVTNgoaiKho.CheckedChanged -= new System.EventHandler(this.chkLayCaThuocVTNgoaiKho_CheckedChanged);
                this.barButtonItemTao.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemTao_ItemClick);
                this.bbtnSearch.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSearch_ItemClick);
                this.bbtnNextPatient.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnNextPatient_ItemClick);
                this.txtFocusKeyword.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.txtFocusKeyword_ItemClick);
                this.rdoAllDay.CheckedChanged -= new System.EventHandler(this.rdoAllDay_CheckedChanged);
                this.btnNextPatient.Click -= new System.EventHandler(this.btnNextPatient_Click);
                this.gridViewPatient.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewPatient_RowCellClick);
                this.gridViewPatient.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewPatient_CustomUnboundColumnData);
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click);
                this.txtKeyword.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
                this.rdoRequestDepartment__All.CheckedChanged -= new System.EventHandler(this.rdoRequestDepartment__All_CheckedChanged);
                this.rdoRequestDepartment__Current.CheckedChanged -= new System.EventHandler(this.rdoRequestDepartment__Current_CheckedChanged);
                this.gridViewPatientType.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewPatientType_CellValueChanged);
                this.btnPublic.Click -= new System.EventHandler(this.btnPublicByDate_Click);
                this.Load -= new System.EventHandler(this.frmPublicServices_NT_Load);
                gridViewPatient.GridControl.DataSource = null;
                gridControlPatient.DataSource = null;
                repositoryItemGridLookUpEdit1View.GridControl.DataSource = null;
                repositoryItemGridLookUpEdit1.DataSource = null;
                gridViewPatientType.GridControl.DataSource = null;
                gridControlPatientType.DataSource = null;
                layoutControlItem7 = null;
                emptySpaceItem7 = null;
                chkLayCaThuocVTNgoaiKho = null;
                emptySpaceItem6 = null;
                emptySpaceItem5 = null;
                emptySpaceItem4 = null;
                emptySpaceItem2 = null;
                layoutControlItem13 = null;
                rdoFromToDay = null;
                layoutControlItem12 = null;
                rdoAllDay = null;
                txtFocusKeyword = null;
                bbtnNextPatient = null;
                bbtnSearch = null;
                gridColumn6 = null;
                gridColumn5 = null;
                gridColumn4 = null;
                gridColumn3 = null;
                layoutControlItem_btnNextPatient = null;
                btnNextPatient = null;
                layoutControlItem_GridControlPatient = null;
                gridViewPatient = null;
                gridControlPatient = null;
                layoutControlItem_btnSearch = null;
                layoutControlItem_txtKeyword = null;
                txtKeyword = null;
                btnSearch = null;
                layoutControlItem5 = null;
                Root = null;
                layoutControl2 = null;
                emptySpaceItem3 = null;
                layoutControlItem_GridPatient = null;
                layoutControlItem6 = null;
                rdoRequestDepartment__Current = null;
                rdoRequestDepartment__All = null;
                layoutControlItem4 = null;
                repositoryItemGridLookUpEdit1View = null;
                repositoryItemGridLookUpEdit1 = null;
                gridColumn2 = null;
                gridColumn1 = null;
                gridViewPatientType = null;
                gridControlPatientType = null;
                layoutControlItem3 = null;
                chkHaoPhi = null;
                layoutControlItem1 = null;
                dtTo = null;
                barButtonItemTao = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                lciDatePublic = null;
                dtFrom = null;
                btnPublic = null;
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
