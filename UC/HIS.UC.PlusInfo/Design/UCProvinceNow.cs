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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using HIS.UC.PlusInfo.ShareMethod;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using HIS.UC.PlusInfo.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCProvinceNow : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateSetValueForUCPlusInfo dlgLoadDistrict;
        DelegateNextControl dlgFocusNextUserControl;
        DelegateReloadData dlgReloadData;

        #endregion

        #region Constructor - Load

        public UCProvinceNow()
            : base("UCPlusInfo", "UCProvinceNow")
        {
            try
            {
                InitializeComponent();

                this.txtProvinceNowCode.TabIndex = this.TabIndex;
                this.SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCProvince_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UCProvinceInit()
        {
            try
            {
                _shareMethod.InitComboCommon(this.cboProvinceNowName, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCProvinceNow
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCProvinceNow).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCProvinceNow.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProvinceNowName.Properties.NullText = Inventec.Common.Resource.Get.Value("UCProvinceNow.cboProvinceNowName.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvinceNow.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCProvinceNow.lciProvinceNow.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvinceNow.Text = Inventec.Common.Resource.Get.Value("UCProvinceNow.lciProvinceNow.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lciProvinceNow.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCProvinceNow", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Data

        internal UCPlusInfoADO GetValue()
        {
            UCPlusInfoADO dataGet = new UCPlusInfoADO();
            try
            {
                if (this.cboProvinceNowName.EditValue != null)
                {
                    dataGet.HT_PROVINCE_NAME = this.cboProvinceNowName.Text;
                    dataGet.HT_PROVINCE_CODE = this.txtProvinceNowCode.Text;
                }
                else
                {
                    dataGet.HT_PROVINCE_NAME = "";
                    dataGet.HT_PROVINCE_CODE = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                dataGet = null;
            }
            return dataGet;
        }

        internal void SetValue(UCPlusInfoADO dataSet)
        {
            try
            {
                if (!String.IsNullOrEmpty(dataSet.HT_PROVINCE_CODE))
                {
                    string proCode = this._shareMethod.GenerateProvinceCode(dataSet.HT_PROVINCE_CODE);
                    var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == proCode);
                    if (province != null)
                    {
                        this.txtProvinceNowCode.Text = province.PROVINCE_CODE;
                        this.cboProvinceNowName.EditValue = province.ID;
                    }
                }
                else if (!String.IsNullOrEmpty(dataSet.HT_PROVINCE_NAME))
                {
                    var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_NAME == dataSet.HT_PROVINCE_NAME);
                    if (province != null)
                    {
                        this.txtProvinceNowCode.Text = province.SEARCH_CODE;
                        this.cboProvinceNowName.EditValue = province.PROVINCE_CODE;
                    }
                }
                else
                {
                    this.txtProvinceNowCode.Text = "";
                    this.cboProvinceNowName.EditValue = null;
                    _shareMethod.InitComboCommon(this.cboProvinceNowName, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                }
                //this.txtProvinceNowCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValueFromUCAddress(object dataSet, bool isCallByUCAddress)
        {
            try
            {
                if (dataSet != null)
                {
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_DISTRICT)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT data = (SDA.EFMODEL.DataModels.V_SDA_DISTRICT)dataSet;
                        if (String.IsNullOrEmpty(data.PROVINCE_CODE) && String.IsNullOrEmpty(data.PROVINCE_NAME))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT pro = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == data.DISTRICT_CODE);
                            if (pro != null)
                            {
                                this.txtProvinceNowCode.Text = pro.PROVINCE_CODE;
                                this.cboProvinceNowName.EditValue = pro.PROVINCE_CODE;
                                if (this.dlgLoadDistrict != null)
                                    this.dlgLoadDistrict(data, isCallByUCAddress);
                            }
                        }
                        else
                        {
                            this.txtProvinceNowCode.Text = data.PROVINCE_CODE;
                            this.cboProvinceNowName.EditValue = data.PROVINCE_CODE;
                            if (this.dlgLoadDistrict != null)
                                this.dlgLoadDistrict(data, isCallByUCAddress);
                        }
                        //Gán lại datasource của combo commune

                    }
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_PROVINCE)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE data = (SDA.EFMODEL.DataModels.V_SDA_PROVINCE)dataSet;
                        this.txtProvinceNowCode.Text = data.PROVINCE_CODE;
                        this.cboProvinceNowName.EditValue = data.PROVINCE_CODE;
                        if (this.dlgLoadDistrict != null)
                            this.dlgLoadDistrict(data, isCallByUCAddress);
                        //Gán lại datasource của combo district && reset datasource của combo commune
                        //this.dlgLoadDistrict(data, false);
                    }
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_COMMUNE)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE data = (SDA.EFMODEL.DataModels.V_SDA_COMMUNE)dataSet;
                        var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == data.DISTRICT_CODE);
                        if (province != null)
                        {
                            this.txtProvinceNowCode.Text = province.PROVINCE_CODE;
                            this.cboProvinceNowName.EditValue = province.PROVINCE_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetDelegateLoadDistrictByProvince(DelegateSetValueForUCPlusInfo _dlgLoadDistrict)
        {
            try
            {
                if (_dlgLoadDistrict != null)
                    this.dlgLoadDistrict = _dlgLoadDistrict;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ReloadDataDistrictAndCommune(DelegateReloadData _dlgReloadData)
        {
            try
            {
                if (_dlgReloadData != null)
                    this.dlgReloadData = _dlgReloadData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/UCProvinceNow/ReloadDistrictAndCommune:\n" + ex);
            }
        }

        #endregion

        #region Event Control

        private void cboProvinceNowName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvinceNowName.EditValue != null
                        && this.cboProvinceNowName.EditValue != this.cboProvinceNowName.OldEditValue)
                    {
                        if (this.dlgReloadData != null)
                            this.dlgReloadData(false);

                        var pro = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == this.cboProvinceNowName.EditValue.ToString());
                        if (pro != null)
                        {
                            this.txtProvinceNowCode.Text = pro.PROVINCE_CODE;
                            this.dlgLoadDistrict(pro, false);
                        }
                    }
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceNowName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvinceNowName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>()
                            .SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvinceNowName.EditValue.ToString());
                        if (commune != null)
                            this.txtProvinceNowCode.Text = commune.PROVINCE_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Focus

        internal void FocusNextControl(DelegateNextControl _dlgFocusNextControl)
        {
            try
            {
                if (_dlgFocusNextControl != null)
                    this.dlgFocusNextUserControl = _dlgFocusNextControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void txtProvinceNowCode_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtProvinceNowCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strSearch = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(strSearch))
                    {
                        this.cboProvinceNowName.EditValue = null;
                        this.cboProvinceNowName.Focus();
                        this.cboProvinceNowName.ShowPopup();
                        //e.Handled = true;
                    }
                    else
                    {
                        List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                        try
                        {
                            int provinceCode = Convert.ToInt32(strSearch);
                            listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE == strSearch).ToList();
                        }
                        catch (Exception)
                        {
                            listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.SEARCH_CODE.Contains(strSearch)).ToList();
                        }
                        if (listResult.Count == 1)
                        {
                            bool isReLoadRef = false;
                            if (listResult[0].PROVINCE_CODE != (this.cboProvinceNowName.EditValue ?? "").ToString())
                            {
                                isReLoadRef = true;
                            }
                            if (isReLoadRef)
                            {
                                this.cboProvinceNowName.EditValue = listResult[0].PROVINCE_CODE;
                                this.txtProvinceNowCode.Text = listResult[0].PROVINCE_CODE;

                                if (this.dlgLoadDistrict != null)
                                    this.dlgLoadDistrict(listResult[0], true);

                                if (this.dlgReloadData != null)
                                    this.dlgReloadData(false);
                            }

                            if (this.dlgFocusNextUserControl != null)
                                this.dlgFocusNextUserControl(this.TabIndex, null);
                        }
                        else
                        {
                            //this.cboProvinceNowName.EditValue = null;
                            if (listResult != null && listResult.Count > 0)
                                this.cboProvinceNowName.Properties.DataSource = listResult;
                            this._shareMethod.FocusShowpopup(this.cboProvinceNowName, false);
                            //e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            DisposeControl();
        }
        internal void DisposeControl()
        {
            try
            {
                dlgReloadData = null;
                dlgFocusNextUserControl = null;
                dlgLoadDistrict = null;
                _shareMethod = null;
                this.cboProvinceNowName.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProvinceNowName_Closed);
                this.cboProvinceNowName.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboProvinceNowName_KeyUp);
                this.txtProvinceNowCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtProvinceNowCode_KeyDown);
                this.txtProvinceNowCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtProvinceNowCode_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCProvince_Load);
                lciProvinceNow = null;
                layoutControlItem1 = null;
                txtProvinceNowCode = null;
                cboProvinceNowName = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
