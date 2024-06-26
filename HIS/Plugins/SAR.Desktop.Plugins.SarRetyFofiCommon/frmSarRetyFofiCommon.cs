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
using Inventec.Common.Logging;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAR.Filter;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;

namespace SAR.Desktop.Plugins.SarRetyFofiCommon
{
    public partial class frmSarRetyFofiCommon : Form
    {
        public List<FilterCriteriaRdo> listFilerCriteria = new List<FilterCriteriaRdo>();// danh sách tiêu chí bộ lọc
        public List<CompareRdo> listCompare = new List<CompareRdo>();// danh sách hình thức so sánh
        public List<SAR_RETY_FOFI> apiResult = new List<SAR_RETY_FOFI>();
        public CommonParam param = new CommonParam();
        public frmSarRetyFofiCommon()
        {
            InitializeComponent();
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
            }
        }
        
        

        #region lấy dữ liệu từ excel
        public void GetComparisonMethodExcel()
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(FilePath.FileFilter);
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook.Sheets[2];
                Microsoft.Office.Interop.Excel.Range xlRange = xlWorkSheet.UsedRange;
                object[,] Array = ((object[,])xlRange.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault));
                for (int row = 2; row <= xlWorkSheet.UsedRange.Rows.Count; ++row)
                {

                    for (int col = 1; col <= xlWorkSheet.UsedRange.Columns.Count; ++col)
                    {
                        string NameFilter = Array[row, 1].ToString();
                        string valueName = Array[row, 2].ToString();
                        CompareRdo compare = new CompareRdo();
                        compare.DisplayMember = NameFilter;
                        compare.ValueMember = valueName;
                        listCompare.Add(compare);
                    }

                }
                lockUpEditCompare.Properties.DataSource = listCompare.Select(x => new { x.DisplayMember, x.ValueMember }).Distinct().ToList();
                lockUpEditCompare.Properties.DisplayMember = "DisplayMember";
                lockUpEditCompare.Properties.ValueMember = "ValueMember";
            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Error(ex.Message);
            }
            
        }
        public void GetDataFilterCriteriaExcel()// lấy dữ liệu tiêu chí bộ lọc
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(FilePath.FileFilter);
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook.Sheets[1];
                Microsoft.Office.Interop.Excel.Range xlRange = xlWorkSheet.UsedRange;
                object[,] Array = ((object[,])xlRange.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault));
                for (int row = 2; row <= xlWorkSheet.UsedRange.Rows.Count; ++row)
                {
                    for (int col = 1; col <= xlWorkSheet.UsedRange.Columns.Count; ++col)
                    {

                        string NameFilter = Array[row, 1].ToString();
                        string valueFilter = Array[row, 2].ToString();
                        string valueNameFilter = Array[row, 3].ToString();
                        FilterCriteriaRdo filterCriteria = new FilterCriteriaRdo();
                        filterCriteria.NAME_FILTER = NameFilter;
                        filterCriteria.VALUE_FILTER = valueFilter;
                        filterCriteria.VALUE_NAME_FILTER = valueNameFilter;
                        listFilerCriteria.Add(filterCriteria);
                    }
                }
                foreach (var item in listFilerCriteria.Select(x => x.NAME_FILTER).Distinct())
                {
                    lstBox_TieuChiFilter.Items.Add(item.ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex.Message);
            }

        }
        #endregion
        public void GetDataFilterCriteria()
        {
            Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.SAR_RETY_FOFI>> apiResult = null;
            SarRetyFofiViewFilter filter = new SarRetyFofiViewFilter();
            filter.IS_ACTIVE = 0;
            apiResult = new BackendAdapter(param).GetRO<List<SAR.EFMODEL.DataModels.SAR_RETY_FOFI>>(SdaRequestUriStore.GET_SAR_RETY_FOFI, ApiConsumers.SarConsumer, filter, param);
            lstBox_TieuChiFilter.DataSource = apiResult.Data;
            lstBox_TieuChiFilter.DisplayMember = "DESCRIPTION";
            lstBox_TieuChiFilter.ValueMember = "FORM_FIELD_ID";
        }
        public void GetComparisonMethod()
        {

        }
        public void GetDataCombo() {
            cbb_danhSach.Items.Add("Thuộc danh sách");
            cbb_danhSach.Items.Add("Không thuộc danh sách");
        }
        private void frmMrsFilter_Load(object sender, EventArgs e)
        {
            try
            {
                GetDataFilterCriteria();
                GetComparisonMethod();
                GetDataCombo();
            }
            catch (Exception ex)
            {
              Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ChangeFilterCriteria() {
            try
            {
                Inventec.Core.ApiResultObject<List<SAR.EFMODEL.DataModels.SAR_RETY_FOFI>> apiResult = null;
                SarRetyFofiViewFilter filter = new SarRetyFofiViewFilter();
                filter.ID = int.Parse(lstBox_TieuChiFilter.SelectedValue.ToString());
                apiResult = new BackendAdapter(param).GetRO<List<SAR.EFMODEL.DataModels.SAR_RETY_FOFI>>(SdaRequestUriStore.GET_SAR_RETY_FOFI, ApiConsumers.SarConsumer, filter, param);
                var listValue = apiResult.Data.Where(x => x.ID == int.Parse(lstBox_TieuChiFilter.SelectedValue.ToString())).ToList();
                //var listValue = listFilerCriteria.Where(x => x.NAME_FILTER == lstBox_TieuChiFilter.SelectedItem.ToString()).Select(x => new { x.NAME_FILTER, x.VALUE_FILTER, x.VALUE_NAME_FILTER }).Distinct().ToList();
                //lstBox_GiaTriFilter.DataSource = listValue;
                //lstBox_GiaTriFilter.DisplayMember = "VALUE_FILTER";
                //lstBox_GiaTriFilter.ValueMember = "VALUE_NAME_FILTER";
                lstBox_GiaTriFilter.DataSource = listValue.First().SAR_FORM_FIELD;
                lstBox_GiaTriFilter.DisplayMember = "DESCRIPTION";
                lstBox_GiaTriFilter.ValueMember = "SAR_FORM_FIELD";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
            }
        }
        public void ChangeValueFilter() {
            try
            {
                txt_GiaTriFIlter.Text = lstBox_GiaTriFilter.SelectedValue.ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
            }
        }
        private void lstBox_TieuChiFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ChangeFilterCriteria();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void lstBox_GiaTriFilter_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                txt_GiaTriFIlter.Text = lstBox_GiaTriFilter.SelectedValue.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lstBox_TieuChiFilter_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ChangeFilterCriteria();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
            }
        }

        private void lstBox_TieuChiFilter_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                ChangeFilterCriteria();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
            }
        }

        private void lstBox_GiaTriFilter_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ChangeValueFilter();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
            }
        }

        private void lstBox_GiaTriFilter_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                ChangeValueFilter();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex.Message);
            }
        }
    }
}
