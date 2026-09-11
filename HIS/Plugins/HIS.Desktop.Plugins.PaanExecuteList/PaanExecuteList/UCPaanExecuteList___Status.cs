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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList
{
    /// <summary>
    /// Hang thong tin phia duoi luoi:
    ///   - "SLYC BHYT da xu ly/SL toi da"
    ///   - "SLDV da xu ly"
    ///   - Tick "Uu tien TG xu ly"
    ///   - Tick "Tu dong mo man hinh cho"
    ///
    /// Chep tu HIS.Desktop.Plugins.ExecuteRoom:
    ///   UCExecuteRoom___Load.cs:3293 (LoadServiceReqCount)
    ///   UCExecuteRoom___Load.cs:3578 (LoadSereServCount)
    ///   UCExecuteRoom.cs:4597       (ckKQCLS_CheckedChanged)
    ///   UCExecuteRoom.cs:4020       (chkScreenSaver_CheckedChanged)
    ///   UCExecuteRoom.cs:821        (InitControlState)
    /// </summary>
    public partial class UCPaanExecuteList
    {
        #region Bien luu trang thai o tick

        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker = null;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO = null;

        /// <summary>
        /// Chan khong cho su kien CheckedChanged chay khi dang KHOI PHUC trang thai
        /// luc mo man hinh. Neu khong co co nay se nap lai luoi thua.
        /// </summary>
        private bool isNotLoadWhileChangeControlStateInFirst = false;

        private const string ModuleLinkName = "HIS.Desktop.Plugins.PaanExecuteList";

        #endregion

        #region Khoi phuc / luu trang thai o tick

        /// <summary>
        /// Khoi phuc trang thai 2 o tick tu bo nho dem cua nguoi dung.
        /// Chep tu UCExecuteRoom.cs:821 (InitControlState).
        /// </summary>
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item == null || String.IsNullOrWhiteSpace(item.KEY)) continue;

                        if (item.KEY == ckKQCLS.Name)
                        {
                            ckKQCLS.Checked = (item.VALUE == "1");
                        }
                        else if (item.KEY == chkScreenSaver.Name)
                        {
                            chkScreenSaver.Checked = (item.VALUE == "1");
                        }
                        else if (item.KEY == txtGateNumber.Name)
                        {
                            txtGateNumber.Text = item.VALUE;
                        }
                        else if (item.KEY == txtStepNumber.Name)
                        {
                            txtStepNumber.Text = item.VALUE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                isNotLoadWhileChangeControlStateInFirst = false;
            }
        }

        /// <summary>Luu trang thai mot control vao bo nho dem.</summary>
        private void SaveControlState(string key, string value)
        {
            try
            {
                if (this.controlStateWorker == null) return;

                var csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.Where(o => o.KEY == key && o.MODULE_LINK == ModuleLinkName).FirstOrDefault()
                    : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = key;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;

                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// "Uu tien TG xu ly" - doi thu tu sap xep chu KHONG loc bot du lieu.
        /// Chep tu UCExecuteRoom.cs:4597.
        /// </summary>
        private void ckKQCLS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst) return;

                SaveControlState(ckKQCLS.Name, ckKQCLS.Checked ? "1" : "");
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// "Tu dong mo man hinh cho" - chi luu trang thai, khong nap lai luoi.
        /// Chep tu UCExecuteRoom.cs:4020.
        /// </summary>
        private void chkScreenSaver_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst) return;

                SaveControlState(chkScreenSaver.Name, chkScreenSaver.Checked ? "1" : "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mo man hinh cho neu nguoi dung da tick.
        /// Chep tu UCExecuteRoom.cs:4057 (LoadDefaultScreenSaver).
        /// </summary>
        private void LoadDefaultScreenSaver()
        {
            try
            {
                if (!chkScreenSaver.Checked) return;

                long roomId = GetRoomId();
                var screenSaverRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (screenSaverRoom == null) return;

                if (!String.IsNullOrEmpty(screenSaverRoom.SCREEN_SAVER_MODULE_LINK))
                {
                    List<object> listObj = new List<object>();
                    listObj.Add(true);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                        screenSaverRoom.SCREEN_SAVER_MODULE_LINK, roomId, GetRoomTypeId(), listObj);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        String.Format("Phòng {0} chưa được cấu hình màn hình chờ",
                            screenSaverRoom.ROOM_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Hai nhan dem

        /// <summary>
        /// "SLDV da xu ly" - so dich vu da xu ly trong ngay tai phong hien tai.
        /// Chep tu UCExecuteRoom___Load.cs:3578 (LoadSereServCount).
        ///
        /// LUU Y NGHIEP VU: con so nay tinh theo PHONG DANG DUNG, khong lien quan
        /// den danh sach Giai phau benh toan vien dang hien tren luoi.
        /// Giu lai theo dung yeu cau "giu nguyen cac truong thong tin ben duoi".
        /// </summary>
        private void LoadSereServCount()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServCountFilter filter = new HisSereServCountFilter();
                filter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                    DateTime.Now.ToString("yyyyMMdd") + "000000");
                filter.EXECUTE_ROOM_ID = GetRoomId();

                var sereServCount = new BackendAdapter(param).Get<HisSereServCountSDO>(
                    "api/HisSereServ/GetCount", ApiConsumers.MosConsumer, filter, param);

                lblSereServCount.Text = (sereServCount != null)
                    ? String.Format("{0}/{1}", sereServCount.TotalProcess, sereServCount.TotalAssign)
                    : "";
            }
            catch (Exception ex)
            {
                lblSereServCount.Text = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// "SLYC BHYT da xu ly/SL toi da" - so y lenh BHYT nguoi dung da xu ly
        /// trong ngay, so voi han muc cua nhan vien.
        /// Chep rut gon tu UCExecuteRoom___Load.cs:3293 (LoadServiceReqCount),
        /// chi giu nhanh theo NHAN VIEN (nhanh mac dinh).
        /// Bo nhanh theo BAN vi man nay khong gan voi ban lam viec nao.
        ///
        /// LUU Y NGHIEP VU: giong LoadSereServCount, con so nay theo NGUOI DUNG
        /// chu khong theo danh sach dang hien.
        /// </summary>
        private void LoadServiceReqCount()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (String.IsNullOrWhiteSpace(loginName)) return;

                var employee = BackendDataWorker.Get<HIS_EMPLOYEE>()
                    .FirstOrDefault(o => o.LOGINNAME == loginName);

                CommonParam param = new CommonParam();
                HisServiceReqCountFilter filter = new HisServiceReqCountFilter();
                filter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(
                    DateTime.Now.ToString("yyyyMMdd") + "000000");
                filter.EXECUTE_LOGINNAME = loginName;

                if (employee != null && employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue)
                {
                    filter.IS_BHYT = true;
                }

                var count = new BackendAdapter(param).Get<long>(
                    "api/HisServiceReq/GetCount", ApiConsumers.MosConsumer, filter, param);

                if (employee != null)
                {
                    long? max = employee.MAX_BHYT_SERVICE_REQ_PER_DAY ?? employee.MAX_SERVICE_REQ_PER_DAY;
                    lblServiceReqCount.Text = max.HasValue
                        ? String.Format("{0}/{1}", count, max.Value)
                        : count.ToString();

                    lblServiceReqCount.Appearance.ForeColor =
                        (max.HasValue && count > max.Value) ? Color.Red : new Color();
                }
                else
                {
                    lblServiceReqCount.Text = count.ToString();
                }
            }
            catch (Exception ex)
            {
                lblServiceReqCount.Text = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
