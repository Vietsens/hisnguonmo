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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.MediStockExpend
{
    internal partial class FormMediStockExpend : FormBase
    {
        private long RoomId;
        private long? MediStockId;
        private Action<long> ReturnMediStockId;
        private List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> ListMestRoom = new List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM>();

        public FormMediStockExpend(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> listMestRoom, long? savedMediStock, Action<long> dlg)
            : base()
        {
            InitializeComponent();
            this.ListMestRoom = listMestRoom;
            this.MediStockId = savedMediStock;
            this.ReturnMediStockId = dlg;
            this.SetIcon();
        }

        private void FormMediStockExpend_Load(object sender, EventArgs e)
        {
            try
            {
                InitDataCbo();
                LoadDefaultData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultData()
        {
            try
            {
                //kho cũ có thông tin kho thiết lập với phòng thì hiển thị mặc định
                if (this.MediStockId > 0 && this.ListMestRoom.Exists(o => o.MEDI_STOCK_ID == this.MediStockId))
                {
                    cboMediStock.EditValue = this.MediStockId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataCbo()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboMediStock, this.ListMestRoom, controlEditorADO);
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
                if (this.cboMediStock.EditValue != null && this.ReturnMediStockId != null)
                {
                    this.ReturnMediStockId(Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMediStock.EditValue.ToString()));
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
