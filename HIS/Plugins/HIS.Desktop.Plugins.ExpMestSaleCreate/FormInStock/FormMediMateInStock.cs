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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.FormInStock
{
    public partial class FormMediMateInStock : HIS.Desktop.Utility.FormBase
    {
        List<MediMateTypeADO> Medicine;
        List<MediMateTypeADO> Material;
        MediMateTypeADO FocusData;
        //List<MediMateInstockADO> MediMateInstock;

        private List<System.Dynamic.ExpandoObject> ListDataSource;
        private List<string> ListMediStock;

        public FormMediMateInStock(List<MediMateTypeADO> medicine, List<MediMateTypeADO> material, MediMateTypeADO focusData)
        {
            InitializeComponent();
            try
            {
                this.Medicine = medicine;
                this.Material = material;
                this.FocusData = focusData;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormInStock_Load(object sender, EventArgs e)
        {
            try
            {
                ListMediStock = new List<string>();
                ListDataSource = new List<System.Dynamic.ExpandoObject>();
                LoadMedicineInstock();
                LoadMaterialInstock();
                ProcessInitColumn();
                gridControl1.DataSource = ListDataSource;
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessInitColumn()
        {
            try
            {
                if (ListMediStock != null && ListMediStock.Count > 0)
                {
                    ListMediStock = ListMediStock.Distinct().ToList();
                    int index = gridView1.Columns.Count;
                    for (int i = 0; i < ListMediStock.Count; i++)
                    {
                        GridColumn colKho = new GridColumn();
                        colKho.Caption = ListMediStock[i].Split('|').Last();
                        colKho.FieldName = ListMediStock[i].Split('|').First();
                        colKho.VisibleIndex = index;
                        colKho.Width = colKho.Caption.Length > 0 ? colKho.Caption.Length * 5 + 10 : 50;
                        colKho.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                        colKho.OptionsColumn.AllowEdit = false;
                        colKho.DisplayFormat.FormatString = "#,##0";
                        colKho.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                        gridView1.Columns.Add(colKho);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterialInstock()
        {
            try
            {
                if (Medicine != null && Medicine.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisMaterialTypeHospitalViewFilter filter = new HisMaterialTypeHospitalViewFilter();
                    filter.IS_BUSINESS = true;
                    filter.MATERIAL_TYPE_IDs = Material.Select(s => s.MEDI_MATE_TYPE_ID).Distinct().ToList();
                    var lstMediInStocks = new BackendAdapter(param).Get<MaterialTypeInHospitalSDO>("api/HisMaterialType/GetInHospitalMaterialType", ApiConsumers.MosConsumer, filter, param);
                    if (lstMediInStocks != null && lstMediInStocks.MediStockCodes != null && lstMediInStocks.MediStockCodes.Count > 0)
                    {
                        for (int i = 0; i < lstMediInStocks.MediStockCodes.Count; i++)
                        {
                            ListMediStock.Add(string.Format("{0}|{1}", lstMediInStocks.MediStockCodes[i], lstMediInStocks.MediStockNames[i]));
                        }

                        ListDataSource.AddRange(lstMediInStocks.MaterialTypeDatas);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicineInstock()
        {
            try
            {
                if (Medicine != null && Medicine.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisMedicineTypeHospitalViewFilter filter = new HisMedicineTypeHospitalViewFilter();
                    filter.IS_BUSINESS = true;
                    filter.MEDICINE_TYPE_IDs = Medicine.Select(s => s.MEDI_MATE_TYPE_ID).Distinct().ToList();
                    var lstMediInStocks = new BackendAdapter(param).Get<MedicineTypeInHospitalSDO>("api/HisMedicineType/GetInHospitalMedicineType", ApiConsumers.MosConsumer, filter, param);
                    if (lstMediInStocks != null && lstMediInStocks.MediStockCodes != null && lstMediInStocks.MediStockCodes.Count > 0)
                    {
                        for (int i = 0; i < lstMediInStocks.MediStockCodes.Count; i++)
                        {
                            ListMediStock.Add(string.Format("{0}|{1}", lstMediInStocks.MediStockCodes[i], lstMediInStocks.MediStockNames[i]));
                        }

                        ListDataSource.AddRange(lstMediInStocks.MedicineTypeDatas);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (System.Dynamic.ExpandoObject)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == Gc_MediMateCode.FieldName)
                        {
                            string code = "";
                            try
                            {
                                code += (((IDictionary<string, object>)data)["MEDICINE_TYPE_CODE"] ?? "").ToString();
                            }
                            catch (Exception)
                            {
                            }

                            try
                            {
                                code += (((IDictionary<string, object>)data)["MATERIAL_TYPE_CODE"] ?? "").ToString();
                            }
                            catch (Exception)
                            {
                            }

                            e.Value = code;
                        }
                        else if (e.Column.FieldName == Gc_MediMateName.FieldName)
                        {
                            string name = "";
                            try
                            {
                                name += (((IDictionary<string, object>)data)["MEDICINE_TYPE_NAME"] ?? "").ToString();
                            }
                            catch (Exception)
                            {
                            }

                            try
                            {
                                name += (((IDictionary<string, object>)data)["MATERIAL_TYPE_NAME"] ?? "").ToString();
                            }
                            catch (Exception)
                            {
                            }

                            e.Value = name;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (System.Dynamic.ExpandoObject)gridView1.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        string code = "";
                        try
                        {
                            code += (((IDictionary<string, object>)data)["ID"] ?? "").ToString();
                        }
                        catch (Exception)
                        {
                        }

                        if (FocusData != null && code == FocusData.MEDI_MATE_TYPE_ID.ToString())
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
