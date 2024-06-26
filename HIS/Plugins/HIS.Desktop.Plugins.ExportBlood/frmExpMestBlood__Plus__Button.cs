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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExportBlood.ADO;
using HIS.Desktop.Plugins.ExportBlood.Base;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportBlood
{
    public partial class frmExpMestBlood : Form
    {
        private void repositoryItemBtnAddBlood_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (this.currentBlty == null || this.resultExpMest != null || !dxValidationProvider1.Validate())
                    return;
                var blood = (V_HIS_BLOOD)gridViewBlood.GetFocusedRow();
                if (blood != null)
                {
                    if (blood.BLOOD_TYPE_ID != this.currentBlty.BLOOD_TYPE_ID)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.TuiMauKhongThuocLoaiMauDangChon, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                    }
                    var count = dicBloodAdo.Select(s => s.Value).ToList().Where(o => o.ExpMestBltyId == this.currentBlty.ID).ToList().Count();
                    if (count >= this.currentBlty.AMOUNT)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Base.ResourceMessageManager.SoLuongTuiMauCuaLoaiLonHonSoLuongYeuCau, this.currentBlty.BLOOD_TYPE_NAME), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                    }

                    WaitingManager.Show();
                    VHisBloodADO ado = new VHisBloodADO(blood);
                    if (this.currentBlty.PATIENT_TYPE_ID.HasValue)
                    {
                        HisBloodPatyFilter bloodPatyFilter = new HisBloodPatyFilter();
                        bloodPatyFilter.BLOOD_ID = blood.ID;
                        bloodPatyFilter.PATIENT_TYPE_ID = this.currentBlty.PATIENT_TYPE_ID.Value;
                        var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BLOOD_PATY>>("api/HisBloodPaty/Get", ApiConsumers.MosConsumer, bloodPatyFilter, null);
                        if (datas == null || datas.Count <= 0)
                        {
                            WaitingManager.Hide();
                            MessageManager.Show(String.Format(Base.ResourceMessageManager.TuiMauKhongCoChinhSachGiaChoDoiTuong, this.currentBlty.PATIENT_TYPE_NAME));
                            return;
                        }
                    }
                    ado.PATIENT_TYPE_ID = this.currentBlty.PATIENT_TYPE_ID;
                    ado.PATIENT_TYPE_CODE = this.currentBlty.PATIENT_TYPE_CODE;
                    ado.PATIENT_TYPE_NAME = this.currentBlty.PATIENT_TYPE_NAME;
                    ado.ExpMestBltyId = currentBlty.ID;
                    ado.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
                    dicBloodAdo[ado.ID] = ado;
                    if (dicShowBlood.ContainsKey(ado.ID))
                    {
                        dicShowBlood.Remove(ado.ID);
                    }
                    FillDataToGridBlood();
                    FillDataToGridExpMestBlood();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled || this.dicBloodAdo.Count == 0 || this.ExpMest == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                List<string> bloodTypeCheck = new List<string>();
                foreach (var expMestBlty in listExpMestBlty)
                {
                    var count = dicBloodAdo.Select(o => o.Value).ToList().Where(o => o.ExpMestBltyId == expMestBlty.ID).ToList().Count();
                    if (count <= 0)
                    {
                        bloodTypeCheck.Add(expMestBlty.BLOOD_TYPE_NAME);
                    }
                }
                bool valid = false;
                if (bloodTypeCheck.Count > 0)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessageManager.LoaiMauKhongCoTuiMauNaoTrongDanhSachXuat, String.Join(",", bloodTypeCheck)), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                    valid = true;
                }
                if (!valid && dicBloodAdo.Count < listExpMestBlty.Sum(s => s.AMOUNT))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessageManager.SoLuongTuiMauXuatNhoHonSoLuongYeuCau, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                }

                HisExportBloodSDO data = new HisExportBloodSDO();
                data.ExpMestId = ExpMest.ID;
                data.Bloods = new List<HisBloodSDO>();

                foreach (var dic in dicBloodAdo)
                {
                    HisBloodSDO sdo = new HisBloodSDO();
                    sdo.BloodId = dic.Value.ID;
                    sdo.ExpMestBltyId = dic.Value.ExpMestBltyId;
                    sdo.ExpiredDate = dic.Value.EXPIRED_DATE ?? 0;
                    data.Bloods.Add(sdo);
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(HisRequestUriStore.HIS_EXP_MEST_EXPORTBLOOD, ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    this.btnPrint.Enabled = true;
                    this.btnSave.Enabled = false;
                    this.btnAssignService.Enabled = true;
                    success = true;
                    resultExpMest = rs;
                    ProcessExportSuccess();
                }

                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || ExpMest == null || resultExpMest == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditor.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__EXPORT_BLOOD__MPS000107, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAssignService.Enabled)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignServiceTest").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AssignServiceTest'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AssignServiceTest' is not plugins");

                List<object> listArgs = new List<object>();
                AssignServiceTestADO assignBloodADO = new AssignServiceTestADO(0, 0, 0, null);
                GetTreatmentIdFromResultData(ref assignBloodADO);
                listArgs.Add(assignBloodADO);
                Inventec.Desktop.Common.Modules.Module module = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(module, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                ((Form)extenceInstance).ShowDialog();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAssignService_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAssignService.Enabled)
                btnAssignService_Click(null, null);
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnPrint.Enabled)
                btnPrint_Click(null, null);
        }

        private void repositoryItemBtnDeleteBlood_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewExMestBlood.FocusedRowHandle >= 0)
                {
                    var data = (VHisBloodADO)gridViewExMestBlood.GetFocusedRow();
                    if (data != null)
                    {
                        if (dicBloodAdo.ContainsKey(data.ID))
                        {
                            dicBloodAdo.Remove(data.ID);
                            dicShowBlood[data.ID] = dicCurrentBlood[data.ID];
                        }
                        FillDataToGridBlood();
                        FillDataToGridExpMestBlood();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessExportSuccess()
        {
            try
            {
                gridColumn_Blood_Add.OptionsColumn.AllowEdit = false;
                gridColumn_ExpMestBlood_Delete.OptionsColumn.AllowEdit = false;
                gridColumn_ExpMestBlood_ExpiredDate.OptionsColumn.AllowEdit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                V_HIS_SERVICE_REQ serviceReq = null;
                if (ExpMest.SERVICE_REQ_ID.HasValue)
                {
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.ID = ExpMest.SERVICE_REQ_ID.Value;
                    var hisServiceReqs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, null);
                    if (hisServiceReqs != null && hisServiceReqs.Count() > 0)
                    {
                        serviceReq = hisServiceReqs[0];
                    }
                }

                HisExpMestBloodViewFilter expMestBloodFilter = new HisExpMestBloodViewFilter();
                expMestBloodFilter.EXP_MEST_ID = ExpMest.ID;
                var listExpMestBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMestBloodFilter, null);
                MPS.Processor.Mps000107.PDO.Mps000107PDO mps107Rdo = new MPS.Processor.Mps000107.PDO.Mps000107PDO(serviceReq, listExpMestBlty, listExpMestBlood);
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps107Rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void GetTreatmentIdFromResultData(ref AssignServiceTestADO assignBloodADO)
        {
            try
            {
                long __expMestId = ((resultExpMest != null && resultExpMest.ID > 0) ? resultExpMest.ID : 0);
                HisExpMestViewFilter expFilter = new HisExpMestViewFilter();
                expFilter.ID = __expMestId;
                var listExp = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expFilter, null);
                if (listExp != null && listExp.Count == 1)
                {
                    HisTreatmentView2Filter treatmentView2Filter = new MOS.Filter.HisTreatmentView2Filter();
                    treatmentView2Filter.PATIENT_ID = listExp.First().TDL_PATIENT_ID;
                    var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_2, ApiConsumers.MosConsumer, treatmentView2Filter, null);
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        assignBloodADO.TreatmentId = listTreatment.First().ID;
                        assignBloodADO.GenderName = listTreatment.First().GENDER_NAME;
                        assignBloodADO.PatientDob = (listTreatment.First().DOB);
                        assignBloodADO.PatientName = listTreatment.First().VIR_PATIENT_NAME;
                        assignBloodADO.ExpMestId = __expMestId;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
          try
          {
            checkBtnRefresh = true;
            resetGridLookup();
            fillDataGridViewBlood();
          }
          catch (Exception ex)
          {
            Inventec.Common.Logging.LogSystem.Warn(ex);
          }
        }
    }
}
