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
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using MOS.SDO;

namespace HIS.Desktop.Plugins.ExpMestBCSCreate.Run
{
    public partial class UCExpMestBCSCreate : HIS.Desktop.Utility.UserControlBase
    {
        HIS_EXP_MEST _ExpMestPrint { get; set; }

        private void onClickPrintPhieuXuatBCS(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("MPS000215", delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode))
                {
                    switch (printTypeCode)
                    {
                        case "MPS000215":
                            InPhieuXuatBCS(ref result, printTypeCode, fileName);
                            break;
                        case "MPS000254":
                            MPS000254(ref result, printTypeCode, fileName);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TCs { get; set; }


        V_HIS_EXP_MEST _BcsExpMest { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<HIS_TREATMENT> ListTreatment { get; set; }
        HisExpMestBcsMoreInfoSDO MoreInfo { get; set; }

        private void InPhieuXuatBCS(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                #region TT Chung
                WaitingManager.Show();
                HisExpMestViewFilter bcsFilter = new HisExpMestViewFilter();
                bcsFilter.ID = this._ExpMestPrint.ID;
                var listBcsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, bcsFilter, null);
                if (listBcsExpMest == null || listBcsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                _BcsExpMest = listBcsExpMest.First();

                CommonParam param = new CommonParam();

                _ExpMestMetyReqs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReqs = new List<HIS_EXP_MEST_MATY_REQ>();

                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                ListTreatment = new List<HIS_TREATMENT>();
                List<long> treatmentIds = new List<long>();
                MoreInfo = null;

                if (_BcsExpMest != null)
                {
                    MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                    metyReqFilter.EXP_MEST_ID = _BcsExpMest.ID;
                    _ExpMestMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/get", ApiConsumers.MosConsumer, metyReqFilter, param);
                    treatmentIds.AddRange(_ExpMestMetyReqs.Select(s => s.TREATMENT_ID ?? 0).ToList());

                    MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                    matyReqFilter.EXP_MEST_ID = _BcsExpMest.ID;
                    _ExpMestMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/get", ApiConsumers.MosConsumer, matyReqFilter, param);
                    treatmentIds.AddRange(_ExpMestMatyReqs.Select(s => s.TREATMENT_ID ?? 0).ToList());

                    if (_BcsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                        || _BcsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        mediFilter.EXP_MEST_ID = _BcsExpMest.ID;
                        _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                        MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                        matyFilter.EXP_MEST_ID = _BcsExpMest.ID;
                        _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    }

                    if (treatmentIds != null && treatmentIds.Count > 0)
                    {
                        int skip = 0;
                        while (treatmentIds.Count - skip > 0)
                        {
                            var listIds = treatmentIds.Skip(skip).Take(100).ToList();
                            skip += 100;

                            MOS.Filter.HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                            treatFilter.IDs = listIds;
                            var treat = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, param);
                            if (treat != null && treat.Count > 0)
                            {
                                ListTreatment.AddRange(treat);
                            }
                        }
                    }

                    if (_BcsExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES
                        || _BcsExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL)
                    {
                        HisExpMestBcsMoreInfoFilter moreFilter = new HisExpMestBcsMoreInfoFilter();
                        moreFilter.BCS_EXP_MEST_ID = _BcsExpMest.ID;
                        MoreInfo = new BackendAdapter(new CommonParam()).Get<HisExpMestBcsMoreInfoSDO>("api/HisExpMest/GetBcsMoreInfo", ApiConsumers.MosConsumer, moreFilter, null);
                    }
                }
                #endregion

                {
                    _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();

                    List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();

                    #region --- Xu Ly Tach GN_HT -----
                    if (_ExpMestMetyReqs != null && _ExpMestMetyReqs.Count > 0)
                    {
                        var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                        var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                        bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                        bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                        bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                        bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                        bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                        bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                        bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                        bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                        bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);


                        var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var item in _ExpMestMetyReqs)
                        {
                            var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (dataMedi != null)
                            {
                                if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_GNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_HTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                {
                                    _ExpMestMetyReq_TDs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                {
                                    _ExpMestMetyReq_PXs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                {
                                    _ExpMestMetyReq_COs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                {
                                    _ExpMestMetyReq_DTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                {
                                    _ExpMestMetyReq_KSs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                {
                                    _ExpMestMetyReq_LAOs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                                {
                                    _ExpMestMetyReq_TCs.Add(item);
                                }
                                else
                                {
                                    _ExpMestMetyReq_Ts.Add(item);
                                }
                            }
                        }
                    }
                    #endregion

                    WaitingManager.Hide();
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate("MPS000254", delegatePrintTemplate);

                    #region ----VatTu----
                    if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 null,
                 _ExpMestMaterials,
                 null,
                 _ExpMestMatyReqs,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.vattu,
                  ListTreatment,
                  MoreInfo
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    #endregion

                    #region ----- Thuong ----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 _ExpMestMedicines,
                 null,
                 _ExpMestMetyReq_Ts,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.thuong,
                  ListTreatment,
                  MoreInfo
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ----- TC ----
                    if (_ExpMestMetyReq_TCs != null && _ExpMestMetyReq_TCs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 _ExpMestMedicines,
                 null,
                 _ExpMestMetyReq_TCs,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.tienchat,
                  ListTreatment,
                  MoreInfo
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MPS000254(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- GOP GN HT -----
                    if (_ExpMestMetyReq_GN_HTs != null && _ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_GN_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.tonghop,
                 ListTreatment,
                  MoreInfo
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_GNs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.gaynghien,
                 ListTreatment,
                  MoreInfo
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- HT -----
                    if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.huongthan,
                 ListTreatment,
                  MoreInfo
                 );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }

                #region ----- TD -----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_TDs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocdoc,
             ListTreatment,
                  MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- PX -----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_PXs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocphongxa,
             ListTreatment,
                  MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO -----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_COs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Corticoid,
             ListTreatment,
                  MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT -----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_DTs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.DichTruyen,
             ListTreatment,
                  MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS -----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_KSs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.KhangSinh,
             ListTreatment,
                  MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- lao -----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this._Module.RoomId);
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_LAOs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Lao,
             ListTreatment,
                  MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
