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
using DevExpress.XtraBars;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisDhst
{
    delegate void PrintMedicine_Click(object sender, ItemClickEventArgs e);

    class PrintPopupMenuProcessor
    {
        PrintMedicine_Click PrintMouseClick;
        BarManager barManager;
        PopupMenu menu;
        V_HIS_DHST dhst;
        long treatmentId;
        string loginName;

        internal enum ModuleType
        {           
            Mps000309,
            Mps000287,
            Mps000293
        }
        
        internal PrintPopupMenuProcessor(PrintMedicine_Click PrintMouseClick, BarManager barManager, long _treatmentId, V_HIS_DHST ado, string _loginName)
        {
            this.PrintMouseClick = PrintMouseClick;
            this.treatmentId = _treatmentId;
            this.barManager = barManager;
            this.dhst = ado;
            this.loginName = _loginName;
        }

        private HIS_TREATMENT LoadDataToCurrentTreatmentData(long treatmentId)
        {
            MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = treatmentId;

                var listTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    treatment = listTreatment[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return treatment;
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem item1 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("HisDhst.btnPrintDHST.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 1);
                item1.Tag = ModuleType.Mps000293;
                item1.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { item1 });

                BarButtonItem item2 = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("HisDhst.btnPrintDangKyKham.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 2);
                item2.Tag = ModuleType.Mps000309;
                item2.ItemClick += new ItemClickEventHandler(PrintMouseClick);
                menu.AddItems(new BarItem[] { item2 });

                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
