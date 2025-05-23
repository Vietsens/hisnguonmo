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
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.Plugins.ApprovalExamSpecialist;
using DevExpress.Utils;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.ADO;
using MOS.SDO;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraTreeList;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using System.Resources;
using System.Reflection;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.Key;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist
{
    public partial class UCTreeListService : UserControl
    {
        Action<ADO.SereServADO> EditButton_Click;
        Action<ADO.SereServADO> EditEnableButton_Click;
        Action<ADO.SereServADO> DeleteEnableButton_Click;
        ImageCollection imageCollection;
        long wkRoomId = 0, wkRoomTypeId = 0;
        DHisSereServ2 TreeClickData;
        bool IsExpand;
        DHisSereServ2 _SereServADORightMouseClick;
        L_HIS_TREATMENT_BED_ROOM RowCellClickBedRoom;
        V_HIS_SPECIALIST_EXAM currentVHisSpecialist;

        HIS_EXP_MEST currentPrescription;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<SereServADO> dataSource = null;
        List<HIS_SERVICE_REQ> listDataServiceReq = null;
        public UCTreeListService(ImageCollection image, Inventec.Desktop.Common.Modules.Module _currentModule)
        {
            InitializeComponent();
            try
            {
                imageCollection = image;
                this.currentModule = _currentModule;
                this.wkRoomId = currentModule != null ? currentModule.RoomId : 0;
                this.wkRoomTypeId = currentModule != null ? currentModule.RoomTypeId : 0;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UCTreeListService_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                //treeSereServ.SelectImageList = imageCollection;
                treeSereServ.StateImageList = imageCollection;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        // cái này à a
        public void ReLoad(Action<ADO.SereServADO> editClick, List<SereServADO> SereServADOs, V_HIS_SPECIALIST_EXAM currentVHisSpecialistExam)
        {
            try
            {
                currentVHisSpecialist = currentVHisSpecialistExam;
                EditButton_Click = editClick;
                treeSereServ.BeginUpdate();
                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    long check = SereServADOs.Where(o => o.IS_RATION == 1).Count();

                    if (check == SereServADOs.Where(o => o.child == 4).Count())
                    {
                        isRation.Caption = Resources.ResourceMessage.MucAn;
                    }
                    else if (check == 0)
                    {
                        isRation.Caption = Resources.ResourceMessage.DTTT;
                        isRation.ToolTip = Resources.ResourceMessage.ToolTipDTTT;
                    }
                    else
                    {
                        isRation.Caption = Resources.ResourceMessage.MucAnDTTT;
                        isRation.ToolTip = Resources.ResourceMessage.ToolTipMucAnDTTT;
                    }
                    dataSource = SereServADOs;
                    treeSereServ.DataSource = new BindingList<SereServADO>(SereServADOs);

                    treeSereServ.ExpandAll();
                    Expand(true);
                }
                else
                {
                    treeSereServ.DataSource = null;
                }
                treeSereServ.EndUpdate();
                try
                {
                    this.treeSereServ.BestFitColumns();
                    this.tc_ServiceCode.BestFit();
                    this.treeSereServ.OptionsView.ShowColumns = false;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Expand(bool isExpand)
        {
            try
            {
                if (isExpand)
                {
                    treeSereServ.ExpandAll();
                }
                else
                {
                    treeSereServ.BeginUpdate();
                    treeSereServ.CollapseAll();
                    treeSereServ.EndUpdate();
                }
                IsExpand = isExpand;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool getExpand()
        {
            return IsExpand;
        }

        public void setExpand(bool stt)
        {
            IsExpand = stt;
        }

        private void treeSereServ_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(e.Node);
                if (data != null && data.SERVICE_REQ_STT_ID > 0)
                {
                    if (e.Column.FieldName == "Delete")
                    {
                        if (e.Node.HasChildren)
                        {
                            if (data.IsEnableDelete)
                                e.RepositoryItem = rep_btnDelete_Enable;
                            else
                                e.RepositoryItem = rep_btnDelete_Disable;
                        }

                    }
                    else if (e.Column.FieldName == "Edit")
                    {
                        if (e.Node.HasChildren)
                        {
                            if (data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                            {
                                if (data.IsEnableEdit)
                                    e.RepositoryItem = rep_btnEdit_Enable;
                                else
                                    e.RepositoryItem = rep_btnEdit_Disable;

                            }

                        }
                    }

                }

                if (data != null && !e.Node.HasChildren)
                {
                    TreeClickData = data;
                    if (e.Column.FieldName == "SendTestServiceReq")
                    {
                        if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)//Xét nghiệm
                            e.RepositoryItem = repositoryItemButton__Send;
                    }
                    else if (e.Column.FieldName == "MEDI_USED")
                    {
                        if (data.IS_USED == 1)
                        {
                            e.RepositoryItem = repositoryItemButton_IsUse;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (data != null && data.SERVICE_REQ_STT_ID > 0)
                    {
                        if (e.Node.HasChildren)
                        {
                            if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                                && data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                                && data.SAMPLE_TIME != null)
                            {
                                e.NodeImageIndex = 5;
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)//Chưa xử lý
                            {
                                e.NodeImageIndex = 0;
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)//Đã xử lý
                            {
                                if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && data.RECEIVE_SAMPLE_TIME != null)
                                {
                                    e.NodeImageIndex = 2;
                                }
                                else
                                    e.NodeImageIndex = 1;
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)//Hoàn thành
                            {
                                e.NodeImageIndex = 4;
                            }
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                    {
                        e.NodeImageIndex = -1;
                    }
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(e.Node);
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        || data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        || data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                    {
                        e.NodeImageIndex = -1;
                    }
                    else
                    {
                        if ((HisConfigCFG.IsShowResultWhenReqComplete == "1" && data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) || (HisConfigCFG.IsShowResultWhenReqComplete != "1" && HisConfigCFG.IsShowResultWhenReqComplete != "2") || (HisConfigCFG.IsShowResultWhenReqComplete == "2" && (data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT))))
                            e.NodeImageIndex = 6;
                        else
                            e.NodeImageIndex = -1;
                    }
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Node.HasChildren)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                    else if (data.IS_NO_EXECUTE == 1)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                if (tree != null)
                {
                    Point pt = tree.PointToClient(MousePosition);
                    TreeListHitInfo hitInfo = tree.CalcHitInfo(e.Point);
                    if (hitInfo != null && (hitInfo.HitInfoType == HitInfoType.Row
                        || hitInfo.HitInfoType == HitInfoType.Cell))
                    {
                        e.Menu.Items.Clear();
                        tree.FocusedNode = hitInfo.Node;
                        var data = (SereServADO)treeSereServ.GetDataRecordByNode(hitInfo.Node);
                        if (data != null)
                        {
                            foreach (var menu in this.menuItem(data))
                            {
                                e.Menu.Items.Add(menu);
                            }
                            e.Menu.Show(pt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_StateImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(e.Node);
                if (data != null && data.SERE_SERV_ID.HasValue)
                {
                    if ((HisConfigCFG.IsShowResultWhenReqComplete == "1" && data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) || (HisConfigCFG.IsShowResultWhenReqComplete != "1" && HisConfigCFG.IsShowResultWhenReqComplete != "2") || (HisConfigCFG.IsShowResultWhenReqComplete == "2" && (data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN || (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT))))
                    {
                        if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                        {
                            if (data.IS_ANTIBIOTIC_RESISTANCE == 1)
                            {
                                //mở module SereServTeinBacterium
                                WaitingManager.Show();
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereServTeinBacterium").FirstOrDefault();
                                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SereServTeinBacterium");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(data.SERE_SERV_ID);
                                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                    WaitingManager.Hide();
                                    ((Form)extenceInstance).ShowDialog();
                                }
                            }
                            else
                            {
                                //mở module chỉ số sereServTein
                                WaitingManager.Show();
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereServTein").FirstOrDefault();
                                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SereServTein");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(data.SERE_SERV_ID);
                                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                    WaitingManager.Hide();
                                    ((Form)extenceInstance).ShowDialog();
                                }
                            }
                        }
                        else if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                        {
                            WaitingManager.Show();
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExamServiceReqResult").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ExamServiceReqResult'");
                            if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ExamServiceReqResult' is not plugins");
                            List<object> listArgs = new List<object>();
                            listArgs.Add(data.SERE_SERV_ID);
                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId), listArgs);
                            if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            WaitingManager.Show();
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqResultView").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqResultView'");
                            if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqResultView' is not plugins");
                            List<object> listArgs = new List<object>();
                            listArgs.Add(data.SERE_SERV_ID);
                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId), listArgs);
                            if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<DevExpress.Utils.Menu.DXMenuItem> menuItem(DHisSereServ2 data)
        {
            List<DevExpress.Utils.Menu.DXMenuItem> dXmenuItem = new List<DevExpress.Utils.Menu.DXMenuItem>();
            try
            {
                _SereServADORightMouseClick = new DHisSereServ2();
                var dXmenu = new DevExpress.Utils.Menu.DXMenuItem();
                if (data != null && data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                {
                    _SereServADORightMouseClick = data;
                    dXmenu.Image = imageCollection1.Images[0];
                    dXmenu.Click += Event_RightMouseClick;
                    //dXmenu.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__MOUSE_RIGHT__KE_DON_DUOC", Base.ResourceLangManager.LanguageUCBedRoomPartial, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    dXmenuItem.Add(dXmenu);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dXmenuItem;

        }

        private void Event_RightMouseClick(object sender, EventArgs e)
        {
            try
            {
                if (_SereServADORightMouseClick != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, _SereServADORightMouseClick);

                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(RowCellClickBedRoom.TREATMENT_ID,
                            0,
                            _SereServADORightMouseClick.SERVICE_REQ_ID ?? 0,
                            sereServInput);

                        assignServiceADO.PatientDob = RowCellClickBedRoom.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = RowCellClickBedRoom.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = RowCellClickBedRoom.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        assignServiceADO.IsAutoCheckExpend = true;
                        listArgs.Add(assignServiceADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Send_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReqDTO = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
                var currentSS = (SereServADO)treeSereServ.GetDataRecordByNode(treeSereServ.FocusedNode);
                if (currentSS != null)
                {
                    var resend = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_TEST_SERVICE_REQ_RESEND, ApiConsumers.MosConsumer, currentSS.SERVICE_REQ_ID, param);
                    if (resend)
                    {
                        success = true;
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController2_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //if (e != null && e.SelectedControl is DevExpress.XtraTreeList.TreeList)// trvService)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("if (e != null && e.SelectedControl is DevExpress.XtraTreeList.TreeList)");
                //    DevExpress.Utils.ToolTipControlInfo info = null;
                //    TreeListHitInfo hi = treeSereServ.CalcHitInfo(e.ControlMousePosition);
                //    if (hi != null && hi.Node != null)
                //    {
                //        Inventec.Common.Logging.LogSystem.Warn(" if (hi != null && hi.Node != null)");

                //        var o = hi.Node;
                //        //if (hi.HitInfoType == HitInfoType.SelectImage)
                //        //{
                //        //    string text = "";
                //        //    var data = (SereServADO)treeSereServ.GetDataRecordByNode(o);
                //        //    if (data != null)
                //        //    {
                //        //        if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                //        //        {
                //        //            if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && data.SAMPLE_TIME != null)
                //        //            {
                //        //                text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__DA_LAY_MAU", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //        //            }
                //        //            else
                //        //                text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__CHUA_XU_LY", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //        //        }
                //        //        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                //        //        {
                //        //            if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && data.RECEIVE_SAMPLE_TIME != null)
                //        //            {
                //        //                text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__DA_NHAN_MAU", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //        //            }
                //        //            else
                //        //                text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__DANG_XU_LY", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //        //        }
                //        //        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                //        //        {
                //        //            text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__HOAN_THANH", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //        //        }

                //        //    }
                //        //    info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                //        //    e.Info = info;
                //        //}
                //        //else 
                //        if (hi.HitInfoType == HitInfoType.StateImage)
                //        {
                //            Inventec.Common.Logging.LogSystem.Warn("HitInfoType");


                //            var data = (SereServADO)treeSereServ.GetDataRecordByNode(o);
                //            if (data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                //                return;
                //            string text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__KET_QUA_XET_NGHIEM", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //            info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                //            e.Info = info;
                //        }
                //        if (hi.Column.FieldName == "NOTE_ADO")
                //        {
                //            string textx = "";
                //            var datax = (SereServADO)treeSereServ.GetDataRecordByNode(o);
                //            if (datax != null)
                //            {
                //                if (o.HasChildren && !string.IsNullOrEmpty(datax.NOTE_ADO))
                //                {
                //                    textx = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__THOI_GIAN_Y_LENH", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                //                }
                //                if (!o.HasChildren)
                //                {
                //                    textx = datax.NOTE_ADO;
                //                }
                //            }
                //            info = new DevExpress.Utils.ToolTipControlInfo(o, textx);
                //            e.Info = info;
                //        }
                //        //if (hi.Column.FieldName == "Edit")
                //        //{
                //        //    string textx = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__SUA", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                //        //    info = new DevExpress.Utils.ToolTipControlInfo(o, textx);
                //        //    e.Info = info;
                //        //}
                //        //if (hi.Column.FieldName == "Delete")
                //        //{
                //        //    string textx = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__XOA", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                //        //    info = new DevExpress.Utils.ToolTipControlInfo(o, textx);
                //        //    e.Info = info;
                //        //}
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_TaoThuHoi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReqDTO = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
                var currentSS = (SereServADO)treeSereServ.GetDataRecordByNode(treeSereServ.FocusedNode);
                CommonParam paramCommon = new CommonParam();
                HIS_EXP_MEST expMest = null;
                if (currentSS != null)
                {
                    HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.SERVICE_REQ_ID = currentSS.SERVICE_REQ_ID;
                    var result = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (result != null && result.Count > 0)
                    {
                        currentPrescription = result.FirstOrDefault();
                        expMest = result.FirstOrDefault();
                    }

                    if (this.currentPrescription != null &&
                    (
                    currentSS.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                    currentSS.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT ||
                    currentSS.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM))
                    {
                        if (currentPrescription.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            List<object> sendObj = new List<object>() { currentPrescription.ID };

                            if (currentPrescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)
                            {
                                CallModule("HIS.Desktop.Plugins.MobaPrescriptionCreate", sendObj);
                            }
                            else if (currentPrescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                            {
                                CallModule("HIS.Desktop.Plugins.MobaCabinetCreate", sendObj);
                            }
                            else if (currentPrescription.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                CallModule("HIS.Desktop.Plugins.MobaBloodCreate", sendObj);
                            }
                            else
                            {
                                MessageManager.Show("Tài khoản không có quyền thực hiện chức năng");
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

        private void rep_btnEdit_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(treeSereServ.FocusedNode);
                if (data != null && this.EditEnableButton_Click != null)
                {
                    this.EditEnableButton_Click(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rep_btnDelete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(treeSereServ.FocusedNode);
                if (data != null && this.EditEnableButton_Click != null)
                {
                    this.DeleteEnableButton_Click(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_Click(object sender, EventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                var data = (SereServADO)treeSereServ.GetDataRecordByNode(hi.Node);
                if (data != null && this.EditButton_Click != null)
                {
                    this.EditButton_Click(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCTreeListService
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__UCTreeListService = new ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Lang", typeof(UCTreeListService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreeListService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.treeSereServ.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCTreeListService.treeSereServ.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_SendTestServiceReq.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_SendTestServiceReq.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_Edit.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_Edit.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_Delete.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_MediUsed.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_MediUsed.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.isRation.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.isRation.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_Number.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_Number.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_NoteAdo.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_NoteAdo.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.tc_RequestDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_RequestDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                //qtcode
                this.tc_TdlMedicineConcentra.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_TdlMedicineConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                //qtcode
                this.repositoryItemButton__Send.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__GUI_LAI_YEU_CAU_XET_NGHIEM", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.repositoryItemButton__Send__Disable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__GUI_LAI_YEU_CAU_XET_NGHIEM", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.repositoryItemButtonEdit_TaoThuHoi.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__TAO_THU_HOI", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.repositoryItemButton_IsUse.Buttons[0].ToolTip = Resources.ResourceMessage.ThuocVtBNDaDung;
                this.rep_btnEdit_Enable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__SUA", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.rep_btnEdit_Disable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__SUA", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.rep_btnDelete_Enable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__XOA", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());
                this.rep_btnDelete_Disable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__XOA", Resources.ResourceLanguageManager.LanguageResource__UCTreeListService, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool IsCheckDepartmentTran()
        {
            bool result = true;
            try
            {
                if (RowCellClickBedRoom != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisDepartmentTranFilter _departmentTranFilter = new HisDepartmentTranFilter();
                    _departmentTranFilter.TREATMENT_ID = RowCellClickBedRoom.ID;
                    _departmentTranFilter.DEPARTMENT_ID = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId).DEPARTMENT_ID;
                    var _DepartmentTrans = new BackendAdapter(param).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, _departmentTranFilter, param);
                    if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
                    {
                        var department = _DepartmentTrans.OrderByDescending(o => o.DEPARTMENT_IN_TIME ?? 0).ToList().FirstOrDefault();
                        if (department != null)
                        {
                            if (department.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Khoa {0} đã khóa chi phí không cho phép thực hiện chỉ định, kê đơn", BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == department.DEPARTMENT_ID).DEPARTMENT_NAME),
                       HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm),
                       MessageBoxButtons.OK) == DialogResult.OK)
                                return false;
                        }
                    }
                    else
                        result = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void DisposeData()
        {
            try
            {
                listDataServiceReq = null;
                dataSource = null;
                currentModule = null;
                currentPrescription = null;
                RowCellClickBedRoom = null;
                _SereServADORightMouseClick = null;
                IsExpand = false;
                TreeClickData = null;
                wkRoomId = 0;
                imageCollection = null;
                DeleteEnableButton_Click = null;
                EditEnableButton_Click = null;
                EditButton_Click = null;
                this.treeSereServ.GetStateImage -= new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeSereServ_GetStateImage);
                this.treeSereServ.GetSelectImage -= new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.treeSereServ_GetSelectImage);
                this.treeSereServ.StateImageClick -= new DevExpress.XtraTreeList.NodeClickEventHandler(this.treeSereServ_StateImageClick);
                this.treeSereServ.CustomNodeCellEdit -= new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.treeSereServ_CustomNodeCellEdit);
                this.treeSereServ.NodeCellStyle -= new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeSereServ_NodeCellStyle);
                this.treeSereServ.PopupMenuShowing -= new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeSereServ_PopupMenuShowing);
                this.treeSereServ.Click -= new System.EventHandler(this.treeSereServ_Click);
                this.repositoryItemButton__Send.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__Send_ButtonClick);
                this.repositoryItemButtonEdit_TaoThuHoi.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit_TaoThuHoi_ButtonClick);
                this.rep_btnEdit_Enable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnEdit_Enable_ButtonClick);
                this.rep_btnDelete_Enable.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnDelete_Enable_ButtonClick);
                this.toolTipController2.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController2_GetActiveObjectInfo);
                this.Load -= new System.EventHandler(this.UCTreeListService_Load);
                rep_btnDelete_Disable = null;
                rep_btnDelete_Enable = null;
                rep_btnEdit_Disable = null;
                rep_btnEdit_Enable = null;
                tc_Delete = null;
                tc_Edit = null;
                isRation = null;
                treeListColumn2 = null;
                treeListColumn1 = null;
                toolTipController2 = null;
                repositoryItemButtonEdit_TaoThuHoi = null;
                repositoryItemButton_IsUse = null;
                repositoryItemButton__Send__Disable = null;
                repositoryItemButton__Send = null;
                imageCollection1 = null;
                tc_RequestDepartmentName = null;
                //qtcode
                tc_TdlMedicineConcentra = null;
                //qtcode
                tc_NoteAdo = null;
                tc_Number = null;
                tc_ServiceName = null;
                tc_ServiceCode = null;
                tc_MediUsed = null;
                tc_SendTestServiceReq = null;
                layoutControlItem1 = null;
                treeSereServ = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
