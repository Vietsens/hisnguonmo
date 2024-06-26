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
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.MRSummaryDetail.ADO;

namespace HIS.Desktop.Plugins.MRSummaryDetail.Run
{
	public partial class frmMRSummaryDetail
	{
		private void LoadComboUser()
        {
            try
            {
				LoadDataComboUser(cboUserRecipt);
				LoadDataComboUser(cboUserReciptKHTH);


			}
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void LoadDataComboUser(GridLookUpEdit cbo)
		{
			try
			{
				this.lstReAcsUserADO = new List<AcsUserADO>();

				foreach (var item in Base.GlobalStore.HisAcsUser)
				{
					AcsUserADO ado = new AcsUserADO(item);

					var VhisEmployee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
					if (VhisEmployee != null)
					{
						this.lstReAcsUserADO.Add(ado);
					}
				}
				List<ColumnInfo> columnInfos = new List<ColumnInfo>();
				columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
				columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
				ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 400);
				ControlEditorLoader.Load(cbo, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
				cbo.Properties.ImmediatePopup = true;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
