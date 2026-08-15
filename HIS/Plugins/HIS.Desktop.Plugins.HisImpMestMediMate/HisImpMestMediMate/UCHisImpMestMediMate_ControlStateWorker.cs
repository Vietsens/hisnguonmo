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
using HIS.Desktop.Plugins.HisImpMestMediMate.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.HisImpMestMediMate
{
    public partial class UCHisImpMestMediMate : HIS.Desktop.Utility.UserControlBase
    {
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private bool isNotLoadWhileChangeControlStateInFirst;

        /// <summary>
        /// Nho lai doi tuong tra cuu (Thuoc / Vat tu) cua lan lam viec truoc.
        /// Mac dinh la Thuoc khi chua co trang thai luu.
        /// </summary>
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this._Module.ModuleLink);

                bool isMedicine = true;
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    var item = this.currentControlStateRDO
                        .Where(o => o.KEY == ControlStateConstant.chkMedicine)
                        .FirstOrDefault();
                    if (item != null)
                    {
                        isMedicine = item.VALUE == "1";
                    }
                }

                this.chkMedicine.Checked = isMedicine;
                this.chkMaterial.Checked = !isMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                isNotLoadWhileChangeControlStateInFirst = false;
            }
        }

        private void SaveControlState()
        {
            try
            {
                if (this.controlStateWorker == null) return;

                string value = this.chkMedicine.Checked ? "1" : "0";

                var csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO
                        .Where(o => o.KEY == ControlStateConstant.chkMedicine && o.MODULE_LINK == this._Module.ModuleLink)
                        .FirstOrDefault()
                    : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkMedicine;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = this._Module.ModuleLink;

                    if (this.currentControlStateRDO == null)
                    {
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    }
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
