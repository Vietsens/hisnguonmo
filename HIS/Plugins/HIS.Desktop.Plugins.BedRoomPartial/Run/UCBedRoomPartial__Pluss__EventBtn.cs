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
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.BedRoomPartial.Base;
using DevExpress.Utils.Menu;
using HIS.Desktop.ModuleExt;
using System.Reflection;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    public partial class UCBedRoomPartial : UserControlBase
    {
        private void btnChiDinhGPBL_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnChiDinhDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null && IsCheckDepartmentTran())
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        HIS.Desktop.ADO.AssignServiceADO assignServiceADO = new HIS.Desktop.ADO.AssignServiceADO(RowCellClickBedRoom.TREATMENT_ID, 0, 0);
                        assignServiceADO.PatientDob = RowCellClickBedRoom.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = RowCellClickBedRoom.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.OpenFromBedRoomPartial = true;
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        assignServiceADO.FilterFromBedRoomPartiral = treatFilter;
                        listArgs.Add(assignServiceADO);                      

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId));


                        if (!IsApplyFormClosingOption(moduleData.ModuleLink))
                        {
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            ((Form)extenceInstance).ShowDialog();

                        }
                        else
                        {
                            if (lstModuleLinkApply.FirstOrDefault(o => o == moduleData.ModuleLink) != null)
                            {
                                if (GlobalVariables.FormAssignService != null)
                                {
                                    GlobalVariables.FormAssignService.WindowState = FormWindowState.Maximized;
                                    GlobalVariables.FormAssignService.ShowInTaskbar = true;
                                    Type classType = GlobalVariables.FormAssignService.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ReloadModuleByInputData");
                                    methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { this.currentModule, assignServiceADO });
                                    Inventec.Common.Logging.LogSystem.Error("CASE 2 _START");
                                    GlobalVariables.FormAssignService.Activate();
                                    Inventec.Common.Logging.LogSystem.Error("CASE 2 _END");
                                }
                                else
                                {
                                    GlobalVariables.FormAssignService = (Form)PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, wkRoomId, wkRoomTypeId), listArgs);
                                    GlobalVariables.FormAssignService.ShowInTaskbar = true;
                                    if (GlobalVariables.FormAssignService == null) throw new ArgumentNullException("moduleData is null");
                                    GlobalVariables.FormAssignService.Show();

                                    Type classType = GlobalVariables.FormAssignService.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ChangeIsUseApplyFormClosingOption");
                                    methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { true });
                                }
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

        private void btnTuTruc_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null && IsCheckDepartmentTran())
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(RowCellClickBedRoom.TREATMENT_ID, 0, 0);
                        assignServiceADO.IsCabinet = true;
                        assignServiceADO.PatientDob = RowCellClickBedRoom.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = RowCellClickBedRoom.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = RowCellClickBedRoom.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        listArgs.Add(assignServiceADO);
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
						var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
						if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
						((Form)extenceInstance).ShowDialog();
					}

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnKeDonThuoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null && IsCheckDepartmentTran())
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(RowCellClickBedRoom.TREATMENT_ID, 0, 0);
                        assignServiceADO.PatientDob = RowCellClickBedRoom.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = RowCellClickBedRoom.TDL_PATIENT_NAME;
                        assignServiceADO.GenderName = RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.TreatmentCode = RowCellClickBedRoom.TREATMENT_CODE;
                        assignServiceADO.TreatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        listArgs.Add(assignServiceADO);
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
						var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
						if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
						((Form)extenceInstance).ShowDialog();
					}

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChiDinhMau_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null && IsCheckDepartmentTran())
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAssignBlood");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignBloodADO assignBloodADO = new HIS.Desktop.ADO.AssignBloodADO(RowCellClickBedRoom.TREATMENT_ID, 0, 0);
                        assignBloodADO.PatientDob = RowCellClickBedRoom.TDL_PATIENT_DOB;
                        assignBloodADO.PatientName = RowCellClickBedRoom.TDL_PATIENT_NAME;
                        assignBloodADO.GenderName = RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME;
                        listArgs.Add(assignBloodADO);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnKetThucDieuTri_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentFinish").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentFinish");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        TreatmentLogADO TreatmentLogADO = new TreatmentLogADO();
                        TreatmentLogADO.TreatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        TreatmentLogADO.RoomId = this.wkRoomId;
                        listArgs.Add(TreatmentLogADO);
                        listArgs.Add((HIS.Desktop.Common.RefeshReference)bbtnSearch);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChamSoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisCareSum").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisCareSum");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);

                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChuyenKhoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransDepartment").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransDepartment'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        TransDepartmentADO transDepartmenADO = new TransDepartmentADO(RowCellClickBedRoom.TREATMENT_ID);
                        transDepartmenADO.TreatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        transDepartmenADO.DepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.wkRoomId).DepartmentId;
                        listArgs.Add(transDepartmenADO);
                        listArgs.Add((RefeshReference)FillDataToGridTreatmentBedRoom);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnYeuCauTamUng_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RequestDeposit'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBangKe_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.Bordereau'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLichSuDieuTri_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentHistory'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        TreatmentHistoryADO currentInput = new TreatmentHistoryADO();
                        currentInput.treatmentId = RowCellClickBedRoom.TREATMENT_ID;
                        currentInput.treatment_code = RowCellClickBedRoom.TREATMENT_CODE;
                        listArgs.Add(currentInput);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHoiChan_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Debate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Debate");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTongHpVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadModuleOpenTab(string moduleLink)
        {
            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == moduleLink).FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + moduleLink);
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    HIS.Desktop.Plugins.BedRoomPartial.Base.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId).ExtensionInfo.Code, RowCellClickBedRoom.TREATMENT_CODE + " - " + RowCellClickBedRoom.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, moduleData);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDanhSachYeuCau_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS_TREATMENT input = new HIS_TREATMENT();
                        input.ID = RowCellClickBedRoom.TREATMENT_ID;
                        input.TREATMENT_CODE = RowCellClickBedRoom.TREATMENT_CODE;
                        listArgs.Add(input);
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnInToDieuTri_Click(object sender, EventArgs e)
        {
            try
            {
                if (RowCellClickBedRoom != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTrackingList").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTrackingList");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(RowCellClickBedRoom.TREATMENT_ID);
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        SetTreatmentBedRoomFilter(ref treatFilter, this.isUseAddedTime);
                        listArgs.Add(treatFilter);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.wkRoomId, this.wkRoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
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
