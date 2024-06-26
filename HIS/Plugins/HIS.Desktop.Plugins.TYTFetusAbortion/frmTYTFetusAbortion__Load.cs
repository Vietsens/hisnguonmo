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

namespace TYT.Desktop.Plugins.FetusAbortion
{
    public partial class frmTYTFetusAbortion : FormBase
    {

        private void LoadDataDefault()
        {
            try
            {
                spinParaChildCount.EditValue = null;
                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                txtExecuteName.Text = userName;
                InitLabelSave();
                LoadDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitLabelSave()
        {
            try
            {
                //if (actionType == TYPE.CREATE)
                //{
                //    btnSave.Text = "Lưu (Ctrl S)";
                //}
                //else if (actionType == TYPE.UPDATE)
                //{
                //    btnSave.Text = "Sửa (Ctrl S)";
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToGrid()
        {
            try
            {
                CommonParam param = new CommonParam();
                TytFetusAbortionFilter filter = new TytFetusAbortionFilter();
                filter.PATIENT_CODE__EXACT = PatientCode;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                lstfetusAbortion = new BackendAdapter(param).Get<List<TYT_FETUS_ABORTION>>("api/TytFetusAbortion/Get", ApiConsumers.TytConsumer, filter, param);
                //if (lstfetusAbortion == null || lstfetusAbortion.Count == 0)
                //{
                //    MessageBox.Show("Không tìm thấy thông tin phá thai", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}
                gridControl1.DataSource = lstfetusAbortion;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToControl()
        {
            if (actionType==TYPE.CREATE)
            {
                btnAdd.Enabled = true;
                btnSave.Enabled = false;
            }
            else
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
            }
            if (fetusAbortion.LAST_MENSES_TIME.HasValue)
            {
                dtLastMensesTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(fetusAbortion.LAST_MENSES_TIME ?? 0) ?? DateTime.Now;
            }
            else
            {
                dtLastMensesTime.EditValue = null;
            }
            if (fetusAbortion.ABORTION_TIME.HasValue)
            {
                dtAbortionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(fetusAbortion.ABORTION_TIME ?? 0) ?? DateTime.Now;
            }
            else
            {
                dtAbortionTime.EditValue = null;
            }
            chkIsSingle.CheckState = fetusAbortion.IS_SINGLE == 1 ? CheckState.Checked : CheckState.Unchecked;
            if (fetusAbortion.PARA_CHILD_COUNT.HasValue)
            {
                spinParaChildCount.EditValue = fetusAbortion.PARA_CHILD_COUNT;
            }
            else
            {
                spinParaChildCount.EditValue = null;
            }

            if (fetusAbortion.DIAGNOSE_TEST.HasValue)
            {
                cboDiagnoseTest.SelectedIndex = (int)fetusAbortion.DIAGNOSE_TEST - 1;
            }
            else
            {
                cboDiagnoseTest.EditValue = null;
            }

            if (fetusAbortion.SM_RESULT.HasValue)
            {
                cboSMResult.SelectedIndex = (int)fetusAbortion.SM_RESULT - 1;
            }
            else
            {
                cboSMResult.EditValue = null;
            }

            mmAbortionMethod.Text = fetusAbortion.ABORTION_METHOD;
            mmObstetricComplication.Text = fetusAbortion.OBSTETRIC_COMPLICATION;
            chkIsDeath.CheckState = fetusAbortion.IS_DEATH == 1 ? CheckState.Checked : CheckState.Unchecked;
            chkExamAfterTwoWeek.CheckState = fetusAbortion.EXAM_AFTER_TWO_WEEK == 1 ? CheckState.Checked : CheckState.Unchecked;
            if (!String.IsNullOrEmpty(fetusAbortion.EXECUTE_NAME))
            {
                txtExecuteName.Text = fetusAbortion.EXECUTE_NAME;
            }
            else
            {
                txtExecuteName.Text ="";
            }

            mmNote.Text = fetusAbortion.NOTE;
        }
    }
}
