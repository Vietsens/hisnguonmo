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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ImportBlood.ADO;
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

namespace HIS.Desktop.Plugins.ImportBlood
{
    public partial class UCImportBloodPlus
    {
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string messageError = "";
                if (!CheckAllowAdd(ref messageError))
                {
                    MessageManager.Show(messageError);
                    return;
                }

                positionHandleControl = -1;
                //if (!btnAdd.Enabled || !dxValidationProvider1.Validate() || this.currentBlood == null)
                //    return;
                if (!dxValidationProvider1.Validate())
                    return;
                if (cboBloodAbo.EditValue == null)
                    return;
                if (this._isHienMau)
                {
                    if (this.updatingBloodGiverADO == null || String.IsNullOrEmpty(this.updatingBloodGiverADO.GIVE_CODE))
                        return;

                    if (this.dicHisBloodGiver_BloodAdo.ContainsKey(this.updatingBloodGiverADO.GIVE_CODE))
                    {
                        var listBloodADO = dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE];
                        if (listBloodADO.Exists(o=>o.BLOOD_CODE == txtBloodCode.Text.Trim()))
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Mã hồ sơ {0} đã tồn tại mã vạch(máu) {1} trong danh sách nhập. Bạn có muốn thay đổi?", this.updatingBloodGiverADO.GIVE_CODE, txtBloodCode.Text.Trim()), Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }
                    WaitingManager.Show();
                    var bloodAbo = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodAbo.EditValue));
                    if (bloodAbo != null)
                    {
                        currentBlood_BloodGiver_ForAdd.BLOOD_ABO_ID = bloodAbo.ID;
                        currentBlood_BloodGiver_ForAdd.BLOOD_ABO_CODE = bloodAbo.BLOOD_ABO_CODE;
                    }

