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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private enum TabInfoPage
        {
            IS_REQUEST_SKIN_CARE,
        }
        private bool GetBulletItem(TabInfoPage tabInfoPage)
        {
            try
            {
                foreach (var control in flowLayoutPanelInfo.Controls)
                {
                    DevExpress.XtraEditors.CheckEdit checkEdit = control as DevExpress.XtraEditors.CheckEdit;
                    if (checkEdit != null && checkEdit.Tag != null
                        && checkEdit.Tag.ToString() == tabInfoPage.ToString())
                    {
                        return checkEdit.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
        private void BuildBulletedInfoList()
        {
            try
            {
                var skinCareText = GetSkinCareInfoText();
                AppendBulletItem(TabInfoPage.IS_REQUEST_SKIN_CARE, skinCareText);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void AppendBulletItem(TabInfoPage tabInfoPage, Tuple<bool, string> tuple)
        {
            if (!string.IsNullOrEmpty(tuple.Item2))
            {
                DevExpress.XtraEditors.CheckEdit checkEdit;
                checkEdit = new DevExpress.XtraEditors.CheckEdit();
                checkEdit.Tag = tabInfoPage.ToString();
                checkEdit.Location = new System.Drawing.Point(2, 2);
                checkEdit.MenuManager = barManager1;
                checkEdit.Properties.Appearance.Font = new System.Drawing.Font(checkEdit.Properties.Appearance.Font, System.Drawing.FontStyle.Bold);
                checkEdit.Properties.Appearance.Options.UseFont = true;
                checkEdit.Properties.Appearance.Options.UseForeColor = true;
                checkEdit.Properties.Caption = tuple.Item2;
                checkEdit.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
                checkEdit.Size = new System.Drawing.Size(532, 22);
                checkEdit.TabIndex = 5;
                checkEdit.CheckedChanged += new System.EventHandler(this.checkEditBulletItem_CheckedChanged);
                flowLayoutPanelInfo.Controls.Add(checkEdit);
                //
                checkEdit.Checked = tuple.Item1;
            }
        }

        private void checkEditBulletItem_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var check = sender as CheckEdit;
                if (check.Checked)
                {
                    check.Properties.Appearance.ForeColor = System.Drawing.Color.DodgerBlue;
                }
                else
                {
                    check.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
                }
                int count = 0;
                foreach (var control in flowLayoutPanelInfo.Controls)
                {
                    DevExpress.XtraEditors.CheckEdit checkEdit = control as DevExpress.XtraEditors.CheckEdit;
                    if (checkEdit != null && checkEdit.Checked)
                    {
                        count++;
                    }
                }
                if (count > 0)
                {
                    var buildingText = new StringBuilder().Append("(").Append(count).Append(")").ToString();
                    xtraTabPageInfoOther.Text = buildingText.ToString();
                }
                else
                {
                    xtraTabPageInfoOther.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Tuple<bool, string> GetSkinCareInfoText()
        {
            string text = null;
            try
            {
                if (HisConfigCFG.HisDesktopPluginsRegisterV2RequestSkinCare == "1" || HisConfigCFG.HisDesktopPluginsRegisterV2RequestSkinCare == "2")
                {
                    text = "Nhận chỉ dẫn hỗ trợ điều trị bằng các sản phẩm chăm sóc da";
                    if (this.HisServiceReqView != null)
                    {
                        CommonParam param = new CommonParam();
                        HisServiceReqFilter hisServiceReqFilter = new HisServiceReqFilter();
                        hisServiceReqFilter.ID = HisServiceReqView.ID;
                        List<HIS_SERVICE_REQ> hisServiceReqKT = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, hisServiceReqFilter, param);
                        if (hisServiceReqKT != null && hisServiceReqKT.Count > 0)
                        {
                            var hss = hisServiceReqKT.First();
                            if (hss.IS_REQUEST_SKIN_CARE.HasValue && hss.IS_REQUEST_SKIN_CARE.Value == 1)
                            {
                                return Tuple.Create(true, text);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Tuple.Create(false, text);
        }
    }
}
