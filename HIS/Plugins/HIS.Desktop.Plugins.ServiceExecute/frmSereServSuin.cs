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
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmSereServSuin : Form
    {
        Action<List<V_HIS_SERE_SERV_SUIN>> SendList { get; set; }
        List<V_HIS_SERE_SERV_SUIN> listSource;
        List<V_HIS_SERE_SERV_SUIN> listSourceSend = new List<V_HIS_SERE_SERV_SUIN>();
        public frmSereServSuin(Action<List<V_HIS_SERE_SERV_SUIN>> SendList, List<V_HIS_SERE_SERV_SUIN> listSource)
        {
            InitializeComponent();

            try
            {
                this.SendList = SendList;
                this.listSource = listSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmSereServSuin_Load(object sender, EventArgs e)
        {
            try
            {
                AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_SUIN, V_HIS_SERE_SERV_SUIN>();
                listSourceSend = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_SUIN>>(listSource);
                this.gridControl1.DataSource = listSourceSend;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                listSourceSend.ForEach(s =>
                {
                    s.VALUE = !string.IsNullOrEmpty(s.VALUE) ? s.VALUE.Trim() : null;
                    s.DESCRIPTION = !string.IsNullOrEmpty(s.DESCRIPTION) ? s.DESCRIPTION.Trim() : null;
                });
                gridControl1.RefreshDataSource();
                var elementError = listSourceSend.FirstOrDefault(o => (!string.IsNullOrEmpty(o.VALUE) && Encoding.UTF8.GetByteCount(o.VALUE.Trim()) > 200) || (!string.IsNullOrEmpty(o.DESCRIPTION) && Encoding.UTF8.GetByteCount(o.DESCRIPTION.Trim()) > 500));
                if (elementError != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Mã chỉ số {0} có {1}", elementError.SUIM_INDEX_CODE, !string.IsNullOrEmpty(elementError.VALUE) && Encoding.UTF8.GetByteCount(elementError.VALUE.Trim()) > 200 ? "giá trị vượt quá 200 ký tự." : "mô tả vượt quá 500 ký tự"));
                    return;
                }
                SendList(listSourceSend);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }
    }
}
