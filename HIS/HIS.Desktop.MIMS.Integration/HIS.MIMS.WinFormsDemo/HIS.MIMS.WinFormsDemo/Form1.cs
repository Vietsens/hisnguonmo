using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Modules;
using HIS.Desktop.MIMS.Integration.View;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;

namespace HIS.MIMS.WinFormsDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Suppress script error dialogs from embedded browser
            //webBrowser1.ScriptErrorsSuppressed = true;
        }

        private void ShowHtml(string html, string errorMessage)
        {
            if (!string.IsNullOrEmpty(html))
            {
                webBrowser1.DocumentText = html;
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "MIMS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 1. Use sample "Drug Information (Product)" from Postman collection
        private void btnTestDrugInfoProduct_Click(object sender, EventArgs e)
        {
            DrugItem drug = new DrugItem(null, "allopurinol Oral Tablet", "B3E75967-D489-5A28-E034-080020E1DD8C", MimsType.GenericItem);

            var service = new DrugInfomationService();
            service.ShowResultAsync(drug);

            // const string xml = "<Request><Content><Product reference=\"{D2E2D654-E6A0-4E8D-82B3-CBAE854F6F60}\" /></Content></Request>";

            // // Call CDS endpoint directly and transform XML -> HTML via XSL
            // string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xml);
            // string html = MimsResponseTransformer.XmlToHtml(xmlResponse);
            // WebViewHelper.ShowHtml(html, "Thông tin thuốc");

            //ShowHtml(html, string.IsNullOrEmpty(xmlResponse) ? "No response from MIMS API" : null);
        }

        // 2. Use sample "Drug Information (GGPI)" from Postman collection
        private void btnTestDrugInfoGGPI_Click(object sender, EventArgs e)
        {
            const string xml = "<Request><Content><GGPI reference=\"{488F9F61-5D37-4989-925E-1742FFFDAA9E}\"/></Content></Request>";

            string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xml);
            string html = MimsResponseTransformer.XmlToHtml(xmlResponse);
            WebViewHelper.ShowHtml(html, "Drug Information (GGPI)");
            //ShowHtml(html, string.IsNullOrEmpty(xmlResponse) ? "No response from MIMS API" : null);

        }

        // 3. Use integration service for CDS Drug-Drug interaction (async, không block UI)
        private void btnTestCdsInteraction_Click(object sender, EventArgs e)
        {
            var current = new List<DrugItem>
            {
                //new DrugItem(null, "Vercef dispersible tab 125 mg", "D2E2D654-E6A0-4E8D-82B3-CBAE854F6F60", MimsDrugType.Product),
                new DrugItem(null, "", "23F5183C-80AB-4E84-9F6A-11FEAD7BFA65", MimsType.GGPI),
                new DrugItem(null, "", "F0FDC06F-E5AD-4A26-8899-D71C800959FB", MimsType.GenericItem)
            };

            var previous = new List<DrugItem>();

            var service = new DrugDrugInteractionService();
            //var service = new DuplicateDrugService();
            service.ShowResultAsync(current, previous);
        }

        // 4. Use integration service for VN Contraindication sample (async, using HIS codes that map to above products)
        private void btnTestVnContra_Click(object sender, EventArgs e)
        {
            var hisCodes = new List<string>
            {
                "12472",
                "12899"
            };

            var service = new VnContraindicationService();
            service.ShowResultAsync(hisCodes);
        }

        // 5. Drug–ICD Alert test using the exact prescriptionquery from Postman collection
        private void btnTestDrugDrugAlert_Click(object sender, EventArgs e)
        {
            var current = new List<DrugItem>
            {
                //new DrugItem(null, "Vercef dispersible tab 125 mg", "D2E2D654-E6A0-4E8D-82B3-CBAE854F6F60", MimsDrugType.Product),
                new DrugItem(null, "", "23F5183C-80AB-4E84-9F6A-11FEAD7BFA65", MimsType.GGPI),
                new DrugItem(null, "", "F0FDC06F-E5AD-4A26-8899-D71C800959FB", MimsType.GenericItem),

                new DrugItem(null, "", "775368B5-254F-4CF0-A605-B64846459BF8", MimsType.Product),
                new DrugItem(null, "hydroCHLOROthiazide 12.5mg - irbesartan 300mg film coated tablet", "49102790-2259-457F-88B5-A968FA397EDA", MimsType.GGPI),

                new DrugItem(null, "", "1F00E3C8-E878-4081-BC07-EDECA6B7E7F6", MimsType.Product),
            };
            var alleries = new List<AllergyItem>
            {
                
                new AllergyItem(null,null,"","3FF1EECF-EDA2-4086-8941-01469BCDA202", MimsType.Product)
            };
            var icdcodes = new List<string>();
            icdcodes.Add("R00.1");

            var service = new DrugHealthService();
            service.CheckAndAlert(current,alleries, icdcodes);
            //var isContinue = service.ShowDialog(current, icdcodes);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var current = new List<DrugItem>
            {
                new DrugItem(null, "", "1F00E3C8-E878-4081-BC07-EDECA6B7E7F6", MimsType.Product),
            };
            var alleries = new List<AllergyItem>
            {
                
                new AllergyItem(null,null,"","3FF1EECF-EDA2-4086-8941-01469BCDA202", MimsType.Product)
            };

            var service = new DrugAllergyService();
            service.ShowResultAsync(current, alleries);
            //var isContinue = service.ShowDialog(current, icdcodes);
        }
    }
}
