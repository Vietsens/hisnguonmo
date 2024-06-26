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
using HIS.UC.ExamTreatmentFinish.ADO;
using DevExpress.XtraLayout;
using HIS.Desktop.ApplicationFont;

namespace HIS.UC.ExamTreatmentFinish.Run
{
    public partial class UCTimes : UserControl
    {
        private List<HisNumOrderBlockSDO> ListNumOrder;
        private Action<TimeADO> SelectNumOrder;
        //private DevExpress.XtraEditors.SimpleButton LastButton;
        Base.Block LastButton;

        private int Dai = 30;
        private int Rong = 50;

        public UCTimes(List<MOS.SDO.HisNumOrderBlockSDO> list, Action<TimeADO> selectNumOrder)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.ListNumOrder = list;
            this.SelectNumOrder = selectNumOrder;
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
                            if (times[j].IS_ISSUED == 1)
                            {
                                blockTime.Enabled = false;
                                blockTime.ForeColor = new Color();
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
