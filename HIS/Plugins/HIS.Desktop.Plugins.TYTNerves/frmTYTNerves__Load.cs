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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using TYT.Desktop.Plugins.Nerves.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYT.EFMODEL.DataModels;
using TYT.Filter;

namespace TYT.Desktop.Plugins.Nerves
{
    public partial class frmTYTNerves : FormBase
    {

        private void LoadDataDefault()
        {
            try
            {
                LoadNam();
                LoadDataForEdit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadNam()
        {
            try
            {
                if (actionType == TYPE.CREATE)
                {
                    DateTime dt = DateTime.Now;
                    txtYear.Text = dt.Year.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataForEdit()
        {
            try
            {
                if (actionType == TYPE.UPDATE && (this.currentData != null || nervesId > 0))
                {
                    mmTamThanPhanLiet.Text = currentData.DIAGNOSE_TTPL;
                    mmTramCam.Text = currentData.DIAGNOSE_TC;
                    mmDongKinh.Text = currentData.DIAGNOSE_DK;
                    txtYear.Value = currentData.YEAR;
                    if (currentData.PHCN_RESULT.HasValue)
                    {
                        cboPHCN.SelectedIndex = (int)currentData.PHCN_RESULT.Value - 1;
                        cboPHCN.Properties.Buttons[1].Visible = true;
                    }

                    chkHomeCheck.CheckState = currentData.IS_HOME_CHECK == 1 ? CheckState.Checked : CheckState.Unchecked;

                    if (!String.IsNullOrEmpty(currentData.MONTHS))//nerves.MEDICINE_MONITOR
                    {
                        List<string> listMonthWithYearTemp = JsonConvert.DeserializeObject<List<string>>(currentData.MONTHS);//nerves.MEDICINE_MONITOR

                        if (listMonthWithYearTemp != null && listMonthWithYearTemp.Count > 0)
                        {
                            List<MonthADO> listMonthADO = (from r in listMonthWithYearTemp select new MonthADO(r)).ToList();
                            if (listMonthADO != null && listMonthADO.Count > 0)
                            {
                                foreach (var item in listMonthADO)
                                {
                                    CheckMonth(item.Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        //txtNam.Text = DateTime.Now.Year.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckMonth(long id)
        {
            try
            {
                switch (id)
                {
                    case 1:
                        chk1.CheckState = CheckState.Checked;
                        break;
                    case 2:
                        chk2.CheckState = CheckState.Checked;
                        break;
                    case 3:
                        chk3.CheckState = CheckState.Checked;
                        break;
                    case 4:
                        chk4.CheckState = CheckState.Checked;
                        break;
                    case 5:
                        chk5.CheckState = CheckState.Checked;
                        break;
                    case 6:
                        chk6.CheckState = CheckState.Checked;
                        break;
                    case 7:
                        chk7.CheckState = CheckState.Checked;
                        break;
                    case 8:
                        chk8.CheckState = CheckState.Checked;
                        break;
                    case 9:
                        chk9.CheckState = CheckState.Checked;
                        break;
                    case 10:
                        chk10.CheckState = CheckState.Checked;
                        break;
                    case 11:
                        chk11.CheckState = CheckState.Checked;
                        break;
                    case 12:
                        chk12.CheckState = CheckState.Checked;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
