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
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.InputExpMestId
{
    public partial class frmInputExpMestId : Form
    {
        public V_HIS_EXP_MEST ExpMest { get; set; }
        public frmInputExpMestId()
        {

            WaitingManager.Hide();
            InitializeComponent();
        }

        private void txtinputExpMestId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    CommonParam param = new CommonParam();
                    HisExpMestViewFilter expfilter = new HisExpMestViewFilter();
                    expfilter.EXP_MEST_CODE__EXACT = LoadExpMestCode(this.txtinputExpMestId.Text);
                    var listExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expfilter, param);
                    if (listExpMest != null && listExpMest.Count > 0 && listExpMest.FirstOrDefault().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                    {
                        this.ExpMest = listExpMest.FirstOrDefault();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy phiếu xuất máu tương ứng với mã xuất");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private string LoadExpMestCode(string p)
        {
            string result = "-1";
            try
            {
                result = string.Format("{0:000000000000}", Convert.ToInt64(p)); ;
            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Error(ex);
               result = "-1";
            }
            return result;
        }
    }
}
