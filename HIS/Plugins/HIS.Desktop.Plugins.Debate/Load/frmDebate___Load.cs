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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Debate
{
    public partial class frmDebate : HIS.Desktop.Utility.FormBase
    {
        private void LoadGridDebate()
        {
            try
            {
                HisDebateViewFilter debaseFilter = new HisDebateViewFilter();
                debaseFilter.ORDER_FIELD = "MODIFY_TIME";
                debaseFilter.ORDER_DIRECTION = "DESC";

                if (treatmentId > 0)
                {
                    txtTreatmentCode.Text = treatment.TREATMENT_CODE;
                    debaseFilter.TREATMENT_ID = treatmentId;
                }
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    debaseFilter.TREATMENT_CODE__EXACT = txtTreatmentCode.Text;
                }
                if (!string.IsNullOrEmpty(txtKeyword.Text))
                {
                    debaseFilter.KEY_WORD = txtKeyword.Text;
                }
                if (dtTimeFrom.EditValue != null && dtTimeTo.EditValue != null)
                {
                    debaseFilter.DEBATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    debaseFilter.DEBATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if (chkCreator.Checked)
                {
                    debaseFilter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }
                else if (chkUserInvite.Checked)
                {
                    debaseFilter.INVITE_USER_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }
                if (executeDepartmentSelecteds != null && executeDepartmentSelecteds.Count() > 0)
                {
                    debaseFilter.DEPARTMENT_IDs = executeDepartmentSelecteds.Select(o => o.ID).ToList();
                }


                hisDebates = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumers.MosConsumer, debaseFilter, new CommonParam());

                if (hisDebates != null && hisDebates.Count() > 0)
                {
                    this.lstTreatment = new List<V_HIS_TREATMENT>();
                    List<long> lstTreatmentIds = new List<long>();
                    lstTreatmentIds = hisDebates.Select(o => o.TREATMENT_ID).ToList();
                    int count = 0;
                    while (lstTreatmentIds.Count - count > 0)
                    {
                        HisTreatmentViewFilter filterTreatment = new HisTreatmentViewFilter();
                        filterTreatment.IDs = lstTreatmentIds.Take(100).Skip(count).ToList();
                        var lstTreatmentTmp = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filterTreatment, null);
                        if(lstTreatmentTmp != null && lstTreatmentTmp.Count > 0)
                        {
                            lstTreatment.AddRange(lstTreatmentTmp);
                        }
                        count += 100;
                    }
                }
                gridControlDebateReq.DataSource = hisDebates;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadTreatmentById(long treatmentId)
        {
            try
            {
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = treatmentId;
                treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool CheckValidTreatmentIsPause(long treatmentIsPause, CommonParam param)
        {
            bool valid = false;
            try
            {
                if (treatmentIsPause != 1)
                {
                    valid = true;
                }
                if (!valid)
                {
                    param.Messages.Add(Resources.ResourceMessage.HoSoDieuTriDangKhoa);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmDebate = new ResourceManager("HIS.Desktop.Plugins.Debate.Resources.Lang", typeof(frmDebate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.btnPrintDebate.Text = Inventec.Common.Resource.Get.Value("frmDebate.btnPrintDebate.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmDebate.btnReset.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmDebate.btnFind.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("frmDebate.navBarControl1.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("frmDebate.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmDebate.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmDebate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmDebate.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmDebate.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.chkAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmDebate.chkAll.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.chkUserInvite.Properties.Caption = Inventec.Common.Resource.Get.Value("frmDebate.chkUserInvite.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.chkCreator.Properties.Caption = Inventec.Common.Resource.Get.Value("frmDebate.chkCreator.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.navBarGroup2.Caption = Inventec.Common.Resource.Get.Value("frmDebate.navBarGroup2.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmDebate.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmDebate.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.cboExecuteDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmDebate.cboExecuteDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmDebate.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.chkAutoSign.Properties.Caption = Inventec.Common.Resource.Get.Value("frmDebate.chkAutoSign.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.btnnew.Text = Inventec.Common.Resource.Get.Value("frmDebate.btnnew.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmDebate.STT.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDebateedit.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridDebateedit.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDebateedit.ToolTip = Inventec.Common.Resource.Get.Value("frmDebate.gridDebateedit.ToolTip", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumnIn.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumnIn.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumnIn.ToolTip = Inventec.Common.Resource.Get.Value("frmDebate.gridColumnIn.ToolTip", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDebateDelete.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridDebateDelete.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDebateDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmDebate.gridDebateDelete.ToolTip", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDebateTime.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridDebateTime.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDabeteconten.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridDabeteconten.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDebateConclusion.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridDebateConclusion.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridDabeteICD_NAME.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridDabeteICD_NAME.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmDebate.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.lciChkAutoSign.Text = Inventec.Common.Resource.Get.Value("frmDebate.lciChkAutoSign.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDebate.Text", Resources.ResourceLanguageManager.LanguagefrmDebate, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
