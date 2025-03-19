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
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.Ado
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
