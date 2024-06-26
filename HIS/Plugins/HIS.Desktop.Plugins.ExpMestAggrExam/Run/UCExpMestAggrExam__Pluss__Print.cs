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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggrExam.ADO;
using HIS.Desktop.Plugins.ExpMestAggrExam.Base;
using HIS.Desktop.Plugins.Library.PrintPrescription;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.ExpMestAggrExam
{
    public partial class UCExpMestAggrExam : HIS.Desktop.Utility.UserControlBase
    {
        internal void PrintAggregateExpMest()
        {
            try
            {
                //if (barManager1 == null)
                //{
                //    barManager1 = new DevExpress.XtraBars.BarManager();
                //    barManager1.Form = this;
                //}

                //UCExpMestAggrExamListPopupMenuProcessor processor = new UCExpMestAggrExamListPopupMenuProcessor(this.currentAggExpMest, ExpMestAggregateMouseRightClick, barManager1);
                //processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void clickItemInGopDonThuoc(V_HIS_EXP_MEST aggExpMest)
        {
            try
            {
                if (aggExpMest == null)
                {
                    throw new ArgumentNullException("aggExpMest is null");
                }
                CommonParam param = new CommonParam();

                //Load expmest
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.AGGR_EXP_MEST_ID = aggExpMest.ID;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                if (expMests == null || expMests.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("expMests is null");
                    return;
                }

                //Load đơn phòng khám
                var serviceReqIds = expMests.Where(p => p.SERVICE_REQ_ID != null).Select(o => o.SERVICE_REQ_ID ?? 0);
                if (serviceReqIds == null || serviceReqIds.Count() == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("serviceReqIds is null");
                    return;
                }

                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.IDs = serviceReqIds.Distinct().ToList();
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                if (serviceReqs == null || serviceReqs.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("serviceReqs is null");
                    return;
                }

                //Laays thuoc va tu trong kho

                HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                if ((expMestMedicines != null && expMestMedicines.Count > 0)
                            || (expMestMaterials != null && expMestMaterials.Count > 0))
                {
                    OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                    outPatientPresResultSDO.ExpMests = expMests;
                    outPatientPresResultSDO.ServiceReqs = serviceReqs;
                    outPatientPresResultSDO.Medicines = expMestMedicines;
                    outPatientPresResultSDO.Materials = expMestMaterials;
                    OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
                }
                HIS_EXP_MEST expMest =new HIS_EXP_MEST();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST,HIS_EXP_MEST>();
                expMest= AutoMapper.Mapper.Map<HIS_EXP_MEST>(aggExpMest);
                PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMest);

                printPrescriptionProcessor.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__IN_GOP_DON_THUOC__MPS000234, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void PrintAggregateExpMest(V_HIS_EXP_MEST currentAggExpMest)
        {
            try
            {
                //Review
                this.currentAggrExpMest = currentAggExpMest;
                clickItemInGopDonThuoc(this.currentAggrExpMest);
                //DevExpress.XtraBars.BarManager barManager1 = new DevExpress.XtraBars.BarManager();
                //barManager1.Form = this;

                //UCExpMestAggrExamListPopupMenuProcessor processor = new UCExpMestAggrExamListPopupMenuProcessor(this.currentAggrExpMest, ExpMestAggregateMouseRightClick, barManager1);
                //processor.InitMenu();
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
                if (e.Item.Tag is UCExpMestAggrExamListPopupMenuProcessor.PrintType)
                {
                    var moduleType = (UCExpMestAggrExamListPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case UCExpMestAggrExamListPopupMenuProcessor.PrintType.InTraDoiThuoc:
                            ShowFormFilter(Convert.ToInt64(1));
                            break;
                        case UCExpMestAggrExamListPopupMenuProcessor.PrintType.InPhieuTongHop:
                            ShowFormFilter(Convert.ToInt64(2));
                            break;
                        case UCExpMestAggrExamListPopupMenuProcessor.PrintType.InPhieuLinhThuocGayNghienHuongTT:
                            break;
                        case UCExpMestAggrExamListPopupMenuProcessor.PrintType.InPhieuLinhThuoc:
                            ShowFormFilter(Convert.ToInt64(3));
                            break;
                        case UCExpMestAggrExamListPopupMenuProcessor.PrintType.InPhieuLinhThuocTheoBenhNhan:
                            ShowFormFilter(Convert.ToInt64(4));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(long printType)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    //Review
                    List<object> listArgs = new List<object>();
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

    }
}
