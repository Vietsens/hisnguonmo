using EMR.SDO;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.WebApiClient;
using HIS.Desktop.ApiConsumer;
using System.IO;
using Inventec.Desktop.Common.Message;
using System.Configuration;
using MOS.KskSignData;
using MOS.EFMODEL.DataModels;


namespace HIS.Desktop.Plugins.InfantInformationList
{
    public partial class frmSettingSign : Form
    {

        private bool isNotLoadWhileChangeControlStateInFirst = false;

        public frmSettingSign()
        {
            InitializeComponent();
        }
       
        private void frmSettingSign_Load(object sender, EventArgs e)
        {
            this.chkHsm.Checked = true;
            cboSignSys.Properties.DataSource = fillComboBoxSignSys();
            cboSignSys.Properties.ValueMember = "Ma";
            cboSignSys.Properties.DisplayMember = "Ten";

            var view = cboSignSys.Properties.View;
            view.Columns.Clear();
            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "Ma", Caption = "Mã", Visible = true });
            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "Ten", Caption = "Tên", Visible = true });
            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "MoTa", Caption = "Mô tả", Visible = true });


            txtPassWord.Properties.UseSystemPasswordChar = true;

           
            
        }
      

        private List<SignSysModel> fillComboBoxSignSys()
        {
            try
            {
                var signSysList = new List<SignSysModel>
                {
                    new SignSysModel { Ma = 1, Ten = "BKAV", MoTa = "Sử dụng HSM của BKAV" },
                    new SignSysModel { Ma = 2, Ten = "VIETRAD", MoTa = "Sử dụng HSM của VIETRAD" },
                    new SignSysModel { Ma = 3, Ten = "VIETSENS", MoTa = "Sử dụng HSM của VIETSENS" },
                    new SignSysModel { Ma = 4, Ten = "EasySign v5.0", MoTa = "Sử dụng HSM EasySign v5.0 của SoftDream" },
                    new SignSysModel { Ma = 5, Ten = "eSignCloud", MoTa = "Sử dụng HSM eSignCloud của Mobile ID" },
                    new SignSysModel { Ma = 6, Ten = "VIN-HSM", MoTa = "Sử dụng HSM của VIN-HSM" },
                    new SignSysModel { Ma = 7, Ten = "VNPT SmartCA", MoTa = "Ký sử dụng HSM theo giải pháp VNPT SmartCA. Nếu chọn giá trị này, cần khai báo các thông tin sau trong danh mục “Người ký”: tài khoản ký vào trường “Mã người ký”, mật khẩu vào trường “Mật khẩu”, mã bí mật sinh OTP vào trường “Mã xác thực cấp 2”" },
                    new SignSysModel { Ma = 8, Ten = "VGCA-Ban Cơ yếu", MoTa = "Sử dụng HSM của VGCA-Ban Cơ yếu Chính phủ. Nếu chọn giá trị này, cần khai báo các thông tin sau trong danh mục “Người ký”: tài khoản ký vào trường “Mã người ký”, mật khẩu vào trường “Mật khẩu”" },
                    new SignSysModel { Ma = 9, Ten = "CMC CA", MoTa = "Sử dụng HSM của CMC CA" },
                    new SignSysModel { Ma = 10, Ten = "Viettel", MoTa = "Sử dụng HSM của Viettel. Nếu chọn giá trị này cần khai báo thông tin sau trong danh mục “Người ký”: tài khoản ký vào trường “Mã người ký”" },
                    new SignSysModel { Ma = 11, Ten = "IntrustCA", MoTa = "Sử dụng HSM của IntrustCA. Nếu chọn giá trị này cần khai báo thông tin sau trong danh mục “Người ký”: tài khoản ký vào trường “Mã người ký”, mật khẩu vào trường “Mật khẩu”, mã pin vào trường “Mã xác thực cấp 2”" }
                };
                return signSysList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<SignSysModel>();
            }
        }

        private void chkUsb_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkHsm.Checked)
                {
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void chkHsm_CheckedChanged(object sender, EventArgs e)
        {


            try
            {
                if (chkUsb.Checked)
                {
                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnCerfi_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_EMR_VIEWER.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_EMR_VIEWER";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog1.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void iconEye_Click(object sender, EventArgs e)
        {


            txtPassWord.Properties.UseSystemPasswordChar = !txtPassWord.Properties.UseSystemPasswordChar;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string signedXmlBase64 = null;

                if (chkHsm.Checked || chkUsb.Checked)
                {
                    string tagStoreSignatureValue = "CHUKYDONVI";

                    if (chkUsb.Checked)
                    {
                        // USB signing logic here (if applicable)
                    }
                    else if (chkHsm.Checked)
                    {
                        SignXmlBhytSDO signInput = new SignXmlBhytSDO();
                       
                        signInput.XmlBase64 = new KskDataProcess().Base64XmlGCS;
                        signInput.TagStoreSignatureValue = tagStoreSignatureValue;
                        signInput.ConfigData.IdentityNumber = txtCCCD.Text;
                        signInput.ConfigData.HsmSerialNumber = txtSerialCerti.Text;
                        signInput.ConfigData.HsmType = long.Parse(cboSignSys.Text);
                        signInput.ConfigData.HsmUserCode = txtSignerCode.Text;
                        signInput.ConfigData.Password = txtPassWord.Text;
                        signInput.ConfigData.SecretKey = txtAuthCode.Text;

                        CommonParam param = new CommonParam();

                        signedXmlBase64 = new BackendAdapter(param)
                            .Post<string>("api/EmrSign/SignXmlBhyt", ApiConsumers.EmrConsumer, signInput, param);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class SignSysModel
    {
        public int Ma { get; set; } 
        public string Ten { get; set; } 
        public string MoTa { get; set; }
    }
}
