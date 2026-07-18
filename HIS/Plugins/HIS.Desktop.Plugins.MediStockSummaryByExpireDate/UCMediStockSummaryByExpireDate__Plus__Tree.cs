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
using HIS.UC.HisMateInStockByExpireDate.ADO;
using MOS.SDO;
using HIS.UC.HisMediInStockByExpireDate.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;

namespace HIS.Desktop.Plugins.MediStockSummaryByExpireDate
{
    public partial class UCMediStockSummaryByExpireDate : HIS.Desktop.Utility.UserControlBase
    {
        //Thuốc
        private void medicineType_GetSelectImage(HisMedicineInStockSDO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                //var noteData = (MOS.SDO.HisMedicineInStockSDO)e.Node.Tag;
                if (data != null)
                {
                    if (data.IS_LEAF == 1 && data.isTypeNode)
                    {
                        e.NodeImageIndex = 1;
                    }
                    else
                    {
                        e.NodeImageIndex = -1;
                    }
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_SelectImageClick(HisMedicineInStockSDO data)
        {
            try
            {
                MessageBox.Show("Báo cáo" + "TODO", "Thông báo");
                //var noteData = (MOS.SDO.HisMedicineInStockSDO)e.Node.Tag;
                // if (noteData != null)
                // {

                //  }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_GetStateImage(HisMedicineInStockSDO data, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                //var noteData = (MOS.SDO.HisMedicineInStockSDO)e.Node.Tag;
                if (data != null)
                {
                    if (data.IS_LEAF == 1 && data.isTypeNode)
                    {
                        e.NodeImageIndex = 0;
                    }
                    else
                    {
                        e.NodeImageIndex = -1;
                    }
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_StateImageClick(HisMedicineInStockSDO data)
        {
            try
            {
                MessageBox.Show("Báo cáo State" + "TODO", "Thông báo");
                //var noteData = (MOS.SDO.HisMedicineInStockSDO)e.Node.Tag;
                // if (noteData != null)
                // {

                //  }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void medicineType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HisMediInStockByExpireDateADO data = e.Row as HisMediInStockByExpireDateADO;
                    if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    {
                        if (data.ParentNodeId != null)
                        {
                            e.Value = (data.IMP_VAT_RATIO * 100);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "TOTAL_PRICE")
                    {
                        if (data.ParentNodeId != null)
                        {
                            decimal SUM_PRICE = data.IMP_PRICE * data.TotalAmount * (1 + data.IMP_VAT_RATIO) ?? 0;
                            e.Value = SUM_PRICE;
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                    {
                        if (data.ParentNodeId != null)
                        {
                            e.Value = data.IMP_PRICE;
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "TotalAmountConvert_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        //e.Value = (data.TotalAmount * data.CONVERT_RATIO);//TODO
                    }
                    else if (e.Column.FieldName == "AvailableAmountConvert_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        //e.Value = (data.AvailableAmount * data.CONVERT_RATIO);//TODO
                    }
                    else if (e.Column.FieldName == "CompensationBaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        if (data.RealBaseAmount.HasValue)
                        {
                            e.Value = data.RealBaseAmount.Value - (data.TotalAmount ?? 0);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "BaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        e.Value = data.BaseAmount;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void medicineType_CustomDrawNodeCell(HisMedicineInStockSDO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data.IS_LEAF == 1 && data.ALERT_EXPIRED_DATE != null)
                    {
                        //Các loại thuốc (lá) có hạn sử dụng nằm trong khoảng Cảnh báo nếu số ngày hết hạn < số ngày khai báo trong danh mục đều được tô màu đỏ
                        //ALERT_EXPIRED_DATE  số ngày cảnh báo
                        //EXPIRED_DATE ngày hết hạn
                        //Số ngày khai báo trong danh mục = ngày hết hạn - thời gian hiện tại
                        if (data.EXPIRED_DATE != null)
                        {
                            DateTime dtNow = DateTime.Now;

                            DateTime EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)data.EXPIRED_DATE) ?? DateTime.Now;
                            long soNgay = (EXPIRED_DATE - dtNow).Days;

                            if (soNgay <= data.ALERT_EXPIRED_DATE)
                            {
                                e.Appearance.ForeColor = Color.Red;
                            }
                        }


                        //long startToday = Inventec.Common.DateTime.Get.StartDay() ?? 0;
                        //string startTodayStr = Inventec.Common.DateTime.Convert.TimeNumberToDateString(startToday);
                        //DateTime startTodayDt = Inventec.Common.TypeConvert.Parse.ToDateTime(startTodayStr);
                        //DateTime startTodayDtAddAlertDay = startTodayDt.AddDays(data.ALERT_EXPIRED_DATE ?? 0);
                        //long expiredDate = Convert.ToInt64(data.EXPIRED_DATE ?? 0);
                        //if (expiredDate > 0)
                        //{
                        //    string expiredDateStr = Inventec.Common.DateTime.Convert.TimeNumberToDateString(expiredDate);
                        //    DateTime expiredDateDt = Inventec.Common.TypeConvert.Parse.ToDateTime(expiredDateStr);
                        //    if (startTodayDtAddAlertDay >= expiredDateDt)
                        //        e.Appearance.ForeColor = Color.Red;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Vật tư
        private void materialType_GetSelectImage(HisMaterialInStockSDO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                //var noteData = (MOS.SDO.HisMaterialInStockSDO)e.Node.Tag;
                if (data != null)
                {
                    if (data.IS_LEAF == 1 && data.isTypeNode)
                    {
                        e.NodeImageIndex = 1;
                    }
                    else
                    {
                        e.NodeImageIndex = -1;
                    }
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_SelectImageClick(HisMaterialInStockSDO data)
        {
            try
            {
                MessageBox.Show("Báo cáo Select" + "TODO", "Thông báo");
                //var noteData = (MOS.SDO.HisMedicineInStockSDO)e.Node.Tag;
                // if (noteData != null)
                // {

                //  }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_GetStateImage(HisMaterialInStockSDO data, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                //var noteData = (MOS.SDO.HisMaterialInStockSDO)e.Node.Tag;
                if (data != null)
                {
                    if (data.IS_LEAF == 1 && data.isTypeNode)
                    {
                        e.NodeImageIndex = 0;
                    }
                    else
                    {
                        e.NodeImageIndex = -1;
                    }
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_StateImageClick(HisMaterialInStockSDO data)
        {
            try
            {
                MessageBox.Show("Báo cáo State" + "TODO", "Thông báo");
                //var noteData = (MOS.SDO.HisMedicineInStockSDO)e.Node.Tag;
                // if (noteData != null)
                // {

                //  }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void materialType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HisMaterialInStockSDO data = e.Row as HisMaterialInStockSDO;
                    //if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                    //{
                    //    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(data.EXPIRED_DATE.ToString()));
                    //}
                    //else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    //{
                    //    e.Value = (data.IMP_VAT_RATIO * 100);
                    //}
                    if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                    {
                        if (data.ParentNodeId != null)
                        {
                            e.Value = (data.IMP_VAT_RATIO * 100);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "TOTAL_PRICE")
                    {
                        if (data.ParentNodeId != null)
                        {
                            decimal SUM_PRICE = data.IMP_PRICE * data.TotalAmount * (1 + data.IMP_VAT_RATIO) ?? 0;
                            e.Value = SUM_PRICE;
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                    {
                        if (data.ParentNodeId != null)
                        {
                            e.Value = data.IMP_PRICE;
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "CompensationBaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        if (data.RealBaseAmount.HasValue)
                        {
                            e.Value = data.RealBaseAmount.Value - (data.TotalAmount ?? 0);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "BaseAmount_Display" && data.IS_LEAF == 1 && (data.isTypeNode || !e.Node.HasChildren))
                    {
                        e.Value = data.BaseAmount;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void materialType_CustomDrawNodeCell(HisMaterialInStockSDO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (data != null)
                    {
                        if (data.IS_LEAF == 1 && data.ALERT_EXPIRED_DATE != null)
                        {
                            //Các loại vật tư (lá) có hạn sử dụng nằm trong khoảng cảnh báo đều được tô màu đỏ

                            if (data.EXPIRED_DATE != null)
                            {
                                DateTime dtNow = DateTime.Now;
                                DateTime EXPIRED_DATE = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)data.EXPIRED_DATE) ?? DateTime.Now;
                                long soNgay = (EXPIRED_DATE - dtNow).Days;
                                if (soNgay <= data.ALERT_EXPIRED_DATE)
                                {
                                    e.Appearance.ForeColor = Color.Red;
                                }
                            }

                            //long startToday = Inventec.Common.DateTime.Get.StartDay() ?? 0;
                            //string startTodayStr = Inventec.Common.DateTime.Convert.TimeNumberToDateString(startToday);
                            //DateTime startTodayDt = Inventec.Common.TypeConvert.Parse.ToDateTime(startTodayStr);
                            //DateTime startTodayDtAddAlertDay = startTodayDt.AddDays(data.ALERT_EXPIRED_DATE ?? 0);
                            //long expiredDate = Convert.ToInt64(data.EXPIRED_DATE ?? 0);
                            //if (expiredDate > 0)
                            //{
                            //    string expiredDateStr = Inventec.Common.DateTime.Convert.TimeNumberToDateString(expiredDate);
                            //    DateTime expiredDateDt = Inventec.Common.TypeConvert.Parse.ToDateTime(expiredDateStr);
                            //    if (startTodayDtAddAlertDay >= expiredDateDt)
                            //        e.Appearance.ForeColor = Color.Red;
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //Máu

        // Số ID tối đa mỗi lần gọi GetView (filter đi qua query string) — tránh HTTP 414 URI Too Long khi có nhiều lô
        private const int GET_VIEW_CHUNK_SIZE = 100;

        /// <summary>
        /// Bổ sung Hãng sản xuất (MANUFACTURER_NAME) theo từng LÔ cho cây tồn kho thuốc.
        /// API cây tồn kho điền hãng SX theo LOẠI (V_HIS_MEDICINE_TYPE), nên ở đây lấy strict theo LÔ
        /// từ V_HIS_MEDICINE (MANUFACTURER_ID) và tra tên hãng từ danh mục HIS_MANUFACTURER (cache RAM).
        /// Chỉ điền cho dòng lô chi tiết (!isTypeNode &amp;&amp; ID > 0); dòng loại (node cha) để trống.
        /// </summary>
        private void FillManufacturerForMedicine(List<List<HisMedicineInStockSDO>> listData)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (listData == null || listData.Count == 0)
                    return;

                List<HisMedicineInStockSDO> flatData = listData
                    .Where(o => o != null)
                    .SelectMany(o => o)
                    .ToList();
                if (flatData.Count == 0)
                    return;

                List<long> medicineIds = flatData
                    .Where(o => !o.isTypeNode && o.ID > 0)
                    .Select(o => o.ID)
                    .Distinct()
                    .ToList();
                if (medicineIds.Count == 0)
                    return;

                // Chia nhỏ danh sách ID để tránh URL quá dài (HTTP 414) — GetView truyền filter qua query string
                List<V_HIS_MEDICINE> medicines = new List<V_HIS_MEDICINE>();
                for (int i = 0; i < medicineIds.Count; i += GET_VIEW_CHUNK_SIZE)
                {
                    List<long> chunk = medicineIds.GetRange(i, Math.Min(GET_VIEW_CHUNK_SIZE, medicineIds.Count - i));
                    MOS.Filter.HisMedicineViewFilter filter = new MOS.Filter.HisMedicineViewFilter();
                    filter.IDs = chunk;
                    List<V_HIS_MEDICINE> part = new BackendAdapter(param).Get<List<V_HIS_MEDICINE>>(
                        HisRequestUriStore.HIS_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (part != null && part.Count > 0)
                        medicines.AddRange(part);
                }
                // Không lấy được lô nào → KHÔNG xóa gì, giữ nguyên dữ liệu backend trả (tránh làm trắng toàn bộ)
                if (medicines.Count == 0)
                    return;

                Dictionary<long, V_HIS_MEDICINE> dicMedicine = new Dictionary<long, V_HIS_MEDICINE>();
                foreach (var medi in medicines)
                {
                    if (!dicMedicine.ContainsKey(medi.ID))
                        dicMedicine.Add(medi.ID, medi);
                }

                Dictionary<long, string> dicManufacturer = new Dictionary<long, string>();
                foreach (var manu in BackendDataWorker.Get<HIS_MANUFACTURER>())
                {
                    if (!dicManufacturer.ContainsKey(manu.ID))
                        dicManufacturer.Add(manu.ID, manu.MANUFACTURER_NAME);
                }

                foreach (var item in flatData)
                {
                    // Node cha (loại) hoặc dòng không phải lô → để trống (chỉ hiển thị theo lô)
                    if (item.isTypeNode || item.ID <= 0)
                    {
                        item.MANUFACTURER_NAME = null;
                        continue;
                    }
                    // Dòng lô: lấy strict theo lô (không mượn giá trị loại)
                    V_HIS_MEDICINE medicine;
                    dicMedicine.TryGetValue(item.ID, out medicine);
                    string manufacturerName = null;
                    if (medicine != null && medicine.MANUFACTURER_ID.HasValue)
                        dicManufacturer.TryGetValue(medicine.MANUFACTURER_ID.Value, out manufacturerName);
                    item.MANUFACTURER_NAME = manufacturerName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Bổ sung Hãng sản xuất (MANUFACTURER_NAME) theo từng LÔ cho cây tồn kho vật tư.
        /// Lấy từ V_HIS_MATERIAL (theo lô — có MANUFACTURER_ID) và tra tên hãng từ HIS_MANUFACTURER.
        /// Chỉ điền cho dòng lô chi tiết (!isTypeNode &amp;&amp; ID > 0); dòng loại (node cha) để trống.
        /// </summary>
        private void FillManufacturerForMaterial(List<List<HisMaterialInStockSDO>> listData)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (listData == null || listData.Count == 0)
                    return;

                List<HisMaterialInStockSDO> flatData = listData
                    .Where(o => o != null)
                    .SelectMany(o => o)
                    .ToList();
                if (flatData.Count == 0)
                    return;

                List<long> materialIds = flatData
                    .Where(o => !o.isTypeNode && o.ID > 0)
                    .Select(o => o.ID)
                    .Distinct()
                    .ToList();
                if (materialIds.Count == 0)
                    return;

                // Chia nhỏ danh sách ID để tránh URL quá dài (HTTP 414) — GetView truyền filter qua query string
                List<V_HIS_MATERIAL> materials = new List<V_HIS_MATERIAL>();
                for (int i = 0; i < materialIds.Count; i += GET_VIEW_CHUNK_SIZE)
                {
                    List<long> chunk = materialIds.GetRange(i, Math.Min(GET_VIEW_CHUNK_SIZE, materialIds.Count - i));
                    MOS.Filter.HisMaterialViewFilter filter = new MOS.Filter.HisMaterialViewFilter();
                    filter.IDs = chunk;
                    List<V_HIS_MATERIAL> part = new BackendAdapter(param).Get<List<V_HIS_MATERIAL>>(
                        HisRequestUriStore.HIS_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (part != null && part.Count > 0)
                        materials.AddRange(part);
                }
                // Không lấy được lô nào → KHÔNG xóa gì, giữ nguyên dữ liệu backend trả (tránh làm trắng toàn bộ)
                if (materials.Count == 0)
                    return;

                Dictionary<long, V_HIS_MATERIAL> dicMaterial = new Dictionary<long, V_HIS_MATERIAL>();
                foreach (var mate in materials)
                {
                    if (!dicMaterial.ContainsKey(mate.ID))
                        dicMaterial.Add(mate.ID, mate);
                }

                Dictionary<long, string> dicManufacturer = new Dictionary<long, string>();
                foreach (var manu in BackendDataWorker.Get<HIS_MANUFACTURER>())
                {
                    if (!dicManufacturer.ContainsKey(manu.ID))
                        dicManufacturer.Add(manu.ID, manu.MANUFACTURER_NAME);
                }

                foreach (var item in flatData)
                {
                    // Node cha (loại) hoặc dòng không phải lô → để trống (chỉ hiển thị theo lô)
                    if (item.isTypeNode || item.ID <= 0)
                    {
                        item.MANUFACTURER_NAME = null;
                        continue;
                    }
                    // Dòng lô: lấy strict theo lô (không mượn giá trị loại)
                    V_HIS_MATERIAL material;
                    dicMaterial.TryGetValue(item.ID, out material);
                    string manufacturerName = null;
                    if (material != null && material.MANUFACTURER_ID.HasValue)
                        dicManufacturer.TryGetValue(material.MANUFACTURER_ID.Value, out manufacturerName);
                    item.MANUFACTURER_NAME = manufacturerName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
