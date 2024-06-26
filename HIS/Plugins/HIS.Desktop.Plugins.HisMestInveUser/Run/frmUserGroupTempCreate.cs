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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisMestInveUser.Validtion;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.HisMestInveUser.Run
{
    public partial class frmUserGroupTempCreate : Form
    {
        List<HisMestInveUserAdo> listRoleUserAdo;
        int positionHandleControlMedicineTypeInfo = -1;
        public frmUserGroupTempCreate()
        {
            InitializeComponent();
            SetIcon();
        }
        public frmUserGroupTempCreate(List<HisMestInveUserAdo> listRoleUserAdo)
        {
            this.listRoleUserAdo = listRoleUserAdo;
            InitializeComponent();
            SetIcon();
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

        private void frmUserGroupTempCreate_Load(object sender, EventArgs e)
        {
            try
            {
                ValidMaxlengthTxt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;

                if (this.listRoleUserAdo != null && this.listRoleUserAdo.Count > 0)
                {
                    this.positionHandleControlMedicineTypeInfo = -1;
                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS_USER_GROUP_TEMP_DT> UserGroupTempDts = new List<HIS_USER_GROUP_TEMP_DT>();
                    MOS.EFMODEL.DataModels.HIS_USER_GROUP_TEMP inputAdo = new MOS.EFMODEL.DataModels.HIS_USER_GROUP_TEMP();
                    inputAdo.USER_GROUP_TEMP_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_USER_GROUP_TEMP_TYPE.ID_HoiDongKiemKe;
                    inputAdo.USER_GROUP_TEMP_NAME = txtTempName.Text.Trim();
                    foreach (var item in this.listRoleUserAdo)
                    {
                        HIS_USER_GROUP_TEMP_DT ado = new HIS_USER_GROUP_TEMP_DT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_USER_GROUP_TEMP_DT>(ado, item);
                        ado.USERNAME = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME
                             == item.LOGINNAME).USERNAME;
                        UserGroupTempDts.Add(ado);
                    }
                    inputAdo.USER_GROUP_TEMP_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_USER_GROUP_TEMP_TYPE.ID_HoiDongKiemKe;

                    inputAdo.HIS_USER_GROUP_TEMP_DT = UserGroupTempDts;
                    //inputAdo.USER_GROUP_TEMP_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("api/HisUserGroupTemp/Create ,, Gui Len : -- ", inputAdo));
                    var rsOutPut = new BackendAdapter(param).Post<HIS_USER_GROUP_TEMP>("api/HisUserGroupTemp/Create", ApiConsumers.MosConsumer, inputAdo, param);
                    if (rsOutPut != null)
                    {
                        success = true;
                        this.Close();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidMaxlengthTxt()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtTempName;
                validateMaxLength.maxLength = 200;
                dxValidationProvider1.SetValidationRule(txtTempName, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

                if (positionHandleControlMedicineTypeInfo == -1)
                {
                    positionHandleControlMedicineTypeInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlMedicineTypeInfo > edit.TabIndex)
                {
                    positionHandleControlMedicineTypeInfo = edit.TabIndex;
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }
    }
}
