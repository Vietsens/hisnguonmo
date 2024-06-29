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
using HIS.UC.TreatmentFinish.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreatmentFinish
{
    public delegate void AutoTreatmentFinish__Checked();
    public delegate void DelegateNextFocus();
    public delegate void CheckedTreatmentFinish();
    public delegate HIS.UC.TreatmentFinish.ADO.DataInputADO DelegateGetDateADO();
    public delegate void CreateEMRVBAOnClick();
    public delegate string DelegateGetStoreStateValue(string key);
    public delegate bool DelegateSetStoreStateValue(string key, string value);
}
