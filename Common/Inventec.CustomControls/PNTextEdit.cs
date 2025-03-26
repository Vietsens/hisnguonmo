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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors.Mask;

namespace Inventec.CustomControls
{
    public partial class PNTextEdit : UserControl
    {
        private int borderSize = 5;
        private int borderRadius = 40;
        private Color borderColor = Color.Blue;
        private Color backColor = Color.White;
        public PNTextEdit()
        {
            InitializeComponent();
        }
        public event EventHandler _TextChanged;
        public event EventHandler _EditValueChanged;
        [Category("PN Code Inventec")]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }
            set
            {
                borderSize = value;
                this.Invalidate();
            }
        }
        [Category("PN Code Inventec")]
        public int BorderRadius
        {
            get
            {
                return borderRadius;
            }
            set
            {
                borderRadius = value;
                this.Invalidate();
            }
        }
        [Category("PN Code Inventec")]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }
        [Category("PN Code Inventec")]
        public Color BackgroundColor
        {
            get { return this.backColor; }
            set { this.backColor = value; }
        }
        [Category("PN Code Inventec")]
        public Color TextColor
        {
            get { return this.ForeColor; }
            set { this.ForeColor = value; }
        }
        [Category("PN Code Inventec")]
        public string Texts
        {
            get
            {
                return textEdit1.Text;
            }
            set
            {
                textEdit1.Text = value;
            }
        }
        [Category("PN Code Inventec")]
        public string EditMaskPn
        {
            get
            {
                return textEdit1.Properties.Mask.EditMask;
            }
            set
            {
                textEdit1.Properties.Mask.EditMask = value;
            }
        }
        [Category("PN Code Inventec")]
        public MaskType MaskTypes
        {
            get
            {
                return textEdit1.Properties.Mask.MaskType;
            }
            set
            {
                textEdit1.Properties.Mask.MaskType = value;
            }
        }
        [Category("PN Code Inventec")]
        public string TextHintNull
        {
            get
            {
                return textEdit1.Properties.NullValuePrompt;
            }
            set
            {
                textEdit1.Properties.NullValuePrompt = value;
                textEdit1.Properties.NullValuePromptShowForEmptyValue = true;
                textEdit1.Properties.ShowNullValuePromptWhenFocused = true;
            }
        }
        [Category("PN Code Inventec")]
        public int MaxLengthTexts
        {
            get
            {
                return textEdit1.Properties.MaxLength;
            }
            set
            {
                textEdit1.Properties.MaxLength = value;
            }
        }
        private GraphicsPath GetFigurePath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gragh = e.Graphics;

            if (borderRadius > 1)
            {
                var rectBorderSmooth = this.ClientRectangle;
                var rectBorder = Rectangle.Inflate(rectBorderSmooth, -borderSize, -borderSize);
                int smoothSize = borderSize > 0 ? borderSize : 1;
                using (GraphicsPath pathBorderSmooth = GetFigurePath(rectBorderSmooth, borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorderSmooth, borderRadius - borderSize))
                using (Pen penBorderSmooth = new Pen(this.Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    this.Region = new Region(pathBorderSmooth);
                    if (borderRadius > 15) SetTextEditRoundedRegion();
                    gragh.SmoothingMode = SmoothingMode.AntiAlias;
                    penBorder.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                    gragh.DrawPath(penBorderSmooth, pathBorderSmooth);
                    gragh.DrawPath(penBorder, pathBorder);

                }
            }
            else
            {

                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    this.Region = new Region(this.ClientRectangle);
                    penBorder.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    gragh.DrawRectangle(penBorder, 0, 0, this.Width - 0.5F, this.Height - 0.5F);

                }
            }
        }

        private void SetTextEditRoundedRegion()
        {
            GraphicsPath pathTxt;
            pathTxt = GetFigurePath(textEdit1.ClientRectangle, borderSize * 2);
            textEdit1.Region = new Region(pathTxt);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.DesignMode)
                UpdateControl();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateControl();
        }

        private void UpdateControl()
        {
            this.Height = textEdit1.Height + this.Padding.Top + this.Padding.Bottom;
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.OnKeyPress(e);            
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void textEdit1_Leave(object sender, EventArgs e)
        {
            this.OnLeave(e);
            
        }

        private void textEdit1_TextChanged(object sender, EventArgs e)
        {
            if (_TextChanged != null)
                _TextChanged.Invoke(sender, e);
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (_EditValueChanged != null)
                _EditValueChanged.Invoke(sender, e);
        }


    }
}
