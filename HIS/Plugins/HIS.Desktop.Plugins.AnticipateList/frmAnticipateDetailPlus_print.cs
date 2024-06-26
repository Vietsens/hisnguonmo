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
using MOS.EFMODEL.DataModels;
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using Inventec.Core;
using Inventec.Common.Adapter;

namespace HIS.Desktop.Plugins.AnticipateList
{
    public partial class frmAnticipateDetail : HIS.Desktop.Utility.FormBase
    {
        //V_HIS_ANTICIPATE resultAnticipate;
        long? treatmentId = null;
        //void initButtonPrint(V_HIS_ANTICIPATE resultAnticipate_1)
        //{
        //    try
        //    {
        //        resultAnticipate = resultAnticipate_1;
        //        DevExpress.Utils.Menu.DXPopupMenu menu = new DXPopupMenu();
        //        menu.Items.Add(new DXMenuItem("In phiếu dự trù thuốc", new EventHandler(OnClickInPhieuDuTruThuoc)));
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        //private void OnClickInPhieuDuTruThuoc(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
        //        store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuDuTru__MPS000117, DeletegatePrintTemplate);

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
            private bool DeletegatePrintTemplate(string printCode, string fileName)
            {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuDuTru__MPS000117:
                        InPhieuDuTruThuoc(printCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void InPhieuDuTruThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.anticipate == null)
                    return;
                WaitingManager.Show();

                anticipateMetiePrints = new List<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>();
                // Thuốc
                HisAnticipateMetyViewFilter ssFilter = new HisAnticipateMetyViewFilter();
                ssFilter.ANTICIPATE_ID = this.anticipate.ID;
                var listAnticipateMety = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_ANTICIPATE_METY>>(HisRequestUriStore.HIS_ANTICIPATE_METY_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                // Vật tư
                HisAnticipateMatyViewFilter hisAnticipateMatyViewFilter = new HisAnticipateMatyViewFilter();
                hisAnticipateMatyViewFilter.ANTICIPATE_ID = this.anticipate.ID;
                var listAnticipateMaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_ANTICIPATE_MATY>>(HisRequestUriStore.HIS_ANTICIPATE_MATY_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                // Máu
                HisAnticipateBltyViewFilter hisbloodviewFilter = new HisAnticipateBltyViewFilter();
                hisbloodviewFilter.ANTICIPATE_ID = this.anticipate.ID;
                var listBloodType = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_BLTY>>(HisRequestUriStore.HIS_ANTICIPATE_BLTY_GETVIEW, ApiConsumers.MosConsumer, hisbloodviewFilter, null);
                if (listAnticipateMety != null && listAnticipateMety.Count > 0)
                {
                    var Groups = listAnticipateMety.GroupBy(o => o.MEDICINE_TYPE_ID).ToList();
                    
                    foreach (var group in Groups)
                    {
                        var listSub = group.ToList<V_HIS_ANTICIPATE_METY>();
                        foreach (var item in listSub)
                        {
                            MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO aAnticipateMety = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(aAnticipateMety, item);
                            aAnticipateMety.TotalMoney = item.AMOUNT * (item.IMP_PRICE ?? 0);
                            anticipateMetiePrints.Add(aAnticipateMety);
                        }
                    }
                }

                if (listAnticipateMaty != null && listAnticipateMaty.Count > 0)
                {
                    var Groups = listAnticipateMaty.GroupBy(o => o.MATERIAL_TYPE_ID).ToList();

                    foreach (var group in Groups)
                    {
                        var listSub = group.ToList<V_HIS_ANTICIPATE_MATY>();
                        foreach (var item in listSub)
                        {
                            MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO aAnticipateMety = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(aAnticipateMety, item);
                            aAnticipateMety.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                            aAnticipateMety.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                            aAnticipateMety.TotalMoney = item.AMOUNT * (item.IMP_PRICE ?? 0);
                            anticipateMetiePrints.Add(aAnticipateMety);
                        }

                    }
                }

                if (listBloodType != null && listBloodType.Count > 0)
                {
                    var Groups = listBloodType.GroupBy(o => o.BLOOD_TYPE_ID).ToList();

                    foreach (var group in Groups)
                    {
                        var listSub = group.ToList<V_HIS_ANTICIPATE_BLTY>();
                        foreach (var item in listSub)
                        {
                            MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO aAnticipateMety = new MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000117.PDO.HisAnticipateMetyADO>(aAnticipateMety, item);
                            aAnticipateMety.MEDICINE_TYPE_CODE = item.BLOOD_TYPE_CODE;
                            aAnticipateMety.MEDICINE_TYPE_NAME = item.BLOOD_TYPE_NAME;
                            aAnticipateMety.TotalMoney = item.AMOUNT * (item.IMP_PRICE ?? 0);
                            anticipateMetiePrints.Add(aAnticipateMety);
                        }

                    }
                }
                MPS.Processor.Mps000117.PDO.Mps000117PDO rdo = new MPS.Processor.Mps000117.PDO.Mps000117PDO(this.anticipate, anticipateMetiePrints);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData( printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show,""));
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
