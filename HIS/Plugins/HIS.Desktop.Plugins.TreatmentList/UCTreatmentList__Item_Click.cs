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
using DevExpress.XtraEditors.Controls;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraBars.Docking2010.Views;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.Plugins.TreatmentList.Config;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        /// <summary>
        /// Chi dinh dich vu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AssignService'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    AssignServiceADO AssignServiceADO = new AssignServiceADO(row.ID, 0, 0);
                    AssignServiceADO.TreatmentId = row.ID;
                    AssignServiceADO.PatientDob = row
                                     .TDL_PATIENT_DOB;
                    AssignServiceADO.PatientName = row.TDL_PATIENT_NAME;
                    AssignServiceADO.GenderName = row.TDL_PATIENT_GENDER_NAME;
                    AssignServiceADO.IsAutoEnableEmergency = true;
                    listArgs.Add(AssignServiceADO);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignService", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Danh sach yeu cau
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT thistreatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(thistreatment, row);
                    listArgs.Add(thistreatment);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceReqList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnServiceReqList_Click(V_HIS_TREATMENT_4 data)
        {
            try
            {
                var row = data;
                if (row != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT thistreatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(thistreatment, row);
                    listArgs.Add(thistreatment);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceReqList", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sua thong tin Dieu tri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnFixTreatment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentIcdEdit").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentIcdEdit'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add((RefeshReference)BtnSearch);
                    listArgs.Add(row.ID);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentIcdEdit", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnServicePackgeView_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServicePackageView").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServicePackageView'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.ID);
                    //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServicePackageView", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Danh sach in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItemButtonEdit_Print_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (barManagerPrint == null)
                {
                    barManagerPrint = new DevExpress.XtraBars.BarManager();
                }
                barManagerPrint.Form = this;
                LoadPrintTreatment(barManagerPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Print_ButtonClick(V_HIS_TREATMENT_4 data)
        {
            try
            {
                if (barManagerPrint == null)
                {
                    barManagerPrint = new DevExpress.XtraBars.BarManager();
                }
                barManagerPrint.Form = this;
                LoadPrintTreatment(barManagerPrint, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ket thuc dieu tri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();
                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((row.IS_PAUSE ?? -1).ToString());
                if (row != null)
                {
                    if (status_ispause == 1)
                    {
                        MessageManager.Show("Bệnh nhân đã kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(row.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentFinish").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentFinish'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();

                    TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();

                    TreatmentLogADO.RoomId = currentModule.RoomId;
                    TreatmentLogADO.TreatmentId = row.ID;
                    listArgs.Add(TreatmentLogADO);
                    listArgs.Add((RefeshReference)BtnSearch);
                    //    ////listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    //else
                    //{
                    //    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentFinish", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnFinish_Click(MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4 data)
        {
            try
            {
                var row = data;
                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((row.IS_PAUSE ?? -1).ToString());
                if (row != null)
                {
                    if (status_ispause == 1)
                    {
                        MessageManager.Show("Bệnh nhân đã kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(row.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    WaitingManager.Show();
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentFinish").FirstOrDefault();
                    //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentFinish'");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();

                    TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();

                    TreatmentLogADO.RoomId = currentModule.RoomId;
                    TreatmentLogADO.TreatmentId = row.ID;
                    listArgs.Add(TreatmentLogADO);
                    listArgs.Add((RefeshReference)BtnSearch);
                    //    ////listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    //else
                    //{
                    //    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    //}
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentFinish", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dong thoi gian
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnTimeLine_btnClick(object sender, ButtonPressedEventArgs e)
        {

        }

        /// <summary>
        /// Sua thong tin benh nhan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnEditTreatment_btnClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();

                if (row != null)
                {
                    //CommonParam param = new CommonParam();
                    //MOS.Filter.HisTreatmentViewFilter treatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                    //treatmentFilter.ID = row.ID;
                    //var currentTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param);
                    //if (currentTreatment != null && currentTreatment.Count > 0)
                    //{
                    //List<object> listArgs = new List<object>();
                    //listArgs.Add(currentTreatment.FirstOrDefault());
                    //listArgs.Add((DelegateSelectData)Search);
                    //HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PatientUpdate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PatientUpdate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.PatientUpdate'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.PATIENT_ID);
                        listArgs.Add(row.ID);
                        listArgs.Add((DelegateSelectData)Search);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Search(object data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        if (btnFind.Enabled)
                        {
                            btnFind_Click(null, null);
                        }
                        HIS.Desktop.Plugins.Library.PrintPatientUpdate.PrintPatientUpdateProcessor rm = new Library.PrintPatientUpdate.PrintPatientUpdateProcessor(data, this.currentModule.RoomId);
                        rm.Print();
                    }));
                }
                else
                {
                    if (btnFind.Enabled)
                    {
                        btnFind_Click(null, null);
                    }
                    HIS.Desktop.Plugins.Library.PrintPatientUpdate.PrintPatientUpdateProcessor rm = new Library.PrintPatientUpdate.PrintPatientUpdateProcessor(data, this.currentModule.RoomId);
                    rm.Print();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        private void repositoryItembtnEditTreatment_btnClick(V_HIS_TREATMENT_4 data)
        {
            try
            {
                var row = data;

                if (row != null)
                {
                    //CommonParam param = new CommonParam();
                    //MOS.Filter.HisTreatmentViewFilter treatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                    //treatmentFilter.ID = row.ID;
                    //var currentTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param);
                    //if (currentTreatment != null && currentTreatment.Count > 0)
                    //{
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.PATIENT_ID);
                    listArgs.Add(row.ID);
                    listArgs.Add((DelegateSelectData)Search);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PatientUpdate", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnActionHurt_btnClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private void repositoryItembtnMergePatient_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Bang ke thanh toan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnPaySereServ_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();
                if (row != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                    //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    //Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    //moduleData.RoomId = currentModule.RoomId;
                    //moduleData.RoomTypeId = currentModule.RoomTypeId;
                    listArgs.Add(row.ID);
                    //    ////listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Bordereau", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnPaySereServ_Click(V_HIS_TREATMENT_4 data)
        {
            try
            {
                var row = data;
                if (row != null)
                {
                    //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                    //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                    //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    //{
                    List<object> listArgs = new List<object>();
                    //Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    //moduleData.RoomId = currentModule.RoomId;
                    //moduleData.RoomTypeId = currentModule.RoomTypeId;
                    listArgs.Add(row.ID);
                    //    ////listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //    ((Form)extenceInstance).ShowDialog();
                    //}
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Bordereau", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// mo dieu tri 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItembtnUnifinish_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)gridViewtreatmentList.GetFocusedRow();
                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((row.IS_PAUSE ?? -1).ToString());
                if (row != null)
                {
                    if (status_ispause != 1)
                    {
                        MessageManager.Show("Bệnh nhân chưa kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(row.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    string mess = null;

                    bool IsContinue = true;
                    if (HisConfigCFG.IsRequiredReasonWhenOpenTreatment)
                    {
                        frmReasonOpenTreatment frm = new frmReasonOpenTreatment(reason =>
                        {
                        if (!String.IsNullOrEmpty(reason))
                        {
                            mess = reason;
                        }
                        }, IsClose =>
                        {
                            IsContinue = !IsClose;
                        }
                        );
                        frm.ShowDialog();
                    }
                    if (!IsContinue)
                        return;
                    WaitingManager.Show();
                    MOS.SDO.HisTreatmentUnfinishSDO sdo = new MOS.SDO.HisTreatmentUnfinishSDO();
                    sdo.Reason = mess;
                    sdo.treatmentId = row.ID;
                    bool unFinishTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("/api/HisTreatment/Unfinish", ApiConsumers.MosConsumer, sdo, param);
                    //CloseTreatmentProcessor.TreatmentUnFinish(, param);
                    WaitingManager.Hide();
                    if (unFinishTreatment == true)
                    {
                        success = true;
                        FillDataToGrid();
                    }

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnUnifinish_Click(V_HIS_TREATMENT_4 data)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var row = data;
                short status_ispause = Inventec.Common.TypeConvert.Parse.ToInt16((row.IS_PAUSE ?? -1).ToString());
                if (row != null)
                {
                    if (status_ispause != 1)
                    {
                        MessageManager.Show("Bệnh nhân chưa kết thúc điều trị");
                        return;
                    }
                    if (!IsStayingDepartment(row.DEPARTMENT_IDS))
                    {
                        MessageManager.Show("Bệnh nhân đang ở khoa khác");
                        return;
                    }
                    string mess = null;
                    bool IsContinue = true;
                    if (HisConfigCFG.IsRequiredReasonWhenOpenTreatment)
                    {
                        frmReasonOpenTreatment frm = new frmReasonOpenTreatment(reason =>
                        {
                            if (!String.IsNullOrEmpty(reason))
                            {
                                mess = reason;
                            }
                        }, IsClose =>
                        {
                            IsContinue = !IsClose;
                        }
                        );
                        frm.ShowDialog();
                    }
                    if (!IsContinue)
                        return;
                    WaitingManager.Show();
                    MOS.SDO.HisTreatmentUnfinishSDO sdo = new MOS.SDO.HisTreatmentUnfinishSDO();
                    sdo.Reason = mess;
                    sdo.treatmentId = row.ID;
                    bool unFinishTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("/api/HisTreatment/Unfinish", ApiConsumers.MosConsumer, sdo, param);
                    //CloseTreatmentProcessor.TreatmentUnFinish(, param);
                    WaitingManager.Hide();
                    if (unFinishTreatment == true)
                    {
                        success = true;
                        FillDataToGrid();
                    }

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
