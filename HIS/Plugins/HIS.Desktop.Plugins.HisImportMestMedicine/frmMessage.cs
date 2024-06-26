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

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
	public partial class frmMessage : HIS.Desktop.Utility.FormBase
	{
		Action<bool> IsCheckSayYes { get; set; }
		V_HIS_IMP_MEST currentImpMest { get;set;}

		private Common.RefeshReference delegateRefresh { get; set; }
		public frmMessage(Action<bool> IsSayYes, V_HIS_IMP_MEST cuImpMest, Common.RefeshReference delegateRefresh)
		{
			InitializeComponent();
			try
			{
				this.IsCheckSayYes = IsSayYes;
				this.currentImpMest = cuImpMest;
				this.delegateRefresh = delegateRefresh;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
 		}

		public bool SucesssApproval = false;
		private void frmMessage_Load(object sender, EventArgs e)
		{
			try
			{
				dteTime.DateTime = DateTime.Now;

			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			try
			{				
				long timeDte = 0;
				if (dteTime.EditValue != null && dteTime.DateTime != DateTime.MinValue)
				{
					timeDte = Convert.ToInt64(dteTime.DateTime.ToString("yyyyMMdd"));
					if (!SucesssApproval && currentImpMest.APPROVAL_TIME != null && timeDte < Convert.ToInt64(currentImpMest.APPROVAL_TIME.ToString().Substring(0,8)))
					{
						if(DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian nhập nhỏ hơn thời gian duyệt. Bạn có muốn cập nhật lại thời gian duyệt không?", Resources.ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							CommonParam param = new CommonParam();
							MOS.EFMODEL.DataModels.HIS_IMP_MEST EVImportMest = new MOS.EFMODEL.DataModels.HIS_IMP_MEST();
							Inventec.Common.Mapper.DataObjectMapper.Map
								<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
								(EVImportMest, currentImpMest);

							EVImportMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL;
							var apiresul = new Inventec.Common.Adapter.BackendAdapter
								(param).Post<MOS.EFMODEL.DataModels.HIS_IMP_MEST>
								("api/HisImpMest/UpdateStatus", ApiConsumer.ApiConsumers.MosConsumer, EVImportMest, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
							if (apiresul != null)
							{
								SucesssApproval = true;
								if (delegateRefresh!=null)
									delegateRefresh();
							}
						}
						else
						{
							this.Close();
							return;
						}
					}
				}
				else
				{
					return;
				}
				long timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd"));
				if (timeDte > timeNow)
				{
					DevExpress.XtraEditors.XtraMessageBox.Show(
						"Không cho phép ngày nhập lớn hơn ngày hiện tại",
				   Resources.ResourceMessage.ThongBao,
				   MessageBoxButtons.OK);
					dteTime.Focus();
					dteTime.SelectAll();
					return;
				}
				else
				{
					if (IsCheckSayYes != null)
					{
						UCHisImportMestMedicine.TimeImpFromMessage = 0;
						UCHisImportMestMedicine.TimeImpFromMessage = Convert.ToInt64(dteTime.DateTime.ToString("yyyyMMddHHmmss"));
						IsCheckSayYes(true);
						this.Close();
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			try
			{
				this.Close();
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}
	}
}
