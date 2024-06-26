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
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.ADO;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientVer7
{
    internal class ChooseRoomForWaitingScreenProcess
    {
        const string frmWaitingScreen = "frmWaitingScreen";
        const string frmWaitingScreenQy = "frmWaitingScreen_QY";

        internal static void LoadDataToComboRoom(DevExpress.XtraEditors.GridLookUpEdit cboRoom)
        {
            try
            {
                List<long> roomTypeIds = new List<long>();
                roomTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL);
                roomTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG);

                cboRoom.Properties.DataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => roomTypeIds.Contains(o.ROOM_TYPE_ID) && o.ROOM_TYPE_CODE == "XL").ToList();
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";

                cboRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.ForceInitialize();
                cboRoom.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboRoom.Properties.View.Columns.AddField("ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboRoom.Properties.View.Columns.AddField("ROOM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void ShowFormInExtendMonitor(frmWaitingScreen_V7 control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    Screen secondScreen = sc.FirstOrDefault(o => o != Screen.PrimaryScreen);
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = secondScreen.Bounds.Width;
                    control.Top = secondScreen.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = secondScreen.Bounds.Location;
                    Point p = new Point(secondScreen.Bounds.Location.X, secondScreen.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void TurnOffExtendMonitor(frmWaitingScreen_V7 control)
        {
            try
            {
                if (control != null)
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreenQy)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public static void LoadDataToVaccinationExamSttGridControl(frmChooseRoomForWaitingScreen control)
        {
            try
            {
                //CommonParam param = new CommonParam();
               // MOS.Filter.HisVaccinationSttFilter filter = new MOS.Filter.HisVaccinationSttFilter();
               // var HisVaccinationExamStts = new BackendAdapter(param).Get<List<HIS_VACCINATION_STT>>("api/HisVaccinationStt/get", ApiConsumers.MosConsumer, filter, param);
                //List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> HisServiceReqStts = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
               
                List<VaccinationSttSDO> vaccinationExamSttSdos = new List<VaccinationSttSDO>();
                VaccinationSttSDO sdo=new VaccinationSttSDO();
                sdo.NAME_STATUS="Chưa khám";
                sdo.VACCINATION_STT_CODE = "01";
                sdo.checkStt = true;
                vaccinationExamSttSdos.Add(sdo);
                VaccinationSttSDO sdo1 = new VaccinationSttSDO();
                sdo1.NAME_STATUS = "Đã khám";
                sdo1.VACCINATION_STT_CODE = "02";
                sdo1.checkStt = false;
                vaccinationExamSttSdos.Add(sdo1);
                //foreach (var item in HisVaccinationExamStts)
                //{
                //    VaccinationSttSDO vaccinationExamSttSdo = new VaccinationSttSDO();
                //    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_VACCINATION_STT, VaccinationSttSDO>();
                //    vaccinationExamSttSdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_VACCINATION_STT, VaccinationSttSDO>(item);
                //    if (item.ID == IMSys.DbConfig.HIS_RS.HIS_VACCINATION_STT.ID__NEW)
                //        vaccinationExamSttSdo.checkStt = true;
                //    else
                //        vaccinationExamSttSdo.checkStt = false;
                //    vaccinationExamSttSdos.Add(vaccinationExamSttSdo);
                //}
                control.gridControl1.DataSource = vaccinationExamSttSdos;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }


    }
}
