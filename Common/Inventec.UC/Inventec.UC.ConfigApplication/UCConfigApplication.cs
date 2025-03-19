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
using System.Xml.Linq;
using Inventec.Core;
using Inventec.Common.XmlConfig;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Common.Logging;

namespace Inventec.UC.ConfigApplication
{
    public delegate void Refesh();
    public partial class UCConfigApplication : UserControl
    {
        public static XmlApplicationConfig ApplicationConfig { get; set; }
        string language;
        string loginName;
        Refesh refesh;
        public UCConfigApplication(string language, string loginName, Refesh refesh)
        {
            InitializeComponent();
            this.language = language;
            this.refesh = refesh;
            this.loginName = loginName;
        }

        public void LoadKeyFromLanguage()
        {
            try
            {
                //btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCCONFIG_APPLICATION_BTN_SAVE", EXE.APP.Resources.ResourceLanguageManager.LanguageUcConfigApplication, EXE.MANAGER.Base.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCConfigApplication_Load(object sender, EventArgs e)
        {
            try
            {
                btnSave.Focus();
                LoadKeyFromLanguage();

                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                string pathXmlFileConfig = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), @"ConfigApplication.xml");
                ApplicationConfig = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFileConfig, this.language);
                gridControlConfigApplication.DataSource = ApplicationConfig.GetElements();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewConfigApplication_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "ModifyTimeDisplay")
                    {
                        try
                        {
                            long createTime = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.ListSourceRowIndex, "ModifyTime") ?? "").ToString());
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((createTime));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ValueDisplay")
                    {
                        try
                        {
                            string value = (view.GetRowCellValue(e.ListSourceRowIndex, "Value") ?? "").ToString();
                            string defaultValue = (view.GetRowCellValue(e.ListSourceRowIndex, "DefaultValue") ?? "").ToString();
                            e.Value = (String.IsNullOrEmpty(value) ? defaultValue : value);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ValueAllow")
                    {
                        try
                        {
                            string valueAllowMin = (view.GetRowCellValue(e.ListSourceRowIndex, "ValueAllowMin") ?? "").ToString();
                            string valueAllowMax = (view.GetRowCellValue(e.ListSourceRowIndex, "ValueAllowMax") ?? "").ToString();
                            string valueAllowIn = (view.GetRowCellValue(e.ListSourceRowIndex, "ValueAllowIn") ?? "").ToString();
                            e.Value += "[";
                            if (!String.IsNullOrEmpty(valueAllowMin))
                            {
                                e.Value += "" + valueAllowMin;
                            }
                            if (!String.IsNullOrEmpty(valueAllowMax))
                            {
                                e.Value += " -> " + valueAllowMax;
                            }
                            e.Value += "]";
                            if (String.IsNullOrEmpty(valueAllowMin) && String.IsNullOrEmpty(valueAllowMax))
                                e.Value = "";
                            if (!String.IsNullOrEmpty(valueAllowIn))
                            {
                                if (!String.IsNullOrEmpty((e.Value ?? "").ToString()))
                                    e.Value += " | ";
                                e.Value += "[" + valueAllowIn + "]";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    gridViewConfigApplication.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        internal void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (this.gridViewConfigApplication.IsEditing)
                    this.gridViewConfigApplication.CloseEditor();

                if (this.gridViewConfigApplication.FocusedRowModified)
                    this.gridViewConfigApplication.UpdateCurrentRow();

                success = ApplicationConfig.UpdateElements((List<Inventec.Common.XmlConfig.ElementNode>)(gridControlConfigApplication.DataSource), loginName);
                if (success)
                {
                    if (refesh != null)
                        refesh();
                }
                else
                {
                    LogSystem.Warn("Update ApplicationConfig error");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewConfigApplication_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view.FocusedColumn.FieldName == "Value")
                {
                    string valueType = gridViewConfigApplication.GetRowCellValue(gridViewConfigApplication.FocusedRowHandle, "ValueType").ToString();
                    string strValueAllowMin = gridViewConfigApplication.GetRowCellValue(gridViewConfigApplication.FocusedRowHandle, "ValueAllowMin").ToString();
                    string strValueAllowMax = gridViewConfigApplication.GetRowCellValue(gridViewConfigApplication.FocusedRowHandle, "ValueAllowMax").ToString();
                    string strValueAllowIn = gridViewConfigApplication.GetRowCellValue(gridViewConfigApplication.FocusedRowHandle, "ValueAllowIn").ToString();
                    if (e.Value != null && e.Value.ToString().Trim() != "")
                    {
                        switch (valueType)
                        {
                            case "long":
                                long lValue = Convert.ToInt64(e.Value);
                                if (!String.IsNullOrEmpty(strValueAllowMin))
                                {
                                    long valueMin = Convert.ToInt64(strValueAllowMin);
                                    if (lValue < valueMin)
                                    {
                                        e.Valid = false;
                                        e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowMax))
                                {
                                    long valueMax = Convert.ToInt64(strValueAllowMax);
                                    if (lValue > valueMax)
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                    }
                                }
                                break;
                            case "string":
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                    }
                                }
                                break;
                            case "short":
                                short shValue = Convert.ToInt16(e.Value);
                                if (!String.IsNullOrEmpty(strValueAllowMin))
                                {
                                    long valueMin = Convert.ToInt16(strValueAllowMin);
                                    if (shValue < valueMin)
                                    {
                                        e.Valid = false;
                                        e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowMax))
                                {
                                    long valueMax = Convert.ToInt16(strValueAllowMax);
                                    if (shValue > valueMax)
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                    }
                                }
                                break;
                            case "decimal":
                                decimal decValue = Convert.ToDecimal(e.Value);
                                if (!String.IsNullOrEmpty(strValueAllowMin))
                                {
                                    decimal valueMin = Convert.ToDecimal(strValueAllowMin);
                                    if (decValue < valueMin)
                                    {
                                        e.Valid = false;
                                        e.ErrorText += "Giá trị nhập vào nhỏ hơn giá trị nhỏ nhất cho phép";
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowMax))
                                {
                                    decimal valueMax = Convert.ToDecimal(strValueAllowMax);
                                    if (decValue > valueMax)
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào vượt quá giá trị lớn nhất cho phép";
                                    }
                                }
                                if (!String.IsNullOrEmpty(strValueAllowIn))
                                {
                                    if (!strValueAllowIn.StartsWith(",") && !strValueAllowIn.StartsWith(";"))
                                        strValueAllowIn = "," + strValueAllowIn;
                                    if (!strValueAllowIn.EndsWith(",") && !strValueAllowIn.EndsWith(";"))
                                        strValueAllowIn = strValueAllowIn + ",";
                                    if (!strValueAllowIn.Contains("," + e.Value.ToString() + ",") && !strValueAllowIn.Contains(";" + e.Value.ToString() + ";"))
                                    {
                                        e.Valid = false;
                                        if (!String.IsNullOrEmpty(e.ErrorText))
                                            e.ErrorText += "\r\n";
                                        e.ErrorText += "Giá trị nhập vào không nằm trong danh sách các giá trị cho phép";
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                e.Valid = false;
                e.ErrorText += "Giá trị nhập vào không đúng định dạng";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewConfigApplication_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                //Do not perform any default action 
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                //Show the message with the error text specified 
                MessageBox.Show(e.ErrorText);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
