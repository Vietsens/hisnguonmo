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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.TreeSereServ7V2;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.ADO;
using System.Reflection;
using HIS.Desktop.Plugins.ExecuteRoom.Base;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ModuleExt;
using System.IO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.ExecuteRoom.PACS;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class UCExecuteRoom : UserControlBase
    {
        const int SERVICE_CODE__MAX_LENGTH = 6;
        const int SERVICE_REQ_CODE__MAX_LENGTH = 9;
        private const string tempFolder = @"Img\Temp";

        private string GetSoPhieu(string serviceReqCode, string serviceCode)
        {
            string result = "";
            try
            {
                result = String.Format("{0}-{1}", ReduceForCode(serviceReqCode, SERVICE_REQ_CODE__MAX_LENGTH), ReduceForCode(serviceCode, SERVICE_CODE__MAX_LENGTH));
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string ReduceForCode(string orderCode, int maxlength)
        {

            if (!string.IsNullOrWhiteSpace(orderCode) && orderCode.Length >= maxlength)
            {

                return orderCode.Substring(orderCode.Length - maxlength);

            }

            return orderCode;

        }

        private void PacsCallModuleProccess(V_HIS_SERE_SERV_6 aSereServ)
        {
            try
            {
                WaitingManager.Show();
                if (aSereServ != null)
                {
                    //var images = LoadImageFromPacsService(this.GetSoPhieu(aSereServ.TDL_SERVICE_REQ_CODE, aSereServ.TDL_SERVICE_CODE)); List<object> listArgs = new List<object>();
                    ////listArgs.Add(this.moduleData);
                    //var currDirect = Directory.GetCurrentDirectory();
                    //var DirectoryFolder = String.Format("{0}\\{1}", currDirect, tempFolder);
                    //if (Directory.Exists(DirectoryFolder))
                    //{
                    //    Directory.Delete(DirectoryFolder, true);
                    //}

                    //Directory.CreateDirectory(DirectoryFolder); if (images != null && images.Count > 0)
                    //{
                    //    foreach (var item in images)
                    //    {
                    //        string[] fss = new string[] { "FSS\\" };
                    //        string direct = item.ImageDirectory.Split(fss, StringSplitOptions.None).LastOrDefault();
                    //        string fullDirect = direct + "\\" + item.ImageDcmFileName; var jpg = Inventec.Fss.Client.FileDownload.GetFile(fullDirect, ConfigSystems.URI_API_FSS_FOR_PACS);
                    //        string fileCreate = String.Format("{0}\\{1}", DirectoryFolder, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) +"__"+ item.ImageDcmFileName);
                    //        var fileStream = File.Create(fileCreate);
                    //        jpg.Seek(0, SeekOrigin.Begin);
                    //        jpg.CopyTo(fileStream);
                    //        fileStream.Close(); listArgs.Add(fileCreate);
                    //    }
                    //}

                    //List<object> listArgs = new List<object>();
                    //listArgs.Add(aSereServ);
                    //PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.DiimRoom", this.roomId, this.roomTypeId, listArgs);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS.Desktop.Plugins.ExecuteRoom.PACS.ADO.ImagesADO> LoadImageFromPacsService(string soPhieu)
        {
            List<HIS.Desktop.Plugins.ExecuteRoom.PACS.ADO.ImagesADO> result = null;
            try
            {
                CommonParam param = new CommonParam();

                HIS.Desktop.Plugins.ExecuteRoom.PACS.ADO.ImageRequestADO layThongTinAnhInputADO = new HIS.Desktop.Plugins.ExecuteRoom.PACS.ADO.ImageRequestADO();
                layThongTinAnhInputADO.SoPhieu = soPhieu;
                //var resultData = new PACS.ApiConsumerRaw().PostRaw<PACS.ImageResponseADO>(RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH, layThongTinAnhInputADO, 0);

                var resultData = new Inventec.Common.Adapter.BackendAdapter(param).PostWithouApiParam<HIS.Desktop.Plugins.ExecuteRoom.PACS.ADO.ImageResponseADO>(RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH, PacsApiConsumer.PacsConsumer, layThongTinAnhInputADO, null, param);

                if (resultData != null && resultData.TrangThai && resultData.Series != null)
                {
                    result = new List<HIS.Desktop.Plugins.ExecuteRoom.PACS.ADO.ImagesADO>();
                    foreach (var item in resultData.Series)
                    {
                        item.Images.ForEach(o => o.SeriesDateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(item.SeriesDateTime)));
                        result.AddRange(item.Images);
                    }
                    result = result.OrderBy(o => o.SeriesDateTime).ToList();
                }

            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

                result = null;

            }
            return result;
        }
    }
}
