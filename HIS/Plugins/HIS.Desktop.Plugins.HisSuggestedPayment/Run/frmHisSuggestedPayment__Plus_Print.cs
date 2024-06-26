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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisSuggestedPayment.Run
{
    public partial class frmHisSuggestedPayment : HIS.Desktop.Utility.FormBase
    {
        public enum PrintType
        {
            IN__BIEN_BAN_NGHIEM_THU,
            IN__DE_NGHI_THANH_TOAN
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN__DE_NGHI_THANH_TOAN:
                        richEditorMain.RunPrintTemplate("Mps000327", DelegateRunPrinter);
                        break;
                    case PrintType.IN__BIEN_BAN_NGHIEM_THU:
                        richEditorMain.RunPrintTemplate("Mps000328", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000327":
                        Mps000327(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000328":
                        Mps000328(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000327(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if ((this._ImpMestPropose_ == null || this._ImpMestPropose_.Count <= 0) && this._ImpMestPropose == null)
                {
                    return;
                }
                else if (this._ImpMestPropose_ == null || this._ImpMestPropose_.Count <= 0)
                {
                    this._ImpMestPropose_ = new List<V_HIS_IMP_MEST_PROPOSE>();
                    this._ImpMestPropose_.Add(this._ImpMestPropose);
                }

                foreach (var item in _ImpMestPropose_)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
                    if (this._HisImpMests == null)
                        this._HisImpMests = new List<V_HIS_IMP_MEST>();
                    if (this._HisImpMestPays == null)
                        this._HisImpMestPays = new List<V_HIS_IMP_MEST_PAY>();
                    var HisImpMests = this._HisImpMests.Where(o => o.IMP_MEST_PROPOSE_ID == item.ID).ToList();
                    var HisImpMestPays = this._HisImpMestPays.Where(o => o.IMP_MEST_PROPOSE_ID == item.ID).ToList();
                    MPS.Processor.Mps000327.PDO.Mps000327PDO mps000327RDO = new MPS.Processor.Mps000327.PDO.Mps000327PDO(
                   item,
                    HisImpMests,
                    HisImpMestPays
                    );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);


                    WaitingManager.Hide();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Mps000328(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if ((this._ImpMestPropose_ == null || this._ImpMestPropose_.Count <= 0) && this._ImpMestPropose == null)
                {
                    return;
                }
                else if (this._ImpMestPropose_ == null || this._ImpMestPropose_.Count <= 0)
                {
                    this._ImpMestPropose_ = new List<V_HIS_IMP_MEST_PROPOSE>();
                    this._ImpMestPropose_.Add(this._ImpMestPropose);
                }

                foreach (var item in _ImpMestPropose_)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    List<HIS_SUPPLIER> _HIS_SUPPLIERs = new List<HIS_SUPPLIER>();
                    _HIS_SUPPLIERs.Add(BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(p => p.ID == item.SUPPLIER_ID));

                    List<HIS_BRANCH> _HIS_BRANCHs = new List<HIS_BRANCH>();
                    _HIS_BRANCHs.Add(BranchDataWorker.Branch);

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
                    if (this._HisImpMests == null)
                        this._HisImpMests = new List<V_HIS_IMP_MEST>();
                    if (this._HisImpMestPays == null)
                        this._HisImpMestPays = new List<V_HIS_IMP_MEST_PAY>();

                    List<V_HIS_IMP_MEST_MEDICINE> _Medicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                    List<V_HIS_IMP_MEST_MATERIAL> _Materials = new List<V_HIS_IMP_MEST_MATERIAL>();
                    if (this._HisImpMests != null && this._HisImpMests.Count > 0)
                    {
                        List<long> _impMestIds = this._HisImpMests.Select(p => p.ID).Distinct().ToList();
                        MOS.Filter.HisImpMestMedicineViewFilter _mediFIlter = new MOS.Filter.HisImpMestMedicineViewFilter();
                        _mediFIlter.IMP_MEST_IDs = _impMestIds;
                        _mediFIlter.ORDER_DIRECTION = "MEDICINE_TYPE_CODE";
                        _mediFIlter.ORDER_FIELD = "ASC";

                        _Medicines = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, _mediFIlter, null);


                        MOS.Filter.HisImpMestMaterialViewFilter _mateFIlter = new MOS.Filter.HisImpMestMaterialViewFilter();
                        _mateFIlter.IMP_MEST_IDs = _impMestIds;
                        _mateFIlter.ORDER_DIRECTION = "MATERIAL_TYPE_CODE";
                        _mateFIlter.ORDER_FIELD = "ASC";
                        _Materials = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, _mateFIlter, null);
                    }

                    var HisImpMests = this._HisImpMests.Where(o => o.IMP_MEST_PROPOSE_ID == item.ID).ToList();
                    var HisImpMestPays = this._HisImpMestPays.Where(o => o.IMP_MEST_PROPOSE_ID == item.ID).ToList();
                    MPS.Processor.Mps000328.PDO.Mps000328PDO mps000328RDO = new MPS.Processor.Mps000328.PDO.Mps000328PDO(
                  item,
                    HisImpMests,
                    HisImpMestPays,
                    _HIS_SUPPLIERs,
                    _HIS_BRANCHs,
                    _Medicines,
                    _Materials
                    );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000328RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000328RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);


                    WaitingManager.Hide();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
