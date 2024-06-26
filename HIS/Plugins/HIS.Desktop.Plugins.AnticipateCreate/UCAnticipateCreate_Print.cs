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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.AnticipateCreate
{
    public partial class UCAnticipateCreate : HIS.Desktop.Utility.UserControlBase
    {
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || anticipateModel == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditor.RunPrintTemplate(MPS.Processor.Mps000117.PDO.Mps000117PDO.printTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                if (anticipatePrint != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE dataPrint = new MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE();
                    MOS.Filter.HisAnticipateViewFilter filter = new MOS.Filter.HisAnticipateViewFilter();
                    filter.ID = anticipatePrint.ID;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE>>(ApiConsumer.HisRequestUriStore.HIS_ANTICIPATE_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());
                    dataPrint = apiresult.FirstOrDefault();
                    MPS.Processor.Mps000117.PDO.Mps000117PDO mps117Rdo = new MPS.Processor.Mps000117.PDO.Mps000117PDO(dataPrint, loadDataAfferSaveSuccess(anticipatePrint));

                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    WaitingManager.Hide();
                    string printerName = "";

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps117Rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps117Rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    result = MPS.MpsPrinter.Run(PrintData);


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
            return result;
        }

        private List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO> loadDataAfferSaveSuccess(MOS.EFMODEL.DataModels.HIS_ANTICIPATE anticipate)
        {
            CommonParam param = new CommonParam();
            List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO> anticipateMetyAdos = new List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>();
            try
            {
                if (anticipate == null) return new List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>();

                if (anticipate.HIS_ANTICIPATE_METY != null && anticipate.HIS_ANTICIPATE_METY.Count > 0)
                {
                    MOS.Filter.HisMedicineStockViewFilter mediFilter = new MOS.Filter.HisMedicineStockViewFilter();
                    mediFilter.MEDI_STOCK_ID = this.mediStockId;
                    var lstMediInStocks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.SDO.HisMedicineInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    HisAnticipateMetyViewFilter filter = new HisAnticipateMetyViewFilter();
                    filter.IDs = anticipate.HIS_ANTICIPATE_METY.Select(o => o.ID).ToList();
                    var listAnticipateMety = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_METY>>("api/HisAnticipateMety/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (listAnticipateMety != null && listAnticipateMety.Count > 0)
                    {
                        foreach (var item in listAnticipateMety)
                        {
                            var metyInStock = lstMediInStocks.FirstOrDefault(o => o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);
                            MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO ado = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(ado, item);
                            ado.Type = Base.GlobalConfig.THUOC;
                            ado.TotalMoney = (ado.AMOUNT) * (ado.IMP_PRICE ?? 0);
                            ado.IN_STOCK_AMOUNT = metyInStock != null && metyInStock.TotalAmount.HasValue ? metyInStock.TotalAmount.Value : 0;
                            anticipateMetyAdos.Add(ado);
                        }
                    }
                }

                if (anticipate.HIS_ANTICIPATE_MATY != null && anticipate.HIS_ANTICIPATE_MATY.Count > 0)
                {
                    MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                    mateFilter.MEDI_STOCK_ID = this.mediStockId;
                    var lstMateInStocks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.SDO.HisMaterialInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    HisAnticipateMatyViewFilter filter = new HisAnticipateMatyViewFilter();
                    filter.IDs = anticipate.HIS_ANTICIPATE_MATY.Select(o => o.ID).ToList();
                    var listAnticipateMaty = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_MATY>>("api/HisAnticipateMaty/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (listAnticipateMaty != null && listAnticipateMaty.Count > 0)
                    {
                        foreach (var item in listAnticipateMaty)
                        {
                            MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO ado = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(ado, item);
                            var matyInStock = lstMateInStocks.FirstOrDefault(o => o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID);
                            ado.Type = Base.GlobalConfig.VATTU;
                            ado.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                            ado.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                            ado.TotalMoney = (ado.AMOUNT) * (ado.IMP_PRICE ?? 0);
                            ado.IN_STOCK_AMOUNT = matyInStock != null && matyInStock.TotalAmount.HasValue ? matyInStock.TotalAmount.Value : 0;
                            anticipateMetyAdos.Add(ado);
                        }
                    }
                }

                if (anticipate.HIS_ANTICIPATE_BLTY != null && anticipate.HIS_ANTICIPATE_BLTY.Count > 0)
                {
                    MOS.Filter.HisBloodTypeStockViewFilter filter = new MOS.Filter.HisBloodTypeStockViewFilter();
                    filter.MEDI_STOCK_ID = this.mediStockId;
                    var lstBlood = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.SDO.HisBloodTypeInStockSDO>>(HisRequestUriStore.HIS_BLOOD_TYPE_GETVIEW_BY_IN_STOCK, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    foreach (var item in anticipate.HIS_ANTICIPATE_BLTY)
                    {
                        var blty = Base.GlobalConfig.HisBloodTypes.FirstOrDefault(o => o.ID == item.BLOOD_TYPE_ID);
                        if (blty != null)
                        {
                            var ado = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO(item, blty);
                            var bltyInStock = lstBlood.FirstOrDefault(o => o.BloodTypeCode == blty.BLOOD_TYPE_CODE);
                            var supplier = Base.GlobalConfig.ListSupplier.FirstOrDefault(o => o.ID == item.SUPPLIER_ID);
                            ado.Type = Base.GlobalConfig.MAU;
                            ado.TotalMoney = ado.AMOUNT * (ado.IMP_PRICE ?? 0);
                            ado.IN_STOCK_AMOUNT = bltyInStock != null && bltyInStock.Amount.HasValue ? bltyInStock.Amount.Value : 0;
                            if (supplier != null)
                            {
                                ado.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                                ado.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                            }
                            anticipateMetyAdos.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>();
            }
            return anticipateMetyAdos;
        }
    }
}
