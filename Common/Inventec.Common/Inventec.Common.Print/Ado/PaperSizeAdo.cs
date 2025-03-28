﻿using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Print.Ado
{
    internal class PaperSizeAdo
    {
        internal PaperSizeAdo() { }
                
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Printing.PaperSize class.
        //
        // Parameters:
        //   name:
        //     The name of the paper.
        //
        //   width:
        //     The width of the paper, in hundredths of an inch.
        //
        //   height:
        //     The height of the paper, in hundredths of an inch.
        public PaperSizeAdo(string name, int width, int height)
        {
            this.PaperName = name;
            this.Width = width;
            this.Height = height;
        }

        // Summary:
        //     Gets or sets the height of the paper, in hundredths of an inch.
        //
        // Returns:
        //     The height of the paper, in hundredths of an inch.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The System.Drawing.Printing.PaperSize.Kind property is not set to System.Drawing.Printing.PaperKind.Custom.
        public int Height { get; set; }
        //
        // Summary:
        //     Gets the type of paper.
        //
        // Returns:
        //     One of the System.Drawing.Printing.PaperKind values.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The System.Drawing.Printing.PaperSize.Kind property is not set to System.Drawing.Printing.PaperKind.Custom.
        public PaperKind Kind { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the type of paper.
        //
        // Returns:
        //     The name of the type of paper.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The System.Drawing.Printing.PaperSize.Kind property is not set to System.Drawing.Printing.PaperKind.Custom.
        public string PaperName { get; set; }
        //
        // Summary:
        //     Gets or sets an integer representing one of the System.Drawing.Printing.PaperSize
        //     values or a custom value.
        //
        // Returns:
        //     An integer representing one of the System.Drawing.Printing.PaperSize values,
        //     or a custom value.
        public int RawKind { get; set; }
        //
        // Summary:
        //     Gets or sets the width of the paper, in hundredths of an inch.
        //
        // Returns:
        //     The width of the paper, in hundredths of an inch.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The System.Drawing.Printing.PaperSize.Kind property is not set to System.Drawing.Printing.PaperKind.Custom.
        public int Width { get; set; }

    }
}
