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
using DevExpress.XtraBars;
using HIS.Desktop.Plugins.LisDeliveryNoteDetail.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.LisDeliveryNoteDetail
{
    public partial class UCLisDeliveryNoteDetail
    {
        internal PopupMenu menu;
        public enum ProcessType
        {
            DUYET,
            TU_CHOI_DUYET
        }
        private void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager1);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem itemDuyet = new BarButtonItem(barManager1, "Duyệt", 1);
                itemDuyet.Tag = ProcessType.DUYET;
                itemDuyet.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                menu.AddItems(new BarButtonItem[] { itemDuyet });

                BarButtonItem itemTuChoiDuyet = new BarButtonItem(barManager1, "Từ chối duyệt", 2);
                itemTuChoiDuyet.Tag = ProcessType.TU_CHOI_DUYET;
                itemTuChoiDuyet.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                menu.AddItems(new BarButtonItem[] { itemTuChoiDuyet });

                menu.ShowPopup(System.Windows.Forms.Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setExecuteProcessMenu(object sender, ItemClickEventArgs e)
        {
            try
            {
                List<SampleADO> sampleSelecteds = GetSampleSelected();
                if (sampleSelecteds == null || sampleSelecteds.Count == 0)
                    throw new Exception("Khong co mau nao duoc chon!");

                var btn = e.Item as BarButtonItem;
                ProcessType processType = (ProcessType)btn.Tag;
                ProcessInitDataAndExecuteMenu(sampleSelecteds, processType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<SampleADO> GetSampleSelected()
        {
            List<SampleADO> result = new List<SampleADO>();
            try
            {
                if (this.currentListSample == null)
                    return null;
                result = this.currentListSample.Where(o => o.IsChecked).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessInitDataAndExecuteMenu(List<SampleADO> sampleSelecteds, ProcessType processType)
        {
            try
            {
                List<long> listSampleID = sampleSelecteds.Select(o => o.ID).ToList();
                if (processType == ProcessType.DUYET)
                {
                    if (IsAllSample_DaLayMau(sampleSelecteds))
                    {
                        ProcessApproveList(listSampleID);
                    }
                    else
                    {
                        MessageBox.Show("Chỉ cho phép duyệt nhận với các dòng đang ở trạng thái đã lấy mẫu!", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                }
                else if (processType == ProcessType.TU_CHOI_DUYET)
                {
                    if (IsAllSample_DaLayMau(sampleSelecteds))
                    {
                        frmRejectReason frmRejectReason = new frmRejectReason(moduleData, ProcessRejectList, listSampleID);
                        frmRejectReason.ShowDialog();
                        //ProcessRejectList(listSampleID, rejectReason);
                    }
                    else
                    {
                        MessageBox.Show("Chỉ cho phép từ chối duyệt nhận với các dòng đang ở trạng thái đã lấy mẫu!", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsAllSample_DaLayMau(List<SampleADO> listSample)
        {
            bool result = false;
            try
            {
                if (listSample == null)
                    return false;

                foreach (var item in listSample)
                {
                    if (item.SAMPLE_STT_ID != 2)
                    {
                        return false;
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

    }
}
