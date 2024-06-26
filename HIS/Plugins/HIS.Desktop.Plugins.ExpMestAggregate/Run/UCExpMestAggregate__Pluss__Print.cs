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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggregate.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggregate
{
    public partial class UCExpMestAggregate : HIS.Desktop.Utility.UserControlBase
    {

        internal void PrintAggregateExpMest(V_HIS_EXP_MEST currentAggExpMest)
        {
            try
            {
                //Review
                this.currentAggrExpMest = currentAggExpMest;
                DevExpress.XtraBars.BarManager barManager1 = new DevExpress.XtraBars.BarManager();
                barManager1.Form = this;

                ExpMestAggregateListPopupMenuProcessor processor = new ExpMestAggregateListPopupMenuProcessor(this.currentAggrExpMest, null, ExpMestAggregateMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExpMestAggregateMouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (e.Item.Tag is ExpMestAggregateListPopupMenuProcessor.PrintType)
                {
                    var moduleType = (ExpMestAggregateListPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuoc:
                            ShowFormFilter(Convert.ToInt64(1));
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTongHop:
                            ShowFormFilter(Convert.ToInt64(2));
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuocGayNghienHuongTT:
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuoc:
                            ShowFormFilter(Convert.ToInt64(3));
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuocTheoBenhNhan:
                            ShowFormFilter(Convert.ToInt64(4));
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhTongHop:
                            ShowFormFilter(Convert.ToInt64(3), true);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuocTongHop:
                            ShowFormFilter(Convert.ToInt64(5), true);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuocTheoBenhNhanTongHop:
                            ShowFormFilter(Convert.ToInt64(4), true);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuHuyThuocVatTu_434:
                            OnClickInPhieuHuyThuocVatTu();
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InTraPhieuDoiThuoc:
                             ShowFormFilter(Convert.ToInt64(7), true);
                            break;
                            
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(long printType, bool selectMulti = false)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) 
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                else if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    //Review
                    List<object> listArgs = new List<object>();
                    if (selectMulti)
                    {
                        List<V_HIS_EXP_MEST> _ExpMestTraDoiChecks = new List<V_HIS_EXP_MEST>();
                        if (gridViewAggrExpMest.RowCount > 0)
                        {
                            for (int i = 0; i < gridViewAggrExpMest.SelectedRowsCount; i++)
                            {
                                if (gridViewAggrExpMest.GetSelectedRows()[i] >= 0)
                                {
                                    _ExpMestTraDoiChecks.Add((V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(gridViewAggrExpMest.GetSelectedRows()[i]));
                                }
                            }
                        }
                        if (_ExpMestTraDoiChecks != null && _ExpMestTraDoiChecks.Count > 0)
                        {
                            listArgs.Add(_ExpMestTraDoiChecks);
                        }
                    }

                    if (chkPrint.Checked)
                    {
                        HIS.Desktop.ADO.AggrExpMestPrintSDO sdo = new HIS.Desktop.ADO.AggrExpMestPrintSDO();
                        sdo.PrintNow = true;
                        listArgs.Add(sdo);
                    }

                    listArgs.Add(this.currentAggrExpMest);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnClickInPhieuHuyThuocVatTu()
        {
            try
            {
                List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();

                CommonParam param = new CommonParam();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.AGGR_EXP_MEST_ID = this.currentAggrExpMest.ID;
                lstExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                var mps000434 = new HIS.Desktop.Plugins.Library.PrintAggrExpMest.PrintAggrExpMestProcessor(lstExpMest);
                mps000434.RoomId = this.currentModule.RoomId;
                if (mps000434 != null)
                {
                    mps000434.Print("Mps000434", false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<V_HIS_PATIENT> patients = new List<V_HIS_PATIENT>();
        List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
        List<V_HIS_BED_LOG> BedLogList = new List<V_HIS_BED_LOG>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqList = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqList = new List<HIS_EXP_MEST_MATY_REQ>();
        List<long> useFormIds = new List<long>();
        List<long> reqRoomIds = new List<long>();
        List<long> serviceUnitIds = new List<long>();
        List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
        HIS_DEPARTMENT department = new HIS_DEPARTMENT();
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        private void OnClickInTraPhieuDoiThuoc(string fileName)
        {
            try
            {

                List<object> listArgs = new List<object>();
                    List<V_HIS_EXP_MEST> _ExpMestTraDoiChecks = new List<V_HIS_EXP_MEST>();
                    if (gridViewAggrExpMest.RowCount > 0)
                    {
                        for (int i = 0; i < gridViewAggrExpMest.SelectedRowsCount; i++)
                        {
                            if (gridViewAggrExpMest.GetSelectedRows()[i] >= 0)
                            {
                                _ExpMestTraDoiChecks.Add((V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(gridViewAggrExpMest.GetSelectedRows()[i]));
                            }
                        }
                    }



                    if (_ExpMestTraDoiChecks != null && _ExpMestTraDoiChecks.Count > 0)
                    {
                        foreach (var currentAggrExpMest in _ExpMestTraDoiChecks)
                        {
                            
                        }
                    }



                CommonParam param = new CommonParam();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.AGGR_EXP_MEST_ID = this.currentAggrExpMest.ID;
                lstExpMest = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);
                reqRoomIds.AddRange(lstExpMest.Select(p => p.REQ_ROOM_ID).ToList());


                department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == currentAggrExpMest.REQ_DEPARTMENT_ID);


                List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> _expMes = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(_expMes, lstExpMest);

                if (lstExpMest != null && lstExpMest.Count > 0)
                {
                    HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                    patientViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    patientViewFilter.IDs = lstExpMest.Select(p => p.TDL_PATIENT_ID ?? 0).ToList(); ;
                    patients = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    HisTreatmentBedRoomViewFilter treatmentBedRoomViewFilter = new HisTreatmentBedRoomViewFilter();
                    treatmentBedRoomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    treatmentBedRoomViewFilter.TREATMENT_IDs = lstExpMest.Select(p => p.TDL_TREATMENT_ID ?? 0).ToList();
                    vHisTreatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentBedRoomViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);


                    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    bedLogFilter.TREATMENT_IDs = lstExpMest.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                    bedLogFilter.TREATMENT_BED_ROOM_IDs = vHisTreatmentBedRooms != null && vHisTreatmentBedRooms.Count > 0
                        ? vHisTreatmentBedRooms.Select(o => o.ID).Distinct().ToList()
                        : null;
                    BedLogList = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, param);
                }
                long keyColumnSize = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.PHIEU_TRA_DOI_THUOC.COLUMN_SIZE"));

                List<MPS.Processor.Mps000047.PDO.Mps000047ADO> listMps000047ADO = new List<MPS.Processor.Mps000047.PDO.Mps000047ADO>();
                CommonParam paramK = new CommonParam();
                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID = this.currentAggrExpMest.ID;
                var dataMedicines = new BackendAdapter(paramK).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramK);
                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    _ExpMestMedicines.AddRange(dataMedicines);
                    serviceUnitIds.AddRange(dataMedicines.Select(p => p.SERVICE_UNIT_ID).ToList());
                    useFormIds.AddRange(dataMedicines.Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                }
                var query = _ExpMestMedicines.ToList();
                if (reqRoomIds != null && reqRoomIds.Count > 0 && lstExpMest != null && lstExpMest.Count > 0)  // lstExpMest  = _ExpMests_Print
                {
                    var expMests = lstExpMest.Where(p => reqRoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                    if (expMests != null && expMests.Count > 0)
                    {
                        query = query.Where(o => expMests.Select(p => p.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                        serviceUnitIds.AddRange(dataMedicines.Select(p => p.SERVICE_UNIT_ID).ToList());
                        useFormIds.AddRange(dataMedicines.Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                    }
                }

                query = query.Where(p => Check(p)).ToList();
                var Groups = query.GroupBy(g => new
                {
                    g.MEDICINE_TYPE_ID,
                    g.EXP_MEST_ID
                }).Select(p => p.ToList()).ToList();

                foreach (var itemGr in Groups)
                {
                    MPS.Processor.Mps000047.PDO.Mps000047ADO ado = new MPS.Processor.Mps000047.PDO.Mps000047ADO();

                    ado.TYPE_ID = 1;
                    if (itemGr[0].IS_EXPEND == 1)
                    {
                        ado.IS_EXPEND_DISPLAY = "X";
                    }
                    ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                    var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemGr[0].MEDICINE_TYPE_ID);
                    if (data != null)
                    {
                        ado.MEDICINE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                        ado.MEDI_MATE_TYPE_ID = data.ID;
                        ado.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                        ado.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        ado.SERVICE_ID = data.SERVICE_ID;
                    }

                    List<V_HIS_EXP_MEST_MEDICINE> listMedicines = (
                        this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                        ? this._ExpMestMedicines.Where(p =>
                            p.MEDICINE_TYPE_ID == itemGr[0].MEDICINE_TYPE_ID).ToList()
                            : null;
                    if (listMedicines != null && listMedicines.Count > 0)
                    {
                        ado.AMOUNT_EXCUTE = listMedicines.Sum(p => p.AMOUNT);
                        ado.PACKAGE_NUMBER = listMedicines.First().PACKAGE_NUMBER;
                        ado.SUPPLIER_NAME = listMedicines.First().SUPPLIER_NAME;
                        ado.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicines.First().EXPIRED_DATE ?? 0);
                        ado.PRICE = listMedicines.First().PRICE;
                        ado.IMP_PRICE = listMedicines.First().IMP_PRICE;
                        ado.IMP_VAT_RATIO = listMedicines.First().IMP_VAT_RATIO * 100;
                        ado.DESCRIPTION = listMedicines.First().DESCRIPTION;
                        ado.MEDI_MATE_NUM_ORDER = listMedicines.First().MEDICINE_NUM_ORDER ?? 0;
                        ado.NUM_ORDER = listMedicines.First().NUM_ORDER;
                    }
                    if (this.currentAggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        ado.AMOUNT_EXPORTED = ado.AMOUNT_EXCUTE;
                    }
                    //}
                    ado.AMOUNT = itemGr.Sum(o => o.AMOUNT);
                    ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT)));
                    ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXCUTE)));
                    ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXPORTED)));

                    var expMest = this.lstExpMest.FirstOrDefault(o => o.ID == itemGr[0].EXP_MEST_ID);
                    if (expMest != null)
                    {
                        ado.TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
                        ado.Patient = patients.SingleOrDefault(o => o.ID == expMest.TDL_PATIENT_ID);
                        ado.TreatmentId = expMest.TDL_TREATMENT_ID ?? 0;
                    }
                    listMps000047ADO.Add(ado);
                }


                // VT


                listMps000047ADO = listMps000047ADO.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                var expMestMedicineGroups = listMps000047ADO.GroupBy(o => new { o.MEDI_MATE_TYPE_ID, o.TYPE_ID }).ToList();

                int countTemp = 0;
                int start = 0;
                int pageCount = 0;
                Dictionary<long, List<MPS.Processor.Mps000047.PDO.Mps000047ADO>> dicMedi = new Dictionary<long, List<MPS.Processor.Mps000047.PDO.Mps000047ADO>>();

                while (countTemp < listMps000047ADO.Count)
                {
                    pageCount += 1;
                    var expMestMedicineGroups__sub1 = expMestMedicineGroups.Skip(start).Take((int)keyColumnSize).Select(o => new { MEDI_MATE_TYPE_ID = o.ToList().First().MEDI_MATE_TYPE_ID, TYPE_ID = o.ToList().First().TYPE_ID }).ToList();
                    if (expMestMedicineGroups__sub1 != null && expMestMedicineGroups__sub1.Count > 0)
                    {
                        var expMestMedicinePrintAdoSplits =
                            (
                            from m in listMps000047ADO
                            from n in expMestMedicineGroups__sub1
                            where m.MEDI_MATE_TYPE_ID == n.MEDI_MATE_TYPE_ID
                            && m.TYPE_ID == n.TYPE_ID
                            select m
                             ).ToList();

                        dicMedi.Add(pageCount, expMestMedicinePrintAdoSplits);
                        //start += 44;
                        start += (int)keyColumnSize;
                        countTemp += expMestMedicinePrintAdoSplits.Count();
                    }
                }
                dicMedi = dicMedi.OrderByDescending(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
               
                
                
                foreach (var item in dicMedi)
                {
                    string printTypeCode = "Mps000047";
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentAggrExpMest != null ? this.currentAggrExpMest.TDL_TREATMENT_CODE : ""), "Mps000047", this.currentModule.RoomId);
                    MPS.Processor.Mps000047.PDO.Mps000047PDO mps000047RDO = new MPS.Processor.Mps000047.PDO.Mps000047PDO(
                      item.Value,
                     this.currentAggrExpMest,
                     _expMes,
                     department,
                     vHisTreatmentBedRooms,
                     BedLogList,
                     keyColumnSize
                 );

                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    string printerName = "";


                    if (GlobalVariables.dicPrinter.ContainsKey("Mps000047"))
                    {
                        printerName = GlobalVariables.dicPrinter["Mps000047"];
                    }

                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData("Mps000047", fileName, mps000047RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData("Mps000047", fileName, mps000047RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    MPS.MpsPrinter.Run(PrintData);
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        bool Check(V_HIS_EXP_MEST_MEDICINE _expMestMedicine)
        {
            bool result = false;
            try
            {
                var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == _expMestMedicine.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (this.serviceUnitIds != null
                        && this.serviceUnitIds.Count > 0)
                    {
                        if (this.serviceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (this.useFormIds != null
                    && this.useFormIds.Count > 0 && this.useFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
                        {
                            result = result && true;
                        }
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
        
        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TRA_DOI__MPS000047:
                        OnClickInTraPhieuDoiThuoc(fileName);
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
