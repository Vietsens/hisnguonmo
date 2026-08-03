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
using HIS.Desktop.Plugins.HisExecuteRoom.RoomConfigOption;

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
                executeRoom.IS_EMERGENCY = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsEmergency);
                executeRoom.IS_PAUSE_ENCLITIC = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsPauseEnclitic);
                executeRoom.IS_SPECIALITY = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsSpeciality);
                executeRoom.IS_SURGERY = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsSurgery);
                executeRoom.MUST_BE_APPROVED_SURGERY = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.MustBeApprovedSurgery);
                executeRoom.IS_EXAM = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsExam);
                executeRoom.ALLOW_NOT_CHOOSE_SERVICE = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.AllowNotChooseService);
                executeRoom.IS_AUTO_EXPEND_ADD_EXAM = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsAutoExpendAddExam);
                executeRoom.IS_VACCINE = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsVaccine);
                executeRoom.IS_VITAMIN_A = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsVitaminA);
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
                Inventec.Common.Logging.LogSystem.Error(
                    "SetDataExecuteRoom thất bại - khong dung duoc DTO HIS_EXECUTE_ROOM."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => executeRoom), executeRoom),
                    ex);
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
                room.DEFAULT_SERVICE_ID = GetEditValueAsLong(cboDefaultService);
                if (cboArea.EditValue != null)
                    room.AREA_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboArea.EditValue ?? 0).ToString());
                else
                    room.AREA_ID = null;
                room.ORDER_ISSUE_CODE = txtOrderIssueCode.Text;
                if (cbbRoomGroup.EditValue != null)
                    room.ROOM_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cbbRoomGroup.EditValue.ToString());
                else room.ROOM_GROUP_ID = null;

                if (cboCashRoom.EditValue != null && !string.IsNullOrEmpty(cboCashRoom.EditValue.ToString()))
                    room.DEFAULT_CASHIER_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboCashRoom.EditValue.ToString());
                else room.DEFAULT_CASHIER_ROOM_ID = null;
                room.IS_PAUSE = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsPause);
                room.IS_USE_KIOSK = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsUseKiosk);
                room.IS_RESTRICT_TIME = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsRestrictTime);
                room.IS_RESTRICT_EXECUTE_ROOM = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsRestrictExecuteRoom);
                room.IS_RESTRICT_MEDICINE_TYPE = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsRestrictMedicineType);
                room.IS_RESTRICT_PATIENT_TYPE = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsRestrictPatientType);
                room.IS_RESTRICT_REQ_SERVICE = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsRestrictReqService);
                room.IS_ALLOW_NO_ICD = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsAllowNoICD);
                room.IS_BLOCK_NUM_ORDER = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsBlockNumOrder);
                room.IS_SPLIT_BY_PRIORITY = SelectedOptions.Any(RoomConfigOption.RoomConfigOption.Option.IsSplitByPriority);
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
                room.DEPOSIT_ACCOUNT_BOOK_ID = GetEditValueAsLong(cboDepositBook);
                room.BILL_ACCOUNT_BOOK_ID = GetEditValueAsLong(cboAccountBook);
                room.DEFAULT_INSTR_PATIENT_TYPE_ID = GetEditValueAsLong(cboDefaultsCLS);
                //qtcode
                room.PAYER_BANK_ID = GetEditValueAsLong(cboPayerBank);
                room.PAYER_ACCOUNT = txtPayerAccount.Text.Trim();
                room.QR_ACCOUNT_BOOK_ID = GetEditValueAsLong(cboAccountQr);
                room.QR_CONFIG_JSON = txtJsonQr.Text;

                room.DEFAULT_EXPEND_MEDI_STOCK_ID = GetEditValueAsLong(cboExpendMediStock);
            }
            catch (Exception ex)
            {
                room = null;
                Inventec.Common.Logging.LogSystem.Error(
                    "SetDataRoom thất bại - khong dung duoc DTO HIS_ROOM."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => room), room),
                    ex);
            }
            return room;
        }

        /// <summary>
        /// Convert EditValue of a lookup editor to nullable long.
        /// Returns null when EditValue is null, an empty string, or not a valid number.
        /// DevExpress editors may keep an empty string as EditValue (designer default,
        /// or free text typed by the user), so a plain null check is not enough.
        /// </summary>
        private long? GetEditValueAsLong(DevExpress.XtraEditors.BaseEdit editor)
        {
            try
            {
                if (editor == null || editor.EditValue == null) return null;

                string editValue = editor.EditValue.ToString();
                if (string.IsNullOrWhiteSpace(editValue)) return null;

                long result;
                if (!Int64.TryParse(editValue.Trim(), out result))
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "EditValue khong phai so - bo qua gia tri. Control=" + editor.Name
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => editValue), editValue));
                    return null;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

    }
}