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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class UCExpMestSaleCreate : UserControlBase 
    {
        public void BtnAddShortcut()
        {
            try
            {
                if (btnAdd.Enabled)
                {
                    btnAdd.Focus();
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusMediMateSearch()
        {
            try
            {
                txtMediMatyForPrescription.Focus();
                txtMediMatyForPrescription.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnSaveShortcut()
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave.Focus();
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnCancelShortcut()
        {
            try
            {
                if (btnCancelExport.Enabled)
                {
                    btnCancelExport.Focus();
                    btnCancelExport_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnSaveWithPrintShortcut()
        {
            try
            {
                if (btnSavePrint.Enabled)
                {
                    btnSavePrint.Focus();
                    btnSavePrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnNewShortcut()
        {
            try
            {
                if (btnNew.Enabled)
                {
                    btnNew.Focus();
                    btnNew_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnNewExpMestShortcut()
        {
            try
            {
                if (btnNewExpMest.Enabled)
                {
                    btnNewExpMest.Focus();
                    btnNewExpMest_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnSaleBillShortcut()
        {
            try
            {
                if (btnSaleBill.Enabled)
                {
                    btnSaleBill.Focus();
                    btnSaleBill_Click(null, null);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusPresCodeShortcut()
        {
            try
            {
                if (txtPrescriptionCode.Enabled)
                {
                    txtPrescriptionCode.Focus();
                    txtPrescriptionCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void BtnPresCodeShortcut()
        {
            try
            {
                ProcessSelectMultiPrescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
