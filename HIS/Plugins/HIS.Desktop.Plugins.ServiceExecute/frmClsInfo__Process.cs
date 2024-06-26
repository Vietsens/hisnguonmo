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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.EkipTemp;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmClsInfo : Form
    {
        private void SetIcdFromServiceReq(V_HIS_SERVICE_REQ data)
        {
            try
            {
                if (!String.IsNullOrEmpty(data.ICD_CODE) && !String.IsNullOrEmpty(data.ICD_NAME))
                {
                    this.LoadDefaultIcd(icdMainProcessor, ucIcdMain, data.ICD_CODE, data.ICD_NAME);
                }

                if (!string.IsNullOrEmpty(data.ICD_SUB_CODE) && !string.IsNullOrEmpty(data.ICD_TEXT))
                {
                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();

                    subIcd.ICD_SUB_CODE = data.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = data.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                }

                //if (!string.IsNullOrEmpty(data.ICD_SUB_CODE))
                //{
                //    txtIcdExtraCode.Text = data.ICD_SUB_CODE;
                //}

                //if (!string.IsNullOrEmpty(data.ICD_TEXT))
                //{
                //    txtIcdText.Text = data.ICD_TEXT;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadSereServExt()
        {
            try
            {
                if (this.sereServExt != null)
                {
                    txtIntructionNote.Text = this.sereServExt.INSTRUCTION_NOTE;
                    //txtMachineCode.Text = this.sereServExt.MACHINE_CODE;
                    //cboMachine.EditValue = this.sereServExt.MACHINE_ID;
                    txtDescription.Text = this.sereServExt.DESCRIPTION;

                    //if (this.sereServExt.BEGIN_TIME.HasValue)
                    //{
                    //    dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.BEGIN_TIME.Value) ?? DateTime.Now;
                    //}
                    //else
                    //{
                    //    dtStart.EditValue = null;
                    //}
                    //if (this.sereServExt.END_TIME.HasValue)
                    //{
                    //    dtFinish.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.END_TIME.Value) ?? DateTime.Now;
                    //    dtFinish.Properties.Buttons[1].Visible = true;
                    //}
                    //else
                    //{
                    //    dtFinish.EditValue = null;
                    //    dtFinish.Properties.Buttons[1].Visible = false;
                    //}

                    //load dữ liệu lược đồ phẫu thuật thủ thuật
                    List<long> ssIds = new List<long>();
                    ssIds.Add(this.sereServExt.SERE_SERV_ID);
                    ProcessLoadSereServFile(ssIds);
                }
                else
                {
                    txtIntructionNote.Text = "";
                    txtMachineCode.Text = "";
                    cboMachine.EditValue = null;
                }

                //if (this.serviceReq != null && this.currentServiceADO.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq") == "1" && ((this.sereServExt != null && this.sereServExt.BEGIN_TIME == null) || this.sereServExt == null))
                //{
                //    dtStart.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serviceReq.INTRUCTION_TIME) ?? DateTime.Now;
                //}

                //xuandv
                //this.InitButtonOther();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetButtonDeleteGridLookup()
        {
            try
            {
                ButtonDeleteGridLookup(cboMethod);
                ButtonDeleteLookup(cbbPtttGroup);
                ButtonDeleteLookup(cbbEmotionlessMethod);
                ButtonDeleteLookup(cbbBlood);
                ButtonDeleteLookup(cbbBloodRh);
                ButtonDeleteLookup(cboCondition);
                ButtonDeleteLookup(cboCatastrophe);
                ButtonDeleteLookup(cboDeathSurg);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refreshControl()
        {
            try
            {
                //txtIcdText.Text = "";
                txtMethodCode.Text = "";
                cboMethod.EditValue = null;
                txtPtttGroupCode.Text = "";
                cbbPtttGroup.EditValue = null;
                txtEmotionlessMethod.Text = "";
                cbbEmotionlessMethod.EditValue = null;
                txtCatastrophe.Text = "";
                cboCatastrophe.EditValue = null;
                txtDeathSurg.Text = "";
                cboDeathSurg.EditValue = null;
                txtBlood.Text = "";
                cbbBlood.EditValue = null;
                txtBloodRh.Text = "";
                cbbBloodRh.EditValue = null;
                txtCondition.Text = "";
                cboCondition.EditValue = null;
                txtMANNER.Text = "";
                //dtFinish.EditValue = null;
                //dtStart.EditValue = null;
                //InitIcd();

                txtMoKTCao.Text = "";
                cboMoKTCao.EditValue = null;
                txtKQVoCam.Text = "";
                cboKQVoCam.EditValue = null;
                txtPhuongPhap2.Text = "";
                cboPhuongPhap2.EditValue = null;
                cboPhuongPhapThucTe.EditValue = null;
                txtPhuongPhapTT.Text = "";
                txtLoaiPT.Text = "";
                cboLoaiPT.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMethod.Focus();
                    cboMethod.ShowPopup();
                }
                else
                {
                    var data = datasPtttMethod.Where(o => o.PTTT_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMethod.EditValue = data[0].ID;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboMethod.EditValue = search.ID;
                                cboMethod.Properties.Buttons[1].Visible = true;
                                txtPhuongPhap2.Focus();
                                txtPhuongPhap2.SelectAll();
                            }
                            else
                            {
                                cboMethod.EditValue = null;
                                cboMethod.Focus();
                                cboMethod.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMethod.EditValue = null;
                        cboMethod.Focus();
                        cboMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadBlood(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbBlood.Focus();
                    cbbBlood.ShowPopup();
                }
                else
                {
                    var data = datasBloodABO.Where(o => o.BLOOD_ABO_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbBlood.EditValue = data[0].ID;
                            cbbBlood.Properties.Buttons[1].Visible = true;
                            txtBloodRh.Focus();
                            txtBloodRh.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.BLOOD_ABO_CODE == searchCode);
                            if (search != null)
                            {
                                cbbBlood.EditValue = search.ID;
                                cbbBlood.Properties.Buttons[1].Visible = true;
                                txtBloodRh.Focus();
                                txtBloodRh.SelectAll();
                            }
                            else
                            {
                                cbbBlood.EditValue = null;
                                cbbBlood.Focus();
                                cbbBlood.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbBlood.EditValue = null;
                        cbbBlood.Focus();
                        cbbBlood.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadEmotionlessMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbEmotionlessMethod.Focus();
                    cbbEmotionlessMethod.ShowPopup();
                }
                else
                {
                    var data = datasEmotionlessMethod2.Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_FIRST == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbEmotionlessMethod.EditValue = data[0].ID;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cbbEmotionlessMethod.EditValue = search.ID;
                                cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                                txtCondition.Focus();
                                txtCondition.SelectAll();
                            }
                            else
                            {
                                cbbEmotionlessMethod.EditValue = null;
                                cbbEmotionlessMethod.Focus();
                                cbbEmotionlessMethod.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbEmotionlessMethod.EditValue = null;
                        cbbEmotionlessMethod.Focus();
                        cbbEmotionlessMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadCondition(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCondition.Focus();
                    cboCondition.ShowPopup();
                }
                else
                {
                    var data = datasPtttCondition.Where(o => o.PTTT_CONDITION_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboCondition.EditValue = data[0].ID;
                            cboCondition.Properties.Buttons[1].Visible = true;
                            txtBlood.Focus();
                            txtBlood.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_CONDITION_CODE == searchCode);
                            if (search != null)
                            {
                                cboCondition.EditValue = search.ID;
                                cboCondition.Properties.Buttons[1].Visible = true;
                                txtBlood.Focus();
                                txtBlood.SelectAll();
                            }
                            else
                            {
                                cboCondition.EditValue = null;
                                cboCondition.Focus();
                                cboCondition.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboCondition.EditValue = null;
                        cboCondition.Focus();
                        cboCondition.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadBloodRh(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbBloodRh.Focus();
                    cbbBloodRh.ShowPopup();
                }
                else
                {
                    var data = datasBloodRh.Where(o => o.BLOOD_RH_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbBloodRh.EditValue = data[0].ID;
                            cbbBloodRh.Properties.Buttons[1].Visible = true;
                            txtCatastrophe.Focus();
                            txtCatastrophe.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.BLOOD_RH_CODE == searchCode);
                            if (search != null)
                            {
                                cbbBloodRh.EditValue = search.ID;
                                cbbBloodRh.Properties.Buttons[1].Visible = true;
                                txtCatastrophe.Focus();
                                txtCatastrophe.SelectAll();
                            }
                            else
                            {
                                cbbBloodRh.EditValue = null;
                                cbbBloodRh.Focus();
                                cbbBloodRh.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbBloodRh.EditValue = null;
                        cbbBloodRh.Focus();
                        cbbBloodRh.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadCatastrophe(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCatastrophe.Focus();
                    cboCatastrophe.ShowPopup();
                }
                else
                {
                    var data = datasPtttCatastrophe.Where(o => o.PTTT_CATASTROPHE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboCatastrophe.EditValue = data[0].ID;
                            cboCatastrophe.Properties.Buttons[1].Visible = true;
                            txtDeathSurg.Focus();
                            txtDeathSurg.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_CATASTROPHE_CODE == searchCode);
                            if (search != null)
                            {
                                cboCatastrophe.EditValue = search.ID;
                                cboCatastrophe.Properties.Buttons[1].Visible = true;
                                txtDeathSurg.Focus();
                                txtDeathSurg.SelectAll();
                            }
                            else
                            {
                                cboCatastrophe.EditValue = null;
                                cboCatastrophe.Focus();
                                cboCatastrophe.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboCatastrophe.EditValue = null;
                        cboCatastrophe.Focus();
                        cboCatastrophe.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadDeathSurg(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDeathSurg.EditValue = null;
                    cboDeathSurg.Focus();
                    cboDeathSurg.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().Where(o => o.DEATH_CAUSE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDeathSurg.EditValue = data[0].ID;
                            txtMachineCode.Focus();
                            txtMachineCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.DEATH_CAUSE_CODE == searchCode);
                            if (search != null)
                            {
                                cboDeathSurg.EditValue = search.ID;
                                txtMachineCode.Focus();
                                txtMachineCode.SelectAll();
                            }
                            else
                            {
                                cboDeathSurg.EditValue = null;
                                cboDeathSurg.Focus();
                                cboDeathSurg.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboDeathSurg.EditValue = null;
                        cboDeathSurg.Focus();
                        cboDeathSurg.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
