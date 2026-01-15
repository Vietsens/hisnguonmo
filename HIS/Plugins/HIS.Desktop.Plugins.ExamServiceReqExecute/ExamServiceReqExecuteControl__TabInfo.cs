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
using HIS.Desktop.LocalStorage.BackendData;
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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private string GetSkinCareInfoText()
        {
            try
            {
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
                            return "Nhận chỉ dẫn hỗ trợ điều trị bằng các sản phẩm chăm sóc da";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return string.Empty;
        }
        private void BuildBulletedInfoList()
        {
            try
            {
                int count = 0;
                richTextBoxInfoOther.Clear();
                richTextBoxInfoOther.SelectionBullet = false;
                var skinCareText = GetSkinCareInfoText();
                if (!string.IsNullOrEmpty(skinCareText))
                {
                    count++;
                    AppendBulletItem(skinCareText);
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
                richTextBoxInfoOther.SelectionStart = 0;
                richTextBoxInfoOther.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AppendBulletItem(string text)
        {
            int start = richTextBoxInfoOther.TextLength;
            // Bullet part (keep default color)
            richTextBoxInfoOther.SelectionColor = richTextBoxInfoOther.ForeColor;
            richTextBoxInfoOther.AppendText("\u2022 ");
            // Text part (blue)
            richTextBoxInfoOther.SelectionColor = Color.DodgerBlue;
            richTextBoxInfoOther.AppendText(text);
            // New line
            richTextBoxInfoOther.SelectionColor = richTextBoxInfoOther.ForeColor;
            richTextBoxInfoOther.AppendText(Environment.NewLine);
            // Ensure bullet stays black even if user selects all etc.
            richTextBoxInfoOther.Select(start, 2);
            richTextBoxInfoOther.SelectionColor = richTextBoxInfoOther.ForeColor;
            richTextBoxInfoOther.Select(richTextBoxInfoOther.TextLength, 0);
        }
    }
}
