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
       
namespace HIS.Desktop.Plugins.InfantInformationList
{
    public partial class frmSettingSign : Form
    {
       
        private bool isNotLoadWhileChangeControlStateInFirst = false;

        public frmSettingSign()
        {
            InitializeComponent();
        }

        private void chkUsb_CheckedChanged(object sender, EventArgs e)
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

        private void chkHsm_CheckedChanged(object sender, EventArgs e)
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

        private void SyncAndSignXml(string xmlBase64)
        {
           
            if (chkHsm.Checked || chkUsb.Checked)
            {
                string tagStoreSignatureValue = "CHUKYDONVI"; 

                if (chkUsb.Checked)
                {
                     
                }
                else if (chkHsm.Checked)
                {
                  
                    SignXmlBhytSDO signInput = new SignXmlBhytSDO();
                    signInput.XmlBase64 = xmlBase64;
                    signInput.TagStoreSignatureValue = tagStoreSignatureValue;
                    signInput.ConfigData.IdentityNumber = txtCCCD.Text;
                    signInput.ConfigData.HsmSerialNumber = txtSerialCerti.Text;
                    signInput.ConfigData.HsmType = long.Parse(txtSignSys.Text);
                    signInput.ConfigData.HsmUserCode = txtSignerCode.Text;
                    signInput.ConfigData.Password = txtPassWord.Text;
                    signInput.ConfigData.SecretKey = txtAuthCode.Text;

                    CommonParam param = new CommonParam();

                    string signedXmlBase64 = new BackendAdapter(param)
                        .Post<string>("api/EmrSign/SignXmlBhyt", ApiConsumers.EmrConsumer, signInput, param);

                }
            }
        }

        private void panelUsb_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
              
                if (string.IsNullOrWhiteSpace(txtSignSys.Text) ||
                    string.IsNullOrWhiteSpace(txtSignerCode.Text) ||
                    string.IsNullOrWhiteSpace(txtPassWord.Text) ||
                    string.IsNullOrWhiteSpace(txtAuthCode.Text) ||
                    string.IsNullOrWhiteSpace(txtCCCD.Text) ||
                    string.IsNullOrWhiteSpace(txtSerialCerti.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tất cả thông tin cấu hình ký.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                var configData = new SignConfigSDO
                {
                    HsmType = long.Parse(txtSignSys.Text),
                    HsmUserCode = txtSignerCode.Text,

                    Password = txtPassWord.Text,
                    SecretKey = txtAuthCode.Text,
                    IdentityNumber = txtCCCD.Text,
                    HsmSerialNumber = txtSerialCerti.Text
                };

                var param = new CommonParam();
                var result = new BackendAdapter(param).Post<bool>(
                    "api/EmrSign/SaveSignConfig",
                    ApiConsumers.EmrConsumer,
                    configData,
                    param
                );

                if (result)
                    MessageBox.Show("Lưu cấu hình thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Lưu cấu hình thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Lỗi xảy ra khi lưu cấu hình: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
