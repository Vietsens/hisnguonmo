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
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestDepaCreate.ADO;
using HIS.Desktop.Plugins.ExpMestDepaCreate.Print;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
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

namespace HIS.Desktop.Plugins.ExpMestDepaCreate
{
    public partial class UCExpMestDepaCreate : UserControlBase
    {
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnAdd.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null)
                    return;


                if ((decimal?)spinExpAmount.EditValue > this.currentMediMate.AVAILABLE_AMOUNT)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Base.ResourceMessageLang.SoLuongXuatLonHonSoLuonKhaDungTrongKho + " Yêu cầu tiếp tục",
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }

                this.currentMediMate.EXP_AMOUNT = spinExpAmount.Value;
                this.currentMediMate.NOTE = txtNote.Text;

                if (this.currentMediMate.IsMedicine && !this.currentMediMate.IsBlood)
                {
                    this.currentMediMate.ExpMedicine.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMedicine.Description = txtNote.Text;
                }
                else if (!this.currentMediMate.IsMedicine && !this.currentMediMate.IsBlood)
                {
                    this.currentMediMate.ExpMaterial.Amount = spinExpAmount.Value;
                    this.currentMediMate.ExpMaterial.Description = txtNote.Text;
                }
                else
                {
                    this.currentMediMate.ExpBlood.Amount = (long)spinExpAmount.Value;
                    this.currentMediMate.ExpBlood.Description = txtNote.Text;
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
                SetDataGridControlDetail();
                this.currentMediMate = null;
                SetValueByMediMateADO();
                SetFocusMediOrMateStock();
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
                if (cboExpMediStock.EditValue == null)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                bool success = false;

                //Review
                HisExpMestDepaSDO data = new HisExpMestDepaSDO();
                data.Description = txtDescription.Text;
                data.ReqRoomId = this.currentRoom.ID;
                if (lciRemedyCount.Enabled && spinRemedyCount.EditValue != null)
                {
                    data.RemedyCount = Convert.ToInt64(spinRemedyCount.EditValue);
                }
                var mestRoom = listExpMediStock.FirstOrDefault(o => o.MEDI_STOCK_ID == Convert.ToInt64(cboExpMediStock.EditValue));
                if (mestRoom != null)
                {
                    data.MediStockId = mestRoom.MEDI_STOCK_ID;
                }

                if (cboREASON.EditValue != null)
                {
                    data.ExpMestReasonId = (long)cboREASON.EditValue;
                }
                data.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboExpMediStock.EditValue.ToString());

                data.Materials = new List<ExpMaterialTypeSDO>();
                data.Medicines = new List<ExpMedicineTypeSDO>();
                data.Bloods = new List<ExpBloodTypeSDO>();

                string str = "";
                foreach (var item in dicMediMateAdo)
                {
                    if (item.Value.EXP_AMOUNT <= 0)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong);
                        goto End;
                    }
                    if (item.Value.AVAILABLE_AMOUNT < item.Value.EXP_AMOUNT)
                    {
                        str += item.Value.MEDI_MATE_TYPE_NAME + "; ";
                    }
                    if (item.Value.IsMedicine && !item.Value.IsBlood)
                    {
                        data.Medicines.Add(item.Value.ExpMedicine);
                    }
                    else if (!item.Value.IsMedicine && !item.Value.IsBlood)
                    {
                        data.Materials.Add(item.Value.ExpMaterial);
                    }
                    else
                    {
                        data.Bloods.Add(item.Value.ExpBlood);
                    }
                }
                if (data.Materials.Count == 0)
                {
                    data.Materials = null;
                }
                if (data.Medicines.Count == 0)
                {
                    data.Medicines = null;
                }
                if (data.Bloods.Count == 0)
                {
                    data.Bloods = null; ;
                }

                if (!String.IsNullOrEmpty(str) && DevExpress.XtraEditors.XtraMessageBox.Show("(" + str + ") " +
                     Base.ResourceMessageLang.SoLuongXuatLonHonSoLuonKhaDungTrongKho + " Yêu cầu tiếp tục",
                     MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                     MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/DepaCreate", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    isUpdate = true;
                    this.resultSdo = rs;
                }
                //}
                if (success)
                {
                    if (mestRoom != null)
                    {
                        LoadDataToTreeList(mestRoom);
                    }
                    ddBtnPrint.Enabled = true;
                    cboExpMediStock.Enabled = false;
                    if (Config.DisableButtonSaveAfterSaveCFG.IsDisable)
                    {
                        btnSave.Enabled = false;
                    }
                }
            End:
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled || this.currentRoom == null)
                    return;
                WaitingManager.Show();
                ResetControlCommon();
                SetDataGridControlDetail();
                V_HIS_MEST_ROOM mestRoom = null;
                if (cboExpMediStock.EditValue != null)
                {
                    mestRoom = listExpMediStock.FirstOrDefault(o => o.MEDI_STOCK_ID == Convert.ToInt64(cboExpMediStock.EditValue));
                }
                LoadDataToTreeList(mestRoom);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewExpMestDetail.FocusedRowHandle < 0)
                    return;
                var data = (MediMateTypeADO)gridViewExpMestDetail.GetFocusedRow();
                if (data != null)
                {
                    dicMediMateAdo.Remove(data.SERVICE_ID);
                }
                SetDataGridControlDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuXuatSuDung(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatSuDung_MPS000135, delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuXuatSuDungTheoDieuKien(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                frmPrintByCondition frm = new frmPrintByCondition(this.resultSdo);
                frm.ShowDialog();
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
                //Review
                if (this.resultSdo != null)
                {
                    //if (this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT || this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST || this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                    //{
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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultSdo), resultSdo));
                //  WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisExpMestViewFilter depaFilter = new HisExpMestViewFilter();
                depaFilter.ID = this.resultSdo.ExpMest.ID;
                var listDepaExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, depaFilter, null);
                if (listDepaExpMest == null || listDepaExpMest.Count != 1)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong lay duoc EXP_MEST bang ID");
                    throw new NullReferenceException("Khong lay duoc EXP_MEST bang ID");
                }
                var _ExpMest = listDepaExpMest.First();
                List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();
                if (this.resultSdo != null)
                {
                    if (this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                        || this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        mediFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                        _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                        MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                        matyFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                        _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);

                        MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                        bloodFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                        _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestBloods), _ExpMestBloods));

                        MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                        metyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                        this.resultSdo.ExpMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, param);

                        MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                        matyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                        this.resultSdo.ExpMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, param);

                    }
                    MOS.Filter.HisExpMestBltyReqFilter bltyReqFilter = new HisExpMestBltyReqFilter();
                    bltyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                    this.resultSdo.ExpBltyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/Get", ApiConsumers.MosConsumer, bltyReqFilter, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultSdo.ExpBltyReqs), resultSdo.ExpBltyReqs));

                }

                //Review
                //if (keyPrintType == 1)
                //{
                //    MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                //    resultSdo.ExpMetyReqs,
                //    resultSdo.ExpMatyReqs,
                //    _ExpMestMedicines,
                //    _ExpMestMaterials,
                //    _ExpMest,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                //    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                //    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                //    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuTongHop
                //                );
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //    }
                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                //else
                {
                    #region
                    // tách riêng gây nghiện hướng thần thành một bản in
                    _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                    if (this.resultSdo != null && this.resultSdo.ExpMetyReqs != null && this.resultSdo.ExpMetyReqs.Count > 0)
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
                                if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_HTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_GNs.Add(item);
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
                    #endregion
                    long keyGOP = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                    if (keyGOP == 1)
                    {
                        #region ---- GN HT ----
                        if (_ExpMestMetyReq_GN_HTs != null && _ExpMestMetyReq_GN_HTs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_GN_HTs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuGN_HT
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion
                    }
                    else
                    {
                        #region ---- GN ----
                        if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_GNs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuGayNghien
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion

                        #region ---- HT ----
                        if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_HTs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                   MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuHuongThan
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion
                    }

                    #region ---- TD ----
                    if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_TDs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuThuocDoc
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- PX ----
                    if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_PXs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuThuocPhongXa
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- T ----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_Ts,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuThuocThuong
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ----- Mau -----
                    if (resultSdo.ExpBltyReqs != null && resultSdo.ExpBltyReqs.Count > 0)
                    {
                        HisBloodTypeViewFilter fb = new HisBloodTypeViewFilter();
                        fb.IDs = resultSdo.ExpBltyReqs.Select(o => o.BLOOD_TYPE_ID).ToList();
                        var dataBlood = new BackendAdapter(param).Get<List<V_HIS_BLOOD_TYPE>>("api/HisBloodType/GetView", ApiConsumers.MosConsumer, fb, param);

                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    null,
                    null,
                    resultSdo.ExpBltyReqs,
                    null,
                    null,
                    _ExpMestBloods,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.Mau
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    #endregion

                    #region ---- VT ----
                    if (resultSdo.ExpMatyReqs != null && resultSdo.ExpMatyReqs.Count > 0)
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
                        if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    null,
                    _ExpMestMatyReq_VTs,
                    null,
                    _ExpMestMaterials,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuVatTu
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        if (_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0)
                        {
                            MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    null,
                    _ExpMestMatyReq_HCs,
                    null,
                    _ExpMestMaterials,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.phieuHoaChat
                                );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    #endregion

                    #region ---- CO ----
                    if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_COs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.Corticoid
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DT ----
                    if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_DTs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.DichTruyen
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- KS ----
                    if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_KSs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.KhangSinh
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- LAO ----
                    if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                    {
                        MPS.Processor.Mps000135.PDO.Mps000135PDO mps000135RDO = new MPS.Processor.Mps000135.PDO.Mps000135PDO(
                    _ExpMestMetyReq_LAOs,
                    null,
                    _ExpMestMedicines,
                    null,
                    _ExpMest,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    MPS.Processor.Mps000135.PDO.Mps000135PDO.keyTitles.Lao
                                );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000135RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
