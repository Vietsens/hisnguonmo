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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceExecute.EkipTemp;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Message;
using MOS.SDO;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmClsInfo : Form
    {
        ADO.ServiceADO currentServiceADO;
        private HIS_SERE_SERV_EXT sereServExt;
        private V_HIS_SERVICE_REQ serviceReq;
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT sereServPTTT { get; set; }
        private List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
        Action<List<HisEkipUserADO>, V_HIS_SERE_SERV_PTTT, HIS_SERE_SERV_EXT> actSaveClick;
        internal List<HIS_EKIP_TEMP> ekipTemps { get; set; }
        internal List<HIS_ICD> dataIcds { get; set; }
        internal long autoCheckIcd;
        string _TextIcdName1 = "";
        string _TextIcdName2 = "";
        string _TextIcdName3 = "";
        public int positionHandle = -1;
        Inventec.Desktop.Common.Modules.Module Module;
        bool isAllowEditInfo;
        internal V_HIS_TREATMENT vhisTreatment { get; set; }

        public frmClsInfo(Inventec.Desktop.Common.Modules.Module moduleData, ADO.ServiceADO serviceADO, List<HisEkipUserADO> ekipUsers, Action<List<HisEkipUserADO>, V_HIS_SERE_SERV_PTTT, HIS_SERE_SERV_EXT> actsaveclick, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT sereservPTTT, HIS_SERE_SERV_EXT sereServExt, V_HIS_SERVICE_REQ serviceReq)
        {
            InitializeComponent();
            this.Module = moduleData;
            this.currentServiceADO = serviceADO;
            this.ekipUserAdos = ekipUsers;
            this.sereServPTTT = sereservPTTT;
            this.actSaveClick = actsaveclick;
            this.serviceReq = serviceReq;
            this.sereServExt = sereServExt;
            if (HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsRequiredPtttPriority)
            {
                lciHinhThucPTTT.AppearanceItemCaption.ForeColor = Color.Maroon;
            }
        }

        private void frmClsInfo_Load(object sender, EventArgs e)
        {
            if (this.currentServiceADO != null)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sereServPTTT), this.sereServPTTT) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentServiceADO), this.currentServiceADO));
                this.isAllowEditInfo = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.AppConfigKeys.MOS__HIS_SERVICE_REQ__ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT) == "1";
                this.LoadTreatment();
                this.GetSereServPtttBySereServId();
                this.SetEnableControl();
                this.ComboMethodICD();
                //this.ComboAcsUser(repositoryItemGridLookUpUsername);//Họ và tên
                this.ComboAcsUser();
                this.ComboExecuteRole(repositoryItemCboRole);//Vai trò
                this.SetIcdFromServiceReq(this.serviceReq);
                //this.ComboPTTTGroup();
                //this.ComboLoaiPT();
                //this.ComboEmotionlessMothod();
                this.ComboEkipTemp(cboEkipTemp);
                this.ProcessLoadEkip();
                this.SetDefaultCboPTTTGroupOnly();
                this.LoadSereServExt();
                this.SetDataControlBySereServPttt();

                this.SetButtonDeleteGridLookup();
                this.ValidateControl();
            }
        }

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = serviceReq.TREATMENT_ID;
                this.vhisTreatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableControl()
        {
            try
            {
                if (this.currentServiceADO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.1");
                    if ((this.currentServiceADO.IS_NO_EXECUTE != null || this.serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT) || this.vhisTreatment.IS_PAUSE == 1)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.2");
                        if (this.isAllowEditInfo)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.3");
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("SetEnableControl.4");

                            ReadOnlyICD(true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);

                            cboEkipTemp.ReadOnly = true;
                            txtIcdExtraCode.ReadOnly = true;
                            txtIcdText.ReadOnly = true;
                            txtPtttGroupCode.ReadOnly = true;
                            cbbPtttGroup.ReadOnly = true;
                            txtMethodCode.ReadOnly = true;
                            cboMethod.ReadOnly = true;
                            txtBlood.ReadOnly = true;
                            cbbBlood.ReadOnly = true;
                            txtEmotionlessMethod.ReadOnly = true;
                            cbbEmotionlessMethod.ReadOnly = true;
                            txtCondition.ReadOnly = true;
                            cboCondition.ReadOnly = true;
                            txtBloodRh.ReadOnly = true;
                            cbbBloodRh.ReadOnly = true;
                            txtCatastrophe.ReadOnly = true;
                            cboCatastrophe.ReadOnly = true;
                            txtDeathSurg.ReadOnly = true;
                            cboDeathSurg.ReadOnly = true;
                            //dtStart.ReadOnly = true;
                            //dtFinish.ReadOnly = true;
                            txtMANNER.ReadOnly = true;
                            //txtDescription.ReadOnly = true;
                            //txtConclude.ReadOnly = true;
                            //txtResultNote.ReadOnly = true;
                            cbbPtttGroup.Properties.Buttons[0].Enabled = false;
                            //dtStart.Properties.Buttons[0].Enabled = false;
                            //dtFinish.Properties.Buttons[0].Enabled = false;
                            cbbPtttGroup.Properties.Buttons[1].Enabled = false;
                            //dtStart.Properties.Buttons[1].Enabled = false;
                            //dtFinish.Properties.Buttons[1].Enabled = false;
                            gridViewEkip.OptionsBehavior.ReadOnly = true;
                            gridViewEkip.OptionsCustomization.AllowFilter = false;
                            gridViewEkip.OptionsCustomization.AllowSort = false;
                            gridViewEkip.OptionsBehavior.Editable = false;
                            btnAdd.ReadOnly = true;
                            btnDelete.ReadOnly = true;
                            gridViewEkip.RefreshData();

                            btnSave.Enabled = false;
                            btnSaveEkipTemp.Enabled = false;
                        }
                    }
                    else if (!String.IsNullOrWhiteSpace(HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.CheckPermisson) && HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.CheckPermisson.Contains("1"))
                    {
                        btnSave.Enabled = true;

                        txtPtttGroupCode.ReadOnly = false;
                        cbbPtttGroup.ReadOnly = false;
                        cbbPtttGroup.Properties.Buttons[0].Enabled = true;
                        cbbPtttGroup.Properties.Buttons[1].Enabled = true;

                        //#17292
                        bool isDoctor = false;
                        var _employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>().FirstOrDefault(p => p.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                        if (_employee != null && _employee.IS_DOCTOR == (short)1)
                        {
                            isDoctor = true;
                        }

                        if (isDoctor)
                        {
                            txtMANNER.ReadOnly = false;
                            //txtDescription.ReadOnly = false;
                            //txtConclude.ReadOnly = false;
                            //txtResultNote.ReadOnly = false;

                            txtMachineCode.ReadOnly = true;
                            cboMachine.ReadOnly = true;
                            txtLoaiPT.ReadOnly = true;
                            cboLoaiPT.ReadOnly = true;
                            txtBanMoCode.ReadOnly = true;
                            cboBanMo.ReadOnly = true;
                            txtPhuongPhap2.ReadOnly = true;
                            cboPhuongPhap2.ReadOnly = true;
                            cboPhuongPhapThucTe.ReadOnly = true;
                            txtPhuongPhapTT.ReadOnly = true;
                            txtKQVoCam.ReadOnly = true;
                            cboKQVoCam.ReadOnly = true;
                            txtMoKTCao.ReadOnly = true;
                            cboMoKTCao.ReadOnly = true;

                            txtIcdExtraCode.ReadOnly = true;
                            txtIcdText.ReadOnly = true;
                            txtMethodCode.ReadOnly = true;
                            cboMethod.ReadOnly = true;
                            txtBlood.ReadOnly = true;
                            cbbBlood.ReadOnly = true;
                            txtEmotionlessMethod.ReadOnly = true;
                            cbbEmotionlessMethod.ReadOnly = true;
                            txtCondition.ReadOnly = true;
                            cboCondition.ReadOnly = true;
                            txtBloodRh.ReadOnly = true;
                            cbbBloodRh.ReadOnly = true;
                            txtCatastrophe.ReadOnly = true;
                            cboCatastrophe.ReadOnly = true;
                            txtDeathSurg.ReadOnly = true;
                            cboDeathSurg.ReadOnly = true;
                            //dtStart.ReadOnly = true;
                            //dtFinish.ReadOnly = true;
                            cboEkipTemp.ReadOnly = true;
                            btnSaveEkipTemp.Enabled = false;
                            //dtStart.Properties.Buttons[0].Enabled = false;
                            //dtFinish.Properties.Buttons[0].Enabled = false;
                            //dtStart.Properties.Buttons[1].Enabled = false;
                            //dtFinish.Properties.Buttons[1].Enabled = false;
                            gridViewEkip.OptionsBehavior.ReadOnly = true;
                            gridViewEkip.OptionsCustomization.AllowFilter = false;
                            gridViewEkip.OptionsCustomization.AllowSort = false;
                            gridViewEkip.OptionsBehavior.Editable = false;
                            btnAdd.ReadOnly = true;
                            btnDelete.ReadOnly = true;
                            gridViewEkip.RefreshData();

                            ReadOnlyICD(true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                        }
                        else
                        {
                            txtMANNER.ReadOnly = true;
                            //txtDescription.ReadOnly = true;
                            //txtConclude.ReadOnly = true;
                            //txtResultNote.ReadOnly = true;

                            txtMachineCode.ReadOnly = false;
                            cboMachine.ReadOnly = false;
                            txtLoaiPT.ReadOnly = false;
                            cboLoaiPT.ReadOnly = false;
                            txtBanMoCode.ReadOnly = false;
                            cboBanMo.ReadOnly = false;
                            txtPhuongPhap2.ReadOnly = false;
                            cboPhuongPhap2.ReadOnly = false;
                            txtPhuongPhapTT.ReadOnly = false;
                            cboPhuongPhapThucTe.ReadOnly = false;
                            txtKQVoCam.ReadOnly = false;
                            cboKQVoCam.ReadOnly = false;
                            txtMoKTCao.ReadOnly = false;
                            cboMoKTCao.ReadOnly = false;

                            txtIcdExtraCode.ReadOnly = false;
                            txtIcdText.ReadOnly = false;
                            txtMethodCode.ReadOnly = false;
                            cboMethod.ReadOnly = false;
                            txtBlood.ReadOnly = false;
                            cbbBlood.ReadOnly = false;
                            txtEmotionlessMethod.ReadOnly = false;
                            cbbEmotionlessMethod.ReadOnly = false;
                            txtCondition.ReadOnly = false;
                            cboCondition.ReadOnly = false;
                            txtBloodRh.ReadOnly = false;
                            cbbBloodRh.ReadOnly = false;
                            txtCatastrophe.ReadOnly = false;
                            cboCatastrophe.ReadOnly = false;
                            txtDeathSurg.ReadOnly = false;
                            cboDeathSurg.ReadOnly = false;
                            //dtStart.ReadOnly = false;
                            //dtFinish.ReadOnly = false;
                            cboEkipTemp.ReadOnly = false;
                            btnSaveEkipTemp.Enabled = true;
                            //dtStart.Properties.Buttons[0].Enabled = true;
                            //dtFinish.Properties.Buttons[0].Enabled = true;
                            //dtStart.Properties.Buttons[1].Enabled = true;
                            //dtFinish.Properties.Buttons[1].Enabled = true;
                            gridViewEkip.OptionsBehavior.ReadOnly = false;
                            gridViewEkip.OptionsCustomization.AllowFilter = true;
                            gridViewEkip.OptionsCustomization.AllowSort = true;
                            gridViewEkip.OptionsBehavior.Editable = true;
                            btnAdd.ReadOnly = false;
                            btnDelete.ReadOnly = false;
                            gridViewEkip.RefreshData();

                            ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                            ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                            ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                        }
                    }
                    else
                    {
                        txtIcdExtraCode.ReadOnly = false;
                        txtIcdText.ReadOnly = false;
                        txtPtttGroupCode.ReadOnly = false;
                        cbbPtttGroup.ReadOnly = false;
                        txtMethodCode.ReadOnly = false;
                        cboMethod.ReadOnly = false;
                        txtBlood.ReadOnly = false;
                        cbbBlood.ReadOnly = false;
                        txtEmotionlessMethod.ReadOnly = false;
                        cbbEmotionlessMethod.ReadOnly = false;
                        txtCondition.ReadOnly = false;
                        cboCondition.ReadOnly = false;
                        txtBloodRh.ReadOnly = false;
                        cbbBloodRh.ReadOnly = false;
                        txtCatastrophe.ReadOnly = false;
                        cboCatastrophe.ReadOnly = false;
                        txtDeathSurg.ReadOnly = false;
                        cboDeathSurg.ReadOnly = false;
                        //dtStart.ReadOnly = false;
                        //dtFinish.ReadOnly = false;
                        txtMANNER.ReadOnly = false;
                        //txtDescription.ReadOnly = false;
                        //txtConclude.ReadOnly = false;
                        //txtResultNote.ReadOnly = false;
                        btnSave.Enabled = true;
                        cboEkipTemp.ReadOnly = false;
                        btnSaveEkipTemp.Enabled = true;

                        cbbPtttGroup.Properties.Buttons[0].Enabled = true;
                        //dtStart.Properties.Buttons[0].Enabled = true;
                        //dtFinish.Properties.Buttons[0].Enabled = true;
                        cbbPtttGroup.Properties.Buttons[1].Enabled = true;
                        //dtStart.Properties.Buttons[1].Enabled = true;
                        //dtFinish.Properties.Buttons[1].Enabled = true;
                        gridViewEkip.OptionsBehavior.ReadOnly = false;
                        gridViewEkip.OptionsCustomization.AllowFilter = true;
                        gridViewEkip.OptionsCustomization.AllowSort = true;
                        gridViewEkip.OptionsBehavior.Editable = true;
                        btnAdd.ReadOnly = false;
                        btnDelete.ReadOnly = false;
                        gridViewEkip.RefreshData();
                        ReadOnlyICD(false, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                        ReadOnlyICD(false, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                        ReadOnlyICD(false, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataControlBySereServPttt()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServPTTT), sereServPTTT) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentServiceADO), currentServiceADO) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServExt), sereServExt));

                //this.refreshControl();

                this.ComboMethodPTTT();
                this.ComboPTTTGroup();
                this.ComboEmotionlessMothod();
                this.ComboBlood();//Nhóm máu
                this.ComboBloodRh();//Nhóm máu RH
                this.ComboPtttCondition();//Tình hình Pttt
                this.ComboCatastrophe();//Tai biến trong PTTT
                this.ComboDeathWithin();//Tử vong trong PTTT
                this.ComboExecuteRole();//Vai trò

                this.ComboLoaiPT();
                this.ComboPhuongPhap2();
                this.ComboKQVoCam();
                this.ComboMoKTCao();
                this.ComboHisMachine();
                this.LoadComboPtttTable(cboBanMo);
                this.ComboPhuongPhapThucTe();

                if (this.sereServPTTT != null && this.sereServPTTT.ID > 0)
                {
                    FillDataToCboIcd(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, this.sereServPTTT.ICD_CODE, this.sereServPTTT.ICD_NAME);
                    FillDataToCboIcd(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, this.sereServPTTT.BEFORE_PTTT_ICD_CODE, this.sereServPTTT.BEFORE_PTTT_ICD_NAME);
                    FillDataToCboIcd(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, this.sereServPTTT.AFTER_PTTT_ICD_CODE, this.sereServPTTT.AFTER_PTTT_ICD_NAME);

                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_SUB_CODE))
                    {
                        this.txtIcdExtraCode.Text = this.sereServPTTT.ICD_SUB_CODE;
                    }
                    if (!string.IsNullOrEmpty(this.sereServPTTT.ICD_TEXT))
                    {
                        this.txtIcdText.Text = this.sereServPTTT.ICD_TEXT;
                    }

                    this.txtMANNER.Text = this.sereServPTTT.MANNER;
                }
                else
                {
                    if (this.currentServiceADO != null && !this.currentServiceADO.EKIP_ID.HasValue)
                    {
                        this.txtMANNER.Text = this.currentServiceADO.TDL_SERVICE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadPtttGroupCode(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbPtttGroup.Focus();
                    cbbPtttGroup.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.PTTT_GROUP_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbPtttGroup.EditValue = data[0].ID;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_GROUP_CODE == searchCode);
                            if (search != null)
                            {
                                cbbPtttGroup.EditValue = search.ID;
                                cbbPtttGroup.Properties.Buttons[1].Visible = true;
                                txtLoaiPT.Focus();
                                txtLoaiPT.SelectAll();
                            }
                            else
                            {
                                cbbPtttGroup.EditValue = null;
                                cbbPtttGroup.Focus();
                                cbbPtttGroup.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbPtttGroup.EditValue = null;
                        cbbPtttGroup.Focus();
                        cbbPtttGroup.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataEkipTemp()
        {
            try
            {
                ComboEkipTemp(cboEkipTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPtttGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cbbPtttGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbPtttGroup.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_GROUP data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtPtttGroupCode.Text = data.PTTT_GROUP_CODE;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbPtttGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbPtttGroup.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_GROUP data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPtttGroupCode.Text = data.PTTT_GROUP_CODE;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEkipTemp.EditValue != null)
                    {
                        var data = this.ekipTemps.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboEkipTemp.Properties.Buttons[1].Visible = true;
                            LoadGridEkipUserFromTemp(data.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmotionlessMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadEmotionlessMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtKQVoCam.Focus();
                            txtKQVoCam.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            txtKQVoCam.Focus();
                            txtKQVoCam.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ekipTempUsers), ekipTempUsers));

                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> glstExcuteRole = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        var role = glstExcuteRole.Where(ex => ex.ID == ekipTempUser.EXECUTE_ROLE_ID).FirstOrDefault();
                        if (role.IS_ACTIVE == 1)
                        {
                            ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                            ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                            ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;

                            if (ekipUserAdoTemps.Count == 0)
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }
                            else
                            {
                                ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }
                    gridControlEkip.DataSource = ekipUserAdoTemps;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbPtttGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbPtttGroup.Properties.Buttons[1].Visible = false;
                    cbbPtttGroup.EditValue = null;
                    txtPtttGroupCode.Text = "";
                    txtPtttGroupCode.Focus();
                    txtPtttGroupCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = false;
                    cbbEmotionlessMethod.EditValue = null;
                    txtEmotionlessMethod.Text = "";
                    txtEmotionlessMethod.Focus();
                    txtEmotionlessMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadLoaiPT(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboLoaiPT.Focus();
                    cboLoaiPT.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().Where(o => o.PTTT_PRIORITY_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboLoaiPT.EditValue = data[0].ID;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_PRIORITY_CODE == searchCode);
                            if (search != null)
                            {
                                cboLoaiPT.EditValue = search.ID;
                                cboLoaiPT.Properties.Buttons[1].Visible = true;
                                //txtMethodCode.Focus();
                                //txtMethodCode.SelectAll();
                            }
                            else
                            {
                                cboLoaiPT.EditValue = null;
                                cboLoaiPT.Focus();
                                cboLoaiPT.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboLoaiPT.EditValue = null;
                        cboLoaiPT.Focus();
                        cboLoaiPT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoaiPT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadLoaiPT(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLoaiPT.Properties.Buttons[1].Visible = false;
                    cboLoaiPT.EditValue = null;
                    txtLoaiPT.Text = "";
                    txtLoaiPT.Focus();
                    txtLoaiPT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoaiPT.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiPT.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiPT.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiPT.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        cboEkipTemp.Properties.Buttons[1].Visible = false;
                        cboEkipTemp.EditValue = null;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HisEkipUserADO participant = new HisEkipUserADO();
                participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                this.ekipUserAdos.Add(participant);
                gridControlEkip.BeginUpdate();
                gridControlEkip.DataSource = null;
                gridControlEkip.DataSource = ekipUserAdos;
                gridControlEkip.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var participant = (HisEkipUserADO)gridViewEkip.GetFocusedRow();
                if (participant != null)
                {
                    gridControlEkip.BeginUpdate();
                    if (ekipUserAdos.Count > 0)
                    {
                        this.ekipUserAdos.Remove(participant);
                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = ekipUserAdos;
                    }
                    else
                    {
                        var dataGrdControl = gridControlEkip.DataSource as List<HisEkipUserADO>;
                        dataGrdControl.Remove(participant);
                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = dataGrdControl;
                    }
                    gridControlEkip.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCboName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                gridViewEkip.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                gridViewEkip.FocusedColumn = gridViewEkip.VisibleColumns[2];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewEkip.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = btnAdd;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = btnDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "USERNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HIS.Desktop.ADO.HisEkipUserADO data = view.GetFocusedRow() as HIS.Desktop.ADO.HisEkipUserADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

                    List<string> loginNames = new List<string>();
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (data.LOGINNAME != null)
                            editor.EditValue = data.LOGINNAME;
                        MOS.Filter.HisExecuteRoleUserFilter filter = new MOS.Filter.HisExecuteRoleUserFilter();
                        filter.EXECUTE_ROLE_ID = data.EXECUTE_ROLE_ID;
                        List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER> executeRoleUsers = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());

                        if (executeRoleUsers != null && executeRoleUsers.Count > 0)
                        {
                            loginNames = executeRoleUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                        }
                    }
                    ComboAcsUser(editor, loginNames);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "USERNAME")
                {
                    gridViewEkip.ShowEditor();
                    ((GridLookUpEdit)gridViewEkip.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonDeleteGridLookup(GridLookUpEdit control)
        {
            try
            {
                if (control.EditValue != null)
                {
                    control.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    control.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonDeleteLookup(LookUpEdit control)
        {
            try
            {
                if (control.EditValue != null)
                {
                    control.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    control.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboExecuteRole(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                data = data.Where(dt => dt.IS_ACTIVE == 1).ToList(); ;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboAcsUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo_UseName, datas, controlEditorADO);


                repositoryItemSearchLookUpEdit1.DataSource = datas;
                repositoryItemSearchLookUpEdit1.DisplayMember = "USERNAME";
                repositoryItemSearchLookUpEdit1.ValueMember = "LOGINNAME";

                repositoryItemSearchLookUpEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                repositoryItemSearchLookUpEdit1.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                repositoryItemSearchLookUpEdit1.ImmediatePopup = true;
                repositoryItemSearchLookUpEdit1.View.Columns.Clear();

                GridColumn aColumnCode = repositoryItemSearchLookUpEdit1.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = repositoryItemSearchLookUpEdit1.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessLoadEkip()
        {
            try
            {
                if (this.ekipUserAdos != null && this.ekipUserAdos.Count > 0)
                {
                    gridControlEkip.BeginUpdate();
                    gridControlEkip.DataSource = null;
                    gridControlEkip.DataSource = this.ekipUserAdos;
                    gridControlEkip.EndUpdate();
                }
                else if (this.currentServiceADO != null && this.currentServiceADO.EKIP_ID.HasValue)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisEkipUserViewFilter hisEkipUserFilter = new MOS.Filter.HisEkipUserViewFilter();
                    hisEkipUserFilter.EKIP_ID = this.currentServiceADO.EKIP_ID;
                    var lst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>(ApiConsumer.HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisEkipUserFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (lst != null && lst.Count > 0)
                    {
                        this.ekipUserAdos = new List<HisEkipUserADO>();
                        foreach (var item in lst)
                        {
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>();
                            var HisEkipUserProcessing = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>(item);
                            if (item != lst[0])
                            {
                                HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            this.ekipUserAdos.Add(HisEkipUserProcessing);
                        }
                    }

                    gridControlEkip.BeginUpdate();
                    gridControlEkip.DataSource = null;
                    gridControlEkip.DataSource = this.ekipUserAdos;
                    gridControlEkip.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServPtttBySereServId()
        {
            try
            {
                if (this.sereServPTTT == null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServPtttViewFilter hisSereServPtttFilter = new HisSereServPtttViewFilter();
                    hisSereServPtttFilter.SERE_SERV_ID = this.currentServiceADO.ID;
                    var hisSereServPttts = new BackendAdapter(param)
                      .Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, hisSereServPtttFilter, param);
                    this.sereServPTTT = (hisSereServPttts != null && hisSereServPttts.Count > 0) ? hisSereServPttts.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultCboPTTTGroupOnly()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.1");
                if (this.currentServiceADO.EKIP_ID == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.2");
                    long ptttGroupId = 0;

                    var surgMisuService = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o =>
                        o.ID == this.currentServiceADO.SERVICE_ID &&
                        (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentServiceADO.SERVICE_ID), this.currentServiceADO.SERVICE_ID) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => surgMisuService), surgMisuService));
                    if (surgMisuService != null)
                    {
                        if (surgMisuService.PTTT_GROUP_ID.HasValue)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3");
                            HIS_PTTT_GROUP ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_GROUP_ID);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ptttGroup), ptttGroup));
                            ptttGroupId = ptttGroup.ID;
                            txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                        }
                    }

                    if (ptttGroupId > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3.1");
                        cbbPtttGroup.EditValue = ptttGroupId;
                        cbbPtttGroup.Enabled = false;
                        txtPtttGroupCode.Enabled = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3.2");
                        cbbPtttGroup.EditValue = null;
                        cbbPtttGroup.Enabled = true;
                        txtPtttGroupCode.Enabled = true;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.4");
                    var surgService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == this.currentServiceADO.SERVICE_ID);
                    if (surgService != null && surgService.PTTT_GROUP_ID != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.5");
                        HIS_PTTT_GROUP ptttGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgService.PTTT_GROUP_ID);
                        cbbPtttGroup.EditValue = ptttGroup.ID;
                        txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                        cbbPtttGroup.Properties.Buttons[1].Visible = true;
                        cbbPtttGroup.Enabled = false;
                        txtPtttGroupCode.Enabled = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Lay PTTT_GROUP_ID mac dinh theo dich vu khong co du lieu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => surgService), surgService));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                HisSurgServiceReqUpdateSDO hisSurgResultSDO = new MOS.SDO.HisSurgServiceReqUpdateSDO();
                bool valid = true;
                this.positionHandle = -1;
                valid = valid && dxValidationProvider1.Validate();
                //valid = valid && (this.isAllowEditInfo ? this.ValidStartDatePTTT(ref hisSurgResultSDO) : true);
                valid = valid && (this.currentServiceADO != null);
                if (valid)
                {
                    ProcessEkipUserForSave();
                    ProcessSereServPtttForSave();
                    ProcessSereServExt();
                    actSaveClick(this.ekipUserAdos, this.sereServPTTT, this.sereServExt);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSereServPtttForSave()
        {
            try
            {
                if (this.sereServPTTT == null)
                {
                    this.sereServPTTT = new MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT();
                    this.sereServPTTT.SERE_SERV_ID = currentServiceADO.ID;
                    this.sereServPTTT.TDL_TREATMENT_ID = currentServiceADO.TDL_TREATMENT_ID;
                }

                //Phuong phap vô cảm
                this.sereServPTTT.EMOTIONLESS_METHOD_ID = cbbEmotionlessMethod.EditValue != null ? (long?)cbbEmotionlessMethod.EditValue : null;

                //Loai PTTT
                this.sereServPTTT.PTTT_GROUP_ID = cbbPtttGroup.EditValue != null ? (long?)cbbPtttGroup.EditValue : null;

                this.sereServPTTT.PTTT_PRIORITY_ID = cboLoaiPT.EditValue != null ? (long?)cboLoaiPT.EditValue : null;

                if (txtIcd1.ErrorText == "")
                {
                    if (chkIcd1.Checked)
                        this.sereServPTTT.ICD_NAME = txtIcd1.Text;
                    else
                        this.sereServPTTT.ICD_NAME = cboIcd1.Text;

                    if (!String.IsNullOrEmpty(txtIcdCode1.Text))
                    {
                        this.sereServPTTT.ICD_CODE = txtIcdCode1.Text;
                    }
                }

                if (txtIcd2.ErrorText == "")
                {
                    if (chkIcd2.Checked)
                        this.sereServPTTT.BEFORE_PTTT_ICD_NAME = txtIcd2.Text;
                    else
                        this.sereServPTTT.BEFORE_PTTT_ICD_NAME = cboIcd2.Text;

                    if (!String.IsNullOrEmpty(txtIcdCode2.Text))
                    {
                        this.sereServPTTT.BEFORE_PTTT_ICD_CODE = txtIcdCode2.Text;
                    }
                }

                if (txtIcd3.ErrorText == "")
                {
                    if (chkIcd3.Checked)
                        this.sereServPTTT.AFTER_PTTT_ICD_NAME = txtIcd3.Text;
                    else
                        this.sereServPTTT.AFTER_PTTT_ICD_NAME = cboIcd3.Text;

                    if (!String.IsNullOrEmpty(txtIcdCode3.Text))
                    {
                        this.sereServPTTT.AFTER_PTTT_ICD_CODE = txtIcdCode3.Text;
                    }
                }

                //Chuan doan phu
                this.sereServPTTT.ICD_TEXT = txtIcdText.Text;
                this.sereServPTTT.ICD_SUB_CODE = txtIcdExtraCode.Text;

                // nhom mau
                if (cbbBlood.EditValue != null)
                {
                    this.sereServPTTT.BLOOD_ABO_ID = (long)cbbBlood.EditValue;
                }
                else
                {
                    this.sereServPTTT.BLOOD_ABO_ID = null;
                }
                //Nhom mau RH
                if (cbbBloodRh.EditValue != null)
                {
                    this.sereServPTTT.BLOOD_RH_ID = (long)cbbBloodRh.EditValue;
                }
                else
                {
                    this.sereServPTTT.BLOOD_RH_ID = null;
                }

                //Tai bien PTTT
                if (cboCatastrophe.EditValue != null)
                {
                    this.sereServPTTT.PTTT_CATASTROPHE_ID = (long)cboCatastrophe.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_CATASTROPHE_ID = null;
                }
                //Tinh hinh PTTT
                if (cboCondition.EditValue != null)
                {
                    this.sereServPTTT.PTTT_CONDITION_ID = (long)cboCondition.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_CONDITION_ID = null;
                }

                //Phuong phap PTTT
                if (cboMethod.EditValue != null)
                {
                    this.sereServPTTT.PTTT_METHOD_ID = (long)cboMethod.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_METHOD_ID = null;
                }
                //Phuong phap Thuc te
                if (cboPhuongPhapThucTe.EditValue != null)
                {
                    this.sereServPTTT.REAL_PTTT_METHOD_ID = (long)cboPhuongPhapThucTe.EditValue;
                }
                else
                {
                    this.sereServPTTT.REAL_PTTT_METHOD_ID = null;
                }

                if (!String.IsNullOrEmpty(txtMANNER.Text))
                {
                    this.sereServPTTT.MANNER = txtMANNER.Text;
                }

                //Tu vong
                if (cboDeathSurg.EditValue != null)
                {
                    this.sereServPTTT.DEATH_WITHIN_ID = (long)cboDeathSurg.EditValue;
                }
                else
                {
                    this.sereServPTTT.DEATH_WITHIN_ID = null;
                }
                if (cboLoaiPT.EditValue != null)
                {
                    this.sereServPTTT.PTTT_PRIORITY_ID = (long)cboLoaiPT.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_PRIORITY_ID = null;
                }
                if (cboPhuongPhap2.EditValue != null)
                {
                    this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID = (long)cboPhuongPhap2.EditValue;
                }
                else
                {
                    this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID = null;
                }
                if (cboKQVoCam.EditValue != null)
                {
                    this.sereServPTTT.EMOTIONLESS_RESULT_ID = (long)cboKQVoCam.EditValue;
                }
                else
                {
                    this.sereServPTTT.EMOTIONLESS_RESULT_ID = null;
                }
                if (cboMoKTCao.EditValue != null)
                {
                    this.sereServPTTT.PTTT_HIGH_TECH_ID = (long)cboMoKTCao.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_HIGH_TECH_ID = null;
                }
                if (cboBanMo.EditValue != null)
                {
                    this.sereServPTTT.PTTT_TABLE_ID = (long)cboBanMo.EditValue;
                }
                else
                {
                    this.sereServPTTT.PTTT_TABLE_ID = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSereServExt()
        {
            try
            {
                if (this.currentServiceADO != null)
                {
                    if (this.sereServExt == null)
                    {
                        this.sereServExt = new HIS_SERE_SERV_EXT();
                    }

                    sereServExt.SERE_SERV_ID = this.currentServiceADO.ID;
                    //if (dtStart.EditValue != null)
                    //    sereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtStart.DateTime);
                    //else
                    //    sereServExt.BEGIN_TIME = null;
                    //if (dtFinish.EditValue != null)
                    //    sereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinish.DateTime);
                    //else
                    //{
                    //    sereServExt.END_TIME = null;
                    //}
                    if (cboMachine.EditValue != null)
                    {
                        sereServExt.MACHINE_ID = (long)cboMachine.EditValue;
                        sereServExt.MACHINE_CODE = this.txtMachineCode.Text;
                    }
                    else
                    {
                        sereServExt.MACHINE_ID = null;
                        sereServExt.MACHINE_CODE = "";
                    }
                    sereServExt.INSTRUCTION_NOTE = txtIntructionNote.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessEkipUserForSave()
        {
            try
            {
                this.ekipUserAdos = gridControlEkip.DataSource as List<HisEkipUserADO>;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveEkipTemp_Click(object sender, EventArgs e)
        {
            try
            {
                var ekipUsers = gridControlEkip.DataSource as List<HisEkipUserADO>;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0 || o.EXECUTE_ROLE_ID == null).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show("Không có thông tin kip thực hiện");
                    return;
                }

                frmEkipTemp frm = new frmEkipTemp(ekipUsers, RefeshDataEkipTemp);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

        #region --- Event ICD ---

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");
                    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtIcdExtraCode.Text, txtIcdText.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.Module.RoomId, this.Module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {

            try
            {
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {

                    txtIcdExtraCode.Text = delegateIcdCodes;

                }


                if (!string.IsNullOrEmpty(delegateIcdNames))
                {

                    txtIcdText.Text = delegateIcdNames;

                }

            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }

        }

        void update(HIS_ICD dataIcd)
        {
            txtIcdText.Text = txtIcdText.Text + dataIcd.ICD_CODE + " - " + dataIcd.ICD_NAME + ", ";
        }

        void stringIcds(string delegateIcds)
        {
            if (!string.IsNullOrEmpty(delegateIcds))
            {
                txtIcdText.Text = delegateIcds;
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCode2.Focus();
                    txtIcdCode2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdExtraCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtIcdExtraCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(ResourceMessage.KhongTimThayICDTuongUng, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtIcdExtraCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtIcdExtraCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtIcdExtraCode.Focus();
                            txtIcdExtraCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtIcdExtraCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCode2.Focus();
                    txtIcdCode2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdExtraCode.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
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


        private void txtIcdCode1_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode2_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode3_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode1.Text.ToUpper(), txtIcdCode1, txtIcd1, cboIcd1, chkIcd1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode2.Text.ToUpper(), txtIcdCode2, txtIcd2, cboIcd2, chkIcd2);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode3.Text.ToUpper(), txtIcdCode3, txtIcd3, cboIcd3, chkIcd3);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode1_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode1.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode1);
                        ValidationICD(10, 500, true, txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, lciIcd1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode2_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode2.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode2);
                        ValidationICD(10, 500, true, txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, lciIcd2);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode3_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode3.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode1);
                        ValidationICD(10, 500, true, txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, lciIcd3);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcd1.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName1 = "";
                    cboIcd1.EditValue = null;
                    txtIcdCode1.Text = "";
                    txtIcd1.Text = "";
                    cboIcd1.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcd2.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName2 = "";
                    cboIcd2.EditValue = null;
                    txtIcdCode2.Text = "";
                    txtIcd2.Text = "";
                    cboIcd2.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcd3.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName3 = "";
                    cboIcd3.EditValue = null;
                    txtIcdCode3.Text = "";
                    txtIcd3.Text = "";
                    cboIcd3.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcd1.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, txtIcdExtraCode);
                    else
                    {
                        txtIcdExtraCode.Focus();
                        txtIcdExtraCode.SelectAll();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcd2.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, txtIcdCode3);
                    else
                    {
                        txtIcdCode3.Focus();
                        txtIcdCode3.SelectAll();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcd3.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, txtPtttGroupCode);
                    else
                    {
                        txtPtttGroupCode.Focus();
                        txtPtttGroupCode.SelectAll();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcd1.ClosePopup();
                    cboIcd1.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcd1.ClosePopup();
                    if (cboIcd1.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode1, txtIcd1, cboIcd1, chkIcd1, txtIcdExtraCode);
                }
                else
                    cboIcd1.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcd2.ClosePopup();
                    cboIcd2.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcd2.ClosePopup();
                    if (cboIcd2.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode2, txtIcd2, cboIcd2, chkIcd2, txtIcdCode3);
                }
                else
                    cboIcd2.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcd3.ClosePopup();
                    cboIcd3.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcd3.ClosePopup();
                    if (cboIcd3.EditValue != null)
                        this.ChangecboChanDoanTD(txtIcdCode3, txtIcd3, cboIcd3, chkIcd3, txtPtttGroupCode);
                }
                else
                    cboIcd3.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcd1.Text))
                {
                    cboIcd1.EditValue = null;
                    txtIcd1.Text = "";
                    chkIcd1.Checked = false;
                }
                else
                {
                    //this._TextIcdName1 = cboIcd1.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcd2.Text))
                {
                    cboIcd2.EditValue = null;
                    txtIcd2.Text = "";
                    chkIcd2.Checked = false;
                }
                else
                {
                    //this._TextIcdName2 = cboIcd2.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcd3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcd3.Text))
                {
                    cboIcd3.EditValue = null;
                    txtIcd3.Text = "";
                    chkIcd3.Checked = false;
                }
                else
                {
                    //this._TextIcdName3 = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcd1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcd1.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcd2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcd2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcd3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkIcd3.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcd1.Checked == true)
                {
                    cboIcd1.Visible = false;
                    txtIcd1.Visible = true;
                    txtIcd1.Text = cboIcd1.Text;
                    txtIcd1.Focus();
                    txtIcd1.SelectAll();
                }
                else if (chkIcd1.Checked == false)
                {
                    txtIcd1.Visible = false;
                    cboIcd1.Visible = true;
                    txtIcd1.Text = cboIcd1.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcd2.Checked == true)
                {
                    cboIcd2.Visible = false;
                    txtIcd2.Visible = true;
                    txtIcd2.Text = cboIcd2.Text;
                    txtIcd2.Focus();
                    txtIcd2.SelectAll();
                }
                else if (chkIcd2.Checked == false)
                {
                    txtIcd2.Visible = false;
                    cboIcd2.Visible = true;
                    txtIcd2.Text = cboIcd2.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIcd3.Checked == true)
                {
                    cboIcd3.Visible = false;
                    txtIcd3.Visible = true;
                    txtIcd3.Text = cboIcd3.Text;
                    txtIcd3.Focus();
                    txtIcd3.SelectAll();
                }
                else if (chkIcd3.Checked == false)
                {
                    txtIcd3.Visible = false;
                    cboIcd3.Visible = true;
                    txtIcd3.Text = cboIcd3.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdExtraCode.Focus();
                    txtIcdExtraCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCode3.Focus();
                    txtIcdCode3.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIcd3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPtttGroupCode.Focus();
                    txtPtttGroupCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtMethodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMethodCode.Text = data.PTTT_METHOD_CODE;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                    }
                }
                else
                {
                    cboMethod.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMethod.EditValue.ToString()));
                        {
                            txtMethodCode.Text = data.PTTT_METHOD_CODE;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtPhuongPhap2.Focus();
                            txtPhuongPhap2.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBlood_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadBlood(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBlood_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbBlood.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_ABO data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBlood.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBlood.Text = data.BLOOD_ABO_CODE;
                            cbbBlood.Properties.Buttons[1].Visible = true;
                            txtBloodRh.Focus();
                            txtBloodRh.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBlood_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbBlood.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_ABO data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBlood.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBlood.Text = data.BLOOD_ABO_CODE;
                            cbbBlood.Properties.Buttons[1].Visible = true;
                            txtBloodRh.Focus();
                            txtBloodRh.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCondition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadCondition(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCondition_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCondition.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCondition.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCondition.Text = data.PTTT_CONDITION_CODE;
                            cboCondition.Properties.Buttons[1].Visible = true;
                            txtBlood.Focus();
                            txtBlood.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCondition_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCondition.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_CONDITION data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCondition.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCondition.Text = data.PTTT_CONDITION_CODE;
                            cboCondition.Properties.Buttons[1].Visible = true;
                            txtBlood.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBloodRh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadBloodRh(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBloodRh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbBloodRh.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_RH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBloodRh.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBloodRh.Text = data.BLOOD_RH_CODE;
                            cbbBloodRh.Properties.Buttons[1].Visible = true;
                            txtCatastrophe.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBloodRh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbBloodRh.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_RH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbBloodRh.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBloodRh.Text = data.BLOOD_RH_CODE;
                            cbbBloodRh.Properties.Buttons[1].Visible = true;
                            txtCatastrophe.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCatastrophe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadCatastrophe(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCatastrophe_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCatastrophe.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCatastrophe.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCatastrophe.Text = data.PTTT_CATASTROPHE_CODE;
                            cboCatastrophe.Properties.Buttons[1].Visible = true;
                            txtDeathSurg.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCatastrophe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCatastrophe.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_CATASTROPHE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCatastrophe.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtCatastrophe.Text = data.PTTT_CATASTROPHE_CODE;
                            cboCatastrophe.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDeathSurg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadDeathSurg(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathSurg_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDeathSurg.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDeathSurg.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtDeathSurg.Text = data.DEATH_CAUSE_CODE;
                            cboDeathSurg.Properties.Buttons[1].Visible = true;
                            txtMachineCode.Focus();
                            txtMachineCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathSurg_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDeathSurg.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDeathSurg.EditValue ?? 0).ToString()));
                        {
                            txtDeathSurg.Text = data.DEATH_CAUSE_CODE;
                            cboDeathSurg.Properties.Buttons[1].Visible = true;
                            txtMachineCode.Focus();
                            txtMachineCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        //private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            //txtConclude.Focus();
        //            //txtConclude.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtConclude_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtResultNote.Focus();
        //            //txtResultNote.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtResultNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            btnSave.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboPosition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LookUpEdit edit = sender as LookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        //grdViewInformationSurg.SetRowCellValue(grdViewInformationSurg.FocusedRowHandle, gridColumnParticipants_Id, edit.EditValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMANNER_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIntructionNote.Enabled)
                        txtIntructionNote.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void dtFinish_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            dtFinish.Properties.Buttons[1].Visible = true;
        //            txtMANNER.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            dtFinish.Focus();
        //            dtFinish.SelectAll();
        //            dtFinish.ShowPopup();
        //            dtStart.Properties.Buttons[1].Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}

        private void cboMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMethod.Properties.Buttons[1].Visible = false;
                    cboMethod.EditValue = null;
                    txtMethodCode.Text = "";
                    txtMethodCode.Focus();
                    txtMethodCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBlood_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbBlood.Properties.Buttons[1].Visible = false;
                    cbbBlood.EditValue = null;
                    txtBlood.Text = "";
                    txtBlood.Focus();
                    txtBlood.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCondition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCondition.Properties.Buttons[1].Visible = false;
                    cboCondition.EditValue = null;
                    txtCondition.Text = "";
                    txtCondition.Focus();
                    txtCondition.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbBloodRh_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbBloodRh.Properties.Buttons[1].Visible = false;
                    cbbBloodRh.EditValue = null;
                    txtBloodRh.Text = "";
                    txtBloodRh.Focus();
                    txtBloodRh.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCatastrophe_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCatastrophe.Properties.Buttons[1].Visible = false;
                    cboCatastrophe.EditValue = null;
                    txtCatastrophe.Text = "";
                    txtCatastrophe.Focus();
                    txtCatastrophe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathSurg_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDeathSurg.Properties.Buttons[1].Visible = false;
                    cboDeathSurg.EditValue = null;
                    txtDeathSurg.Text = "";
                    txtDeathSurg.Focus();
                    txtDeathSurg.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void dtStart_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            dtStart.Properties.Buttons[1].Visible = false;
        //            dtStart.EditValue = null;
        //            dtStart.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtFinish_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            dtFinish.Properties.Buttons[1].Visible = false;
        //            dtFinish.EditValue = null;
        //            dtFinish.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtFinish_Closed(object sender, ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (dtFinish.EditValue != null)
        //        {
        //            DateTime dt = dtFinish.DateTime;
        //            dtFinish.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        //            dtFinish.Properties.Buttons[1].Visible = true;
        //            txtMANNER.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void dtStart_Closed(object sender, ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (dtStart.EditValue != null)
        //        {
        //            DateTime dt = dtStart.DateTime;
        //            dtStart.DateTime = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        //            dtStart.Properties.Buttons[1].Visible = true;
        //            dtFinish.Focus();
        //            dtFinish.SelectAll();
        //            dtFinish.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        

        private void txtPhuongPhap2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadPhuongPhap2(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPhuongPhap2(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhuongPhap2.Focus();
                    cboPhuongPhap2.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_SECOND == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhuongPhap2.EditValue = data[0].ID;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtPhuongPhapTT.Focus();
                            txtPhuongPhapTT.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhuongPhap2.EditValue = search.ID;
                                cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                                txtPhuongPhapTT.Focus();
                                txtPhuongPhapTT.SelectAll();
                            }
                            else
                            {
                                cboPhuongPhap2.EditValue = null;
                                cboPhuongPhap2.Focus();
                                cboPhuongPhap2.ShowPopup();
                            }
                        }
                    }
                    else
                    {
                        cboPhuongPhap2.EditValue = null;
                        cboPhuongPhap2.Focus();
                        cboPhuongPhap2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhuongPhap2.Properties.Buttons[1].Visible = false;
                    cboPhuongPhap2.EditValue = null;
                    txtPhuongPhap2.Text = "";
                    txtPhuongPhap2.Focus();
                    txtPhuongPhap2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhuongPhap2.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPhuongPhap2.EditValue.ToString()));
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtPhuongPhapTT.Focus();
                            txtPhuongPhapTT.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhap2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhuongPhap2.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhuongPhap2.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboPhuongPhap2.Properties.Buttons[1].Visible = true;
                            txtPhuongPhapTT.Focus();
                            txtPhuongPhapTT.SelectAll();
                        }
                    }
                }
                else
                {
                    cboPhuongPhap2.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKQVoCam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadKQVoCam(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKQVoCam(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboKQVoCam.Focus();
                    cboKQVoCam.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.EMOTIONLESS_RESULT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboKQVoCam.EditValue = data[0].ID;
                            cboKQVoCam.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_RESULT_CODE == searchCode);
                            if (search != null)
                            {
                                cboKQVoCam.EditValue = search.ID;
                                cboKQVoCam.Properties.Buttons[1].Visible = true;
                                txtCondition.Focus();
                                txtCondition.SelectAll();
                            }
                            else
                            {
                                cboKQVoCam.EditValue = null;
                                cboKQVoCam.Focus();
                                cboKQVoCam.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboKQVoCam.EditValue = null;
                        cboKQVoCam.Focus();
                        cboKQVoCam.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKQVoCam_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboKQVoCam.Properties.Buttons[1].Visible = false;
                    cboKQVoCam.EditValue = null;
                    txtKQVoCam.Text = "";
                    txtKQVoCam.Focus();
                    txtKQVoCam.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKQVoCam_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboKQVoCam.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_RESULT data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboKQVoCam.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                            cboKQVoCam.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKQVoCam_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboKQVoCam.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_RESULT data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboKQVoCam.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                            cboKQVoCam.Properties.Buttons[1].Visible = true;
                            txtCondition.Focus();
                            txtCondition.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMoKTCao_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadMoKTCao(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMoKTCao(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMoKTCao.Focus();
                    cboMoKTCao.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.PTTT_HIGH_TECH_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMoKTCao.EditValue = data[0].ID;
                            cboMoKTCao.Properties.Buttons[1].Visible = true;
                            txtMANNER.Focus();
                            txtMANNER.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_HIGH_TECH_CODE == searchCode);
                            if (search != null)
                            {
                                cboMoKTCao.EditValue = search.ID;
                                cboMoKTCao.Properties.Buttons[1].Visible = true;
                                txtMANNER.Focus();
                                txtMANNER.SelectAll();
                            }
                            else
                            {
                                cboMoKTCao.EditValue = null;
                                cboMoKTCao.Focus();
                                cboMoKTCao.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMoKTCao.EditValue = null;
                        cboMoKTCao.Focus();
                        cboMoKTCao.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMoKTCao_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMoKTCao.Properties.Buttons[1].Visible = false;
                    cboMoKTCao.EditValue = null;
                    txtMoKTCao.Text = "";
                    txtMoKTCao.Focus();
                    txtMoKTCao.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMoKTCao_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMoKTCao.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_HIGH_TECH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMoKTCao.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                            cboMoKTCao.Properties.Buttons[1].Visible = true;
                            txtMANNER.Focus();
                            txtMANNER.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMoKTCao_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMoKTCao.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_HIGH_TECH data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMoKTCao.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                            cboMoKTCao.Properties.Buttons[1].Visible = true;
                            txtMANNER.Focus();
                            txtMANNER.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtMachineCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadMachine(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMachine(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMachine.Focus();
                    cboMachine.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.MACHINE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboMachine.EditValue = data[0].ID;
                            cboMachine.Properties.Buttons[1].Visible = true;
                            txtMoKTCao.Focus();
                            txtMoKTCao.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MACHINE_CODE == searchCode);
                            if (search != null)
                            {
                                cboMachine.EditValue = search.ID;
                                cboMachine.Properties.Buttons[1].Visible = true;
                                txtMoKTCao.Focus();
                                txtMoKTCao.SelectAll();
                            }
                            else
                            {
                                cboMachine.EditValue = null;
                                cboMachine.Focus();
                                cboMachine.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboMachine.EditValue = null;
                        cboMachine.Focus();
                        cboMachine.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMachine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMachine.Properties.Buttons[1].Visible = false;
                    cboMachine.EditValue = null;
                    txtMachineCode.Text = "";
                    txtMachineCode.Focus();
                    txtMachineCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMachine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMachine.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMachine.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMachineCode.Text = data.MACHINE_CODE;
                            cboMachine.Properties.Buttons[1].Visible = true;
                            txtMoKTCao.Focus();
                            txtMoKTCao.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMachine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMachine.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMachine.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMachineCode.Text = data.MACHINE_CODE;
                            cboMachine.Properties.Buttons[1].Visible = true;
                            txtMoKTCao.Focus();
                            txtMoKTCao.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBanMoCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadBanMo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBanMo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboBanMo.Focus();
                    cboBanMo.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().Where(o =>
                        o.IS_ACTIVE == 1
                        && o.PTTT_TABLE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            txtBanMoCode.Text = data[0].PTTT_TABLE_CODE;
                            cboBanMo.EditValue = data[0].ID;
                            cboBanMo.Properties.Buttons[1].Visible = true;
                            txtMethodCode.Focus();
                            txtMethodCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_TABLE_CODE == searchCode);
                            if (search != null)
                            {
                                cboBanMo.EditValue = search.ID;
                                cboBanMo.Properties.Buttons[1].Visible = true;
                                txtMethodCode.Focus();
                                txtMethodCode.SelectAll();
                            }
                            else
                            {
                                cboBanMo.EditValue = null;
                                cboBanMo.Focus();
                                cboBanMo.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboBanMo.EditValue = null;
                        cboBanMo.Focus();
                        cboBanMo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBanMo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBanMo.Properties.Buttons[1].Visible = false;
                    cboBanMo.EditValue = null;
                    txtBanMoCode.Text = "";
                    txtBanMoCode.Focus();
                    txtBanMoCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBanMo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBanMo.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_TABLE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBanMo.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                            cboBanMo.Properties.Buttons[1].Visible = true;
                            txtMethodCode.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBanMo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBanMo.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_TABLE data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBanMo.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                            cboBanMo.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtPhuongPhapTT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadPhuongPhapThucTe(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadPhuongPhapThucTe(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboPhuongPhapThucTe.Focus();
                    cboPhuongPhapThucTe.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(o => o.PTTT_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboPhuongPhapThucTe.EditValue = data[0].ID;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cboPhuongPhapThucTe.EditValue = search.ID;
                                cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                                txtEmotionlessMethod.Focus();
                                txtEmotionlessMethod.SelectAll();
                            }
                            else
                            {
                                cboPhuongPhapThucTe.EditValue = null;
                                cboPhuongPhapThucTe.Focus();
                                cboPhuongPhapThucTe.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboPhuongPhapThucTe.EditValue = null;
                        cboPhuongPhapThucTe.Focus();
                        cboPhuongPhapThucTe.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhapThucTe_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhuongPhapThucTe.Properties.Buttons[1].Visible = false;
                    cboPhuongPhapThucTe.EditValue = null;
                    txtPhuongPhapTT.Text = "";
                    txtPhuongPhapTT.Focus();
                    txtPhuongPhapTT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhapThucTe_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhuongPhapThucTe.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPhuongPhapThucTe.EditValue.ToString()));
                        {
                            txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongPhapThucTe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhuongPhapThucTe.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPhuongPhapThucTe.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                            cboPhuongPhapThucTe.Properties.Buttons[1].Visible = true;
                            txtEmotionlessMethod.Focus();
                            txtEmotionlessMethod.SelectAll();
                        }
                    }
                }
                else
                {
                    cboPhuongPhapThucTe.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
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
