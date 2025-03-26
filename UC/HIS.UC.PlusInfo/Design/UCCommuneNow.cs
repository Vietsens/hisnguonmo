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
    public partial class UCCommuneNow : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;
        DelegateSetValueForUCPlusInfo dlgLoadProDistByCommune;
        #endregion

        #region Constructor - Load

        public UCCommuneNow()
            : base("UCPlusInfo", "UCCommuneNow")
        {
            try
            {
                InitializeComponent();

                this.txtCommuneNowCode.TabIndex = this.TabIndex;
                //this.SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCCommuneNow_Load(object sender, EventArgs e)
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
                this.lciCommuneNow.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCCommuneNow", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCCommuneNow
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCCommuneNow).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCCommuneNow.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCommuneNowName.Properties.NullText = Inventec.Common.Resource.Get.Value("UCCommuneNow.cboCommuneNowName.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCommuneNow.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCCommuneNow.lciCommuneNow.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCommuneNow.Text = Inventec.Common.Resource.Get.Value("UCCommuneNow.lciCommuneNow.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                if (dataSet.HT_DISTRICT_NAME != null && dataSet.HT_DISTRICT_NAME.Count() > 0)
                {
                    var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == dataSet.HT_DISTRICT_CODE || o.DISTRICT_NAME == dataSet.HT_DISTRICT_NAME);

                    this.LoadCommune("", district.DISTRICT_CODE);
                }
                var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().SingleOrDefault(o => ((o.COMMUNE_CODE == dataSet.HT_COMMUNE_CODE || o.COMMUNE_NAME == dataSet.HT_COMMUNE_NAME) && (o.DISTRICT_CODE == dataSet.HT_DISTRICT_CODE || o.DISTRICT_NAME == dataSet.HT_DISTRICT_NAME)));
                if (commune != null)
                {
                    this.txtCommuneNowCode.Text = commune.SEARCH_CODE;
                    this.cboCommuneNowName.EditValue = commune.COMMUNE_CODE;
                }
                else
                {
                    this.txtCommuneNowCode.Text = "";
                    this.cboCommuneNowName.EditValue = null;
                }
                //this.txtCommuneNowCode.TabIndex = this.TabIndex;
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
                dataGet.HT_COMMUNE_NAME = this.cboCommuneNowName.Text;
                dataGet.HT_COMMUNE_CODE = this.txtCommuneNowCode.Text;
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
                //this._shareMethod.InitComboCommon(this.cboCommuneNowName, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>(), "COMMUNE_CODE", "COMMUNE_NAME", "SEARCH_CODE");
                if (dataSet != null)
                {
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_COMMUNE)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE data = (SDA.EFMODEL.DataModels.V_SDA_COMMUNE)dataSet;
                        if (isCallByUCAddress == true)
                        {
                            this.txtCommuneNowCode.Text = data.COMMUNE_CODE;
                            this.cboCommuneNowName.EditValue = data.COMMUNE_CODE;
                        }
                        else
                            this._shareMethod.FocusShowpopup(this.cboCommuneNowName, false);
                    }
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_DISTRICT)
                    {
                        //if (isCallByUCAddress == false)
                        //{
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT data = (SDA.EFMODEL.DataModels.V_SDA_DISTRICT)dataSet;
                        this.LoadCommune("", data.DISTRICT_CODE);
                        cboCommuneNowName.EditValue = null;
                        txtCommuneNowCode.Text = "";
                        //}
                    }
                }
                else
                {
                    this.cboCommuneNowName.EditValue = null;
                    this.txtCommuneNowCode.Text = "";
                    this.cboCommuneNowName.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadCommune(string searchCode, string districtCode)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                if (!String.IsNullOrEmpty(searchCode) || !String.IsNullOrEmpty(districtCode))
                {
                    try
                    {
                        int communeCode = Convert.ToInt32(searchCode);
                        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                        .Where(o => o.COMMUNE_CODE == searchCode || o.DISTRICT_CODE == districtCode).ToList();
                        // .Where(o => o.COMMUNE_CODE == searchCode && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                    }
                    catch (Exception)
                    {
                        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => (o.SEARCH_CODE ?? "").Contains(searchCode ?? "")
                                                    && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                    }
                }
                //this._shareMethod.InitComboCommon(this.cboCommuneNowName, listResult, "COMMUNE_CODE", "COMMUNE_NAME", "SEARCH_CODE"); // Load Commune to combo
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode))
                {
                    this._shareMethod.FocusShowpopup(this.cboCommuneNowName, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboCommuneNowName.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtCommuneNowCode.Text = listResult[0].COMMUNE_CODE;
                        if (this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl(this.TabIndex, null);
                    }
                    else
                    {
                        this.cboCommuneNowName.EditValue = null;
                        this.cboCommuneNowName.Properties.DataSource = null;
                        if (listResult != null && listResult.Count > 0)
                            this._shareMethod.InitComboCommon(this.cboCommuneNowName, listResult, "COMMUNE_CODE", "COMMUNE_NAME", "SEARCH_CODE");
                            //this.cboCommuneNowName.Properties.DataSource = listResult;
                        this.txtCommuneNowCode.Text = "";
                        if (String.IsNullOrEmpty(districtCode))
                        {
                            this._shareMethod.FocusShowpopup(this.cboCommuneNowName, false);
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void DelegateSetProDistByCommune(DelegateSetValueForUCPlusInfo _dlg)
        {
            try
            {
                if (_dlg != null)
                    this.dlgLoadProDistByCommune = _dlg;
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
                    //this._shareMethod.InitComboCommon(this.cboCommuneNowName, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>(), "COMMUNE_CODE", "COMMUNE_NAME", "SEARCH_CODE");
                    this.cboCommuneNowName.Properties.DataSource = null;
                    this.cboCommuneNowName.EditValue = null;
                    this.txtCommuneNowCode.Text = "";
                }
                else
                {
                    this.cboCommuneNowName.EditValue = null;
                    this.txtCommuneNowCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/UCCommuneNow/RefreshUserControl: \n" + ex);
            }
        }

        #endregion

        #region Event Control

        private void cboCommuneNowName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCommuneNowName.EditValue != null
                        && this.cboCommuneNowName.EditValue != cboCommuneNowName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommuneNowName.EditValue.ToString());
                        if (commune != null)
                            this.txtCommuneNowCode.Text = commune.COMMUNE_CODE;
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

        private void cboCommuneNowName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCommuneNowName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o => o.COMMUNE_CODE == this.cboCommuneNowName.EditValue.ToString());
                        if (commune != null)
                            this.txtCommuneNowCode.Text = commune.COMMUNE_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommuneNowCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtCommuneNowCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string communeCode = "";
                    if (this.cboCommuneNowName.EditValue != null)
                    {
                        communeCode = this.cboCommuneNowName.EditValue.ToString();
                    }
                    this.LoadCommune((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), communeCode);
                    e.Handled = true;

                    //string districtCode = "";
                    //if (this.cboCommuneNowName.EditValue != null)
                    //{
                    //    districtCode = this.cboCommuneNowName.EditValue.ToString();
                    //}
                    //string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    //List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                    //if (!String.IsNullOrEmpty(searchCode) || !String.IsNullOrEmpty(districtCode))
                    //{
                    //    try
                    //    {
                    //        int converValue = Convert.ToInt32(searchCode);
                    //        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.DISTRICT_CODE == searchCode).ToList();
                    //    }
                    //    catch (Exception)
                    //    {
                    //        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) && (districtCode == "" || o.COMMUNE_CODE == districtCode)).ToList();
                    //    }
                    //}
                    //if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                    //{
                    //    this.cboCommuneNowName.Properties.DataSource = listResult;
                    //    this.cboCommuneNowName.EditValue = null;
                    //    this.txtCommuneNowCode.Text = "";
                    //    this._shareMethod.FocusShowpopup(this.cboCommuneNowName, false);
                    //    e.Handled = true;
                    //}
                    //else
                    //{
                    //    if (listResult.Count == 1)
                    //    {
                    //        this.cboCommuneNowName.EditValue = listResult[0].DISTRICT_CODE;
                    //        this.txtCommuneNowCode.Text = listResult[0].DISTRICT_CODE;
                    //    }
                    //    else
                    //    {
                    //        if (listResult.Count > 0)
                    //            this.cboCommuneNowName.Properties.DataSource = listResult;
                    //        this.cboCommuneNowName.EditValue = null;
                    //        this.txtCommuneNowCode.Text = "";
                    //        this._shareMethod.FocusShowpopup(this.cboCommuneNowName, false);
                    //        e.Handled = true;
                    //    }
                    //}

                    //if (this.dlgFocusNextUserControl != null)
                    //    this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboCommuneNowName_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            DisposeControl();
        }
        internal void DisposeControl()
        {
            try
            {
                dlgLoadProDistByCommune = null;
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.cboCommuneNowName.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCommuneNowName_Closed);
                this.cboCommuneNowName.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboCommuneNowName_KeyDown);
                this.cboCommuneNowName.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboCommuneNowName_KeyUp);
                this.txtCommuneNowCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtCommuneNowCode_KeyDown);
                this.txtCommuneNowCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCommuneNowCode_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCCommuneNow_Load);
                lciCommuneNow = null;
                layoutControlItem1 = null;
                txtCommuneNowCode = null;
                cboCommuneNowName = null;
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
