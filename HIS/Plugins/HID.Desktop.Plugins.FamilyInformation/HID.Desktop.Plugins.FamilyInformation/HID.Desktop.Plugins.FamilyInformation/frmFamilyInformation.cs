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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Modules;
using Inventec.Core;
using HID.Filter;
using Inventec.Common.Adapter;
using HID.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HID.Desktop.Plugins.FamilyInformation.HID.Desktop.Plugins.FamilyInformation
{
    public partial class frmFamilyInformation : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        String person_code = "";
        Module moduleData = new Module();
        HID_PERSON person = new HID_PERSON();
        public frmFamilyInformation(Module _moduleData, String _person_code)
            : base(_moduleData)
        {
            InitializeComponent();
            this.person_code = _person_code;
            this.moduleData = _moduleData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }



        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<HID_PERSON>> apiResult = null;
                HidPersonFilter filter = new HidPersonFilter();
                filter.IS_ACTIVE = 1;
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HID_PERSON>>("api/HidPerson/Get", ApiConsumers.HidConsumer, filter, paramCommon);

                if (apiResult != null)
                {
                    var data = (List<HID_PERSON>)apiResult.Data;
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HidPersonFilter filter)
        {
            filter.KEY_WORD = txtFind.Text.Trim();
            filter.HOUSEHOLD_CODE = txtHouseCode.Text.Trim();
        }

        private void frmFamilyInformation_Load(object sender, EventArgs e)
        {
            LoadDefautValue();
            FillDataToGrid();
            SetLanguage();

        }

        private void SetLanguage()
        {
            if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
            {
                this.Text = this.moduleData.text;
            }
        }

        private void LoadDefautValue()
        {
            try
            {
                CommonParam parm = new CommonParam();
                HidPersonFilter filter = new HidPersonFilter();
                filter.PERSON_CODE = person_code;
                var person1 = new BackendAdapter(parm).Get<List<HID_PERSON>>("api/HidPerson/Get", ApiConsumers.HidConsumer, filter, parm);
                if (person1 != null && person1.Count > 0)
                {
                    person = person1.FirstOrDefault();
                    txtPersoncode.Text = person1.FirstOrDefault().PERSON_CODE;
                    txtName.Text = person1.FirstOrDefault().LAST_NAME + " " + person1.FirstOrDefault().FIRST_NAME;
                    txtMotherCode.Text = person1.FirstOrDefault().MOTHER_CODE;
                    txtMotherName.Text = person1.FirstOrDefault().MOTHER_NAME;
                    txtFatherCode.Text = person1.FirstOrDefault().FATHER_CODE;
                    txtFatherName.Text = person1.FirstOrDefault().FATHER_NAME;
                    txtHouseCode.Text = person1.FirstOrDefault().HOUSEHOLD_CODE;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HID_PERSON pData = (HID_PERSON)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "GENDER")
                    {
                        e.Value = pData.GENDER_ID == 1 ? "Nữ" : "Nam";
                    }
                    else if (e.Column.FieldName == "PERSON_NAME")
                    {
                        e.Value = pData.LAST_NAME + " " + pData.FIRST_NAME;
                    }
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FillDataToGrid();
        }

        private void txtFatherCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtFatherCode.Text != null && txtFatherCode.Text != "")
                {
                    try
                    {
                        txtFatherCode.Text = String.Format("{0:000000000}", Convert.ToInt32(txtFatherCode.Text));
                    }
                    catch
                    {
                        MessageManager.Show("Nhập sai mã số y tế");
                        return;
                    }
                    CommonParam parm = new CommonParam();
                    HidPersonFilter filter = new HidPersonFilter();
                    filter.PERSON_CODE = txtFatherCode.Text.Trim();
                    var father = new BackendAdapter(parm).Get<List<HID_PERSON>>("api/HidPerson/Get", ApiConsumers.HidConsumer, filter, parm);
                    if (father == null)
                    {
                        MessageManager.Show("Không tìm thấy mã số y tế đã nhập");
                        txtFatherCode.Text = "";
                        gridControl1.Focus();
                        return;
                    }
                    else
                    {
                        txtFatherName.Text = father.FirstOrDefault().LAST_NAME + " " + father.FirstOrDefault().FIRST_NAME;
                        txtMotherCode.Focus();

                    }
                }
                else
                {
                    txtFatherName.Text = "";
                    txtMotherCode.Focus();
                }
            }
        }

        private void txtMotherCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtMotherCode.Text != null && txtMotherCode.Text != "")
                {
                    try
                    {
                        txtMotherCode.Text = String.Format("{0:000000000}", Convert.ToInt32(txtMotherCode.Text));
                    }
                    catch
                    {
                        MessageManager.Show("Nhập sai mã số y tế");
                        return;
                    }
                    CommonParam parm = new CommonParam();
                    HidPersonFilter filter = new HidPersonFilter();
                    filter.PERSON_CODE = txtMotherCode.Text.Trim();
                    var mother = new BackendAdapter(parm).Get<List<HID_PERSON>>("api/HidPerson/Get", ApiConsumers.HidConsumer, filter, parm);
                    if (mother == null)
                    {
                        MessageManager.Show("Không tìm thấy mã số y tế đã nhập");
                        txtMotherCode.Text = "";
                        gridControl1.Focus();
                        txtMotherCode.Focus();
                        return;
                    }
                    else
                    {
                        txtMotherName.Text = mother.FirstOrDefault().LAST_NAME + " " + mother.FirstOrDefault().FIRST_NAME;
                        btnSave.Focus();
                    }
                }
                else
                {
                    txtMotherName.Text = "";
                    btnSave.Focus();
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Boolean success = false;
                CommonParam param = new CommonParam();
                if (txtMotherCode.Text.Trim() == person.PERSON_CODE)
                {
                    MessageManager.Show("Không nhập trùng với mã y tế của bệnh nhân");
                    return;
                }
                else
                {
                    person.MOTHER_CODE = txtMotherCode.Text.Trim();
                    person.MOTHER_NAME = txtMotherName.Text.Trim();
                }
                if (txtFatherCode.Text.Trim() == person.PERSON_CODE)
                {
                    MessageManager.Show("Không nhập trùng với mã y tế của bệnh nhân");
                    return;
                }
                else
                {
                    person.FATHER_CODE = txtFatherCode.Text.Trim();
                    person.FATHER_NAME = txtFatherName.Text.Trim();
                }
                LogSystem.Info(LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => person), person));
                var resultData = new BackendAdapter(param).Post<HID_PERSON>("api/HidPerson/Update", ApiConsumers.HidConsumer, person, param);
                updateHisPatient();
                if (resultData != null) { success = true; }
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {

            }
        }

        private void updateHisPatient()
        {
            CommonParam paramCommon = new CommonParam();
            List<HIS_PATIENT> apiResult = null;
            HisPatientFilter filter = new HisPatientFilter();
            //filter.PERSON_CODE = txtPersoncode.Text;
            apiResult = new BackendAdapter(paramCommon).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, paramCommon);
            var x = apiResult.Where(o => o.PERSON_CODE == txtPersoncode.Text).ToList().FirstOrDefault();
            if (txtMotherName.Text != null && txtMotherName.Text != "") x.MOTHER_NAME = txtMotherName.Text;
            if (txtFatherName.Text != null && txtFatherName.Text != "") x.FATHER_NAME = txtFatherName.Text;
            CommonParam param = new CommonParam();
            var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/Update", ApiConsumers.MosConsumer, x, param);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
