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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute.ViewImage
{
    public partial class FormViewImageV2 : FormBase
    {
        private List<ADO.ImageADO> DataImages;
        private Action<List<ADO.ImageADO>> SaveData;

        private DevExpress.XtraBars.BarManager barManager1;
        private PopupMenu _PopupMenu;
        private List<long> ServiceIds;
        List<V_HIS_SERVICE> lstService { get; set; }
        public FormViewImageV2(Inventec.Desktop.Common.Modules.Module module, List<long> serviceIds, List<ADO.ImageADO> listImage, Action<List<ADO.ImageADO>> saveData, List<V_HIS_SERVICE> lstService)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.DataImages = listImage;
                this.SaveData = saveData;
                this.ServiceIds = serviceIds;
                this.lstService = lstService;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormViewImageV2_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();

                if (DataImages == null || DataImages.Count <= 0)
                {
                    this.Close();
                }

                InitPopupBodyPart();

                gridControl1.DataSource = DataImages;
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitPopupBodyPart()
        {
            try
            {
                this.barManager1 = new BarManager();
                this._PopupMenu = new PopupMenu(this.barManager1);

                List<HIS_BODY_PART> listBodyPart = BackendDataWorker.Get<HIS_BODY_PART>().Where(o => o.IS_ACTIVE == 1).ToList();
                if (listBodyPart != null && listBodyPart.Count > 0)
                {
                    List<long> listBodyPartIds = new List<long>();
                    if (this.ServiceIds != null)
                    {
                        List<V_HIS_SERVICE> listService = lstService.Where(o => this.ServiceIds.Contains(o.ID)).ToList();
                        if (listService != null && listService.Count > 0)
                        {
                            foreach (var service in listService)
                            {
                                if (!String.IsNullOrWhiteSpace(service.BODY_PART_IDS))
                                {
                                    string[] ids = service.BODY_PART_IDS.Split(',');
                                    foreach (var id in ids)
                                    {
                                        listBodyPartIds.Add(Inventec.Common.TypeConvert.Parse.ToInt64(id));
                                    }
                                }
                            }
                        }
                    }

                    if (listBodyPartIds != null && listBodyPartIds.Count > 0)
                    {
                        listBodyPart = listBodyPart.Where(o => listBodyPartIds.Contains(o.ID)).ToList();
                    }

                    if (listBodyPart != null && listBodyPart.Count > 0)
                    {
                        listBodyPart = listBodyPart.OrderBy(o => o.BODY_PART_NAME).ToList();
                        foreach (var item in listBodyPart)
                        {
                            BarButtonItem BodyPart = new BarButtonItem(this.barManager1, item.BODY_PART_NAME, -1);
                            BodyPart.Tag = item.ID;
                            BodyPart.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                            _PopupMenu.AddItem(BodyPart);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormViewImageV2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.SaveData != null)
                {
                    gridControl1.RefreshDataSource();
                    this.SaveData(this.DataImages);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var data = (ADO.ImageADO)layoutView1.GetFocusedRow();
                if (data != null)
                {
                    if (e.Column.FieldName == gc_Check.FieldName)
                    {
                        if (data.IsChecked)
                        {
                            List<int> listSTT = this.DataImages.Select(o => o.STTImage ?? 0).Distinct().ToList();
                            listSTT = listSTT != null ? listSTT.OrderBy(o => o).ToList() : listSTT;
                            data.STTImage = 1;

                            if (listSTT != null && listSTT.Count() == 1)
                            {
                                data.STTImage = listSTT.Max() + 1;
                            }
                            else
                            {
                                for (int i = 0; i < listSTT.Count() - 1; i++)
                                {
                                    if (listSTT[i] + 1 != listSTT[i + 1])
                                    {
                                        data.STTImage = listSTT[i] + 1;
                                        break;
                                    }
                                    else
                                        data.STTImage = listSTT.Max() + 1;
                                }
                            }
                        }
                        else
                        {
                            data.STTImage = null;

                            var listImageTemp = this.DataImages.OrderByDescending(o => o.IsChecked).ThenBy(o => o.STTImage ?? 9999999999).ToList();
                            int sttNew = 1;
                            foreach (var imgADO in listImageTemp)
                            {
                                if (imgADO.IsChecked)
                                {
                                    imgADO.STTImage = sttNew;
                                    sttNew += 1;
                                }
                            }
                        }
                    }
                    else if (e.Column.FieldName == gc_Stt.FieldName)
                    {
                        if (!data.IsChecked)
                        {
                            data.STTImage = null;
                        }
                    }
                    else if (e.Column.FieldName == gc_Caption.FieldName)//sửa tên sẽ xóa id chọn
                    {
                        data.BODY_PART_ID = null;
                    }

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                layoutView1.PostEditor();
                //gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemSpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.SpinEdit edit = sender as DevExpress.XtraEditors.SpinEdit;
                var data = (ADO.ImageADO)layoutView1.GetFocusedRow();
                if (edit != null && edit.EditValue != null && data != null && data.IsChecked)
                {
                    ProcessUpdateImageOrder(data, edit.Value);
                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// - Khi người dùng nhập số thứ tự vào thì sẽ cho phép hình ảnh đó được chèn vào stt được nhập đó. Và sắp xếp lại số thứ tự:
        ///VD: i1 = A, i2 = B, i3 = C, i4 = D, i5 = E (i1,2,3,4,5: STT; A,B,C,D,E: Hình ảnh)
        ///TH1: Đổi B có STT = 4 ==> STT sẽ sắp xếp lại như sau: i1 = A, i2 = C, i3 = D, i4 = B, i5 = E (Các STT của C,D sẽ bị đẩy lên 1 đơn vị để B thay thế vào).
        ///TH2: Đổi D có STT = 2 ==> STT sẽ sắp xếp lại như sau: i1 = A, i2 = D, i3 = B, i4 = C, i5 = E (Các STT của B,C sẽ bị đẩy xuống 1 đơn vị để D chèn vào).
        /// </summary>
        /// <param name="sttNumber"></param>
        private void ProcessUpdateImageOrder(ADO.ImageADO currentDataClick, decimal sttNumber)
        {
            try
            {
                if (currentDataClick.STTImage == (int)sttNumber)
                    return;

                bool isChangePlus = currentDataClick.STTImage < (int)sttNumber ? true : false;

                if (this.DataImages != null && this.DataImages.Count > 0)
                {
                    var orderImages = ProcessOrderImage(this.DataImages);

                    var imgDuplicate = orderImages.Where(o => o.IsChecked && o.STTImage == (int)sttNumber).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imgDuplicate), imgDuplicate) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isChangePlus), isChangePlus));
                    int duplicateSttNumber = imgDuplicate != null ? (imgDuplicate.STTImage ?? 0) : 0;
                    int rawSttNumber = currentDataClick != null ? (currentDataClick.STTImage ?? 0) : 0;
                    int numC = 1;
                    foreach (var itemImg in orderImages)
                    {
                        int maxStt = orderImages.Where(o => o.IsChecked).Max(o => o.STTImage ?? 0);
                        if (itemImg.IsChecked)
                        {
                            if (isChangePlus)
                            {
                                if ((int)sttNumber > maxStt)
                                {
                                    if ((itemImg.STTImage ?? 0) > rawSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage - 1;
                                    }
                                    else if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                        itemImg.STTImage = maxStt;
                                }
                                else if (duplicateSttNumber > 0)
                                {
                                    if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                    {
                                        itemImg.STTImage = duplicateSttNumber;
                                    }
                                    else if ((itemImg.STTImage ?? 0) > rawSttNumber && (itemImg.STTImage ?? 0) <= duplicateSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage - 1;
                                    }
                                }
                            }
                            else
                            {
                                if (duplicateSttNumber > 0)
                                {
                                    if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                    {
                                        itemImg.STTImage = duplicateSttNumber;
                                    }
                                    else if ((itemImg.STTImage ?? 0) < rawSttNumber && (itemImg.STTImage ?? 0) >= duplicateSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage + 1;
                                    }
                                }
                            }

                            numC += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<ADO.ImageADO> ProcessOrderImage(List<ADO.ImageADO> images)
        {
            try
            {
                if (images != null && images.Count > 0)
                {
                    List<ADO.ImageADO> listImageTemp = new List<ADO.ImageADO>();
                    var listImageSTTOrder = images.Where(o => o.IsChecked).OrderBy(o => o.STTImage).ToList();
                    var listImageNoSTTOrder = images.Where(o => !o.IsChecked).OrderBy(o => o.FileName).ToList();
                    if (listImageSTTOrder != null && listImageSTTOrder.Count > 0)
                        listImageTemp.AddRange(listImageSTTOrder);
                    if (listImageNoSTTOrder != null && listImageNoSTTOrder.Count > 0)
                        listImageTemp.AddRange(listImageNoSTTOrder);
                    return listImageTemp;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return null;
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (ADO.ImageADO)layoutView1.GetFocusedRow();
                if (data != null)
                {
                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void _MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var data = (ADO.ImageADO)layoutView1.GetFocusedRow();
                if (data != null)
                {
                    if (e.Item is BarButtonItem)
                    {
                        long bodyPartId = (long)(e.Item.Tag);
                        var bodyPart = BackendDataWorker.Get<HIS_BODY_PART>().FirstOrDefault(o => o.ID == bodyPartId);
                        if (bodyPart != null)
                        {
                            data.CAPTION = bodyPart.BODY_PART_NAME;
                            data.BODY_PART_ID = bodyPart.ID;
                        }
                    }

                    gridControl1.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormViewImageV2
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2 = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(FormViewImageV2).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormViewImageV2.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2, LanguageManager.GetCulture());
                this.gc_Image.Caption = Inventec.Common.Resource.Get.Value("FormViewImageV2.gc_Image.Caption", Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2, LanguageManager.GetCulture());
                this.gc_Check.Caption = Inventec.Common.Resource.Get.Value("FormViewImageV2.gc_Check.Caption", Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2, LanguageManager.GetCulture());
                this.gc_Caption.Caption = Inventec.Common.Resource.Get.Value("FormViewImageV2.gc_Caption.Caption", Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2, LanguageManager.GetCulture());
                this.gc_Stt.Caption = Inventec.Common.Resource.Get.Value("FormViewImageV2.gc_Stt.Caption", Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2, LanguageManager.GetCulture());
                this.layoutViewCard1.Text = Inventec.Common.Resource.Get.Value("FormViewImageV2.layoutViewCard1.Text", Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormViewImageV2.Text", Resources.ResourceLanguageManager.LanguageResourceFormViewImageV2, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormViewImageV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                lstService = null;
                ServiceIds = null;
                _PopupMenu = null;
                barManager1 = null;
                SaveData = null;
                DataImages = null;
                this.layoutView1.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.layoutView1_CellValueChanged);
                this.repositoryItemCheckEdit1.CheckedChanged -= new System.EventHandler(this.repositoryItemCheckEdit1_CheckedChanged);
                this.repositoryItemSpinEdit1.EditValueChanged -= new System.EventHandler(this.repositoryItemSpinEdit1_EditValueChanged);
                this.repositoryItemButtonEdit1.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1_ButtonClick);
                this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.FormViewImageV2_FormClosing);
                this.Load -= new System.EventHandler(this.FormViewImageV2_Load);
                gridControl1.DataSource = null;
                repositoryItemButtonEdit1 = null;
                layoutViewCard1 = null;
                layoutViewField_layoutViewColumn1 = null;
                layoutViewField_gridColumnName = null;
                layoutViewField_gridColumnCheck = null;
                layoutViewField_gridColumnImage = null;
                repositoryItemSpinEdit1 = null;
                repositoryItemCheckEdit1 = null;
                repositoryItemPictureEdit1 = null;
                layoutControlItem1 = null;
                gc_Stt = null;
                gc_Caption = null;
                gc_Check = null;
                gc_Image = null;
                layoutView1 = null;
                gridControl1 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
