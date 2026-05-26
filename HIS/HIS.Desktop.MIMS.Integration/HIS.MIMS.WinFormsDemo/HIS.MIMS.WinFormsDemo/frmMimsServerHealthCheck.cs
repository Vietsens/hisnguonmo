using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Modules;

namespace HIS.MIMS.WinFormsDemo
{
    /// <summary>
    /// MIMS Server Health Check — test trực tiếp 4 endpoint MIMS API.
    /// Form này KHÔNG dùng BaseService.MappingMIMS / ExtractAtcCodes nên không cần HIS backend.
    /// </summary>
    public partial class frmMimsServerHealthCheck : Form
    {
        // ATC codes for VN Contraindication test (Ceftriaxon + Ringer Lactat — known interaction per Postman)
        private static readonly List<string> VnTestAtcCodes = new List<string> { "J01DD04", "B05BB01" };

        // MIMS GUIDs sample (từ Form1 hiện tại — đã verify gọi được CDS API)
        private const string TestGenericGuid1 = "BF5DDF41-AEDC-2324-E034-0003BA299378";
        private const string TestGenericGuid2 = "AB1E57B8-E83E-443D-8526-905FD7E5C47D";
        private const string TestGgpiGuid = "488F9F61-5D37-4989-925E-1742FFFDAA9E";

        private static readonly List<string> TestIcd10 = new List<string> { "I10" };

        public frmMimsServerHealthCheck()
        {
            InitializeComponent();
        }

        private void frmMimsServerHealthCheck_Load(object sender, EventArgs e)
        {
            try
            {
                txtCdsUrl.Text = MimsConfig.CdsApiUrl;
                txtVnUrl.Text = MimsConfig.VnContraApiUrl;
                lblStatus.Text = string.Format("Sẵn sàng. Cache: HIS_ATC={0}, V_HIS_MEDICINE_TYPE={1}",
                    MimsDemoCacheLoader.CountAtc(), MimsDemoCacheLoader.CountMedicineType());
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Lỗi đọc App.config: " + ex.Message;
            }
        }