                    if (cboBloodRh.EditValue != null)
                    {
                        var bloodRh = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodRh.EditValue));
                        if (bloodRh != null)
                        {
                            currentBlood_BloodGiver_ForAdd.BLOOD_RH_ID = bloodRh.ID;
                            currentBlood_BloodGiver_ForAdd.BLOOD_RH_CODE = bloodRh.BLOOD_RH_CODE;
                        }
                    }

                    currentBlood_BloodGiver_ForAdd.IMP_PRICE = spinImpPrice.Value;
                    currentBlood_BloodGiver_ForAdd.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                    currentBlood_BloodGiver_ForAdd.ImpVatRatio = spinImpVatRatio.Value;
                    if (dtPackingTime.EditValue != null && dtPackingTime.DateTime != DateTime.MinValue)
                    {
                        currentBlood_BloodGiver_ForAdd.PACKING_TIME = Convert.ToInt64(dtPackingTime.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    if (cboImpSource.EditValue != null)
                    {
                        var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpSource.EditValue));
                        if (impSource != null)
                        {
                            currentBlood_BloodGiver_ForAdd.IMP_SOURCE_ID = impSource.ID;
                            currentBlood_BloodGiver_ForAdd.IMP_SOURCE_CODE = impSource.IMP_SOURCE_CODE;
                            currentBlood_BloodGiver_ForAdd.IMP_SOURCE_NAME = impSource.IMP_SOURCE_NAME;
                        }
                    }
                    currentBlood_BloodGiver_ForAdd.PACKAGE_NUMBER = txtPackageNumber.Text;

                    if (checkIsInfect.Checked)
                    {
                        currentBlood_BloodGiver_ForAdd.IS_INFECT = 1;
                    }

                    if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                    {
                        currentBlood_BloodGiver_ForAdd.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    currentBlood_BloodGiver_ForAdd.BLOOD_CODE = txtBloodCode.Text.Trim();

                    if (resultADO != null && dicHisBloodGiver_BloodAdo.ContainsKey(this.updatingBloodGiverADO.GIVE_CODE))
                    {
                        var listBloodADO = dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE];
                        if (listBloodADO.Exists(o => o.BLOOD_CODE == currentBlood_BloodGiver_ForAdd.BLOOD_CODE))
                        {
                            var blood = dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE].Where(o => o.BLOOD_CODE == currentBlood_BloodGiver_ForAdd.BLOOD_CODE).FirstOrDefault();
                            currentBlood_BloodGiver_ForAdd.ID = blood != null ? blood.ID : 0;
                        }
                    }
                    if (dicHisBloodGiver_BloodAdo.ContainsKey(this.updatingBloodGiverADO.GIVE_CODE))
                    {
                        var listBloodADO = dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE];
                        if (listBloodADO.Exists(o => o.BLOOD_CODE == currentBlood_BloodGiver_ForAdd.BLOOD_CODE))
                        {
                            var blood = dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE].Where(o => o.BLOOD_CODE == currentBlood_BloodGiver_ForAdd.BLOOD_CODE).FirstOrDefault();
                            if (blood != null)
                            {
                                listBloodADO.Remove(blood);
                            }
                        }
                        listBloodADO.Add(currentBlood_BloodGiver_ForAdd);
                        if (listBloodADO.Exists(o => o.IsEmptyRow == true))
                        {
                            var emptyRow = dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE].Where(o => o.IsEmptyRow == true).FirstOrDefault();
                            if (emptyRow != null)
                            {
                                listBloodADO.Remove(emptyRow);
                            }
                        }
                        dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE] = listBloodADO;
                    }
                    WaitingManager.Hide();
                    SetDataSourceGridBlood_BloodGiver();
                    this.currentBlood_BloodGiver_ForAdd = null;
                    this.SetControlValueByBloodType(false);
                }
                else
                {
                    if (dicBloodAdo.ContainsKey(txtBloodCode.Text.Trim()))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.TuiMauDaCoTrongDanhSachNhap_BanCoMuonThayDoi, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                    }

                    WaitingManager.Show();
                    var bloodAbo = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodAbo.EditValue));
                    if (bloodAbo != null)
                    {
                        currentBlood.BLOOD_ABO_ID = bloodAbo.ID;
                        currentBlood.BLOOD_ABO_CODE = bloodAbo.BLOOD_ABO_CODE;
                    }

                    if (cboBloodRh.EditValue != null)
                    {
                        var bloodRh = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodRh.EditValue));
                        if (bloodRh != null)
                        {
                            currentBlood.BLOOD_RH_ID = bloodRh.ID;
                            currentBlood.BLOOD_RH_CODE = bloodRh.BLOOD_RH_CODE;
                        }
                    }

                    currentBlood.IMP_PRICE = spinImpPrice.Value;
                    currentBlood.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                    currentBlood.ImpVatRatio = spinImpVatRatio.Value;
                    if (dtPackingTime.EditValue != null && dtPackingTime.DateTime != DateTime.MinValue)
                    {
                        currentBlood.PACKING_TIME = Convert.ToInt64(dtPackingTime.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    if (cboImpSource.EditValue != null)
                    {
                        var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpSource.EditValue));
                        if (impSource != null)
                        {
                            currentBlood.IMP_SOURCE_ID = impSource.ID;
                            currentBlood.IMP_SOURCE_CODE = impSource.IMP_SOURCE_CODE;
                            currentBlood.IMP_SOURCE_NAME = impSource.IMP_SOURCE_NAME;
                        }
                    }
                    currentBlood.GIVE_CODE = txtGiveCode.Text;
                    currentBlood.GIVE_NAME = txtGiveName.Text;
                    //currentBlood.BID_NUM_ORDER = txtBidNumOrder.Text;
                    currentBlood.PACKAGE_NUMBER = txtPackageNumber.Text;

                    if (checkIsInfect.Checked)
                    {
                        currentBlood.IS_INFECT = 1;
                    }
                    if (dtExpiredDate.EditValue != null)
                    {
                        //currentBlood.EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dtExpiredDate.EditValue.ToString());
                    }

                    if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                    {
                        currentBlood.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    currentBlood.BLOOD_CODE = txtBloodCode.Text.Trim();
                    if (resultADO != null && dicBloodAdo.ContainsKey(currentBlood.BLOOD_CODE))
                    {
                        currentBlood.ID = dicBloodAdo[currentBlood.BLOOD_CODE + "_" + currentBlood.BLOOD_TYPE_CODE].ID;
                    }
                    dicBloodAdo[currentBlood.BLOOD_CODE + "_" + currentBlood.BLOOD_TYPE_CODE] = currentBlood;
                    bloodTypeADO = new UC.BloodType.ADO.BloodTypeADO();
                    V_HIS_BLOOD_TYPE hisBookType = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().Where(o => o.BLOOD_TYPE_CODE == currentBlood.BLOOD_TYPE_CODE).FirstOrDefault();
                    Inventec.Common.Mapper.DataObjectMapper.Map<UC.BloodType.ADO.BloodTypeADO>(bloodTypeADO, hisBookType);

                    this.SetDataSourceGridBlood();
                    //this.ProcessChoiceBloodTypeADO(null);
                    gridBloodType_RowClick(bloodTypeADO);
                    //this.SetTextAfterAdd(currentBlood);
                    this.bloodTypeProcessor.FocusKeyword(this.ucBloodType);
                    txtBloodCode.Focus();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnUpdate.Enabled || !dxValidationProvider1.Validate() || this.currentBlood == null)
                    return;
                if (cboBloodAbo.EditValue == null)
                    return;
                WaitingManager.Show();
                var bloodAbo = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodAbo.EditValue));
                if (bloodAbo != null)
                {
                    currentBlood.BLOOD_ABO_ID = bloodAbo.ID;
                    currentBlood.BLOOD_ABO_CODE = bloodAbo.BLOOD_ABO_CODE;
                }

                if (cboBloodRh.EditValue != null)
                {
                    var bloodRh = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboBloodRh.EditValue));
                    if (bloodRh != null)
                    {
                        currentBlood.BLOOD_RH_ID = bloodRh.ID;
                        currentBlood.BLOOD_RH_CODE = bloodRh.BLOOD_RH_CODE;
                    }
                }

                currentBlood.IMP_PRICE = spinImpPrice.Value;
                currentBlood.IMP_VAT_RATIO = spinImpVatRatio.Value / 100;
                currentBlood.ImpVatRatio = currentBlood.IMP_VAT_RATIO;
                if (dtPackingTime.EditValue != null && dtPackingTime.DateTime != DateTime.MinValue)
                {
                    currentBlood.PACKING_TIME = Convert.ToInt64(dtPackingTime.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    currentBlood.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (cboImpSource.EditValue != null)
                {
                    var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpSource.EditValue));
                    if (impSource != null)
                    {
                        currentBlood.IMP_SOURCE_ID = impSource.ID;
                        currentBlood.IMP_SOURCE_CODE = impSource.IMP_SOURCE_CODE;
                        currentBlood.IMP_SOURCE_NAME = impSource.IMP_SOURCE_NAME;
                    }
                }
                currentBlood.GIVE_CODE = txtGiveCode.Text;
                currentBlood.GIVE_NAME = txtGiveName.Text;
                //currentBlood.BID_NUM_ORDER = txtBidNumOrder.Text;
                currentBlood.PACKAGE_NUMBER = txtPackageNumber.Text;

                if (checkIsInfect.Checked)
                {
                    currentBlood.IS_INFECT = 1;
                }
                dicBloodAdo.Remove(currentBlood.BLOOD_CODE + "_" + currentBlood.BLOOD_TYPE_CODE);
                currentBlood.BLOOD_CODE = txtBloodCode.Text.Trim();
                dicBloodAdo[currentBlood.BLOOD_CODE + "_" + currentBlood.BLOOD_TYPE_CODE] = currentBlood;
                dicBloodAdo.Reverse();
                this.SetDataSourceGridBlood();
                this.ProcessChoiceBloodTypeADO(null);
                this.bloodTypeProcessor.FocusKeyword(this.ucBloodType);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCancel.Enabled)
                    return;
                this.ProcessChoiceBloodTypeADO(null);
                this.bloodTypeProcessor.FocusKeyword(this.ucBloodType);
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
                positionHandleControl = -1;
                if (!btnSave.Enabled)
                    return;
                if (this._isHienMau)
                {
                    if (this.dicHisBloodGiver_BloodAdo == null || this.dicHisBloodGiver_BloodAdo.Count == 0)
                        return;
                }
                else
                {
                    if (!dxValidationProvider2.Validate() || dicBloodAdo.Count == 0)
                        return;
                    List<string> lstMessage = new List<string>();
                    var ImpMestListProcessor = dicBloodAdo.Select(o => o.Value).ToList();
                    ProcessListBloodTypeToProcessList(ref ImpMestListProcessor);
                    dicBloodAdo = new Dictionary<string, VHisBloodADO>();
                    foreach (var item in ImpMestListProcessor)
                    {
                        dicBloodAdo[item.BLOOD_CODE + "_" + item.BLOOD_TYPE_CODE] = item;
                    }
                    // kiểm tra vat hợp lệ
                    foreach (var item in dicBloodAdo)
                    {
                        if (item.Value != null && (item.Value.ImpVatRatio < 0 || item.Value.ImpVatRatio > 100))
                        {
                            return;
                        }
                        if (item.Value != null && item.Value.ErrorDescriptions != null && item.Value.ErrorDescriptions.Count > 0)
                        {
                            lstMessage.Add(string.Format("{0}: {1}", item.Value.BLOOD_TYPE_NAME ?? item.Value.BLOOD_TYPE_CODE ?? item.Value.BLOOD_CODE, string.Join(", ", item.Value.ErrorDescriptions)));
                        }
                    }
                    if(lstMessage != null && lstMessage.Count > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Join("\r\n", lstMessage), Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                        return;
                    }
                }
                
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_IMP_MEST_TYPE impMestType = null;
                if (cboImpMestType.EditValue != null)
                {
                    impMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));
                }
                if (impMestType != null)
                {
                    var sdo = getImpMestTypeSDORequest(impMestType, IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST);
                    if (sdo != null)
                    {
                        //btnHoiDongKiemNhap.Enabled = false;

                        if (sdo.GetType() == typeof(HisImpMestManuSDO))
                        {
                            HisImpMestManuSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestManuSDO>(HisRequestUri.HIS_MANU_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestManuSDO>(HisRequestUri.HIS_MANU_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.ManuBloods;


                                this.InitMenuToButtonPrint(resultADO.HisManuSDO.ImpMest);

                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestInitSDO))
                        {
                            HisImpMestInitSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInitSDO>(HisRequestUri.HIS_INIT_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInitSDO>(HisRequestUri.HIS_INIT_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.InitBloods;
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestInveSDO))
                        {
                            HisImpMestInveSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInveSDO>(HisRequestUri.HIS_INVE_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInveSDO>(HisRequestUri.HIS_INVE_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.InveBloods;
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestOtherSDO))
                        {
                            HisImpMestOtherSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>(HisRequestUri.HIS_OTHER_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>(HisRequestUri.HIS_OTHER_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.OtherBloods;
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestDonationSDO))
                        {
                            HisImpMestDonationSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestDonationSDO>(HisRequestUri.HIS_DONATION_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestDonationSDO>(HisRequestUri.HIS_DONATION_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                            }
                        }
                        if (success)
                        {
                            this.ProcessSaveSuccess();
                            impMestId = resultADO.ImpMestId;
                            btnHoiDongKiemNhap.Enabled = true;
                            cboPrint.Enabled = true;
                            btnSave.Enabled = false;
                            btnSaveDraft.Enabled = false;
                        }
                    }
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this.ParentForm, param, success);
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

        private void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSaveDraft.Enabled || !dxValidationProvider2.Validate() || dicBloodAdo.Count == 0)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_IMP_MEST_TYPE impMestType = null;
                if (cboImpMestType.EditValue != null)
                {
                    impMestType = BackendDataWorker.Get<HIS_IMP_MEST_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMestType.EditValue));
                }
                if (impMestType != null)
                {
                    var sdo = getImpMestTypeSDORequest(impMestType, IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT);
                    if (sdo != null)
                    {
                        if (sdo.GetType() == typeof(HisImpMestManuSDO))
                        {
                            HisImpMestManuSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestManuSDO>(HisRequestUri.HIS_MANU_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestManuSDO>(HisRequestUri.HIS_MANU_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.ManuBloods;
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestInitSDO))
                        {
                            //param.Messages.Add("Chức năng nhập đầu kỳ đang trong qua trình hoàn thiện.");
                            HisImpMestInitSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInitSDO>(HisRequestUri.HIS_INIT_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInitSDO>(HisRequestUri.HIS_INIT_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.InitBloods;
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestInveSDO))
                        {
                            //param.Messages.Add("Chức năng nhập kiểm kê đang trong qua trình hoàn thiện.");
                            HisImpMestInveSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInveSDO>(HisRequestUri.HIS_INVE_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestInveSDO>(HisRequestUri.HIS_INVE_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.InveBloods;
                            }
                        }
                        else if (sdo.GetType() == typeof(HisImpMestOtherSDO))
                        {
                            //param.Messages.Add("Chức năng nhập khác đang trong qua trình hoàn thiện.");
                            HisImpMestOtherSDO rs = null;
                            if (resultADO == null)
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>(HisRequestUri.HIS_OTHER_IMP_MEST_CREATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            else
                            {
                                rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisImpMestOtherSDO>(HisRequestUri.HIS_OTHER_IMP_MEST_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                            }
                            if (rs != null)
                            {
                                success = true;
                                this.resultADO = new ResultImpMestADO(rs);
                                this.resultADO.HisBloodSDOs = new List<HIS_BLOOD>();
                                this.resultADO.HisBloodSDOs = rs.OtherBloods;
                            }
                        }
                        if (success)
                        {
                            this.ProcessSaveSuccess();
                            cboPrint.Enabled = true;
                        }
                    }
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this.ParentForm, param, success);
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                WaitingManager.Show();
                ResetControlCommon();
                this.FillDataToGridBloodType();
                this.SetDefaultImpMestType();
                this.SetDataSourceGridBlood();
                this.SetDefaultValueMediStock();
                this.ProcessChoiceBloodTypeADO(null);
                cboPrint.Enabled = false;
                this.bloodTypeProcessor.FocusKeyword(this.ucBloodType);
                AllowImpMest();

                SetDefaultDataBloodGiverForm();
                this.dicHisBloodGiver = new Dictionary<string, HisBloodGiverADO>();
                this.dicHisBloodGiver_BloodAdo = new Dictionary<string, List<VHisBloodADO>>();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!cboPrint.Enabled || this.resultADO == null)
                    return;
                //Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                //if (resultADO != null && resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                //{
                //    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuNhapMauTuNhaCungCap_MPS000149, delegatePrintTemplate);
                //}
                //else
                //{
                //    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuNhapMauKhacDauKyKiemKe_MPS000212, delegatePrintTemplate);
                //}
                cboPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //private void btnRoleUser_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (this.impMestId > 0)
        //        {
        //            Inventec.Desktop.Common.Message.WaitingManager.Show();
        //            Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisRoleUser").FirstOrDefault();
        //            if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisRoleUser");
        //            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
        //            {
        //                List<object> listArgs = new List<object>();
        //                MOS.EFMODEL.DataModels.HIS_IMP_MEST ado = new HIS_IMP_MEST();
        //                ado.ID = this.impMestId;
        //                ado.IMP_MEST_TYPE_ID = this.IMP_MEST_TYPE_ID;
        //                ado.IMP_MEST_STT_ID = this.ImpMestSttId;
        //                listArgs.Add(ado);
        //                listArgs.Add(this.impMest);
        //                listArgs.Add(this.impMestMaterials);
        //                listArgs.Add(this.impMestMedicines);
        //                listArgs.Add(this.impMestBloods);
        //                listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0));
        //                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
        //                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

        //                ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
        //            }
        //            Inventec.Desktop.Common.Message.WaitingManager.Hide();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (!btnHoi.Enabled || this.resultADO == null)
            //        return;
            //    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
            //    if (resultADO != null && resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
            //    {
            //        store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuNhapMauTuNhaCungCap_MPS000149, delegatePrintTemplate);
            //    }
            //    else
            //    {
            //        store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuNhapMauKhacDauKyKiemKe_MPS000212, delegatePrintTemplate);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewImpMestBlood.FocusedRowHandle < 0)
                    return;
                var data = (VHisBloodADO)gridViewImpMestBlood.GetFocusedRow();
                if (this._isHienMau)
                {
                    if (data != null)
                    {
                        if (data.IsEmptyRow)
                        {
                            this.dicHisBloodGiver_BloodAdo.Remove(data.GIVE_CODE);
                            this.dicHisBloodGiver.Remove(data.GIVE_CODE);
                        }
                        else
                        {
                            if (this.dicHisBloodGiver_BloodAdo.ContainsKey(data.GIVE_CODE))
                            {
                                var listBloodADO = dicHisBloodGiver_BloodAdo[data.GIVE_CODE];
                                if (listBloodADO != null && listBloodADO.Remove(data))
                                {
                                    if (listBloodADO.Count == 0)
                                    {
                                        VHisBloodADO emptyBloodADO = new VHisBloodADO();
                                        emptyBloodADO.IsEmptyRow = true;
                                        emptyBloodADO.IsBloodDonation = true;
                                        emptyBloodADO.BloodDonationCode = data.GIVE_CODE;
                                        emptyBloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1} - {2} - {3}", data.GIVE_CODE, data.GIVE_NAME, data.DOB_ForDisplay, data.GENDER_ForDisplay);
                                        emptyBloodADO.GIVE_CODE = data.GIVE_CODE;
                                        emptyBloodADO.GIVE_NAME = data.GIVE_NAME;
                                        listBloodADO.Add(emptyBloodADO);
                                    }
                                    this.dicHisBloodGiver_BloodAdo[data.GIVE_CODE] = listBloodADO;
                                }
                            }
                        }
                    }
                    SetDataSourceGridBlood_BloodGiver();
                }
                else
                {
                    if (data != null)
                    {
                        dicBloodAdo.Remove(data.BLOOD_CODE + "_" + data.BLOOD_TYPE_CODE);
                    }
                    SetDataSourceGridBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (gridViewImpMestBlood.FocusedRowHandle < 0)
                    return;
                WaitingManager.Show();
                this.currentBlood = (VHisBloodADO)gridViewImpMestBlood.GetFocusedRow();
                this.SetEnableButtonAdd(false);
                this.CheckBloodTypeInBid();
                this.SetControlValueByBloodType(false);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private object getImpMestTypeSDORequest(HIS_IMP_MEST_TYPE impMestType, long impMestSttId)
        {
            try
            {
                List<HIS_BLOOD> hisBloodSdos = new List<HIS_BLOOD>();
                long? impSourceId = null;
                if (cboImpSource.EditValue != null)
                {
                    var impSource = BackendDataWorker.Get<HIS_IMP_SOURCE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpSource.EditValue));
                    if (impSource != null)
                    {
                        impSourceId = impSource.ID;
                    }
                }
                long? supplierId = null;
                if (cboSupplier.EditValue != null)
                {
                    var supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboSupplier.EditValue));
                    if (supplier != null)
                    {
                        supplierId = supplier.ID;
                    }
                }

                long mediStockId = 0;
                if (cboMediStock.EditValue != null)
                {
                    var mediStock = listMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboMediStock.EditValue));
                    if (mediStock != null)
                    {
                        mediStockId = mediStock.ID;
                    }
                }

                if (dicBloodAdo != null)
                {
                    foreach (var dic in dicBloodAdo)
                    {
                        //HisBloodWithPatySDO bloodWithPaty = new HisBloodWithPatySDO();
                        HIS_BLOOD blood = new HIS_BLOOD();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BLOOD>(blood, dic.Value);
                        //bloodWithPaty.Blood = blood;
                        blood.IMP_SOURCE_ID = impSourceId;
                        //bloodWithPaty.BloodPaties = new List<HIS_BLOOD_PATY>();

                        //foreach (var item in dic.Value.HisBloodPatyAdos)
                        //{
                        //    if (!item.IsNotSell)
                        //    {
                        //        HIS_BLOOD_PATY bloodPaty = new HIS_BLOOD_PATY();
                        //        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BLOOD_PATY>(bloodPaty, item);
                        //        bloodWithPaty.BloodPaties.Add(bloodPaty);
                        //    }
                        //}
                        hisBloodSdos.Add(blood);
                    }
                }

                if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    HisImpMestInitSDO initSdo = new HisImpMestInitSDO();
                    if (resultADO != null)
                    {
                        initSdo = resultADO.HisInitSDO;
                        initSdo.InitBloods = hisBloodSdos;
                    }
                    else
                    {
                        initSdo.ImpMest = new HIS_IMP_MEST();
                        initSdo.InitBloods = new List<HIS_BLOOD>();
                    }
                    initSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    initSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    initSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    initSdo.ImpMest.REQ_ROOM_ID = this.currentModule.RoomId;
                    initSdo.InitBloods = hisBloodSdos;
                    return initSdo;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    HisImpMestInveSDO inveSdo = new HisImpMestInveSDO();
                    if (resultADO != null)
                    {
                        inveSdo = resultADO.HisInveSDO;
                        inveSdo.InveBloods = hisBloodSdos;
                    }
                    else
                    {
                        inveSdo.ImpMest = new HIS_IMP_MEST();
                        inveSdo.InveBloods = new List<HIS_BLOOD>();
                    }
                    inveSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    inveSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    inveSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    inveSdo.ImpMest.REQ_ROOM_ID = this.currentModule.RoomId;
                    inveSdo.InveBloods = hisBloodSdos;
                    return inveSdo;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    HisImpMestOtherSDO otherSdo = new HisImpMestOtherSDO();
                    if (resultADO != null)
                    {
                        otherSdo = resultADO.HisOtherSDO;
                        otherSdo.OtherBloods = hisBloodSdos;
                    }
                    else
                    {
                        otherSdo.ImpMest = new HIS_IMP_MEST();
                        otherSdo.OtherBloods = new List<HIS_BLOOD>();
                    }
                    otherSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    otherSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    otherSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    otherSdo.ImpMest.REQ_ROOM_ID = this.currentModule.RoomId;
                    otherSdo.OtherBloods = hisBloodSdos;
                    return otherSdo;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HM)
                {
                    HisImpMestDonationSDO donationSdo = new HisImpMestDonationSDO();
                    if (resultADO != null)
                    {
                        donationSdo = resultADO.HisDonationSDO ?? new HisImpMestDonationSDO();
                    }
                    if (donationSdo.ImpMest == null)
                        donationSdo.ImpMest = new HIS_IMP_MEST();
                    if (donationSdo.DonationDetail == null)
                        donationSdo.DonationDetail = new List<DonationDetailSDO>();
                    donationSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    donationSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    donationSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    donationSdo.ImpMest.REQ_ROOM_ID = this.currentModule.RoomId;
                    List<DonationDetailSDO> listDonationDetailSDO = new List<DonationDetailSDO>();
                    if (this.dicHisBloodGiver != null)
                    {
                        foreach (var item in this.dicHisBloodGiver)
                        {
                            DonationDetailSDO detailSDO = new DonationDetailSDO();
                            detailSDO.BloodGiver = item.Value;
                            List<HIS_BLOOD> listBlood = new List<HIS_BLOOD>();
                            if (this.dicHisBloodGiver_BloodAdo != null && this.dicHisBloodGiver_BloodAdo.ContainsKey(item.Key) && this.dicHisBloodGiver_BloodAdo[item.Key] != null)
                            {
                                foreach (var itemBlood in this.dicHisBloodGiver_BloodAdo[item.Key])
                                {
                                    if (itemBlood.IsEmptyRow)
                                        continue;
                                    HIS_BLOOD blood = new HIS_BLOOD();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BLOOD>(blood, itemBlood);
                                    listBlood.Add(blood);
                                }
                            }
                            detailSDO.Bloods = listBlood;
                            listDonationDetailSDO.Add(detailSDO);
                        }
                    }
                    donationSdo.DonationDetail = listDonationDetailSDO;
                    return donationSdo;
                }
                else if (impMestType.ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    HisImpMestManuSDO manuSdo = new HisImpMestManuSDO();
                    if (resultADO != null)
                    {
                        manuSdo = resultADO.HisManuSDO;
                    }
                    else
                    {
                        manuSdo.ImpMest = new HIS_IMP_MEST();
                        manuSdo.ManuBloods = new List<HIS_BLOOD>();
                    }
                    manuSdo.ImpMest.MEDI_STOCK_ID = mediStockId;
                    manuSdo.ImpMest.IMP_MEST_STT_ID = impMestSttId;
                    manuSdo.ImpMest.IMP_MEST_TYPE_ID = impMestType.ID;
                    manuSdo.ImpMest.DELIVERER = txtDeliever.Text;
                    manuSdo.ImpMest.DESCRIPTION = txtDescription.Text;
                    var totalPrice = dicBloodAdo.Select(s => s.Value).Sum(o => (o.IMP_PRICE));
                    var discount = spinDiscountPrice.Value;
                    manuSdo.ImpMest.DISCOUNT = discount;
                    if (totalPrice > 0)
                    {
                        spinDiscountRatio.Value = (discount / totalPrice) * 100;
                        manuSdo.ImpMest.DISCOUNT_RATIO = spinDiscountRatio.Value / 100;
                    }
                    manuSdo.ImpMest.DOCUMENT_PRICE = spinDocumentPrice.Value;
                    manuSdo.ImpMest.DOCUMENT_NUMBER = txtDocumentNumber.Text;
                    if (dtDocumentDate.EditValue != null && dtDocumentDate.DateTime != DateTime.MinValue)
                    {
                        manuSdo.ImpMest.DOCUMENT_DATE = Convert.ToInt64(dtDocumentDate.DateTime.ToString("yyyyMMddHHmmss"));
                    }

                    manuSdo.ImpMest.SUPPLIER_ID = supplierId.HasValue ? supplierId.Value : 0;
                    manuSdo.ImpMest.REQ_ROOM_ID = this.currentModule.RoomId;
                    manuSdo.ManuBloods = hisBloodSdos;
                    return manuSdo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private void ProcessSaveSuccess()
        {
            try
            {
                if (this.resultADO != null)
                {
                    cboImpMestType.EditValue = this.resultADO.ImpMestTypeId;
                    cboImpMestType.Enabled = false;
                    if (this._isHienMau)
                    {

                        SetDataSourceGridBlood_BloodGiver();
                    }
                    else
                    {
                        if (this.resultADO.HisBloodSDOs != null && this.resultADO.HisBloodSDOs.Count > 0)
                        {
                            foreach (var item in this.resultADO.HisBloodSDOs)
                            {
                                if (dicBloodAdo.ContainsKey(item.BLOOD_CODE))
                                {

                                    var ado = dicBloodAdo[item.BLOOD_CODE + "_" + BackendDataWorker.Get<HIS_BLOOD_TYPE>().FirstOrDefault(o=>o.ID == item.BLOOD_TYPE_ID).BLOOD_TYPE_CODE];
                                    if (ado != null)
                                    {
                                        ado.SetValueByHisBlood(item);
                                        //if (item.BloodPaties == null)
                                        //    continue;
                                        //foreach (var paty in item.BloodPaties)
                                        //{
                                        //    var patyAdo = ado.HisBloodPatyAdos.FirstOrDefault(o => o.PATIENT_TYPE_ID == paty.PATIENT_TYPE_ID);
                                        //    if (patyAdo != null)
                                        //    {
                                        //        patyAdo.EXP_PRICE = paty.EXP_PRICE;
                                        //        patyAdo.EXP_VAT_RATIO = paty.EXP_VAT_RATIO;
                                        //        patyAdo.ExpVatRatio = paty.EXP_VAT_RATIO * 100;
                                        //    }
                                        //}
                                    }
                                }
                            }
                        }
                        SetDataSourceGridBlood();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlCommon()
        {
            try
            {
                positionHandleControl = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider2, dxErrorProvider);
                this.currentBlood = null;
                this.currentImpMestType = null;
                this.resultADO = null;
                dicBloodAdo.Clear();
                //listBloodPatyAdo = new List<VHisBloodPatyADO>();
                //cboMediStock.EditValue = null;
                cboImpMestType.EditValue = null;
                cboImpSource.EditValue = null;
                cboSupplier.EditValue = null;
                txtDocumentNumber.EditValue = null;
                txtDocumentNumber.Text = "";
                dtDocumentDate.EditValue = null;
                txtDeliever.Text = "";
                spinDiscountPrice.Value = 0;
                spinDiscountRatio.Value = 0;
                spinDocumentPrice.Value = 0;
                txtDescription.Text = "";
                cboMediStock.Enabled = false;
                txtMediStock.Enabled = false;
                txtPackingTime.Text = "";
                dtPackingTime.EditValue = null;
                dtExpiredDate.EditValue = null;
                //cboPrint.Enabled = true;
                btnSaveDraft.Enabled = true;
                btnSave.Enabled = true;
                RemoveControlDxError1();
                RemoveControlDxError2();
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
                if (resultADO == null || (resultADO.HisManuSDO == null && resultADO.HisInitSDO == null && resultADO.HisInveSDO == null && resultADO.HisOtherSDO == null))
                    return result;
                WaitingManager.Show();
                // nhập từ nhà cung cấp
                if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    HisImpMestViewFilter manuFilter = new HisImpMestViewFilter();
                    manuFilter.ID = resultADO.HisManuSDO.ImpMest.ID;
                    var listManuImpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, manuFilter, null);
                    if (listManuImpMest == null || listManuImpMest.Count != 1)
                    {
                        throw new NullReferenceException("Khong lay duoc VHisManuImpMest theo Id");
                    }
                    var manuImpMest = listManuImpMest.First();
                    HisImpMestBloodViewFilter impBloodFilter = new HisImpMestBloodViewFilter();
                    impBloodFilter.IMP_MEST_ID = resultADO.HisManuSDO.ImpMest.ID;
                    var listImpMestBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impBloodFilter, null);
                    if (listImpMestBlood == null || listImpMestBlood.Count <= 0)
                    {
                        throw new NullReferenceException("Khong lay duoc danh sach VHisImpMestBlood theo ImpMestId");
                    }
                    MPS.Processor.Mps000149.PDO.Mps000149PDO rdo = new MPS.Processor.Mps000149.PDO.Mps000149PDO(manuImpMest, listImpMestBlood);
                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null));
                    }
                }
                else if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK || resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC || resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    string TitleByImpMestType = "";
                    HisImpMestViewFilter manuFilter = new HisImpMestViewFilter();
                    HisImpMestBloodViewFilter impBloodFilter = new HisImpMestBloodViewFilter();
                    if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                    {
                        TitleByImpMestType = "Phiếu nhập máu khác";
                        manuFilter.ID = resultADO.HisOtherSDO.ImpMest.ID;
                        impBloodFilter.IMP_MEST_ID = resultADO.HisOtherSDO.ImpMest.ID;
                    }
                    else if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                    {
                        TitleByImpMestType = "Phiếu nhập máu đầu kỳ";
                        manuFilter.ID = resultADO.HisInitSDO.ImpMest.ID;
                        impBloodFilter.IMP_MEST_ID = resultADO.HisInitSDO.ImpMest.ID;
                    }
                    else if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                    {
                        TitleByImpMestType = "Phiếu nhập máu kiểm kê";
                        manuFilter.ID = resultADO.HisInveSDO.ImpMest.ID;
                        impBloodFilter.IMP_MEST_ID = resultADO.HisInveSDO.ImpMest.ID;
                    }

                    var listImpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, manuFilter, null);
                    if (listImpMest == null || listImpMest.Count != 1)
                    {
                        throw new NullReferenceException("Khong lay duoc VHisManuImpMest theo Id");
                    }
                    var ImpMest = listImpMest.First();
                    var listImpMestBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impBloodFilter, null);
                    if (listImpMestBlood == null || listImpMestBlood.Count <= 0)
                    {
                        throw new NullReferenceException("Khong lay duoc danh sach VHisImpMestBlood theo ImpMestId");
                    }
                    MPS.Processor.Mps000212.PDO.Mps000212PDO rdo = new MPS.Processor.Mps000212.PDO.Mps000212PDO(ImpMest, listImpMestBlood, TitleByImpMestType);
                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
