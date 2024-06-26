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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AggrApprove.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrApprove
{
    public partial class frmAggrApprove : HIS.Desktop.Utility.FormBase
    {
        private void GenerateMenuPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_EXP_MEST_AGGR_APPROVE__PRINT_MENU__ITEM_IN_PHIEU_XUAT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInPhieuXuat)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInPhieuXuat(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TONG_HOP__MPS000046, DeletegatePrintTemplate);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TONG_HOP__MPS000046:
                        InPhieuLinhDaDuyet(printCode, fileName, ref result);
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

        private void InPhieuLinhDaDuyet(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                List<long> requestRoomIds = new List<long>();
                CommonParam param = new CommonParam();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.AGGR_EXP_MEST_ID = this.expMestId;
                var listExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                var listExpmestApproves = listExpMests.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList();

                HisExpMestMetyReqFilter medicineFilter = new HisExpMestMetyReqFilter();
                if (dataTreePrints != null)
                {
                    medicineFilter.EXP_MEST_IDs = dataTreePrints.EXP_MEST_IDs;
                }
                else
                {
                    medicineFilter.EXP_MEST_IDs = listExpmestApproves.Select(o => o.ID).ToList();
                }
                var medicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, medicineFilter, param);

                HisExpMestMatyReqFilter materialFilter = new HisExpMestMatyReqFilter();
                if (dataTreePrints != null)
                {
                    medicineFilter.EXP_MEST_IDs = dataTreePrints.EXP_MEST_IDs;
                }
                else
                {
                    materialFilter.EXP_MEST_IDs = listExpmestApproves.Select(o => o.ID).ToList();
                }
               
                var materials = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, materialFilter, param);

                var requestRooms = BackendDataWorker.Get<HIS_ROOM>();

                MPS.Processor.Mps000046.PDO.Mps000046PDO pdo = new MPS.Processor.Mps000046.PDO.Mps000046PDO(medicines, materials, expMestMedicines, expMestMaterials, null, listExpmestApproves, null, requestRoomIds, requestRoomIds, requestRoomIds, true, true, true, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(), MPS.Processor.Mps000046.PDO.keyTitles.phieuLinhTongHop);

                MPS.ProcessorBase.Core.PrintData printData = null;

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void treeListAggrApprove_SelectImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
               // dataTreePrint = new List<ExpMestApproveADO>();
                dataTreePrints = new ExpMestApproveADO();
                dataTreePrints = (ExpMestApproveADO)treeListAggrApprove.GetDataRecordByNode(e.Node);
                if (dataTreePrints != null)
                    printMediMate(dataTreePrints);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void printMediMate(ExpMestApproveADO dataTree)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TONG_HOP__MPS000046, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
