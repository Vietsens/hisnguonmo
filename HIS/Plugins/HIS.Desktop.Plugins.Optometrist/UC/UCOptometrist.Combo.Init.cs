using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private List<ACS_USER> listDataUser = new List<ACS_USER>();

        private void InitExecuteName()
        {
            try
            {
                List<ACS_USER> datas = BackendDataWorker.Get<ACS_USER>();
                List<HIS_EMPLOYEE> employeeList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                listDataUser = new List<ACS_USER>();

                foreach (var item in employeeList)
                {
                    if (String.IsNullOrWhiteSpace(item.LOGINNAME)) continue;

                    ACS_USER user = new ACS_USER();
                    user.LOGINNAME = item.LOGINNAME;
                    var check = datas.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {
                        user.USERNAME = check.USERNAME;
                        user.MOBILE = check.MOBILE;
                        user.PASSWORD = check.PASSWORD;
                    }

                    listDataUser.Add(user);
                }

                listDataUser = listDataUser.OrderBy(o => o.USERNAME).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 750);
                ControlEditorLoader.Load(VISION_TEST_USERNAME, listDataUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitExcuteRoom()
        {
            try
            {
                //var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                //if (excuteRoomCombo != null && executeRoomViews != null && serviceRoomViews != null && serviceRoomViews.Count > 0)
                {
                    //var arrExcuteRoomCode = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).Select(o => o.ROOM_ID).ToList();
                    //if (arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0)
                    {
                        //List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = executeRoomViews.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList();
                        this.InitComboExecuteRoom(VISION_TEST_ROOM_NAME, executeRoomViews);
                    }
                    //else
                    //{
                    //    this.InitComboExecuteRoom(excuteRoomCombo, null);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboExecuteRoom(DevExpress.XtraEditors.GridLookUpEdit excuteRoomCombo, List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(excuteRoomCombo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}