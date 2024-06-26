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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BedHistory.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedHistory
{
    public partial class FormBedHistory : HIS.Desktop.Utility.FormBase
    {
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("<PLACE_YOUR_SERVICE_KEY_HERE>");

        private async Task<Face[]> UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream,
                        true,
                        true,
                        new FaceAttributeType[] { 
                    FaceAttributeType.Gender, 
                    FaceAttributeType.Age, 
                    FaceAttributeType.Emotion 
                });
                    return faces.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new Face[0];
            }
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            Face[] faces = await UploadAndDetectFaces(_imagePath);

            if (faces.Length > 0)
            {
                var faceBitmap = new Bitmap(imgBox.Image);

                using (var g = Graphics.FromImage(faceBitmap))
                {
                    // Alpha-black rectangle on entire image 
                    g.FillRectangle(new SolidBrush(Color.FromArgb(200, 0, 0, 0)), g.ClipBounds);

                    var br = new SolidBrush(Color.FromArgb(200, Color.LightGreen));

                    // Loop each face recognized 
                    foreach (var face in faces)
                    {
                        var fr = face.FaceRectangle;
                        var fa = face.FaceAttributes;

                        // Get original face image (color) to overlap the grayed image 
                        var faceRect = new Rectangle(fr.Left, fr.Top, fr.Width, fr.Height);
                        g.DrawImage(imgBox.Image, faceRect, faceRect, GraphicsUnit.Pixel);
                        g.DrawRectangle(Pens.LightGreen, faceRect);

                        // Loop face.FaceLandmarks properties for drawing landmark spots 
                        var pts = new List<Point>();
                        Type type = face.FaceLandmarks.GetType();
                        foreach (PropertyInfo property in type.GetProperties())
                        {
                            g.DrawRectangle(Pens.LightGreen, GetRectangle((FeatureCoordinate)property.GetValue(face.FaceLandmarks, null)));
                        }

                        // Calculate where to position the detail rectangle 
                        int rectTop = fr.Top + fr.Height + 10;
                        if (rectTop + 45 > faceBitmap.Height) rectTop = fr.Top - 30;

                        // Draw detail rectangle and write face informations                      
                        g.FillRectangle(br, fr.Left - 10, rectTop, fr.Width < 120 ? 120 : fr.Width + 20, 25);
                        g.DrawString(string.Format("{0:0.0} / {1} / {2}", fa.Age, fa.Gender, fa.Emotion.ToRankedList().OrderByDescending(x => x.Value).First().Key),
                                     this.Font, Brushes.Black,
                                     fr.Left - 8,
                                     rectTop + 4);
                    }
                }

                imgBox.Image = faceBitmap;
            }
        }
    }
}
