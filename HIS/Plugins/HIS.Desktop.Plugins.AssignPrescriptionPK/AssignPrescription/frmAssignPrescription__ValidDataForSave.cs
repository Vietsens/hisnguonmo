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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseICD;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.Library.AlertWarningFee;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// HIS.HIS_ICD_SERVICE.HAS_CHECK. Giá trị:
        ///- 0 (hoặc ko khai báo): Không thay đổi gì, giữ nguyên nghiệp vụ như hiện tại
        ///- 1: Có kiểm tra thuốc/vật tư đã kê có nằm trong danh sách đã được cấu hình (dữ liệu trong HIS_ICD_SERVICE) tương ứng với ICD (căn cứ theo ICD_CODE và ICD_SUB_CODE)của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
        ///- 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu
        /// </summary>
        /// <returns></returns>
        private bool CheckICDService()
        {
            bool result = true;
            try
            {
                List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
                var icdCodeArr = (icdFromUc != null && icdFromUc.Count > 0) ? icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList() : null;
                if (icdCodeArr != null && icdCodeArr.Count > 0)
                {
                    if (HisConfigCFG.icdServiceHasCheck != 0)
                    {
                        var serviceADOs = this.mediMatyTypeADOs.Where(o => o.SERVICE_ID > 0).ToList();
                        List<long> serviceNotConfigIds = GetServiceIdNotIcdService(icdCodeArr, serviceADOs);

                        if (serviceNotConfigIds != null && serviceNotConfigIds.Count > 0)
                        {
                            List<MediMatyTypeADO> serviceNotConfigs = this.mediMatyTypeADOs.Where(o => serviceNotConfigIds.Contains(o.SERVICE_ID)).ToList();
                            string medicineTypeNameStr = "";
                            foreach (var item in serviceNotConfigs)
                            {
                                medicineTypeNameStr += item.MEDICINE_TYPE_NAME + ";";
                            }

                            if (HisConfigCFG.icdServiceHasCheck == 1)
                            {
                                MessageBox.Show(String.Format(ResourceMessage.DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICD, medicineTypeNameStr, "\n\r"), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                            else if (HisConfigCFG.icdServiceHasCheck == 2)
                            {
                                DialogResult myResult = MessageBox.Show(String.Format(ResourceMessage.DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICDBanCoMuonTiepTuc, medicineTypeNameStr, "\n\r"), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (myResult != DialogResult.Yes)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Lấy danh sách thuốc được cấu hình giới hạn
                        string medicineCodeLimit = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignPrescription.MedicineLimitAssign");
                        if (!String.IsNullOrEmpty(medicineCodeLimit))
                        {
                            string[] medicineCodeLimitArr = medicineCodeLimit.Split(',');
                            if (medicineCodeLimitArr != null && medicineCodeLimitArr.Length > 0)
                            {
                                List<long> serviceIdCFGs = lstService
                                    .Where(o => medicineCodeLimitArr.Contains(o.SERVICE_CODE)).Select(p => p.ID).ToList();
                                var serviceADOs = this.mediMatyTypeADOs.Where(o => serviceIdCFGs.Contains(o.SERVICE_ID)).ToList();

                                if (serviceADOs != null && serviceADOs.Count > 0)
                                {
                                    List<long> serviceNotConfigIds = GetServiceIdNotIcdService(icdCodeArr.ToList(), serviceADOs);

                                    if (serviceNotConfigIds != null && serviceNotConfigIds.Count > 0)
                                    {
                                        List<MediMatyTypeADO> serviceNotConfigs = this.mediMatyTypeADOs.Where(o => serviceNotConfigIds.Contains(o.SERVICE_ID)).ToList();
                                        string medicineTypeNameStr = "";
                                        foreach (var item in serviceNotConfigs)
                                        {
                                            medicineTypeNameStr += item.MEDICINE_TYPE_NAME + ";";
                                        }

                                        DialogResult myResult = MessageBox.Show(String.Format(ResourceMessage.DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICDBanCoMuonTiepTuc, medicineTypeNameStr, "\n\r"), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (myResult != DialogResult.Yes)
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (HisConfigCFG.icdServiceHasCheck == 3 || HisConfigCFG.icdServiceHasCheck == 4 || HisConfigCFG.icdServiceHasCheck == 5)
                {
                    //Trong trường hợp key cấu hình "HIS.HIS_ICD_SERVICE.HAS_CHECK" có giá trị 3 thì xử lý:
                    //Khi lưu, nếu tồn tại các thuốc hoặc hoạt chất, mà có các chẩn đoán được thiết lập (trong his_icd_service) ko nằm trong các chẩn đoán (chính + phụ) đã nhập cho bệnh nhân thì hiển thị popup danh sách dịch vụ và các chẩn đoán phù hợp tương ứng. Cho phép người dùng chọn để bổ sung vào chẩn đoán chính hoặc chẩn đoán phụ. Cụ thể:
                    //- Giao diện như hình đính kèm (ICD.png)
                    //- Danh sách chỉ hiển thị các dòng tương ứng với các chẩn đoán mà ko có trong thông tin "chẩn đoán" của y lệnh
                    //- Cột chẩn đoán chính chỉ được chọn 1 dòng (radiobox)
                    //- 1 dòng chỉ được check chọn là "chẩn đoán phụ" hoặc "chẩn đoán chính", ko cho phép chọn cả 2.
                    //- Khi nhấn nút "bổ sung chẩn đoán", thì:
                    //+ Lấy bệnh chính được chọn điền vào thông tin chẩn đoán chính của y lệnh (nếu ko có dòng nào được check "chẩn đoán chính" thì bỏ qua)
                    //+ Lấy tất cả các chẩn đoán phụ được chọn, thực hiện "distinct" và ghép chuỗi phân tách bằng dấu chấm phẩy (;) và điền vào thông tin chẩn đoán phụ của y lệnh (nếu ko có dòng nào được check "chẩn đoán phụ" thì bỏ qua)

                    //Lưu ý:
                    //- Cần check theo cả thuốc và hoạt chất tương ứng với thuốc.
                    //- Có thể tham khảo code ở chức năng chỉ định DVKT

                    //#43511 MM: khi cấu hình HIS.HIS_ICD_SERVICE.HAS_CHECK giá trị cảnh báo của phác đồ là 3 chỉ cần phần bệnh chính hoặc bệnh phụ có 1 icd gắn với phác đồ của thuốc hoặc hoạt chất thì sẽ không đưa ra cảnh báo nữa popup chọn các icd còn lại nữa

                    //TODO
                    List<HIS_ICD_SERVICE> icdServiceByServices = GetServiceNotInByMedicineAndAcIngrInServiceIcd(icdCodeArr, this.mediMatyTypeADOs);
                    //V+: 58249,116732
                    if (icdServiceByServices != null && icdServiceByServices.Count > 0)
                    {
                        isConfirmYes = false;
                        frmMissingIcd frmWaringConfigIcdService = new frmMissingIcd(icdFromUc, mediMatyTypeADOs, this.currentModule, icdServiceByServices, getDataFromMissingIcdDelegate, HisConfigCFG.icdServiceHasCheck == 5, getSkip);
                        frmWaringConfigIcdService.ShowDialog();
                        if (isConfirmYes && HisConfigCFG.icdServiceHasCheck == 5)
                            result = true;
                        else
                            result = false;
                    }
                    //Inventec.Common.Logging.LogSystem.Debug("CheckICDService_____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.icdServiceHasCheck), HisConfigCFG.icdServiceHasCheck)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdServiceByServices), icdServiceByServices)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdCodeArr), icdCodeArr)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isConfirmYes), isConfirmYes)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOs), mediMatyTypeADOs));
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void getSkip(bool obj)
        {
            try
            {
                isConfirmYes = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void getDataFromMissingIcdDelegate(object data, object dataRemove)
        {
            isNotProcessWhileChangedTextSubIcd = true;
            List<MissingIcdADO> missingIcdADOList = new List<MissingIcdADO>();
            List<MissingIcdADO> RemoveIcdADOList = new List<MissingIcdADO>();
            try
            {
                if (data != null && data is List<MissingIcdADO>)
                {
                    missingIcdADOList = (List<MissingIcdADO>)data;
                    if (missingIcdADOList != null && missingIcdADOList.Count > 0)
                    {
                        isConfirmYes = true;
                        var icdMainCheck = missingIcdADOList.FirstOrDefault(o => o.ICD_MAIN_CHECK);
                        if (icdMainCheck != null)
                        {
                            var icdMainData = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == icdMainCheck.ICD_CODE);
                            if (icdMainData != null)
                            {
                                cboIcds.EditValue = icdMainData.ID;
                                txtIcdCode.Text = icdMainData.ICD_CODE;
                                txtIcdMainText.Text = icdMainData.ICD_NAME;
                                if (icdMainData.IS_LATENT_TUBERCULOSIS == 1)
                                    btnLatentTuberCulosis.Enabled = true;
                                else
                                    btnLatentTuberCulosis.Enabled = false;
                            }
                        }

                        var icdCauses = missingIcdADOList.Where(o => o.ICD_CAUSE_CHECK).ToList();
                        if (icdCauses != null && icdCauses.Count > 0)
                        {
                            var icdCauseDatas = this.currentIcds.Where(o => icdCauses.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();

                            if (icdCauseDatas != null && icdCauseDatas.Count > 0)
                            {
                                icdCauseDatas = icdCauseDatas.GroupBy(o => o.ICD_CODE)
                                    .Select(p => p.FirstOrDefault())
                                    .OrderBy(k => k.ICD_CODE).ToList();

                                string icdCausesCodestr = String.Join(";", icdCauseDatas.Select(o => o.ICD_CODE).ToList());
                                string icdCausesstr = String.Join(";", icdCauseDatas.Select(o => o.ICD_NAME).ToList());
                                txtIcdSubCode.Text += ";" + icdCausesCodestr;
                                txtIcdText.Text += ";" + icdCausesstr;
                            }
                        }

                    }
                }

                if (dataRemove != null && dataRemove is List<MissingIcdADO>)
                {
                    RemoveIcdADOList = (List<MissingIcdADO>)dataRemove;
                    if (RemoveIcdADOList != null && RemoveIcdADOList.Count > 0)
                    {
                        var icdMainCheck = RemoveIcdADOList.FirstOrDefault(o => o.ICD_MAIN_CHECK);
                        if (icdMainCheck != null)
                        {
                            var icdMainData = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == icdMainCheck.ICD_CODE);
                            if (icdMainData != null)
                            {
                                this.currentIcds.Remove(icdMainData);
                                DataToComboChuanDoanTD(cboIcds, this.currentIcds);
                            }
                        }

                        var icdCauses = RemoveIcdADOList.Where(o => o.ICD_CAUSE_CHECK).ToList();
                        if (icdCauses != null && icdCauses.Count > 0)
                        {
                            var icdCauseDatas = this.currentIcds.Where(o => icdCauses.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();

                            if (icdCauseDatas != null && icdCauseDatas.Count > 0)
                            {
                                foreach (var item in icdCauseDatas)
                                {
                                    this.currentIcds.Remove(item);
                                }
                            }
                        }
                    }
                }
                ReloadIcdSubContainerByCodeChanged();
                isNotProcessWhileChangedTextSubIcd = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// 0 (hoặc ko khai báo): Không thay đổi gì, giữ nguyên nghiệp vụ như hiện tại
        ///- 1: Có kiểm tra dịch vụ đã kê có nằm trong danh sách đã được cấu hình tương ứng với ICD (căn cứ theo ICD_CODE và ICD_SUB_CODE) của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
        ///- 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu
        internal List<HIS_ICD> GetIcdCodeListFromUcIcd()
        {
            List<HIS_ICD> icdList = new List<HIS_ICD>();
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null && !string.IsNullOrEmpty(icdValue.ICD_CODE))
                {
                    HIS_ICD icdMain = new HIS_ICD();

                    var icd = this.currentIcds.Where(o => o.ICD_CODE == icdValue.ICD_CODE).FirstOrDefault();
                    icdMain.ID = icd != null ? icd.ID : 0;
                    icdMain.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                    icdMain.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                    icdList.Add(icdMain);
                }

                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    string icd_sub_code = subIcd.ICD_SUB_CODE;
                    if (!string.IsNullOrEmpty(icd_sub_code))
                    {
                        String[] icdCodes = icd_sub_code.Split(';');
                        foreach (var item in icdCodes)
                        {
                            var icd = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == item);
                            if (icd != null)
                            {
                                HIS_ICD icdSub = new HIS_ICD();
                                icdSub.ID = icd != null ? icd.ID : 0;
                                icdSub.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                                icdSub.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                                icdList.Add(icdSub);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                icdList = new List<HIS_ICD>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return icdList;
        }

        internal List<HIS_ICD> GetIcdCodeListPatientSelected(List<V_HIS_TREATMENT_BED_ROOM> PatientSelecteds)
        {
            List<HIS_ICD> icdList = new List<HIS_ICD>();
            try
            {
                foreach (var itemPatient in PatientSelecteds)
                {
                    if (itemPatient != null)
                    {
                        if (!string.IsNullOrEmpty(itemPatient.ICD_CODE))
                        {
                            HIS_ICD icdMain = new HIS_ICD();

                            var icd = this.currentIcds.Where(o => o.ICD_CODE == itemPatient.ICD_CODE).FirstOrDefault();
                            icdMain.ID = icd != null ? icd.ID : 0;
                            icdMain.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                            icdMain.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                            icdList.Add(icdMain);
                        }

                        string icd_sub_code = itemPatient.ICD_SUB_CODE;
                        if (!string.IsNullOrEmpty(icd_sub_code))
                        {
                            String[] icdCodes = icd_sub_code.Split(';');
                            foreach (var item in icdCodes)
                            {
                                var icd = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == item);
                                if (icd != null)
                                {
                                    HIS_ICD icdSub = new HIS_ICD();
                                    icdSub.ID = icd != null ? icd.ID : 0;
                                    icdSub.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                                    icdSub.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                                    icdList.Add(icdSub);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                icdList = new List<HIS_ICD>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return icdList;
        }

        /// <summary>
        /// Thuốc cho phép trong "phác đồ" là thuốc thỏa mãn 1 trong 2 điều kiện: loại thuốc (service_id) hoặc hoạt chất (active_ingradient_id) được khai báo tương ứng với ICD
        /// </summary>
        /// <param name="icdCodes"></param>
        /// <param name="serviceIds"></param>
        /// <returns></returns>
        internal List<long> GetServiceIdNotIcdService(List<string> icdCodes, List<MediMatyTypeADO> mediADOs)
        {
            List<long> serviceIdNotIcdServices = new List<long>();
            try
            {
                List<long> serviceIds = mediADOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).Distinct().ToList();

                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.ICD_CODE__EXACTs = icdCodes;
                List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                icdServices = icdServices != null ? icdServices.Where(o => o.IS_INDICATION == 1).ToList() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.icdServiceHasRequireCheck), HisConfigCFG.icdServiceHasRequireCheck) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdServices), icdServices));
                if (HisConfigCFG.icdServiceHasRequireCheck || (!HisConfigCFG.icdServiceHasRequireCheck && icdServices != null && icdServices.Count > 0))
                {
                    if (icdServices == null || icdServices.Count == 0)
                    {
                        return serviceIds;
                    }

                    bool checkByService = icdServices.Any(o => o.SERVICE_ID > 0);
                    bool checkByActiveIngredient = icdServices.Any(o => o.ACTIVE_INGREDIENT_ID > 0);
                    foreach (var item in mediADOs)
                    {
                        bool valid = true;
                        if (checkByService)
                        {
                            valid = icdServices.Any(o => (o.SERVICE_ID > 0 && o.SERVICE_ID == item.SERVICE_ID) || (o.SERVICE_ID == null && o.ACTIVE_INGREDIENT_ID > 0));
                        }

                        Inventec.Common.Logging.LogSystem.Debug("GetServiceIdNotIcdService:1." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid) + " DataType: " + item.DataType);
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByActiveIngredient && valid)
                        {
                            valid = ValidMediAcin(icdServices, item);
                        }

                        Inventec.Common.Logging.LogSystem.Debug("GetServiceIdNotIcdService:2." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                        if (!valid)
                        {
                            serviceIdNotIcdServices.Add(item.SERVICE_ID);
                        }
                    }
                }

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.icdServiceHasRequireCheck), HisConfigCFG.icdServiceHasRequireCheck) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdServices), icdServices) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceIdNotIcdServices), serviceIdNotIcdServices) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdCodes), icdCodes));
            }
            catch (Exception ex)
            {
                serviceIdNotIcdServices = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serviceIdNotIcdServices;
        }

        /// <summary>
        ///      - Với các nghiệp vụ liên quan đến phác đồ điều trị mà hiện tại đang xử lý thì chỉ lấy các dữ liệu HIS_ICD_SERVICE có IS_INDICATION = 1.
        ///- Lúc bổ sung thuốc/hoạt chất: Nếu key cấu hình HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION = 2 thì:
        ///+ Lấy ra các mã ICD của hồ sơ đang kê đơn (mã bệnh chính và mã bênh phụ).
        ///+ Lấy dữ liệu HIS_ICD_SERVICE có IS_CONTRAINDICATION = 1 và có mã ICD thuốc các mã ICD của hồ sơ trên.
        ///+ Từ dữ liệu HIS_ICD_SERVICE lấy ra các dữ liệu có SERVICE_ID trùng với thuốc/hoạt chất đang bổ sung.
        ///+ Nếu tồn tài dữ liệu HIS_ICD_SERVICE thì hiển thị cảnh báo:
        ///"Cảnh báo chống chỉ định:
        ///- Tên thuốc/hoạt chất: Mã ICD 1, Mã ICD 2, ..
        ///Bạn có muốn tiếp tục?"
        ///+ Có thì bổ sung bình thường. Không thì không làm gì.
        ///
        ///- Lúc chọn đơn mẫu/đơn cũ có thuốc/hoạt chất: Nếu key cấu hình HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION = 2 thì:
        ///+ Lấy ra các mã ICD của hồ sơ đang kê đơn (mã bệnh chính và mã bênh phụ).
        ///+ Lấy dữ liệu HIS_ICD_SERVICE có IS_CONTRAINDICATION = 1 và có mã ICD thuốc các mã ICD của hồ sơ trên.
        ///+ Từ dữ liệu HIS_ICD_SERVICE lấy ra các dữ liệu có SERVICE_ID trùng với các thuốc/hoạt chất trong đơn mẫu/đơn cũ.
        ///+ Nếu tồn tài dữ liệu HIS_ICD_SERVICE thì hiển thị cảnh báo:
        ///"Cảnh báo chống chỉ định:
        ///- Tên thuốc/hoạt chất 1: Mã ICD 1, Mã ICD 2, ..
        ///- Tên thuốc/hoạt chất 2: Mã ICD 3, Mã ICD 4, ..
        ///Bạn có muốn tiếp tục?"
        ///+ Có thì bổ sung bình thường. Không thì không làm gì.
        ///
        ///- Lúc lưu kê đơn: Nếu key cấu hình HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION = 1 thì:
        ///+ Lấy ra các mã ICD của hồ sơ đang kê đơn (mã bệnh chính và mã bênh phụ).
        ///+ Lấy dữ liệu HIS_ICD_SERVICE có IS_CONTRAINDICATION = 1 và có mã ICD thuốc các mã ICD của hồ sơ trên.
        ///+ Từ dữ liệu HIS_ICD_SERVICE lấy ra các dữ liệu có SERVICE_ID trùng với các thuốc/hoạt chất trong đơn.
        ///+ Nếu tồn tài dữ liệu HIS_ICD_SERVICE thì hiển thị cảnh báo:
        ///"Chặn chống chỉ định:
        ///- Tên thuốc/hoạt chất 1: Mã ICD 1, Mã ICD 2, ..
        ///- Tên thuốc/hoạt chất 2: Mã ICD 3, Mã ICD 4, ..       
        /// </summary>
        /// <returns></returns>
        internal bool CheckICDServiceForContraindicaterWarningOption(List<MediMatyTypeADO> medicineTypeSDOs, bool isSave = false)
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.ContraindicaterWarningOption == "2")
                {
                    List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
                    var icdCodeArr = (icdFromUc != null && icdFromUc.Count > 0) ? icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList() : null;
                    if (icdCodeArr != null && icdCodeArr.Count > 0)
                    {
                        List<long> serviceIds = medicineTypeSDOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).Distinct().ToList();
                        if (!isSave)
                            icdsWarning = new List<string>();
                        CommonParam param = new CommonParam();
                        HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                        icdServiceFilter.ICD_CODE__EXACTs = icdCodeArr;
                        List<HIS_ICD_SERVICE> icdServicesForContraindicaterWarningOption = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                        List<HIS_ICD_SERVICE> icdServicesForWarningOption = new List<HIS_ICD_SERVICE>();
                        if (icdServicesForContraindicaterWarningOption != null && icdServicesForContraindicaterWarningOption.Count > 0)
                        {
                            icdServicesForWarningOption = icdServicesForContraindicaterWarningOption.Where(o => o.IS_WARNING == 1).ToList();
                            icdServicesForContraindicaterWarningOption = icdServicesForContraindicaterWarningOption.Where(o => o.IS_CONTRAINDICATION == 1).ToList();
                        }

                        if ((icdServicesForContraindicaterWarningOption == null || icdServicesForContraindicaterWarningOption.Count == 0) && (icdServicesForWarningOption == null || icdServicesForWarningOption.Count == 0))
                        {
                            return true;
                        }

                        #region IS_CONTRAINDICATION
                        List<string> icdsWarningTmp = new List<string>();
                        List<long> serviceIdsWarning = new List<long>();
                        string messageWarn = "";
                        string messageWarnDetail = "";
                        Dictionary<string, string> dicWarn = new Dictionary<string, string>();
                        List<IcdServiceADO> icdServiceADOs = new List<IcdServiceADO>();
                        if (icdServicesForContraindicaterWarningOption != null && icdServicesForContraindicaterWarningOption.Count > 0)
                        {
                            bool checkByService = icdServicesForContraindicaterWarningOption.Any(o => o.SERVICE_ID > 0);
                            bool checkByActiveIngredient = icdServicesForContraindicaterWarningOption.Any(o => o.ACTIVE_INGREDIENT_ID > 0);
                            foreach (var item in medicineTypeSDOs)
                            {
                                bool existsData = false;

                                if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByService)
                                {
                                    var serviceIcdBySVs = icdServicesForContraindicaterWarningOption.Where(o => o.SERVICE_ID > 0 && o.SERVICE_ID == item.SERVICE_ID).ToList();
                                    existsData = serviceIcdBySVs != null && serviceIcdBySVs.Count > 0;
                                    if (existsData)
                                    {
                                        string icdCodes = String.Join(", ", serviceIcdBySVs.Select(o => o.ICD_CODE));
                                        if (dicWarn.ContainsKey(String.Format("s{0}", item.SERVICE_ID)))
                                        {
                                            dicWarn[String.Format("s{0}", item.SERVICE_ID)] += ", " + icdCodes;
                                        }
                                        else
                                        {
                                            dicWarn.Add(String.Format("s{0}", item.SERVICE_ID), icdCodes);
                                        }
                                        if ((icdServiceADOs.Count > 0 && icdServiceADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID) == null) || icdServiceADOs.Count == 0)
                                        {
                                            IcdServiceADO ado = new IcdServiceADO();
                                            ado.AMOUNT = item.AMOUNT;
                                            ado.SERVICE_CODE = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_CODE;
                                            ado.SERVICE_NAME = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_NAME;
                                            icdServiceADOs.Add(ado);
                                        }
                                    }
                                }

                                if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByActiveIngredient)
                                {
                                    List<HIS_ICD_SERVICE> icdServiceByAcins = GetValidMediAcin(icdServicesForContraindicaterWarningOption, item);
                                    if (icdServiceByAcins != null && icdServiceByAcins.Count > 0)
                                    {
                                        string icdCodes = String.Join(", ", icdServiceByAcins.Select(o => o.ICD_CODE));
                                        foreach (var itemSvAc in icdServiceByAcins)
                                        {
                                            if (dicWarn.ContainsKey(String.Format("a{0}", itemSvAc.ACTIVE_INGREDIENT_ID ?? 0)))
                                            {
                                                dicWarn[String.Format("a{0}", itemSvAc.ACTIVE_INGREDIENT_ID ?? 0)] += ", " + itemSvAc.ICD_CODE;
                                            }
                                            else
                                            {
                                                dicWarn.Add(String.Format("a{0}", itemSvAc.ACTIVE_INGREDIENT_ID ?? 0), itemSvAc.ICD_CODE);
                                            }
                                            if ((icdServiceADOs.Count > 0 && icdServiceADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID) == null) || icdServiceADOs.Count == 0)
                                            {
                                                IcdServiceADO ado = new IcdServiceADO();
                                                ado.AMOUNT = item.AMOUNT;
                                                ado.SERVICE_CODE = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_CODE;
                                                ado.SERVICE_NAME = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_NAME;
                                                icdServiceADOs.Add(ado);
                                            }
                                        }

                                    }
                                }

                            }
                        }
                        #endregion
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicWarn), dicWarn));
                        #region IS_WARNING
                        string messageWarning = "";
                        string messageWarningDetail = "";
                        Dictionary<string, string> dicWarning = new Dictionary<string, string>();
                        List<IcdServiceADO> icdServiceADOsWarn = new List<IcdServiceADO>();
                        if (icdServicesForWarningOption != null && icdServicesForWarningOption.Count > 0)
                        {
                            bool checkByService = icdServicesForWarningOption.Any(o => o.SERVICE_ID > 0);
                            bool checkByActiveIngredient = icdServicesForWarningOption.Any(o => o.ACTIVE_INGREDIENT_ID > 0);
                            foreach (var item in medicineTypeSDOs)
                            {
                                bool existsData = false;

                                if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByService)
                                {
                                    var serviceIcdBySVs = icdServicesForWarningOption.Where(o => o.SERVICE_ID > 0 && o.SERVICE_ID == item.SERVICE_ID).ToList();
                                    existsData = serviceIcdBySVs != null && serviceIcdBySVs.Count > 0;
                                    if (existsData)
                                    {
                                        serviceIdsWarning.Add(item.SERVICE_ID);
                                        string icdCodes = String.Join(", ", serviceIcdBySVs.Select(o => o.ICD_CODE));
                                        if (dicWarning.ContainsKey(String.Format("s{0}", item.SERVICE_ID)))
                                        {
                                            dicWarning[String.Format("s{0}", item.SERVICE_ID)] += ", " + icdCodes;
                                        }
                                        else
                                        {
                                            dicWarning.Add(String.Format("s{0}", item.SERVICE_ID), icdCodes);
                                        }
                                        if ((icdServiceADOsWarn.Count > 0 && icdServiceADOsWarn.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID) == null) || icdServiceADOsWarn.Count == 0)
                                        {
                                            IcdServiceADO ado = new IcdServiceADO();
                                            ado.AMOUNT = item.AMOUNT;
                                            ado.SERVICE_CODE = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_CODE;
                                            ado.SERVICE_NAME = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_NAME;
                                            icdServiceADOsWarn.Add(ado);
                                        }
                                    }
                                }

                                if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByActiveIngredient)
                                {
                                    List<HIS_ICD_SERVICE> icdServiceByAcins = GetValidMediAcin(icdServicesForWarningOption, item);
                                    if (icdServiceByAcins != null && icdServiceByAcins.Count > 0)
                                    {
                                        serviceIdsWarning.Add(item.SERVICE_ID);
                                        string icdCodes = String.Join(", ", icdServiceByAcins.Select(o => o.ICD_CODE));
                                        foreach (var itemSvAc in icdServiceByAcins)
                                        {
                                            if (dicWarning.ContainsKey(String.Format("a{0}", itemSvAc.ACTIVE_INGREDIENT_ID ?? 0)))
                                            {
                                                dicWarning[String.Format("a{0}", itemSvAc.ACTIVE_INGREDIENT_ID ?? 0)] += ", " + itemSvAc.ICD_CODE;
                                            }
                                            else
                                            {
                                                dicWarning.Add(String.Format("a{0}", itemSvAc.ACTIVE_INGREDIENT_ID ?? 0), itemSvAc.ICD_CODE);
                                            }
                                            if ((icdServiceADOsWarn.Count > 0 && icdServiceADOsWarn.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID) == null) || icdServiceADOsWarn.Count == 0)
                                            {
                                                IcdServiceADO ado = new IcdServiceADO();
                                                ado.AMOUNT = item.AMOUNT;
                                                ado.SERVICE_CODE = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_CODE;
                                                ado.SERVICE_NAME = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID).SERVICE_NAME;
                                                icdServiceADOsWarn.Add(ado);
                                            }
                                        }

                                    }
                                }

                            }
                        }
                        #endregion
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicWarning), dicWarning));
                        if (dicWarn != null && dicWarn.Count > 0)
                        {
                            foreach (var itemW in dicWarn)
                            {
                                messageWarn = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(ResourceMessage.ChanChongChiDinh, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                if (itemW.Key.Contains("s"))
                                {
                                    var metyByName = medicineTypeSDOs.FirstOrDefault(o => itemW.Key == String.Format("s{0}", o.SERVICE_ID));
                                    string metyName = metyByName != null ? metyByName.MEDICINE_TYPE_NAME : "";
                                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => metyByName), metyByName)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => metyByName), metyByName));
                                    var dtIcd = itemW.Value.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                                    foreach (var item in dtIcd)
                                    {
                                        string icdName = null;
                                        string contraindicationContent = null;
                                        var data = icdServicesForContraindicaterWarningOption.FirstOrDefault(o => o.ICD_CODE == item && o.SERVICE_ID == Int64.Parse(itemW.Key.Replace("s", "")));
                                        if(data!= null)
                                        {
                                            icdName = data.ICD_NAME;
                                            contraindicationContent = data.CONTRAINDICATION_CONTENT;
                                        }    
                                        messageWarnDetail += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("- {0}: {1} - {2} {3}", metyName, item, icdName, string.IsNullOrEmpty(contraindicationContent) ? null : ("- " + contraindicationContent)), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    }
                                }
                                else
                                {
                                    if (ValidAcinInteractiveWorker.currentMedicineTypeAcins == null || ValidAcinInteractiveWorker.currentMedicineTypeAcins.Count == 0)
                                        ValidAcinInteractiveWorker.currentMedicineTypeAcins = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE_ACIN>();
                                    MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN acInByName = (ValidAcinInteractiveWorker.currentMedicineTypeAcins != null && ValidAcinInteractiveWorker.currentMedicineTypeAcins.Count > 0) ? ValidAcinInteractiveWorker.currentMedicineTypeAcins.FirstOrDefault(o => itemW.Key == String.Format("a{0}", o.ACTIVE_INGREDIENT_ID)) : null;
                                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => acInByName), acInByName)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => acInByName), acInByName));
                                    string acinName = acInByName != null ? acInByName.ACTIVE_INGREDIENT_NAME : "";
                                    var dtIcd = itemW.Value.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                                    foreach (var item in dtIcd)
                                    {
                                        string icdName = null;
                                        string contraindicationContent = null;
                                        var data = icdServicesForContraindicaterWarningOption.FirstOrDefault(o => o.ICD_CODE == item && o.ACTIVE_INGREDIENT_ID == Int64.Parse(itemW.Key.Replace("a", "")));
                                        if (data != null)
                                        {
                                            icdName = data.ICD_NAME;
                                            contraindicationContent = data.CONTRAINDICATION_CONTENT;
                                        }
                                        messageWarnDetail += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("- {0}: {1} - {2} {3}", acinName, item, icdName, string.IsNullOrEmpty(contraindicationContent) ? null : ("- " + contraindicationContent)), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    }
                                }
                            }
                        }

                        if (dicWarning != null && dicWarning.Count > 0)
                        {
                            foreach (var itemW in dicWarning)
                            {
                                messageWarning = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(ResourceMessage.CanhBaoChongChiDinh, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                if (itemW.Key.Contains("s"))
                                {
                                    var metyByName = medicineTypeSDOs.FirstOrDefault(o => itemW.Key == String.Format("s{0}", o.SERVICE_ID));
                                    string metyName = metyByName != null ? metyByName.MEDICINE_TYPE_NAME : "";
                                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => metyByName), metyByName)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => metyByName), metyByName));
                                    var dtIcd = itemW.Value.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                                    foreach (var item in dtIcd)
                                    {
                                        string icdName = null;
                                        string contraindicationContent = null;
                                        var data = icdServicesForWarningOption.FirstOrDefault(o => o.ICD_CODE == item && o.SERVICE_ID == Int64.Parse(itemW.Key.Replace("s", "")));
                                        if (data != null)
                                        {
                                            icdName = data.ICD_NAME;
                                            contraindicationContent = data.CONTRAINDICATION_CONTENT;
                                        }
                                        messageWarningDetail += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("- {0}: {1} - {2} {3}", metyName, item, icdName, string.IsNullOrEmpty(contraindicationContent) ? null : ("- " + contraindicationContent)), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        icdsWarningTmp.Add(item);
                                    }
                                }
                                else
                                {
                                    if (ValidAcinInteractiveWorker.currentMedicineTypeAcins == null || ValidAcinInteractiveWorker.currentMedicineTypeAcins.Count == 0)
                                        ValidAcinInteractiveWorker.currentMedicineTypeAcins = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE_ACIN>();
                                    MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN acInByName = (ValidAcinInteractiveWorker.currentMedicineTypeAcins != null && ValidAcinInteractiveWorker.currentMedicineTypeAcins.Count > 0) ? ValidAcinInteractiveWorker.currentMedicineTypeAcins.FirstOrDefault(o => itemW.Key == String.Format("a{0}", o.ACTIVE_INGREDIENT_ID)) : null;
                                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => acInByName), acInByName)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => acInByName), acInByName));
                                    string acinName = acInByName != null ? acInByName.ACTIVE_INGREDIENT_NAME : "";
                                    var dtIcd = itemW.Value.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                                    foreach (var item in dtIcd)
                                    {
                                        string icdName = null;
                                        string contraindicationContent = null;
                                        var data = icdServicesForWarningOption.FirstOrDefault(o => o.ICD_CODE == item && o.ACTIVE_INGREDIENT_ID == Int64.Parse(itemW.Key.Replace("a", "")));
                                        if (data != null)
                                        {
                                            icdName = data.ICD_NAME;
                                            contraindicationContent = data.CONTRAINDICATION_CONTENT;
                                        }
                                        messageWarningDetail += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format("- {0}: {1} - {2} {3}", acinName, item, icdName,string.IsNullOrEmpty(contraindicationContent) ? null : ("- "+ contraindicationContent)), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        icdsWarningTmp.Add(item);
                                    }
                                }
                            }
                        }

                        bool isSavetmp = isSave;
                        if (isSave)
                            isSave = false;

                        if (!String.IsNullOrEmpty(messageWarningDetail) && !isSave && (medicineTypeSDOs.Count(o => (o.IcdsWarning == null || o.IcdsWarning.Count == 0)) == medicineTypeSDOs.Count || medicineTypeSDOs.FirstOrDefault(o => (o.IcdsWarning == null || o.IcdsWarning.Count == 0) && serviceIdsWarning.Exists(p => p == o.SERVICE_ID)) != null))
                        {
                            if (isSavetmp) isSave = true;
                            //ResourceMessage.BanCoMuonTiepTuc.Replace("{0} {1}. ", "")
                            DialogResult myResult = DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0}{1}{2}", messageWarning, messageWarningDetail, "Bạn có muốn tiếp tục"), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                            if (myResult != DialogResult.Yes)
                            {
                                ProcessPatientSelectForm(icdServiceADOsWarn);
                                return false;
                            }
                            icdsWarning = icdsWarningTmp;
                        }

                        if (!String.IsNullOrEmpty(messageWarnDetail) && (isSave || isSavetmp))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0}{1}{2}", messageWarn, messageWarnDetail, ""), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DevExpress.Utils.DefaultBoolean.True);
                            ProcessPatientSelectForm(icdServiceADOs);
                            return false;
                        }
                    }
                }
                else if (HisConfigCFG.ContraindicaterWarningOption == "1")
                {
                    if (!isSave)
                        IsWarned = false;
                    List<long> serviceWarnIds = new List<long>();
                    List<HIS_ICD> icdFromUc = new List<HIS_ICD>();
                    List<IcdServiceADO> lstIcdServiceADOs = new List<IcdServiceADO>();
                    List<IcdServiceADO> lstIcdServiceADOsWarning = new List<IcdServiceADO>();
                    if (GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                    {
                        var listPatientSelecteds = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                        if (listPatientSelecteds != null && listPatientSelecteds.Count > 1)
                        {
                            icdFromUc.AddRange(GetIcdCodeListPatientSelected(listPatientSelecteds));
                        }
                    }

                    icdFromUc.AddRange(GetIcdCodeListFromUcIcd());

                    if (icdFromUc != null && icdFromUc.Count > 0)
                    {
                        icdFromUc = icdFromUc.Distinct().ToList();
                    }

                    var icdCodeArr = (icdFromUc != null && icdFromUc.Count > 0) ? icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList() : null;

                    if (icdCodeArr != null && icdCodeArr.Count > 0)
                    {
                        List<long> serviceIds = medicineTypeSDOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).Distinct().ToList();
                        List<long> acinIgrIds = new List<long>();
                        List<long> metyIds = medicineTypeSDOs.Where(o => (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)).Select(t => t.ID).ToList();

                        var medicineTypeAcinF1s = ValidAcinInteractiveWorker.currentMedicineTypeAcins.Where(o => metyIds.Contains(o.MEDICINE_TYPE_ID)).ToList();
                        if (medicineTypeAcinF1s != null && medicineTypeAcinF1s.Count > 0)
                        {
                            acinIgrIds = medicineTypeAcinF1s.Select(o => o.ACTIVE_INGREDIENT_ID).Distinct().ToList();
                        }
                        CommonParam param = new CommonParam();
                        HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                        icdServiceFilter.SERVICE_ID_OR_ACTIVE_INGREDIENT_ID = new ActiveIngredientOrServiceId() { ServiceIds = serviceIds, ActiveIngredientIds = acinIgrIds };
                        List<HIS_ICD_SERVICE> icdServicesForContraindicaterWarningOption = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                        List<HIS_ICD_SERVICE> icdServicesForWarningOption = new List<HIS_ICD_SERVICE>();
                        if (icdServicesForContraindicaterWarningOption != null && icdServicesForContraindicaterWarningOption.Count > 0)
                        {
                            icdServicesForWarningOption = icdServicesForContraindicaterWarningOption.Where(o => o.IS_WARNING == 1).ToList();
                            icdServicesForContraindicaterWarningOption = icdServicesForContraindicaterWarningOption.Where(o => o.IS_CONTRAINDICATION == 1).ToList();
                        }

                        if ((icdServicesForContraindicaterWarningOption == null || icdServicesForContraindicaterWarningOption.Count == 0) && (icdServicesForWarningOption == null || icdServicesForWarningOption.Count == 0))
                        {
                            return true;
                        }
                        bool valid1 = false, valid2 = false;
                        if (isSave)
                        {
                            if (icdServicesForContraindicaterWarningOption != null && icdServicesForContraindicaterWarningOption.Count > 0)
                            {
                                var checkIcd = icdServicesForContraindicaterWarningOption.Where(o => icdCodeArr.Contains(o.ICD_CODE)).ToList();
                                if (checkIcd == null || checkIcd.Count == 0)
                                {
                                    valid1 = true;
                                }
                            }
                            if (icdServicesForWarningOption != null && icdServicesForWarningOption.Count > 0)
                            {
                                var checkIcdW = icdServicesForWarningOption.Where(o => icdCodeArr.Contains(o.ICD_CODE)).ToList();
                                if (checkIcdW == null || checkIcdW.Count == 0)
                                {
                                    valid2 = true;
                                }
                            }
                            if (valid1 && valid2) return true;
                        }
                        else
                        {
                            if (icdServicesForWarningOption != null && icdServicesForWarningOption.Count > 0)
                            {
                                var checkIcdW = icdServicesForWarningOption.Where(o => icdCodeArr.Contains(o.ICD_CODE)).ToList();
                                if (checkIcdW == null || checkIcdW.Count == 0)
                                {
                                    return true;
                                }
                            }
                        }
                        bool checkByService = icdServicesForContraindicaterWarningOption.Any(o => o.SERVICE_ID > 0) && !valid1;
                        bool checkByActiveIngredient = icdServicesForContraindicaterWarningOption.Any(o => o.ACTIVE_INGREDIENT_ID > 0) && !valid1;


                        bool checkByServiceW = icdServicesForWarningOption.Any(o => o.SERVICE_ID > 0);
                        bool checkByActiveIngredientW = icdServicesForWarningOption.Any(o => o.ACTIVE_INGREDIENT_ID > 0);
                        Dictionary<string, string> dicWarn = new Dictionary<string, string>();
                        foreach (var item in medicineTypeSDOs)
                        {
                            if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByService)
                            {
                                var serviceIcdBySVs = icdServicesForContraindicaterWarningOption.Where(o => o.SERVICE_ID > 0 && o.SERVICE_ID == item.SERVICE_ID).ToList();

                                foreach (var serviceIcd in serviceIcdBySVs)
                                {
                                    IcdServiceADO ado = new IcdServiceADO(serviceIcd, item);
                                    lstIcdServiceADOs.Add(ado);
                                }
                            }

                            if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByServiceW)
                            {
                                var serviceIcdBySVs = icdServicesForWarningOption.Where(o => o.SERVICE_ID > 0 && o.SERVICE_ID == item.SERVICE_ID).ToList();
                                if (serviceIcdBySVs != null && serviceIcdBySVs.Count > 0)
                                {
                                    serviceWarnIds.Add(item.SERVICE_ID);
                                    foreach (var serviceIcd in serviceIcdBySVs)
                                    {
                                        IcdServiceADO ado = new IcdServiceADO(serviceIcd, item);
                                        lstIcdServiceADOsWarning.Add(ado);
                                    }
                                }
                            }

                            if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByActiveIngredient)
                            {
                                List<HIS_ICD_SERVICE> icdServiceByAcins = GetValidMediAcin(icdServicesForContraindicaterWarningOption, item);
                                if (icdServiceByAcins != null && icdServiceByAcins.Count > 0)
                                {
                                    foreach (var itemSvAc in icdServiceByAcins)
                                    {
                                        IcdServiceADO ado = new IcdServiceADO(itemSvAc, item);
                                        lstIcdServiceADOs.Add(ado);
                                    }

                                }
                            }

                            if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByActiveIngredientW)
                            {
                                List<HIS_ICD_SERVICE> icdServiceByAcins = GetValidMediAcin(icdServicesForWarningOption, item);
                                if (icdServiceByAcins != null && icdServiceByAcins.Count > 0)
                                {
                                    serviceWarnIds.Add(item.SERVICE_ID);
                                    foreach (var itemSvAc in icdServiceByAcins)
                                    {
                                        IcdServiceADO ado = new IcdServiceADO(itemSvAc, item);
                                        lstIcdServiceADOsWarning.Add(ado);
                                    }

                                }
                            }
                        }


                        if (lstIcdServiceADOs != null && lstIcdServiceADOs.Count > 0 && isSave)
                        {
                            frmBlockNoAdd frm = new frmBlockNoAdd(lstIcdServiceADOs);
                            frm.ShowDialog();
                            var dtGroup = lstIcdServiceADOs.GroupBy(o => new { o.SERVICE_ID }).ToList();
                            foreach (var item in dtGroup)
                            {
                                ProcessPatientSelectForm(new List<IcdServiceADO>() { item.First() });
                            }
                            return false;
                        }

                        if (lstIcdServiceADOsWarning != null && lstIcdServiceADOsWarning.Count > 0 && (medicineTypeSDOs.Count(o => !o.IsWarned) == medicineTypeSDOs.Count || medicineTypeSDOs.FirstOrDefault(o => !o.IsWarned && serviceWarnIds.Exists(p => p == o.SERVICE_ID)) != null))
                        {
                            frmBlockNoAdd frm = new frmBlockNoAdd(lstIcdServiceADOsWarning, ActionContinue);
                            frm.ShowDialog();
                            var dtGroup = lstIcdServiceADOsWarning.GroupBy(o => new { o.SERVICE_ID }).ToList();
                            foreach (var item in dtGroup)
                            {
                                ProcessPatientSelectForm(new List<IcdServiceADO>() { item.First() });
                            }
                            if(IsContinue)
                                IsWarned = true;
                            return IsContinue;
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

        private void ActionContinue(bool obj)
        {
            IsContinue = obj;
        }

        private void ProcessPatientSelectForm(List<IcdServiceADO> ListObj = null)
        {
            try
            {
                if (GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    var listPatientSelecteds = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    foreach (var item in listPatientSelecteds)
                    {
                        CallApiObeyCotraindie(item.TREATMENT_ID, ListObj);
                    }
                }
                else
                {
                    CallApiObeyCotraindie(treatmentId, ListObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallApiObeyCotraindie(long treatmentId, List<IcdServiceADO> ListObj = null)
        {
            try
            {
                CommonParam param = new CommonParam();
                foreach (var item in ListObj)
                {
                    HIS_OBEY_CONTRAINDI obj = new HIS_OBEY_CONTRAINDI();
                    obj.REQUEST_LOGINNAME = cboUser.EditValue.ToString();
                    obj.REQUEST_DEPARTMENT_ID = currentWorkPlace.DepartmentId;
                    obj.TREATMENT_ID = treatmentId;
                    obj.INTRUCTION_TIME = this.InstructionTime;
                    obj.ICD_CODE = txtIcdCode.Text;
                    obj.ICD_NAME = cboIcds.Text;
                    obj.ICD_SUB_CODE = txtIcdSubCode.Text;
                    obj.ICD_TEXT = txtIcdText.Text;
                    obj.SERVICE_ID = item.SERVICE_ID;
                    obj.REQUEST_ROOM_ID = currentModule.RoomId;
                    obj.SERVICE_CODE = item.SERVICE_CODE;
                    obj.SERVICE_NAME = item.SERVICE_NAME;
                    obj.AMOUNT = item.AMOUNT;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                    var dt = new BackendAdapter(param).Post<HIS_OBEY_CONTRAINDI>("api/HisObeyContraindi/Create", ApiConsumers.MosConsumer, obj, ProcessLostToken, param);
                    if (dt != null)
                    {
                        ObeyContraindiSave.Add(dt);
                        Inventec.Common.Logging.LogSystem.Debug("CALL API api/HisObeyContraindie/Create: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dt), dt));
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CALL API api/HisObeyContraindie/Create: null");

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal List<long> GetServiceIdNotIcdServiceForContraindicaterWarningOption(List<string> icdCodes, List<MediMatyTypeADO> mediADOs)
        {
            List<long> serviceIdNotIcdServices = new List<long>();
            try
            {
                List<long> serviceIds = mediADOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).Distinct().ToList();

                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.ICD_CODE__EXACTs = icdCodes;
                List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.icdServiceHasRequireCheck), HisConfigCFG.icdServiceHasRequireCheck) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdServices), icdServices));

                if (icdServices != null && icdServices.Count > 0)
                {
                    icdServices = icdServices.Where(o => o.IS_CONTRAINDICATION == 1).ToList();
                    //icdServices = icdServices.Where(o => o.IS_INDICATION == 1).ToList();
                }

                if (icdServices == null || icdServices.Count == 0)
                {
                    return serviceIds;
                }

                bool checkByService = icdServices.Any(o => o.SERVICE_ID > 0);
                bool checkByActiveIngredient = icdServices.Any(o => o.ACTIVE_INGREDIENT_ID > 0);
                foreach (var item in mediADOs)
                {
                    bool valid = true;
                    if (checkByService)
                    {
                        valid = icdServices.Any(o => o.SERVICE_ID > 0 && o.SERVICE_ID == item.SERVICE_ID);
                    }

                    Inventec.Common.Logging.LogSystem.Debug("GetServiceIdNotIcdServiceForContraindicaterWarningOption:1." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                    if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByActiveIngredient && valid)
                    {
                        valid = ValidMediAcin(icdServices, item);
                    }

                    Inventec.Common.Logging.LogSystem.Debug("GetServiceIdNotIcdServiceForContraindicaterWarningOption:2." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                    if (!valid)
                    {
                        serviceIdNotIcdServices.Add(item.SERVICE_ID);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.ContraindicaterWarningOption), HisConfigCFG.ContraindicaterWarningOption) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdServices), icdServices) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceIdNotIcdServices), serviceIdNotIcdServices) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdCodes), icdCodes));
            }
            catch (Exception ex)
            {
                serviceIdNotIcdServices = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serviceIdNotIcdServices;
        }

        private List<HIS_ICD_SERVICE> GetServiceNotInByMedicineAndAcIngrInServiceIcd(List<string> icdCodes, List<MediMatyTypeADO> mediADOs)
        {
            List<HIS_ICD_SERVICE> result = new List<HIS_ICD_SERVICE>();
            try
            {
                if (icdCodes == null || icdCodes.Count == 0 || mediADOs == null || mediADOs.Count == 0)
                {
                    return result;
                }

                List<long> serviceIds = mediADOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).Distinct().ToList();
                List<long> medicineTypeIds = mediADOs.Where(o => o.SERVICE_ID > 0).Select(p => p.ID).Distinct().ToList();
                List<long> metyIds = mediADOs.Where(o => (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)).Select(t => t.ID).ToList();
                List<HIS_ICD_SERVICE> icdServices = new List<HIS_ICD_SERVICE>();
                CommonParam param = new CommonParam();

                var medicineTypeAcinByMety = ValidAcinInteractiveWorker.GetMedicineTypeAcinByMedicineType(metyIds);

                List<long> acinIgrIds = new List<long>();
                if (medicineTypeAcinByMety != null && medicineTypeAcinByMety.Count > 0)
                {
                    acinIgrIds = medicineTypeAcinByMety.Select(o => o.ACTIVE_INGREDIENT_ID).Distinct().ToList();
                }
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.SERVICE_ID_OR_ACTIVE_INGREDIENT_ID = new ActiveIngredientOrServiceId() { ServiceIds = serviceIds, ActiveIngredientIds = acinIgrIds };
                icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                List<HIS_ICD_SERVICE> icdsvNotExists = new List<HIS_ICD_SERVICE>();
                if (HisConfigCFG.icdServiceHasCheck == 4)
                    icdServices = icdServices.Where(o => !icdServices.Where(p => p.IS_CONTRAINDICATION == 1).Select(p => p.ICD_CODE).ToList().Exists(p => p == o.ICD_CODE)).ToList();
                else if (HisConfigCFG.icdServiceHasCheck == 5)
                {
                    icdServices = icdServices.Where(p => p.IS_CONTRAINDICATION != 1 && p.IS_WARNING != 1).ToList();
                    if(icdServices.Count > 0)
                    {
                        var group = icdServices.GroupBy(o => o.SERVICE_ID);
                        foreach(var groupItem in group)
                        {
                            if(!groupItem.ToList().Exists(o => icdCodes.Contains(o.ICD_CODE)))
                                icdsvNotExists.AddRange(groupItem.ToList().Where(o => !icdCodes.Contains(o.ICD_CODE)).ToList());
                        }
                    }
                }
                if (HisConfigCFG.icdServiceHasCheck != 5)
                {
                    icdsvNotExists = (icdServices.Count > 0 && !icdServices.Exists(o => icdCodes.Contains(o.ICD_CODE))) ? icdServices.Where(o => !icdCodes.Contains(o.ICD_CODE)).ToList() : null;
                }
                if (icdsvNotExists != null && icdsvNotExists.Count > 0)
                {
                    foreach (var isvno in icdsvNotExists)
                    {
                        if (isvno.SERVICE_ID.HasValue && isvno.SERVICE_ID > 0 && (result.Count == 0 || !result.Exists(o => o.SERVICE_ID == isvno.SERVICE_ID && o.ICD_CODE == isvno.ICD_CODE)))
                        {
                            result.Add(isvno);
                        }
                        else if (isvno.ACTIVE_INGREDIENT_ID.HasValue && isvno.ACTIVE_INGREDIENT_ID > 0)
                        {
                            var medicineTypeAcinCheck = medicineTypeAcinByMety.Where(o => o.ACTIVE_INGREDIENT_ID == isvno.ACTIVE_INGREDIENT_ID.Value).ToList();
                            if (medicineTypeAcinCheck != null && medicineTypeAcinCheck.Count > 0)
                            {
                                foreach (var metyAcin in medicineTypeAcinCheck)
                                {
                                    var mediADO = mediADOs.Where(o => (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)).FirstOrDefault(o => o.ID == metyAcin.MEDICINE_TYPE_ID);
                                    if (mediADO != null && (result.Count == 0 || !result.Exists(o => o.SERVICE_ID == mediADO.SERVICE_ID && o.ICD_CODE == isvno.ICD_CODE)))
                                    {
                                        HIS_ICD_SERVICE svAdd = new HIS_ICD_SERVICE();
                                        svAdd.SERVICE_ID = mediADO.SERVICE_ID;
                                        svAdd.ICD_CODE = isvno.ICD_CODE;
                                        svAdd.ICD_NAME = isvno.ICD_NAME;
                                        svAdd.ID = isvno.ID;
                                        result.Add(svAdd);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ValidMediAcin(List<HIS_ICD_SERVICE> icdServices1, MediMatyTypeADO mediADO)
        {
            bool isValid = false;
            try
            {
                if (ValidAcinInteractiveWorker.currentMedicineTypeAcins == null)
                {
                    ValidAcinInteractiveWorker.currentMedicineTypeAcins = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE_ACIN>();
                }
                if (ValidAcinInteractiveWorker.currentMedicineTypeAcins != null && ValidAcinInteractiveWorker.currentMedicineTypeAcins.Count > 0 && mediADO != null)
                {
                    var medicineTypeAcinF1s = ValidAcinInteractiveWorker.currentMedicineTypeAcins.Where(o => mediADO.ID == o.MEDICINE_TYPE_ID).ToList();
                    if (medicineTypeAcinF1s != null && medicineTypeAcinF1s.Count > 0)
                    {
                        var acinIgrIds = medicineTypeAcinF1s.Select(o => o.ACTIVE_INGREDIENT_ID).ToList();
                        isValid = icdServices1.Any(o => o.ACTIVE_INGREDIENT_ID > 0 && acinIgrIds.Contains(o.ACTIVE_INGREDIENT_ID ?? 0));
                        if (!isValid)
                            Inventec.Common.Logging.LogSystem.Debug("Khong tim thay HIS_ICD_SERVICE theo medicineTypeId = " + mediADO.ID + " & cac hoat chat cua loai thuoc do (" + String.Join(",", acinIgrIds) + ")");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ValidMediAcin: Khong co du lieu MedicineTypeAcins theo medicineTypeId = " + mediADO.ID);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ValidMediAcin: Khong co du lieu danh muc MedicineTypeAcins");
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return isValid;
        }

        private List<HIS_ICD_SERVICE> GetValidMediAcin(List<HIS_ICD_SERVICE> icdServices1, MediMatyTypeADO mediADO)
        {
            List<HIS_ICD_SERVICE> result = null;
            try
            {
                if (ValidAcinInteractiveWorker.currentMedicineTypeAcins == null)
                {
                    ValidAcinInteractiveWorker.currentMedicineTypeAcins = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE_ACIN>();
                }
                if (ValidAcinInteractiveWorker.currentMedicineTypeAcins != null && ValidAcinInteractiveWorker.currentMedicineTypeAcins.Count > 0 && mediADO != null)
                {
                    var medicineTypeAcinF1s = ValidAcinInteractiveWorker.currentMedicineTypeAcins.Where(o => mediADO.ID == o.MEDICINE_TYPE_ID).ToList();
                    if (medicineTypeAcinF1s != null && medicineTypeAcinF1s.Count > 0)
                    {
                        var acinIgrIds = medicineTypeAcinF1s.Select(o => o.ACTIVE_INGREDIENT_ID).ToList();
                        result = icdServices1.Where(o => o.ACTIVE_INGREDIENT_ID > 0 && acinIgrIds.Contains(o.ACTIVE_INGREDIENT_ID ?? 0)).ToList();
                        if (result != null && result.Count > 0)
                            Inventec.Common.Logging.LogSystem.Debug("GetValidMediAcin: Tim thay HIS_ICD_SERVICE theo medicineTypeId = " + mediADO.ID + " & cac hoat chat cua loai thuoc do (" + String.Join(",", acinIgrIds) + ")");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetValidMediAcin: Khong co du lieu MedicineTypeAcins theo medicineTypeId = " + mediADO.ID);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("GetValidMediAcin: Khong co du lieu danh muc MedicineTypeAcins");
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private bool CheckWarringIntructionUseDayNum()
        {
            bool result = true;
            try
            {
                if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && HisConfigCFG.WarringIntructionUseDayNum.HasValue)
                {
                    var instructionTimeMedis = GetInstructionTimeMedi();
                    DateTime dtAdd = DateTime.Now.AddDays(HisConfigCFG.WarringIntructionUseDayNum.Value);
                    long intructionTimeTemp = instructionTimeMedis.OrderByDescending(o => o).First();
                    DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeTemp).Value;
                    if (DateTime.Compare(dtIntructionTime.Date, dtAdd.Date) > 0)
                    {
                        MessageBox.Show(String.Format("Thời gian y lệnh lớn hơn thời gian giới hạn kê thuốc. ({0} ngày)", HisConfigCFG.WarringIntructionUseDayNum.Value));
                        result = false;
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

        private bool CheckUseDayAndExpTimeBHYT()
        {
            bool result = true;
            try
            {
                //Check key cau hinh
                if (HisConfigCFG.IsWarringUseDayAndExpTimeBHYT && !GlobalStore.IsTreatmentIn)
                {
                    if (this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        //Thoi gian dung
                        List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                        if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                        {
                            var instructionTimeMedis = GetInstructionTimeMedi();
                            long? maxUseTimeTo = mediMatyTypeADOs.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).Max(o => o.UseTimeTo);
                            if (maxUseTimeTo > 0
                                && this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME.HasValue
                                && maxUseTimeTo / 1000000 > this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME.Value / 1000000)
                            {
                                long intructionTimeLong = instructionTimeMedis != null ? instructionTimeMedis
                                    .OrderBy(o => o).FirstOrDefault() : 0;
                                if (intructionTimeLong > 0)
                                {
                                    DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeLong).Value;
                                    DateTime dtHeinCardToTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value;
                                    TimeSpan ts = (TimeSpan)(dtHeinCardToTime.Date - dtIntructionTime.Date);

                                    DialogResult myResult;
                                    myResult = MessageBox.Show(String.Format(ResourceMessage.BenhNhanSapHetHanTheBHYT_SoNgayThuocToiDa_BanCoMuonTiepTuc, ts.TotalDays + 1), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (myResult != DialogResult.Yes)
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;

        }

        private bool CheckTreatmentFinish()
        {
            bool result = true;
            try
            {
                if (treatmentFinishProcessor != null)
                {
                    var treatDT = treatmentFinishProcessor.GetDataOutput(ucTreatmentFinish);
                    if (treatDT.IsAutoTreatmentFinish)
                    {
                        if (HisConfigCFG.CheckSameHein == 1)
                        {
                            bool checkSameHein = false;
                            CommonParam param = new CommonParam();

                            HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                            patientTypeAlterFilter.TREATMENT_ID = treatmentId;

                            var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                            if (patientTypeAlter != null && patientTypeAlter.Count >= 2)
                            {
                                foreach (var item in patientTypeAlter)
                                {
                                    var sameHein = patientTypeAlter.Where(o => o.HEIN_CARD_NUMBER == item.HEIN_CARD_NUMBER).ToList();
                                    if (sameHein != null && sameHein.Count >= 2)
                                    {
                                        var checkHeinOrg = sameHein.Select(o => o.HEIN_MEDI_ORG_CODE).Distinct().ToList();
                                        if (checkHeinOrg.Count > 1)
                                        {
                                            //Mã cskcb khác nhau
                                            checkSameHein = true;
                                            break;
                                        }
                                        else
                                        {
                                            var checkRightRoute = sameHein.Select(o => o.RIGHT_ROUTE_CODE).Distinct().ToList();
                                            if (checkRightRoute.Count == 1)
                                            {
                                                //Đúng tuyến và lý do đúng tuyến khác nhau
                                                if (checkRightRoute.FirstOrDefault() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                                                {
                                                    var checkRightRouteType = sameHein.Select(o => o.RIGHT_ROUTE_TYPE_CODE).Distinct().ToList();
                                                    if (checkRightRouteType.Count > 1)
                                                    {
                                                        checkSameHein = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Trái tuyến
                                                checkSameHein = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (checkSameHein)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoSaiThongTinTheBHYT, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                result = false;
                            }
                        }

                        //1. HIS_RS: Bảng phòng khám/cls/pttt bổ sung:
                        //- Cột Số lượt hẹn khám tối đa trên ngày (MAX_APPOINTMENT_BY_DAY)
                        //6. Sửa chức năng "Kê đơn": Xử trí kết thúc điều trị loại hẹn khám: Lúc lưu kê đơn thì sẽ xử lý như sau:
                        //- Kiểm tra các phòng hẹn khám được chọn xem có phòng nào được cấu hình số lượt hẹn khám trên 1 ngày (MAX_APPOINTMENT_BY_DAY > 0) hay không.
                        //- Nếu không có phòng nào được cấu hình thì tiếp tục lưu kê đơn.
                        //- Nếu có phòng được cấu hình thì gọi api lấy số lượt thiết lập và số lượt đã chỉ định hoặc hẹn khám vào các phòng đấy: Từ kết quả trả về của api kiểm tra xem có phòng nào có sl chỉ định và hẹn khám >= số lượng thiết lập hay không.
                        //+ Nếu không thì thực hiện xử lý lưu kê đơn.
                        //+ Nếu có thì hiển thị cảnh báo:
                        //"Phòng khám có số lượt hẹn khám vượt số lượt cho phép: PK mắt (hiện tại/max), PK Tim (hiện tại/max),.... Bạn có muốn tiếp tục?" mặc định focus vào nút Không. Nếu nhấn "Có" thì xử lý lưu kê đơn tiếp nếu nhấn "Không" thì không làm gì.
                        if (result
                            && treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                            && treatDT.AppointmentNextRoomIds != null && treatDT.AppointmentNextRoomIds.Count > 0)
                        {
                            var executeRooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>();
                            if (executeRooms != null && executeRooms.Count > 0)
                            {
                                var executeRoomByAppointmentNextRooms = executeRooms.Where(o => treatDT.AppointmentNextRoomIds.Contains(o.ROOM_ID) && o.MAX_APPOINTMENT_BY_DAY.HasValue && o.MAX_APPOINTMENT_BY_DAY > 0).ToList();
                                if (executeRoomByAppointmentNextRooms != null && executeRoomByAppointmentNextRooms.Count > 0)
                                {
                                    //TODO
                                    string listRoomAlert = "";

                                    CommonParam param = new CommonParam();
                                    HisExecuteRoomAppointedFilter executeRoomAppointedFilter = new HisExecuteRoomAppointedFilter();
                                    executeRoomAppointedFilter.EXECUTE_ROOM_IDs = executeRoomByAppointmentNextRooms.Select(o => o.ID).Distinct().ToList();
                                    executeRoomAppointedFilter.INTR_OR_APPOINT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(treatDT.dtAppointmentTime.ToString("yyyyMMdd") + "000000");
                                    List<HisExecuteRoomAppointedSDO> executeRoomAppointedSDOs = new BackendAdapter(param)
                                    .Get<List<HisExecuteRoomAppointedSDO>>("api/HisExecuteRoom/GetCountAppointed", ApiConsumers.MosConsumer, executeRoomAppointedFilter, param);

                                    foreach (var itemExecuteRoom in executeRoomByAppointmentNextRooms)
                                    {
                                        var roomAppointedValid = executeRoomAppointedSDOs != null && executeRoomAppointedSDOs.Count > 0 ? executeRoomAppointedSDOs.FirstOrDefault(o => o.ExecuteRoomId == itemExecuteRoom.ID && o.MaxAmount > 0 && o.CurrentAmount >= o.MaxAmount) : null;
                                        if (roomAppointedValid != null)
                                        {
                                            listRoomAlert += String.Format("{0} ({1}/{2}),", itemExecuteRoom.EXECUTE_ROOM_NAME, roomAppointedValid.CurrentAmount, roomAppointedValid.MaxAmount);
                                        }
                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemExecuteRoom), itemExecuteRoom)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomAppointedValid), roomAppointedValid)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => executeRoomAppointedSDOs), executeRoomAppointedSDOs)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => executeRoomAppointedFilter), executeRoomAppointedFilter));
                                    }

                                    if (!String.IsNullOrEmpty(listRoomAlert) && DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.PhongKhamCoSoLuongKhamVuotSoLuotChoPhep, listRoomAlert),
                      HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                      MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                    {
                                        Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listRoomAlert), listRoomAlert));
                                        return false;
                                    }
                                }
                            }
                        }

                        ///#39011
                        ///Sửa chức năng "Kê đơn phòng khám" để cho phép cấu hình số ngày hẹn khám tối đa
                        ///2. Sửa chức năng "Kê đơn phòng khám"(HIS.Desktop.Plugins.AssignPrescriptionPK)
                        ///Trong trường hợp xử trí kết thúc điều trị và chọn loại kết thúc là "Hẹn khám", thì kiểm tra, nếu key cấu hình MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS có giá trị thì xử lý:
                        ///Kiểm tra nếu số ngày hẹn khám vượt quá số ngày được cấu hình, thì:
                        ///+ Nếu MOS.HIS_TREATMENT.WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS có giá trị 1, thì hiển thị cảnh báo:
                        ///"Số ngày hẹn khám tối đa cho phép là XXX ngày. Bạn có muốn tiếp tục không?".
                        ///Trong đó, XXX là giá trị được cấu hình trong thẻ MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS
                        ///Nếu người dùng chọn "Đồng ý", thì thực hiện xử lý như cũ (gọi api lưu dữ liệu)
                        ///Nếu người dùng chọn "Không", thì tắt thông báo và kết thúc xử lý

                        ///+ Nếu MOS.HIS_TREATMENT.WARNING_OPTION_WHEN_EXCEEDING_MAX_OF_APPOINTMENT_DAYS có giá trị 2, thì hiển thị thông báo:
                        ///"Số ngày hẹn khám tối đa cho phép là XXX ngày." và không cho phép lưu.
                        ///Trong đó, XXX là giá trị được cấu hình trong thẻ MOS.HIS_TREATMENT.MAX_OF_APPOINTMENT_DAYS
                        //    if (result
                        //        && treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                        //    {
                        //        long useDay = (long)treatmentFinishProcessor.GetUseDay(ucTreatmentFinish);
                        //        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => useDay), useDay)
                        //            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatDT), treatDT)
                        //            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.MaxOfAppointmentDays), HisConfigCFG.MaxOfAppointmentDays)); 
                        //        if (HisConfigCFG.MaxOfAppointmentDays.HasValue && HisConfigCFG.MaxOfAppointmentDays.Value < useDay)
                        //        {
                        //            if (HisConfigCFG.WarningOptionWhenExceedingMaxOfAppointmentDays.HasValue && HisConfigCFG.WarningOptionWhenExceedingMaxOfAppointmentDays == 1)
                        //            {
                        //                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.SoNgayHenKhamToiDaChoPhepLaXXX__BanCoMuonTiepTuc, HisConfigCFG.MaxOfAppointmentDays.Value),
                        //HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        //MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        //                {
                        //                    Inventec.Common.Logging.LogSystem.Warn(String.Format(ResourceMessage.SoNgayHenKhamToiDaChoPhepLaXXX__BanCoMuonTiepTuc, HisConfigCFG.MaxOfAppointmentDays.Value) + "____Nguoi dung chon khong==> tat thong bao & ket thuc xu ly.");
                        //                    return false;
                        //                }
                        //            }
                        //            else if (HisConfigCFG.WarningOptionWhenExceedingMaxOfAppointmentDays.HasValue && HisConfigCFG.WarningOptionWhenExceedingMaxOfAppointmentDays == 2)
                        //            {
                        //                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.SoNgayHenKhamToiDaChoPhepLaXXX, HisConfigCFG.MaxOfAppointmentDays.Value),
                        //HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        //                return false;
                        //            }
                        //        }
                        //    }
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

        private bool CheckTuberCulosis()
        {
            bool valid = true;
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null && !string.IsNullOrEmpty(icdValue.ICD_CODE))
                {
                    var icd = this.currentIcds.Where(o => o.ICD_CODE == icdValue.ICD_CODE).FirstOrDefault();
                    if (icd.IS_LATENT_TUBERCULOSIS == 1 && string.IsNullOrEmpty(VHistreatment.TUBERCULOSIS_ISSUED_ORG_CODE))
                    {
                        ShowFrmTub();
                        valid = !string.IsNullOrEmpty(VHistreatment.TUBERCULOSIS_ISSUED_ORG_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private bool CheckMedicineGroupTuberCulosis(bool IsCheckList)
        {
            bool valid = true;
            try
            {
                if (HisConfigCFG.TuberculosisOption == "1" && ((currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO) || (IsCheckList && mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0 && mediMatyTypeADOs.Exists(o => o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO))) && VHistreatment != null && string.IsNullOrEmpty(currentTreatment.TUBERCULOSIS_CODE))
                {
                    XtraMessageBox.Show("Hồ sơ chưa có thông tin số quản lý bệnh lao. Vui lòng cập nhật thông tin trước khi thực hiện kê thuốc lao");
                    if(IsCheckList)
                        mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private bool CheckMaxExpend()
        {
            bool result = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    //Get list expend
                    List<MediMatyTypeADO> mediMatyIsExpends = mediMatyTypeADOs.Where(o => o.IsExpend).ToList();
                    if (mediMatyIsExpends != null && mediMatyIsExpends.Count > 0)
                    {
                        decimal totalPrice = mediMatyIsExpends.Sum(o => o.TotalPrice);
                        V_HIS_SERE_SERV sereServParent = currentSereServInEkip != null ? currentSereServInEkip : currentSereServ;
                        if (sereServParent != null)
                        {
                            V_HIS_SERVICE service = lstService
                       .FirstOrDefault(o => o.ID == sereServParent.SERVICE_ID && o.MAX_EXPEND.HasValue);
                            if (service != null && totalPrice > service.MAX_EXPEND)
                            {
                                MessageBox.Show(String.Format(ResourceMessage.TongTienDichVuHaoPhiVuotQuaMucGiaTriDuocCauHinh, service.MAX_EXPEND), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Nếu trong ngày có 2 thuốc cùng thuộc nhóm kháng sinh
        ///thì khi lưu đơn đưa ra cảnh báo. Trong ngày đã có thuốc kháng sinh: ABC. bạn có muốn kê (có => lưu, không => ko lưu để người dùng sửa)
        ///Kiểm tra theo ngày y lệnh (intruction_date)
        ///Hoặc nếu trong 1 đơn có 2 thuốc cùng thuộc nhóm kháng sinh thì cảnh báo khi lưu.
        ///Đơn thuốc có 2 thuốc thuộc nhóm kháng sinh: ABC, DEF. Bạn có muốn kê ?(có => lưu, không => ko lưu để người dùng sửa)
        /// </summary>
        /// <returns></returns>
        private bool CheckThuocKhangSinhTrongNgay()
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CheckThuocKhangSinhTrongNgay. 1");
                List<V_HIS_MEDICINE_TYPE> medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                var instructionTimeMedis = GetInstructionTimeMedi();
                List<V_HIS_MEDICINE_TYPE> medicineTypeInTreatments = new List<V_HIS_MEDICINE_TYPE>();
                //Khang sinh theo trong ngay
                if (sereServWithTreatment != null && sereServWithTreatment.Count > 0)
                {
                    List<string> intructionDateStrs = instructionTimeMedis != null && instructionTimeMedis.Count > 0
                        ? (from r in instructionTimeMedis select r.ToString().Substring(0, 8)).ToList() : null;
                    if (intructionDateStrs != null && intructionDateStrs.Count > 0)
                    {
                        List<HIS_SERE_SERV> sereServTodays = sereServWithTreatment.Where(o => intructionDateStrs.Contains(o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8))).ToList();
                        if (sereServTodays != null && sereServTodays.Count > 0)
                        {
                            List<long> sereServWithTreatmentServiceIds = sereServTodays.Select(o => o.SERVICE_ID).ToList();
                            medicineTypeInTreatments = medicineTypes.Where(
                                o => sereServWithTreatmentServiceIds.Contains(o.SERVICE_ID)
                                && o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS
                                ).ToList();
                        }
                    }
                }
                string mssThongBao = LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao);
                //Khang sinh trong 1 lan ke don
                List<MediMatyTypeADO> mediMatyTypeADONotEdits = this.mediMatyTypeADOs.Where(o => o.IsEdit == false).ToList();
                List<long> mediMatyTypeADONotEditServiceIds = mediMatyTypeADONotEdits.Select(o => o.SERVICE_ID).ToList();
                List<V_HIS_MEDICINE_TYPE> medicineTypeAssigns = medicineTypes.Where(
                o => mediMatyTypeADONotEditServiceIds.Contains(o.SERVICE_ID)
                && o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS).ToList();
                if (medicineTypeAssigns != null && medicineTypeAssigns.Count > 0)
                {
                    if (medicineTypeAssigns.Count > 1)
                    {
                        string medicineTypeNames = medicineTypeAssigns.Aggregate((i, j) =>
                            new V_HIS_MEDICINE_TYPE { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME;
                        string messageError = String.Format(ResourceMessage.DonThuocCoCacThuocThuocNhomKhangSinhBanCOMuonTiepTuc, medicineTypeNames);

                        DialogResult myResult;
                        myResult = DevExpress.XtraEditors.XtraMessageBox.Show(messageError, mssThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                            result = false;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mssThongBao), mssThongBao) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageError), messageError));
                    }
                    else if (medicineTypeInTreatments != null && medicineTypeInTreatments.Count > 0)
                    {
                        string medicineTypeNames = medicineTypeInTreatments.Aggregate((i, j) =>
                            new V_HIS_MEDICINE_TYPE { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME;
                        string messageError = String.Format(ResourceMessage.TrongNgayDaCoThuocKhangSinhBanCoMuonTiepTuc, medicineTypeNames);
                        DialogResult myResult;
                        myResult = DevExpress.XtraEditors.XtraMessageBox.Show(messageError, mssThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                            result = false;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageError), messageError));
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        /// <summary>
        /// Khi kê đơn thuốc (kiểm tra thuốc trong đơn)
        ///Nếu có 2 thuốc trùng hoạt chất thì đưa ra cảnh báo (kiểm tra theo hoạt chất bhyt)
        ///Đơn thuốc có 2 thuốc trùng hoạt chất: Abc, DEF, bạn có muốn tiếp tục? (có => lưu, không => ko lưu để người dùng sửa)
        ///issue: 13452
        ///----------------------------------------------
        ///issue: 21496
        ///Chỉnh sửa lại cảnh báo hoạt chất:
        ///- Kiểm tra tất cả các đơn của bn trong ngày (tất cả các mã điều trị nếu có)
        ///- Kiểm tra theo mã hoạt chất (có thể kiểm tra cắt chuỗi theo mã hoạt chất ví dụ thuốc A có mã hoạt chất '030' , thuốc B có mã hoạt chất '030 + 352' thì vẫn có thể cảnh báo)
        ///- Cảnh báo khi bổ sung thuốc
        /// </summary>
        /// <returns></returns>
        private bool CheckCungHoatChat()
        {
            bool result = true;
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    result = ValidActiveIngrBhytWorker.Valid(this.mediMatyTypeADOs, null);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckMediStockWhenEditPrescription()
        {
            bool result = true;
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && isMediMatyIsOutStock)
                {
                    var groups = this.mediMatyTypeADOs.Where(o => o.MEDI_STOCK_ID > 0).GroupBy(o => new { o.MEDI_STOCK_ID });
                    if (groups.Count() > 1)
                    {
                        MessageBox.Show(ResourceMessage.DanhSachThuocVatTuTonTai2KhoXuat, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = false;
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

        private bool CheckPrescriptionSplitOutMediStock()
        {
            bool result = true;
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && this.actionType == GlobalVariables.ActionEdit && HisConfigCFG.isPrescriptionSplitOutMediStock)
                {
                    MediMatyTypeADO mediMatyTypeADOInStock = this.mediMatyTypeADOs.FirstOrDefault(o => o.MEDI_STOCK_ID > 0);
                    MediMatyTypeADO mediMatyTypeADOOutStock = this.mediMatyTypeADOs.FirstOrDefault(o => o.MEDI_STOCK_ID == null || o.MEDI_STOCK_ID == 0);
                    if (mediMatyTypeADOInStock != null && mediMatyTypeADOOutStock != null)
                    {
                        MessageBox.Show(ResourceMessage.SuaDonThuocKhongChoPhepTachDonThuocTrongKhoVaNgoaiKho, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = false;
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

        /// <summary>
        /// Có số lượng không khả dụng trong kho
        /// </summary>
        /// <returns></returns>
        private bool CheckAmoutWarringInStock()
        {
            bool result = true;
            try
            {
                string message = "";
                var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => ((o.AmountAlert ?? 0) > 0 || o.ErrorTypeMediMatyBean == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)).ToList();
                if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                {
                    foreach (var item in mediMatyTypeInStockWarnings)
                    {
                        message += (Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Red)
                            + " : " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString((item.AmountAlert ?? 0), ConfigApplications.NumberSeperator), System.Drawing.Color.Maroon) + "; ");
                    }
                    message += ResourceMessage._KhongDuKhaDungTrongKho;
                }
                if (!String.IsNullOrEmpty(message))
                {
                    Inventec.Common.Logging.LogSystem.Debug("CheckAmoutWarringInStock____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message));
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);

                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsWarnAtcByCode(MediMatyTypeADO mdADO, List<MediMatyTypeADO> mdADOs, ref Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn)
        {
            bool valid = false;
            if (!String.IsNullOrEmpty(mdADO.ATC_CODES))
            {
                var atcCodeArr = mdADO.ATC_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcCodeArr), atcCodeArr));
                if (atcCodeArr != null && atcCodeArr.Length > 0)
                {
                    foreach (var item in atcCodeArr)
                    {
                        var mdADOsWarnDuplicates = (mdADOs != null && mdADOs.Exists(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_CODES != null && t.ATC_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item))) ? mdADOs.Where(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item)).ToList() : null;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarnDuplicates), mdADOsWarnDuplicates));
                        if (mdADOsWarnDuplicates != null && mdADOsWarnDuplicates.Count > 0)
                        {
                            valid = true;
                            if (dicAtcWarn == null) dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();
                            dicAtcWarn.Add(item, mdADOsWarnDuplicates);
                        }
                    }
                }
            }
            return valid;
        }
        bool IsWarnAtcByGroupCode(MediMatyTypeADO mdADO, List<MediMatyTypeADO> mdADOs, ref Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn)
        {
            bool valid = false;
            if (!String.IsNullOrEmpty(mdADO.ATC_GROUP_CODES))
            {
                var atcCodeArr = mdADO.ATC_GROUP_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcCodeArr), atcCodeArr));
                if (atcCodeArr != null && atcCodeArr.Length > 0)
                {
                    foreach (var item in atcCodeArr)
                    {
                        var mdADOsWarnDuplicates = (mdADOs != null && mdADOs.Exists(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_GROUP_CODES != null && t.ATC_GROUP_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item))) ? mdADOs.Where(t => mdADO.SERVICE_ID != t.SERVICE_ID && t.ATC_GROUP_CODES.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item)).ToList() : null;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarnDuplicates), mdADOsWarnDuplicates));
                        if (mdADOsWarnDuplicates != null && mdADOsWarnDuplicates.Count > 0)
                        {
                            valid = true;
                            if (dicAtcWarn == null) dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();
                            dicAtcWarn.Add(item, mdADOsWarnDuplicates);
                        }
                    }
                }
            }
            return valid;
        }
        private bool CheckOverlapWarningOption()
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.AtcCodeOverlarWarningOption == "1")
                {
                    var mediMatyTypeOverlapWarnings = this.mediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.ATC_CODES) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)).ToList();
                    if (mediMatyTypeOverlapWarnings != null && mediMatyTypeOverlapWarnings.Count > 0)
                    {
                        string medicineTypeNames = "";
                        var atcDatas = BackendDataWorker.Get<HIS_ATC>();
                        atcDatas = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                        Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();

                        foreach (var item in mediMatyTypeOverlapWarnings)
                        {
                            Dictionary<string, List<MediMatyTypeADO>> dicAtcWarnOne = new Dictionary<string, List<MediMatyTypeADO>>();
                            List<MediMatyTypeADO> mdADOsWarn = new List<MediMatyTypeADO>();
                            if (IsWarnAtcByCode(item, mediMatyTypeOverlapWarnings, ref dicAtcWarnOne))
                            {
                                if (dicAtcWarnOne != null && dicAtcWarnOne.Count > 0)
                                {
                                    foreach (var itemAtcSearch in dicAtcWarnOne)
                                    {
                                        var atc = atcDatas != null ? atcDatas.Where(kk => kk.ATC_CODE == itemAtcSearch.Key).FirstOrDefault() : null;
                                        if (atc != null)
                                        {
                                            mdADOsWarn = itemAtcSearch.Value;
                                            if (dicAtcWarn.ContainsKey(atc.ATC_CODE))
                                            {
                                                var listValue = dicAtcWarn[atc.ATC_CODE];
                                                listValue.AddRange(mdADOsWarn);
                                                dicAtcWarn[atc.ATC_CODE] = listValue.Distinct().ToList();
                                            }
                                            else
                                            {
                                                dicAtcWarn.Add(atc.ATC_CODE, mdADOsWarn);
                                            }
                                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atc), atc)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarn), mdADOsWarn));
                                        }
                                        else
                                        {
                                            Inventec.Common.Logging.LogSystem.Debug("Khong tim thay atc theo ma:" + itemAtcSearch.Key
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcDatas), atcDatas)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                        }
                                    }
                                }

                            }
                        }

                        if (dicAtcWarn != null && dicAtcWarn.Count > 0)
                        {
                            foreach (var aw in dicAtcWarn)
                            {
                                medicineTypeNames += "[" + spinSoNgay.Text + "]" + (aw.Value.Aggregate((i, j) => new MediMatyTypeADO { ACTIVE_INGR_BHYT_NAME = i.ACTIVE_INGR_BHYT_NAME + " - " + j.ACTIVE_INGR_BHYT_NAME }).ACTIVE_INGR_BHYT_NAME) + " - (" + aw.Key + " - " + GetAtcNameByCode(aw.Key) + ")" + "\r\n";
                            }
                        }

                        if (!String.IsNullOrEmpty(medicineTypeNames))
                        {
                            DialogResult myResult;
                            string tt = ResourceMessage.BanCoMuonTiepTuc.Replace("{1}.", "");

                            myResult = DevExpress.XtraEditors.XtraMessageBox.Show(
                         String.Format(tt, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format(ResourceMessage.CacThuocCoCungMaATC_BanCoMuonTiepTuc, medicineTypeNames), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br)), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True);
                            if (myResult != System.Windows.Forms.DialogResult.Yes)
                                result = false;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeNames), medicineTypeNames)
                            + "_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicAtcWarn), dicAtcWarn));
                    }
                }
                else if (HisConfigCFG.AtcCodeOverlarWarningOption == "2"){
                    var mediMatyTypeOverlapWarnings = this.mediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.ATC_GROUP_CODES) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)).ToList();
                    if (mediMatyTypeOverlapWarnings != null && mediMatyTypeOverlapWarnings.Count > 0)
                    {
                        string medicineTypeNames = "";
                        var atcDatas = BackendDataWorker.Get<HIS_ATC_GROUP>();
                        atcDatas = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                        Dictionary<string, List<MediMatyTypeADO>> dicAtcWarn = new Dictionary<string, List<MediMatyTypeADO>>();

                        foreach (var item in mediMatyTypeOverlapWarnings)
                        {
                            Dictionary<string, List<MediMatyTypeADO>> dicAtcWarnOne = new Dictionary<string, List<MediMatyTypeADO>>();
                            List<MediMatyTypeADO> mdADOsWarn = new List<MediMatyTypeADO>();
                            if (IsWarnAtcByGroupCode(item, mediMatyTypeOverlapWarnings, ref dicAtcWarnOne))
                            {
                                if (dicAtcWarnOne != null && dicAtcWarnOne.Count > 0)
                                {
                                    foreach (var itemAtcSearch in dicAtcWarnOne)
                                    {
                                        var atc = atcDatas != null ? atcDatas.Where(kk => kk.ATC_GROUP_CODE == itemAtcSearch.Key).FirstOrDefault() : null;
                                        if (atc != null)
                                        {
                                            mdADOsWarn = itemAtcSearch.Value;
                                            if (dicAtcWarn.ContainsKey(atc.ATC_GROUP_CODE))
                                            {
                                                var listValue = dicAtcWarn[atc.ATC_GROUP_CODE];
                                                listValue.AddRange(mdADOsWarn);
                                                dicAtcWarn[atc.ATC_GROUP_CODE] = listValue.Distinct().ToList();
                                            }
                                            else
                                            {
                                                dicAtcWarn.Add(atc.ATC_GROUP_CODE, mdADOsWarn);
                                            }
                                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atc), atc)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarn), mdADOsWarn));
                                        }
                                        else
                                        {
                                            Inventec.Common.Logging.LogSystem.Debug("Khong tim thay atc theo ma:" + itemAtcSearch.Key
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => atcDatas), atcDatas)
                                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                        }
                                    }
                                }

                            }
                        }

                        if (dicAtcWarn != null && dicAtcWarn.Count > 0)
                        {
                            foreach (var aw in dicAtcWarn)
                            {
                                medicineTypeNames += (aw.Value.Aggregate((i, j) => new MediMatyTypeADO { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + " - " + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME) + " - ("+ GetAtcNameByGroupCode(aw.Key) + ")" + "\r\n";
                            }
                        }

                        if (!String.IsNullOrEmpty(medicineTypeNames))
                        {
                            DialogResult myResult;
                            string tt = ResourceMessage.BanCoMuonTiepTuc.Replace("{1}.", "");

                            myResult = DevExpress.XtraEditors.XtraMessageBox.Show(
                         String.Format(tt, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(String.Format(ResourceMessage.CacThuocCoCungMaGroupATC_BanCoMuonTiepTuc, medicineTypeNames), Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br)), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True);
                            if (myResult != System.Windows.Forms.DialogResult.Yes)
                                result = false;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeNames), medicineTypeNames)
                            + "_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicAtcWarn), dicAtcWarn));
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

        private string GetAtcNameByCode(string code)
        {
            string rs = "";
            var atcDatas = BackendDataWorker.Get<HIS_ATC>();
            var atc = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.ATC_CODE == code).FirstOrDefault() : null;
            rs = atc != null ? atc.ATC_NAME : "";
            return rs;
        }
        private string GetAtcNameByGroupCode(string code)
        {
            string rs = "";
            var atcDatas = BackendDataWorker.Get<HIS_ATC_GROUP>();
            var atc = atcDatas != null && atcDatas.Count > 0 ? atcDatas.Where(o => o.ATC_GROUP_CODE == code).FirstOrDefault() : null;
            rs = atc != null ? atc.ATC_GROUP_NAME : "";
            return rs;
        }

        private string GetContrainDationNameByCode(string code)
        {
            string rs = "";
            var contrainDatas = BackendDataWorker.Get<HIS_CONTRAINDICATION>();
            var contrainDation = contrainDatas != null && contrainDatas.Count > 0 ? contrainDatas.Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(code)).FirstOrDefault() : null;
            rs = contrainDation != null ? contrainDation.CONTRAINDICATION_NAME : "";
            return rs;
        }

        ///#17940
        ///Trong trường hợp kê đơn nhiều ngày thì chỉ cho phép kê với những ngày y lệnh nhỏ hơn hoặc bằng ngày hết hạn 
        ///sử dụng. Các ngày y lệnh lớn hơn ngày hết hạn thì chặn lại, không cho lưu. Khi lưu sẽ có cảnh báo, lấy ra 
        ///ngày hết hạn của thuốc. Xử lý thất bại.Thuốc/ vật tư  (Mã thuốc. vật tư - tên thuốc, vật tư) có hạn sử dụng < thời gian y lệnh. Ngày hết hạn:..... --- Ngày y lệnh:......
        private bool CheckMediMatyDontPresExpiredTime()
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    string message = "";
                    List<MediMatyTypeADO> mediMatyTypeWarnings =
                        this.mediMatyTypeADOs.Where(o => ((o.EXPIRED_DATE ?? 0) > 0)).ToList();
                    if (mediMatyTypeWarnings != null && mediMatyTypeWarnings.Count > 0)
                    {
                        foreach (var item in mediMatyTypeWarnings)
                        {
                            message += String.Format(ResourceMessage.WarnThuocVatTuCoHanSuDungNhoHonThoiGianYLenh,
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_CODE, System.Drawing.Color.Maroon),
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Maroon),
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0), System.Drawing.Color.Maroon),
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.DateTime.Convert.TimeNumberToDateString(InstructimeForWarnPresExpiredTime(item)), System.Drawing.Color.Maroon) + " \n");
                        }
                    }
                    if (!String.IsNullOrEmpty(message))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);

                        result = false;
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

        long InstructimeForWarnPresExpiredTime(MediMatyTypeADO item)
        {
            try
            {
                return (item.IntructionTimeSelecteds != null && item.IntructionTimeSelecteds.Count > 0 ? item.IntructionTimeSelecteds.First() : this.intructionTimeSelecteds.First());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        private bool CheckAmoutWarringNumber()
        {
            bool result = true;
            try
            {
                string message = "";
                var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => (o.AMOUNT ?? 0) == 0 || o.ErrorTypeAmount == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning).ToList();
                if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                {
                    foreach (var item in mediMatyTypeInStockWarnings)
                    {
                        message += (Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Red)
                            + " : " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString((item.AmountAlert ?? 0), ConfigApplications.NumberSeperator), System.Drawing.Color.Maroon) + "; ");
                    }
                    message += ResourceMessage._KhongNhapSoLuongKe;
                }
                if (!String.IsNullOrEmpty(message))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);

                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsWarnChongChiDinhOne(string contrainIds, List<MediMatyTypeADO> mdADOs, ref Dictionary<string, List<MediMatyTypeADO>> dicContraindicationWarn)
        {
            bool valid = false;
            if (!String.IsNullOrEmpty(contrainIds))
            {
                var contrainIdsArr = contrainIds.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => contrainIdsArr), contrainIdsArr));
                if (contrainIdsArr != null && contrainIdsArr.Length > 0)
                {
                    contrainIdsArr = contrainIdsArr.Distinct().ToArray();
                    foreach (var item in contrainIdsArr)
                    {
                        var mdADOsWarnDuplicates = (mdADOs != null && mdADOs.Exists(t => t.CONTRAINDICATION_IDS != null && t.CONTRAINDICATION_IDS.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item))) ? mdADOs.Where(t => t.CONTRAINDICATION_IDS.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).Contains(item)).ToList() : null;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mdADOsWarnDuplicates), mdADOsWarnDuplicates));
                        if (mdADOsWarnDuplicates != null && mdADOsWarnDuplicates.Count > 0)
                        {
                            valid = true;
                            if (dicContraindicationWarn == null) dicContraindicationWarn = new Dictionary<string, List<MediMatyTypeADO>>();
                            dicContraindicationWarn.Add(item, mdADOsWarnDuplicates);
                        }
                    }
                }
            }
            return valid;
        }

        private bool CheckChongChiDinhWarring()
        {
            bool result = true;
            try
            {
                if (this.currentTreatmentWithPatientType != null && !String.IsNullOrEmpty(this.currentTreatmentWithPatientType.CONTRAINDICATION_IDS))
                {
                    var mediMatyTypeOChongChiDinhWarnings = this.mediMatyTypeADOs.Where(o => !String.IsNullOrEmpty(o.CONTRAINDICATION_IDS) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)).ToList();
                    if (mediMatyTypeOChongChiDinhWarnings != null && mediMatyTypeOChongChiDinhWarnings.Count > 0)
                    {
                        string medicineTypeNamesWarn = "";
                        var contrainDatas = BackendDataWorker.Get<HIS_CONTRAINDICATION>();
                        contrainDatas = contrainDatas != null && contrainDatas.Count > 0 ? contrainDatas.Where(o => o.IS_ACTIVE == 1).ToList() : null;
                        Dictionary<string, List<MediMatyTypeADO>> dicContrainWarn = new Dictionary<string, List<MediMatyTypeADO>>();

                        List<MediMatyTypeADO> mdADOsWarn = new List<MediMatyTypeADO>();
                        if (IsWarnChongChiDinhOne(this.currentTreatmentWithPatientType.CONTRAINDICATION_IDS, mediMatyTypeOChongChiDinhWarnings, ref dicContrainWarn))
                        {
                            if (dicContrainWarn != null && dicContrainWarn.Count > 0)
                            {
                                foreach (var aw in dicContrainWarn)
                                {
                                    medicineTypeNamesWarn += String.Format(ResourceMessage.ThuocXChongChiDinhVoiBenhNhanY, (aw.Value.Aggregate((i, j) => new MediMatyTypeADO { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME), GetContrainDationNameByCode(aw.Key)) + "\r\n";
                                }
                            }
                        }
                        if (!String.IsNullOrEmpty(medicineTypeNamesWarn))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(medicineTypeNamesWarn, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DevExpress.Utils.DefaultBoolean.True);
                            result = false;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeNamesWarn), medicineTypeNamesWarn)
                            + "_______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicContrainWarn), dicContrainWarn));
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

        private bool WarningAlertWarningFeeProcess()
        {
            bool valid = true;
            try
            {
                string messageErr = "";
                decimal tongtienThuocPhatSinh = 0;

                var bhyt__Exists = this.mediMatyTypeADOs
                          .Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                              && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && !o.IsExpend).ToList();
                //Kiểm tra tiền bhyt đã kê vượt mức giới hạn chưa

                if (bhyt__Exists != null
                    && bhyt__Exists.Count > 0
                    )
                {
                    foreach (var item in bhyt__Exists)
                    {
                        tongtienThuocPhatSinh += (item.TotalPrice);
                    }
                }

                AlertWarningFeeManager alertWarningFeeManager = new AlertWarningFeeManager();
                if (!alertWarningFeeManager.RunOption(treatmentId, currentHisPatientTypeAlter.PATIENT_TYPE_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_ID, currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE, HisConfigCFG.PatientTypeId__BHYT, totalHeinPriceByTreatment, HisConfigCFG.IsUsingWarningHeinFee, tongtienThuocPhatSinh, ref messageErr, true))
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// 1. Bổ sung cấu hình hệ thống:
        ///- HIS.DESKTOP.PRESCRIPTION.WARNING_ALLERGENIC_OPTION
        ///- Tùy chọn cảnh báo dị ứng thuốc: 1 - Cảnh báo theo thẻ dị ứng của bệnh nhân. 0 - Không cảnh báo. Mặc định 0.
        ///2. Sửa chức năng "Kê đơn":
        ///- Mở chức năng Kiểm tra key cấu hình HIS.DESKTOP.PRESCRIPTION.WARNING_ALLERGENIC_OPTION nếu giá trị 1 thì:
        ///+ Gọi api lấy tất cả các thông tin dị ứng của bệnh nhân (bảng HIS_ALLERGENIC, lọc thẻ TDL_PATIENT_ID và chỉ lấy các dữ liệu có MEDICINE_TYPE_ID. Nếu dùng tiến trình hoặc bất đồng bộ để lấy).
        ///- Khi lưu kê đơn nếu key cấu hình HIS.DESKTOP.PRESCRIPTION.WARNING_ALLERGENIC_OPTION nếu giá trị 1 thì: Kiểm tra bệnh nhân có thông tin dị ứng không và có thuốc nào được kê nằm trong thông tin dị ứng đấy không.
        ///+ Nếu không có thì lưu như bình thường.
        ///+ Nếu có thì show cảnh báo:
        ///"Cảnh báo dị ứng thuốc:
        ///1. Tên thuốc 1: Biểu hiện lâm sang 1 (trường CLINICAL_EXPRESSION trong HIS_ALLERGENIC).
        ///2. Tên thuốc 2: Biểu hiện lâm sàng 2 (trường CLINICAL_EXPRESSION trong HIS_ALLERGENIC).
        ///...
        ///Bạn có muốn tiếp tục?".
        ///+ Nếu nhấn có thì lưu bình thường.
        ///+ Nếu nhấn không thì không làm gì.
        /// </summary>
        /// <returns></returns>
        private bool WarningAllegericOptionProcess()
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("WarningAllegericOptionProcess.1");
                if (HisConfigCFG.WarningAllegericOption == GlobalVariables.CommonStringTrue)
                {
                    Inventec.Common.Logging.LogSystem.Info("WarningAllegericOptionProcess.2");
                    if (this.allergenics == null || this.allergenics.Count == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong co du lieu thong tin di ung cua benh nhan____co cau hinh WarningAllegericOption= " + HisConfigCFG.WarningAllegericOption);
                        return result;
                    }
                    var metyIdsInAllergenic = this.allergenics.Exists(k => k.MEDICINE_TYPE_ID.HasValue) ? this.allergenics.Where(k => k.MEDICINE_TYPE_ID.HasValue).Select(k => k.MEDICINE_TYPE_ID).ToList() : null;

                    var mediMatyTypeWarnings = (metyIdsInAllergenic != null && metyIdsInAllergenic.Count > 0) ? this.mediMatyTypeADOs.Where(o => (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && metyIdsInAllergenic.Contains(o.ID)).ToList() : null;
                    string messageError = "";
                    Dictionary<long, string> dicWarn = new Dictionary<long, string>();
                    if (mediMatyTypeWarnings != null && mediMatyTypeWarnings.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("WarningAllegericOptionProcess.3");
                        int indexRowError = 0;
                        messageError = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(ResourceMessage.CanhBaoDiUngThuoc, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                        foreach (var item in mediMatyTypeWarnings)
                        {
                            if (!dicWarn.ContainsKey(item.ID))
                            {
                                HIS_ALLERGENIC allergencic = this.allergenics.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.ID);
                                if (allergencic != null && !String.IsNullOrEmpty(allergencic.CLINICAL_EXPRESSION))
                                {
                                    indexRowError++;
                                    string messageErrorRow = String.Format("{0}. {1}: {2}.", indexRowError, item.MEDICINE_TYPE_NAME, allergencic.CLINICAL_EXPRESSION);
                                    dicWarn.Add(item.ID, messageErrorRow);
                                    messageError += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageErrorRow, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Info("Khong tim thay canh bao di ung cua thuoc hoac canh bao khong co gia tri truong CLINICAL_EXPRESSION____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => allergencic), allergencic));
                                }
                            }
                        }

                        messageError = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageError, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + ResourceMessage.BanCoMuonTiepTuc.Replace("{0} {1}. ", "");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageError), messageError));
                        DialogResult myResult;
                        if (dicWarn.Count > 0)
                        {
                            myResult = DevExpress.XtraEditors.XtraMessageBox.Show(messageError, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (myResult != DialogResult.Yes)
                            {
                                result = false;
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("WarningAllegericOptionProcess.4");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// 1. Bổ sung key cấu hình hệ thống:
        ///HIS.Desktop.Plugins.AssignPrescription.WarningWhenNotFinishingIncaseOfOutPatient
        ///2. Sửa chức năng "Kê đơn phòng khám":
        ///- Khi key cấu hình trên có giá trị = 1 thì xử lý:
        ///Trong trường hợp loại là "Kê đơn phòng khám" và công khám đang xử lý là khám chính (HIS_SERVICE_REQ có IS_MAIN_EXAM = 1), và khi người dùng nhấn "Lưu" hoặc "Lưu in" nhưng ko nhập thông tin kết thúc điều trị thì hiển thị cảnh báo:
        ///"Bạn có muốn nhập thông tin kết thúc điều trị không?"
        ///+ Nếu người dùng chọn "Đồng ý" thì tắt thông báo vào tự động check vào checkbox "Kết thúc điều trị" và mặc định focus vào combobox "Loại ra viện" để người dùng nhập
        ///+ Nếu người dùng chọn "Không" thì thực hiện gọi api để xử lý lưu đơn như hiện tại
        /// </summary>
        /// <returns></returns>
        private bool WarningWhenNotFinishingIncaseOfOutPatientProcess(bool isHasTreatmentFinishChecked)
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("WarningWhenNotFinishingIncaseOfOutPatientProcess.1");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.WarningWhenNotFinishingIncaseOfOutPatient), HisConfigCFG.WarningWhenNotFinishingIncaseOfOutPatient)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.WarningWhenNotFinishingIncaseOfOutPatient), HisConfigCFG.WarningWhenNotFinishingIncaseOfOutPatient)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsTreatmentIn), GlobalStore.IsTreatmentIn)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalStore.IsCabinet), GlobalStore.IsCabinet)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isHasTreatmentFinishChecked), isHasTreatmentFinishChecked)
                    + Inventec.Common.Logging.LogUtil.TraceData("IS_MAIN_EXAM", (this.serviceReqMain != null ? this.serviceReqMain.IS_MAIN_EXAM : null)));
                if (HisConfigCFG.WarningWhenNotFinishingIncaseOfOutPatient == GlobalVariables.CommonStringTrue && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM == 1 && !isHasTreatmentFinishChecked)
                {
                    Inventec.Common.Logging.LogSystem.Info("WarningWhenNotFinishingIncaseOfOutPatientProcess.2");
                    string messageError = ResourceMessage.BanCoMuonNhapThongTinKetThucDieuTriKhong;
                    Inventec.Common.Logging.LogSystem.Warn("WarningWhenNotFinishingIncaseOfOutPatientProcess.3:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageError), messageError));
                    DialogResult myResult;
                    myResult = DevExpress.XtraEditors.XtraMessageBox.Show(messageError, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                    if (myResult == DialogResult.Yes)
                    {
                        result = true;
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("WarningWhenNotFinishingIncaseOfOutPatientProcess.4");
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
