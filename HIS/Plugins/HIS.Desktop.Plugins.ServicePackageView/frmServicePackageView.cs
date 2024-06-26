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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Data;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServicePackageView
{
    public partial class frmServicePackageView :  HIS.Desktop.Utility.FormBase
    {
        private List<V_HIS_SERE_SERV> sereServs;
        private List<V_HIS_SERE_SERV> sereServHightechs;
        internal long treatmentId;
        long roomId;
        Inventec.Desktop.Common.Modules.Module Module { get; set; }

        public frmServicePackageView()
        {
            InitializeComponent();
        }

        public frmServicePackageView(Inventec.Desktop.Common.Modules.Module _Module, long _treatmentId)
		:base(_Module)
        {
            InitializeComponent();
            SetIcon();
            this.treatmentId = _treatmentId;
            this.Module = _Module;
        }

        private void frmServicePackageView_Load(object sender, EventArgs e)
        {
            try
            {
                //Load các dịch vụ gói
                //var updateSereServ = ServicePackageView__Process.UpdateSereServ;

                loadServicePackages();
                SetCaptionByLanguageKey();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServicePackageView.Resources.Lang", typeof(HIS.Desktop.Plugins.ServicePackageView.frmServicePackageView).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmServicePackageView.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnServiceName.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnAmount.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnPrice.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnPriceTotal.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnPriceTotal.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnPatientType.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnCPNgoaiGoi.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnCPNgoaiGoi.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnExecuteRoom.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnExecuteRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnRequestRoom.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnRequestRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnRequestDepartment.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumnRequestDepartment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmServicePackageView.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmServicePackageView.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void treeListSereServDetail_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {

                var nodeTag = e.Node.Tag;
                var node = sender as TreeList;
                if (nodeTag == "KTC")
                {
                    e.Node.Expanded = true;
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);

                }

                if (nodeTag == "Department")
                {
                    e.Node.Expanded = true;
                }

                if (nodeTag == "ExecuteGroup")
                {
                    //e.Node.ExpandAll();
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListSereServDetail_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                if (e.Node.FirstNode == null && e.Node.Tag=="SERESERV")
                {
                    if (e.Column.FieldName == "CPNgoaiGoi")
                    {
                        ArrayList treeListTemp = (ArrayList)treeListSereServDetail.GetDataRecordByNode(e.Node);
                        V_HIS_SERE_SERV sereServ = (V_HIS_SERE_SERV)treeListTemp[treeListTemp.Count - 1];
                        if (sereServ.EXECUTE_ROOM_CODE == SdaConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_ROOM_TYPE__ROOM_TYPE_CODE__EXECUTE))
                            e.RepositoryItem = chkCPNgoaiGoi;
                        else
                            e.RepositoryItem = chkCPNgoaiGoi_Disable;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListSereServDetail_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.FieldName=="CPNgoaiGoi")
                {
                    
                }
            }
            catch (Exception ex)
            {
                 Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListSereServDetail_GetNodeDisplayValue(object sender, GetNodeDisplayValueEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListSereServDetail_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCPNgoaiGoi_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCPNgoaiGoi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                CheckEdit checkEdit = sender as CheckEdit;
                CommonParam param = new CommonParam();
                ArrayList sereServTemp = (ArrayList)treeListSereServDetail.GetDataRecordByNode(treeListSereServDetail.FocusedNode);
                V_HIS_SERE_SERV sereServV = (V_HIS_SERE_SERV)sereServTemp[sereServTemp.Count - 1];
                //Get SereServ
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.ID = sereServV.ID;
                sereServV.ID = roomId;
                //HIS_SERE_SERV sereServ = new HisSereServLogic().GetView<List<HIS_SERE_SERV>>(sereServFilter).FirstOrDefault();
                HIS_SERE_SERV sereServ = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, sereServFilter, param).FirstOrDefault();
                if (sereServ != null)
                {
                    if (checkEdit.Checked)
                        sereServ.IS_OUT_PARENT_FEE = IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_OUT_PARENT_FEE__TRUE;
                    else sereServ.IS_OUT_PARENT_FEE = null;

                    bool update = ServicePackageView__Process.UpdateSereServ(sereServ);
                    if (update == false)
                    {
                        if (checkEdit.Checked)
                            checkEdit.CheckState = CheckState.Unchecked;
                        else
                            checkEdit.CheckState = CheckState.Checked;
                    }
                }
                success = true;
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex )
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
    }
}
