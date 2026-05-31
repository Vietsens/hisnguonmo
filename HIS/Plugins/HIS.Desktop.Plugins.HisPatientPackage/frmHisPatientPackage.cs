/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    /// <summary>
    /// Cửa sổ màn 6.2 Danh sách gói — bọc UserControl danh sách vào 1 Form (FormBase).
    /// FormBase tự gán icon ứng dụng + tiêu đề (từ Module.text) trong constructor.
    /// </summary>
    public partial class frmHisPatientPackage : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private UcHisPatientPackage ucList;

        public frmHisPatientPackage(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.moduleData = module;
        }

        private void frmHisPatientPackage_Load(object sender, EventArgs e)
        {
            try
            {
                // FormBase đã set Text từ Module.text; nếu rỗng (mở không có module) -> đặt mặc định.
                if (string.IsNullOrEmpty(this.Text))
                    this.Text = "Danh sách gói dịch vụ bệnh nhân";

                ucList = new UcHisPatientPackage(moduleData);
                ucList.Dock = DockStyle.Fill;
                this.Controls.Add(ucList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
