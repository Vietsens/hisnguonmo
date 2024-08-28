using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.DocumentViewer
{
    public class DocumentViewerManager
    {
        public ViewType.ENUM type { get; set; }
        public DocumentViewerManager(ViewType.ENUM _type)
        {
            this.type = _type;
        }

        public void Run(string url)
        {
            switch (this.type)
            {
                case ViewType.ENUM.Pdf:
                    TelerikPdf.FormTelerikPdf pdfViewer = new TelerikPdf.FormTelerikPdf(url);
                    pdfViewer.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        public void Run(InputADO data, ViewType.Platform platform)
        {
            switch (this.type)
            {
                case ViewType.ENUM.Pdf:
                    switch (platform)
                    {
                        case ViewType.Platform.Devexpress:
                            Template.frmPdfViewer DevexpressViewer = new Template.frmPdfViewer(data);
                            DevexpressViewer.ShowDialog();
                            break;
                        case ViewType.Platform.Telerik:
                            TelerikPdf.FormTelerikPdf TelerikViewer = new TelerikPdf.FormTelerikPdf(data);
                            TelerikViewer.ShowDialog();
                            break;
                        default:
                            TelerikPdf.FormTelerikPdf defaultViewer = new TelerikPdf.FormTelerikPdf(data);
                            defaultViewer.ShowDialog();
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void Print(InputADO data, ViewType.Platform platform)
        {
            switch (this.type)
            {
                case ViewType.ENUM.Pdf:
                    switch (platform)
                    {
                        case ViewType.Platform.Devexpress:
                            Template.frmPdfViewer DevexpressViewer = new Template.frmPdfViewer(data);
                            DevexpressViewer.PrintFile();
                            break;
                        case ViewType.Platform.Telerik:
                            TelerikPdf.FormTelerikPdf TelerikViewer = new TelerikPdf.FormTelerikPdf(data);
                            TelerikViewer.PrintFile();
                            break;
                        default:
                            TelerikPdf.FormTelerikPdf defaultViewer = new TelerikPdf.FormTelerikPdf(data);
                            defaultViewer.PrintFile();
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
