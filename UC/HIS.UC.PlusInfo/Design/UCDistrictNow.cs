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
using HIS.UC.PlusInfo.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCDistrictNow : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;
        DelegateSetValueForUCPlusInfo dlgLoadCommune;

        #endregion

        #region Constructor - Load

        public UCDistrictNow()
            : base("UCPlusInfo", "UCDistrictNow")
        {
            try
            {
                InitializeComponent(); 
                
                //this.SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                this.txtDistrictNowCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCDistrictNow_Load(object sender, EventArgs e)
        {
            try
            {
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
                this.lciDistrictNow.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCDistrictNow", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCDistrictNow
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCDistrictNow).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDistrictNow.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDistrictNowName.Properties.NullText = Inventec.Common.Resource.Get.Value("UCDistrictNow.cboDistrictNowName.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDistrictNow.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCDistrictNow.lciDistrictNow.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDistrictNow.Text = Inventec.Common.Resource.Get.Value("UCDistrictNow.lciDistrictNow.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        #region Data

        internal void SetValue(UCPlusInfoADO dataSet)
        {
            try
            {
                if (dataSet.HT_PROVINCE_NAME != null && dataSet.HT_PROVINCE_NAME.Count() > 0)
                {
                    var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_NAME == dataSet.HT_PROVINCE_NAME);
                    this.LoadDistrict("", province.PROVINCE_CODE);
                }
                var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == dataSet.HT_DISTRICT_CODE || o.DISTRICT_NAME == dataSet.HT_DISTRICT_NAME);
                if (district != null)
                {
                    this.txtDistrictNowCode.Text = district.SEARCH_CODE;
                    this.cboDistrictNowName.EditValue = district.DISTRICT_CODE;
                }
                else
                {
                    this.txtDistrictNowCode.Text = "";
                    this.cboDistrictNowName.EditValue = null;
                }
                //this.txtDistrictNowCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal UCPlusInfoADO GetValue()
        {
            UCPlusInfoADO dataGet = new UCPlusInfoADO();
            try
            {
                dataGet.HT_DISTRICT_NAME = this.cboDistrictNowName.Text;
                dataGet.HT_DISTRICT_CODE = this.txtDistrictNowCode.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataGet;
        }

        internal void SetValueFromUCAddress(object dataSet, bool isCallByUCAddress)
        {
            try
            {
                //this._shareMethod.InitComboCommon(this.cboDistrictNowName, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>(), "DISTRICT_CODE", "DISTRICT_NAME", "SEARCH_CODE");
                if (dataSet != null)
                {
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_DISTRICT)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT data = (SDA.EFMODEL.DataModels.V_SDA_DISTRICT)dataSet;
                        if (isCallByUCAddress == true)
                        {
                            this.txtDistrictNowCode.Text = data.DISTRICT_CODE;
                            this.cboDistrictNowName.EditValue = data.DISTRICT_CODE;
                            if (this.dlgLoadCommune != null)
                                this.dlgLoadCommune(data, isCallByUCAddress);
                        }
                        else
                            this.LoadDistrict("", data.PROVINCE_CODE);
                    }
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_PROVINCE)
                    {
                        //if (isCallByUCAddress == false)
                        //{
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE data = (SDA.EFMODEL.DataModels.V_SDA_PROVINCE)dataSet;
                        this.LoadDistrict("", data.PROVINCE_CODE);
                        cboDistrictNowName.EditValue = null;
                        txtDistrictNowCode.Text = "";
                        if (dlgLoadCommune != null)
                            dlgLoadCommune(null, false);
                        //}
                    }
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_COMMUNE)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE data = (SDA.EFMODEL.DataModels.V_SDA_COMMUNE)dataSet;
                        this.LoadDistrict("", data.DISTRICT_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetDelegateLoadCommuneByDistrict(DelegateSetValueForUCPlusInfo _dlgLoadCommune)
        {
            try
            {
                if (_dlgLoadCommune != null)
                    this.dlgLoadCommune = _dlgLoadCommune;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDistrict(string searchCode, string provinceCode)
        {
            try
            {

                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                if (!String.IsNullOrEmpty(searchCode) || !String.IsNullOrEmpty(provinceCode))
                {
                    try
                    {
                        int districtCode = Convert.ToInt32(searchCode);
                        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE == searchCode).ToList();
                    }
                    catch (Exception)
                    {
                        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();
                    }
                }
                //this._shareMethod.InitComboCommon(this.cboDistrictNowName, listResult, "DISTRICT_CODE", "DISTRICT_NAME", "SEARCH_CODE"); // Load district to combo
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboDistrictNowName.Properties.DataSource = null;
                    //this._shareMethod.InitComboCommon(this.cboDistrictNowName, listResult, "DISTRICT_CODE", "DISTRICT_NAME", "SEARCH_CODE");
                    this.cboDistrictNowName.Properties.DataSource = listResult;
                    this.cboDistrictNowName.EditValue = null;
                    this.txtDistrictNowCode.Text = "";
                    this._shareMethod.FocusShowpopup(this.cboDistrictNowName, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboDistrictNowName.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictNowCode.Text = listResult[0].DISTRICT_CODE;
                        if (this.dlgLoadCommune != null)
                            this.dlgLoadCommune(listResult[0], false);
                    }
                    else
                    {
                        this.cboDistrictNowName.EditValue = null;
                        this.cboDistrictNowName.Properties.DataSource = null;
                        if (listResult.Count > 0)
                            this._shareMethod.InitComboCommon(this.cboDistrictNowName, listResult, "DISTRICT_CODE", "DISTRICT_NAME", "SEARCH_CODE");
                        //this.cboDistrictNowName.Properties.DataSource = listResult;
                        this.txtDistrictNowCode.Text = "";
                        if (String.IsNullOrEmpty(provinceCode))
                        {
                            this.cboDistrictNowName.Focus();
                            this.cboDistrictNowName.ShowPopup();
                        }

                        //this._shareMethod.FocusShowpopup(this.cboDistrictNowName, false);
                        //this.dlgFocusNextUserControl(this.TabIndex, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RefreshUserControl(bool loadDataAgain)
        {
            try
            {
                if (loadDataAgain)
                {
                    //this._shareMethod.InitComboCommon(this.cboDistrictNowName, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>(), "DISTRICT_CODE", "DISTRICT_NAME", "SEARCH_CODE");
                    this.cboDistrictNowName.Properties.DataSource = null;
                    this.cboDistrictNowName.EditValue = null;
                    this.txtDistrictNowCode.Text = "";
                }
                else
                {
                    this.cboDistrictNowName.EditValue = null;
                    this.txtDistrictNowCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/UCCommuneNow/RefreshUserControl: \n" + ex);
            }
        }

        #endregion

        #region Event Control

        private void cboDistrictNowName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboDistrictNowName.EditValue != null
                        && this.cboDistrictNowName.EditValue != this.cboDistrictNowName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrictNowName.EditValue.ToString());
                        if (district != null)
                        {
                            this.txtDistrictNowCode.Text = district.DISTRICT_CODE;
                            if (this.dlgLoadCommune != null)
                                this.dlgLoadCommune(district, false);
                            //if (this.dlgFocusNextUserControl != null)
                            //    this.dlgFocusNextUserControl(this.TabIndex, null);
                        }
                        if (this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl(this.TabIndex, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictNowName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string districtCode = "";
                    if (this.cboDistrictNowName.EditValue != null)
                    {
                        districtCode = this.cboDistrictNowName.EditValue.ToString();
                        this.LoadDistrict(districtCode, "");
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboDistrictNowName_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtDistrictNowCode_KeyDown_1(object sender, KeyEventArgs e)
        {
            
        }

        private void txtDistrictNowCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string districtCode = "";
                    if (this.cboDistrictNowName.EditValue != null)
                    {
                        districtCode = this.cboDistrictNowName.EditValue.ToString();
                    }
                    //this.LoadDistrict((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), districtCode);
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                    if (!String.IsNullOrEmpty(searchCode) || !String.IsNullOrEmpty(districtCode))
                    {
                        try
                        {
                            int converValue = Convert.ToInt32(searchCode);
                            listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE == searchCode).ToList();
                        }
                        catch (Exception)
                        {
                            listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) && (districtCode == "" || o.PROVINCE_CODE == districtCode)).ToList();
                        }
                    }
                    //this._shareMethod.InitComboCommon(this.cboDistrictNowName, listResult, "DISTRICT_CODE", "DISTRICT_NAME", "SEARCH_CODE"); // Load district to combo
                    if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                    {
                        this.cboDistrictNowName.Properties.DataSource = listResult;
                        this.cboDistrictNowName.EditValue = null;
                        this.txtDistrictNowCode.Text = "";
                        this._shareMethod.FocusShowpopup(this.cboDistrictNowName, false);
                        //e.Handled = true;
                    }
                    else
                    {
                        if (listResult.Count == 1)
                        {
                            bool isReLoadRef = false;
                            if (listResult[0].DISTRICT_CODE != (this.cboDistrictNowName.EditValue ?? "").ToString())
                            {
                                isReLoadRef = true;
                            }
                            if (isReLoadRef)
                            {
                                this.cboDistrictNowName.EditValue = listResult[0].DISTRICT_CODE;
                                this.txtDistrictNowCode.Text = listResult[0].DISTRICT_CODE;

                                if (this.dlgLoadCommune != null)
                                    this.dlgLoadCommune(listResult[0], true);//this.dlgLoadCommune(listResult[0], false);
                            }

                            if (this.dlgFocusNextUserControl != null)
                                this.dlgFocusNextUserControl(this.TabIndex, null);
                        }
                        else
                        {
                            if (listResult.Count > 0)
                                this.cboDistrictNowName.Properties.DataSource = listResult;
                            this.cboDistrictNowName.EditValue = null;
                            this.txtDistrictNowCode.Text = "";
                            this._shareMethod.FocusShowpopup(this.cboDistrictNowName, false);
                            //e.Handled = true;
                            //this.dlgFocusNextUserControl(this.TabIndex, null);
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
                dlgLoadCommune = null;
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.cboDistrictNowName.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDistrictNowName_Closed);
                this.cboDistrictNowName.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboDistrictNowName_KeyDown);
                this.cboDistrictNowName.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboDistrictNowName_KeyUp);
                this.txtDistrictNowCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtDistrictNowCode_KeyDown_1);
                this.txtDistrictNowCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDistrictNowCode_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCDistrictNow_Load);
                lciDistrictNow = null;
                layoutControlItem1 = null;
                txtDistrictNowCode = null;
                cboDistrictNowName = null;
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
