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
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ExpMestSaleCreateV2.Config;

namespace HIS.Desktop.Plugins.ExpMestSaleCreateV2
{
    public partial class UCExpMestSaleCreateV2 : UserControl
    {
        private void InitMenuPrint(MOS.EFMODEL.DataModels.HIS_EXP_MEST expMest)
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                if (HisConfigCFG.IS_MUST_BE_FINISHED_BEFORED_PRINTING)
                {
                    if (expMest != null && expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        menu.Items.Add(new DXMenuItem("In phiếu xuất bán", new EventHandler(onClickInPhieuXuatBan)));
                }
                else
                {
                    menu.Items.Add(new DXMenuItem("In phiếu xuất bán", new EventHandler(onClickInPhieuXuatBan)));
                }
                menu.Items.Add(new DXMenuItem("Hướng dẫn sử dụng thuốc", new EventHandler(onClickInHuongDanSuDung)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickInPhieuXuatBan(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092, deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHuongDanSuDung(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeWorker.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092:
                            InPhieuXuatBan(printTypeCode, fileName);
                            break;
                        case PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099:
                            InHuongDanSuDungThuoc(printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuXuatBan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                long expMestIdTemp = (this.expMestResult != null && this.expMestResult.ExpMest != null) ? this.expMestResult.ExpMest.ID : (expMestId.HasValue ? (expMestId ?? 0) : 0);

                if (expMestIdTemp == 0)
                {
                    return;
                }
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = expMestIdTemp;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                if (HisConfigCFG.IS_MUST_BE_FINISHED_BEFORED_PRINTING)
                {
                    if (expMests != null && expMests.Count > 0 && expMests.FirstOrDefault().EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Phiếu chưa thực xuất");
                        return;
                    }
                }

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_ID = expMestIdTemp;
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_ID = expMestIdTemp;
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                V_HIS_TRANSACTION transaction = new V_HIS_TRANSACTION();
                List<V_HIS_IMP_MEST> impMests = null;
                if (expMests.FirstOrDefault().BILL_ID.HasValue)
                {
                    HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    tranFilter.ID = expMests.FirstOrDefault().BILL_ID;
                    var lstTran = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetVIew", ApiConsumers.MosConsumer, tranFilter, param);
                    if (lstTran != null && lstTran.Count > 0)
                    {
                        transaction = lstTran.FirstOrDefault();
                    }


                    HisImpMestFilter impMestFilter = new HisImpMestFilter();
                    impMestFilter.MOBA_EXP_MEST_ID = expMests.FirstOrDefault().ID;
                    impMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param);
                }

                var hisCashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
                var treatment = new V_HIS_TREATMENT();
                if (expMests.FirstOrDefault().TDL_TREATMENT_ID.HasValue)
                {
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = expMests.FirstOrDefault().TDL_TREATMENT_ID;
                    var listTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                        treatment = listTreatment.FirstOrDefault();
                }

                WaitingManager.Hide();

                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(expMests, expMestMedicines, expMestMaterials, transaction,
                 hisCashierRoom,
                 treatment,
                 impMests);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHuongDanSuDungThuoc(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                long expMestIdTemp = (this.expMestResult != null && this.expMestResult.ExpMest != null) ? this.expMestResult.ExpMest.ID : expMestId.HasValue ? expMestId ?? 0 : 0;

                if (expMestIdTemp == 0)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = expMestIdTemp;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param).ToList();

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_ID = expMestIdTemp;
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);


                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_ID = expMestIdTemp;
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterial = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);


                MPS.Processor.Mps000099.PDO.Mps000099PDO rdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(expMests, expMestMedicines, expMestMaterial);


                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
