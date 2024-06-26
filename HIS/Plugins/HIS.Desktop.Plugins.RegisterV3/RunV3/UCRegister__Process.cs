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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Inventec.Common.Adapter;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private void DefaultGroupCaseType()
        {

            try
            {
                //cboCaseType.EditValue = null;
                //chkOneMonth.Checked = false;
                //chkSick.Checked = false;
                //layoutControlItem25.Visibility = LayoutVisibility.Never;
                //layoutControlItem13.Visibility = LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void FillDataDefaultToControl()
        {
            try
            {
                GetCaseType();
                //this.InitComboCommon(this.cboCashierRoom, this.GetCashierRoomByUser(), "ID", "CASHIER_ROOM_NAME", "CASHIER_ROOM_CODE");
                //this.InitComboCommon(this.cboRoom, new List<V_HIS_EXECUTE_ROOM>(), "ID", "EXECUTE_ROOM_NAME", "EXECUTE_ROOM_CODE");
                //this.InitCombo1Column(this.cboCaseType, this.listCaseTypeAdos, "ID", "NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetCaseType()
        {

            try
            {
                //listCaseTypeAdos = new List<ADO.CaseTypeADO>();
                //listCaseTypeAdos.Add(new ADO.CaseTypeADO(1, "Trẻ em"));
                //listCaseTypeAdos.Add(new ADO.CaseTypeADO(2, "Phụ nữ sau sinh"));
                //listCaseTypeAdos.Add(new ADO.CaseTypeADO(3, "Khác"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultCashierRoom()
        {
            //try
            //{
            //    var listCashier = GetCashierRoomByUser();
            //    if (listCashier != null && listCashier.Count == 1)
            //    {
            //        this.cboCashierRoom.EditValue = listCashier.First().ID;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
        private void DHSTLoadDataDefault()
        {
            try
            {
                dtExecuteTime.DateTime = DateTime.Now;
                spinBloodPressureMin.EditValue = null;
                spinBloodPressureMax.EditValue = null;
                spinBreathRate.EditValue = null;
                spinHeight.EditValue = null;
                spinChest.EditValue = null;
                spinBelly.EditValue = null;
                spinPulse.EditValue = null;
                spinTemperature.EditValue = null;
                spinWeight.EditValue = null;
                spinSPO2.EditValue = null;
                txtNote.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void DHSTFillDataToBmiAndLeatherArea()
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.Value != null && spinHeight.Value != 0)
                {
                    bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
                }
                double leatherArea = 0.007184 * Math.Pow((double)spinHeight.Value, 0.725) * Math.Pow((double)spinWeight.Value, 0.425);
                lblBMI.Text = Math.Round(bmi, 2) + "";
                lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                if (bmi < 16)
                {
                    lblBmiDisplayText.Text = "(Gầy độ III)";
                }
                else if (16 <= bmi && bmi < 17)
                {
                    lblBmiDisplayText.Text = "(Gầy độ II)";
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    lblBmiDisplayText.Text = "(Gầy độ I)";

                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    lblBmiDisplayText.Text = "(Bình thường)";

                }
                else if (25 <= bmi && bmi < 30)
                {
                    lblBmiDisplayText.Text = "(Thừa cân)";
                }
                else if (30 <= bmi && bmi < 35)
                {
                    lblBmiDisplayText.Text = "(Béo phì độ I)";

                }
                else if (35 <= bmi && bmi < 40)
                {
                    lblBmiDisplayText.Text = "(Béo phì độ II)";
                }
                else if (40 < bmi)
                {
                    lblBmiDisplayText.Text = "(Béo phì độ III)";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
