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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Patient
{
    //internal class UCListPatientProcess
    //{
    //    internal static int rowCount = 0;
    //    private static WaitDialogForm waitLoad = null;
    //    internal static void ClickButtonSearch(UCListPatient control)
    //    {
    //        try
    //        {
    //            waitLoad = new WaitDialogForm(MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageUtil.GetMessage(Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm));
    //            CommonParam param = new CommonParam(0, Convert.ToInt32(control.txtPageSize.Text));
    //            MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
    //            EXE.MANAGER.HisPatient.HisPatientLogic mngUIConcrete = new EXE.MANAGER.HisPatient.HisPatientLogic(param);
    //            var mngUIConcrete = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT>(patientFilter);
    //            try
    //            {
    //                filter.KEY_WORD = control.txtKeyWord.Text.Trim();
    //                if (control.dtFromTime != null && control.dtFromTime.DateTime != DateTime.MinValue)
    //                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
    //                if (control.dtToTime != null && control.dtToTime.DateTime != DateTime.MinValue)
    //                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtToTime.EditValue).ToString("yyyyMMdd") + "232359");
    //                if (control.dtBirthdayFrom != null && control.dtBirthdayFrom.DateTime != DateTime.MinValue)
    //                    filter.DOB_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtBirthdayFrom.EditValue).ToString("yyyyMMdd") + "000000");
    //                if (control.dtBirthdayTo != null && control.dtBirthdayTo.DateTime != DateTime.MinValue)
    //                    filter.DOB_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtBirthdayTo.EditValue).ToString("yyyyMMdd") + "232359");
    //                try
    //                {
    //                    if (control.navBarFilter.Controls.Count > 0)
    //                    {
    //                        for (int i = 0; i < control.navBarFilter.Controls.Count; i++)
    //                        {
    //                            if (control.navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainer)
    //                            {
    //                                continue;
    //                            }
    //                            if (control.navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
    //                            {
    //                                var groupWrapper = control.navBarFilter.Controls[i] as DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper;
    //                                foreach (DevExpress.XtraNavBar.NavBarGroupControlContainer group in groupWrapper.Controls)
    //                                {
    //                                    foreach (var itemCheckEdit in group.Controls)
    //                                    {
    //                                        if (itemCheckEdit is CheckEdit)
    //                                        {
    //                                            var checkEdit = itemCheckEdit as CheckEdit;
    //                                            if (checkEdit.Checked)
    //                                            {
    //                                                if (filter.GENDER_IDs == null)
    //                                                    filter.GENDER_IDs = new List<long>();
    //                                                filter.GENDER_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    Inventec.Common.Logging.LogSystem.Warn(ex);
    //                }

    //                var apiResult = mngUIConcrete.GetViewForPaging(filter);
    //                if (apiResult != null)
    //                {
    //                    control.gridControlPatientList.DataSource = null;
    //                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>)apiResult.Data;
    //                    if (data != null)
    //                    {
    //                        control.gridControlPatientList.DataSource = data;
    //                        var recordData = data.Count;
    //                        rowCount = apiResult.Param.Count ?? 0;
    //                        control.pagingGrid.Innitial(control.lblDisplayPageNo, control.txtPageSize, control.txtCurrentPage, control.lblTotalPage, control.btnLastPage, control.btnPreviousPage, control.btnFirstPage, control.btnNextPage, rowCount, recordData);
    //                    }
    //                }
    //                waitLoad.Dispose();

    //                #region Process has exception
    //                SessionManager.ProcessTokenLost(param);
    //                #endregion
    //            }
    //            catch (Exception ex)
    //            {
    //                Inventec.Common.Logging.LogSystem.Warn(ex);
    //                waitLoad.Dispose();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            waitLoad.Dispose();
    //            Inventec.Common.Logging.LogSystem.Warn(ex);
    //        }
    //    }
    //    internal static void UpdateDataForPaging(UCListPatient control)
    //    {
    //        try
    //        {
    //            waitLoad = new WaitDialogForm(MessageUtil.GetMessage(EXE.LibraryFEMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageUtil.GetMessage(EXE.LibraryFEMessage.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm));
    //            CommonParam param = new CommonParam(control.pagingGrid.RecNo, Convert.ToInt32(control.txtPageSize.Text));
    //            MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
    //            EXE.MANAGER.HisPatient.HisPatientLogic mngUIConcrete = new EXE.MANAGER.HisPatient.HisPatientLogic(param);
    //            try
    //            {
    //                if (control.dtFromTime != null && control.dtFromTime.DateTime != DateTime.MinValue)
    //                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
    //                if (control.dtToTime != null && control.dtToTime.DateTime != DateTime.MinValue)
    //                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtToTime.EditValue).ToString("yyyyMMdd") + "232359");
    //                if (control.dtBirthdayFrom != null && control.dtBirthdayFrom.DateTime != DateTime.MinValue)
    //                    filter.DOB_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtBirthdayFrom.EditValue).ToString("yyyyMMdd") + "000000");
    //                if (control.dtBirthdayTo != null && control.dtBirthdayTo.DateTime != DateTime.MinValue)
    //                    filter.DOB_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtBirthdayTo.EditValue).ToString("yyyyMMdd") + "232359");
    //                try
    //                {
    //                    if (control.navBarFilter.Controls.Count > 0)
    //                    {
    //                        for (int i = 0; i < control.navBarFilter.Controls.Count; i++)
    //                        {
    //                            if (control.navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainer)
    //                            {
    //                                continue;
    //                            }
    //                            if (control.navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
    //                            {
    //                                var groupWrapper = control.navBarFilter.Controls[i] as DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper;
    //                                foreach (DevExpress.XtraNavBar.NavBarGroupControlContainer group in groupWrapper.Controls)
    //                                {
    //                                    foreach (var itemCheckEdit in group.Controls)
    //                                    {
    //                                        if (itemCheckEdit is CheckEdit)
    //                                        {
    //                                            var checkEdit = itemCheckEdit as CheckEdit;
    //                                            if (checkEdit.Checked)
    //                                            {
    //                                                if (filter.GENDER_IDs == null)
    //                                                    filter.GENDER_IDs = new List<long>();
    //                                                filter.GENDER_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    Inventec.Common.Logging.LogSystem.Warn(ex);
    //                }
    //                filter.KEY_WORD = control.txtKeyWord.Text.Trim();

    //                var apiResult = mngUIConcrete.GetViewForPaging(filter);
    //                if (apiResult != null)
    //                {
    //                    control.gridControlPatientList.DataSource = null;
    //                    control.gridControlPatientList.DataSource = (List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>)apiResult.Data;
    //                    rowCount = apiResult.Param.Count ?? 0;
    //                }
    //                waitLoad.Dispose();

    //                #region Process has exception
    //                SessionManager.ProcessTokenLost(param);
    //                #endregion
    //            }
    //            catch (Exception ex)
    //            {
    //                waitLoad.Dispose();
    //                Inventec.Common.Logging.LogSystem.Warn(ex);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            waitLoad.Dispose();
    //            Inventec.Common.Logging.LogSystem.Warn(ex);
    //        }
    //    }

    //}
    class UCListPatientProcess
    {
        internal static void loadataToGridControl(UCListPatient control)
        {
            //try
            //{
            //    try
            //    {
            //        //MSS.MANAGER.Anticipate.HisAnticipateLogic anticipateLogic = new MANAGER.Anticipate.HisAnticipateLogic();
            //        MOS.Filter.HisPatientViewFilter patientFilter = new MOS.Filter.HisPatientViewFilter();
            //        //var anticipates = anticipateLogic.Get(anticipateFilter);
            //        var patient = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT>(patientFilter);
            //        control.gridControlPatientList.DataSource = null;
            //        control.gridControlPatientList.DataSource = patient;
            //    }
            //    catch (Exception ex)
            //    {
            //        Inventec.Common.Logging.LogSystem.Warn(ex);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }
        internal static void SearchPatientControl(UCListPatient control)
        {
            //try
            //{
            //    WaitDialogForm waitLoad = new WaitDialogForm(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoMoTaChoWaitDialogForm), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm));
            //    CommonParam param = new CommonParam();
            //    MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
            //    //MSS.MANAGER.Anticipate.HisAnticipateLogic anticipateLogic = new MANAGER.Anticipate.HisAnticipateLogic(param);

            //    filter.KEY_WORD = control.txtKeyWord.Text;
            //    if (control.dtCreateTimeFrom.EditValue != null && control.dtCreateTimeFrom.DateTime != DateTime.MinValue)
            //    {
            //        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
            //    }
            //    if (control.dtCreateTimeTo.EditValue != null && control.dtCreateTimeTo.DateTime != DateTime.MinValue)
            //    {
            //        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(control.dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
            //    }
            //    //var anticipates = anticipateLogic.Get(filter);
            //    var anticipates = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT>(filter);
            //    control.gridControlPatientList.DataSource = null;
            //    control.gridControlPatientList.DataSource = anticipates;
            //    waitLoad.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }
    }
}