        private void btnTestDrugDrugByCode_Click(object sender, EventArgs e)
        {
            // End-to-end test: DrugItem CHỈ có MEDICINE_TYPE_CODE.
            // Service.MappingMIMS() phải lookup CODE → ATC_CODES → HIS_ATC.MIMS_GUID → call API.
            // 2 MEDICINE_TYPE_CODE từ HIS_MEDICINE_TYPE.csv có ATC_CODES set:
            //   "thuocatc1" → ATC J01DD04 (Ceftriaxon) → MIMS GUID AB1E57B8-...
            //   "thuocatc2" → ATC B05BB01 (Ringer Lactat) → MIMS GUID BF5DDF41-...
            try
            {
                Cursor = Cursors.WaitCursor;
                var drugs = new List<DrugItem>
                {
                    new DrugItem("thuocatc1"),
                    new DrugItem("thuocatc2")
                };
                var previous = new List<DrugItem>();
                //bool result = new DrugDrugInteractionService().ShowDialog(drugs, previous);
                bool result = new DrugHealthService().CheckAndAlert(drugs,null);
                lblStatus.ForeColor = System.Drawing.Color.Black;
                lblStatus.Text = string.Format("Drug-Drug by CODE: ShowDialog returned {0}", result);
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "EXCEPTION: " + ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnTestVnContra_Click(object sender, EventArgs e)
        {
            RunTest("VN Contraindication", () =>
            {
                string xml = MimsRequestBuilder.BuildVnContraindicationRequest(VnTestAtcCodes);
                return CallApi(MimsConfig.VnContraApiUrl, xml);
            });
        }

        private void btnTestCdsDrugDrug_Click(object sender, EventArgs e)
        {
            RunTest("CDS Drug-Drug", () =>
            {
                var current = new List<DrugItem>
                {
                    new DrugItem(null, "", TestGenericGuid1, MimsType.GenericItem),
                    new DrugItem(null, "", TestGenericGuid2, MimsType.GenericItem)
                };
                string xml = MimsRequestBuilder.BuildDrugDrugInteractionRequest(current, new List<DrugItem>(), false);
                return CallApi(MimsConfig.CdsApiUrl, xml);
            });
        }

        private void btnTestCdsDrugHealth_Click(object sender, EventArgs e)
        {
            RunTest("CDS Drug-Health", () =>
            {
                var drugs = new List<DrugItem>
                {
                    new DrugItem(null, "", TestGgpiGuid, MimsType.GGPI)
                };
                string xml = MimsRequestBuilder.BuildDrugHealthAlertRequest(drugs, null, TestIcd10, false, false);
                return CallApi(MimsConfig.CdsApiUrl, xml);
            });
        }

        private void btnTestDrugInfo_Click(object sender, EventArgs e)
        {
            RunTest("Drug Information", () =>
            {
                var drug = new DrugItem(null, "", TestGgpiGuid, MimsType.GGPI);
                string xml = MimsRequestBuilder.BuildDrugInformationRequest(drug);
                return CallApi(MimsConfig.CdsApiUrl, xml);
            });
        }

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            var report = new StringBuilder();
            int passCount = 0;
            int totalCount = 0;
            try
            {
                Cursor = Cursors.WaitCursor;

                totalCount++;
                passCount += AppendSummary(report, "VN Contraindication", MimsConfig.VnContraApiUrl,
                    () => MimsRequestBuilder.BuildVnContraindicationRequest(VnTestAtcCodes))
                    ? 1 : 0;

                totalCount++;
                passCount += AppendSummary(report, "CDS Drug-Drug", MimsConfig.CdsApiUrl, () =>
                {
                    var current = new List<DrugItem>
                    {
                        new DrugItem(null, "", TestGenericGuid1, MimsType.GenericItem),
                        new DrugItem(null, "", TestGenericGuid2, MimsType.GenericItem)
                    };
                    return MimsRequestBuilder.BuildDrugDrugInteractionRequest(current, new List<DrugItem>(), false);
                }) ? 1 : 0;

                totalCount++;
                passCount += AppendSummary(report, "CDS Drug-Health", MimsConfig.CdsApiUrl, () =>
                {
                    var drugs = new List<DrugItem> { new DrugItem(null, "", TestGgpiGuid, MimsType.GGPI) };
                    return MimsRequestBuilder.BuildDrugHealthAlertRequest(drugs, null, TestIcd10, false, false);
                }) ? 1 : 0;

                totalCount++;
                passCount += AppendSummary(report, "Drug Information", MimsConfig.CdsApiUrl,
                    () => MimsRequestBuilder.BuildDrugInformationRequest(
                        new DrugItem(null, "", TestGgpiGuid, MimsType.GGPI)))
                    ? 1 : 0;
            }
            catch (Exception ex)
            {
                report.AppendLine("EXCEPTION: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            txtRequest.Text = string.Empty;
            txtResponse.Text = report.ToString();
            lblStatus.ForeColor = (passCount == totalCount)
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red;
            lblStatus.Text = string.Format("Run All: {0}/{1} PASS", passCount, totalCount);
        }

        private bool AppendSummary(StringBuilder report, string name, string url, Func<string> buildRequest)
        {
            try
            {
                string xml = buildRequest();
                TestResult r = CallApi(url, xml);
                report.AppendLine(string.Format("[{0}] {1} ({2}ms, length={3}, timeout={4}, error={5})",
                    name, r.Pass ? "PASS" : "FAIL", r.ElapsedMs, r.ResponseXml.Length, r.IsTimeout, r.IsError));
                return r.Pass;
            }
            catch (Exception ex)
            {
                report.AppendLine(string.Format("[{0}] EXCEPTION: {1}", name, ex.Message));
                return false;
            }
        }

        private TestResult CallApi(string url, string xmlRequest)
        {
            var sw = Stopwatch.StartNew();
            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(url, xmlRequest, out isTimeout);
            sw.Stop();

            string trimmed = xmlResponse == null ? string.Empty : xmlResponse.TrimStart();
            bool isError = !string.IsNullOrEmpty(trimmed)
                && trimmed.StartsWith("<Error", StringComparison.OrdinalIgnoreCase);

            return new TestResult
            {
                RequestXml = xmlRequest,
                ResponseXml = xmlResponse ?? string.Empty,
                IsTimeout = isTimeout,
                IsError = isError,
                ElapsedMs = sw.ElapsedMilliseconds,
                Pass = !isTimeout && !isError && !string.IsNullOrEmpty(xmlResponse)
            };
        }

        private void RunTest(string name, Func<TestResult> action)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                TestResult r = action();
                txtRequest.Text = r.RequestXml;
                txtResponse.Text = r.ResponseXml;
                lblStatus.ForeColor = r.Pass ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                lblStatus.Text = string.Format("[{0}] {1} — timeout={2}, error={3}, length={4}, took={5}ms",
                    name, r.Pass ? "PASS" : "FAIL", r.IsTimeout, r.IsError, r.ResponseXml.Length, r.ElapsedMs);
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "[" + name + "] EXCEPTION: " + ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private class TestResult
        {
            public string RequestXml { get; set; }
            public string ResponseXml { get; set; }
            public bool IsTimeout { get; set; }
            public bool IsError { get; set; }
            public long ElapsedMs { get; set; }
            public bool Pass { get; set; }
        }
    }
}
