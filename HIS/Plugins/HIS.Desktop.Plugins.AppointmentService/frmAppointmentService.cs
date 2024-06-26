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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AppointmentService.Base;
using HIS.Desktop.Plugins.AppointmentService.Config;
using HIS.Desktop.Plugins.AppointmentService.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AppointmentService
{
    public partial class frmAppointmentService : FormBase
    {
        private long treatmentId { get; set; }
        Action<List<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_SERV>> actSelect;
        List<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_SERV> appointmentServiceInput;
        private const string commonString__true = "1";
        private enum SERVICE_ENUM
        {
            ALL,
            PARENT,
            PARENT_FOR_GRID_SERVCIE
        }

        private Dictionary<SERVICE_ENUM, List<ServiceADO>> dicService = new Dictionary<SERVICE_ENUM, List<ServiceADO>>();
        private List<SereServADO> ServiceIsleafADOs;
        private BindingList<ServiceADO> records;
        private HideCheckBoxHelper hideCheckBoxHelper__Service;
        private V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        private HIS_TREATMENT treatment;
        private HIS_BRANCH branch;
        private long isSingleCheckService { get; set; }
        private int groupType__ServiceTypeName = 1;
        private int groupType__PtttGroupName = 2;
        private bool IscheckAllTreeService = false;
        private long instructionTime;
        private ACTION action;
        List<SereServADO> listDatasFix { get; set; }
        List<HIS_PATIENT_TYPE> listPatientType = new List<HIS_PATIENT_TYPE>();
        List<long> patientTypeIdAls;
        int notSearch = 0;

        bool isHandlerResetServiceStateCheckedForTreeNodes;
        bool isHandlerResetServiceStateCheckeds;
        private bool isProcessingAfternodeChecked;

        public frmAppointmentService(Inventec.Desktop.Common.Modules.Module _module, long treatmentId)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmAppointmentService(Inventec.Desktop.Common.Modules.Module _module, AppointmentServiceADO appointmentServiceADO)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = appointmentServiceADO.TreatmentId;
                this.actSelect = appointmentServiceADO.ActSelect;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmAppointmentService_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Du lieu ben ngoai truyen vào treatmentId: " + this.treatmentId + "____actSelect:" + (actSelect == null ? "null" : "actSelect"));
                WaitingManager.Show();
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                dtIntruction.DateTime = DateTime.Now;
                this.instructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                this.LoadConfig();
                this.branch = BranchDataWorker.Branch;
                if (this.actSelect != null && !String.IsNullOrEmpty(ResourceMessage.TieuDeNutLuuChon))
                {
                    this.btnSave.Text = ResourceMessage.TieuDeNutLuuChon;
                }
                this.LoadCurrentPatyAlter();
                this.LoadCboServiceGroup();
                this.InitComboRepositoryPatientType(this.listPatientType);
                this.InitGridCheckMarksSelectionServiceGroup();
                this.BindTree();
                LoadDataToGrid(true);
                //Kiem tra xem hs da tao chua, set lai action neu ton tai du lieu
                LoadAppointmentService(treatmentId);
                this.treeService.CollapseAll();
                txtServiceName_Search.Width = grcServiceName_TabService.VisibleWidth - 2;
                this.listDatasFix = this.ServiceIsleafADOs;
                this.SetDefaultSerServTotalPrice();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_SERVICE_GROUP> sgSelectedNews = new List<HIS_SERVICE_GROUP>();
                    foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.SERVICE_GROUP_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    SelectOneServiceGroupProcess(sgSelectedNews);
                }

                this.cboServiceGroup.Text = sb.ToString();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = this.treeService.GetDataRecordByNode(e.Node);
                if (data != null && data is ServiceADO)
                {
                    var noteData = (HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO)data;
                    if (String.IsNullOrEmpty(noteData.PARENT_ID__IN_SETY) && noteData.ID == 0)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("treeService_BeforeCheckNode");
                this.isHandlerResetServiceStateCheckedForTreeNodes = false;
                if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                {
                    this.treeService.UncheckAll();
                }
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                treeService.FocusedNode = e.Node;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("treeService_AfterCheckNode");
                var data = this.treeService.GetDataRecordByNode(e.Node) as ServiceADO;

                List<TreeListNode> allParentNodes = new List<TreeListNode>();
                foreach (TreeListNode treeListNode in treeService.GetAllCheckedNodes())
                {
                    if (treeListNode.RootNode != this.treeService.FocusedNode.RootNode)
                    {
                        if (!allParentNodes.Contains(treeListNode.RootNode))
                        {
                            //isChangeParentNode = true;
                            allParentNodes.Add(treeListNode.RootNode);
                            treeListNode.RootNode.UncheckAll();
                        }
                    }
                }

                this.isHandlerResetServiceStateCheckeds = (data != null && data is ServiceADO && ((ServiceADO)data).IsParentServiceId == true);
                if (this.isHandlerResetServiceStateCheckeds || this.isHandlerResetServiceStateCheckedForTreeNodes)
                    this.ResetServiceGroupSelected();

                this.isProcessingAfternodeChecked = true;
                //if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                //{
                //    this.treeService.FocusedNode = e.Node;
                //    //if (e.Node.ParentNode != null)
                //    //{
                //    //    e.Node.ParentNode.UncheckAll();
                //    //}
                //    //e.Node.Checked = true;
                //}
                //else
                //{
                    // Nếu đang chọn huyết học của xét nghiệm mà bấm vào chẩn đoán hình ảnh (check ô vuông) thì chưa hủy tick huyết học => MM: tự động mở cây của chẩn đoán hình ảnh và tick các dịch vụ con của chẩn đoán hình ảnh đồng thời hủy tick huyết học, thu lại cây xét nghiệm.

                    //foreach (TreeListNode treeListNode in treeService.GetAllCheckedNodes())
                    //{
                    //    List<TreeListNode> allParentNodes = new List<TreeListNode>();
                    //    GetParent(treeListNode, allParentNodes);
                    //    var checkParentNode = allParentNodes.FirstOrDefault(o => o.CheckState == CheckState.Unchecked);
                    //    if (checkParentNode != null)
                    //    {
                    //        treeListNode.CheckState = CheckState.Unchecked;
                    //    }
                    //}
                //}
                this.toggleSwitchDataChecked.EditValue = false;
                this.LoadDataToGrid(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetParent(TreeListNode focusedNode, List<TreeListNode> parentNodes)
        {
            try
            {
                if (focusedNode != null && focusedNode.ParentNode != null)
                {
                    parentNodes.Add(focusedNode.ParentNode);
                    this.GetParent(focusedNode.ParentNode, parentNodes);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<ServiceADO> GetAllChildByparent(ServiceADO parent)
        {

            List<ServiceADO> result = new List<ServiceADO>();
            List<ServiceADO> childs = new List<ServiceADO>();

            result.Add(parent);

            if (parent.ID <= 0)
            {
                childs = dicService[SERVICE_ENUM.PARENT_FOR_GRID_SERVCIE].Where(o => o.SERVICE_TYPE_ID == parent.SERVICE_TYPE_ID && o.ID != parent.ID).ToList();
            }
            else
            {
                childs = dicService[SERVICE_ENUM.PARENT_FOR_GRID_SERVCIE].Where(o => o.PARENT_ID == parent.ID && o.SERVICE_TYPE_ID == parent.SERVICE_TYPE_ID).ToList();
            }

            if (parent != null && childs != null && childs.Count > 0)
            {

                foreach (var item in childs)
                {

                    var child1 = GetAllChildByparent(item);

                    if (child1 != null && child1.Count > 0)
                    {

                        result.AddRange(child1);

                    }
                }

            }
            return result;

        }

        private void treeService_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("treeService_Click");
                this.isHandlerResetServiceStateCheckedForTreeNodes = false;

                if (this.isProcessingAfternodeChecked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("isProcessingAfternodeChecked = true");
                    this.isProcessingAfternodeChecked = false;
                    return;
                }

                IscheckAllTreeService = false;
                bool isChangeParentNode = false;
                if (HisConfigCFG.IsSingleCheckservice != commonString__true)
                {
                    // Nếu đang chọn huyết học của xét nghiệm mà bấm vào chẩn đoán hình ảnh (check ô vuông) thì chưa hủy tick huyết học => MM: tự động mở cây của chẩn đoán hình ảnh và tick các dịch vụ con của chẩn đoán hình ảnh đồng thời hủy tick huyết học, thu lại cây xét nghiệm.

                    //foreach (TreeListNode treeListNode in treeService.GetAllCheckedNodes())
                    //{
                    //    List<TreeListNode> allParentNodes = new List<TreeListNode>();
                    //    GetParent(treeListNode, allParentNodes);
                    //    var checkParentNode = allParentNodes.FirstOrDefault(o => o.CheckState == CheckState.Unchecked);
                    //    if (checkParentNode != null)
                    //    {
                    //        treeListNode.CheckState = CheckState.Unchecked;
                    //    }
                    //}

                    List<TreeListNode> allParentNodes = new List<TreeListNode>();
                    foreach (TreeListNode treeListNode in treeService.GetAllCheckedNodes())
                    {
                        if (treeListNode.RootNode != this.treeService.FocusedNode.RootNode)
                        {
                            if (!allParentNodes.Contains(treeListNode.RootNode))
                            {
                                isChangeParentNode = true;
                                allParentNodes.Add(treeListNode.RootNode);
                                treeListNode.RootNode.UncheckAll();
                            }
                        }
                    }
                }
                if (this.treeService.FocusedNode != null)
                {
                    //Process expand node
                    var parent = this.treeService.FocusedNode.ParentNode;
                    //Trường hợp node đang chọn có cha
                    if (parent != null)
                    {
                        this.ProcessExpandTree(this.treeService.FocusedNode);
                    }
                    //Trường hợp node đang chọn không có cha
                    else
                    {
                        this.treeService.CollapseAll();
                        this.treeService.FocusedNode.Expanded = true;
                        bool checkState = this.treeService.FocusedNode.Checked;

                        if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                        {
                            this.treeService.UncheckAll();
                        }

                        if (checkState)
                            this.treeService.FocusedNode.CheckAll();
                    }

                    //Process check state node is leaf
                    var data = this.treeService.GetDataRecordByNode(this.treeService.FocusedNode);

                    this.isHandlerResetServiceStateCheckeds = (data != null && data is ServiceADO && ((ServiceADO)data).IsParentServiceId == true);
                    if (this.isHandlerResetServiceStateCheckeds || this.isHandlerResetServiceStateCheckedForTreeNodes)
                        this.ResetServiceGroupSelected();

                    if (this.treeService.FocusedNode != null
                        && !this.treeService.FocusedNode.HasChildren
                        && data != null
                        && data is ServiceADO)
                    {
                        //Cấu hình cho phép chọn một/nhiều nhóm dịch vụ trên cây là node lá
                        //Nếu không có cấu hình thì mặc định là chọn nhiều
                        //Nếu có cấu hình thì xử lý theo cấu hình
                        if (HisConfigCFG.IsSingleCheckservice == commonString__true)
                        {
                            if (parent != null)
                            {
                                parent.UncheckAll();
                            }
                            this.treeService.FocusedNode.Checked = true;
                        }
                        else
                        {
                            //this.treeService.FocusedNode.Checked = !this.treeService.FocusedNode.Checked;

                            this.treeService.FocusedNode.Checked = (this.treeService.FocusedNode.CheckState == CheckState.Checked ? false : true);
                        }
                    }

                    this.toggleSwitchDataChecked.EditValue = false;
                    if (!this.treeService.FocusedNode.HasChildren || isChangeParentNode)
                    {
                        this.LoadDataToGrid(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessStateTree(TreeListNode focusedNode, int currentLevel)
        {
            try
            {

                if (focusedNode.Level <= currentLevel)
                {
                    if (focusedNode.ParentNode != null)
                    {
                        treeService.BeginUpdate();
                        var list = treeService.GetNodeList().Where(o => o.ParentNode == focusedNode.ParentNode
                            && o.CheckState == CheckState.Unchecked);
                        TreeListNode parent = treeService.GetNodeList().FirstOrDefault(o => o == focusedNode.ParentNode);
                        if (list == null || list.Count() == 0)
                        {
                            parent.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            parent.CheckState = CheckState.Unchecked;
                        }
                        treeService.EndUpdate();
                        ProcessStateTree(parent, currentLevel);
                    }
                }

                if (focusedNode.Level >= currentLevel)
                {
                    if (focusedNode.HasChildren)
                    {
                        var list = treeService.GetNodeList().Where(o => o.ParentNode == focusedNode);
                        foreach (var item in list)
                        {
                            treeService.BeginUpdate();
                            item.CheckState = item.Checked ? CheckState.Unchecked : CheckState.Checked;
                            treeService.EndUpdate();
                            ProcessStateTree(item, currentLevel);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessExpandTree(TreeListNode focusedNode)
        {
            try
            {
                TreeListNode parent = focusedNode.ParentNode;
                if (parent != null)
                {
                    this.treeService.CollapseAll();
                    List<TreeListNode> allParentNodes = new List<TreeListNode>();
                    this.GetParent(focusedNode, allParentNodes);
                    if (allParentNodes != null && allParentNodes.Count > 0)
                    {
                        var nodes = this.treeService.GetNodeList();
                        foreach (var item in nodes)
                        {
                            //item.Checked = false;
                            if (focusedNode == item)
                            {
                                focusedNode.Expanded = true;
                            }
                            else if (allParentNodes.ToArray().Contains(item))
                            {
                                item.Expanded = true;
                            }
                            else
                            {
                                item.Expanded = false;
                            }
                        }
                    }
                }
                this.treeService.FocusedNode = focusedNode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceProcess_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.treeService.FocusedNode != null)
                    {
                        this.treeService.FocusedNode.Checked = true;
                        this.gridControlServiceProcess.Focus();
                        this.gridViewServiceProcess.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var node = this.treeService.FocusedNode;
                    var data = this.treeService.GetDataRecordByNode(node);
                    if (node != null && node.HasChildren && data != null && data is ServiceADO)
                    {
                        node.Expanded = !node.Expanded;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                string rowValue = Convert.ToString(this.gridViewServiceProcess.GetGroupRowValue(e.RowHandle, info.Column));
                info.GroupText = rowValue;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        e.RepositoryItem = this.repositoryItemcboPatientType_TabService;
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        e.RepositoryItem = this.repositoryItemSpinAmount_TabService;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void gridViewServiceProcess_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            if (e.ColumnName == "AMOUNT")
            {
                this.gridViewServiceProcess_CustomRowError(sender, e);
            }
        }

        private void gridViewServiceProcess_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlServiceProcess.DataSource as List<SereServADO>;
                var row = listDatas[index];
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.IsChecked && row.AMOUNT <= 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Số lượng phải lớn hơn 0";
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> getSereServWithMinDuration(List<SereServADO> serviceCheckeds)
        {
            List<HIS_SERE_SERV> listSereServResult = null;
            try
            {
                if (serviceCheckeds != null && serviceCheckeds.Count > 0 && this.treatment != null)
                {
                    List<SereServADO> sereServADOExistMinDUration = serviceCheckeds.Where(o => o.MIN_DURATION != null).ToList();
                    if (sereServADOExistMinDUration != null && sereServADOExistMinDUration.Count > 0)
                    {
                        List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                        foreach (var item in sereServADOExistMinDUration)
                        {
                            ServiceDuration serviceDuration = new ServiceDuration();
                            serviceDuration.ServiceId = item.SERVICE_ID;
                            serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                            serviceDurations.Add(serviceDuration);
                        }
                        // gọi api để lấy về thông báo
                        CommonParam param = new CommonParam();
                        HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                        hisSereServMinDurationFilter.ServiceDurations = serviceDurations;

                        hisSereServMinDurationFilter.InstructionTime = this.treatment.APPOINTMENT_TIME ?? 0;

                        hisSereServMinDurationFilter.PatientId = this.treatment.PATIENT_ID;

                        var result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);

                        if (result == null || result.Count() == 0)
                            return listSereServResult;

                        listSereServResult = new List<HIS_SERE_SERV>();
                        var listSSTemp = result.GroupBy(o => o.SERVICE_ID).ToList();
                        foreach (var item in listSSTemp)
                        {
                            var itemGroup = item.OrderByDescending(o => o.TDL_INTRUCTION_TIME).FirstOrDefault();
                            listSereServResult.Add(itemGroup);
                        }
                    }
                    else
                    {
                        listSereServResult = null;
                    }
                }
                else
                {
                    listSereServResult = null;
                }
            }
            catch (Exception ex)
            {
                listSereServResult = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return listSereServResult;
        }

        private bool ValidSereServWithMinDuration(List<SereServADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                List<HIS_SERE_SERV> sereServMinDurations = getSereServWithMinDuration(serviceCheckeds__Send);
                if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                {
                    string sereServMinDurationStr = "";
                    foreach (var item in sereServMinDurations)
                    {
                        sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + " - " +
                           Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TDL_INTRUCTION_TIME) +
                           " (" + item.TDL_SERVICE_REQ_CODE +
                           "); ";
                    }

                    if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                        return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("btnSave_Click.1");
                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                bool valid = (this.action == ACTION.UPDATE || this.Check());
                Inventec.Common.Logging.LogSystem.Debug("btnSave_Click.2. valid = " + valid + ", action=" + action);
                if (valid)
                {
                    Inventec.Common.Logging.LogSystem.Debug("btnSave_Click.3");
                    List<HIS_APPOINTMENT_SERV> appointmentServs = null;
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisAppointmentServSDO hisAppointmentServSDO = new HisAppointmentServSDO();
                    this.MakeAppointmentServ(ref hisAppointmentServSDO);
                    if (this.actSelect != null)
                    {
                        this.actSelect(hisAppointmentServSDO.AppointmentServs);
                        Inventec.Common.Logging.LogSystem.Info("Neu module ngoai truyen vao option \"gui du lieu da luu\", thi sua label nut \"Luu\" thanh nut \"Dong y\" - Khi nguoi dung nhan \"Dong y\" thi van thuc hien goi len server de luu du lieu, ma tra về danh sach cac dich vu da chon cho module ngoai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisAppointmentServSDO.AppointmentServs), hisAppointmentServSDO.AppointmentServs));
                    }
                    Inventec.Common.Logging.LogSystem.Debug("btnSave_Click.4");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisAppointmentServSDO), hisAppointmentServSDO));
                    this.EnableControlSave(false);
                    WaitingManager.Show();
                    string api = this.action == ACTION.UPDATE ? "api/HisAppointmentServ/Update" : "api/HisAppointmentServ/Create";
                    appointmentServs = new BackendAdapter(param)
                        .Post<List<HIS_APPOINTMENT_SERV>>(api,
                        ApiConsumers.MosConsumer, hisAppointmentServSDO, param);

                    WaitingManager.Hide();
                    this.EnableControlSave(true);
                    if (appointmentServs != null)
                    {
                        success = true;
                        this.action = appointmentServs.Count > 0 ? ACTION.UPDATE : ACTION.CREATE;
                        List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                        foreach (var item in serviceCheckeds__Send)
                        {
                            HIS_APPOINTMENT_SERV appointmentServ = appointmentServs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                            if (appointmentServ != null)
                            {
                                item.AppointmentSereServId = appointmentServ.ID;
                            }
                        }
                        gridControlAppointmentServ.DataSource = serviceCheckeds__Send;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("btnSave_Click.5");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MakeAppointmentServ(ref HisAppointmentServSDO _hisAppointmentServSDO)
        {
            try
            {
                if (_hisAppointmentServSDO == null)
                {
                    _hisAppointmentServSDO = new HisAppointmentServSDO();
                }

                _hisAppointmentServSDO.TreatmentId = treatmentId;
                _hisAppointmentServSDO.AppointmentServs = new List<HIS_APPOINTMENT_SERV>();
                List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                foreach (var item in serviceCheckeds__Send)
                {
                    HIS_APPOINTMENT_SERV appointmentServ = new HIS_APPOINTMENT_SERV();
                    appointmentServ.AMOUNT = item.AMOUNT;
                    appointmentServ.SERVICE_ID = item.SERVICE_ID;
                    appointmentServ.ID = item.AppointmentSereServId ?? 0;
                    if (item.PATIENT_TYPE_ID > 0)
                    {
                        appointmentServ.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
                    }
                    _hisAppointmentServSDO.AppointmentServs.Add(appointmentServ);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {
                List<SereServADO> serviceCheckeds = this.ServiceIsleafADOs.Where(o => o.IsChecked).ToList();
                if (serviceCheckeds == null || serviceCheckeds.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dịch vụ nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                List<SereServADO> serviceCheckedAmountFails = this.ServiceIsleafADOs.Where(o => o.IsChecked && o.AMOUNT <= 0).ToList();
                if (serviceCheckedAmountFails != null && serviceCheckedAmountFails.Count > 0)
                {
                    return false;
                }
                result = ValidSereServWithMinDuration(serviceCheckeds);

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
        {
            try
            {
                ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch != null)
                {
                    this.LoadDataToGrid(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDongTatCa_Click(object sender, EventArgs e)
        {
            try
            {
                this.treeService.CollapseAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnMoTatCa_Click(object sender, EventArgs e)
        {
            try
            {
                IscheckAllTreeService = true;
                this.treeService.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAppointmentServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        private void btnLamLai_Click(object sender, EventArgs e)
        {
            try
            {
                this.notSearch = 2;
                txtServiceCode_Search.Text = "";
                txtServiceName_Search.Text = "";
                this.treeService.UncheckAll();
                foreach (var item in this.ServiceIsleafADOs)
                {
                    item.AMOUNT = 1;
                    item.IsChecked = false;
                    item.PATIENT_TYPE_ID = 0;
                }
                LoadDataToGrid(true);
                gridControlAppointmentServ.DataSource = null;
                this.LoadAppointmentService(this.treatmentId);
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnLamLai.Enabled)
            {
                btnLamLai_Click(null, null);
            }
        }

        private void cboServiceGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = this.cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                        gridCheckMark.ClearSelection(this.cboServiceGroup.Properties.View);
                    this.cboServiceGroup.EditValue = null;
                    this.cboServiceGroup.Properties.Buttons[1].Visible = false;
                    this.gridControlServiceProcess.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;

                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_GROUP_NAME.ToString());
                }
                if (gridCheckMark.Selection != null && gridCheckMark.Selection.Count > 0)
                {
                    e.DisplayText = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboPriviousServiceReq.Properties.Buttons[1].Visible = false;
                    this.cboPriviousServiceReq.EditValue = null;
                    this.gridControlServiceProcess.DataSource = null;
                    foreach (var item in this.ServiceIsleafADOs)
                    {
                        item.IsChecked = false;
                    }
                    this.toggleSwitchDataChecked.EditValue = false;
                }
                else if (e.Button.Kind == ButtonPredefines.Search)
                {
                    WaitingManager.Show();
                    LogSystem.Debug("Begin FillDataToComboPriviousServiceReq");
                    this.FillDataToComboPriviousServiceReq(PatientTypeAlter);
                    LogSystem.Debug("End FillDataToComboPriviousServiceReq");
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    List<V_HIS_SERVICE_REQ_6> currentPreServiceReqs = cboPriviousServiceReq.Properties.DataSource as List<V_HIS_SERVICE_REQ_6>;
                    if (this.cboPriviousServiceReq.EditValue != null && currentPreServiceReqs != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 data = currentPreServiceReqs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPriviousServiceReq.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceServiceReqPrevious(data);
                            this.btnSave.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_INTRUCTION_TIME")
                {
                    var item = ((List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>)this.cboPriviousServiceReq.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.INTRUCTION_TIME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPriviousServiceReq_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    List<V_HIS_SERVICE_REQ_6> currentPreServiceReqs = cboPriviousServiceReq.Properties.DataSource as List<V_HIS_SERVICE_REQ_6>;
                    if (this.cboPriviousServiceReq.EditValue != null && currentPreServiceReqs != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 data = currentPreServiceReqs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPriviousServiceReq.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceServiceReqPrevious(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPriviousServiceReq.EditValue != null)
                {
                    cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboPriviousServiceReq.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void treeService_BeforeExpand(object sender, DevExpress.XtraTreeList.BeforeExpandEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("treeService_BeforeExpand");
                if (e.Node != null && !IscheckAllTreeService)
                {

                    //Process expand node
                    var parent = e.Node.ParentNode;
                    //Trường hợp node đang chọn có cha
                    if (parent != null)
                    {
                        this.ProcessExpandTree(e.Node);
                    }
                    //Trường hợp node đang chọn không có cha
                    else
                    {
                        this.treeService.CollapseAll();
                        e.Node.Expanded = true;
                    }
                    // bỏ focus node tránh trường hợp sang hàm click tree
                    this.treeService.FocusedNode = e.Node;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboServiceGroup.Focus();
                    cboServiceGroup.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtIntruction_Closed(object sender, ClosedEventArgs e)
        {
            if (dtIntruction.EditValue != null)
            {
                cboServiceGroup.Focus();
                cboServiceGroup.SelectAll();
            }
        }

        private void cboServiceGroup_Closed(object sender, ClosedEventArgs e)
        {
            if (cboServiceGroup.EditValue != null)
            {
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;

                if (gridCheckMark == null) return;

                List<HIS_SERVICE_GROUP> serviceGroups = cboServiceGroup.Properties.DataSource as List<HIS_SERVICE_GROUP>;
                HIS_SERVICE_GROUP serviceGroup = serviceGroups.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceGroup.EditValue.ToString()));

                List<HIS_SERVICE_GROUP> list = new List<HIS_SERVICE_GROUP>();
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP rv in gridCheckMark.Selection)
                {
                    list.Add(rv);
                }
                if (serviceGroup != null)
                {
                    if (list.Contains(serviceGroup))
                    {
                        list = list.Where(o => o.ID != serviceGroup.ID).ToList();
                    }
                    else
                    {
                        list.Add(serviceGroup);
                    }
                }

                gridCheckMark.SelectAll(list);
                cboPriviousServiceReq.Focus();
                cboPriviousServiceReq.SelectAll();
            }
        }

        private void cboServiceGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {


                    cboPriviousServiceReq.Focus();
                    cboPriviousServiceReq.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceCode_Search_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.notSearch > 0)
                {
                    this.notSearch--;
                    return;
                }
                this.gridViewServiceProcess.BeginDataUpdate();
                var nodeCheckeds = this.treeService.GetAllCheckedNodes();
                List<SereServADO> listSereServADO = new List<SereServADO>();
                if (nodeCheckeds != null && nodeCheckeds.Count > 0 && !HisConfigCFG.IsSearchAll)
                {
                    var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;

                    List<ServiceADO> parentNodes = new List<ServiceADO>();
                    foreach (var node in nodeCheckeds)
                    {
                        var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
                        if (data != null)
                        {
                            parentNodes.Add(data);
                        }
                    }
                    if (parentNodes.Count > 0)
                    {
                        var parentIdAllows = parentNodes.Select(o => o.ID).ToArray();

                        //Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
                        var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRootSetys != null && parentRootSetys.Count > 0)
                        {
                            foreach (var item in parentRootSetys)
                            {
                                if (item != null)
                                {
                                    var childOfParentNodeNoParents = allDatas.Where(o =>
                                    (o.PARENT_ID == null || item.ID == o.PARENT_ID)
                                    && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
                                    && parentIdAllows.Contains(o.PARENT_ID ?? 0)
                                    ).ToList();
                                    if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
                                    {
                                        listSereServADO.AddRange(childOfParentNodeNoParents);
                                    }
                                }
                            }
                        }
                        var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRoots != null && parentRoots.Count > 0)
                        {
                            foreach (var item in parentRoots)
                            {
                                var childs = GetChilds(item);
                                if (childs != null && childs.Count > 0)
                                {
                                    listSereServADO.AddRange(childs);
                                }
                            }
                        }
                        listSereServADO = listSereServADO.Distinct().ToList();
                        this.gridControlServiceProcess.DataSource = null;
                        if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text))
                        {
                            var listResult = listSereServADO.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList();
                            this.gridControlServiceProcess.DataSource = listResult;
                        }
                        else
                        {
                            this.gridControlServiceProcess.DataSource = listSereServADO;
                        }
                    }
                }
                else
                {
                    this.gridControlServiceProcess.DataSource = null;
                    if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text))
                    {
                        var listResult = (this.listDatasFix != null && this.listDatasFix.Count() > 0) ? this.listDatasFix.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList() : null;
                        this.gridControlServiceProcess.DataSource = listResult;
                    }
                    else
                    {
                        this.gridControlServiceProcess.DataSource = this.listDatasFix;
                    }
                }
                this.gridViewServiceProcess.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CalcRowHeight(object sender, DevExpress.XtraGrid.Views.Grid.RowHeightEventArgs e)
        {
            try
            {
                if (gridViewServiceProcess.IsFilterRow(e.RowHandle))
                {
                    var fontSize = ApplicationFontWorker.GetFontSize();
                    if (fontSize == ApplicationFontConfig.FontSize825)
                    {
                        //txtServiceName_Search.Point = Point(123, 21);
                        //txtServiceCode_Search.Point = Point(31, 21);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize875)
                    {
                        e.RowHeight = 23;
                        txtServiceName_Search.Location = new Point(123, 23);
                        txtServiceCode_Search.Location = new Point(31, 23);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize925)
                    {
                        e.RowHeight = 25;
                        txtServiceName_Search.Location = new Point(123, 25);
                        txtServiceCode_Search.Location = new Point(31, 25);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize975)
                    {
                        e.RowHeight = 27;
                        txtServiceName_Search.Location = new Point(123, 27);
                        txtServiceCode_Search.Location = new Point(31, 27);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize1025)
                    {
                        txtServiceName_Search.Location = new Point(123, 29);
                        txtServiceCode_Search.Location = new Point(31, 29);
                        e.RowHeight = 30;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_ColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "TDL_SERVICE_NAME")
                {
                    txtServiceName_Search.Width = e.Column.Width - 2;
                }
                else if (e.Column.FieldName == "TDL_SERVICE_CODE")
                {
                    txtServiceCode_Search.Width = e.Column.Width;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceName_Search_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.notSearch > 0)
                {
                    this.notSearch--;
                    return;
                }
                this.gridViewServiceProcess.BeginDataUpdate();
                var nodeCheckeds = this.treeService.GetAllCheckedNodes();
                List<SereServADO> listSereServADO = new List<SereServADO>();
                if (nodeCheckeds != null && nodeCheckeds.Count > 0 && !HisConfigCFG.IsSearchAll)
                {
                    var allDatas = this.ServiceIsleafADOs != null && this.ServiceIsleafADOs.Count > 0 ? this.ServiceIsleafADOs.AsQueryable() : null;

                    List<ServiceADO> parentNodes = new List<ServiceADO>();
                    foreach (var node in nodeCheckeds)
                    {
                        var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
                        if (data != null)
                        {
                            parentNodes.Add(data);
                        }
                    }
                    if (parentNodes.Count > 0)
                    {
                        var parentIdAllows = parentNodes.Select(o => o.ID).ToArray();
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parentIdAllows), parentIdAllows));
                        //Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
                        var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRootSetys != null && parentRootSetys.Count > 0)
                        {
                            foreach (var item in parentRootSetys)
                            {
                                if (item != null)
                                {
                                    var childOfParentNodeNoParents = allDatas.Where(o =>
                                    (o.PARENT_ID == null || item.ID == o.PARENT_ID)
                                    && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
                                    && parentIdAllows.Contains(o.PARENT_ID ?? 0)
                                    ).ToList();
                                    if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
                                    {
                                        listSereServADO.AddRange(childOfParentNodeNoParents);
                                    }
                                }
                            }
                        }
                        var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                        if (parentRoots != null && parentRoots.Count > 0)
                        {
                            foreach (var item in parentRoots)
                            {
                                var childs = GetChilds(item);
                                if (childs != null && childs.Count > 0)
                                {
                                    listSereServADO.AddRange(childs);
                                }
                            }
                        }
                        listSereServADO = listSereServADO.Distinct().ToList();
                        this.gridControlServiceProcess.DataSource = null;
                        if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text))
                        {
                            var listResult = listSereServADO.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList();
                            this.gridControlServiceProcess.DataSource = listResult;
                        }
                        else
                        {
                            this.gridControlServiceProcess.DataSource = listSereServADO;
                        }
                    }
                }
                else
                {
                    this.gridControlServiceProcess.DataSource = null;
                    if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text))
                    {
                        var listResult = (this.listDatasFix != null && this.listDatasFix.Count() > 0) ? this.listDatasFix.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList() : null;
                        this.gridControlServiceProcess.DataSource = listResult;
                    }
                    else
                    {
                        this.gridControlServiceProcess.DataSource = this.listDatasFix;
                    }
                }
                this.gridViewServiceProcess.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
            {
                cboServiceGroup.ClosePopup();
                cboServiceGroup.SelectAll();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                cboServiceGroup.ClosePopup();
            }
            else
                cboServiceGroup.ShowPopup();
            e.Handled = true;
        }

        private void cboServiceGroup_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboServiceGroup_Leave(object sender, EventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                    cboServiceGroup.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServiceProcess_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        SereServADO oneServiceSDO = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "PRICE_DISPLAY" && oneServiceSDO.IsChecked)
                            {
                                if (oneServiceSDO.PATIENT_TYPE_ID != 0 && BranchDataWorker.DicServicePatyInBranch.ContainsKey(oneServiceSDO.SERVICE_ID) && instructionTime > 0)
                                {
                                    long? intructionNumByType = 1;
                                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(oneServiceSDO.SERVICE_ID, this.patientTypeIdAls);
                                    V_HIS_SERVICE_PATY oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, this.branch.ID, null, null, null, instructionTime, this.treatment.IN_TIME, oneServiceSDO.SERVICE_ID, oneServiceSDO.PATIENT_TYPE_ID, null, intructionNumByType);

                                    if (oneServicePatyPrice != null)
                                    {
                                        e.Value = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("oneServiceSDO.PATIENT_TYPE_ID else continued");
                                }
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServiceProcess_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                if (sereServADO != null)
                {
                    if (e.Column.FieldName == this.grcChecked_TabService.FieldName
                        || e.Column.FieldName == this.grcAmount_TabService.FieldName
                        || e.Column.FieldName == this.gridColumn_ServicePatientType.FieldName
                        )
                    {
                        if (!sereServADO.IsChecked)
                        {
                            this.ResetOneService(sereServADO);
                            sereServADO.IsNoDifference = false;
                        }
                        else
                        {
                            if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                            {
                                MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        this.gridControlServiceProcess.RefreshDataSource();
                    }
                    SetDefaultSerServTotalPrice();
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServiceProcess_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SereServADO data = view.GetFocusedRow() as SereServADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        this.FillDataIntoPatientTypeCombo(data, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetOneService(SereServADO item)
        {
            try
            {
                item.PATIENT_TYPE_ID = 0;
                item.PATIENT_TYPE_CODE = null;
                item.PATIENT_TYPE_NAME = null;
                item.TDL_EXECUTE_ROOM_ID = 0;
                item.PRICE = 0;

                item.IsNoDifference = false;
                item.PRIMARY_PATIENT_TYPE_ID = null;
                item.ErrorMessageAmount = "";
                item.ErrorTypeAmount = ErrorType.None;
                item.ErrorMessagePatientTypeId = "";
                item.ErrorTypePatientTypeId = ErrorType.None;
                item.ErrorMessageIsAssignDay = "";
                item.ErrorTypeIsAssignDay = ErrorType.None;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtServiceName_Search_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewServiceProcess.Focus();
                    gridViewServiceProcess.FocusedRowHandle = 0;
                    gridViewServiceProcess.FocusedColumn = grcServiceName_TabService;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (sereServADO.IsChecked)
                    {
                        if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                        {
                            MessageBox.Show("Dịch vụ cận lâm sàng có giới hạn chỉ định thanh toán BHYT. Đề nghị BS xem xét trước khi chỉ định", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceCode_Search_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    gridViewServiceProcess.Focus();
                    gridViewServiceProcess.FocusedRowHandle = 0;
                    gridViewServiceProcess.FocusedColumn = grcServiceName_TabService;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (sereServADO.IsChecked)
                    {
                        if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                        {
                            MessageBox.Show("Dịch vụ cận lâm sàng có giới hạn chỉ định thanh toán BHYT. Đề nghị BS xem xét trước khi chỉ định", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceProcess_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        UpdateCurrentFocusRow(sereServADO);
                    }
                }
                if (e.KeyCode == Keys.Enter)
                {
                    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        if (sereServADO.IsChecked)
                        {
                            if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                            {
                                MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void UpdateCurrentFocusRow(SereServADO sereServADO)
        {
            try
            {
                sereServADO.IsChecked = !sereServADO.IsChecked;
                if (!sereServADO.IsChecked)
                {
                    this.ResetOneService(sereServADO);
                    sereServADO.IsNoDifference = false;
                }
                else 
                {
                    if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                    {
                        MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                this.gridControlServiceProcess.RefreshDataSource();
                SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeService_NodeChanged(object sender, DevExpress.XtraTreeList.NodeChangedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("treeService_NodeChanged");
                var dataRecord = this.treeService.GetDataRecordByNode(e.Node) as ServiceADO;
                if (dataRecord != null && dataRecord.IsParentServiceId == true)
                {
                    this.isHandlerResetServiceStateCheckedForTreeNodes = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckExistServicePaymentLimit(string ServiceCode)
        {
            bool result = false;
            try
            {
                string servicePaymentLimit = HisConfigCFG.ServiceHasPaymentLimitBHYT.ToLower();
                if (!String.IsNullOrEmpty(servicePaymentLimit))
                {
                    string[] serviceArr = servicePaymentLimit.Split(',');
                    if (serviceArr != null && serviceArr.Length > 0)
                    {
                        if (serviceArr.Contains(ServiceCode.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
