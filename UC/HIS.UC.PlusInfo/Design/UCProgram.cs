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
using HIS.UC.PlusInfo.ShareMethod;
using HIS.Desktop.DelegateRegister;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.UC.PlusInfo.ClassGlobal;
using HIS.Desktop.ApiConsumer;
using HIS.UC.PlusInfo.ADO;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System.Resources;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCProgram : UserControl
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;

        List<V_HIS_PATIENT_PROGRAM> _HisPatientPrograms { get; set; }

        #endregion

        #region Constructor - Load

        public UCProgram()
        {
            try
            {
                InitializeComponent();

                this.txtProgramCode.TabIndex = this.TabIndex;
                this.SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCProgram_Load(object sender, EventArgs e)
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
        ///Hàm xét ngôn ngữ cho giao diện UCProgram
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCProgram).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCProgram.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProgram.Properties.NullText = Inventec.Common.Resource.Get.Value("UCProgram.cboProgram.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProgram.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCProgram.lciProgram.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProgram.Text = Inventec.Common.Resource.Get.Value("UCProgram.lciProgram.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                this._HisPatientPrograms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_PROGRAM>(false, true, true, false);
                if (_HisPatientPrograms == null || _HisPatientPrograms.Count <= 0)
                    this._HisPatientPrograms = new List<V_HIS_PATIENT_PROGRAM>();

                var programIds = this._HisPatientPrograms != null ? this._HisPatientPrograms.OrderBy(o => o.PATIENT_PROGRAM_CODE).Select(o => o.PROGRAM_ID).ToList() : null;
                var _HisPrograms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PROGRAM>();

                //lấy ra danh sách chương trình đã được gán với hồ sơ.
                List<HIS_PROGRAM> listProgram = new List<HIS_PROGRAM>();
                if (programIds != null && programIds.Count > 0 && _HisPrograms != null && _HisPrograms.Count > 0)
                {
                    listProgram = _HisPrograms.Where(o => programIds.Contains(o.ID)).OrderBy(o => o.PROGRAM_CODE).ToList();
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listProgram), listProgram));

                _shareMethod.InitComboCommon(this.cboProgram, listProgram, "ID", "PROGRAM_NAME", "PROGRAM_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lciProgram.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCProgram", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control

        private void txtProgramCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboProgram.Focus();
                        this.cboProgram.ShowPopup();
                    }
                    else
                    {
                        var data = this._HisPatientPrograms.Where(o => o.PROGRAM_CODE.Contains(searchCode)).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.PROGRAM_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboProgram.EditValue = searchResult[0].PROGRAM_ID;
                            this.txtProgramCode.Text = searchResult[0].PROGRAM_NAME;
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
                            this.cboProgram.EditValue = null;
                            this.cboProgram.Focus();
                            this.cboProgram.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboProgram.EditValue != null)
                    {
                        var data = this._HisPatientPrograms.FirstOrDefault(o => o.PROGRAM_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboProgram.EditValue.ToString()));
                        if (data != null)
                        {
                            txtProgramCode.Text = data.PROGRAM_CODE;
                        }
                    }
                }
                if (this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl(this.TabIndex, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Focus ra khoi UCProgram that bai: \n" + ex);
                SendKeys.Send("{TAB}");
            }
        }

        private void cboProgram_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboProgram.EditValue != null)
                    {
                        var data = this._HisPatientPrograms.FirstOrDefault(o => o.PROGRAM_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboProgram.EditValue.ToString()));
                        if (data != null)
                        {
                            txtProgramCode.Text = data.PROGRAM_CODE;
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

        #endregion

        #region Data

        public void SetValue(long patientID, long programid)
        {
            try
            {
                this.txtProgramCode.Text = "";
                this.cboProgram.EditValue = null;
                if (patientID > 0)
                {

                    var patientPrograms = this._HisPatientPrograms.Where(p => p.PATIENT_ID == patientID).ToList();
                    _shareMethod.InitComboCommon(this.cboProgram, patientPrograms, "PROGRAM_ID", "PROGRAM_NAME", "PROGRAM_CODE");
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.ISALLOWPROGRAMPATIENTOLD == "1" && patientPrograms.Any(p => p.PROGRAM_ID == programid) )
                    {
                        this.cboProgram.EditValue = programid;
                        var selectedProgram = patientPrograms.FirstOrDefault(p => p.PROGRAM_ID == programid);
                        if (selectedProgram != null)
                        {
                            this.txtProgramCode.Text = selectedProgram.PROGRAM_CODE;
                        }
                    }
                }
                else
                {
                    DataDefault();
                }
                //this.txtProgramCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal long GetValue()
        {
            long PROGRAM_ID = 0;
            try
            {
                if (this.cboProgram.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_PATIENT_PROGRAM data = this._HisPatientPrograms.FirstOrDefault(o => o.PROGRAM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboProgram.EditValue ?? "0").ToString()));
                    if (data != null)
                        PROGRAM_ID = data.PROGRAM_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return PROGRAM_ID;
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

        private void cboProgram_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboProgram.EditValue = null;
                    txtProgramCode.Text = "";
                    txtProgramCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal void DisposeControl()
        {
            try
            {
                _HisPatientPrograms = null;
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.txtProgramCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtProgramCode_KeyDown);
                this.cboProgram.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProgram_Closed);
                this.cboProgram.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboProgram_ButtonClick);
                this.cboProgram.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.cboProgram_KeyDown);
                this.Load -= new System.EventHandler(this.UCProgram_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                gridLookUpEdit1View = null;
                cboProgram = null;
                lciProgram = null;
                layoutControlItem1 = null;
                txtProgramCode = null;
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
