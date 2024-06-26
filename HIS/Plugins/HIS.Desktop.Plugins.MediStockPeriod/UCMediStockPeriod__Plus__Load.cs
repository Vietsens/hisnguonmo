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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Print;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.MediStockPeriod.ADO;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.MediStockPeriod
{
    public partial class UCMediStockPeriod : UserControl
    {
        private void LoadDataMedicine(MediStockPeriodADO hisMediStockPeriod)
        {
            try
            {
                WaitingManager.Show();
                gridControlMedicine.DataSource = null;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisMestPeriodMediViewFilter mestPeriodMetyFilter = new MOS.Filter.HisMestPeriodMediViewFilter();
                mestPeriodMetyFilter.KEY_WORD = txtSearchMedicine.Text.Trim();
                //if (chkHienThiDongLoiKhongQuanTamTonKho.Checked)
                //    mestPeriodMetyFilter.IS_ERROR_NOT_INVENTORY = true;
                //if (chkHienDongLoi.Checked)
                //    mestPeriodMetyFilter.IS_ERROR = true;
                //if (chkKhongHienDongHet.Checked)
                //    mestPeriodMetyFilter.IS_EMPTY = false;
                //if (chkKhongHienDongTangGiam.Checked)
                //    mestPeriodMetyFilter.IS_NO_CHANGE = false;
                //if (chkHienThiDongLoiKhongQuanTamTonKho.Checked)
                //    mestPeriodMetyFilter.IS_NO_CHANGE = false;

                mestPeriodMetyFilter.ORDER_DIRECTION = "ASC";
                mestPeriodMetyFilter.ORDER_FIELD = "MEDICINE_TYPE_NAME";
                mestPeriodMetyFilter.MEDI_STOCK_PERIOD_ID = hisMediStockPeriod.ID;

                ListMestPeriodMety = new List<HisMestPeriodMediAdo>();

                ListMestPeriodMety = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HisMestPeriodMediAdo>>(RequestUriStore.HIS_MEST_PERIOD_MEDI_GETVIEW, ApiConsumers.MosConsumer, mestPeriodMetyFilter, paramCommon);
                if (ListMestPeriodMety != null && ListMestPeriodMety.Count > 0)
                {
                    ListMestPeriodMety.ForEach(o => o.KK_AMOUNT = o.INVENTORY_AMOUNT ?? 0);
                }
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = ListMestPeriodMety;
                gridControlMedicine.EndUpdate();
                //gridViewMedicine.OptionsSelection.EnableAppearanceFocusedCell = false;
                //gridViewMedicine.OptionsSelection.EnableAppearanceFocusedRow = false;
                //gridViewMedicine.BestFitColumns();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineByFilter()
        {
            try
            {
                WaitingManager.Show();
                gridControlMedicine.DataSource = null;
                string KEY_WORD = txtSearchMedicine.Text.Trim();

                var mestPeriodMetys = this.ListMestPeriodMety.AsQueryable();

                if (!String.IsNullOrEmpty(KEY_WORD))
                {
                    KEY_WORD = KEY_WORD.ToLower().Trim();
                    mestPeriodMetys = mestPeriodMetys.Where(o =>
                        o.CREATOR.ToLower().Contains(KEY_WORD) ||
                        o.MODIFIER.ToLower().Contains(KEY_WORD) ||
                        o.MEDICINE_TYPE_CODE.ToLower().Contains(KEY_WORD) ||
                        o.MEDICINE_TYPE_NAME.ToLower().Contains(KEY_WORD) ||
                        (o.PACKAGE_NUMBER != null && o.PACKAGE_NUMBER.ToLower().Contains(KEY_WORD)) ||
                        o.MEDI_STOCK_PERIOD_NAME.ToLower().Contains(KEY_WORD) ||
                        (o.NATIONAL_NAME != null && o.NATIONAL_NAME.ToLower().Contains(KEY_WORD)) ||
                        o.SERVICE_UNIT_CODE.ToLower().Contains(KEY_WORD) ||
                        o.SERVICE_UNIT_NAME.ToLower().Contains(KEY_WORD)
                        );
                }

                if (chkHienDongLoi.Checked)
                {
                    mestPeriodMetys = mestPeriodMetys.Where(o => o.IN_AMOUNT < 0 || o.OUT_AMOUNT < 0 || o.BEGIN_AMOUNT < 0 || o.VIR_END_AMOUNT < 0 || o.INVENTORY_AMOUNT < 0 || o.VIR_END_AMOUNT != o.INVENTORY_AMOUNT);
                }
                //else
                //{
                //    mestPeriodMetys = mestPeriodMetys.Where(o => o.IN_AMOUNT >= 0 && o.OUT_AMOUNT >= 0 && o.BEGIN_AMOUNT >= 0 && o.VIR_END_AMOUNT >= 0 && o.INVENTORY_AMOUNT >= 0 && o.VIR_END_AMOUNT == o.INVENTORY_AMOUNT);
                //}
                if (chkKhongHienDongHet.Checked)
                {
                    mestPeriodMetys = mestPeriodMetys.Where(o => o.VIR_END_AMOUNT == 0);
                }
                //else
                //{
                //    mestPeriodMetys = mestPeriodMetys.Where(o => o.VIR_END_AMOUNT != 0);
                //}
                if (chkKhongHienDongTangGiam.Checked)
                {
                    mestPeriodMetys = mestPeriodMetys.Where(o => o.BEGIN_AMOUNT == o.VIR_END_AMOUNT);
                }
                //else
                //{
                //    mestPeriodMetys = mestPeriodMetys.Where(o => o.BEGIN_AMOUNT != o.VIR_END_AMOUNT);
                //}
                if (chkHienThiDongLoiKhongQuanTamTonKho.Checked)
                {
                    mestPeriodMetys = mestPeriodMetys.Where(o => o.IN_AMOUNT < 0 || o.OUT_AMOUNT < 0 || o.BEGIN_AMOUNT < 0 || o.VIR_END_AMOUNT < 0);
                }
                //else
                //{
                //    mestPeriodMetys = mestPeriodMetys.Where(o => o.IN_AMOUNT >= 0 && o.OUT_AMOUNT >= 0 && o.BEGIN_AMOUNT >= 0 && o.VIR_END_AMOUNT >= 0);
                //}
                gridControlMedicine.BeginUpdate();
                gridControlMedicine.DataSource = mestPeriodMetys.ToList();
                gridControlMedicine.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(MediStockPeriodADO hisMediStockPeriod)
        {
            try
            {
                WaitingManager.Show();
                gridControlMaterial.DataSource = null;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisMestPeriodMateViewFilter mestPeriodMatyFilter = new MOS.Filter.HisMestPeriodMateViewFilter();
                mestPeriodMatyFilter.KEY_WORD = txtSearchMaterial.Text.Trim();
                //if (chkHienThiDongLoiKhongQuanTamTonKho.Checked)
                //    mestPeriodMatyFilter.IS_ERROR_NOT_INVENTORY = true;
                //if (chkHienDongLoi.Checked)
                //    mestPeriodMatyFilter.IS_ERROR = true;
                //if (chkKhongHienDongHet.Checked)
                //    mestPeriodMatyFilter.IS_EMPTY = false;
                //if (chkKhongHienDongTangGiam.Checked)
                //    mestPeriodMatyFilter.IS_NO_CHANGE = false;
                mestPeriodMatyFilter.ORDER_DIRECTION = "ASC";
                mestPeriodMatyFilter.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                mestPeriodMatyFilter.MEDI_STOCK_PERIOD_ID = hisMediStockPeriod.ID;

                ListMestPeriodMaty = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HisMestPeriodMateAdo>>(RequestUriStore.HIS_MEST_PERIOD_MATE_GETVIEW, ApiConsumers.MosConsumer, mestPeriodMatyFilter, paramCommon);
                if (ListMestPeriodMaty != null && ListMestPeriodMaty.Count > 0)
                {
                    ListMestPeriodMaty.ForEach(o => o.KK_AMOUNT = o.INVENTORY_AMOUNT ?? 0);
                }
                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = ListMestPeriodMaty;
                gridControlMaterial.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialByFilter()
        {
            try
            {
                WaitingManager.Show();
                gridControlMaterial.DataSource = null;
                string KEY_WORD = txtSearchMaterial.Text.Trim();

                var mestPeriodMatys = this.ListMestPeriodMaty.AsQueryable();

                if (!String.IsNullOrEmpty(KEY_WORD))
                {
                    KEY_WORD = KEY_WORD.ToLower().Trim();
                    mestPeriodMatys = mestPeriodMatys.Where(o =>
                        o.CREATOR.ToLower().Contains(KEY_WORD) ||
                        o.MODIFIER.ToLower().Contains(KEY_WORD) ||
                        o.MATERIAL_TYPE_CODE.ToLower().Contains(KEY_WORD) ||
                        o.MATERIAL_TYPE_NAME.ToLower().Contains(KEY_WORD) ||
                        (o.PACKAGE_NUMBER != null && o.PACKAGE_NUMBER.ToLower().Contains(KEY_WORD)) ||
                        o.MEDI_STOCK_PERIOD_NAME.ToLower().Contains(KEY_WORD) ||
                        (o.NATIONAL_NAME != null && o.NATIONAL_NAME.ToLower().Contains(KEY_WORD)) ||
                        o.SERVICE_UNIT_CODE.ToLower().Contains(KEY_WORD) ||
                        o.SERVICE_UNIT_NAME.ToLower().Contains(KEY_WORD)
                        );
                }

                if (chkHienDongLoi.Checked)
                {
                    mestPeriodMatys = mestPeriodMatys.Where(o => o.IN_AMOUNT < 0 || o.OUT_AMOUNT < 0 || o.BEGIN_AMOUNT < 0 || o.VIR_END_AMOUNT < 0 || o.INVENTORY_AMOUNT < 0 || o.VIR_END_AMOUNT != o.INVENTORY_AMOUNT);
                }
                //else
                //{
                //    mestPeriodMatys = mestPeriodMatys.Where(o => o.IN_AMOUNT >= 0 && o.OUT_AMOUNT >= 0 && o.BEGIN_AMOUNT >= 0 && o.VIR_END_AMOUNT >= 0 && o.INVENTORY_AMOUNT >= 0 && o.VIR_END_AMOUNT == o.INVENTORY_AMOUNT);
                //}
                if (chkKhongHienDongHet.Checked)
                {
                    mestPeriodMatys = mestPeriodMatys.Where(o => o.VIR_END_AMOUNT == 0);
                }
                //else
                //{
                //    mestPeriodMatys = mestPeriodMatys.Where(o => o.VIR_END_AMOUNT != 0);
                //}
                if (chkKhongHienDongTangGiam.Checked)
                {
                    mestPeriodMatys = mestPeriodMatys.Where(o => o.BEGIN_AMOUNT == o.VIR_END_AMOUNT);
                }
                //else
                //{
                //    mestPeriodMatys = mestPeriodMatys.Where(o => o.BEGIN_AMOUNT != o.VIR_END_AMOUNT);
                //}
                if (chkHienThiDongLoiKhongQuanTamTonKho.Checked)
                {
                    mestPeriodMatys = mestPeriodMatys.Where(o => o.IN_AMOUNT < 0 || o.OUT_AMOUNT < 0 || o.BEGIN_AMOUNT < 0 || o.VIR_END_AMOUNT < 0);
                }
                //else
                //{
                //    mestPeriodMatys = mestPeriodMatys.Where(o => o.IN_AMOUNT >= 0 && o.OUT_AMOUNT >= 0 && o.BEGIN_AMOUNT >= 0 && o.VIR_END_AMOUNT >= 0);
                //}
                gridControlMaterial.BeginUpdate();
                gridControlMaterial.DataSource = mestPeriodMatys.OrderBy(o => o.MATERIAL_TYPE_NAME).ToList();
                gridControlMaterial.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBlood(MediStockPeriodADO hisMediStockPeriod)
        {
            try
            {
                return;
                WaitingManager.Show();
                gridControlBlood.DataSource = null;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisMestPeriodBloodFilter mestPeriodBltyFilter = new MOS.Filter.HisMestPeriodBloodFilter();
                mestPeriodBltyFilter.KEY_WORD = txtSearchBlood.Text.Trim();
                //if (chkHienThiDongLoiKhongQuanTamTonKho.Checked)
                //    mestPeriodBltyFilter.IS_ERROR_NOT_INVENTORY = true;
                //if (chkHienDongLoi.Checked)
                //    mestPeriodBltyFilter.IS_ERROR = true;
                //if (chkKhongHienDongHet.Checked)
                //    mestPeriodBltyFilter.IS_EMPTY = false;
                //if (chkKhongHienDongTangGiam.Checked)
                //    mestPeriodBltyFilter.IS_NO_CHANGE = false;
                mestPeriodBltyFilter.ORDER_DIRECTION = "ASC";
                mestPeriodBltyFilter.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                mestPeriodBltyFilter.MEDI_STOCK_PERIOD_ID = hisMediStockPeriod.ID;

                ListMestPeriodBloods = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HisMestPeriodBloodAdo>>(RequestUriStore.HIS_MEST_PERIOD_BLOOD_GETVIEW, ApiConsumers.MosConsumer, mestPeriodBltyFilter, paramCommon);
                if (ListMestPeriodBloods != null && ListMestPeriodBloods.Count > 0)
                {
                    var bloods = BackendDataWorker.Get<V_HIS_BLOOD>();
                    var services = BackendDataWorker.Get<V_HIS_SERVICE>();
                    foreach (var item in ListMestPeriodBloods)
                    {
                        var blood = bloods != null ? bloods.Where(o => o.ID == item.BLOOD_ID).FirstOrDefault() : null;
                        var service = services != null ? services.Where(o => o.ID == blood.SERVICE_ID).FirstOrDefault() : null;
                        if (blood != null)
                        {
                            item.BLOOD_TYPE_CODE = blood.BLOOD_TYPE_CODE;
                            item.BLOOD_TYPE_NAME = blood.BLOOD_TYPE_NAME;
                            item.BLOOD_TYPE_ID = blood.BLOOD_TYPE_ID;
                            item.VOLUME = blood.VOLUME;
                            //item.SERVICE_UNIT_CODE = service.SERVICE_UNIT_NAME;
                            //item.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                            item.PACKAGE_NUMBER = blood.PACKAGE_NUMBER;
                            item.EXPIRED_DATE = blood.EXPIRED_DATE;
                            //item.KK_AMOUNT = blood.INVENTORY_AMOUNT;
                        }
                    }
                }
                gridControlBlood.BeginUpdate();
                gridControlBlood.DataSource = ListMestPeriodBloods;
                gridControlBlood.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodByFilter()
        {
            try
            {
                WaitingManager.Show();
                gridControlBlood.DataSource = null;
                string KEY_WORD = txtSearchBlood.Text.Trim();

                var mestPeriodBloods = this.ListMestPeriodBloods.AsQueryable();

                if (!String.IsNullOrEmpty(KEY_WORD))
                {
                    KEY_WORD = KEY_WORD.ToLower().Trim();
                    mestPeriodBloods = mestPeriodBloods.Where(o =>
                        o.CREATOR.ToLower().Contains(KEY_WORD) ||
                        o.MODIFIER.ToLower().Contains(KEY_WORD) ||
                        o.BLOOD_TYPE_CODE.ToLower().Contains(KEY_WORD) ||
                        o.BLOOD_TYPE_NAME.ToLower().Contains(KEY_WORD) ||
                        (o.PACKAGE_NUMBER != null && o.PACKAGE_NUMBER.ToLower().Contains(KEY_WORD)) ||
                        (o.SERVICE_UNIT_CODE != null && o.SERVICE_UNIT_CODE.ToLower().Contains(KEY_WORD)) ||
                        (o.SERVICE_UNIT_NAME != null && o.SERVICE_UNIT_NAME.ToLower().Contains(KEY_WORD))
                        );
                }

                if (chkHienDongLoi.Checked)
                {
                    mestPeriodBloods = mestPeriodBloods.Where(o => o.IN_AMOUNT < 0 || o.OUT_AMOUNT < 0 || o.BEGIN_AMOUNT < 0 || o.VIR_END_AMOUNT < 0 || o.INVENTORY_AMOUNT < 0 || o.VIR_END_AMOUNT != o.INVENTORY_AMOUNT);
                }

                if (chkKhongHienDongHet.Checked)
                {
                    mestPeriodBloods = mestPeriodBloods.Where(o => o.VIR_END_AMOUNT == 0);
                }

                if (chkKhongHienDongTangGiam.Checked)
                {
                    mestPeriodBloods = mestPeriodBloods.Where(o => o.BEGIN_AMOUNT == o.VIR_END_AMOUNT);
                }

                if (chkHienThiDongLoiKhongQuanTamTonKho.Checked)
                {
                    mestPeriodBloods = mestPeriodBloods.Where(o => o.IN_AMOUNT < 0 || o.OUT_AMOUNT < 0 || o.BEGIN_AMOUNT < 0 || o.VIR_END_AMOUNT < 0);
                }

                gridControlBlood.BeginUpdate();
                gridControlBlood.DataSource = mestPeriodBloods.ToList();
                gridControlBlood.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
