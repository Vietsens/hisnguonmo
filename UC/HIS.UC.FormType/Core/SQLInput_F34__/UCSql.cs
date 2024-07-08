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
//using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.Core.SQLInput_F34__
{
    public partial class UCSql : DevExpress.XtraEditors.XtraUserControl
    {
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;
        bool isValidData = false;
        List<SqlAdo> LstData = new List<SqlAdo>();
        SqlAdo CurrentRow = new SqlAdo();
        bool AllFilterNotNull = true;

        string OutPut = "\"FILTER\":[{0}]";

        public UCSql(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            InitializeComponent();
            try
            {
                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);

                SetTitle();

                if (this.isValidData)
                {
                    Validation();
                    LciUcSql.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Validation()
        {
            try
            {
                AllFilterNotNull = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    LciUcSql.Text = this.config.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCSql_Load(object sender, EventArgs e)
        {
            try
            {
                InitData();

                if (this.report != null)
                {
                    SetValue();
                }

                ProcessGridData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGridData()
        {
            try
            {
                gridControl1.BeginUpdate();
                gridControl1.DataSource = null;
                gridControl1.DataSource = LstData;
                gridControl1.EndUpdate();

                if (LstData != null && LstData.Count > 0)
                    FillDataToControl(LstData.First());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitData()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null)
                {
                    //"{\"FILTER\":[\"a:b\",\"c:d\"]}"
                    var filter = this.config.JSON_OUTPUT.Split(';');
                    foreach (var item in filter)
                    {
                        SqlAdo ado = new SqlAdo();
                        ado.NAME = item;

                        LstData.Add(ado);
                    }

                    LstData = LstData.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValue()
        {
            try
            {
                if (LstData != null && LstData.Count > 0 && this.report.JSON_FILTER != null)
                {
                    //"{\"FILTER\":[\"a:b\",\"c:d\"]}"
                    var filter = this.report.JSON_FILTER.Substring(report.JSON_FILTER.IndexOf(':') + 1).Replace("\"", "").TrimEnd('}').TrimStart('[').TrimEnd(']').Split(',');
                    foreach (var item in filter)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            var filterValue = item.Split(':');
                            if (filterValue.Length == 2)
                            {
                                foreach (var ado in LstData)
                                {
                                    if (ado.NAME == filterValue[0])
                                    {
                                        if (filterValue[1] != "null")
                                            ado.VALUE = filterValue[1];

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (SqlAdo)gridView1.GetFocusedRow();
                if (row != null)
                {
                    //update dữ liệu trước khi gán mới
                    UpdateDataToRow(CurrentRow);
                    FillDataToControl(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDataToRow(SqlAdo CurrentRow)
        {
            try
            {
                if (CurrentRow != null)
                {
                    foreach (var item in LstData)
                    {
                        if (item.NAME == CurrentRow.NAME)
                        {
                            item.VALUE = TxtValue.Text.Trim();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl(SqlAdo data)
        {
            try
            {
                CurrentRow = data;
                if (data != null)
                {
                    TxtName.Text = data.NAME;
                    TxtValue.Text = data.VALUE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (AllFilterNotNull)
                {
                    UpdateDataToRow(CurrentRow);

                    foreach (var item in LstData)
                    {
                        if (String.IsNullOrWhiteSpace(item.VALUE))
                        {
                            item.ErrorMessageValue = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                            item.ErrorValue = ErrorType.Warning;
                            result = false;
                        }
                        else
                        {
                            item.ErrorMessageValue = "";
                            item.ErrorValue = ErrorType.None;
                        }
                    }

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public string GetValue()
        {
            string result = "";
            try
            {
                List<string> values = new List<string>();
                UpdateDataToRow(CurrentRow);

                if (LstData != null && LstData.Count > 0)
                {
                    foreach (var item in LstData)
                    {
                        values.Add(string.Format("\"{0}:{1}\"", item.NAME, String.IsNullOrWhiteSpace(item.VALUE) ? "null" : item.VALUE));
                    }
                }

                result = String.Format(OutPut, string.Join(",", values));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void gridView1_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridView1.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridView1.DataSource as List<SqlAdo>;
                var row = listDatas[index];
                if (e.ColumnName == Gc_Name.FieldName)
                {
                    if (row.ErrorValue == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorValue);
                        e.Info.ErrorText = (string)(row.ErrorMessageValue);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
