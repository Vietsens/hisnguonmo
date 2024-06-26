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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ServiceExecute.EkipTemp;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmClsInfo1 : Form
    {
        ADO.ServiceADO currentServiceADO;
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT sereServPTTT { get; set; }
        private List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
        Action<List<HisEkipUserADO>, V_HIS_SERE_SERV_PTTT> actSaveClick;
        internal List<HIS_EKIP_TEMP> ekipTemps { get; set; }
        public int positionHandle = -1;

        public frmClsInfo1(ADO.ServiceADO serviceADO, List<HisEkipUserADO> ekipUsers, Action<List<HisEkipUserADO>, V_HIS_SERE_SERV_PTTT> actsaveclick, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT sereservPTTT)
        {
            InitializeComponent();
            this.currentServiceADO = serviceADO;
            this.ekipUserAdos = ekipUsers;
            this.sereServPTTT = sereservPTTT;
            this.actSaveClick = actsaveclick;
        }

        private void frmClsInfo_Load(object sender, EventArgs e)
        {
            if (this.currentServiceADO != null)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sereServPTTT), this.sereServPTTT) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentServiceADO), this.currentServiceADO));
                this.GetSereServPtttBySereServId();
                this.ComboAcsUser(repositoryItemCboName);//Họ và tên
                this.ComboExecuteRole(repositoryItemCboRole);//Vai trò
                this.ComboPTTTGroup();
                this.ComboLoaiPT();
                this.ComboEmotionlessMothod();
                this.ComboEkipTemp(cboEkipTemp);
                this.ProcessLoadEkip();
                this.SetDefaultCboPTTTGroupOnly();
                this.SetDataControlBySereServPttt();
            }
        }

        private void SetDataControlBySereServPttt()
        {
            try
            {
                if (this.sereServPTTT != null)
                {

                    var pttgr = this.sereServPTTT.PTTT_GROUP_ID > 0 ? HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.ID == this.sereServPTTT.PTTT_GROUP_ID).FirstOrDefault() : null;

                    var pttgrem = this.sereServPTTT.EMOTIONLESS_METHOD_ID > 0 ? HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o => o.ID == this.sereServPTTT.EMOTIONLESS_METHOD_ID).FirstOrDefault() : null;

                    txtEmotionlessMethod.Text = pttgrem != null ? pttgrem.EMOTIONLESS_METHOD_CODE : "";
                    cbbEmotionlessMethod.EditValue = pttgrem != null ? (long?)pttgrem.ID : null;
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = pttgrem != null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pttgrem), pttgrem) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pttgrem), pttgrem));
                    if (pttgr != null)
                    {
                        txtPtttGroupCode.Text = pttgr.PTTT_GROUP_CODE;
                        cbbPtttGroup.EditValue = pttgr.ID;
                        cbbPtttGroup.Properties.Buttons[1].Visible = true;
                    }
                    if (this.sereServPTTT.PTTT_PRIORITY_ID > 0)
                    {
                        var dataPtttPrio = BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_PRIORITY_ID);
                        if (dataPtttPrio != null)
                        {
                            txtLoaiPT.Text = dataPtttPrio.PTTT_PRIORITY_CODE;
                            cboLoaiPT.EditValue = dataPtttPrio.ID;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                        }
                        else
                        {
                            txtLoaiPT.Text = "";
                            cboLoaiPT.EditValue = null;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataPtttPrio), dataPtttPrio));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadPtttGroupCode(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbPtttGroup.Focus();
                    cbbPtttGroup.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.PTTT_GROUP_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbPtttGroup.EditValue = data[0].ID;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_GROUP_CODE == searchCode);
                            if (search != null)
                            {
                                cbbPtttGroup.EditValue = search.ID;
                                cbbPtttGroup.Properties.Buttons[1].Visible = true;
                                txtLoaiPT.Focus();
                                txtLoaiPT.SelectAll();
                            }
                            else
                            {
                                cbbPtttGroup.EditValue = null;
                                cbbPtttGroup.Focus();
                                cbbPtttGroup.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbPtttGroup.EditValue = null;
                        cbbPtttGroup.Focus();
                        cbbPtttGroup.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataEkipTemp()
        {
            try
            {
                ComboEkipTemp(cboEkipTemp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtPtttGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadPtttGroupCode(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbPtttGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbPtttGroup.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_GROUP data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtPtttGroupCode.Text = data.PTTT_GROUP_CODE;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbPtttGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbPtttGroup.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_GROUP data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbPtttGroup.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPtttGroupCode.Text = data.PTTT_GROUP_CODE;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Focus();
                            txtLoaiPT.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEkipTemp.EditValue != null)
                    {
                        var data = this.ekipTemps.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboEkipTemp.Properties.Buttons[1].Visible = true;
                            LoadGridEkipUserFromTemp(data.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadEmotionlessMethod(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbbEmotionlessMethod.Focus();
                    cbbEmotionlessMethod.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(o =>
                        o.IS_ACTIVE == 1
                        && (o.IS_FIRST == 1 || (o.IS_FIRST != 1 && o.IS_SECOND != 1))
                        && o.EMOTIONLESS_METHOD_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbbEmotionlessMethod.EditValue = data[0].ID;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            cboEkipTemp.Focus();
                            cboEkipTemp.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.EMOTIONLESS_METHOD_CODE == searchCode);
                            if (search != null)
                            {
                                cbbEmotionlessMethod.EditValue = search.ID;
                                cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                                cboEkipTemp.Focus();
                                cboEkipTemp.SelectAll();
                            }
                            else
                            {
                                cbbEmotionlessMethod.EditValue = null;
                                cbbEmotionlessMethod.Focus();
                                cbbEmotionlessMethod.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cbbEmotionlessMethod.EditValue = null;
                        cbbEmotionlessMethod.Focus();
                        cbbEmotionlessMethod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmotionlessMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadEmotionlessMethod(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            cboEkipTemp.Focus();
                            cboEkipTemp.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cbbEmotionlessMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbbEmotionlessMethod.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionlessMethod.Text = data.EMOTIONLESS_METHOD_CODE;
                            cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                            cboEkipTemp.Focus();
                            cboEkipTemp.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        internal void LoadGridEkipUserFromTemp(long ekipTempId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = ekipTempId;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);
               
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ekipTempUsers), ekipTempUsers));
                    
                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                    List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> glstExcuteRole=BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                    foreach (var ekipTempUser in ekipTempUsers)
                    {
                        HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                        var role = glstExcuteRole.Where(ex => ex.ID == ekipTempUser.EXECUTE_ROLE_ID).FirstOrDefault();
                        if(role.IS_ACTIVE==1){
                        ekipUserAdoTemp.EXECUTE_ROLE_ID = ekipTempUser.EXECUTE_ROLE_ID;
                        ekipUserAdoTemp.LOGINNAME = ekipTempUser.LOGINNAME;
                        ekipUserAdoTemp.USERNAME = ekipTempUser.USERNAME;

                        if (ekipUserAdoTemps.Count == 0)
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
                            ekipUserAdoTemps.Add(ekipUserAdoTemp);
                        }
                    }
                    gridControlEkip.DataSource = ekipUserAdoTemps;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cbbPtttGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbPtttGroup.EditValue = null;
                    txtPtttGroupCode.Text = "";
                    cbbPtttGroup.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbEmotionlessMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cbbEmotionlessMethod.EditValue = null;
                    txtEmotionlessMethod.Text = "";
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadLoaiPT(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboLoaiPT.Focus();
                    cboLoaiPT.ShowPopup();
                }
                else
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().Where(o => o.PTTT_PRIORITY_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboLoaiPT.EditValue = data[0].ID;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.PTTT_PRIORITY_CODE == searchCode);
                            if (search != null)
                            {
                                cboLoaiPT.EditValue = search.ID;
                                cboLoaiPT.Properties.Buttons[1].Visible = true;
                                //txtMethodCode.Focus();
                                //txtMethodCode.SelectAll();
                            }
                            else
                            {
                                cboLoaiPT.EditValue = null;
                                cboLoaiPT.Focus();
                                cboLoaiPT.ShowPopup();
                            }

                        }
                    }
                    else
                    {
                        cboLoaiPT.EditValue = null;
                        cboLoaiPT.Focus();
                        cboLoaiPT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoaiPT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim().ToUpper();
                    LoadLoaiPT(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboLoaiPT.Properties.Buttons[1].Visible = false;
                    cboLoaiPT.EditValue = null;
                    txtLoaiPT.Text = "";
                    txtLoaiPT.Focus();
                    txtLoaiPT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoaiPT.Text != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiPT.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiPT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiPT.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiPT.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                            cboLoaiPT.Properties.Buttons[1].Visible = true;
                            //txtMethodCode.Focus();
                            //txtMethodCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        cboEkipTemp.Properties.Buttons[1].Visible = false;
                        cboEkipTemp.EditValue = null;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HisEkipUserADO participant = new HisEkipUserADO();
                participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                this.ekipUserAdos.Add(participant);
                gridControlEkip.BeginUpdate();
                gridControlEkip.DataSource = null;
                gridControlEkip.DataSource = ekipUserAdos;
                gridControlEkip.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var participant = (HisEkipUserADO)gridViewEkip.GetFocusedRow();
                if (participant != null)
                {
                    gridControlEkip.BeginUpdate();
                    if (ekipUserAdos.Count > 0)
                    {
                        this.ekipUserAdos.Remove(participant);
                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = ekipUserAdos;
                    }
                    else
                    {
                        var dataGrdControl = gridControlEkip.DataSource as List<HisEkipUserADO>;
                        dataGrdControl.Remove(participant);
                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = dataGrdControl;
                    }
                    gridControlEkip.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCboName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                gridViewEkip.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;
                gridViewEkip.FocusedColumn = gridViewEkip.VisibleColumns[2];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BUTTON_ADD")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewEkip.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repositoryItemButtonAdd;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repositoryItemButtonDelete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "USERNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HIS.Desktop.ADO.HisEkipUserADO data = view.GetFocusedRow() as HIS.Desktop.ADO.HisEkipUserADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

                    List<string> loginNames = new List<string>();
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (data.LOGINNAME != null)
                            editor.EditValue = data.LOGINNAME;
                        MOS.Filter.HisExecuteRoleUserFilter filter = new MOS.Filter.HisExecuteRoleUserFilter();
                        filter.EXECUTE_ROLE_ID = data.EXECUTE_ROLE_ID;
                        List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER> executeRoleUsers = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());

                        if (executeRoleUsers != null && executeRoleUsers.Count > 0)
                        {
                            loginNames = executeRoleUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                        }
                    }
                    ComboAcsUser(editor, loginNames);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "USERNAME")
                {
                    gridViewEkip.ShowEditor();
                    ((GridLookUpEdit)gridViewEkip.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboExecuteRole(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                data = data.Where(dt => dt.IS_ACTIVE == 1).ToList(); ;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessLoadEkip()
        {
            try
            {
                if (this.ekipUserAdos != null && this.ekipUserAdos.Count > 0)
                {
                    gridControlEkip.BeginUpdate();
                    gridControlEkip.DataSource = null;
                    gridControlEkip.DataSource = this.ekipUserAdos;
                    gridControlEkip.EndUpdate();
                }
                else if (this.currentServiceADO != null && this.currentServiceADO.EKIP_ID.HasValue)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisEkipUserViewFilter hisEkipUserFilter = new MOS.Filter.HisEkipUserViewFilter();
                    hisEkipUserFilter.EKIP_ID = this.currentServiceADO.EKIP_ID;
                    var lst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>(ApiConsumer.HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisEkipUserFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (lst != null && lst.Count > 0)
                    {
                        this.ekipUserAdos = new List<HisEkipUserADO>();
                        foreach (var item in lst)
                        {
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>();
                            var HisEkipUserProcessing = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>(item);
                            if (item != lst[0])
                            {
                                HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            this.ekipUserAdos.Add(HisEkipUserProcessing);
                        }
                    }

                    gridControlEkip.BeginUpdate();
                    gridControlEkip.DataSource = null;
                    gridControlEkip.DataSource = this.ekipUserAdos;
                    gridControlEkip.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServPtttBySereServId()
        {
            try
            {
                if (this.sereServPTTT == null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServPtttViewFilter hisSereServPtttFilter = new HisSereServPtttViewFilter();
                    hisSereServPtttFilter.SERE_SERV_ID = this.currentServiceADO.ID;
                    var hisSereServPttts = new BackendAdapter(param)
                      .Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, hisSereServPtttFilter, param);
                    this.sereServPTTT = (hisSereServPttts != null && hisSereServPttts.Count > 0) ? hisSereServPttts.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboPTTTGroup()
        {
            try
            {
                List<HIS_PTTT_GROUP> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbbPtttGroup, datas, controlEditorADO);

                //if (this.sereServPTTT != null && this.sereServPTTT.PTTT_GROUP_ID != null)
                //{
                //    txtPtttGroupCode.Text = this.sereServPTTT.PTTT_GROUP_CODE;
                //    cbbPtttGroup.EditValue = this.sereServPTTT.PTTT_GROUP_ID;
                //    cbbPtttGroup.Properties.Buttons[1].Visible = true;
                //}
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cbbPtttGroup.EditValue), cbbPtttGroup.EditValue));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultCboPTTTGroupOnly()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.1");
                if (this.currentServiceADO.EKIP_ID == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.2");
                    long ptttGroupId = 0;

                    var surgMisuService = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o =>
                        o.ID == this.currentServiceADO.SERVICE_ID &&
                        (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentServiceADO.SERVICE_ID), this.currentServiceADO.SERVICE_ID) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => surgMisuService), surgMisuService));
                    if (surgMisuService != null)
                    {
                        if (surgMisuService.PTTT_GROUP_ID.HasValue)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3");
                            HIS_PTTT_GROUP ptttGroup = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_GROUP_ID);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ptttGroup), ptttGroup));
                            ptttGroupId = ptttGroup.ID;
                            txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                            cbbPtttGroup.Properties.Buttons[1].Visible = true;
                        }
                    }

                    if (ptttGroupId > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3.1");
                        cbbPtttGroup.EditValue = ptttGroupId;
                        cbbPtttGroup.Enabled = false;
                        txtPtttGroupCode.Enabled = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.3.2");
                        cbbPtttGroup.EditValue = null;
                        cbbPtttGroup.Enabled = true;
                        txtPtttGroupCode.Enabled = true;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.4");
                    var surgService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == this.currentServiceADO.SERVICE_ID);
                    if (surgService != null && surgService.PTTT_GROUP_ID != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("SetDefaultCboPTTTGroupOnly.5");
                        HIS_PTTT_GROUP ptttGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == surgService.PTTT_GROUP_ID);
                        cbbPtttGroup.EditValue = ptttGroup.ID;
                        txtPtttGroupCode.Text = ptttGroup.PTTT_GROUP_CODE;
                        cbbPtttGroup.Properties.Buttons[1].Visible = true;
                        cbbPtttGroup.Enabled = false;
                        txtPtttGroupCode.Enabled = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Lay PTTT_GROUP_ID mac dinh theo dich vu khong co du lieu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => surgService), surgService));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboEmotionlessMothod()
        {
            try
            {
                //List<HIS_EMOTIONLESS_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(p => p.IS_ACTIVE == 1
                //   && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList();

                List<HIS_EMOTIONLESS_METHOD> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>();

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1
                  && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbbEmotionlessMethod, datas, controlEditorADO);

                if (this.sereServPTTT != null)
                {
                    var emoless = this.sereServPTTT.EMOTIONLESS_METHOD_ID > 0 ? datas.Where(o => o.ID == this.sereServPTTT.EMOTIONLESS_METHOD_ID).First() : null;
                    txtEmotionlessMethod.Text = emoless != null ? emoless.EMOTIONLESS_METHOD_CODE : "";
                    cbbEmotionlessMethod.EditValue = emoless != null ? (long?)emoless.ID : null;
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = this.sereServPTTT.EMOTIONLESS_METHOD_ID > 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboLoaiPT()
        {
            try
            {
                List<HIS_PTTT_PRIORITY> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>();

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_PRIORITY_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_PRIORITY_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_PRIORITY_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboLoaiPT, datas, controlEditorADO);

                if (datas != null && this.sereServPTTT != null && this.sereServPTTT.PTTT_PRIORITY_ID > 0)
                {
                    var data = datas.FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_PRIORITY_ID);
                    if (data != null)
                    {
                        txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                        cboLoaiPT.EditValue = data.ID;
                        cboLoaiPT.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        txtLoaiPT.Text = "";
                        cboLoaiPT.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task ComboEkipTemp(GridLookUpEdit cbo)
        {
            try
            {
                string logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                ekipTemps = await new BackendAdapter(param)
                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>("/api/HisEkipTemp/Get", ApiConsumers.MosConsumer, filter, param);

                if (ekipTemps != null && ekipTemps.Count > 0)
                {
                    ekipTemps = ekipTemps.Where(o => o.IS_PUBLIC == 1 || o.CREATOR == logginName).OrderByDescending(o => o.CREATE_TIME).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EKIP_TEMP_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EKIP_TEMP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, ekipTemps, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessEkipUserForSave();
                ProcessSereServPtttForSave();
                actSaveClick(this.ekipUserAdos, this.sereServPTTT);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSereServPtttForSave()
        {
            try
            {
                if (this.sereServPTTT == null)
                {
                    this.sereServPTTT = new MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT();
                    this.sereServPTTT.SERE_SERV_ID = currentServiceADO.ID;
                    this.sereServPTTT.TDL_TREATMENT_ID = currentServiceADO.TDL_TREATMENT_ID;
                }

                //Phuong phap vô cảm
                this.sereServPTTT.EMOTIONLESS_METHOD_ID = cbbEmotionlessMethod.EditValue != null ? (long?)cbbEmotionlessMethod.EditValue : null;

                //Loai PTTT
                this.sereServPTTT.PTTT_GROUP_ID = cbbPtttGroup.EditValue != null ? (long?)cbbPtttGroup.EditValue : null;

                this.sereServPTTT.PTTT_PRIORITY_ID = cboLoaiPT.EditValue != null ? (long?)cboLoaiPT.EditValue : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessEkipUserForSave()
        {
            try
            {
                this.ekipUserAdos = gridControlEkip.DataSource as List<HisEkipUserADO>;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveEkipTemp_Click(object sender, EventArgs e)
        {
            try
            {
                var ekipUsers = gridControlEkip.DataSource as List<HisEkipUserADO>;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0 || o.EXECUTE_ROLE_ID == null).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show("Không có thông tin kip thực hiện");
                    return;
                }

                frmEkipTemp frm = new frmEkipTemp(ekipUsers, RefeshDataEkipTemp);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
                btnSave_Click(null, null);
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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
    }
}
