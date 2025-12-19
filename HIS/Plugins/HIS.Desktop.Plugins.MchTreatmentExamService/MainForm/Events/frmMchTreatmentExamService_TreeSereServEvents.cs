using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.TreeSereServ7;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region TreeSereServ Event Handlers

        private void treeSereServ_GetSelectImage(SereServADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (!e.Node.HasChildren && data.Tein == null && data.Baby == null)
                    {
                        if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
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
                        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && data.RECEIVE_SAMPLE_TIME != null)
                            {
                                e.NodeImageIndex = 2;
                            }
                            else
                                e.NodeImageIndex = 1;
                        }
                        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
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
                    if (data.Tein != null)
                    {
                        e.NodeImageIndex = -1;
                    }
                    else if (data.Baby != null)
                    {
                        e.NodeImageIndex = 7;
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
                    if (data.Baby != null && data.Baby.ID > 0)
                    {
                        CopyBabyDataToChildTab(data.Baby);
                        return;
                    }
                    if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                    {
                        WaitingManager.Show();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereServTein").FirstOrDefault();
                        if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SereServTein");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(data.ID);
                            listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            WaitingManager.Hide();
                            ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
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
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                        WaitingManager.Hide();
                        ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
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
                            if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                            {
                                e.RepositoryItem = repositoryItemButton__Send;
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

        #endregion
    }
}
