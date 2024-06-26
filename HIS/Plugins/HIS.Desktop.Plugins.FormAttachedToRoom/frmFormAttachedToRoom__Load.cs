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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.FormAttachedToRoom
{
    public partial class frmFormAttachedToRoom : HIS.Desktop.Utility.FormBase
    {
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListFileToGrid()
        {
            try
            {
                gridControlListFile.DataSource = null;


                this.printTypeTemplates = new List<SarPrintTypeAdo>();

                var printTypeByCodes = RichEditorConfig.PrintTypes.Where(o => (o.PRINT_TYPE_CODE ?? "").ToLower() == "Mps000477".ToLower()).ToList();
                if (printTypeByCodes != null && printTypeByCodes.Count > 0)
                {
                    if (!Directory.Exists(System.IO.Path.Combine(FileLocalStore.Rootpath, "Mps000477")))
                    {
                        MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_FOLDER_TUONG_UNG_VOI_MA_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + "Mps000477", "Mps000477"));
                        throw new DirectoryNotFoundException("Khong ton tai folder chua bieu mau in: " + System.IO.Path.Combine(FileLocalStore.Rootpath, "Mps000477") + " . " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => "Mps000477"), "Mps000477"));
                    }

                    ProcessDirectory(System.IO.Path.Combine(FileLocalStore.Rootpath, "Mps000477"), "Mps000477");
                    ProcessFile(System.IO.Path.Combine(FileLocalStore.Rootpath, "Mps000477"), "Mps000477");

                    if (printTypeTemplates == null || printTypeTemplates.Count == 0)
                    {
                        MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_BIEU_MAU_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + "Mps000477", "Mps000477"));

                        // to get the location the assembly is executing from
                        //(not necessarily where the it normally resides on disk)
                        // in the case of the using shadow copies, for instance in NUnit tests, 
                        // this will be in a temp directory.
                        string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

                        //To get the location the assembly normally resides on disk or the install directory
                        string path1 = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                        Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc file template. path(Location) ="
                            + path + " - path(CodeBase)=" + path1);
                        Inventec.Common.Logging.LogSystem.Debug("FileLocalStore.rootpath =" + FileLocalStore.Rootpath);
                    }
                }
                else
                {
                    //Inventec.Common.Logging.LogSystem.Info("Khong lay duoc sarprint type theo dieu kien tim kiem. fileName=" + this.fileName + " - printTypeCode=" + this.printTypeCode);
                    MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_MA_IN, "Mps000477"));
                }

                gridControlListFile.DataSource = printTypeTemplates;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                InitComboRoomType();
                if (RichEditorConfig.PrintTypes != null && RichEditorConfig.PrintTypes.Count > 0)
                {
                    this.currentPrintType = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == "Mps000477");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoomType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_TYPE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboRoomType, BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                ControlEditorLoader.Load(cboRoomType2, BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitWordContentWithInputParam()
        {
            try
            {
                if ((this.sarPrints == null || this.sarPrints.Count == 0) && this.printTypeTemplates != null && this.printTypeTemplates.Count == 1)
                {
                    CreateClickByNew(this.printTypeTemplates[0]);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<long> PrintIdByJsonPrint(List<V_HIS_ROOM> rooms)
        {
            List<long> result = new List<long>();
            try
            {
                foreach (var item in rooms)
                {
                    if (!string.IsNullOrEmpty(item.JSON_PRINT_ID))
                    {
                        var arrIds = item.JSON_PRINT_ID.Split(',', ';');
                        if (arrIds != null && arrIds.Length > 0)
                        {
                            foreach (var id in arrIds)
                            {
                                long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                                if (printId > 0)
                                {
                                    result.Add(printId);
                                }
                            }
                        }
                    }
                }
                if (result.Count() > 0)
                    result = result.Distinct().ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
