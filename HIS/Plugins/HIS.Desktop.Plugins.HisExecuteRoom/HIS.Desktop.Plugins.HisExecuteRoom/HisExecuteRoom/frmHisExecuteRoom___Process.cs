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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.HisExecuteRoom.HisExecuteRoom
{
    public partial class frmHisExecuteRoom : FormBase
    {
        private HIS_EXECUTE_ROOM SetDataExecuteRoom()
        {
            HIS_EXECUTE_ROOM executeRoom = new HIS_EXECUTE_ROOM();
            try
            {
                if (!String.IsNullOrEmpty(txtExecuteRoomCode.Text))
                    executeRoom.EXECUTE_ROOM_CODE = txtExecuteRoomCode.Text;
                if (!String.IsNullOrEmpty(txtExecuteRoomName.Text))
                    executeRoom.EXECUTE_ROOM_NAME = txtExecuteRoomName.Text;
                executeRoom.IS_EMERGENCY = (short)(chkIsEmergency.Checked ? 1 : 0);
                executeRoom.IS_PAUSE_ENCLITIC = (short)(chkIsPauseEnclitic.Checked ? 1 : 0);
                executeRoom.IS_SPECIALITY = (short)(chkIsSpeciality.Checked ? 1 : 0);
                executeRoom.IS_SURGERY = (short)(chkIsSurgery.Checked ? 1 : 0);
                executeRoom.IS_EXAM = (short)(chkIsExam.Checked ? 1 : 0);
                executeRoom.ALLOW_NOT_CHOOSE_SERVICE = (short)(chkKhongCanChonDV.Checked ? 1 : 0);
                executeRoom.IS_AUTO_EXPEND_ADD_EXAM = (short)(chkIsExamPlus.Checked ? 1 : 0);
                executeRoom.IS_VACCINE = (short)(chkVaccine2.Checked ? 1 : 0);
                executeRoom.IS_VACCINE = (short)(chkVaccine2.Checked ? 1 : 0);
                executeRoom.TEST_TYPE_CODE = txtTestTypeCode.Text.Trim();
                if (spSTT.EditValue != null)
                {
                    executeRoom.NUM_ORDER = (long)spSTT.Value;
                }
                if (spMaxRequestByDay.EditValue != null)
                {
                    executeRoom.MAX_REQUEST_BY_DAY = (long)spMaxRequestByDay.Value;
                }
                else
                {
                    executeRoom.MAX_REQUEST_BY_DAY = null;
                }

                if (spMaxPatientByDay.EditValue != null)
                {
                    executeRoom.MAX_PATIENT_BY_DAY = (long)spMaxPatientByDay.Value;
                }
                else
                {
                    executeRoom.MAX_PATIENT_BY_DAY = null;
                }

                if (spMaxReqBhytByDay.EditValue != null)
                {
                    executeRoom.MAX_REQ_BHYT_BY_DAY = (long)spMaxReqBhytByDay.Value;
                }
                else
                {
                    executeRoom.MAX_REQ_BHYT_BY_DAY = null;
                }

                if (spinMaxAppointment.EditValue != null)
                {
                    executeRoom.MAX_APPOINTMENT_BY_DAY = (long)spinMaxAppointment.Value;
                }
                else
                {
                    executeRoom.MAX_APPOINTMENT_BY_DAY = null;
                }
                if (spAVERAGE_ETA.EditValue != null)
                {
                    executeRoom.AVERAGE_ETA = (long)spAVERAGE_ETA.Value;
                }
                else
                {
                    executeRoom.AVERAGE_ETA = null;
                }
                if (chkIsKidney.CheckState == CheckState.Checked)
                {
                    executeRoom.IS_KIDNEY = 1;
                }
                else
                {
                    executeRoom.IS_KIDNEY = null;
                }
                if (spinKidneyCount.EditValue != null && chkIsKidney.Checked)
                {
                    executeRoom.KIDNEY_SHIFT_COUNT = (long)spinKidneyCount.Value;
                }
                else
                {
                    executeRoom.KIDNEY_SHIFT_COUNT = null;
                }
            }
            catch (Exception ex)
            {
                executeRoom = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return executeRoom;
        }

        private HIS_ROOM SetDataRoom()
        {
            HIS_ROOM room = new HIS_ROOM();
            try
            {
                room.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL;
                if (!string.IsNullOrWhiteSpace(txtHein_card_number.Text))
                {
                    room.BHYT_CODE = txtHein_card_number.Text;
                }
                if (lkRoomId.EditValue != null) room.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkRoomId.EditValue ?? "0").ToString());
                if (cboDefaultService.EditValue != null)
                {
                    room.DEFAULT_SERVICE_ID = Int32.Parse(cboDefaultService.EditValue.ToString());
                }
                else
                {
                    room.DEFAULT_SERVICE_ID = null;
                }
                if (cboArea.EditValue != null)
                    room.AREA_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboArea.EditValue ?? 0).ToString());
                else
                    room.AREA_ID = null;
                room.IS_PAUSE = (short)(chkIsPause.Checked ? 1 : 0);
                if (checkEdit1.Checked)
                {
                    room.IS_USE_KIOSK = 1;
                }
                else
                {
                    room.IS_USE_KIOSK = null;
                }
                room.ORDER_ISSUE_CODE = txtOrderIssueCode.Text;
                if (cbbRoomGroup.EditValue != null)
                    room.ROOM_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cbbRoomGroup.EditValue.ToString());
                else room.ROOM_GROUP_ID = null;

                if (cboCashRoom.EditValue != null && !string.IsNullOrEmpty(cboCashRoom.EditValue.ToString()))
                    room.DEFAULT_CASHIER_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboCashRoom.EditValue.ToString());
                else room.DEFAULT_CASHIER_ROOM_ID = null;

                if (chkRestrictTime.Checked)
                    room.IS_RESTRICT_TIME = 1;
                else
                    room.IS_RESTRICT_TIME = null;
                if (chkRestrictExecuteRoom.Checked)
                    room.IS_RESTRICT_EXECUTE_ROOM = 1;
                else
                    room.IS_RESTRICT_EXECUTE_ROOM = null;
                if (chkRestrictMedicineType.Checked)
                    room.IS_RESTRICT_MEDICINE_TYPE = 1;
                else
                    room.IS_RESTRICT_MEDICINE_TYPE = null;
                if (chkRestrictPatientType.Checked)
                    room.IS_RESTRICT_PATIENT_TYPE = 1;
                else
                    room.IS_RESTRICT_PATIENT_TYPE = null;
                if (c.Checked)
                    room.IS_RESTRICT_REQ_SERVICE = 1;
                else
                    room.IS_RESTRICT_REQ_SERVICE = null;
                if (chkIsAllowNoICD.Checked)
                    room.IS_ALLOW_NO_ICD = 1;
                else
                    room.IS_ALLOW_NO_ICD = null;
                if (chkIsBlockNumOrder.Checked)
                    room.IS_BLOCK_NUM_ORDER = 1;
                else
                    room.IS_BLOCK_NUM_ORDER = null;

                if (spHoldOrder.EditValue != null)
                {
                    room.HOLD_ORDER = (long)spHoldOrder.Value;
                }
                else
                    room.HOLD_ORDER = null;
                if (cboChuyenKhoa.EditValue != null)
                    room.SPECIALITY_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboChuyenKhoa.EditValue.ToString());
                else
                    room.SPECIALITY_ID = null;
                room.ADDRESS = txtAddress.Text.Trim();

                if (CboResponsible.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == CboResponsible.EditValue.ToString());
                    room.RESPONSIBLE_LOGINNAME = user != null ? user.LOGINNAME : "";
                    room.RESPONSIBLE_USERNAME = user != null ? user.USERNAME : "";
                }
                if (cboDefaultDrug.EditValue != null)
                {
                    GridCheckMarksSelection gridCheckMarkBusiness = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                    {
                        List<string> codes = new List<string>();
                        foreach (HIS_MEDI_STOCK rv in gridCheckMarkBusiness.Selection)
                        {
                            if (rv != null && !codes.Contains(rv.ID.ToString()))
                                codes.Add(rv.ID.ToString());
                        }

                        room.DEFAULT_DRUG_STORE_IDS = String.Join(",", codes);
                    }
                }
                else
                {
                    room.DEFAULT_DRUG_STORE_IDS = null;
                }
                if (cboWaitingScreen.EditValue != null)
                {
                    room.SCREEN_SAVER_MODULE_LINK = cboWaitingScreen.EditValue.ToString();
                }
                if (cboDepositBook.EditValue != null)
                {
                    room.DEPOSIT_ACCOUNT_BOOK_ID = Int32.Parse(cboDepositBook.EditValue.ToString());
                }
                else
                {
                    room.DEPOSIT_ACCOUNT_BOOK_ID = null;
                }
                if (cboAccountBook.EditValue != null)
                {
                    room.BILL_ACCOUNT_BOOK_ID = Int64.Parse(cboAccountBook.EditValue.ToString());
                }
                else
                {
                    room.BILL_ACCOUNT_BOOK_ID = null;
                }
                if (cboDefaultsCLS.EditValue != null)
                {
                    room.DEFAULT_INSTR_PATIENT_TYPE_ID = Int64.Parse(cboDefaultsCLS.EditValue.ToString());
                }
                else
                {
                    room.DEFAULT_INSTR_PATIENT_TYPE_ID = null;
                }

                room.QR_ACCOUNT_BOOK_ID = cboAccountQr.EditValue != null ? (long?)Int64.Parse(cboAccountQr.EditValue.ToString()) : null;
                room.QR_CONFIG_JSON = txtJsonQr.Text;
            }
            catch (Exception ex)
            {
                room = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return room;
        }

    }
}
