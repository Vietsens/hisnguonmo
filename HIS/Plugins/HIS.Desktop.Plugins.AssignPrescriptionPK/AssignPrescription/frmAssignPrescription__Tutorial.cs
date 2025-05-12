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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal void RebuildTutorialWithInControlContainer(object data)
        {
            try
            {
                gridViewTutorial.OptionsView.ShowColumnHeaders = false;
                gridViewTutorial.BeginUpdate();
                gridViewTutorial.Columns.Clear();
                popupControlContainerTutorial.Width = 550;
                popupControlContainerTutorial.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "TUTORIAL";
                col1.Caption = "Hướng dẫn sử dụng";
                col1.Width = 400;
                col1.VisibleIndex = 1;
                gridViewTutorial.Columns.Add(col1);

                gridViewTutorial.GridControl.DataSource = data;
                gridViewTutorial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Tutorial_RowClick(object data)
        {
            try
            {
                HIS_MEDICINE_TYPE_TUT medicineTypeTut = data as HIS_MEDICINE_TYPE_TUT;
                if (medicineTypeTut != null)
                {
                    //Nếu hướng dẫn sử dụng mẫu có đường dùng thì lấy ra
                    if (medicineTypeTut.MEDICINE_USE_FORM_ID > 0)
                    {
                        this.cboMedicineUseForm.EditValue = medicineTypeTut.MEDICINE_USE_FORM_ID;
                    }
                    //Nếu không có đường dùng thì lấy đường dùng từ danh mục loại thuốc
                    else
                    {
                        var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeTut.MEDICINE_TYPE_ID);
                        if (medicineType != null && (medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                        {
                            this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                        }
                    }
                    this.cboHtu.Text = null;
                    if (DataHtuList != null && DataHtuList.Count > 0)
                    {
                        DataHtuList.ForEach(o => o.IsChecked = o.ID == medicineTypeTut.HTU_ID);
                        this.cboHtu.Text = string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME));
                    }
                    if (medicineTypeTut.HTU_ID != null)
                        this.cboHtu.Properties.Buttons[1].Visible = true;
                    else
                        this.cboHtu.Properties.Buttons[1].Visible = false;

                    if (this.spinSoNgay.Value < (medicineTypeTut.DAY_COUNT ?? 0))
                        this.spinSoNgay.EditValue = medicineTypeTut.DAY_COUNT;
                    this.spinSoLuongNgay.EditValue = medicineTypeTut.DAY_COUNT;

                    Inventec.Common.Logging.LogSystem.Debug("Truong hop co HDSD thuoc theo tai khoan cua loai thuoc (HIS_MEDICINE_TYPE_TUT)--> lay truong DAY_COUNT gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeTut), medicineTypeTut));

                    this.spinSang.EditValue = medicineTypeTut.MORNING;
                    this.spinTrua.EditValue = medicineTypeTut.NOON;
                    this.spinChieu.EditValue = medicineTypeTut.AFTERNOON;
                    this.spinToi.EditValue = medicineTypeTut.EVENING;

                    //Nếu có trường hướng dẫn thì sử dụng luôn
                    if (!String.IsNullOrEmpty(medicineTypeTut.TUTORIAL))
                    {
                        this.txtTutorial.Text = medicineTypeTut.TUTORIAL;
                    }

                    if (!String.IsNullOrEmpty(medicineTypeTut.HTU_TEXT))
                    {
                        this.memHtu.Text = medicineTypeTut.HTU_TEXT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildHtuWithInControlContainer(object data)
        {
            try
            {
                gridViewHtuText.OptionsView.ShowColumnHeaders = false;
                gridViewHtuText.BeginUpdate();
                gridViewHtuText.Columns.Clear();
                popupControlContainerHtuText.Width = 550;
                popupControlContainerHtuText.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "HTU_TEXT";
                col2.Caption = "Cách dùng";
                col2.Width = 400;
                col2.VisibleIndex = 1;
                gridViewHtuText.Columns.Add(col2);

                gridViewHtuText.GridControl.DataSource = data;

                gridViewHtuText.OptionsBehavior.Editable = false;
                gridViewHtuText.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HtuText_RowClick(object data)
        {
            try
            {
                HIS_MEDICINE_TYPE_TUT medicineTypeTut = data as HIS_MEDICINE_TYPE_TUT;
                if (medicineTypeTut != null)
                {
                    //Nếu hướng dẫn sử dụng mẫu có đường dùng thì lấy ra
                    if (medicineTypeTut.MEDICINE_USE_FORM_ID > 0)
                    {
                        this.cboMedicineUseForm.EditValue = medicineTypeTut.MEDICINE_USE_FORM_ID;
                    }
                    //Nếu không có đường dùng thì lấy đường dùng từ danh mục loại thuốc
                    else
                    {
                        var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeTut.MEDICINE_TYPE_ID);
                        if (medicineType != null && (medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                        {
                            this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                        }
                    }
                    this.cboHtu.Text = null;
                    if (DataHtuList != null && DataHtuList.Count > 0)
                    {
                        DataHtuList.ForEach(o => o.IsChecked = o.ID == medicineTypeTut.HTU_ID);
                        this.cboHtu.Text = string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME));
                    }
                    if (medicineTypeTut.HTU_ID != null)
                        this.cboHtu.Properties.Buttons[1].Visible = true;       
                    else
                        this.cboHtu.Properties.Buttons[1].Visible = false;

                    if (this.spinSoNgay.Value < (medicineTypeTut.DAY_COUNT ?? 0))
                        this.spinSoNgay.EditValue = medicineTypeTut.DAY_COUNT;
                    this.spinSoLuongNgay.EditValue = medicineTypeTut.DAY_COUNT;

                    Inventec.Common.Logging.LogSystem.Debug("Truong hop co HDSD thuoc theo tai khoan cua loai thuoc (HIS_MEDICINE_TYPE_TUT)--> lay truong DAY_COUNT gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeTut), medicineTypeTut));

                    this.spinSang.EditValue = medicineTypeTut.MORNING;
                    this.spinTrua.EditValue = medicineTypeTut.NOON;
                    this.spinChieu.EditValue = medicineTypeTut.AFTERNOON;
                    this.spinToi.EditValue = medicineTypeTut.EVENING;

                    //Nếu có trường hướng dẫn thì sử dụng luôn
                    if (!String.IsNullOrEmpty(medicineTypeTut.TUTORIAL))
                    {
                        this.txtTutorial.Text = medicineTypeTut.TUTORIAL;
                    }

                    if (!String.IsNullOrEmpty(medicineTypeTut.HTU_TEXT))
                    {
                        this.memHtu.Text = medicineTypeTut.HTU_TEXT;
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
