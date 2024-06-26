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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignServiceTestMulti.ADO;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceTestMulti.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {

        /// <summary>
        /// Init set tab index
        /// </summary>
        private void InitTabIndex()
        {
            try
            {
                //dicOrderTabIndexControl.Add("lkAccidentHurtTypeId", 0);
                //dicOrderTabIndexControl.Add("txtContent", 0);
                //dicOrderTabIndexControl.Add("lkTreatmentId", 0);
                //dicOrderTabIndexControl.Add("chkUseAlcohol", 0);
                //dicOrderTabIndexControl.Add("dtAccidentTime", 0);
                //dicOrderTabIndexControl.Add("lkAccidentCareId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentResultId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentHelmetId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentVehicleId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentPoisonId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentLocationId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentBodyPartId", 0);

                //dicOrderTabIndexControl.Add("txtPatientCode", 0);
                //dicOrderTabIndexControl.Add("txtPatientName", 1);

                if (dicOrderTabIndexControl != null && dicOrderTabIndexControl.Count > 0)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        //SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

    }
}
