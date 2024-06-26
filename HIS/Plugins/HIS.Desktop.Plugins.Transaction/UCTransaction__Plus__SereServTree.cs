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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Transaction
{
    public partial class UCTransaction : UserControlBase
    {
        private void treeSereServ_GetSelectImage(SereServADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (!e.Node.HasChildren)
                    {
                        if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)//Chưa xử lý
                        {
                            e.NodeImageIndex = 2;
                        }
                        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)//Đã xử lý
                        {
                            e.NodeImageIndex = 3;
                        }
                        else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)//Hoàn thành
                        {
                            e.NodeImageIndex = 4;
                        }
                        else
                        {
                            e.NodeImageIndex = -1;
                        }
                    }
                    else
                    {
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_StateImageClick(SereServADO data)
        {
            try
            {
                if (data == null || data.ID <= 0)
                    return;
                if (dicSereServBill.ContainsKey(data.ID))
                {
                    return;
                }
                //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DiscountSereServ").FirstOrDefault();
                //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.DiscountSereServ'");

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Exemptions").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.Exemptions'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, data);
                    listArgs.Add(sereServ);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("extenceInstance is null");

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<long> listSereServID_DaTamUngDichVu;
        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                //Bổ sung màu cam để phân biệt đối với dịch vụ đã được tạm ứng dịch vụ
                if (!e.Node.HasChildren && this.listSereServID_DaTamUngDichVu != null && this.listSereServID_DaTamUngDichVu.Contains(data.ID))
                {
                    e.Appearance.ForeColor = Color.Orange;
                }

                if (data != null && e.Node.ParentNode == null)
                {

                }
                else if (data != null && e.Node.HasChildren && e.Column.FieldName != "VAT")
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                }
                else if (data != null && !e.Node.HasChildren)
                {
                    if (data.IS_NO_EXECUTE == 1)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                    }
                    else if (data.VIR_TOTAL_PATIENT_PRICE.HasValue && data.VIR_TOTAL_PATIENT_PRICE.Value > 0)
                    {
                        if (data.INVOICE_ID.HasValue)
                            e.Appearance.ForeColor = Color.Red;
                        else if (dicSereServBill.ContainsKey(data.ID))
                        {
                            var ssBills = dicSereServBill[data.ID];
                            var totalPrice = ssBills.Sum(s => s.PRICE);
                            if (totalPrice >= data.VIR_TOTAL_PATIENT_PRICE)
                                e.Appearance.ForeColor = Color.Blue;
                            else
                                e.Appearance.ForeColor = Color.Green;
                        }
                        else if (e.Node.Checked)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else
                        {
                            //e.Appearance.ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
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
