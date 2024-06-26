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
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Core;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCMilitaryRank : UserControl
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;

        #endregion

        #region Constructor - Load

        public UCMilitaryRank()
        {
            try
            {
                InitializeComponent(); 
                
                this.txtMilitaryRankCode.TabIndex = this.TabIndex;
                this.SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCMilitaryRank
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCMilitaryRank).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMilitaryRank.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMilitaryRank.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMilitaryRank.cboMilitaryRank.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCMilitaryRank.layoutControlItem2.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void UCMilitaryRank_Load(object sender, EventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task InitMilitaryRank()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>>("api/HisMilitaryRank/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_MILITARY_RANK), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                _shareMethod.InitComboCommon(this.cboMilitaryRank, datas, "ID", "MILITARY_RANK_NAME", "MILITARY_RANK_CODE");
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
                this.lcgMilitaryRank.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCMilitaryRank", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event UserControl

        private void txtMilitaryRankCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboMilitaryRank.EditValue = null;
                        _shareMethod.FocusShowpopup(this.cboMilitaryRank, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().Where(o => o.MILITARY_RANK_CODE.Contains(searchCode)).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.MILITARY_RANK_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboMilitaryRank.EditValue = searchResult[0].ID;
                            this.txtMilitaryRankCode.Text = searchResult[0].MILITARY_RANK_CODE;
                            try
                            {
                                this.dlgFocusNextUserControl(this.TabIndex, null);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                                SendKeys.Send("{TAB}");
                            }
                        }
                        else
                        {
                            this.cboMilitaryRank.EditValue = null;
                            _shareMethod.FocusShowpopup(this.cboMilitaryRank, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboMilitaryRank.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MILITARY_RANK commune = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMilitaryRank.EditValue.ToString()));
                        if (commune != null)
                            this.txtMilitaryRankCode.Text = commune.MILITARY_RANK_CODE;
                    }
                    this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    MOS.EFMODEL.DataModels.HIS_MILITARY_RANK data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMilitaryRank.EditValue.ToString()));
                //    if (data != null)
                //        this.txtMilitaryRankCode.Text = data.MILITARY_RANK_CODE;
                //    if (this.dlgFocusNextUserControl != null)
                //        this.dlgFocusNextUserControl(this.TabIndex, null);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_KeyDown(object sender, KeyEventArgs e)
        {

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

        internal long? GetValue()
        {
            long? MILITARYRANK_ID = 0;
            try
            {
                if (this.cboMilitaryRank.EditValue != null)
                    MILITARYRANK_ID = (long)(this.cboMilitaryRank.EditValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return MILITARYRANK_ID;
        }

        internal void SetValue(long? militaryRankID)
        {
            try
            {
                if (militaryRankID != null)
                {
                    var militaryRank = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().FirstOrDefault(o => o.ID == militaryRankID);
                    if (militaryRank != null)
                    {
                        this.txtMilitaryRankCode.Text = (militaryRank != null ? militaryRank.MILITARY_RANK_CODE : "");
                        this.cboMilitaryRank.EditValue = militaryRankID;
                    }
                }
                else
                {
                    this.txtMilitaryRankCode.Text = "";
                    this.cboMilitaryRank.EditValue = null;
                }
                //this.txtMilitaryRankCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        internal void DisposeControl()
        {
            try
            {
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.txtMilitaryRankCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMilitaryRankCode_PreviewKeyDown);
                this.cboMilitaryRank.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMilitaryRank_Closed);
                this.cboMilitaryRank.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboMilitaryRank_KeyDown);
                this.cboMilitaryRank.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboMilitaryRank_KeyUp);
                this.Load -= new System.EventHandler(this.UCMilitaryRank_Load);
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                cboMilitaryRank = null;
                txtMilitaryRankCode = null;
                lcgMilitaryRank = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
