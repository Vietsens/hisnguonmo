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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using His.UC.UCHein;
using His.UC.UCHein.Base;
using His.UC.UCHein.ControlProcess;
using His.UC.UCHein.Data;
using His.UC.UCHein.Resources;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateKskContract1
{
    public partial class Template__KskContract1 : UserControl
    {
        internal void UpdateDataFormIntoPatientTypeAlter(HisPatientProfileSDO HisPatientTypeAlter)
        {
            try
            {
                if (HisPatientTypeAlter.HisPatientTypeAlter == null)
                {
                    HisPatientTypeAlter.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
                }              

                if (cboKskContract.EditValue != null)
                {
                   // HisPatientTypeAlter.HisPatientTypeAlter..HisPatyAlterKsk.KSK_CONTRACT_ID = (long)(cboKskContract.EditValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
