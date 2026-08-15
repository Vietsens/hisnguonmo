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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.Desktop.Plugins.HisNumOrderBlockChooser.ADO;
using DevExpress.XtraLayout;
using HIS.Desktop.ApplicationFont;

namespace HIS.Desktop.Plugins.HisNumOrderBlockChooser.Run
{
    public partial class UCTimes : UserControl
    {
        private List<HisNumOrderBlockSDO> ListNumOrder;
        private Action<TimeADO> SelectNumOrder;
        //private DevExpress.XtraEditors.SimpleButton LastButton;
        Base.Block LastButton;

        private int Dai = 30;
        private int Rong = 50;
        long? timeSelected;

        //Chu thich ly do o bi khoa (viec 54282)
        private ToolTip toolTipBlock = new ToolTip();

        /// <summary>
        /// 20260813143944 -> "14:39"
        /// </summary>
        private string ToHourText(long? timeNumber)
        {
            try
            {
                if (!timeNumber.HasValue || timeNumber.Value <= 0)
                {
                    return "";
                }
                string s = timeNumber.Value.ToString();
                if (s.Length < 12)
                {
                    return "";
                }
                return s.Substring(8, 2) + ":" + s.Substring(10, 2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "";
            }
        }

        /// <summary>
        /// Chi ap luat "da qua gio" khi ngay dang xem la HOM NAY (viec 54282).
        /// Ngay tuong lai thi moi o deu con hieu luc.
        /// </summary>
        private bool isToday = true;

        public UCTimes(List<MOS.SDO.HisNumOrderBlockSDO> list, Action<TimeADO> selectNumOrder)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.ListNumOrder = list;
            this.SelectNumOrder = selectNumOrder;
        }
        public UCTimes(List<MOS.SDO.HisNumOrderBlockSDO> list, Action<TimeADO> selectNumOrder, long? timeSelected)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.ListNumOrder = list;
            this.SelectNumOrder = selectNumOrder;
            this.timeSelected = timeSelected;
        }
        public UCTimes(List<MOS.SDO.HisNumOrderBlockSDO> list, Action<TimeADO> selectNumOrder, long? timeSelected, bool _isToday)
        {
            InitializeComponent();
            this.ListNumOrder = list;
            this.SelectNumOrder = selectNumOrder;
            this.timeSelected = timeSelected;
            this.isToday = _isToday;
        }

        private void UCTimes_Load(object sender, EventArgs e)
        {
            try
            {

                if (ListNumOrder != null && ListNumOrder.Count > 0)
                {
                    float fontSize = HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize();
                    if (fontSize != ApplicationFontConfig.FontSize825)
                    {
                        Dai += (int)((fontSize - ApplicationFontConfig.FontSize825) / 10);
                        Rong += (int)((fontSize - ApplicationFontConfig.FontSize825) / 10);
                    }

                    List<TimeADO> lstData = (from n in ListNumOrder select new TimeADO(n)).ToList();
                    var groupHour = lstData.GroupBy(o => o.HOUR).ToList();

                    groupHour = groupHour.OrderBy(o => o.Key).ToList();

                    for (int i = 0; i < groupHour.Count; i++)
                    {
                        List<TimeADO> times = groupHour[i].OrderBy(o => o.FROM_TIME).ToList();
                        for (int j = 0; j < times.Count; j++)
                        {
                            var blockTime = new Base.Block();
                            blockTime.Text = times[j].HOUR_STR;
                            blockTime.ForeColor = Color.Blue;

                            //Moi o chi mo hoac khoa vi MOT ly do, va ly do do duoc ghi vao chu thich
                            //de nguoi dung khong phai doan (viec 54282)
                            string lyDo = null;

                            //Gio hien tai phai dem 0 (truoc day noi chuoi Hour + Minute nen 9 gio 05 thanh "95",
                            //khien o gio da qua van bam duoc gan nhu ca ngay)
                            var time = DateTime.Now.ToString("HHmm");
                            if (this.isToday
                                && Convert.ToInt64(times[j].HOUR_STR.Replace(":", "")) < Convert.ToInt64(time))
                            {
                                blockTime.Enabled = false;
                                blockTime.ForeColor = Color.FromArgb(150, 150, 150);
                                blockTime.Font = new Font(blockTime.Font, FontStyle.Strikeout);
                                lyDo = "Đã qua giờ";
                            }

                            if (timeSelected != null && timeSelected != 0 && timeSelected == Convert.ToInt64(times[j].HOUR_STR.Replace(":", "")))
                            {
                                blockTime.Enabled = false;
                            }

                            if (times[j].IS_ISSUED == 1)
                            {
                                blockTime.Enabled = false;
                                blockTime.ForeColor = new Color();
                                blockTime.Font = new Font(blockTime.Font, FontStyle.Regular);
                                lyDo = "Đã có người đặt";

                                if (times[j].IS_HOLDING == 1)
                                {
                                    string denGio = ToHourText(times[j].HOLD_EXPIRE_TIME);
                                    if (times[j].IS_MINE == 1)
                                    {
                                        //Cho cua chinh minh: van cho bam lai, tru khi da qua gio
                                        blockTime.Enabled = blockTime.Font.Strikeout ? false : true;
                                        //Nen dac chu trang thay vi xanh nhat: o minh dang giu phai
                                        //nhin ra ngay giua luoi o trang (viec 54282)
                                        blockTime.ForeColor = Color.White;
                                        blockTime.BackColor = Color.FromArgb(21, 101, 192);
                                        lyDo = "Bạn đang giữ chỗ này" + (!string.IsNullOrEmpty(denGio) ? ", đến " + denGio : "");
                                    }
                                    else
                                    {
                                        blockTime.ForeColor = Color.FromArgb(180, 83, 9);
                                        blockTime.BackColor = Color.FromArgb(253, 240, 216);
                                        lyDo = "Người khác đang giữ" + (!string.IsNullOrEmpty(denGio) ? ", đến " + denGio : "");
                                    }
                                }
                            }
                            else if (blockTime.Enabled)
                            {
                                lyDo = "Còn trống";
                            }

                            if (!string.IsNullOrEmpty(lyDo))
                            {
                                this.toolTipBlock.SetToolTip(blockTime, times[j].HOUR_STR + " — " + lyDo);
                            }

                            blockTime.Tag = times[j];
                            blockTime.TextAlign = ContentAlignment.MiddleCenter;
                            blockTime.GridX = i;
                            blockTime.GridY = j;
                            blockTime.Location = new System.Drawing.Point(blockTime.GridX * Rong + (blockTime.GridX > 0 ? (blockTime.GridX) * 5 : 0), blockTime.GridY * Dai + (blockTime.GridY > 0 ? (blockTime.GridY) * 5 : 0));
                            blockTime.Size = new System.Drawing.Size(Rong, Dai);
                            blockTime.TabIndex = 0;
                            blockTime.TabStop = false;
                            blockTime.Click += new EventHandler(button_Click);

                            this.xtraScrollableControl1.Controls.Add(blockTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            try
            {
                if (LastButton != null)
                {
                    LastButton.Font = new Font(LastButton.Font, FontStyle.Regular);
                }

                Base.Block button = sender as Base.Block;
                if (button != null)
                {
                    LastButton = button;

                    button.Font = new Font(button.Font, FontStyle.Bold);
                    if (button.Tag != null && this.SelectNumOrder != null)
                    {
                        TimeADO dataNum = button.Tag as TimeADO;
                        this.SelectNumOrder(dataNum);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
