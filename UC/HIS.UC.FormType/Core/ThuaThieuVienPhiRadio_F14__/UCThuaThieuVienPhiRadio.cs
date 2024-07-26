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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.UC.FormType.Properties;

namespace HIS.UC.FormType.ThuaThieuVienPhiRadio
{
    public partial class UCThuaThieuVienPhiRadio : DevExpress.XtraEditors.XtraUserControl
    {
        object generateRDO;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        const string StrOutput0 = "_OUTPUT0:";
        string Output0 = "";
        string JsonOutput = "";

        public UCThuaThieuVienPhiRadio(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 25;
                this.generateRDO = paramRDO;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.config = config;

                isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);

                if (isValidData)
                {
                    layoutCaption.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lblTitleName.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    var splitD = this.config.DESCRIPTION.Split(';');
                    if (splitD.Count() == 4)
                    {
                        lblTitleName.Text = splitD[0];
                        rdTatCaVienPhi.Text = splitD[1];
                        rdThuaVienPhi.Text = splitD[2];
                        rdThieuVienPhi.Text = splitD[3];
                    }
                    else
                        lblTitleName.Text = this.config.DESCRIPTION;
                    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lblTitleName.Text = " ";
                    //lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            short? OWED_OR_RESIDUAL = null;
            try
            {
                if (rdTatCaVienPhi.Checked)
                    OWED_OR_RESIDUAL = null;
                else if (rdThuaVienPhi.Checked)
                {
                    OWED_OR_RESIDUAL = 1;
                }
                else if (rdThieuVienPhi.Checked)
                {
                    OWED_OR_RESIDUAL = 0;
                }

                value = String.Format(this.JsonOutput, ConvertUtils.ConvertToObjectFilter(OWED_OR_RESIDUAL));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }
        public void SetValue()
        {
            try
            {
                if (this.JsonOutput != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config, this.JsonOutput, this.report.JSON_FILTER);
                    if (value == "1") rdThuaVienPhi.Checked = true;
                    else if (value == "0") rdThieuVienPhi.Checked = true;
                    else rdTatCaVienPhi.Checked = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCThuaThieuVienPhiRadio_Load(object sender, EventArgs e)
        {
            try
            {
                lblTitleName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_THUA_THIEU_VIEN_PHI_RADIO_LBL_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCThuaThieuVienPhiRadio, Base.LanguageManager.GetCulture());
                rdTatCaVienPhi.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_THUA_THIEU_VIEN_PHI_RADIO_RD_TAT_CA_VIEN_PHI", Resources.ResourceLanguageManager.LanguageUCThuaThieuVienPhiRadio, Base.LanguageManager.GetCulture());
                rdThuaVienPhi.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_THUA_THIEU_VIEN_PHI_RADIO_RD_THUA_VIEN_PHI", Resources.ResourceLanguageManager.LanguageUCThuaThieuVienPhiRadio, Base.LanguageManager.GetCulture());
                rdThieuVienPhi.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_THUA_THIEU_VIEN_PHI_RADIO_RD_THIEU_VIEN_PHI", Resources.ResourceLanguageManager.LanguageUCThuaThieuVienPhiRadio, Base.LanguageManager.GetCulture());

                SetTitle();//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                GetValueOutput0(this.config.JSON_OUTPUT, ref Output0);
                JsonOutput = this.config.JSON_OUTPUT;
                RemoveStrOutput0(ref JsonOutput);
                if (!string.IsNullOrWhiteSpace(Output0))
                {
                    if (Output0 == "1") rdThuaVienPhi.Checked = true;
                    else if (Output0 == "0") rdThieuVienPhi.Checked = true;
                    else rdTatCaVienPhi.Checked = true;
                }
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetValueOutput0(string JSON_OUTPUT, ref string Output0)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = JSON_OUTPUT.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    Output0 = JSON_OUTPUT.Substring(lastIndex + StrOutput0.Length);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RemoveStrOutput0(ref string JsonOutput)
        {
            try
            {
                //string JSON_OUTPUT = "sdfsdf_OUTPUT0:2x";
                int lastIndex = JsonOutput.LastIndexOf(StrOutput0);
                if (lastIndex >= 0)
                {
                    JsonOutput = JsonOutput.Substring(0, lastIndex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        } 
    }
}
