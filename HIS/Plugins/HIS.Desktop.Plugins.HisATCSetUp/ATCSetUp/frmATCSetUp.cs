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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Modules;


namespace HIS.Desktop.Plugins.HisATCSetUp.ATCSetUp
{ 
    public partial class frmATCSetUp : HIS.Desktop.Utility.FormBase
    {
        #region ---Declate---
        DelegateReturnMutilObject resultAct;
        List<HIS_ATC> listAtcChecks;
        List<ATCSetUpADO> lsAtcADO;
        List<ATCSetUpADO> listAtcChecked;
        List<HIS_ATC> lstAtcChecked = new List<HIS_ATC>();
        Module moduleCurrent;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        #endregion
        #region ---Contructor---
        public frmATCSetUp()
        {
            InitializeComponent();
        }
        public frmATCSetUp(DelegateReturnMutilObject _resultAtc, List<HIS_ATC> _listAtc, Module module)
        {
            InitializeComponent();
            this.resultAct = _resultAtc;
            this.listAtcChecks = _listAtc;
            this.moduleCurrent = module;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.Text = (this.moduleCurrent != null ? this.moduleCurrent.text : "");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Private Method---
        private void frmATCSetUp_Load(object sender, EventArgs e)
        {
            try
            {
                // Nap danh sach dang duoc chon TRUOC khi do du lieu len luoi
                // -> luoi moi biet dong nao can tich san va day len dau danh sach
                LoadDataChecked();
                FillDataTogrilControl();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataChecked()
        {
            try
            {
                this.lstAtcChecked = new List<HIS_ATC>();
                if (this.listAtcChecks != null && this.listAtcChecks.Count > 0)
                {
                    foreach (var item in this.listAtcChecks)
                    {
                        // Bo qua ban ghi null (ma phan biet da bi xoa khoi danh muc) va ban ghi trung
                        if (item == null) continue;
                        if (this.lstAtcChecked.Any(o => o.ID == item.ID)) continue;
                        this.lstAtcChecked.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataTogrilControl()
        {

            try
            {
                int numPageSize;
                if (ucpagin.pagingGrid != null)
                {
                    numPageSize = ucpagin.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadData(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucpagin.Init(LoadData, param, numPageSize);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadData(object data)
        {
            try
            {
                WaitingManager.Show();
                listAtcChecked = new List<ATCSetUpADO>();
                lsAtcADO = new List<ATCSetUpADO>();
                int start1 = ((CommonParam)data).Start ?? 0;
                int limit1 = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start1, limit1);
                HisAtcFilter hisAtcFilter = new HisAtcFilter();
                hisAtcFilter.KEY_WORD = txtSearch.Text;
                hisAtcFilter.ORDER_FIELD = "MODIFY_TIME";
                hisAtcFilter.ORDER_DIRECTION = "DESC";

                var atc = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<List<ATCSetUpADO>>(
                    "/api/HisAtc/Get",
                    HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                    hisAtcFilter,
                    param);

                int pageRowCount = 0;
                if (atc != null && atc.Data != null && atc.Data.Count > 0)
                {
                    pageRowCount = atc.Data.Count;
                    foreach (var item in atc.Data)
                    {
                        // Danh dau theo danh sach dang chon hien tai (lstAtcChecked) chu khong theo
                        // danh sach truyen vao ban dau -> giu nguyen tich chon khi tim kiem / chuyen trang
                        item.check = IsAtcChecked(item.ID);
                        this.lsAtcADO.Add(item);
                    }
                }

                // Bo sung cac ma dang chon nhung khong nam trong trang du lieu hien tai
                AppendCheckedNotInPage(this.lsAtcADO, start1);

                // Day cac dong da chon len dau danh sach (OrderByDescending on dinh -> giu thu tu tra ve tu server)
                this.listAtcChecked = this.lsAtcADO.OrderByDescending(o => o.check).ToList();

                gridViewATC.BeginUpdate();
                gridControlATC.DataSource = this.listAtcChecked;
                gridViewATC.EndUpdate();
                rowCount1 = (data == null ? 0 : pageRowCount);
                dataTotal1 = (atc == null || atc.Param == null ? 0 : atc.Param.Count ?? 0);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsAtcChecked(long id)
        {
            return this.lstAtcChecked != null && this.lstAtcChecked.Any(o => o != null && o.ID == id);
        }

        private void AppendCheckedNotInPage(List<ATCSetUpADO> lsData, int start)
        {
            try
            {
                // Chi chen o trang dau tien de khong lam sai lech so dong cua cac trang sau
                if (start > 0 || lsData == null || this.lstAtcChecked == null || this.lstAtcChecked.Count == 0) return;

                string keyWord = (txtSearch.Text ?? "").Trim();
                List<ATCSetUpADO> lsInsert = new List<ATCSetUpADO>();
                foreach (var item in this.lstAtcChecked)
                {
                    if (item == null) continue;
                    if (lsData.Any(o => o.ID == item.ID)) continue;
                    if (!MatchKeyWord(item, keyWord)) continue;

                    ATCSetUpADO ado = new ATCSetUpADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ATCSetUpADO>(ado, item);
                    ado.check = true;
                    lsInsert.Add(ado);
                }
                if (lsInsert.Count > 0)
                {
                    lsData.InsertRange(0, lsInsert);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private bool MatchKeyWord(HIS_ATC atc, string keyWord)
        {
            if (atc == null) return false;
            if (String.IsNullOrEmpty(keyWord)) return true;

            string key = keyWord.ToUpper();
            return (!String.IsNullOrEmpty(atc.ATC_CODE) && atc.ATC_CODE.ToUpper().Contains(key))
                || (!String.IsNullOrEmpty(atc.ATC_NAME) && atc.ATC_NAME.ToUpper().Contains(key))
                || (!String.IsNullOrEmpty(atc.BHYT_CODE) && atc.BHYT_CODE.ToUpper().Contains(key));
        }
        #endregion
        #region ---Even---
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataTogrilControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkCheck_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var datarow = gridViewATC.GetFocusedRow() as ATCSetUpADO;
                if (datarow == null) return;

                // EditValueChanged phat sinh TRUOC khi gia tri duoc post xuong dong du lieu
                // -> lay gia tri moi truc tiep tu editor, khong doc datarow.check (van con gia tri cu)
                var checkEdit = sender as DevExpress.XtraEditors.CheckEdit;
                bool isChecked = (checkEdit != null && checkEdit.EditValue != null)
                    ? Convert.ToBoolean(checkEdit.EditValue)
                    : !datarow.check;
                datarow.check = isChecked;

                if (this.lstAtcChecked == null)
                {
                    this.lstAtcChecked = new List<HIS_ATC>();
                }

                if (isChecked)
                {
                    if (!this.lstAtcChecked.Any(o => o != null && o.ID == datarow.ID))
                    {
                        HIS_ATC data = new HIS_ATC();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ATC>(data, datarow);
                        this.lstAtcChecked.Add(data);
                    }
                }
                else
                {
                    this.lstAtcChecked.RemoveAll(o => o != null && o.ID == datarow.ID);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region ---Even Click---

        private void btnChoise_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.lstAtcChecked != null && this.resultAct != null)
                {
                    this.resultAct(new List<HIS_ATC>[] { this.lstAtcChecked });
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    simpleButton1_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnChoise.Enabled)
            {
                btnChoise_Click(null, null);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataTogrilControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutControl1_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion
    }
}
