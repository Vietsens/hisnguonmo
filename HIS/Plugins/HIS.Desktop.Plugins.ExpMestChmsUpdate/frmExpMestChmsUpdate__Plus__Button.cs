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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestChmsUpdate.ADO;
using HIS.Desktop.Print;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestChmsUpdate
{
    public partial class frmExpMestChmsUpdate : HIS.Desktop.Utility.FormBase
    {
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods { get; set; }
        V_HIS_EXP_MEST chmsExpMest = new V_HIS_EXP_MEST();
        string Req_Department_Name = "";
        string Req_Room_Name = "";
        string Exp_Department_Name = "";
        long roomIdByMediStockIdPrint = 0;
        long keyPhieuTra = 0;

        Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnAddd.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null)
                    return;


                if ((decimal?)spinExpAmount.EditValue > this.currentMediMate.AVAILABLE_AMOUNT)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho + " Yêu cầu tiếp tục",
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                }

                this.currentMediMate.EXP_AMOUNT = spinExpAmount.Value;
                this.currentMediMate.NOTE = txtNote.Text;
                this.currentMediMate.IsPackage = chkHienThiLo.Checked;
                if (this.currentMediMate.IsMedicine)
                {
                    this.currentMediMate.ExpMedicine.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMedicine.Description = txtNote.Text;
                }
                else if (this.currentMediMate.IsBlood)
                {
                    this.currentMediMate.ExpBlood.Amount = Inventec.Common.TypeConvert.Parse.ToInt64(spinExpAmount.Value.ToString());
                    this.currentMediMate.ExpBlood.Description = txtNote.Text;
                    this.currentMediMate.ExpBlood.BloodAboId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                    this.currentMediMate.ExpBlood.BloodRhId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());
                    this.currentMediMate.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                    this.currentMediMate.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());
                }
                else
                {
                    this.currentMediMate.ExpMaterial.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMaterial.Description = txtNote.Text;
                }

                if (dicMediMateAdo.ContainsKey(this.currentMediMate.SERVICE_ID))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.ThuocVatTuDaCoTrongDanhSachXuat_BanCoMuonThayThe, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                WaitingManager.Show();
                dicMediMateAdo[this.currentMediMate.SERVICE_ID] = this.currentMediMate;
                gridControlExpMestChmsDetail.BeginUpdate();
                gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpMestChmsDetail.EndUpdate();
                ResetValueControlDetail();
                if (this.currentMediMate.IsMedicine)
                {
                    txtSearchMedicine.Focus();
                }
                else if (this.currentMediMate.IsBlood)
                {
                    txtSearch.Focus();
                }
                else
                {
                    txtSearchMaterial.Focus();
                }
                this.currentMediMate = null;

                WaitingManager.Hide();
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
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || dicMediMateAdo.Count == 0)
                    return;
                if (cboExpMediStock.EditValue == null || cboImpMediStock.EditValue == null)
                {
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref param, ref success);
                if (success)
                {
                    ProcessFillDataBySuccess();
                    FillDataToGridExpMest();
                    ddBtnPrint.Enabled = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSave(ref CommonParam param, ref bool success)
        {
            try
            {

                HisExpMestChmsSDO data = new HisExpMestChmsSDO();
                data.Description = txtDescription.Text;
                data.ExpMestId = this.hisExpMest.ID;
                data.Type = ChmsTypeEnum.GIVE_BACK;
                data.ImpMediStockId = this.hisExpMest.IMP_MEDI_STOCK_ID ?? 0;
                data.MediStockId = this.hisExpMest.MEDI_STOCK_ID;
                data.ReqRoomId = this.currentModule.RoomId;

                data.Medicines = new List<ExpMedicineTypeSDO>();
                data.Materials = new List<ExpMaterialTypeSDO>();
                data.Bloods = new List<ExpBloodTypeSDO>();
                data.ExpMaterialSdos = new List<ExpMaterialSDO>();
                data.ExpMedicineSdos = new List<ExpMedicineSDO>();
                string str = "";
                foreach (var item in dicMediMateAdo)
                {
                    if (item.Value.EXP_AMOUNT <= 0)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong);
                        return;
                    }
                    if (item.Value.AVAILABLE_AMOUNT < item.Value.EXP_AMOUNT)
                    {
                        str += item.Value.MEDI_MATE_TYPE_NAME + "; ";
                    }
                    if (item.Value.IsMedicine)
                    {
                        if (item.Value.IsPackage)
                        {
                            ExpMedicineSDO ado = new ExpMedicineSDO();
                            ado.Amount = item.Value.EXP_AMOUNT;
                            ado.Description = item.Value.NOTE;
                            ado.MedicineId = item.Value.MEDICINE_ID;
                            ado.NumOrder = item.Value.NUM_ORDER;
                            data.ExpMedicineSdos.Add(ado);
                        }
                        else
                        {
                            item.Value.ExpMedicine.ExpMestMetyReqId = item.Value.MEDI_MATE_REQ_ID;
                            item.Value.ExpMedicine.Amount = item.Value.EXP_AMOUNT;
                            item.Value.ExpMedicine.Description = item.Value.NOTE;
                            item.Value.ExpMedicine.NumOrder = item.Value.NUM_ORDER;
                            data.Medicines.Add(item.Value.ExpMedicine);
                        }
                    }
                    else if (item.Value.IsBlood)
                    {
                        item.Value.ExpBlood.BloodAboId = item.Value.BLOOD_ABO_ID ?? 0;
                        item.Value.ExpBlood.BloodRhId = item.Value.BLOOD_RH_ID ?? 0;
                        item.Value.ExpBlood.ExpMestBltyReqId = item.Value.MEDI_MATE_REQ_ID;
                        item.Value.ExpBlood.Amount = Inventec.Common.TypeConvert.Parse.ToInt64(item.Value.EXP_AMOUNT.ToString());
                        item.Value.ExpBlood.Description = item.Value.NOTE;
                        item.Value.ExpBlood.NumOrder = item.Value.NUM_ORDER;
                        data.Bloods.Add(item.Value.ExpBlood);
                    }
                    else
                    {
                        if (item.Value.IsPackage)
                        {
                            ExpMaterialSDO ado = new ExpMaterialSDO();
                            ado.Amount = item.Value.EXP_AMOUNT;
                            ado.Description = item.Value.NOTE;
                            ado.MaterialId = item.Value.MATERIAL_ID;
                            ado.NumOrder = item.Value.NUM_ORDER;
                            data.ExpMaterialSdos.Add(ado);
                        }
                        else
                        {
                            item.Value.ExpMaterial.ExpMestMatyReqId = item.Value.MEDI_MATE_REQ_ID;
                            item.Value.ExpMaterial.Amount = item.Value.EXP_AMOUNT;
                            item.Value.ExpMaterial.Description = item.Value.NOTE;
                            item.Value.ExpMaterial.NumOrder = item.Value.NUM_ORDER;
                            data.Materials.Add(item.Value.ExpMaterial);
                        }
                    }
                }
                if (!String.IsNullOrEmpty(str) && DevExpress.XtraEditors.XtraMessageBox.Show("(" + str + ") " +
                    Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho + " Yêu cầu tiếp tục",
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    WaitingManager.Hide();
                    return;
                }

                Inventec.Common.Logging.LogSystem.Info("Du lieu gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/ChmsUpdate", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    isUpdate = true;
                    this.resultSdo = rs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!btnNew.Enabled)
                //    return;
                //WaitingManager.Show();
                //ResetValueControlCommon();
                //SetEnableCboMediStockAndButton(true);
                //FillDataToGridExpMest();
                //radioImport.Checked = true;
                ////LoadDataToTreeList(null);
                ////V_HIS_MEDI_STOCK mestRoom = null;
                ////if (cboExpMediStock.EditValue != null)
                ////{
                ////    mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                ////}
                ////LoadDataToTreeList(mestRoom);
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPrintPhieuXuatChuyenKho(object sender, EventArgs e)
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086, delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPrintPhieuGayNghienHuongThan(object sender, EventArgs e)
        {
            if (this.resultSdo == null || this.resultSdo.ExpMedicines == null)
                return;
            Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
            store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089, delegatePrintTemplate);
        }

        private void onClickPrintPhieuKhongPhaiGayNghienHuongThan(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090, delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessFillDataBySuccess()
        {
            try
            {
                if (this.resultSdo != null)
                {
                    if (this.resultSdo.ExpMetyReqs != null && this.resultSdo.ExpMetyReqs.Count > 0)
                    {
                        this.resultSdo.ExpMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                        foreach (var item in this.resultSdo.ExpMetyReqs)
                        {
                            HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                            ado.AMOUNT = item.AMOUNT;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.TDL_MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID;
                            this.resultSdo.ExpMedicines.Add(ado);
                        }
                    }
                    if (this.resultSdo.ExpMatyReqs != null && this.resultSdo.ExpMatyReqs.Count > 0)
                    {
                        this.resultSdo.ExpMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                        foreach (var item in this.resultSdo.ExpMatyReqs)
                        {
                            HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                            ado.AMOUNT = item.AMOUNT;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.TDL_MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                            this.resultSdo.ExpMaterials.Add(ado);
                        }
                    }

                    //}
                    if (this.resultSdo.ExpMedicines != null && this.resultSdo.ExpMedicines.Count > 0)
                    {
                        var Group = this.resultSdo.ExpMedicines.GroupBy(o => o.TDL_MEDICINE_TYPE_ID).ToList();
                        var _medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var group in Group)
                        {
                            var dataMediType = _medicineType.FirstOrDefault(p => p.ID == group.Key);
                            var listByGroup = group.ToList<HIS_EXP_MEST_MEDICINE>();
                            if (dataMediType != null)
                            {
                                if (dicMediMateAdo.ContainsKey(dataMediType.SERVICE_ID))
                                {
                                    var medicine = dicMediMateAdo[dataMediType.SERVICE_ID];
                                    medicine.ExpMedicine.Amount = listByGroup.Sum(s => s.AMOUNT);
                                    medicine.ExpMedicine.Description = listByGroup.First().DESCRIPTION;
                                    medicine.ExpMedicine.MedicineTypeId = listByGroup.First().TDL_MEDICINE_TYPE_ID ?? 0;
                                }
                            }
                        }
                    }

                    if (this.resultSdo.ExpMaterials != null && this.resultSdo.ExpMaterials.Count > 0)
                    {

                        var Group = this.resultSdo.ExpMaterials.GroupBy(o => o.TDL_MATERIAL_TYPE_ID).ToList();
                        var _materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        foreach (var group in Group)
                        {
                            var dataMate = _materialType.FirstOrDefault(p => p.ID == group.Key);
                            if (dataMate != null)
                            {
                                var listByGroup = group.ToList<HIS_EXP_MEST_MATERIAL>();
                                if (dicMediMateAdo.ContainsKey(dataMate.SERVICE_ID))
                                {
                                    var material = dicMediMateAdo[dataMate.SERVICE_ID];
                                    material.ExpMaterial.Amount = listByGroup.Sum(s => s.AMOUNT);
                                    material.ExpMaterial.Description = listByGroup.First().DESCRIPTION;
                                    material.ExpMaterial.MaterialTypeId = listByGroup.First().TDL_MATERIAL_TYPE_ID ?? 0;
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

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086:
                            InPhieuXuatChuyenKho(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089:
                            InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090:
                            InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<HIS_MEDICINE> _Medicines = null;
        List<HIS_MATERIAL> _Materials = null;
        List<HIS_BLOOD> _Bloods = null;

        private void InPhieuXuatChuyenKho(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                long keyOrder = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__ODER_OPTION));

                #region TT Chung
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                this._Bloods = new List<HIS_BLOOD>();
                this._Medicines = new List<HIS_MEDICINE>();
                this._Materials = new List<HIS_MATERIAL>();

                this.resultSdo = new HisExpMestResultSDO();
                MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                metyReqFilter.EXP_MEST_ID = this.hisExpMest.ID;
                this.resultSdo.ExpMetyReqs = new List<HIS_EXP_MEST_METY_REQ>();
                this.resultSdo.ExpMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                matyReqFilter.EXP_MEST_ID = this.hisExpMest.ID;
                this.resultSdo.ExpMatyReqs = new List<HIS_EXP_MEST_MATY_REQ>();
                this.resultSdo.ExpMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                MOS.Filter.HisExpMestBltyReqFilter bltyReqFilter = new HisExpMestBltyReqFilter();
                bltyReqFilter.EXP_MEST_ID = this.hisExpMest.ID;
                this.resultSdo.ExpBltyReqs = new List<HIS_EXP_MEST_BLTY_REQ>();
                this.resultSdo.ExpBltyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/Get", ApiConsumers.MosConsumer, bltyReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);


                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = this.hisExpMest.ID;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                this.chmsExpMest = new V_HIS_EXP_MEST();
                this.chmsExpMest = listChmsExpMest.First();

                Req_Department_Name = "";
                Req_Room_Name = "";
                Exp_Department_Name = "";
                var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == this.chmsExpMest.REQ_DEPARTMENT_ID).ToList();
                if (Req_Department != null && Req_Department.Count > 0)
                {
                    Req_Department_Name = Req_Department.FirstOrDefault().DEPARTMENT_NAME;
                }

                var Req_Room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.chmsExpMest.REQ_ROOM_ID).ToList();
                if (Req_Room != null && Req_Room.Count > 0)
                {
                    Req_Room_Name = Req_Room.FirstOrDefault().ROOM_NAME;
                }
                var Exp_Department = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == this.chmsExpMest.MEDI_STOCK_ID).ToList();
                if (Exp_Department != null && Exp_Department.Count > 0)
                {
                    Exp_Department_Name = Exp_Department.FirstOrDefault().DEPARTMENT_NAME;
                }

                roomIdByMediStockIdPrint = 0;
                roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.chmsExpMest.MEDI_STOCK_ID).ROOM_ID;

                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ExpMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();

                _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                if (this.resultSdo != null)
                {
                    if (this.chmsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                        || this.chmsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                        || this.chmsExpMest.IS_REQUEST_BY_PACKAGE == 1)
                    {
                        MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        mediFilter.EXP_MEST_ID = this.chmsExpMest.ID;
                        _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                        if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                        {
                            List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                            MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                            medicineFilter.IDs = _MedicineIds;
                            this._Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                        }

                        MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                        matyFilter.EXP_MEST_ID = this.chmsExpMest.ID;
                        _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);

                        if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                        {
                            List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                            MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                            materialFilter.IDs = _MaterialIds;
                            this._Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                        }

                        MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                        bloodFilter.EXP_MEST_ID = this.chmsExpMest.ID;
                        _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                        if (_ExpMestBloods != null && _ExpMestBloods.Count > 0)
                        {
                            List<long> _BloodIds = _ExpMestBloods.Select(p => p.BLOOD_ID).ToList();
                            MOS.Filter.HisBloodFilter bloodNewFilter = new HisBloodFilter();
                            bloodNewFilter.IDs = _BloodIds;
                            this._Bloods = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumers.MosConsumer, bloodNewFilter, param);
                        }
                    }
                }

                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_MPS_AGGR_EXP_MEST_MEDICINE_MERGER_DATA));
                WaitingManager.Hide();
                #endregion

                //#region In Tong Hop
                //if (keyPrintType == 1)
                //{
                //    string keyName = "";
                //    if (roomIdByMediStockIdPrint > 0)
                //    {
                //        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                //        {
                //            keyPhieuTra = 1;
                //            keyName = "PHIẾU TRẢ TỔNG HỢP";
                //        }
                //        else
                //        {
                //            keyName = "PHIẾU LĨNH TỔNG HỢP";
                //            keyPhieuTra = 0;
                //        }
                //    }
                //    MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                //(
                // chmsExpMest,
                // _ExpMestMedicines,
                // _ExpMestMaterials,
                // _ExpMestBloods,
                // this.resultSdo.ExpMetyReqs,
                // this.resultSdo.ExpMatyReqs,
                // this.resultSdo.ExpBltyReqs,
                // Req_Department_Name,
                // Req_Room_Name,
                // Exp_Department_Name,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                // BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                // BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                // BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                // BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                // BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                // BackendDataWorker.Get<HIS_BLOOD_RH>(),
                // keyName,
                // configKeyMert,
                // keyPhieuTra,
                // this._Medicines,
                // this._Materials,
                // this._Bloods
                //  );
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //    }
                //    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                //    PrintData.EmrInputADO = inputADO;
                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                //#endregion
                //else
                {
                    #region --- Xu Ly Tach GN_HT -----
                    if (this.resultSdo != null)
                    {
                        if (this.resultSdo.ExpMetyReqs != null && this.resultSdo.ExpMetyReqs.Count > 0)
                        {
                            var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                            var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                            bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                            bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                            bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                            bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                            bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                            bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                            bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                            bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);

                            var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                            foreach (var item in this.resultSdo.ExpMetyReqs)
                            {
                                var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                                if (dataMedi != null)
                                {
                                    if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                    {
                                        _ExpMestMetyReq_GN_HTs.Add(item);
                                        _ExpMestMetyReq_GNs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                    {
                                        _ExpMestMetyReq_GN_HTs.Add(item);
                                        _ExpMestMetyReq_HTs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                    {
                                        _ExpMestMetyReq_TDs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                    {
                                        _ExpMestMetyReq_PXs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                    {
                                        _ExpMestMetyReq_COs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                    {
                                        _ExpMestMetyReq_DTs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                    {
                                        _ExpMestMetyReq_KSs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                    {
                                        _ExpMestMetyReq_LAOs.Add(item);
                                    }
                                    else
                                    {
                                        _ExpMestMetyReq_Ts.Add(item);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region ----- In GN_HT ------


                    richEditorMain.RunPrintTemplate("Mps000198", DelegateRunMps);

                    richEditorMain.RunPrintTemplate("Mps000048", DelegateRunMps);
                    #endregion

                    #region -----In Thuoc Thuong -----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        string keyNameAggr = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyNameAggr = "PHIẾU TRẢ THUỐC THƯỜNG";
                        }
                        else
                        {
                            keyNameAggr = "PHIẾU LĨNH THUỐC THƯỜNG";
                            keyPhieuTra = 0;
                        }
                        MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                     (
                      chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 _ExpMestMetyReq_Ts,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNameAggr,
                 configKeyMert,
                 keyPhieuTra,
                 this._Medicines,
                 this._Materials,
                 this._Bloods,
                 keyOrder,
                 BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>()
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region -----In Vat Tu -----

                    if (this.resultSdo.ExpMatyReqs != null && this.resultSdo.ExpMatyReqs.Count > 0)
                    {
                        var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                        foreach (var item in this.resultSdo.ExpMatyReqs)
                        {
                            var dataMaty = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE == 1)
                            {
                                _ExpMestMatyReq_HCs.Add(item);
                            }
                            else
                                _ExpMestMatyReq_VTs.Add(item);
                        }
                        if (_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0)
                        {
                            string keyNameAggrHc = "";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggrHc = "PHIẾU TRẢ HÓA CHẤT";
                                }
                                else
                                {
                                    keyPhieuTra = 0;
                                    keyNameAggrHc = "PHIẾU LĨNH HÓA CHẤT";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                       (
                         chmsExpMest,
                 null,
                 _ExpMestMaterials,
                 null,
                 null,
                 _ExpMestMatyReq_HCs,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 keyNameAggrHc,
                 configKeyMert,
                 keyPhieuTra,
                 this._Medicines,
                 this._Materials,
                 this._Bloods
                         );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                        {
                            string keyNameAggr = "";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggr = "PHIẾU TRẢ VẬT TƯ";
                                }
                                else
                                {
                                    keyPhieuTra = 0;
                                    keyNameAggr = "PHIẾU LĨNH VẬT TƯ";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                       (
                         chmsExpMest,
                 null,
                 _ExpMestMaterials,
                 null,
                 null,
                 _ExpMestMatyReq_VTs,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 keyNameAggr,
                 configKeyMert,
                 keyPhieuTra,
                 this._Medicines,
                 this._Materials,
                 this._Bloods
                         );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunMps(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000048":
                        Mps000048(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000198":
                        Mps000198(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000048(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA")); long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- gop GN HT ----
                    if (this._ExpMestMetyReq_GN_HTs != null && this._ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        string keyAddictive = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN, HƯỚNG THẦN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN, HƯỚNG THẦN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      chmsExpMest,
                     _ExpMestMedicines,
                     null,
                     null,
                     this._ExpMestMetyReq_GN_HTs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyAddictive,
                     configKeyMert,
                     keyPhieuTra
                       );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (this._ExpMestMetyReq_GNs != null && this._ExpMestMetyReq_GNs.Count > 0)
                    {
                        string keyAddictive = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      chmsExpMest,
                     _ExpMestMedicines,
                     null,
                     null,
                     this._ExpMestMetyReq_GNs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyAddictive,
                     configKeyMert,
                     keyPhieuTra
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- HT ----
                    if (this._ExpMestMetyReq_HTs != null && this._ExpMestMetyReq_HTs.Count > 0)
                    {
                        string keyNeurological = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyNeurological = "PHIẾU TRẢ THUỐC HƯỚNG THẦN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyNeurological = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      chmsExpMest,
                     _ExpMestMedicines,
                     null,
                     null,
                     this._ExpMestMetyReq_HTs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyNeurological,
                     configKeyMert,
                     keyPhieuTra
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }

                #region ---- DOC ----
                if (this._ExpMestMetyReq_TDs != null && this._ExpMestMetyReq_TDs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC ĐỘC";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC ĐỘC";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_TDs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- PX ----
                if (this._ExpMestMetyReq_PXs != null && this._ExpMestMetyReq_PXs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC PHÓNG XẠ";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC PHÓNG XẠ";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_PXs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO ----
                if (this._ExpMestMetyReq_COs != null && this._ExpMestMetyReq_COs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC CORTICOID";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC CORTICOID";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_COs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT ----
                if (this._ExpMestMetyReq_DTs != null && this._ExpMestMetyReq_DTs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC DỊCH TRUYỀN";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC DỊCH TRUYỀN";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_DTs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS ----
                if (this._ExpMestMetyReq_KSs != null && this._ExpMestMetyReq_KSs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC KHÁNG SINH";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC KHÁNG SINH";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_KSs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- LAO ----
                if (this._ExpMestMetyReq_LAOs != null && this._ExpMestMetyReq_LAOs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC LAO";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC LAO";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_LAOs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000198(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                if (this.resultSdo.ExpBltyReqs != null && this.resultSdo.ExpBltyReqs.Count > 0)
                {
                    string keyAddictive = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyAddictive = "PHIẾU TRẢ MÁU";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyAddictive = "PHIẾU LĨNH MÁU";
                    }
                    MPS.Processor.Mps000198.PDO.Mps000198PDO mps000198PDO = new MPS.Processor.Mps000198.PDO.Mps000198PDO
                 (
                  chmsExpMest,
                 this.resultSdo.ExpBltyReqs,
                 this._ExpMestBloods,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                 BackendDataWorker.Get<HIS_BLOOD_RH>(),
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 keyAddictive,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.hisExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                //HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
                //chmsFilter.ID = this.resultSdo.ChmsExpMest.ID;
                //var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                //if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                //    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                //var chmsExpMest = listChmsExpMest.First();
                ////MPS.Core.Mps000089.Mps000089RDO rdo = new MPS.Core.Mps000089.Mps000089RDO(chmsExpMest, this.resultSdo.ExpMedicines);
                ////WaitingManager.Hide();
                ////result = MPS.Printer.Run(printTypeCode, fileName, rdo);

                //MPS.Processor.Mps000089.PDO.Mps000089PDO mps000089RDO = new MPS.Processor.Mps000089.PDO.Mps000089PDO(
                //    chmsExpMest,
                //    this.resultSdo.ExpMedicines,
                //    HisExpMestSttCFG.HisExpMestSttId__Draft,
                //    HisExpMestSttCFG.HisExpMestSttId__Request,
                //    HisExpMestSttCFG.HisExpMestSttId__Rejected,
                //    HisExpMestSttCFG.HisExpMestSttId__Approved,
                //    HisExpMestSttCFG.HisExpMestSttId__Exported
                //                );
                //MPS.ProcessorBase.Core.PrintData PrintData = null;
                //if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000089RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000089RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //}
                //result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                //HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
                //chmsFilter.ID = this.resultSdo.ChmsExpMest.ID;
                //var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                //if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                //    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                //var chmsExpMest = listChmsExpMest.First();
                ////MPS.Core.Mps000090.Mps000090RDO rdo = new MPS.Core.Mps000090.Mps000090RDO(chmsExpMest, this.resultSdo.ExpMedicines, this.resultSdo.ExpMaterials);
                ////WaitingManager.Hide();
                ////result = MPS.Printer.Run(printTypeCode, fileName, rdo);

                //MPS.Processor.Mps000090.PDO.Mps000090PDO mps000090RDO = new MPS.Processor.Mps000090.PDO.Mps000090PDO(
                //    chmsExpMest,
                //    this.resultSdo.ExpMedicines,
                //    this.resultSdo.ExpMaterials,
                //    HisExpMestSttCFG.HisExpMestSttId__Draft,
                //    HisExpMestSttCFG.HisExpMestSttId__Request,
                //    HisExpMestSttCFG.HisExpMestSttId__Rejected,
                //    HisExpMestSttCFG.HisExpMestSttId__Approved,
                //    HisExpMestSttCFG.HisExpMestSttId__Exported
                //                );
                //MPS.ProcessorBase.Core.PrintData PrintData = null;
                //if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000090RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000090RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //}
                //result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
