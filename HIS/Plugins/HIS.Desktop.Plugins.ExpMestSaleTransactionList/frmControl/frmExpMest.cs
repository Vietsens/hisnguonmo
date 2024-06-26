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
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleTransactionList.frmControl
{
    public partial class frmExpMest : HIS.Desktop.Utility.FormBase
    {
        List<string> _datas;
        DHisTransExpSDO dataRow;

        public frmExpMest()
        {
            InitializeComponent();
        }

        public frmExpMest(List<string> __sss, DHisTransExpSDO dataR)
        {
            InitializeComponent();
            try
            {
                _datas = __sss;
                dataRow = dataR;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExpMest_Load(object sender, EventArgs e)
        {
            try
            {
                List<ADOCode> dataAdos = new List<ADOCode>();

                foreach (var item in this._datas)
                {
                    ADOCode ado = new ADOCode(item);
                    dataAdos.Add(ado);
                }

                gridControlControl.DataSource = dataAdos;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private class ADOCode
        {
            public string CODE { get; set; }

            public ADOCode() { }
            public ADOCode(string code)
            {
                try
                {
                    this.CODE = code;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (ADOCode)gridViewControl.GetFocusedRow();
                if (data != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    PharmacyCashierExpCancelSDO sdo = new PharmacyCashierExpCancelSDO();
                    sdo.ExpMestCode = data.CODE;

                    var room = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(p => p.ID == dataRow.CASHIER_ROOM_ID);
                    sdo.WorkingRoomId = room != null ? room.ROOM_ID : 0;

                    var rs = new BackendAdapter(param).Post<bool>("api/HisExpMest/PharmacyCashierExpCancel", ApiConsumers.MosConsumer, sdo, param);
                    if (rs)
                    {
                        success = true;

                        // ReLoadDataBeforDelete(success);
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

}
