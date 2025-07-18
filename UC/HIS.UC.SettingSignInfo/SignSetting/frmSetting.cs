using HIS.UC.SettingSignInfo.ADO;
using DevExpress.XtraEditors;
using EMR.WCF.DCO;
using HIS.Desktop.LocalStorage.EmrConfig;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.SignLibrary.ServiceSign;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using HIS.Desktop.LocalStorage.BackendData;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ADO;

namespace HIS.UC.SettingSignInfo
{
    public partial class frmSetting : Form
    {
        string SerialNumber { get; set; }
        SettingSignADO CurrentSetting { get; set; }
        HIS.Desktop.Common.DelegateSelectData delegateSelectData { get; set; }
        bool IsActionSave = false;
        public frmSetting(SettingSignADO ado, HIS.Desktop.Common.DelegateSelectData delegateSelectData)
        {
            InitializeComponent();
            try
            {
                CurrentSetting = ado;
                this.delegateSelectData = delegateSelectData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void StateChanged(object sender, EventArgs e)
        {
            try
            {
                SetVisibleControl(chkHsm.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetVisibleControl(bool IsHsm)
        {
            try
            {
                layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (!IsHsm)
                {
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {

                    layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always; layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataCboSystem();
                LoadValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboSystem()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ID", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("NAME", "Tên", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, true, 350);
                var EmrConfig = BackendDataWorker.Get<EMR_CONFIG>().FirstOrDefault(o => o.KEY == "EMR.HSM.INTEGRATE_OPTION");
                if (EmrConfig != null)
                {
                    ControlEditorLoader.Load(this.cboSystem, ParseHsmConfigs(EmrConfig.DESCRIPTION), controlEditorADO);
                    this.cboSystem.Properties.ImmediatePopup = true; this.cboSystem.Properties.PopupFormMinSize = new Size(350, 0);
                    this.cboSystem.EditValue = !string.IsNullOrEmpty(EmrConfig.VALUE) ? Int64.Parse(EmrConfig.VALUE) : 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public static List<HsmConfigADO> ParseHsmConfigs(string input)
        {
            var result = new List<HsmConfigADO>();
            try
            {
                if (string.IsNullOrEmpty(input))
                    return result;
                var regex = new System.Text.RegularExpressions.Regex(
                    @"- (\d+):\s*([^\n\.]+)\.?\s*(.*?)(?=(?:\n- \d+:)|\z)",
                    System.Text.RegularExpressions.RegexOptions.Singleline);

                var matches = regex.Matches(input);
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    int id = int.Parse(match.Groups[1].Value.Trim());
                    string name = match.Groups[2].Value.Trim();
                    string desc = match.Groups[3].Value.Trim();
                    result.Add(new HsmConfigADO
                    {
                        ID = id,
                        NAME = name,
                        DESCRIPTION = desc
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadValue()
        {
            try
            {

                chkHsm.Checked = true;
                if (CurrentSetting != null)
                {
                    chkHsm.Checked = CurrentSetting.IsHsm;
                    chkUsb.Checked = !CurrentSetting.IsHsm;
                    if (CurrentSetting.Id > 0)
                        cboSystem.EditValue = CurrentSetting.Id;
                    txtSignerCode.Text = CurrentSetting.Name;
                    txtPassword.Text = CurrentSetting.Password;
                    txtSerial.Text =  chkHsm.Checked ? CurrentSetting.SerialNumber : null;
                    txtSecretKey.Text = CurrentSetting.SercetKey;
                    txtCccd.Text = CurrentSetting.CccdNumber;
                    lblSerial.Text = CurrentSetting.SerialNumberInfo;
                }
                else
                {
                    CurrentSetting = new SettingSignADO();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnViewPw_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.Properties.PasswordChar = new char();
            txtPassword.Properties.UseSystemPasswordChar = false;
        }

        private void btnViewPw_MouseUp(object sender, MouseEventArgs e)
        {

            txtPassword.Properties.PasswordChar = '*';
            txtPassword.Properties.UseSystemPasswordChar = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                SettingSignADO ado = new SettingSignADO
                {
                    IsHsm = chkHsm.Checked,
                    Id = cboSystem.EditValue != null ? Convert.ToInt64(cboSystem.EditValue) : 0,
                    Name = txtSignerCode.Text.Trim(),
                    Password = txtPassword.Text.Trim(),
                    SerialNumber = chkHsm.Checked ? txtSerial.Text.Trim() : SerialNumber,
                    SercetKey = txtSecretKey.Text.Trim(),
                    CccdNumber = txtCccd.Text.Trim(),
                    SerialNumberInfo = chkHsm.Checked ? null : lblSerial.Text.Trim()
                };
                if (delegateSelectData != null)
                    delegateSelectData(ado);
                IsActionSave = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnChooseSer_Click(object sender, EventArgs e)
        {

            try
            {
                if (VerifyServiceSignProcessorIsRunning())
                {
                    WcfSignDCO wcfSignDCO = new WcfSignDCO();
                    wcfSignDCO.HwndParent = this.Handle;
                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();
                    var wcfSignResultDCO = signProcessorClient.GetSerialNumber(jsonData);  //EDIT
                    if (wcfSignResultDCO != null)
                    {
                        SerialNumber = wcfSignResultDCO.OutputFile;
                    }
                }
                if (string.IsNullOrEmpty(SerialNumber))
                {
                    XtraMessageBox.Show("Không lấy được chứng thư hoặc chứng thư không hợp lệ", "Thông báo");
                }
                else
                {
                    var certificate = Inventec.Common.SignFile.CertUtil.GetBySerial(SerialNumber, requirePrivateKey: true, validOnly: false);
                    CurrentSetting.SerialNumber = SerialNumber;
                    lblSerial.Text = GetCertificateInfo(certificate);
                    CurrentSetting.SerialNumberInfo = lblSerial.Text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        public string GetCertificateInfo(X509Certificate2 cert)
        {
            if (cert == null) return "Certificate is null";
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Subject: {0}", cert.Subject));
            sb.AppendLine(string.Format("Issuer: {0}",cert.Issuer));
            sb.AppendLine(string.Format("Serial Number: {0}",cert.SerialNumber));
            sb.AppendLine(string.Format("Valid From: {0}",cert.NotBefore));
            sb.AppendLine(string.Format("Valid To: {0}",cert.NotAfter));
            sb.AppendLine(string.Format("Thumbprint: {0}",cert.Thumbprint));
            sb.AppendLine(string.Format("Has Private Key: {0}",cert.HasPrivateKey));
            return sb.ToString();
        }

        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {
                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }

        private void frmSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (!IsActionSave)
                    delegateSelectData(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
