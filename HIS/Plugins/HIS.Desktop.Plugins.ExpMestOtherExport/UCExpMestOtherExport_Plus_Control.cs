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
using HIS.UC.ExpMestMedicineGrid;
using MOS.SDO;
using HIS.UC.ExpMestMedicineGrid.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ExpMestOtherExport.ADO;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.ExpMestOtherExport
{
    public partial class UCExpMestOtherExport : HIS.Desktop.Utility.UserControlBase
    {
        private void cboSampleForm_Closed(object sender, ClosedEventArgs e)
        {
            //try
            //{
            //    if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
            //    {
            //        if (cboSampleForm.EditValue != null)
            //        {
            //            var template = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().First(o => o.ID == Convert.ToInt64(cboSampleForm.EditValue));
            //            if (template != null)
            //            {
            //                txtSampleForm.Text = template.EXP_MEST_TEMPLATE_CODE;
            //                FillDataToGridDetailByTemplate(template);
            //            }
            //        }
            //        txtDescription.Focus();
            //        txtDescription.SelectAll();
            //        cboSampleForm.Properties.Buttons[1].Visible = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void txtSampleForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        bool valid = false;
            //        if (!String.IsNullOrEmpty(txtSampleForm.Text))
            //        {
            //            string key = txtSampleForm.Text.Trim().ToLower();
            //            var listData = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().Where(o => o.EXP_MEST_TEMPLATE_CODE.ToLower().Contains(key) || o.EXP_MEST_TEMPLATE_NAME.ToLower().Contains(key)).ToList();
            //            if (listData != null && listData.Count == 1)
            //            {
            //                valid = true;
            //                txtSampleForm.Text = listData.First().EXP_MEST_TEMPLATE_CODE;
            //                cboSampleForm.EditValue = listData.First().ID;
            //                FillDataToGridDetailByTemplate(listData.First());
            //                txtDescription.Focus();
            //                txtDescription.SelectAll();
            //            }
            //        }
            //        if (!valid)
            //        {
            //            cboSampleForm.Focus();
            //            cboSampleForm.ShowPopup();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void FillDataToGridDetailByTemplate(HIS_EXP_MEST_TEMPLATE data)
        {
            try
            {
                if (data != null)
                {
                    dicTypeAdo.Clear();
                    HisEmteMedicineTypeViewFilter medicineFilter = new HisEmteMedicineTypeViewFilter();
                    medicineFilter.EXP_MEST_TEMPLATE_ID = data.ID;
                    var listMedicineTem = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MEDICINE_TYPE>>(HisRequestUriStore.HIS_EMTE_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    HisEmteMaterialTypeViewFilter materialFilter = new HisEmteMaterialTypeViewFilter();
                    materialFilter.EXP_MEST_TEMPLATE_ID = data.ID;
                    var listMaterialTem = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MATERIAL_TYPE>>(HisRequestUriStore.HIS_EMTE_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    if (listMedicineTem != null && listMedicineTem.Count > 0)
                    {
                        foreach (var item in listMedicineTem)
                        {
                            HisMedicineInStockSDO medicineInStockSDO = listMediInStock.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);
                            if (!dicTypeAdo.ContainsKey(TYPE_MEDICINE))
                                dicTypeAdo[TYPE_MEDICINE] = new Dictionary<long, MediMateTypeADO>();
                            var dic = dicTypeAdo[TYPE_MEDICINE];
                            if (medicineInStockSDO != null)
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(medicineInStockSDO);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.TUTORIAL = item.TUTORIAL;
                                ado.ExpMedicine.Amount = ado.EXP_AMOUNT;
                                dic[ado.MEDI_MATE_TYPE_ID] = ado;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO();
                                ado.SERVICE_ID = item.SERVICE_ID ?? 0;
                                ado.MEDI_MATE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                //ado.CONCENTRA = item.CONCENTRA;
                                ado.MEDI_MATE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.MEDI_MATE_TYPE_ID = item.MEDICINE_TYPE_ID ?? 0;
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.IsMedicine = true;
                                ado.IsNotHasMest = true;
                                dic[ado.MEDI_MATE_TYPE_ID] = ado;
                            }
                        }
                    }

                    if (listMaterialTem != null && listMaterialTem.Count > 0)
                    {
                        foreach (var item in listMaterialTem)
                        {
                            HisMaterialInStockSDO materialInStockSDO = listMateInStock.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID);
                            if (!dicTypeAdo.ContainsKey(TYPE_MATERIAL))
                                dicTypeAdo[TYPE_MATERIAL] = new Dictionary<long, MediMateTypeADO>();
                            var dic = dicTypeAdo[TYPE_MEDICINE];

                            if (materialInStockSDO != null)
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(materialInStockSDO);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.ExpMaterial.Amount = ado.EXP_AMOUNT;
                                if (ado.EXP_AMOUNT > ado.AVAILABLE_AMOUNT)
                                {
                                    ado.IsGreatAvailable = true;
                                }
                                dic[ado.MEDI_MATE_TYPE_ID] = ado;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO();
                                ado.SERVICE_ID = item.SERVICE_ID ?? 0;
                                ado.MEDI_MATE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.MEDI_MATE_TYPE_ID = item.MATERIAL_TYPE_ID ?? 0;
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.IsMedicine = false;
                                ado.IsNotHasMest = true;
                                dic[ado.MEDI_MATE_TYPE_ID] = ado;
                            }
                        }
                    }
                    xtraTabControlExpMediMate.SelectedTabPage = xtraTabPageExpMediMate;
                    FillDataGridExpMestDetail();
                    gridControlExpBlood.BeginUpdate();
                    gridControlExpBlood.DataSource = null;
                    gridControlExpBlood.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
