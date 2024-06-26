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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.OtherFormAssTreatment.Base;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.RichEditor.Base;
using Inventec.Common.WordContent;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
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

namespace HIS.Desktop.Plugins.OtherFormAssTreatment
{
    public partial class frmOtherFormAssTreatment : HIS.Desktop.Utility.FormBase
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
                List<string> PrintTypeCodeList = new List<string>();

                if (this.otherFormAssTreatmentInputADO != null && !String.IsNullOrEmpty(this.otherFormAssTreatmentInputADO.PrintTypeCode))
                {
                    PrintTypeCodeList.Add(this.otherFormAssTreatmentInputADO.PrintTypeCode);
                }
                else
                {
                    PrintTypeCodeList.Add(PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000277);
                    PrintTypeCodeList.Add(PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000380);
                    PrintTypeCodeList.Add(PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000379);
                    PrintTypeCodeList.Add(PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000378);
                    PrintTypeCodeList.Add(PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000377);
                }

                this.printTypeTemplates = new List<SarPrintTypeAdo>();

                foreach (var item in PrintTypeCodeList)
                {
                    var printTypeByCodes = RichEditorConfig.PrintTypes.Where(o => (o.PRINT_TYPE_CODE ?? "").ToLower() == item.ToLower()).ToList();
                    if (printTypeByCodes != null && printTypeByCodes.Count > 0)
                    {
                        if (!Directory.Exists(System.IO.Path.Combine(FileLocalStore.Rootpath, item)))
                        {
                            MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_FOLDER_TUONG_UNG_VOI_MA_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + item, item));
                            throw new DirectoryNotFoundException("Khong ton tai folder chua bieu mau in: " + System.IO.Path.Combine(FileLocalStore.Rootpath, item) + " . " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                        }

                        ProcessDirectory(System.IO.Path.Combine(FileLocalStore.Rootpath, item), item);
                        ProcessFile(System.IO.Path.Combine(FileLocalStore.Rootpath, item), item);

                        if (printTypeTemplates == null || printTypeTemplates.Count == 0)
                        {
                            MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_BIEU_MAU_IN_TRONG_FOLDER_TEMPLATE, FileLocalStore.Rootpath + "/" + item, item));

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
                        MessageBox.Show(String.Format(RichEditorConfig.MESSAGE__KHONG_TON_TAI_MA_IN, item));
                    }
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
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.TreatmentId;
                this.Treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                param = new CommonParam();
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = Treatment.PATIENT_ID;
                this.Patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                if (String.IsNullOrWhiteSpace(otherFormAssTreatmentInputADO.PrintTypeCode)
                    || otherFormAssTreatmentInputADO.DicParam == null 
                    || otherFormAssTreatmentInputADO.DicParam.Count <= 0)
                {
                    param = new CommonParam();
                    HisTreatmentBedRoomFilter filter = new HisTreatmentBedRoomFilter();
                    filter.TREATMENT_ID = this.TreatmentId;
                    this.TreatmentBedRooms = new BackendAdapter(param)
                       .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, param);

                }

                if (RichEditorConfig.PrintTypes != null && RichEditorConfig.PrintTypes.Count > 0 && otherFormAssTreatmentInputADO != null && !String.IsNullOrEmpty(otherFormAssTreatmentInputADO.PrintTypeCode))
                {
                    this.currentPrintType = RichEditorConfig.PrintTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == otherFormAssTreatmentInputADO.PrintTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadJSonPrintOld()
        {
            try
            {
                gridControlDetail.DataSource = null;
                this.sarPrints = new List<SAR.EFMODEL.DataModels.SAR_PRINT>();
                if (this.Treatment != null && !string.IsNullOrEmpty(this.Treatment.JSON_PRINT_ID))
                {
                    SAR.Filter.SarPrintFilter printFilter = new SAR.Filter.SarPrintFilter();

                    var printIds = PrintIdByJsonPrint(this.Treatment.JSON_PRINT_ID);
                    if (printIds != null && printIds.Count > 0)
                    {
                        printFilter.IDs = printIds;
                        printFilter.ORDER_FIELD = "CREATE_TIME";
                        printFilter.ORDER_DIRECTION = "DESC";
                        this.sarPrints = new BackendAdapter(new CommonParam())
                        .Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>("api/SarPrint/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, printFilter, new CommonParam());
                    }
                    if (this.sarPrints != null && this.sarPrints.Count > 0)
                    {
                        if (this.currentPrintType != null)
                        {
                            this.sarPrints = this.sarPrints.Where(o => (o.PRINT_TYPE_ID != null && o.PRINT_TYPE_ID == this.currentPrintType.ID)).ToList();
                        }
                        else
                        {
                            var ptAllowIds = RichEditorConfig.PrintTypes != null ? RichEditorConfig.PrintTypes.Where(o => (
                                o.PRINT_TYPE_CODE == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000277
                                || o.PRINT_TYPE_CODE == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000380
                                || o.PRINT_TYPE_CODE == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000379
                                || o.PRINT_TYPE_CODE == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000378
                                || o.PRINT_TYPE_CODE == PrintTypeWorker.PRINT_TYPE_CODE__TREATMENT_MPS000377
                                )).Select(o => o.ID).ToList() : null;

                            this.sarPrints = this.sarPrints.Where(o => o.PRINT_TYPE_ID == null || (ptAllowIds != null && o.PRINT_TYPE_ID != null && ptAllowIds.Contains(o.PRINT_TYPE_ID.Value))).ToList();
                        }
                    }

                    gridControlDetail.DataSource = this.sarPrints;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        public List<long> PrintIdByJsonPrint(string json_Print_Id)
        {
            List<long> result = new List<long>();
            try
            {
                var arrIds = json_Print_Id.Split(',', ';');
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
