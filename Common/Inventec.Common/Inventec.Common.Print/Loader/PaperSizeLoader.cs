using DevExpress.XtraGrid.Columns;
using Inventec.Common.Logging;
using Inventec.Common.Print.Ado;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Print.Loader
{
    class PaperSizeLoader
    {
        public static void LoadData(PrinterSettings PrinterSettings, DevExpress.XtraEditors.GridLookUpEdit cboPaperSize)
        {
            List<PaperSize> paperSizes = new List<PaperSize>();
            List<PaperSizeAdo> paperSizeAdos = new List<PaperSizeAdo>();
            try
            {
                foreach (PaperSize size in PrinterSettings.PaperSizes)
                {
                    PaperSizeAdo paperSizeAdo = new Ado.PaperSizeAdo(size.PaperName, size.Width, size.Height);
                    paperSizeAdo.Kind = size.Kind;
                    paperSizeAdo.RawKind = size.RawKind;
                    paperSizeAdos.Add(paperSizeAdo);

                    paperSizes.Add(size);
                }

                cboPaperSize.Properties.DataSource = paperSizes;
                cboPaperSize.Properties.DisplayMember = "PaperName";
                cboPaperSize.Properties.ValueMember = "Kind";

                cboPaperSize.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboPaperSize.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboPaperSize.Properties.ImmediatePopup = true;
                cboPaperSize.ForceInitialize();
                cboPaperSize.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboPaperSize.Properties.View.Columns.AddField("Kind");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;
                
                GridColumn aColumnName = cboPaperSize.Properties.View.Columns.AddField("PaperName");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = false;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                cboPaperSize.Properties.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
