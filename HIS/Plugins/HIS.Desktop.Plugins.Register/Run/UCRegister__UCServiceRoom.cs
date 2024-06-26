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
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.RoomExamService;
using HIS.UC.RoomExamService.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        void CreateExamServiceRoomInfoPanel()
        {
            try
            {
                this.roomExamServiceProcessor = new RoomExamServiceProcessor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitExamServiceRoom(bool isInit, V_HIS_SERE_SERV sereServExam)
        {
            try
            {
                if (this.roomExamServiceProcessor != null)
                {
                    RoomExamServiceInitADO roomExamServiceData = new RoomExamServiceInitADO();
                    roomExamServiceData.RemoveUC = this.DeleteServiceRoomInfo;
                    roomExamServiceData.FocusOutUC = this.FoucusMoveOutServiceRoomInfo;
                    roomExamServiceData.TemplateDesign = TemplateDesign.T01;
                    List<long> _roomIdByPatientTypeRooms = new List<long>();
                    if (this.cboPatientType.EditValue != null)
                    {
                        roomExamServiceData.CurrentPatientTypeAlter = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
                        roomExamServiceData.CurrentPatientTypeAlter.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString());

                        MOS.Filter.HisPatientTypeRoomFilter _patienttypeRoomFIlter = new MOS.Filter.HisPatientTypeRoomFilter();
                        _patienttypeRoomFIlter.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString());
                        _patienttypeRoomFIlter.IS_ACTIVE = (short)1;
                        var dataTypeRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/Get", ApiConsumers.MosConsumer, _patienttypeRoomFIlter, null);
                        if (dataTypeRooms != null && dataTypeRooms.Count > 0)
                        {
                            _roomIdByPatientTypeRooms = dataTypeRooms.Select(p => p.ROOM_ID).Distinct().ToList();
                        }
                    }
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString()) == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                    {
                        roomExamServiceData.RegisterPatientWithRightRouteBHYT = ProcessRegisterPatientWithRightRouteBHYT;
                        roomExamServiceData.ChangeRoomNotEmergency = ChangeRoomNotEmergency;
                    }
                    roomExamServiceData.CurrentCulture = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataExecutes = new List<V_HIS_EXECUTE_ROOM>();
                    if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>())
                    {
                        dataExecutes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                        dataExecutes = dataExecutes.Where(o => o.IS_ACTIVE == (short)1 && o.IS_EXAM == (short)1).ToList();
                    }
                    else
                    {
                        MOS.Filter.HisExecuteRoomFilter executeRoomFIlter = new MOS.Filter.HisExecuteRoomFilter();
                        executeRoomFIlter.IS_EXAM = true;
                        executeRoomFIlter.IS_ACTIVE = (short)1;
                        dataExecutes = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/GetView", ApiConsumers.MosConsumer, executeRoomFIlter, null);
                    }
                    if (dataExecutes != null && dataExecutes.Count > 0)
                    {
                        var roomIdActivesV2 = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => (p.IS_RESTRICT_PATIENT_TYPE == null
                       || _roomIdByPatientTypeRooms.Contains(p.ID))).Select(p => p.ID).ToList();
                        dataExecutes = dataExecutes.Where(p => p.BRANCH_ID == WorkPlace.GetBranchId()
                            && p.IS_PAUSE_ENCLITIC != 1
                            && roomIdActivesV2.Contains(p.ROOM_ID)).ToList();
                    }
                    //roomExamServiceData.HisServicePatys = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                    roomExamServiceData.HisExecuteRooms = dataExecutes;
                    roomExamServiceData.HisServiceRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o =>
                        o.BRANCH_ID == WorkPlace.GetBranchId()
                        && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        && o.IS_ACTIVE == 1).ToList();
                    //Lấy danh sách các service & room đang hoạt động (IS_ACTIVE = 1), sau đó lọc các HisServiceRooms
                    var roomIdActives = roomExamServiceData.HisExecuteRooms.Select(o => o.ROOM_ID).ToList();
                    var serviceIdActives = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        && o.IS_ACTIVE == 1).Select(o => o.ID).ToList();
                    roomExamServiceData.HisServiceRooms = roomExamServiceData.HisServiceRooms.Where(o => roomIdActives != null
                        && roomIdActives.Contains(o.ROOM_ID)
                        && serviceIdActives != null
                        && serviceIdActives.Contains(o.SERVICE_ID)).ToList();
                    roomExamServiceData.UcName = roomExamServiceNumber + "";
                    roomExamServiceData.IsInit = isInit;
                    if (sereServExam != null)
                        roomExamServiceData.SereServExam = sereServExam;
                    roomExamServiceData.TextSize = new System.Drawing.Size(90, 20);
                    roomExamServiceData.CurrentPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1 && o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? 0).ToString())).ToList();
                    this.ucRoomExamService = this.roomExamServiceProcessor.Run(roomExamServiceData) as UserControl;
                    if (this.ucRoomExamService != null)
                    {
                        this.ucRoomExamService.Name = roomExamServiceNumber + "";
                        this.ucRoomExamService.Dock = DockStyle.Top;
                        this.ucRoomExamService.AutoSize = false;
                        //if (roomExamServiceNumber == 0)
                        //    this.pnlServiceRoomInfomation.Controls.Clear();

                        this.pnlServiceRoomInfomation.Controls.Add(this.ucRoomExamService);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessRegisterPatientWithRightRouteBHYT()
        {
            try
            {
                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    this.mainHeinProcessor.AutoCheckRightRoute(this.ucHeinBHYT, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ChangeRoomNotEmergency()
        {
            try
            {
                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    this.mainHeinProcessor.ChangeRoomNotEmergency(this.ucHeinBHYT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DeleteServiceRoomInfo(object LayoutControlItemName)
        {
            bool result = false;
            try
            {
                if (LayoutControlItemName != null)
                {
                    foreach (Control item in this.pnlServiceRoomInfomation.Controls)
                    {
                        if (item != null && (item is UserControl || item is XtraUserControl) && item.Name == (string)LayoutControlItemName)
                        {
                            this.pnlServiceRoomInfomation.Controls.Remove(item);
                            item.Dispose();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        void FoucusMoveOutServiceRoomInfo(object uc)
        {
            try
            {
                this.txtEthnicCode.Focus();
                this.txtEthnicCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FocusInServiceRoomInfo()
        {
            try
            {
                if (this.roomExamServiceProcessor != null)
                {
                    foreach (Control item in this.pnlServiceRoomInfomation.Controls)
                    {
                        if (item != null && (item is UserControl || item is XtraUserControl))
                        {
                            this.roomExamServiceProcessor.FocusAndShow(item);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FocusInService()
        {
            try
            {
                if (this.roomExamServiceProcessor != null)
                {
                    foreach (Control item in this.pnlServiceRoomInfomation.Controls)
                    {
                        if (item != null && (item is UserControl || item is XtraUserControl))
                        {
                            this.roomExamServiceProcessor.FocusService(item);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
