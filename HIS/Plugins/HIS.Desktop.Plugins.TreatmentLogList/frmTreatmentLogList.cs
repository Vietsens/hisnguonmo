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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentLogList
{
 
 public partial class frmTreatmentLogList : HIS.Desktop.Utility.FormBase
 {
long TreatmentId = 0;
CommonParam param = new CommonParam();
Inventec.Desktop.Common.Modules.Module module;
long currentRoomId = 0;
public frmTreatmentLogList(Inventec.Desktop.Common.Modules.Module Module, long treatmentId, long currentRoomId)
:base(Module)
  {
   TreatmentId = treatmentId;
   module = Module;
   this.currentRoomId = currentRoomId;
   InitializeComponent();

   UCTreatmentProcessPartial UCtreatmentProcessPartial = new UCTreatmentProcessPartial(module, TreatmentId, currentRoomId);
   this.xtraUserControl1.Controls.Add(UCtreatmentProcessPartial);
UCtreatmentProcessPartial.Dock= DockStyle.Fill;

  }

 
 }
}
