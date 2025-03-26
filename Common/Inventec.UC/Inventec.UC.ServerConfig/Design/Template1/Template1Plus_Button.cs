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
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ServerConfig.Design.Template1
{
    internal partial class Template1
    {
        private void btnSave_Click(object sender, EventArgs e)
        {         
            try
            {
                SetListElements();
                string acs_Uri = ListData[0].VALUE;
                string fss_Uri = ListData[1].VALUE;
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["Inventec.Token.ClientSystem.Acs.Base.Uri"].Value = acs_Uri;
                config.AppSettings.Settings["fss.uri.base"].Value = fss_Uri;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                bool valid = Data.DataStore.SystemConfigXLM.UpdateElements(ListElement, "");
                if (valid)
                {
                    Data.DataAppConfig.ACS_BASE_URI = System.Configuration.ConfigurationSettings.AppSettings["Inventec.Token.ClientSystem.Acs.Base.Uri"] ?? "";
                    DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thành công.", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    Data.DataStore.LoadSystemConfigKey();
                    if (_CloseForm != null) _CloseForm();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại. Có trục trặc khi gán biến cấu hình hệ thống", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
            }
        }


        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetListElements()
        {
            try
            {
                foreach (var item in (List<Data.DataShow>)gridControlSystemConfig.DataSource)
                {
                    if (item.KEYCODE == ListData[0].KEYCODE)
                    {
                        ListData[0].VALUE = item.VALUE;
                    }
                    else
                    {
                        for (int i = 0; i < ListElement.Count; i++)
                        {
                            if (item.KEYCODE == ListElement[i].KeyCode)
                            {
                                ListElement[i].Value = item.VALUE;
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

        private void gridViewSystemConfig_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    Data.DataShow data = (Data.DataShow)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
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
