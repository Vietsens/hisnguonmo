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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
namespace HIS.Desktop.Plugins.CallPatientVer7
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen_V7 aFrmWaitingScreenQy = null;
        const string frmWaitingScreenStr = "frmWaitingScreen_V7";
        const string frmWaitingScreenQyStr = "frmWaitingScreen_TP6";
        int positionHandleControl;
        internal MOS.EFMODEL.DataModels.HIS_VACCINATION_EXAM HisVaccinationExam = null;
        MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        Inventec.Desktop.Common.Modules.Module _module;
        long roomId = 0;

        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == frmWaitingScreenQyStr || frm.Name == frmWaitingScreenStr)
                {
                    this.Close();
                    return;
                }
            }
            this._module = module;
            InitializeComponent();
            this.roomId = module.RoomId;
        }

        private void frmChooseRoomForWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                ChooseRoomForWaitingScreenProcess.LoadDataToVaccinationExamSttGridControl(this);
                Inventec.Common.Logging.LogSystem.Debug(roomId.ToString());
                room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                ValidateRoom();
                ToogleExtendMonitor();
                SetDataTolblControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataTolblControl()
        {
            try
            {
                if (room != null)
                {
                    lblRoom.Text = (room.ROOM_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
                }
                else
                {
                    lblRoom.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ToogleExtendMonitor()
        {
            try
            {
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == frmWaitingScreenStr || f.Name == frmWaitingScreenQyStr)
                        {
                            tgExtendMonitor.IsOn = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateRoom()
        {
            try
            {
                RoomWaitingScreenValidation roomRule = new RoomWaitingScreenValidation();
                roomRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                roomRule.ErrorType = ErrorType.Warning;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tgExtendMonitor_Toggled(object sender, EventArgs e)
        {
            try
            {
                List<VaccinationSttSDO> vaccinationSttSdos = new List<VaccinationSttSDO>();
                if (gridControl1.DataSource != null)
                {
                    vaccinationSttSdos = (List<VaccinationSttSDO>)gridControl1.DataSource;
                }
                List<VaccinationSttSDO> vaccinationExamStts = new List<VaccinationSttSDO>();
                foreach (var item in vaccinationSttSdos)
                {
                    if (item.checkStt)
                    {
                        //MOS.EFMODEL.DataModels.HIS_VACCINATION_STT vaccinationExamStt = new MOS.EFMODEL.DataModels.HIS_VACCINATION_STT();
                        //AutoMapper.Mapper.CreateMap<VaccinationSttSDO, MOS.EFMODEL.DataModels.HIS_VACCINATION_STT>();
                        //vaccinationExamStt = AutoMapper.Mapper.Map<VaccinationSttSDO, MOS.EFMODEL.DataModels.HIS_VACCINATION_STT>(item);
                        VaccinationSttSDO sdo = new VaccinationSttSDO();
                        sdo.NAME_STATUS = item.NAME_STATUS;
                        sdo.VACCINATION_STT_CODE = item.VACCINATION_STT_CODE;
                        sdo.checkStt = item.checkStt; ;
                        vaccinationExamStts.Add(sdo);
                    }
                }
                this.positionHandleControl = -1;
                if (!dxValidationProviderControl.Validate())
                    return;
                aFrmWaitingScreenQy = new frmWaitingScreen_V7(HisVaccinationExam, vaccinationExamStts);
                if (this.room != null)
                {
                    aFrmWaitingScreenQy.room = this.room;
                }
                if (aFrmWaitingScreenQy != null && tgExtendMonitor.IsOn)
                {
                    HIS.Desktop.Plugins.CallPatientVer7.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmWaitingScreenQy);
                    this.Close();
                }
                else
                {
                    HIS.Desktop.Plugins.CallPatientVer7.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmWaitingScreenQy);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExecuteStatus_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    VaccinationSttSDO dataRow = (VaccinationSttSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckEditStt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
        }


    }
}
