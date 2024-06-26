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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.TreeSereServ7;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.TreatmentHistory
{
    public partial class frmTreatmentHistory : HIS.Desktop.Utility.FormBase
    {
        private void treeSereServ_GetSelectImage(SereServADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (!e.Node.HasChildren)
                    {
                        if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)//Chưa xử lý
                        {
                            if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && data.SAMPLE_TIME != null)
                            {
                                e.NodeImageIndex = 5;
                            }
                            else
                            {
                                e.NodeImageIndex = 0;
                            }
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_GetStateImage(SereServADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        e.NodeImageIndex = -1;
                    }
                    else if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        e.NodeImageIndex = -1;
                    }
                    else
                    {
                        e.NodeImageIndex = 6;
                    }
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_StateImageClick(SereServADO data)
        {
            try
            {
                if (data != null)
                {
                    if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                    {
                        //mở module chỉ số sereServTein
                        WaitingManager.Show();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereServTein").FirstOrDefault();
                        if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SereServTein");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(data.ID);
                            listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();
                        }
                    }
                    else if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                    {
                        WaitingManager.Show();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExamServiceReqResult").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ExamServiceReqResult'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ExamServiceReqResult' is not plugins");

                        List<object> listArgs = new List<object>();

                        listArgs.Add(data.ID);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
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

                        listArgs.Add(data.ID);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                        WaitingManager.Hide();
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomNodeCellEdit(SereServADO data, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (!e.Node.HasChildren)
                    {
                        if (e.Column.FieldName == "SendTestServiceReq")
                        {
                            if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)//Xét nghiệm
                            {
                                //if (HIS.Desktop.LocalStorage.SdaConfigKey.Config.RocheIsIntergateCFG.ROCHE_IS_INTEGRATE == 1)
                                //{
                                e.RepositoryItem = repositoryItemButton__Send;
                                //}
                                //else
                                //{
                                //    e.RepositoryItem = repositoryItemButton__Send__Disable;
                                //}
                            }
                        }
                        //else if (e.Column.FieldName == "EditServiceReq")
                        //{
                        //    if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                        //        && data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        //        && data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        //        && data.CREATOR.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim())
                        //    {
                        //        e.RepositoryItem = repositoryEditServiceReq__Enable;
                        //    }
                        //    else
                        //    {
                        //        e.RepositoryItem = repositoryEditServiceReq__Disable;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_NodeCellStyle(SereServADO data, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
