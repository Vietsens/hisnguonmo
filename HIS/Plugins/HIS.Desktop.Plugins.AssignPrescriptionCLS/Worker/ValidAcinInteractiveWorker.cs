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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS
{
    class ValidAcinInteractiveWorker
    {
        /// <summary>
        /// Kiểm tra có tồn tại cấu hình mức cảnh báo căn cứ theo hoạt chất có trong các loại thuốc, 
        /// thực hiện cảnh báo cho các loại thuốc có trong, kiểu dữ liệu số nguyên.
        /// Nếu cấu hình này khác null thì nếu mức cảnh báo > cấu hình thì client sẽ thực hiện chặn không cho phép kê.      
        /// </summary>
        internal static bool ValidGrade(object data, List<MediMatyTypeADO> MediMatyTypeADOs)
        {
            bool valid = true;
            try
            {
                //Kiểm tra nếu có cấu hình key 'AcinInteractive__Grade' trên ccc thì mới tiếp tục, không có thì bỏ qua không xử lý gì
                if (HisConfigCFG.AcinInteractive__Grade.HasValue
                    && MediMatyTypeADOs != null && MediMatyTypeADOs.Count > 0)
                {
                    var acinInteractives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE>();
                    string messageErr = "";
                    if (data == null) throw new ArgumentNullException("data");
                    MediMatyTypeADO mediMatyTypeADOTemp = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOTemp, data);
                    if (mediMatyTypeADOTemp == null) throw new ArgumentNullException("mediMatyTypeADOTemp");

                    if (mediMatyTypeADOTemp.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        //Lấy dữ liệu cấu hình loại thuốc - hoạt chất của thuốc đang chọn
                        //Có được 1 tập các medicineTypeAcin
                        var mediId__Adds = new List<long>();
                        mediId__Adds.Add(mediMatyTypeADOTemp.ID);
                        var medicineTypeAcin__Adds = GetMedicineTypeAcinByMedicineType(mediId__Adds);
                        if (medicineTypeAcin__Adds != null && medicineTypeAcin__Adds.Count > 0)
                        {
                            //Lấy tất cả các thuốc đã chọn kê trong grid bên phải
                            //Mỗi thuốc đã kê lấy ra tất cả các cấu hình loại thuốc - hoạt chất
                            var mediId__InGrids = MediMatyTypeADOs.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).Select(o => o.ID).Distinct().ToList();
                            foreach (var item in medicineTypeAcin__Adds)
                            {
                                var medicineTypeAcin__InGrids = GetMedicineTypeAcinByMedicineType(mediId__InGrids);
                                if (medicineTypeAcin__InGrids != null && medicineTypeAcin__InGrids.Count > 0)
                                {
                                    //Duyệt danh sách các hoạt chất của thuốc đang chọn so sánh với các hoạt chất của các thuốc đã kê, 
                                    //nếu có 2 hoạt chất bị xung đột nhau (có grade bằng hoặc lớn hơn cấu hình giới hạn grade trên ccc)
                                    //thì thêm vào chuỗi thông báo có dạng: Hoạt chất {0} có trong thuốc {1} xung đột với hoạt chất {2} có trong thuốc {3} Thông tin xung đột: {4}. Mức độ: {5}.
                                    var medicineTypeAcin__conflicts =
                                        (
                                        from n in medicineTypeAcin__InGrids
                                        from ac in acinInteractives
                                        where
                                         (item.ACTIVE_INGREDIENT_ID == ac.ACTIVE_INGREDIENT_ID
                                         && (ac.INTERACTIVE_GRADE ?? -1) > HisConfigCFG.AcinInteractive__Grade
                                         && n.ACTIVE_INGREDIENT_ID == ac.CONFLICT_ID) ||

                                          (item.ACTIVE_INGREDIENT_ID == ac.CONFLICT_ID
                                         && (ac.INTERACTIVE_GRADE ?? -1) > HisConfigCFG.AcinInteractive__Grade
                                         && n.ACTIVE_INGREDIENT_ID == ac.ACTIVE_INGREDIENT_ID)

                                        select new
                                        {
                                            A = n,
                                            B = ac
                                        }
                                        ).ToList();
                                    if (medicineTypeAcin__conflicts != null && medicineTypeAcin__conflicts.Count > 0)
                                    {
                                        foreach (var item1 in medicineTypeAcin__conflicts)
                                        {
                                            //messageErr += String.Format(ResourceMessage.AcinInteractive__OverGrade, item1.A.ACTIVE_INGREDIENT_NAME, item1.A.MEDICINE_TYPE_NAME, item.ACTIVE_INGREDIENT_NAME, item.MEDICINE_TYPE_NAME, item1.B.DESCRIPTION, item1.B.GRADE);

                                            string messageErrTemp = String.Format(ResourceMessage.AcinInteractive__OverGrade, item1.A.ACTIVE_INGREDIENT_NAME, item1.A.MEDICINE_TYPE_NAME, item.ACTIVE_INGREDIENT_NAME, item.MEDICINE_TYPE_NAME, item1.B.DESCRIPTION, item1.B.INTERACTIVE_GRADE);
                                            if (!messageErr.Contains(messageErrTemp))
                                            {
                                                messageErr += messageErrTemp;
                                            }

                                            messageErr += "\r\n";
                                            valid = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!valid && !String.IsNullOrEmpty(messageErr))
                    {
                        if (HisConfigCFG.IsBlockWhileAcinByMedicineType)
                        {
                            MessageManager.Show(messageErr);
                            Inventec.Common.Logging.LogSystem.Debug("ValidGrade. "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageErr), messageErr));
                        }
                        else
                        {
                            DialogResult myResult;
                            myResult = MessageBox.Show(messageErr + ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonBoSung, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (myResult != DialogResult.Yes)
                            {
                                valid = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// Khi kê đơn --> Nếu phát hiện thuốc có hoạt chất dấu * --> đưa cảnh báo Thuốc có hoạt chất dấu cần hội chẩn --> Bạn có muốn hội chẩn không? *
        ///Nếu đồng ý thì tự động mở form BB hội chẩn nhập dữ liệu --> tự động insert thông tin hành chính, thông tin thuốc dấu * (tên thuốc, liều dùng, cách dùng) --> Lưu và in ra report như hình Mẫu biên bản hội chẩn thuốc dấu sao.png
        ///Nếu không đồng ý thì chỉ lưu đơn bình thường
        /// </summary>
        /// <param name="MediMatyTypeADOs"></param>
        /// <returns></returns>
        internal static bool ValidConsultationReqiured(int actionType, HIS_SERVICE_REQ oldServiceReq, Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, List<MediMatyTypeADO> MediMatyTypeADOs)
        {
            bool valid = true;
            try
            {
                //Kiểm tra nếu có cấu hình key 'AcinInteractive__Grade' trên ccc thì mới tiếp tục, không có thì bỏ qua không xử lý gì
                if (MediMatyTypeADOs != null && MediMatyTypeADOs.Count > 0)
                {
                    foreach (var mediTemp in MediMatyTypeADOs)
                    {
                        List<string> messageErr = new List<string>();
                        List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicineForPrints = new List<HIS_DEBATE>();
                        if (mediTemp.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            //Lấy dữ liệu cấu hình loại thuốc - hoạt chất của thuốc đang chọn
                            //Có được 1 tập các medicineTypeAcin
                            var mediId__Adds = new List<long>();
                            mediId__Adds.Add(mediTemp.ID);
                            var medicineTypeAcin__Adds = GetMedicineTypeAcinByMedicineType(mediId__Adds);
                            if (medicineTypeAcin__Adds != null && medicineTypeAcin__Adds.Count > 0)
                            {
                                var acinIngredients = GetActiveIngredientByMedicineType(medicineTypeAcin__Adds.Select(o => o.ACTIVE_INGREDIENT_ID).ToList());

                                if (acinIngredients != null && acinIngredients.Count > 0)
                                {
                                    messageErr.Add(mediTemp.MEDICINE_TYPE_NAME);
                                    medicineForPrints.Add(new MOS.EFMODEL.DataModels.HIS_DEBATE()
                                    {
                                        MEDICINE_TYPE_NAME = mediTemp.MEDICINE_TYPE_NAME,
                                        MEDICINE_USE_FORM_NAME = mediTemp.MEDICINE_USE_FORM_NAME,
                                        MEDICINE_TUTORIAL = mediTemp.TUTORIAL,
                                        MEDICINE_USE_TIME = mediTemp.UseTimeTo,
                                        MEDICINE_CONCENTRA = mediTemp.CONCENTRA,
                                    });
                                }
                            }

                            if (messageErr != null && messageErr.Count > 0)
                            {
                                DialogResult myResult;
                                bool showMess = false;
                                List<object> listArgs = new List<object>();
                                if (actionType == GlobalVariables.ActionEdit && oldServiceReq != null && oldServiceReq.ID > 0)
                                {
                                    MOS.Filter.HisDebateFilter debateFilter = new MOS.Filter.HisDebateFilter();
                                    debateFilter.TREATMENT_ID = oldServiceReq.TREATMENT_ID;
                                    debateFilter.ORDER_DIRECTION = "ASC";
                                    debateFilter.ORDER_FIELD = "ID";
                                    var debateUsers = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_GET, ApiConsumer.ApiConsumers.MosConsumer, debateFilter, null);
                                    var debate = (debateUsers != null && debateUsers.Count > 0) ? debateUsers.Where(o =>
                                           o.MEDICINE_TYPE_NAME == mediTemp.MEDICINE_TYPE_NAME
                                           && o.MEDICINE_USE_TIME == mediTemp.UseTimeTo
                                           && o.MEDICINE_TUTORIAL == mediTemp.TUTORIAL
                                           && o.MEDICINE_USE_FORM_NAME == mediTemp.MEDICINE_USE_FORM_NAME
                                           && o.MEDICINE_CONCENTRA == mediTemp.CONCENTRA
                                           ).FirstOrDefault() : null;

                                    showMess = (debate != null && debate.ID > 0);
                                    if (showMess)
                                    {
                                        listArgs.Add(debate);
                                        Inventec.Common.Logging.LogSystem.Debug("Sua don thuoc tim thay bien ban hoi chan dau sao____Service_reqId =" + oldServiceReq.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageErr), messageErr));

                                    }
                                }
                                if (!showMess)
                                {
                                    myResult = MessageBox.Show(String.Format(ResourceMessage.ThuocCoDauCanHoiChan_BanCoMuonHoiChan, String.Join(",", messageErr.ToArray())), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    showMess = (myResult == DialogResult.Yes);
                                    if (showMess)
                                    {
                                        CommonParam param = new CommonParam();
                                        MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                                        filter.TREATMENT_ID = treatmentId;
                                        filter.ORDER_DIRECTION = "DESC";
                                        filter.ORDER_FIELD = "MODIFY_TIME";
                                        HIS_SERVICE_REQ rs = new HIS_SERVICE_REQ();
                                        rs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                                        listArgs.Add(rs);
                                    }
                                }

                                if (showMess)
                                {
                                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");

                                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                    {
                                        listArgs.Add(medicineForPrints);
                                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                                        ((Form)extenceInstance).ShowDialog();
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
            return valid;
        }

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcinByMedicineType(List<long> medicineTypeIds)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeIds.Contains(o.MEDICINE_TYPE_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT> GetActiveIngredientByMedicineType(List<long> AcinIds)
        {
            List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT>()
                    .Where(o => AcinIds.Contains(o.ID)).Where(o => o.IS_CONSULTATION_REQUIRED == GlobalVariables.CommonNumberTrue).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
