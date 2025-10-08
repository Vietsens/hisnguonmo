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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientExpMest.ADO;
using HIS.Desktop.Plugins.CallPatientExpMest.Config;
using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientExpMest
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen_ExpMest aFrmWaitingScreenQy = null;
        //frmWaitingScreen_ExpMest_SeparateScreen aFrmWaitingScreenQy_SeparateScreen = null;
        const string frmWaitingScreenStr = "frmWaitingScreen_ExpMest";
        const string frmWaitingScreenQyStr = "frmWaitingScreen_TP6";
        int positionHandleControl;
        internal MOS.EFMODEL.DataModels.HIS_EXP_MEST hisExpMest = null;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK room;
        long roomId = 0;
        internal ExpMestCallADO currentAdo;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        Inventec.Desktop.Common.Modules.Module currentModule;
        string moduleLink = "HIS.Desktop.Plugins.CallPatientExpMest";
        List<ExpMestTypeADO> expMestTypes = new List<ExpMestTypeADO>();
        List<ExpMestSttADO> expMestStts = new List<ExpMestSttADO>();


        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == frmWaitingScreenStr)
                {
                    this.Close();
                    return;
                }
            }
            InitializeComponent();
            this.roomId = module.RoomId;
            this.currentModule = module;
        }

        private void frmChooseRoomForWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                //ChooseRoomForWaitingScreenProcess.LoadDataToExamServiceReqSttGridControl(this);
                var expMestType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN ||
                o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK ||
                o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT ||
                o.ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)).ToList();
                foreach(var item in expMestType)
                {
                    ExpMestTypeADO expMestTypeADO = new ExpMestTypeADO();
                    expMestTypeADO.ID = item.ID;
                    expMestTypeADO.EXP_MEST_TYPE_CODE = item.EXP_MEST_TYPE_CODE;
                    expMestTypeADO.EXP_MEST_TYPE_NAME = item.EXP_MEST_TYPE_NAME;
                    //expMestTypeADO.chkExpMestStt = false;
                    expMestTypes.Add(expMestTypeADO);
                }
                gridControl1.DataSource = expMestTypes;


                var listStt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT>()
                                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    .ToList();
                foreach (var item in listStt)
                {
                    ExpMestSttADO expMestStt = new ExpMestSttADO();
                    expMestStt.ID = item.ID;
                    expMestStt.EXP_MEST_STT_CODE = item.EXP_MEST_STT_CODE;
                    expMestStt.EXP_MEST_STT_NAME = item.EXP_MEST_STT_NAME;
                    //expMestTypeADO.chkExpMestStt = false;
                    expMestStts.Add(expMestStt);
                }
                gridControlExecuteStatus.DataSource = expMestStts;

                // SetControlState
                InitControlState();

                room = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == roomId);
                ValidateRoom();
                ToogleExtendMonitor();
                SetDataTolblControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(currentModuleBase.ModuleLink);
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru)
                        {
                            chkSeparatePatientNoiTruNgoaiTru.Checked = item.VALUE == "1";
                        }
                    }
                }
                this.currentAdo = new ExpMestCallADO();
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {

                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "ExpMestCallADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                currentAdo = JsonConvert.DeserializeObject<ExpMestCallADO>(item.VALUE);
                            }
                        }
                    }
                }

                if (currentAdo != null)
                {
                    //
                    spnTextSize.EditValue = currentAdo.sizeText;
                    if (!string.IsNullOrEmpty(currentAdo.colorText))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.colorText);
                        cboColorTextSize.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }                    
                    if (!string.IsNullOrEmpty(currentAdo.bgColorText))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.bgColorText);
                        cboBgColorTextSize.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    //
                    spnTextTitle.EditValue = currentAdo.sizeTextTitle;
                    if (!string.IsNullOrEmpty(currentAdo.colorTextTitle))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.colorTextTitle);
                        cboColorTextTitle.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    if (!string.IsNullOrEmpty(currentAdo.bgColorTextTitle))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.bgColorTextTitle);
                        cboBgColorTextTitle.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    //
                    spnListText.EditValue = currentAdo.sizeTextList;
                    if (!string.IsNullOrEmpty(currentAdo.colorTextList))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.colorTextList);
                        cboColorTextList.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    if (!string.IsNullOrEmpty(currentAdo.bgColorTextList))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.bgColorTextList);
                        cboBgColorTextList.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    //
                    spnTextCalling.EditValue = currentAdo.sizeTextCalling;
                    if (!string.IsNullOrEmpty(currentAdo.colorTextCalling))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.colorTextCalling);
                        cboColorTextCalling.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    if (!string.IsNullOrEmpty(currentAdo.bgColorTextCalling))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.bgColorTextCalling);
                        cboBgColorTextCalling.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    //
                    spnContent.EditValue = currentAdo.sizeTextContent;
                    if (!string.IsNullOrEmpty(currentAdo.colorTextContent))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.colorTextContent);
                        cboColorContent.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    if (!string.IsNullOrEmpty(currentAdo.bgColorTextContent))
                    {
                        List<int> backgroundColorStall = new List<int>();
                        backgroundColorStall = GetColorValues(currentAdo.bgColorTextContent);
                        cboBgColorContent.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                    }
                    txtContent.Text = currentAdo.ContentTitle;
                    txtNote.Text = currentAdo.NoteTitle;

                    foreach (var item in expMestTypes)
                    {
                        if (currentAdo.expMestTypeADOs.Exists(o => o.ID == item.ID && o.chkExpMestStt))
                        {
                            item.chkExpMestStt = true;
                        }
                    }
                    expMestTypes = expMestTypes.OrderByDescending(o => o.chkExpMestStt).ToList();

                    foreach (var item in expMestStts)
                    {
                        if (currentAdo.expMestSttADOs.Exists(o => o.ID == item.ID && o.checkStt))
                        {
                            item.checkStt = true;
                        }
                    }
                    expMestStts = expMestStts.OrderByDescending(o => o.checkStt).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static List<int> GetColorValues(string code)
        {
            List<int> result = new List<int>();
            try
            {
                //string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                //string pattern = ",";
                //Regex myRegex = new Regex(pattern);
                //string[] Codes = myRegex.Split(value);

                string[] Codes = code.Split(',');

                //if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                if (!(Codes != null) || Codes.Length <= 0) throw new ArgumentNullException(code + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                foreach (var item in Codes) ///
                {
                    result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        void ToogleExtendMonitor()
        {
            try
            {
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == frmWaitingScreenStr)
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

        private void SetDataTolblControl()
        {

            try
            {
                if (room != null)
                {
                    lblRoom.Text = (room.MEDI_STOCK_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
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

        private void tgExtendMonitor_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (!tgExtendMonitor.IsOn) return;
                if (aFrmWaitingScreenQy != null) aFrmWaitingScreenQy.Close();

                List<ExpMestSttADO> expSTT = new List<ExpMestSttADO>();
                List<ExpMestTypeADO> expType = new List<ExpMestTypeADO>();
                if (gridControlExecuteStatus.DataSource != null)
                {
                    expSTT = (List<ExpMestSttADO>)gridControlExecuteStatus.DataSource;
                }
                if (gridControl1.DataSource != null)
                {
                    expType = (List<ExpMestTypeADO>)gridControl1.DataSource;
                }
                if (expSTT.Count() > 0 || expType.Count() >0)
                {
                    var expSTTcheck = expSTT.Where(o => o.checkStt);
                    var expTypeCheck = expType.Where(o => o.chkExpMestStt);
                    this.positionHandleControl = -1;
                    if (!dxValidationProviderControl.Validate())
                    {
                        tgExtendMonitor.IsOn = false;
                        return;
                    }
                    //if (expSTTcheck == null || expSTTcheck.Count() == 0)
                    //{
                    //    if (tgExtendMonitor.IsOn)
                    //    {
                    //        tgExtendMonitor.IsOn = false;
                    //        return;
                    //    }
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn ít nhất 1 phòng khám", "Thông báo");
                    //    return;
                    //}

                    SavePin(expSTTcheck.ToList(), expTypeCheck.ToList());
                    frmWaitingScreen_ExpMest frm = new frmWaitingScreen_ExpMest(currentAdo, room);
                    ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(frm);
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SavePin(List<ExpMestSttADO> lstExpMestStt, List<ExpMestTypeADO> lstExpMestType)
        {
            try
            {

                ExpMestCallADO ado = new ExpMestCallADO();
                ado.expMestSttADOs = lstExpMestStt;
                ado.expMestTypeADOs = lstExpMestType;
                if (spnTextSize.Value > 0)
                    ado.sizeText = (long)spnTextSize.Value;
                ado.colorText = cboColorTextSize.Text;
                ado.bgColorText = cboBgColorTextSize.Text;

                if (spnTextTitle.Value > 0)
                    ado.sizeTextTitle = (long)spnTextTitle.Value;
                ado.colorTextTitle = cboColorTextTitle.Text;
                ado.bgColorTextTitle = cboBgColorTextTitle.Text;

                if (spnListText.Value > 0)
                    ado.sizeTextList = (long)spnListText.Value;
                ado.colorTextList = cboColorTextList.Text;
                ado.bgColorTextList = cboBgColorTextList.Text;

                if (spnContent.Value > 0)
                    ado.sizeTextContent = (long)spnContent.Value;
                ado.colorTextContent = cboColorContent.Text;
                ado.bgColorTextContent = cboBgColorContent.Text;

                if (spnTextCalling.Value > 0)
                    ado.sizeTextCalling = (long)spnTextCalling.Value;
                ado.colorTextCalling = cboColorTextCalling.Text;
                ado.bgColorTextCalling = cboBgColorTextCalling.Text;


                ado.ContentTitle = txtContent.Text.Trim();
                ado.NoteTitle = txtNote.Text.Trim();
                this.currentAdo = ado;
                LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ado__send__:", ado));
                string textJson = JsonConvert.SerializeObject(ado);

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "ExpMestCallADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = textJson;
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "ExpMestCallADO";
                    csAddOrUpdateValue.VALUE = textJson;
                    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

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
                    ExpMestSttADO dataRow = (ExpMestSttADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderControl_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSeparatePatientNoiTruNgoaiTru_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru && o.MODULE_LINK == ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSeparatePatientNoiTruNgoaiTru.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru;
                    csAddOrUpdate.VALUE = (chkSeparatePatientNoiTruNgoaiTru.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
