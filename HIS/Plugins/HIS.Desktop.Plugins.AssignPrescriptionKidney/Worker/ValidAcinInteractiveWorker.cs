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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney
{
    class ValidAcinInteractiveWorker
    {
        /// <summary>
        /// Kiểm tra có tồn tại cấu hình mức cảnh báo căn cứ theo hoạt chất có trong các loại thuốc, 
        /// thực hiện cảnh báo cho các loại thuốc có trong, kiểu dữ liệu số nguyên.
        /// Nếu cấu hình này khác null thì nếu mức cảnh báo > cấu hình thì client sẽ thực hiện chặn không cho phép kê.      
        /// </summary>
        internal static bool Valid(object data, List<MediMatyTypeADO> MediMatyTypeADOs)
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
                                         && (ac.GRADE ?? -1) > HisConfigCFG.AcinInteractive__Grade
                                         && n.ACTIVE_INGREDIENT_ID == ac.CONFLICT_ID) ||

                                          (item.ACTIVE_INGREDIENT_ID == ac.CONFLICT_ID
                                         && (ac.GRADE ?? -1) > HisConfigCFG.AcinInteractive__Grade
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

                                            string messageErrTemp = String.Format(ResourceMessage.AcinInteractive__OverGrade, item1.A.ACTIVE_INGREDIENT_NAME, item1.A.MEDICINE_TYPE_NAME, item.ACTIVE_INGREDIENT_NAME, item.MEDICINE_TYPE_NAME, item1.B.DESCRIPTION, item1.B.GRADE);
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
                        }
                        else
                        {
                            DialogResult myResult;
                            myResult = MessageBox.Show(messageErr + ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonBoSung, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult != DialogResult.OK)
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

        private static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcinByMedicineType(List<long> medicineTypeIds)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> result = null;
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeIds.Contains(o.MEDICINE_TYPE_ID)).ToList();

                var medis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
