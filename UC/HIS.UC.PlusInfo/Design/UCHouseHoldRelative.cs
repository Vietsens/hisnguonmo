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
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using HIS.UC.PlusInfo.ShareMethod;
using HIS.UC.PlusInfo.ADO;
using Inventec.Core;
using HIS.UC.PlusInfo.ClassGlobal;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HID.EFMODEL.DataModels;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCHouseHoldRelative : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;

        #endregion

        #region Contructor

        public UCHouseHoldRelative()
            : base("UCPlusInfo", "UCHouseHoldRelative")
        {
            try
            {
                InitializeComponent();
                SetCaptionByLanguageKeyNew();
                //this.SetCaptionByLanguageKey();
                this.cboHouseHoldRelative.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCHouseHoldRelative_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCHouseHoldRelative
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCHouseHoldRelative).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHouseHoldRelative.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHouseHoldRelative.Properties.NullText = Inventec.Common.Resource.Get.Value("UCHouseHoldRelative.cboHouseHoldRelative.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHouseHoldRelative.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHouseHoldRelative.lciHouseHoldRelative.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHouseHoldRelative.Text = Inventec.Common.Resource.Get.Value("UCHouseHoldRelative.lciHouseHoldRelative.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                this.lciHouseHoldRelative.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCHouseHoldRelative", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
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

        #region Get - Set - Default Data

        public async Task DataDefault()
        {
            try
            {
                List<HID_HOUSEHOLD_RELATION> houseHoldRelates = null;
                if (BackendDataWorker.IsExistsKey<HID_HOUSEHOLD_RELATION>())
                {
                    houseHoldRelates = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HID_HOUSEHOLD_RELATION>();
                }
                else
                {
                    CommonParam param = new CommonParam();
                    HID.Filter.HidHouseholdRelationFilter householdRelationFilter = new HID.Filter.HidHouseholdRelationFilter();
                    householdRelationFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //var houseHoldRelates = new BackendAdapter(param).Get<List<HID.EFMODEL.DataModels.HID_HOUSEHOLD_RELATION>>(RequestUriStore.HID_HOUSEHOLD_RELATION_GET, ApiConsumers.HidConsumer, householdRelationFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    houseHoldRelates = ApiConsumers.HidWrapConsumer.Get<List<HID_HOUSEHOLD_RELATION>>(true, RequestUriStore.HID_HOUSEHOLD_RELATION_GET, param, householdRelationFilter);
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HOUSEHOLD_RELATION_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HOUSEHOLD_RELATION_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboHouseHoldRelative, houseHoldRelates, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(UCPlusInfoADO dataSet)
        {
            try
            {
                if (dataSet.HOUSEHOLD_RELATION_ID > 0)
                {
                    cboHouseHoldRelative.EditValue = dataSet.HOUSEHOLD_RELATION_ID;
                }
                else if (!String.IsNullOrEmpty(dataSet.HOUSEHOLD_RELATION_NAME))
                {
                    CommonParam param = new CommonParam();
                    HID.Filter.HidHouseholdRelationFilter householdRelationFilter = new HID.Filter.HidHouseholdRelationFilter();
                    householdRelationFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var houseHoldRelates = new BackendAdapter(param).Get<List<HID.EFMODEL.DataModels.HID_HOUSEHOLD_RELATION>>(RequestUriStore.HID_HOUSEHOLD_RELATION_GET, ApiConsumers.HidConsumer, householdRelationFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    var hrl = houseHoldRelates.FirstOrDefault(o => o.HOUSEHOLD_RELATION_NAME == dataSet.HOUSEHOLD_RELATION_NAME);
                    if (hrl != null)
                    {
                        this.cboHouseHoldRelative.EditValue = hrl.ID;
                    }
                }
                else
                {
                    this.cboHouseHoldRelative.EditValue = null;
                }
                //this.cboHouseHoldRelative.TabIndex = this.TabIndex;
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
                if (this.cboHouseHoldRelative.EditValue != null)
                {
                    dataGet.HOUSEHOLD_RELATION_ID = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboHouseHoldRelative.EditValue.ToString());
                    dataGet.HOUSEHOLD_RELATION_NAME = this.cboHouseHoldRelative.Text;

                }
                else
                {
                    dataGet.HOUSEHOLD_RELATION_ID = null;
                    dataGet.HOUSEHOLD_RELATION_NAME = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataGet;
        }

        #endregion

        #region Event Control

        private void cboHouseHoldRelative_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl(sender, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboHouseHoldRelative_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(cboHouseHoldRelative.Text))
                    {
                        this.cboHouseHoldRelative.ShowPopup();
                        e.Handled = true;
                    }
                    else if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/UCHouseHoldRelative/cboHouseHoldRelative_KeyDown:\n" + ex);
            }
        }
        internal void DisposeControl()
        {
            try
            {
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.cboHouseHoldRelative.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHouseHoldRelative_Closed);
                this.cboHouseHoldRelative.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboHouseHoldRelative_KeyDown);
                this.Load -= new System.EventHandler(this.UCHouseHoldRelative_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                lciHouseHoldRelative = null;
                gridLookUpEdit1View = null;
                cboHouseHoldRelative = null;
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
