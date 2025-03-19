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

namespace HIS.UC.FormType.MediStockSttFilterCheckBoxGroup
{
    public partial class UCMediStockSttFilterCheckBoxGroup : DevExpress.XtraEditors.XtraUserControl
    {
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK mediStock;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCMediStockSttFilterCheckBoxGroup(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 25;
             
                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Init()
        {
            try
            {
                radioAll.Checked = true;

                if (this.isValidData)
                {
                    lblTitleName.ForeColor = Color.Maroon;
                }
                SetTitle();//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                
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
                //if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                //{
                //    lblTitleName.Text = this.config.DESCRIPTION;
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}
                //else
                //{
                //    lciTitleName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //}
                //if (this.config != null)
                //{
                //    lciTitleName.Text = this.config.DESCRIPTION ?? " ";
                //}
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    var splitD = this.config.DESCRIPTION.Split(';');
                    if (splitD.Count() == 5)
                    {
                        lblTitleName.Text = splitD[0];
                        radioAll.Text = splitD[1];
                        radioImp.Text = splitD[2];
                        radioExp.Text = splitD[3];
                        radioInventory.Text = splitD[4];
                    }
                    else
                        lblTitleName.Text = this.config.DESCRIPTION;
                }
                else
                {
                    lblTitleName.Text = " ";
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
            short? IMP_EXP_INVENTORY = null;
            long MEDI_STOCK_ID = 0;
            try
            {
                if (radioAll.Checked)
                {
                    //Nothing
                }
                else if (radioImp.Checked)
                {
                    IMP_EXP_INVENTORY = 0;
                }
                else if (radioExp.Checked)
                {
                    IMP_EXP_INVENTORY = 1;
                }
                else if (radioInventory.Checked)
                {
                    IMP_EXP_INVENTORY = 2;
                }

                if (this.mediStock != null)
                {
                    MEDI_STOCK_ID = mediStock.ID;
                }

                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(IMP_EXP_INVENTORY), MEDI_STOCK_ID);
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
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    var jsOutputSub = this.config.JSON_OUTPUT.Split(new string[] { "," }, StringSplitOptions.None);
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[0], this.report.JSON_FILTER);
                    if (value != null && value != "null")
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(value) == 0) radioImp.Checked=true;
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(value) == 1) radioExp.Checked = true;
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(value) == 2) radioInventory.Checked = true;
                    }
                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        {
                            mediStock.ID = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.isValidData)
                {
                    if (!radioAll.Checked && !radioExp.Checked && !radioImp.Checked && !radioInventory.Checked)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void UCMediStockSttFilterCheckBoxGroup_Load(object sender, EventArgs e)
        {
            try
            {
                //radioAll.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_STR_FILTER_CHECKBOX_GROUP_RADIO_ALL", Resources.ResourceLanguageManager.LanguageUCMediStockSttFilterCheckBoxGroup, Base.LanguageManager.GetCulture());
                //radioImp.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_STR_FILTER_CHECKBOX_GROUP_RADIO_IMP", Resources.ResourceLanguageManager.LanguageUCMediStockSttFilterCheckBoxGroup, Base.LanguageManager.GetCulture());
                //radioExp.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_STR_FILTER_CHECKBOX_GROUP_RADIO_EXP", Resources.ResourceLanguageManager.LanguageUCMediStockSttFilterCheckBoxGroup, Base.LanguageManager.GetCulture());
                //radioInventory.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_MEDISTOCK_STR_FILTER_CHECKBOX_GROUP_RADIO_INVENTORY", Resources.ResourceLanguageManager.LanguageUCMediStockSttFilterCheckBoxGroup, Base.LanguageManager.GetCulture());
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
    }
}
