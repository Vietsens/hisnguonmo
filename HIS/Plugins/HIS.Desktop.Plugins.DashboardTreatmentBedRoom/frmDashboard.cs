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
using HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.EFMODEL.Decorator;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom
{
    /// <summary>
    /// Man hinh mo rong: bang dien tu buong benh.
    ///
    /// Mo tu frmTreatmentBedRoom khi gat toggle. Nhan vao khoa, danh sach ID phong da tich 
    /// va chu ky lam moi. Tu cac phong do lay ra buong benh, moi buong mot the tren luoi.
    /// Cua so khong co nut dieu khien, chi thoat bang phim ESC.
    /// </summary>
    public partial class frmDashboard : Form
    {
        /// <summary>Khoa dang xem, lay tu man hinh thiet lap.</summary>
        private long departmentId;

        /// <summary>Danh sach ID phong duoc tich ben man hinh thiet lap.</summary>
        private List<long> roomIds = new List<long>();

        /// <summary>Buong benh cua cac phong tren, lay tu api/HisBedRoom/Get.</summary>
        private List<HIS_BED_ROOM> bedRooms = new List<HIS_BED_ROOM>();

        /// <summary>So cot mac dinh khi nguoi dung khong dat, hoac dat gia tri khong dung duoc.</summary>
        public const int DEFAULT_COLUMN_COUNT = 4;

        /// <summary>Gioi han tren cua so cot. Qua nguong nay the buong hep den muc khong doc noi.</summary>
        private const int MAX_COLUMN_COUNT = 12;

        public frmDashboard()
            : this(0, null, 0, DEFAULT_COLUMN_COUNT)
        {
        }

        /// <param name="departmentId">Khoa dang xem.</param>
        /// <param name="roomIds">ID cac phong duoc tich ben frmTreatmentBedRoom.</param>
        /// <param name="reloadSecond">Chu ky lam moi tinh bang giay. 0 = khong tu lam moi.</param> 
        /// <param name="columnCount">So cot nguoi dung chon. Ngoai khoang cho phep thi lay mac dinh.</param> 
        public frmDashboard(long departmentId, List<long> roomIds, int reloadSecond, int columnCount)
        {
            try
            {
                InitializeComponent();

                this.departmentId = departmentId;
                this.roomIds = roomIds ?? new List<long>();
                this.ucBoard.RefreshIntervalSecond = reloadSecond;
                this.ucBoard.ColumnCount = NormalizeColumnCount(columnCount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chot so cot ngay tai dau vao. Man hinh thiet lap chi doc so tren spin roi chuyen sang,
        /// khong tu suy dien - moi quyet dinh ve gia tri hop le nam o day cho khoi rai rac hai noi.
        /// Bo trong, so am hoac qua lon deu ve mac dinh.
        /// </summary>
        private int NormalizeColumnCount(int value)
        {
            if (value < 1 || value > MAX_COLUMN_COUNT)
            {
                if (value != 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "So cot khong dung duoc: {0}. Lay mac dinh {1}.", value, DEFAULT_COLUMN_COUNT));
                }
                return DEFAULT_COLUMN_COUNT;
            }
            return value;
        }

        /// <summary>Khoa dang xem.</summary>
        public long DepartmentId
        {
            get { return this.departmentId; }
        }

        /// <summary>ID cac buong benh dang hien thi tren bang.</summary>
        public List<long> GetBedRoomIds()
        {
            if (this.bedRooms == null) return new List<long>();
            return this.bedRooms.Select(o => o.ID).ToList();
        }

        #region Load
        private void frmDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadBedRooms();
                GetApiData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Resources.Lang", typeof(frmDashboard).Assembly);

                this.Text = GetLang("frmDashboard.Text", this.Text);
                this.lblEscHint.Text = GetLang("frmDashboard.EscHint", this.lblEscHint.Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
        #endregion

        #region Gọi api
        private void GetApiData()
        {
            try
            {
                List<long> BED_ROOM_IDs = GetBedRoomIds();

                HisTreatmentBedRoomDashboardFilterSDO sdo = new HisTreatmentBedRoomDashboardFilterSDO();
                sdo.DEPARTMENT_ID = this.departmentId;
                sdo.BED_ROOM_IDs = BED_ROOM_IDs;

                HisTreatmentBedRoomDashboardSDO data = new BackendAdapter(new CommonParam())
                    .Post<HisTreatmentBedRoomDashboardSDO>("api/HisTreatmentBedRoom/Dashboard", ApiConsumers.MosConsumer, sdo, new CommonParam());

                if (data != null)
                {
                    ucBoard.SetData(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion


        #region Load data
        /// <summary>
        /// Tu cac ROOM_ID duoc tich lay ra buong benh tuong ung.
        /// Loc ngay tren server bang ROOM_IDs, khong keo het buong toan vien ve roi loc o client.
        /// </summary>
        private void LoadBedRooms()
        {
            this.bedRooms = new List<HIS_BED_ROOM>();
            try
            {
                if (this.roomIds == null || this.roomIds.Count == 0)
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisBedRoomFilter filterHisBedRoom = new HisBedRoomFilter();
                filterHisBedRoom.ROOM_IDs = this.roomIds.ToList();
                filterHisBedRoom.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                List<HIS_BED_ROOM> apiHisBedRoom = new BackendAdapter(param)
                    .Get<List<HIS_BED_ROOM>>("api/HisBedRoom/Get", ApiConsumers.MosConsumer, filterHisBedRoom, param);

                if (apiHisBedRoom == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("api/HisBedRoom/Get tra ve null. ROOM_IDs: "
                        + string.Join(",", this.roomIds));
                    return;
                }

                this.bedRooms = apiHisBedRoom;
                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "LoadBedRooms: DEPARTMENT_ID={0}, so phong={1}, so buong={2}",
                    this.departmentId, this.roomIds.Count, this.bedRooms.Count));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Public
        /// <summary>Ban to yeu cau nap lai theo chu ky. Bat su kien nay roi goi lai SetData.</summary>
        public event EventHandler DataRefreshRequested
        {
            add { this.ucBoard.DataRefreshRequested += value; }
            remove { this.ucBoard.DataRefreshRequested -= value; }
        }

        /// <summary>Ban ra khi nguoi dung bam vao mot giuong dang co nguoi nam.</summary>
        public event EventHandler<TreatmentBedRoomDashboardBedSDO> BedClicked
        {
            add { this.ucBoard.BedClicked += value; }
            remove { this.ucBoard.BedClicked -= value; }
        }

        /// <summary>Chu ky tu nap lai, tinh bang giay. 0 = tat.</summary>
        public int RefreshIntervalSecond
        {
            get { return this.ucBoard.RefreshIntervalSecond; }
            set { this.ucBoard.RefreshIntervalSecond = value; }
        }
        #endregion

        #region Close by ESC
        /// <summary>
        /// Cua so khong co nut dong nen ESC la duong thoat duy nhat.
        /// Dung ProcessCmdKey chu khong dung KeyDown: phim van an duoc ke ca khi con tro dang
        /// nam trong mot control con, va khong bi editor cua DevExpress nuot mat.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
    }
}
