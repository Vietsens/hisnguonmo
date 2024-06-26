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
using Inventec.UC.TreeSereServHein.Data;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using DevExpress.Utils.Menu;
using Inventec.UC.TreeSereServHein.Delegate;
using Inventec.UC.TreeSereServHein.ADO;

namespace Inventec.UC.TreeSereServHein
{
    public partial class UCTreeSereServHein : UserControl
    {
        InitData data;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> ListSereServByTreatment = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
        public static List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> ListPatientType = new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
        PopupMenuShowingForTreeList popupMenuShowingForTreeList;
        TotalPrice TotalPrice;

        public UCTreeSereServHein()
        {
            InitializeComponent();
        }

        public UCTreeSereServHein(Data.InitData data)
        {
            InitializeComponent();
            this.data = data;
        }

        private void UCTreeSereServHein_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataToListPatientType();
                this.popupMenuShowingForTreeList = this.data.PopupMenuShowing_Click;
                if (this.data.hisTreatmentSDO != null)
                {
                    LoadPatientTypeAlter(this.data.hisTreatmentSDO, this);
                    this.TotalPrice = this.data.TotalPrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Load Data
        private void LoadPatientTypeAlter(MOS.SDO.HisTreatmentHeinSDO dataForm, UCTreeSereServHein control)
        {
            try
            {
                if (dataForm != null)
                {
                    CommonParam param = new CommonParam();
                    List<MOS.EFMODEL.DataModels.V_HIS_PATY_ALTER_BHYT> litHisPatyAlterBHYT = new BackendAdapter(param).Get<List<V_HIS_PATY_ALTER_BHYT>>(HisRequestUriStore.HIS_PATY_ALTER_BHYT_GETVIEW_MIE, ApiConsumers.MosConsumer, dataForm.ID, param);
                    if (litHisPatyAlterBHYT != null && litHisPatyAlterBHYT.Count > 0)
                    {
                        FillDataToControl(litHisPatyAlterBHYT.FirstOrDefault(), this);
                    }
                    //
                    LoadSereServFromApiByHospitalFee(dataForm.ID);
                    FillDataToSereServTree(dataForm, this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl(MOS.EFMODEL.DataModels.V_HIS_PATY_ALTER_BHYT dataForm, UCTreeSereServHein control)
        {
            try
            {
                if (dataForm != null)
                {
                    //LoadSereServFromApiByHospitalFee(, control);
                    // FillDataToSereServTree(dataForm, control);
                    // FillDataToPriceControl(dataForm, control);
                }
                else
                {
                    //FillDataToTreatmentFeeControl(null, control);
                    //FillDataToSereServTree(null, control);
                    //FillDataToPriceControl(null, control);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServFromApiByHospitalFee(long treatmentId)
        {
            CommonParam param = new CommonParam();
            MOS.Filter.HisSereServViewFilter filter = new MOS.Filter.HisSereServViewFilter();
            filter.TREATMENT_ID = treatmentId;
            ListSereServByTreatment = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, filter, param);
        }

        private void LoadDataToListPatientType()
        {
            CommonParam param = new CommonParam();
            MOS.Filter.HisPatientTypeFilter filter = new MOS.Filter.HisPatientTypeFilter();
            ListPatientType = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>(HisRequestUriStore.HIS_PATIENT_TYPE_GET, ApiConsumers.MosConsumer, filter, param);
        }

        #endregion

        #region Load data to TreeList
        internal static void FillDataToSereServTree(MOS.SDO.HisTreatmentHeinSDO dataHospitalFee, UCTreeSereServHein control)
        {
            try
            {
                if (dataHospitalFee != null)
                {
                    control.treeSereServ.ClearNodes();
                    TreeListNode parentForRootNodes = null;
                    var listRoot = ListSereServByTreatment.Where(o => o.IS_EXPEND != 1 && o.PATIENT_TYPE_ID == 22).Select(o => o.PATIENT_TYPE_ID).Distinct();
                    var listRootExpend = ListSereServByTreatment.Where(o => o.IS_EXPEND == 1).ToList();
                    foreach (var item in listRoot)
                    {
                        var patientTypeObj = ListPatientType.FirstOrDefault(o => o.ID == item);
                        if (patientTypeObj != null)
                        {
                            TreeListNode rootNode = control.treeSereServ.AppendNode(
                       new object[] { patientTypeObj.PATIENT_TYPE_NAME, null, null, null, null, null, null, null, null, null, null },
                       parentForRootNodes, null);

                            CreateChildNodeServiceType(rootNode, item, control);
                        }
                    }
                    if (listRootExpend != null && listRootExpend.Count > 0)
                    {
                        TreeListNode rootNode = control.treeSereServ.AppendNode(
                       new object[] { "Hao phí", null, null, null, null, null, null, null, null, null, null },
                       parentForRootNodes, null);
                        CreateChildNodeServiceTypeExpend(rootNode, control);
                    }

                    control.treeSereServ.ExpandAll();
                }
                else
                {
                    control.treeSereServ.ClearNodes();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal static void CreateChildNodeServiceType(TreeListNode rootNode, long patientTypeId, UCTreeSereServHein control)
        {
            try
            {
                var listChild = ListSereServByTreatment.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.IS_EXPEND != 1).ToList();
                var listServiceTypeId = ListSereServByTreatment.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.IS_EXPEND != 1).Select(o => o.SERVICE_TYPE_ID).Distinct().ToList();
                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                    new object[] { serviceTypeObj.SERVICE_TYPE_NAME, null, null, null, null, null, null, null, null, null, null },
                    rootNode, null);
                            CreateChildNodeService(childNode, patientTypeId, serviceTypeId, control);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeService(TreeListNode childNode, long patientTypeId, long serviceTypeId, UCTreeSereServHein control)
        {
            try
            {
                var listChild = ListSereServByTreatment.FindAll(o => o.PATIENT_TYPE_ID == patientTypeId && o.SERVICE_TYPE_ID == serviceTypeId && o.IS_EXPEND != 1).ToList();
                if (listChild != null && listChild.Count > 0)
                {
                    foreach (var item in listChild)
                    {
                        string expen = "";
                        if (item.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE)
                        {
                            expen = "Hao phí";
                        }
                        TreeListNode childChildNode = control.treeSereServ.AppendNode(
                   new object[] { item.SERVICE_NAME, item.AMOUNT, item.PRICE, item.VAT_RATIO * 100, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.VIR_TOTAL_PRICE ?? 0), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.VIR_TOTAL_HEIN_PRICE ?? 0), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.VIR_TOTAL_PATIENT_PRICE ?? 0), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.DISCOUNT ?? 0), item.SERVICE_CODE, item.SERVICE_REQ_CODE, item.TRANSACTION_CODE, expen, null },
                   childNode, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeServiceTypeExpend(TreeListNode rootNode, UCTreeSereServHein control)
        {
            try
            {
                var listChild = ListSereServByTreatment.FindAll(o => o.IS_EXPEND == 1);
                var listServiceTypeId = ListSereServByTreatment.FindAll(o => o.IS_EXPEND == 1).Select(o => o.SERVICE_TYPE_ID).Distinct().ToList();
                if (listServiceTypeId != null && listServiceTypeId.Count > 0)
                {
                    foreach (var serviceTypeId in listServiceTypeId)
                    {
                        var serviceTypeObj = listChild.FirstOrDefault(o => o.SERVICE_TYPE_ID == serviceTypeId);
                        if (serviceTypeObj != null)
                        {
                            TreeListNode childNode = control.treeSereServ.AppendNode(
                   new object[] { serviceTypeObj.SERVICE_TYPE_NAME, null, null, null, null, null, null, null, null, null, null },
                   rootNode, null);
                            CreateChildNodeServiceExpend(childNode, serviceTypeId, control);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CreateChildNodeServiceExpend(TreeListNode childNode, long serviceTypeId, UCTreeSereServHein control)
        {
            try
            {
                var listChild = ListSereServByTreatment.FindAll(o => o.SERVICE_TYPE_ID == serviceTypeId && o.IS_EXPEND == 1).ToList();
                if (listChild != null && listChild.Count > 0)
                {
                    foreach (var item in listChild)
                    {
                        string expen = "";
                        if (item.IS_EXPEND == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE)
                        {
                            expen = "Hao phí";
                        }
                        TreeListNode childChildNode = control.treeSereServ.AppendNode(
                  new object[] { item.SERVICE_NAME, item.AMOUNT, item.PRICE, item.VAT_RATIO * 100, Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.VIR_TOTAL_PRICE ?? 0), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.VIR_TOTAL_HEIN_PRICE ?? 0), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.VIR_TOTAL_PATIENT_PRICE ?? 0), Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(item.DISCOUNT ?? 0), item.SERVICE_CODE, item.SERVICE_REQ_CODE, item.TRANSACTION_CODE, expen, null },
                  childNode, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void treeSereServHein_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                TreeList treeList = sender as TreeList;
                TreeListHitInfo hInfo = treeList.CalcHitInfo(e.Point);
                if (hInfo.HitInfoType == HitInfoType.Cell && hInfo.Node == treeList.FocusedNode)
                {
                    object DataRow = (object)treeSereServ.GetDataRecordByNode(treeSereServ.FocusedNode);
                    this.popupMenuShowingForTreeList(DataRow, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void treeSereServHein_GetStateImage(object sender, GetStateImageEventArgs e)
        {
            try
            {
                var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)e.Node.Tag;
                if (this.data.moduleType == Store.GlobalStore.ModuleType.HFS)
                {
                    if (noteData != null && noteData.IS_NO_EXECUTE == null)
                    {
                        if (noteData.PARENT_ID == null && noteData.BILL_ID == null)
                            e.NodeImageIndex = 0;
                        else if (noteData.PARENT_ID == null && noteData.BILL_ID != null)
                            e.NodeImageIndex = 1;
                        else
                            e.NodeImageIndex = -1;

                    }
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServHein_StateImageClick(object sender, NodeClickEventArgs e)
        {
            try
            {
                var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)e.Node.Tag;
                if (noteData != null && noteData.BILL_ID != null)
                {
                    MessageBox.Show("ẩn");
                    return;
                }
                else if (noteData != null && noteData.IS_NO_EXECUTE == null)
                {
                    MessageBox.Show("Hiện");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_GetSelectImage(object sender, GetSelectImageEventArgs e)
        {
            try
            {
                // khi nao co 2 image thy enable

                var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)e.Node.Tag;
                if (this.data.moduleType == Store.GlobalStore.ModuleType.HFS)
                {
                    e.NodeImageIndex = 0;
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {

        }

        internal void RefeshData(Data.InitData data)
        {
            try
            {
                if (data != null)
                {
                    this.data = data;
                    LoadPatientTypeAlter(this.data.hisTreatmentSDO, this);
                    this.TotalPrice = this.data.TotalPrice;
                }
                else
                {
                    treeSereServ.Nodes.Clear();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                MessageBox.Show("Cell Value Changing");
                var noteData = (SereServADO)treeSereServ.GetDataRecordByNode(e.Node);
                if (noteData != null)
                {
                    if (noteData.CheckState != null && Convert.ToBoolean(noteData.CheckState))
                    {
                        decimal price = (noteData.Price != null ? Convert.ToDecimal(noteData.Price) : 0);
                        decimal heinprice = (noteData.HeinPrice != null ? Convert.ToDecimal(noteData.HeinPrice) : 0);
                        decimal amount = (noteData.Amount != null ? Convert.ToDecimal(noteData.Amount) : 0);
                        if (price > 0 && amount > 0)
                        {
                            noteData.TotalPrice = (heinprice != null ? ((price - heinprice) * amount).ToString() : (price * amount).ToString());
                        }
                    }
                    else
                    {
                        noteData.TotalPrice = null;
                        noteData.Amount = null;
                    }

                    //Update total 
                    var listdata = (List<SereServADO>)treeSereServ.DataSource;
                    if (listdata != null && listdata.Count > 0)
                    {
                        decimal tongPhi = 0;
                        decimal bhyt = 0;
                        decimal phaiTra = 0;
                        decimal tamUng = 0;
                        decimal thanhToan = 0;
                        decimal hoanthu = 0;
                        decimal mienGiam = 0;
                        decimal hoanUng = 0;
                        decimal nopThem = 0;
                        decimal thua = 0;
                        foreach (var item in listdata)
                        {
                            decimal price = (noteData.Price != null ? Convert.ToDecimal(noteData.Price) : 0);
                            decimal heinprice = (noteData.HeinPrice != null ? Convert.ToDecimal(noteData.HeinPrice) : 0);
                            decimal amount = (noteData.Amount != null ? Convert.ToDecimal(noteData.Amount) : 0);
                            if (item.CheckState != null && Convert.ToBoolean(item.CheckState))
                            {
                                //if (price > 0 && amount > 0)
                                //{
                                //    totalAmount += (heinprice != null ? ((price - heinprice) * amount) : (price * amount));
                                //}
                            }
                            if (price > 0 && amount > 0)
                            {
                                tongPhi += (heinprice != null ? ((price - heinprice) * amount) : (price * amount));
                            }
                            bhyt += heinprice;
                        }
                        phaiTra = tongPhi - bhyt;

                        this.TotalPrice(tongPhi, bhyt, phaiTra);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                //todo
                MessageBox.Show("Cell Value Changed");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }
    }
}
