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
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCBloodRh : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;

        #endregion

        #region Contructor - Load

        public UCBloodRh()
            : base("UCPlusInfo", "UCBloodRh")
        {
            try
            {
                InitializeComponent();

                this.cboBloodRh.TabIndex = this.TabIndex;
                //this.SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCBloodRh_Load(object sender, EventArgs e)
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
                this.lciBloodRh.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCBloodRh", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCBloodRh
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCBloodRh).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCBloodRh.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodRh.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBloodRh.cboBloodRh.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodRh.Text = Inventec.Common.Resource.Get.Value("UCBloodRh.lciBloodRh.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        #region Get - Set Value

        internal UCPlusInfoADO GetValue()
        {
            UCPlusInfoADO dataGet = new UCPlusInfoADO();
            try
            {
                if (this.cboBloodRh.EditValue != null)
                {
                    dataGet.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodRh.EditValue.ToString());
                    dataGet.BLOOD_RH_CODE = this.cboBloodRh.Text;
                }
                else
                {
                    dataGet.BLOOD_RH_ID = null;
                    dataGet.BLOOD_RH_CODE = "";
                }
                //this.cboBloodRh.TabIndex = this.TabIndex;
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
                if (dataSet.BLOOD_ABO_ID > 0)
                {
                    cboBloodRh.EditValue = dataSet.BLOOD_RH_ID;
                }
                else if (!String.IsNullOrEmpty(dataSet.BLOOD_RH_CODE))
                {
                    var abo = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().FirstOrDefault(o => o.BLOOD_RH_CODE == dataSet.BLOOD_RH_CODE);
                    if (abo != null)
                    {
                        cboBloodRh.EditValue = abo.ID;
                    }
                }
                else
                    this.cboBloodRh.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task DataDefault()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>();
                }
                else
                {
                    Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>>("api/HisBloodRh/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_BLOOD_RH), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBloodRh, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboBloodRh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl(this.TabIndex, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRh_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBloodRh.EditValue == null)
                    {
                        this.cboBloodRh.ShowPopup();
                        e.Handled = true;
                    }
                    else if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/UCBloodRh/btnBloodRh_KeyDown:\n" + ex);
            }
        }

        internal void DisposeControl()
        {
            try
            {
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.cboBloodRh.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBloodRh_Closed);
                this.cboBloodRh.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboBloodRh_KeyDown);
                this.Load -= new System.EventHandler(this.UCBloodRh_Load);
                gridView1.GridControl.DataSource = null;
                lciBloodRh = null;
                gridView1 = null;
                cboBloodRh = null;
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
