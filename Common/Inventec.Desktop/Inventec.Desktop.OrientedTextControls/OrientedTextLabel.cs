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
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace CustomControl.OrientAbleTextControls
{
	/// <summary>
	/// This is a lable, in which you can set the text in any direction/angle
	/// </summary>
	
	#region Orientation

	//Orientation of the text
	
	public enum Orientation
	{
		Circle,
		Arc,
		Rotate
	}

	public enum Direction
	{
		Clockwise,
		AntiClockwise
	}
	

	#endregion

	public class OrientedTextLabel : System.Windows.Forms.Label
	{

		#region Variables

		private double rotationAngle;
		private string text;
        private Orientation textOrientation;
		private Direction textDirection;

		#endregion

		#region Constructor

		public OrientedTextLabel()
		{
			//Setting the initial condition.
			rotationAngle = 0d;
			textOrientation = Orientation.Rotate;
			this.Size = new Size(105,12);
		}

		#endregion

		#region Properties

		[Description("Rotation Angle"),Category("Appearance")]
		public double RotationAngle
		{
			get
			{
				return rotationAngle;
			}
			set
			{
				
				rotationAngle = value; 
				this.Invalidate();
			}
		}

		[Description("Kind of Text Orientation"),Category("Appearance")]
		public Orientation TextOrientation
		{
			get
			{
				return textOrientation;
			}
			set
			{
				
				textOrientation = value; 
				this.Invalidate();
			}
		}

		[Description("Direction of the Text"),Category("Appearance")]
		public Direction TextDirection
		{
			get
			{
				return textDirection;
			}
			set
			{
				
				textDirection = value; 
				this.Invalidate();
			}
		}

		[Description("Display Text"),Category("Appearance")]
		public override string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				this.Invalidate();
			}
		}

		#endregion

		#region Method
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.Trimming = StringTrimming.None;
			
			Brush textBrush = new SolidBrush(this.ForeColor);
			
			//Getting the width and height of the text, which we are going to write
			float width = graphics.MeasureString(text,this.Font).Width;
			float height = graphics.MeasureString(text,this.Font).Height;

			//The radius is set to 0.9 of the width or height, b'cos not to 
			//hide and part of the text at any stage
			float radius = 0f;
			if(ClientRectangle.Width<ClientRectangle.Height)
			{
				radius = ClientRectangle.Width *0.9f/2;
			}
			else
			{
				radius = ClientRectangle.Height *0.9f/2;
			}

			//Setting the text according to the selection
			switch(textOrientation)
			{
				case Orientation.Arc :
				{
					//Arc angle must be get from the length of the text.
					float arcAngle = (2*width/radius)/text.Length;
					if(textDirection == Direction.Clockwise)
					{
						for(int i=0;i<text.Length;i++)
						{
						
							graphics.TranslateTransform(
								(float)(radius*(1 - Math.Cos(arcAngle*i + rotationAngle/180 * Math.PI))),
								(float)(radius*(1 - Math.Sin(arcAngle*i + rotationAngle/180*Math.PI))));
							graphics.RotateTransform((-90 + (float)rotationAngle + 180*arcAngle*i/(float)Math.PI));
							graphics.DrawString(text[i].ToString(), this.Font, textBrush, 0, 0);
							graphics.ResetTransform();
						}
					}
					else
					{
						for(int i=0;i<text.Length;i++)
						{
						
							graphics.TranslateTransform(
								(float)(radius*(1 - Math.Cos(arcAngle*i + rotationAngle/180*Math.PI))),
								(float)(radius*(1 + Math.Sin(arcAngle*i + rotationAngle/180*Math.PI))));
							graphics.RotateTransform((-90 - (float)rotationAngle - 180*arcAngle*i/(float)Math.PI));
							graphics.DrawString(text[i].ToString(), this.Font, textBrush, 0, 0);
							graphics.ResetTransform();
					
						}
					}
					break;
				}
				case Orientation.Circle :
				{
					if(textDirection == Direction.Clockwise)
					{
							for(int i=0;i<text.Length;i++)
							{
								graphics.TranslateTransform(
									(float)(radius*(1 - Math.Cos((2*Math.PI/text.Length)*i + rotationAngle/180*Math.PI))),
									(float)(radius*(1 - Math.Sin((2*Math.PI/text.Length)*i + rotationAngle/180*Math.PI))));
								graphics.RotateTransform(-90 + (float)rotationAngle + (360/text.Length)*i);
								graphics.DrawString(text[i].ToString(), this.Font, textBrush, 0, 0);
								graphics.ResetTransform();
							}
					}
					else
					{
						for(int i=0;i<text.Length;i++)
						{
							graphics.TranslateTransform(
								(float)(radius*(1 - Math.Cos((2*Math.PI/text.Length)*i + rotationAngle/180*Math.PI))),
								(float)(radius*(1 + Math.Sin((2*Math.PI/text.Length)*i + rotationAngle/180*Math.PI))));
							graphics.RotateTransform(-90 - (float)rotationAngle - (360/text.Length)*i);
							graphics.DrawString(text[i].ToString(), this.Font, textBrush, 0, 0);
							graphics.ResetTransform();
						}
						
					}
					break;
				}
				case Orientation.Rotate :
				{
					//For rotation, who about rotation?
					double angle = (rotationAngle/180)*Math.PI;
					graphics.TranslateTransform(
						(ClientRectangle.Width+(float)(height*Math.Sin(angle))-(float)(width*Math.Cos(angle)))/2,
						(ClientRectangle.Height-(float)(height*Math.Cos(angle))-(float)(width*Math.Sin(angle)))/2);
					graphics.RotateTransform((float)rotationAngle);
					graphics.DrawString(text,this.Font,textBrush,0,0);
					graphics.ResetTransform();

					break;
				}
			}
		}
		#endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

	}
}
