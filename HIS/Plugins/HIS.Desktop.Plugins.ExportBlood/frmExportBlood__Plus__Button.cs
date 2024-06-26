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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ExportBlood.ADO;
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
    public partial class frmExportBlood : Form
    {
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnAdd.Enabled || !dxValidationProvider1.Validate() || ExpMest == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                string bloodCode = txtBarCode.Text.Trim();
                if (String.IsNullOrEmpty(bloodCode))
                {
                    param.Messages.Add(Base.ResourceMessageManager.ThieuTruongDuLieuBatBuoc);
                    goto End;
                }
                if (ListBlood.Count > 0 && ListBlood.Select(s => s.BLOOD_CODE).Contains(bloodCode))
                {
                    param.Messages.Add(Base.ResourceMessageManager.TuiMauDaDuocThemVaoDanhSachXuat);
                    goto End;
                }
                if (dtExpiredDate.EditValue == null || dtExpiredDate.DateTime == DateTime.MinValue)
                {
                    param.Messages.Add(Base.ResourceMessageManager.ThieuTruongDuLieuBatBuoc);
                    goto End;
                }
                HisBloodViewFilter bloodFilter = new HisBloodViewFilter();
                bloodFilter.BLOOD_CODE__EXACT = txtBarCode.Text.Trim();
                bloodFilter.MEDI_STOCK_ID = ExpMest.MEDI_STOCK_ID;
                var hisBloods = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BLOOD>>(HisRequestUriStore.HIS_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                if (hisBloods == null || hisBloods.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageManager.MaVachKhongChinhXac);
                    goto End;
                }
                if (hisBloods.Count > 1)
                {
                    param.Messages.Add(Base.ResourceMessageManager.MaVachKhongChinhXac);
                    goto End;
                }
                VHisBloodADO blood = new VHisBloodADO(hisBloods[0]);
                if (blood.BLOOD_TYPE_ID != currentExpMestBlty.BLOOD_TYPE_ID)
                {
                    param.Messages.Add(Base.ResourceMessageManager.TuiMauKhongThuocLoaiMauDangChon);
                    goto End;
                }

                success = true;
                blood.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
                blood.ExpMestBltyId = currentExpMestBlty.ID;
                blood.PATIENT_TYPE_ID = currentExpMestBlty.PATIENT_TYPE_ID;
                blood.PATIENT_TYPE_CODE = currentExpMestBlty.PATIENT_TYPE_CODE;
                blood.PATIENT_TYPE_NAME = currentExpMestBlty.PATIENT_TYPE_NAME;
                if (!dicBlood.ContainsKey(currentExpMestBlty.BLOOD_TYPE_ID))
                    dicBlood[currentExpMestBlty.BLOOD_TYPE_ID] = new List<VHisBloodADO>();
                bool valid = false;
                if (dicBlood[currentExpMestBlty.BLOOD_TYPE_ID].Count() >= currentExpMestBlty.AMOUNT)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Base.ResourceMessageManager.SoLuongTuiMauCuaLoaiLonHonSoLuongYeuCau, currentExpMestBlty.BLOOD_TYPE_NAME), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        valid = true;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    valid = true;
                }
                if (valid)
                {
                    ListBlood.Add(blood);
                    dicBlood[currentExpMestBlty.BLOOD_TYPE_ID].Add(blood);
                    gridControlBlood.BeginUpdate();
                    gridControlBlood.DataSource = ListBlood;
                    gridControlBlood.EndUpdate();
                    if (success)
                    {
                        txtBarCode.Text = "";
                        dtExpiredDate.EditValue = null;
                        txtBarCode.Focus();
                        txtBarCode.SelectAll();
                    }
                }

            End:
                WaitingManager.Hide();
                if (!success)
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefresh.Enabled)
                    return;
                txtBarCode.Text = "";
                dtExpiredDate.EditValue = null;
                txtBarCode.Focus();
                txtBarCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled || ListBlood.Count == 0 || ExpMest == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                string messageFail = "";
                bool valid = true;
                foreach (var expMestBlty in ListExpMestBlty)
                {
                    if (!dicBlood.ContainsKey(expMestBlty.BLOOD_TYPE_ID))
                    {
                        messageFail = messageFail + MessageUtil.GetMessage(LibraryMessage.Message.Enum.Plugins_ExportBlood__KhongCoTuiMauNaoThuocLoaiMau, new string[] { expMestBlty.BLOOD_TYPE_NAME }) + ".";
                        if (valid)
                            valid = false;
                    }
                    else
                    {
                        if (dicBlood[expMestBlty.BLOOD_TYPE_ID].Count < expMestBlty.AMOUNT)
                        {
                            messageFail = messageFail + MessageUtil.GetMessage(LibraryMessage.Message.Enum.Plugins_ExportBlood__SoLuongTuiMauThuocLoaiMauNhoHonSoLuongYeuCau, new string[] { expMestBlty.BLOOD_TYPE_NAME }) + ".";
                            if (valid)
                                valid = false;
                        }
                    }
                }

                if (!valid)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(messageFail + " " + MessageUtil.GetMessage(LibraryMessage.Message.Enum.Common_BanCoDongYThucXuatHayKhong), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        valid = true;
                    }
                }

                if (valid)
                {
                    HisExportBloodSDO data = new HisExportBloodSDO();
                    data.ExpMestId = ExpMest.ID;
                    data.Bloods = new List<HisBloodSDO>();
                    foreach (var expMestBlty in ListExpMestBlty)
                    {
                        if (dicBlood.ContainsKey(expMestBlty.BLOOD_TYPE_ID))
                        {
                            foreach (var item in dicBlood[expMestBlty.BLOOD_TYPE_ID])
                            {
                                HisBloodSDO blood = new HisBloodSDO();
                                blood.BloodId = item.ID;
                                blood.ExpMestBltyId = item.ExpMestBltyId;
                                blood.ExpiredDate = item.EXPIRED_DATE ?? 0;
                                data.Bloods.Add(blood);
                            }
                        }
                    }
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(HisRequestUriStore.HIS_EXP_MEST_EXPORTBLOOD, ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        if (rs.ID != ExpMest.ID)
                        {
                            MessageUtil.GetMessage(LibraryMessage.Message.Enum.Plugins_ExportBlood__PhieuXuatTraVeKhongGiongVoiPhieuXuatGuiLen);
                        }
                        else
                        {
                            btnPrint.Enabled = true;
                            btnSave.Enabled = false;
                            btnAdd.Enabled = false;
                            success = true;
                            resultExpMest = rs;
                            LoadDataToGridExpMestBlood();
                        }
                    }
                }

                WaitingManager.Hide();
                MessageManager.Show(param, success);
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
                if (!btnPrint.Enabled || ExpMest == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditor.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__EXPORT_BLOOD__MPS000107, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void bbtnRCRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefresh_Click(null, null);
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void repositoryItemBtnDeleteBlood_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewBlood.FocusedRowHandle >= 0)
                {
                    var blood = (VHisBloodADO)gridViewBlood.GetFocusedRow();
                    if (blood != null)
                    {
                        ListBlood.Remove(blood);
                        if (dicBlood.ContainsKey(blood.BLOOD_TYPE_ID) && dicBlood[blood.BLOOD_TYPE_ID].Contains(blood))
                        {
                            dicBlood[blood.BLOOD_TYPE_ID].Remove(blood);
                        }
                        gridControlBlood.BeginUpdate();
                        gridControlBlood.DataSource = ListBlood;
                        gridControlBlood.EndUpdate();
                    }
                }
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

                MPS.Core.Mps000107.Mps000107RDO mps107Rdo = new MPS.Core.Mps000107.Mps000107RDO(serviceReq, ListExpMestBlty, ListExpMestBlood);
                WaitingManager.Hide();
                result = MPS.Printer.Run(printTypeCode, fileName, mps107Rdo);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadDataToGridExpMestBlood()
        {
            try
            {
                HisExpMestBloodViewFilter expMestBloodFilter = new HisExpMestBloodViewFilter();
                expMestBloodFilter.EXP_MEST_ID = resultExpMest.ID;
                ListExpMestBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, expMestBloodFilter, null);
                gridControlExpMestBlood.BeginUpdate();
                gridControlExpMestBlood.DataSource = ListExpMestBlood;
                gridControlExpMestBlood.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
