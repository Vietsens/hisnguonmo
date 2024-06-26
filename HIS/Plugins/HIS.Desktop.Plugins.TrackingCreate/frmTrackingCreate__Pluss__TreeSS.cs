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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TrackingCreate.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using MPS.ADO.TrackingPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    public partial class frmTrackingCreateNew : FormBase
    {
        private void treeListServiceReq_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                //SetCheckChildNode(e.Node, e.Node.CheckState);
                // SetCheckParentNode(e.Node, e.Node.CheckState);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckChildNode(TreeListNode node, CheckState check)
        {
            try
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    node.Nodes[i].CheckState = check;
                    SetCheckChildNode(node.Nodes[i], check);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckParentNode(TreeListNode node, CheckState check)
        {
            try
            {
                if (node.ParentNode != null)
                {
                    bool b = false;
                    CheckState state;
                    for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                    {
                        state = node.ParentNode.Nodes[i].CheckState;
                        if (!check.Equals(state))
                        {
                            b = !b;
                            break;
                        }
                    }
                    node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                    SetCheckParentNode(node.ParentNode, check);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (e.Node.HasChildren)
                {
                    if (e.Node.Checked)
                    {
                        e.Node.UncheckAll();
                    }
                    else
                    {
                        var data = treeListServiceReq.GetDataRecordByNode(e.Node) as TreeSereServADO;
                        if (data.IS_DISABLE == 1 || data.IS_TEMPORARY_PRES == 1)
                        {
                            e.Node.UncheckAll();
                            e.State = CheckState.Unchecked;
                        }
                        else
                            CheckNode(e.Node);
                    }
                    TreeListNode node = e.Node;
                    CheckNodesParent(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_CustomDrawNodeCheckBox(object sender, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (!e.Node.HasChildren)
                {
                    e.Handled = true;
                }
                else
                {
                    var data = treeListServiceReq.GetDataRecordByNode(e.Node) as TreeSereServADO;
                    if (data.IS_DISABLE == 1 || data.IS_TEMPORARY_PRES == 1)
                    {
                        e.ObjectArgs.State = DevExpress.Utils.Drawing.ObjectState.Disabled;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<TreeSereServADO> GetListCheck()
        {
            List<TreeSereServADO> result = new List<TreeSereServADO>();
            try
            {
                foreach (TreeListNode node in treeListServiceReq.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<TreeSereServADO>();
            }
            return result;
        }

        public List<TreeSereServADO> GetListCheckTab2()
        {
            List<TreeSereServADO> result = new List<TreeSereServADO>();
            try
            {
                foreach (TreeListNode node in treeListPreventive.Nodes)
                {
                    GetListNodeCheckTab2(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<TreeSereServADO>();
            }
            return result;
        }

        private void GetListNodeCheckTab2(ref List<TreeSereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((TreeSereServADO)treeListPreventive.GetDataRecordByNode(node));
                    }
                }
                else
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheckTab2(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetListNodeCheck(ref List<TreeSereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((TreeSereServADO)treeListServiceReq.GetDataRecordByNode(node));
                    }
                }
                else
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheck(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(e.Node);
                if (e.Node.HasChildren)
                {
                    if (data.IS_THUHOI)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (data.IS_OUT_MEDI_MATE)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (data.IS_RATION)
                    {
                        e.Appearance.ForeColor = Color.Green;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
                else
                {
                    e.Appearance.ForeColor = Color.Black;
                }
                if (e.Node.Checked && this._ServiceReqByTrackings != null && this._ServiceReqByTrackings.Count > 0)
                {
                    foreach (var item in this._ServiceReqByTrackings)
                    {
                        if (data.SERVICE_REQ_ID == item.ID)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                var data = (TreeSereServADO)treeListServiceReq.GetDataRecordByNode(e.Node);
                if (data != null && e.Column.FieldName == "AMOUNT_STR" && !e.Node.HasChildren)
                {
                    e.Value = data.AMOUNT;
                }
                else if (e.Column.FieldName == "IMG" && ((data.LEVER == 3 && !data.IsMedicinePreventive) || (data.LEVER == 4 && data.IsServiceUseForTracking)))
                {
                    try
                    {
                        long statusId = data.SERVICE_REQ_STT_ID;
                        if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot icon trang thai yeu cau dich vu IMG", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void treeSereServ_CheckYLFromBB(TreeListNodes treeListNodes)
        {
            try
            {
                if (treeListNodes != null)
                {
                    int stt = 0;
                    foreach (TreeListNode node in treeListNodes)
                    {
                        node.UncheckAll();
                        CheckNodeYLFromBB(node);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNodeYLFromBB(TreeListNode node)
        {
            try
            {
                var roomsIsBB = BackendDataWorker.Get<HIS_ROOM>().Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG).Select(o => o.ID).ToList();
                if (roomsIsBB == null)
                {
                    roomsIsBB = new List<long>();
                }
                if (node != null)
                {
                    var nodeData = (TreeSereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (roomsIsBB.Contains(nodeData.TDL_REQUEST_ROOM_ID) && nodeData.IS_TEMPORARY_PRES != 1 && nodeData.IS_DISABLE != 1)
                    {
                        node.CheckAll();
                    }
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        if (nodeData != null)
                        {
                            if (roomsIsBB.Contains(nodeData.TDL_REQUEST_ROOM_ID) && nodeData.IS_TEMPORARY_PRES != 1 && nodeData.IS_DISABLE != 1)
                            {
                                childNode.CheckAll();
                            }
                        }
                        CheckNodeYLFromBB(childNode);
                    }

                    CheckNodesParent(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CheckAllNode(TreeListNodes treeListNodes)
        {
            try
            {
                if (treeListNodes != null)
                {
                    foreach (TreeListNode node in treeListNodes)
                    {
                        node.CheckAll();
                        CheckNode(node);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNode(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        var nodeData = (TreeSereServADO)node.TreeList.GetDataRecordByNode(node);
                        if (nodeData != null)
                        {
                            if (nodeData.IS_THUHOI)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else if (nodeData.IsNotShowMediAndMate)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else if (nodeData.IS_TEMPORARY_PRES == 1 || nodeData.IS_DISABLE == 1)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else
                            {
                                childNode.CheckAll();
                                CheckNode(childNode);
                            }
                        }
                        //  CheckNode(childNode);
                    }
                    CheckNodesParent(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNodesParent(TreeListNode node)
        {
            if (node != null)
            {
                while (node.ParentNode != null)
                {
                    node = node.ParentNode;
                    bool hasCheck = false;
                    bool allCheck = true;
                    foreach (TreeListNode item in node.Nodes)
                    {
                        if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                        {
                            hasCheck = true;
                        }
                        if (item.CheckState == CheckState.Unchecked || item.CheckState == CheckState.Indeterminate)
                        {
                            allCheck = false;
                        }
                    }
                    if (allCheck)
                    {
                        node.CheckState = CheckState.Checked;
                    }
                    else if (hasCheck)
                    {
                        node.CheckState = CheckState.Indeterminate;
                    }
                    else
                    {
                        node.CheckState = CheckState.Unchecked;
                    }
                }
            }
        }

        private void repositoryItemServiceReqDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var rowData = treeListServiceReq.GetDataRecordByNode(treeListServiceReq.FocusedNode) as TreeSereServADO;
                if (rowData == null)
                {
                    //Inventec.Common.Logging.LogSystem.Info("rowData thuc hien huy yeu cau dich vu null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowData), rowData));
                    return;
                }

                if (MessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (CheckParentBeforeDelete(rowData.ID) && DevExpress.XtraEditors.XtraMessageBox.Show("Đã có y lệnh đính kèm (CLS). Bạn có chắc chắn muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }

                    WaitingManager.Show();
                    MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                    sdo.Id = rowData.SERVICE_REQ_ID;
                    sdo.RequestRoomId = this.currentModule.RoomId;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_SERVICE_REQ_DELETE, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    WaitingManager.Hide();
                    if (success)
                    {
                        LoadDataSS(false);
                        isSearch = false;
                        //backgroundWorker1.RunWorkerAsync();
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private bool CheckParentBeforeDelete(long _serviceReqId)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.PARENT_ID = _serviceReqId;
                var serviceReqParent = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(UriStores.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);
                if (serviceReqParent != null && serviceReqParent.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }


        private void repositoryItemServiceReqEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = treeListServiceReq.GetDataRecordByNode(treeListServiceReq.FocusedNode) as TreeSereServADO;
                if (data != null)// && data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                {
                    var paramCommon = new CommonParam();
                    var treatment = new HIS_TREATMENT();
                    HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                    treatFilter.ID = data.TDL_TREATMENT_ID;
                    var currentTreats = new BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(UriStores.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (currentTreats != null && currentTreats.Count == 1)
                    {
                        var treat = currentTreats.FirstOrDefault();
                        if (treat.IS_PAUSE == 1 || treat.IS_ACTIVE != 1)
                        {
                            //Inventec.Common.Logging.LogSystem.Debug(Resources.ResourceMessage.HoSoDieuTriDangTamKhoa);
                            MessageBox.Show(Resources.ResourceMessage.HoSoDieuTriDangTamKhoa);
                            return;
                        }
                    }
                    else
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Resources.ResourceMessage.KhongTimThayHoSoDieuTri);
                        MessageBox.Show(Resources.ResourceMessage.KhongTimThayHoSoDieuTri);
                        return;
                    }

                    var serviceReqPrintRaw = GetServiceReqForPrint(data.SERVICE_REQ_ID ?? 0);

                    if (data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    {
                        WaitingManager.Show();
                        List<object> sendObj = new List<object>() { serviceReqPrintRaw.ID };
                        CallModule("HIS.Desktop.Plugins.UpdateExamServiceReq", sendObj);
                        WaitingManager.Hide();
                    }
                    else if (data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT ||
                        data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT)
                    {
                        WaitingManager.Show();
                        AssignPrescriptionEditADO assignEditADO = null;
                        var serviceReq = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(serviceReq, serviceReqPrintRaw);
                        HisExpMestFilter expfilter = new HisExpMestFilter();
                        expfilter.SERVICE_REQ_ID = serviceReqPrintRaw.ID;
                        var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (expMests != null && expMests.Count == 1)
                        {
                            var expMest = expMests.FirstOrDefault();
                            if (expMest.IS_NOT_TAKEN.HasValue && expMest.IS_NOT_TAKEN.Value == 1)
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(Resources.ResourceMessage.DonKhongLayKhongChoPhepSua);
                                return;
                            }
                            assignEditADO = new AssignPrescriptionEditADO(serviceReq, expMest, FillDataApterSave);
                        }
                        else
                        {
                            assignEditADO = new AssignPrescriptionEditADO(serviceReq, null, FillDataApterSave);
                        }

                        if (data.IS_EXECUTE_KIDNEY_PRES == 1)
                        {
                            AssignPrescriptionKidneyADO assignPrescriptionKidneyADO = new AssignPrescriptionKidneyADO();
                            assignPrescriptionKidneyADO.AssignPrescriptionEditADO = assignEditADO;
                            List<object> sendObj = new List<object>() { assignPrescriptionKidneyADO };

                            CallModule("HIS.Desktop.Plugins.AssignPrescriptionKidney", sendObj);
                        }
                        else
                        {
                            var assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(data.TDL_TREATMENT_ID ?? 0, 0, serviceReq.ID);
                            assignServiceADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                            assignServiceADO.PatientDob = data.TDL_PATIENT_DOB;
                            assignServiceADO.PatientName = data.TDL_PATIENT_NAME;

                            assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                            List<object> sendObj = new List<object>() { assignServiceADO };

                            if (data.PRESCRIPTION_TYPE_ID == 1)
                            {
                                CallModule("HIS.Desktop.Plugins.AssignPrescriptionPK", sendObj);
                            }
                            else if (data.PRESCRIPTION_TYPE_ID == 2)
                            {
                                CallModule("HIS.Desktop.Plugins.AssignPrescriptionYHCT", sendObj);
                            }
                            else if (data.PRESCRIPTION_TYPE_ID == 3)
                            {
                                CallModule("HIS.Desktop.Plugins.AssignPrescriptionCLS", sendObj);
                            }
                        }

                        WaitingManager.Hide();
                    }
                    else if (data.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                    {
                        // MessageManager.Show(Resources.ResourceMessage.DonMauKhongChoPhepSua);
                        var serviceReq = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(serviceReq, serviceReqPrintRaw);

                        HIS.Desktop.ADO.AssignBloodADO assignBloodADO = new HIS.Desktop.ADO.AssignBloodADO(data.TDL_TREATMENT_ID ?? 0, 0, 0);
                        assignBloodADO.PatientDob = data.TDL_PATIENT_DOB;
                        assignBloodADO.DgProcessDataResult = FillDataApterSave;
                        assignBloodADO.PatientName = data.TDL_PATIENT_NAME;
                        assignBloodADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                        List<object> sendObj = new List<object>() { assignBloodADO, serviceReq };
                        CallModule("HIS.Desktop.Plugins.HisAssignBlood", sendObj);
                    }
                    else
                    {
                        AssignServiceEditADO assignServiceEditADO = new AssignServiceEditADO(data.SERVICE_REQ_ID ?? 0, data.TDL_INTRUCTION_TIME, (HIS.Desktop.Common.RefeshReference)RefreshClickTab2);
                        List<object> sendObj = new List<object>() { assignServiceEditADO };
                        CallModule("HIS.Desktop.Plugins.AssignServiceEdit", sendObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemServiceReqUseEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var rowData = treeListServiceReq.GetDataRecordByNode(treeListServiceReq.FocusedNode) as TreeSereServADO;
                if (rowData == null)
                {
                    return;
                }
                WaitingManager.Show();
                MOS.SDO.TemporaryServiceReqSDO sdo = new MOS.SDO.TemporaryServiceReqSDO();
                sdo.ServiceReqId = rowData.SERVICE_REQ_ID ?? 0;
                sdo.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime) ?? 0;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>(UriStores.HIS_SERVICE_REQ_UPDATE_TEMPORARY_PRES, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    LoadDataSS(true);
                    isSearch = false;
                }

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        #region public method
        public void RefreshClick()
        {
            try
            {
                WaitingManager.Show();
                isSearch = true;
                //backgroundWorker1.RunWorkerAsync();
                LoadDataSS(true);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void RefreshClickTab2()
        {
            try
            {
                WaitingManager.Show();
                //backgroundWorker2.RunWorkerAsync();
                LoadDataSSTab2(true);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void CallModule(string moduleLink, List<object> data)
        {
            try
            {
                CallModule callModule = new CallModule(moduleLink, currentModule.RoomId, currentModule.RoomTypeId, data);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_SERVICE_REQ GetServiceReqForPrint(long serviceReqId)
        {
            V_HIS_SERVICE_REQ result = new V_HIS_SERVICE_REQ();
            try
            {
                if (serviceReqId > 0)
                {
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.ID = serviceReqId;
                    serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var serviceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (serviceReq != null && serviceReq.Count > 0)
                    {
                        result = serviceReq.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new V_HIS_SERVICE_REQ();
            }
            return result;
        }

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
