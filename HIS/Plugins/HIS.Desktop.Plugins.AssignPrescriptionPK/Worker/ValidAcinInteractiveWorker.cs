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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Utility;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    class ValidAcinInteractiveWorker
    {
        internal static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> currentMedicineTypeAcins { get; set; }
        internal static List<L_HIS_EXP_MEST_MEDICINE_1> CurrentLExpMestMedicine1s { get; set; }
        static int actChooseDebateType;
        internal const int actChooseDebateType__Create = 1;
        internal const int actChooseDebateType__None = 2;
        static string _InteractionReason = "";
        static string strtxtLoginName;
        static long requestDepartmentId;
        static long treatmentId;
        static IcdInputADO ucIcdValue;
        static IcdInputADO icdCauseValue;
        static SecondaryIcdDataADO secondaryIcdValue;

        static long? MedicineTypeId_Interactive1 = null, MedicineTypeId_Interactive2 = null, InteractiveGradeId = null;

        /// <summary>
        ///- Kiểm tra tất cả các đơn của bn trong ngày (tất cả các mã điều trị nếu có)
        ///- Kiểm tra theo mã hoạt chất (có thể kiểm tra cắt chuỗi theo mã hoạt chất ví dụ thuốc A có mã hoạt chất '030' , thuốc B có mã hoạt chất '030 + 352' thì vẫn có thể cảnh báo)
        ///- Cảnh báo khi bổ sung thuốc
        /// </summary>
        internal static bool ValidSameAcin(List<MediMatyTypeADO> mediMatyTypeADOs, MediMatyTypeADO mediMatyTypeADO)
        {
            bool valid = true;
            try
            {
                string medicineTypeNames = "";
                List<string> medicineTypeCodes = new List<string>();
                var medimatys = mediMatyTypeADOs != null ? mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM).ToList() : null;
                if (medimatys != null && medimatys.Count > 0)
                    medicineTypeCodes.AddRange(medimatys.Select(o => o.MEDICINE_TYPE_CODE).Distinct().ToList());
                if (CurrentLExpMestMedicine1s != null && CurrentLExpMestMedicine1s.Count > 0)
                    medicineTypeCodes.AddRange(CurrentLExpMestMedicine1s.Select(o => o.MEDICINE_TYPE_CODE).Distinct().ToList());
                medicineTypeCodes = medicineTypeCodes.Count > 0 ? medicineTypeCodes.Distinct().ToList() : medicineTypeCodes;
                var medicineTypeCodeOthers = (medicineTypeCodes.Count > 0 && medicineTypeCodes.Exists(o => o != mediMatyTypeADO.MEDICINE_TYPE_CODE)) ? medicineTypeCodes.Where(o => o != mediMatyTypeADO.MEDICINE_TYPE_CODE).ToList() : null;
                if (medicineTypeCodeOthers != null && medicineTypeCodeOthers.Count > 0 && currentMedicineTypeAcins != null && currentMedicineTypeAcins.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Cac thuoc dang ke hoac thuoc da ke trong ngay____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeCodes), medicineTypeCodes)
                        + "________Thuoc dang chon de bo sung____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADO), mediMatyTypeADO));

                    var dicMedicineTypeAcin = currentMedicineTypeAcins.GroupBy(o => o.MEDICINE_TYPE_ID).ToDictionary(o => o.Key, t => t.ToList());
                    var mediMatyTypeAcinByCurrents = dicMedicineTypeAcin.ContainsKey(mediMatyTypeADO.ID) ? dicMedicineTypeAcin[mediMatyTypeADO.ID] : null;
                    var mediMatyTypeAcinIdByCurrents = (mediMatyTypeAcinByCurrents != null && mediMatyTypeAcinByCurrents.Count > 0) ? mediMatyTypeAcinByCurrents.Select(o => o.ACTIVE_INGREDIENT_ID).ToList() : null;
                    var mediMatyTypeAcinOthers = (mediMatyTypeAcinIdByCurrents != null && mediMatyTypeAcinIdByCurrents.Count > 0) ? currentMedicineTypeAcins.Where(o => medicineTypeCodeOthers.Contains(o.MEDICINE_TYPE_CODE)).ToList() : null;

                    if (mediMatyTypeAcinOthers != null && mediMatyTypeAcinOthers.Count > 0)
                    {
                        var dicMediMatyTypeAcinDeplicates = mediMatyTypeAcinOthers.Where(k => mediMatyTypeAcinIdByCurrents.Contains(k.ACTIVE_INGREDIENT_ID)).GroupBy(o => o.ACTIVE_INGREDIENT_ID).ToDictionary(o => o.Key, t => t.ToList());
                        if (dicMediMatyTypeAcinDeplicates != null && dicMediMatyTypeAcinDeplicates.Count > 0)
                        {
                            foreach (var gDup in dicMediMatyTypeAcinDeplicates)
                            {
                                medicineTypeNames += String.Format("{0} ({1})", gDup.Value.First().ACTIVE_INGREDIENT_NAME, mediMatyTypeADO.MEDICINE_TYPE_NAME + ";" + (gDup.Value.Aggregate((i, j) => new V_HIS_MEDICINE_TYPE_ACIN { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME)) + "\r\n";
                            }
                        }

                        if (!String.IsNullOrEmpty(medicineTypeNames))
                        {
                            DialogResult myResult;
                            string mesageWarn = String.Format(ResourceMessage.CacThuocTrungDuocTinh_BanCoMuonTiepTuc, "\r\n", medicineTypeNames, "\r\n");
                            myResult = XtraMessageBox.Show(mesageWarn, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                            if (myResult != System.Windows.Forms.DialogResult.Yes)
                                valid = false;

                            Inventec.Common.Logging.LogSystem.Warn("ValidSameAcin: Tim thay it nhat 2 thuoc cung hoat chat & nguoi dung chon " + (valid ? "\"co\"" : "\"khong\"") + "tiep tuc____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mesageWarn), mesageWarn));
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("ValidSameAcin: thuoc dang ke khong co thiet lap thuoc - hoat chat hoac cac thuoc da ke khong co thiet lap thuoc - hoat chat" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeCodes), medicineTypeCodes) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeAcinIdByCurrents), mediMatyTypeAcinIdByCurrents) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeAcinOthers), mediMatyTypeAcinOthers));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("ValidSameAcin: khong du 2 thuoc de so sanh hoac thuoc khong co thiet lap thuoc - hoat chat" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeCodes), medicineTypeCodes) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeCodeOthers), medicineTypeCodeOthers));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        static string GetMedicineTypeNameById(long id)
        {
            try
            {
                return BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.ID == id).FirstOrDefault().MEDICINE_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>
        /// Kiểm tra có tồn tại cấu hình mức cảnh báo căn cứ theo hoạt chất có trong các loại thuốc, 
        /// thực hiện cảnh báo cho các loại thuốc có trong, kiểu dữ liệu số nguyên.
        /// Nếu cấu hình này khác null thì nếu mức cảnh báo > cấu hình thì client sẽ thực hiện chặn không cho phép kê.      
        /// </summary>
        internal static bool ValidGrade(object data, List<MediMatyTypeADO> MediMatyTypeADOs, ref MemoEdit txtInteractionReason, frmAssignPrescription _frmAssignPrescription)
        {
            bool valid = true;
            try
            {
                strtxtLoginName = _frmAssignPrescription.txtLoginName.Text;
                requestDepartmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(_frmAssignPrescription.currentModule.RoomTypeId); ;
                treatmentId = _frmAssignPrescription.currentTreatmentWithPatientType.ID;
                ucIcdValue = _frmAssignPrescription.UcIcdGetValue() as IcdInputADO;
                icdCauseValue = _frmAssignPrescription.UcIcdCauseGetValue() as IcdInputADO; ;
                secondaryIcdValue = _frmAssignPrescription.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;


                //Kiểm tra nếu có cấu hình key 'AcinInteractive__Grade' thì mới tiếp tục, không có thì bỏ qua không xử lý gì
                if (HisConfigCFG.AcinInteractive__Grade.HasValue
                    && ((MediMatyTypeADOs != null && MediMatyTypeADOs.Count > 0) || ((HisConfigCFG.AcinInteractiveOption == "1" || HisConfigCFG.AcinInteractiveOption == "2" || HisConfigCFG.AcinInteractiveOption == "3") && _frmAssignPrescription.ListMedicineTypeAcin != null && _frmAssignPrescription.ListMedicineTypeAcin.Count > 0)))
                {
                    var acinInteractives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ACIN_INTERACTIVE>();
                    string messageErr = "", Message1 = "";
                    string MucDo = "", CoChe = "", HauQua = "", MoTa = "", XuLy = "";
                    List<string> lstCoche = new List<string>();
                    List<string> lstHauQua = new List<string>();
                    List<string> lstMoTa = new List<string>();
                    List<string> lstXuLy = new List<string>();

                    _InteractionReason = "";
                    Dictionary<string, MedicineAcinInteractiveWarnADO> dicMessageErrors = new Dictionary<string, MedicineAcinInteractiveWarnADO>();
                    if (data == null) throw new ArgumentNullException("data");
                    MediMatyTypeADO mediMatyTypeADOTemp = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(mediMatyTypeADOTemp, data);
                    if (mediMatyTypeADOTemp == null) throw new ArgumentNullException("mediMatyTypeADOTemp");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.BlockingInteractiveGrade), HisConfigCFG.BlockingInteractiveGrade));
                    long lBlockWhileAcinByMedicineType = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigCFG.BlockingInteractiveGrade);
                    bool bIsBlockWhileAcinByMedicineType = false;

                    if (mediMatyTypeADOTemp.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        //Lấy dữ liệu cấu hình loại thuốc - hoạt chất của thuốc đang chọn
                        //Có được 1 tập các medicineTypeAcin
                        var mediId__Adds = new List<long>();
                        mediId__Adds.Add(mediMatyTypeADOTemp.ID);
                        var medicineTypeAcin__Adds = GetMedicineTypeAcinByMedicineType(mediId__Adds);
                        if (medicineTypeAcin__Adds != null && medicineTypeAcin__Adds.Count > 0)
                        {
                            List<long> mediId_Check = new List<long>();
                            if (HisConfigCFG.AcinInteractiveOption == "3")
                            {
                                var HtuMediCheckAT = _frmAssignPrescription.DataHtuList != null && _frmAssignPrescription.DataHtuList.Count > 0 && _frmAssignPrescription.DataHtuList.Exists(o => o.IsChecked && o.CHECK_ACIN_INTERACTIVE == 1) ? _frmAssignPrescription.DataHtuList.Where(o => o.IsChecked && o.CHECK_ACIN_INTERACTIVE == 1).ToList() : null;
                                if (HtuMediCheckAT == null || HtuMediCheckAT.Count == 0)
                                {
                                    return valid;
                                }
                                else if (HtuMediCheckAT != null && HtuMediCheckAT.Count > 0)
                                {
                                    foreach (var item in MediMatyTypeADOs)
                                    {
                                        if (item.HTU_IDs != null && item.HTU_IDs.Count > 0 && HtuMediCheckAT.Exists(o => item.HTU_IDs.Exists(p => p == o.ID)))
                                        {
                                            mediId_Check.Add(item.ID);
                                        }
                                    }
                                    if(_frmAssignPrescription.ListMedicineTypeOld != null && _frmAssignPrescription.ListMedicineTypeOld.Count > 0)
                                    {
                                        foreach (var item in _frmAssignPrescription.ListMedicineTypeOld)
                                        {
                                            if (!string.IsNullOrEmpty(item.HTU_IDS) && HtuMediCheckAT.Exists(o => item.HTU_IDS.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(p => p == o.ID.ToString())))
                                            {
                                                mediId_Check.Add(item.MEDICINE_TYPE_ID);
                                            }
                                        }
                                    }
                                }
                                if (mediId_Check == null || mediId_Check.Count == 0)
                                    return valid;
                            }
                            //Lấy tất cả các thuốc đã chọn kê trong grid bên phải
                            //Mỗi thuốc đã kê lấy ra tất cả các cấu hình loại thuốc - hoạt chất
                            var mediId__InGrids = MediMatyTypeADOs.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).Select(o => o.ID).Distinct().ToList();

                            foreach (var item in medicineTypeAcin__Adds)
                            {
                                var medicineTypeAcin__InGrids = GetMedicineTypeAcinByMedicineType(mediId__InGrids);
                                //Khi bật key AcinInteractiveOption thì thực hiện lấy thêm tất cả các thuốc được kê tương ứng với ngày chỉ định của hsđt.
                                if ((HisConfigCFG.AcinInteractiveOption == "1" || HisConfigCFG.AcinInteractiveOption == "2" || HisConfigCFG.AcinInteractiveOption == "3") && _frmAssignPrescription.ListMedicineTypeAcin != null && _frmAssignPrescription.ListMedicineTypeAcin.Count > 0)
                                {
                                    if (medicineTypeAcin__InGrids != null && medicineTypeAcin__InGrids.Count > 0)
                                        medicineTypeAcin__InGrids.AddRange(_frmAssignPrescription.ListMedicineTypeAcin.Where(o => medicineTypeAcin__InGrids.Exists
                                        (p => p.MEDICINE_TYPE_ID != o.MEDICINE_TYPE_ID)));
                                    else
                                        medicineTypeAcin__InGrids = _frmAssignPrescription.ListMedicineTypeAcin;
                                }

                                if (mediId_Check.Count > 0)
                                {
                                    medicineTypeAcin__InGrids = medicineTypeAcin__InGrids.Where(o => mediId_Check.Exists(p => p == o.MEDICINE_TYPE_ID)).ToList();
                                }
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


                                            string messageErrKey = String.Format("{0}_{1}_{2}_{3}", item1.A.ACTIVE_INGREDIENT_ID, item1.A.MEDICINE_TYPE_ID, item.ACTIVE_INGREDIENT_ID, item.MEDICINE_TYPE_ID);
                                            if (dicMessageErrors.ContainsKey(messageErrKey))
                                            {
                                                var dataEdit = dicMessageErrors[messageErrKey];
                                                dataEdit.AcinInteractives.Add(item1.B);
                                            }
                                            else
                                            {
                                                MedicineAcinInteractiveWarnADO medicineAcinInteractiveWarnADO = new ADO.MedicineAcinInteractiveWarnADO();
                                                medicineAcinInteractiveWarnADO.Key = messageErrKey;
                                                medicineAcinInteractiveWarnADO.A = item1.A;
                                                medicineAcinInteractiveWarnADO.Item = item;
                                                medicineAcinInteractiveWarnADO.B = item1.B;
                                                medicineAcinInteractiveWarnADO.AcinInteractives = new List<V_HIS_ACIN_INTERACTIVE>();
                                                medicineAcinInteractiveWarnADO.AcinInteractives.Add(item1.B);

                                                dicMessageErrors.Add(messageErrKey, medicineAcinInteractiveWarnADO);
                                            }


                                            valid = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (dicMessageErrors != null && dicMessageErrors.Count > 0)
                    {
                        foreach (var item in dicMessageErrors)
                        {
                            string MucDoStr = "";
                            string messageErrTemp = String.Format(ResourceMessage.AcinInteractive__OverGrade, item.Value.A.ACTIVE_INGREDIENT_NAME, item.Value.A.MEDICINE_TYPE_NAME, item.Value.Item.ACTIVE_INGREDIENT_NAME, item.Value.Item.MEDICINE_TYPE_NAME);
                            //Hoạt chất {0} ({1}) tương tác với hoạt chất {2} ({3}).


                            Message1 = String.Format(ResourceMessage.ThuocTuongtacVoiThuocNhungVanKeDo, item.Value.A.MEDICINE_TYPE_NAME, item.Value.Item.MEDICINE_TYPE_NAME);

                            if (lBlockWhileAcinByMedicineType > 0 && item.Value.AcinInteractives.Max(k => k.INTERACTIVE_GRADE) >= lBlockWhileAcinByMedicineType)
                            {
                                bIsBlockWhileAcinByMedicineType = true;
                            }

                            if (!messageErr.Contains(messageErrTemp))
                            {
                                messageErr += messageErrTemp;
                            }

                            messageErr += "\r\n";

                            var DataGrade = item.Value.AcinInteractives.OrderByDescending(o => o.INTERACTIVE_GRADE).FirstOrDefault();
                            if (DataGrade != null && DataGrade.INTERACTIVE_GRADE != null)
                            {
                                MucDoStr += DataGrade.INTERACTIVE_GRADE;
                            }

                            if (DataGrade != null && !String.IsNullOrEmpty(DataGrade.INTERACTIVE_GRADE_NAME))
                            {
                                MucDoStr += " - " + DataGrade.INTERACTIVE_GRADE_NAME;
                            }

                            if (!MucDo.Contains(MucDoStr))
                            {
                                MucDo += MucDoStr;
                            }

                            MucDo += "\r\n";
                            lstCoche.AddRange(item.Value.AcinInteractives.Select(k => k.MECHANISM));
                            lstHauQua.AddRange(item.Value.AcinInteractives.Select(k => k.CONSEQUENCE));
                            lstMoTa.AddRange(item.Value.AcinInteractives.Select(k => k.DESCRIPTION));
                            lstXuLy.AddRange(item.Value.AcinInteractives.Select(k => k.INSTRUCTION));

                            MedicineTypeId_Interactive1 = item.Value.A.MEDICINE_TYPE_ID;
                            MedicineTypeId_Interactive2 = item.Value.Item.MEDICINE_TYPE_ID;
                            InteractiveGradeId = DataGrade.INTERACTIVE_GRADE_ID;
                        }

                        CoChe += String.Join(", ", lstCoche.Distinct().ToList());
                        HauQua += String.Join(", ", lstHauQua.Distinct().ToList());
                        MoTa += String.Join(", ", lstMoTa.Distinct().ToList());
                        XuLy += String.Join(", ", lstXuLy.Distinct().ToList());
                    }

                    Inventec.Common.Logging.LogSystem.Debug("ValidGrade. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMessageErrors), dicMessageErrors)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lBlockWhileAcinByMedicineType), lBlockWhileAcinByMedicineType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bIsBlockWhileAcinByMedicineType), bIsBlockWhileAcinByMedicineType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageErr), messageErr));

                    if (!valid && !String.IsNullOrEmpty(messageErr))
                    {

                        if (bIsBlockWhileAcinByMedicineType)
                        {
                            //MessageManager.Show(messageErr);
                            frmMessageBoxInteraction FrmMessage = new frmMessageBoxInteraction(messageErr, MucDo, CoChe, HauQua, MoTa, XuLy, null, null, CreatMedicineInteractive);
                            FrmMessage.ShowDialog();
                            Inventec.Common.Logging.LogSystem.Warn("ValidGrade 1");
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Warn("ValidGrade 2");
                            //DialogResult myResult;
                            //myResult = MessageBox.Show(messageErr + ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonBoSung, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            //if (myResult != DialogResult.Yes)
                            //{
                            //    Inventec.Common.Logging.LogSystem.Warn("ValidGrade 3");
                            //    valid = true;
                            //}

                            frmMessageBoxInteraction FrmMessage = new frmMessageBoxInteraction(messageErr, MucDo, CoChe, HauQua, MoTa, XuLy, ResourceMessage.BanCoMuonBoSungKhongNeuCoVuiLongNhapLyDo, ReloadTxtInteractionReason, CreatMedicineInteractive);
                            FrmMessage.ShowDialog();

                            if (!string.IsNullOrEmpty(_InteractionReason))
                            {
                                valid = true;
                                txtInteractionReason.Enabled = true;
                                txtInteractionReason.Text += Message1 + _InteractionReason + "\r\n";
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

        private static void CreatMedicineInteractive()
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_MEDICINE_INTERACTIVE data = new HIS_MEDICINE_INTERACTIVE();
                data.REQUEST_LOGINNAME = strtxtLoginName;
                data.REQUEST_DEPARTMENT_ID = requestDepartmentId;
                data.TREATMENT_ID = treatmentId;

                if (ucIcdValue != null)
                {
                    data.ICD_CODE = ucIcdValue.ICD_CODE;
                    data.ICD_NAME = ucIcdValue.ICD_NAME;
                }

                if (icdCauseValue != null)
                {
                    data.ICD_CAUSE_CODE = icdCauseValue.ICD_CODE;
                    data.ICD_CAUSE_NAME = icdCauseValue.ICD_NAME;
                }

                if (secondaryIcdValue != null)
                {
                    data.ICD_SUB_CODE = secondaryIcdValue.ICD_SUB_CODE;
                    data.ICD_TEXT = secondaryIcdValue.ICD_TEXT;
                }

                data.MEDICINE_TYPE_ID1 = MedicineTypeId_Interactive1 ?? 0;
                data.MEDICINE_TYPE_ID2 = MedicineTypeId_Interactive2 ?? 0;
                data.INTERACTIVE_GRADE_ID = InteractiveGradeId ?? 0;

                var resultData = new BackendAdapter(param).Post<HIS_MEDICINE_INTERACTIVE>("api/HisMedicineInteractive/Create", ApiConsumers.MosConsumer, data, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void ReloadTxtInteractionReason(string InteractionReason)
        {
            try
            {
                _InteractionReason = InteractionReason;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT> GetActiveIngredientByMedicineType(List<long> medicineTypeIds)
        {
            List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT> acinIngredients = new List<HIS_ACTIVE_INGREDIENT>();
            try
            {
                var medicineTypeAcin__Adds = GetMedicineTypeAcinByMedicineType(medicineTypeIds);
                if (medicineTypeAcin__Adds != null && medicineTypeAcin__Adds.Count > 0)
                {
                    acinIngredients = GetActiveIngredientByIds(medicineTypeAcin__Adds.Select(o => o.ACTIVE_INGREDIENT_ID).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return acinIngredients;
        }

        /// <summary>
        /// Khi kê đơn --> Nếu phát hiện thuốc có hoạt chất dấu * --> đưa cảnh báo Thuốc có hoạt chất dấu cần hội chẩn --> Bạn có muốn hội chẩn không? *
        ///Nếu đồng ý thì tự động mở form BB hội chẩn nhập dữ liệu --> tự động insert thông tin hành chính, thông tin thuốc dấu * (tên thuốc, liều dùng, cách dùng) --> Lưu và in ra report như hình Mẫu biên bản hội chẩn thuốc dấu sao.png
        ///Nếu không đồng ý thì chỉ lưu đơn bình thường
        /// </summary>
        /// <param name="MediMatyTypeADOs"></param>
        /// <returns></returns>
        internal static bool ValidConsultationReqiured(int actionType, HIS_SERVICE_REQ oldServiceReq, Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_ROOM requestRoom, long treatmentId, List<MediMatyTypeADO> MediMatyTypeADOs)
        {
            bool valid = true;
            try
            {
                if ((MediMatyTypeADOs == null || MediMatyTypeADOs.Count == 0)) return valid;
                actChooseDebateType = 0;
                if (HisConfigCFG.MedicineDebateOption == "1" || HisConfigCFG.MedicineDebateOption == "2")
                {
                    //Khi người dùng thực hiện lưu đơn, và key cấu hình "MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION" có giá trị 1 hoặc 2 thì thực hiện kiểm tra:
                    //Nếu có tồn tại thuốc có tick "thuốc dấu sao" (HIS_MEDICINE_TYPE có IS_STAR_MARK = 1) hoặc có hoạt chất tương ứng được tick "hoạt chất dấu sao" (HIS_ACTIVE_INGREDIENT có IS_CONSULTATION_REQUIRED = 1) thì thực hiện gọi api để kiểm tra, tương ứng với khoa chỉ định đã có biên bản hội chẩn tương ứng chưa:
                    //+ Nếu MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION = 1, gọi api kiểm tra BB hội chẩn theo loại thuốc (medicine_type_id)
                    //+ Nếu MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION = 2, gọi api kiểm tra BB hội chẩn theo hoạt chất (ACTIVE_INGREDIENT_ID)
                    //- Nếu đã tồn tại BB hội chẩn thì bỏ qua, thực hiện gọi api lưu đơn.
                    //- Nếu chưa tồn tại BB hội chẩn thì hiển thị popup:
                    //+ Có message thông báo: "Khoa chỉ định chưa tạo Biên bản hội chẩn đối với (các) thuốc: XXX, YYY." Trong đó XXX, YYY là tên các thuốc cần hội chẩn
                    //+ Có 2 nút: "Tạo biên bản hội chẩn", "Đóng"
                    //+ Khi nhấn nút "Tạo biên bản hội chẩn" thì mở ra module "Tạo biên bản hội chẩn", và điền mặc định các thông tin:
                    //"Loại hội chẩn" mặc định là "Hội chẩn thuốc"
                    //"Tên thuốc", "Đường dùng", "Hàm lượng", "HDSD", "Hoạt chất" mặc định điền theo thông tin thuốc khi kê đơn

                    List<string> messageErr = new List<string>();
                    List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicineForPrints = new List<HIS_DEBATE>();
                    List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT> acinIngredients = null;

                    CommonParam commonParam = new CommonParam();
                    MOS.Filter.HisDebateFilter debateFilter = new MOS.Filter.HisDebateFilter();
                    debateFilter.TREATMENT_ID = treatmentId;
                    debateFilter.ORDER_DIRECTION = "ASC";
                    debateFilter.ORDER_FIELD = "ID";
                    debateFilter.DEPARTMENT_ID = requestRoom.DEPARTMENT_ID;
                    debateFilter.CONTENT_TYPE = (short)IMSys.DbConfig.HIS_RS.HIS_DEBATE.CONTENT_TYPE__MEDICINE;
                    var debateLists = new Inventec.Common.Adapter.BackendAdapter(commonParam).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_GET, ApiConsumer.ApiConsumers.MosConsumer, debateFilter, commonParam);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => debateLists), debateLists));
                    HIS_DEBATE medicineForAdd = null;
                    var activeIngrErrIds = new List<long>();
                    string messageWithActiveIngrs = "";

                    foreach (var mediTemp in MediMatyTypeADOs)
                    {
                        if (mediTemp.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                            continue;

                        bool hasDebate = false;

                        if (HisConfigCFG.MedicineDebateOption == "1")
                        {
                            if (mediTemp.IS_STAR_MARK != GlobalVariables.CommonNumberTrue)
                                continue;

                            var debateUsers1 = (debateLists != null && debateLists.Count > 0) ? debateLists.Where(o =>
                                    IsConstainId(o.MEDICINE_TYPE_IDS, mediTemp.ID)
                                   ).ToList() : null;
                            hasDebate = (debateUsers1 != null && debateUsers1.Count > 0);

                            if (hasDebate)
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ho so dieu tri da co bien ban hoi chan tuong ung voi thuoc dau sao(theo loai thuoc hoac theo hoat chat cua thuoc) theo cau hinh MedicineDebateOption=" + HisConfigCFG.MedicineDebateOption + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasDebate), hasDebate));
                                continue;
                            }

                            Inventec.Common.Logging.LogSystem.Debug("Tim thay Thuoc " + mediTemp.MEDICINE_TYPE_NAME + " la thuoc dau sao can hoi chan");

                            if (medicineForAdd == null)
                            {
                                medicineForAdd = new HIS_DEBATE();
                                medicineForAdd.MEDICINE_TYPE_IDS = mediTemp.ID + ",";
                                medicineForAdd.MEDICINE_TYPE_NAME = mediTemp.MEDICINE_TYPE_NAME;
                                medicineForAdd.MEDICINE_USE_FORM_NAME = mediTemp.MEDICINE_USE_FORM_NAME;
                                medicineForAdd.MEDICINE_TUTORIAL = mediTemp.TUTORIAL;
                                medicineForAdd.MEDICINE_USE_TIME = mediTemp.UseTimeTo;
                                medicineForAdd.MEDICINE_CONCENTRA = mediTemp.CONCENTRA;
                            }
                            else if (!medicineForAdd.MEDICINE_TYPE_IDS.Contains(mediTemp.ID + ","))
                            {
                                medicineForAdd.MEDICINE_TYPE_IDS += mediTemp.ID + ",";
                                medicineForAdd.MEDICINE_TYPE_NAME = "";
                                medicineForAdd.MEDICINE_USE_FORM_NAME = "";
                                medicineForAdd.MEDICINE_TUTORIAL = "";
                                medicineForAdd.MEDICINE_USE_TIME = null;
                                medicineForAdd.MEDICINE_CONCENTRA = "";
                            }

                            messageErr.Add(mediTemp.MEDICINE_TYPE_NAME);
                        }
                        else if (HisConfigCFG.MedicineDebateOption == "2")
                        {
                            var mediId__Adds = new List<long>();
                            mediId__Adds.Add(mediTemp.ID);
                            acinIngredients = GetActiveIngredientByMedicineType(mediId__Adds);
                            if (acinIngredients != null && acinIngredients.Count > 0)
                            {
                                var activeIngrErrIdTemps = new List<long>();

                                foreach (var acinIngr in acinIngredients)
                                {
                                    bool bdebateForAcinGr = (debateLists != null && debateLists.Count > 0) ? debateLists.Exists(o =>
                                        IsConstainId(o.ACTIVE_INGREDIENT_IDS, acinIngr.ID)
                                       ) : false;
                                    if (!bdebateForAcinGr)
                                    {
                                        activeIngrErrIdTemps.Add(acinIngr.ID);
                                        if (activeIngrErrIds.Count == 0 || !activeIngrErrIds.Contains(acinIngr.ID))
                                        {
                                            messageWithActiveIngrs += acinIngr.ACTIVE_INGREDIENT_NAME + ",";
                                            activeIngrErrIds.Add(acinIngr.ID);
                                        }
                                    }
                                }

                                if ((activeIngrErrIdTemps.Count > 0 && activeIngrErrIdTemps.Count <= acinIngredients.Count))
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Truong hop cau hinh (MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION == 2), va tim thay it nhat 1 trong cac hoat chat cua thuoc khong co trong ACTIVE_INGREDIENT_IDS cua ban ghi HIS_DEBATE==> thong bao va hien thi form tao moi bien ban hoi chan____ "
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => activeIngrErrIdTemps), activeIngrErrIdTemps)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => acinIngredients), acinIngredients.Select(o => o.ID)));
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("ho so dieu tri da co bien ban hoi chan tuong ung voi thuoc dau sao(theo loai thuoc hoac theo hoat chat cua thuoc) theo cau hinh MedicineDebateOption=" + HisConfigCFG.MedicineDebateOption + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasDebate), hasDebate));
                                    continue;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Truong hop cau hinh (MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION == 2) va khong tim thay hoat chat nao thoa man (HIS_ACTIVE_INGREDIENT co IS_CONSULTATION_REQUIRED =1)");
                                continue;
                            }

                            Inventec.Common.Logging.LogSystem.Debug("Tim thay Thuoc " + mediTemp.MEDICINE_TYPE_NAME + " la thuoc co hoat chat can hoi chan");

                            if (medicineForAdd == null)
                            {
                                medicineForAdd = new HIS_DEBATE();
                                medicineForAdd.MEDICINE_TYPE_IDS = mediTemp.ID + ",";
                                medicineForAdd.MEDICINE_TYPE_NAME = mediTemp.MEDICINE_TYPE_NAME;
                                medicineForAdd.MEDICINE_USE_FORM_NAME = mediTemp.MEDICINE_USE_FORM_NAME;
                                medicineForAdd.MEDICINE_TUTORIAL = mediTemp.TUTORIAL;
                                medicineForAdd.MEDICINE_USE_TIME = mediTemp.UseTimeTo;
                                medicineForAdd.MEDICINE_CONCENTRA = mediTemp.CONCENTRA;
                            }
                            else if (!medicineForAdd.MEDICINE_TYPE_IDS.Contains(mediTemp.ID + ","))
                            {
                                medicineForAdd.MEDICINE_TYPE_IDS += mediTemp.ID + ",";
                                medicineForAdd.MEDICINE_TYPE_NAME = "";
                                medicineForAdd.MEDICINE_USE_FORM_NAME = "";
                                medicineForAdd.MEDICINE_TUTORIAL = "";
                                medicineForAdd.MEDICINE_USE_TIME = null;
                                medicineForAdd.MEDICINE_CONCENTRA = "";
                            }

                            messageErr.Add(mediTemp.MEDICINE_TYPE_NAME);
                        }
                    }

                    if (medicineForAdd != null)
                    {
                        if (activeIngrErrIds != null && activeIngrErrIds.Count > 0)
                        {
                            medicineForAdd.ACTIVE_INGREDIENT_IDS = String.Join(",", activeIngrErrIds.Select(o => o));
                        }
                        medicineForPrints.Add(medicineForAdd);

                        string messages =
                            !String.IsNullOrEmpty(messageWithActiveIngrs) ?
                            String.Format(ResourceMessage.ThuocCoDauCanHoiChan_ChuaTaoHetVoiHoatChat_BanCoMuonHoiChan, String.Join(", ", messageErr.ToArray()), messageWithActiveIngrs)
                            :
                            String.Format(ResourceMessage.ThuocCoDauCanHoiChan_BanCoMuonHoiChan, String.Join(", ", messageErr.ToArray()));

                        frmMedicineDebateConfirm frmMedicineDebateConfirm = new frmMedicineDebateConfirm(ActChooseDebateType, messages);
                        frmMedicineDebateConfirm.ShowDialog();
                        if (actChooseDebateType > 0)
                        {
                            switch (actChooseDebateType)
                            {
                                case actChooseDebateType__Create:
                                    List<object> listArgs = new List<object>();
                                    CommonParam param = new CommonParam();
                                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                                    filter.TREATMENT_ID = treatmentId;
                                    filter.ORDER_DIRECTION = "DESC";
                                    filter.ORDER_FIELD = "MODIFY_TIME";
                                    HIS_SERVICE_REQ rs = new HIS_SERVICE_REQ();
                                    rs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                                    listArgs.Add(rs);
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
                                    break;
                                case actChooseDebateType__None:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("cấu hình hệ thống: MOS.HIS_SERVICE_REQ.MEDICINE_DEBATE_OPTION: giá trị: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.MedicineDebateOption), HisConfigCFG.MedicineDebateOption) + "==> khong xu ly cai gi");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        static bool IsConstainId(string medicineTypeIds, long metyId)
        {
            var arrMetyIds = !String.IsNullOrEmpty(medicineTypeIds) ? medicineTypeIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) : null;
            return arrMetyIds != null && arrMetyIds.Contains(metyId + "");
        }

        static void ActChooseDebateType(int actType)
        {
            actChooseDebateType = actType;
        }

        internal static List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcinByMedicineType(List<long> medicineTypeIds)
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

        private static List<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT> GetActiveIngredientByIds(List<long> AcinIds)
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
