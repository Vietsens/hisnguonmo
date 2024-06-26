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
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.Vaccination
{
    public partial class frmChangeMedicine : Form
    {
        V_HIS_EXP_MEST_MEDICINE currentData;
        DelegateSelectData dlg;
        long roomId;
        V_HIS_MEDI_STOCK currentMediStock;

        public frmChangeMedicine(V_HIS_EXP_MEST_MEDICINE data, DelegateSelectData dlg, long roomId, V_HIS_MEDI_STOCK mediStock)
        {
            InitializeComponent();
            try
            {
                this.currentData = data;
                this.dlg = dlg;
                this.roomId = roomId;
                this.currentMediStock = mediStock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChangeMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                if (this.currentData.TDL_MEDICINE_TYPE_ID != null)
                {
                    LoadComboLoThuoc(this.currentData.TDL_MEDICINE_TYPE_ID ?? 0, cboMedicine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboLoThuoc(long medicineTypeId, Control control)
        {
            try
            {
                if (this.currentMediStock != null)
                {
                    CommonParam param = new CommonParam();
                    HisMedicineStockViewFilter medicineFilter = new HisMedicineStockViewFilter();
                    medicineFilter.MEDICINE_TYPE_ID = medicineTypeId;
                    medicineFilter.MEDI_STOCK_ID = this.currentMediStock.ID;
                    medicineFilter.IS_LEAF = 1;
                    medicineFilter.MEDICINE_TYPE_IS_ACTIVE = true;
                    medicineFilter.ORDER_DIRECTION = "ASC";
                    medicineFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";

                    var medicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisMedicineInStockSDO>>("/api/HisMedicine/GetInStockMedicine", ApiConsumers.MosConsumer, medicineFilter, param);

                    if (medicine != null && medicine.Count > 0)
                    {
                        var availableMedicine = medicine.Where(o => (o.AvailableAmount != null && o.AvailableAmount > 0) && o.ID != this.currentData.MEDICINE_ID && o.AvailableAmount >= this.currentData.AMOUNT && o.IMP_PRICE == this.currentData.IMP_PRICE && o.IMP_VAT_RATIO == this.currentData.IMP_VAT_RATIO).ToList();
                        InitCombo(control, "PACKAGE_NUMBER", "ID", "AvailableAmount", availableMedicine);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(Control control, string displayMember, string valueMember, string column1, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();

                columnInfos.Add(new ColumnInfo(displayMember, "", 250, 1));
                if (!string.IsNullOrEmpty(column1))
                {
                    columnInfos.Add(new ColumnInfo(column1, "", 100, 2));
                }

                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboMedicine.EditValue == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn lô", "Thông báo");
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();

                HisVaccinationChangeMedicineSDO sdo = new HisVaccinationChangeMedicineSDO();
                sdo.ExpMestMedicineId = this.currentData.ID;
                sdo.NewMedicineId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMedicine.EditValue.ToString());
                sdo.WorkingRoomId = this.roomId;

                var rsApi = new BackendAdapter(param).Post<HIS_EXP_MEST_MEDICINE>("api/HisVaccination/ChangeMedicine", ApiConsumers.MosConsumer, sdo, param);
                if (rsApi != null)
                {
                    success = true;
                    if (this.dlg != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(this.currentData, rsApi);
                        this.dlg(this.currentData);
                    }
                    this.Close();
                }
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
