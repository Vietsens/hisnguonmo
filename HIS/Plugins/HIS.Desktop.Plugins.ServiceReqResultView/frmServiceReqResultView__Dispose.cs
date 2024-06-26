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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqResultView
{
    public partial class frmServiceReqResultView : HIS.Desktop.Utility.FormBase
    {
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                isLoadingForm = false;
                taskForm_Load = null;
                isContineuCheckbox = false;
                currentControlStateRDO = null;
                controlStateWorker = null;
                isSense = false;
                currentBussinessCode = null;
                patient = null;
                TreatmentWithPatientTypeAlter = null;
                UserName = null;
                keyPrint = null;
                dicImage = null;
                dicParam = null;
                currentServiceReq = null;
                sereServExt = null;
                sereServ = null;
                currentModule = null;
                sereServId = null;
                documentData = null;
                listEmrDocument = null;
                isShowEmrDocument = false;
                this.btnOpenWeb.Click -= new System.EventHandler(this.btnOpenWeb_Click);
                this.chkAutoOpenWeb.CheckedChanged -= new System.EventHandler(this.chkAutoOpenWeb_CheckedChanged);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Print_ItemClick);
                this.BtnEmr.Click -= new System.EventHandler(this.BtnEmr_Click);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.Load -= new System.EventHandler(this.frmServiceReqResultView_Load);
                layoutControlItem14 = null;
                lblEndTime = null;
                layoutControlItem13 = null;
                lblStartTime = null;
                layoutControlItem12 = null;
                pdfViewer1 = null;
                layoutControlItem11 = null;
                layoutControlGroup_TabPdf = null;
                layoutControl_TabPdf = null;
                xtraTabPage_TabPdf = null;
                layoutControlItem8 = null;
                layoutControlGroup_TabDocument = null;
                txtDescription = null;
                layoutControl_TabDocument = null;
                xtraTabPage_TabDocument = null;
                xtraTabControl_TabHIS = null;
                layoutControlItem10 = null;
                layoutControlItem9 = null;
                chkAutoOpenWeb = null;
                btnOpenWeb = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlGroup3 = null;
                layoutControl3 = null;
                layoutControlItem7 = null;
                layoutControlItem6 = null;
                layoutControlGroup2 = null;
                layoutControl2 = null;
                layoutControlItem1 = null;
                xtraTabPacs = null;
                xtraTabHis = null;
                xtraTab = null;
                webView1 = null;
                webControl1 = null;
                txtUrl = null;
                layoutControlItem5 = null;
                BtnEmr = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                lciPrint = null;
                txtConclude = null;
                txtNote = null;
                btnPrint = null;
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
