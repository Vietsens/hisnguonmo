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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestDepaCreate.Print
{
    public partial class frmPrintByCondition : HIS.Desktop.Utility.FormBase
    {
        HisExpMestResultSDO resultSdo = null;
        List<long> _MaterialTypeIdHc = null;

        public frmPrintByCondition(HisExpMestResultSDO data)
        {
            InitializeComponent();
            try
            {
                this.resultSdo = data;
                this.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_PRINT_BY_CONDITION__CAPTION", Base.ResourceLangManager.LanguageFrmPrintByCondition, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmPrintByCondition_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this._MaterialTypeIdHc = new List<long>();
                txtTitlePrint.Enabled = false;
                checkMedicine.Enabled = false;
                checkMaterial.Enabled = false;
                checkChemistry.Enabled = false;

                if (this.resultSdo != null)
                {
                    CommonParam param = new CommonParam();
                    if (this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                        || this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                        metyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                        this.resultSdo.ExpMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, param);

                        MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                        matyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                        this.resultSdo.ExpMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, param);
                    }
                }

                if (this.resultSdo != null)
                {
                    txtTitlePrint.Enabled = true;
                    if (this.resultSdo.ExpMetyReqs != null && this.resultSdo.ExpMetyReqs.Count > 0)
                    {
                        checkMedicine.Enabled = true;
                        checkMedicine.Checked = true;
                    }
                    if (this.resultSdo.ExpMatyReqs != null && this.resultSdo.ExpMatyReqs.Count > 0)
                    {
                        var _mateTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        foreach (var item in this.resultSdo.ExpMatyReqs)
                        {
                            var dataMate = _mateTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (dataMate != null)
                            {
                                if (dataMate.IS_CHEMICAL_SUBSTANCE == 1)
                                {
                                    this._MaterialTypeIdHc.Add(item.MATERIAL_TYPE_ID);
                                    checkChemistry.Enabled = true;
                                    checkChemistry.Checked = true;
                                }
                                else
                                {
                                    checkMaterial.Enabled = true;
                                    checkMaterial.Checked = true;
                                }
                            }
                        }
                    }
                    SetTitleByCheck();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTitleByCheck()
        {
            try
            {
                List<string> titles = new List<string>();
                titles.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_PRINT_BY_CONDITION__TITLE_PRINT_COMMON", Base.ResourceLangManager.LanguageFrmPrintByCondition, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                if (checkMedicine.Checked)
                {
                    titles.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_PRINT_BY_CONDITION__TITLE_PRINT_MEDICINE", Base.ResourceLangManager.LanguageFrmPrintByCondition, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                }
                if (checkMaterial.Checked)
                {
                    titles.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_PRINT_BY_CONDITION__TITLE_PRINT_MATERIAL", Base.ResourceLangManager.LanguageFrmPrintByCondition, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                }
                if (checkChemistry.Checked)
                {
                    titles.Add(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_PRINT_BY_CONDITION__TITLE_PRINT_CHEMISTRY", Base.ResourceLangManager.LanguageFrmPrintByCondition, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));
                }
                txtTitlePrint.Text = String.Join(" ", titles).ToUpper();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTitlePrint_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkMedicine_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SetTitleByCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkMaterial_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (checkMaterial.Checked)
                //{
                //    checkChemistry.Checked = false;
                //}
                SetTitleByCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkChemistry_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (checkChemistry.Checked)
                //{
                //    checkMaterial.Checked = false;
                //}
                SetTitleByCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatSuDungTheoDieuKien_MPS000134, delegateRunTemplate);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                //Review
                // WaitingManager.Show();
                HisExpMestViewFilter depaFilter = new HisExpMestViewFilter();
                depaFilter.ID = this.resultSdo.ExpMest.ID;
                var listDepaExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, depaFilter, null);
                if (listDepaExpMest == null || listDepaExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc DepaExpMest bang ID");
                var _ExpMest = listDepaExpMest.First();


                List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                mediFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                _ExpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                MOS.Filter.HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                mateFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                _ExpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, param);

                List<V_HIS_EXP_MEST_MEDICINE> listMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> listMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();

                List<HIS_EXP_MEST_METY_REQ> listMetyReq = new List<HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_MATY_REQ> listMatyReq = new List<HIS_EXP_MEST_MATY_REQ>();

                if ((!checkMedicine.Checked
                    && !checkMaterial.Checked
                    && !checkChemistry.Checked))
                {
                    listMedicine = _ExpMestMedicines;
                    listMaterial = _ExpMestMaterials;
                    listMetyReq = this.resultSdo.ExpMetyReqs;
                    listMatyReq = this.resultSdo.ExpMatyReqs;
                }
                else
                {
                    if (checkMedicine.Checked)
                    {
                        listMedicine = _ExpMestMedicines;
                        listMetyReq = this.resultSdo.ExpMetyReqs;
                    }

                    if (resultSdo.ExpMatyReqs != null
                        && resultSdo.ExpMatyReqs.Count > 0)
                    {
                        listMaterial = _ExpMestMaterials;
                        if (checkChemistry.Checked)
                        {
                            if (this._MaterialTypeIdHc != null
                                && this._MaterialTypeIdHc.Count > 0)
                            {
                                var hcs = this.resultSdo.ExpMatyReqs.Where(p => this._MaterialTypeIdHc.Contains(p.MATERIAL_TYPE_ID)).ToList();
                                listMatyReq.AddRange(hcs);
                            }
                        }
                        if (checkMaterial.Checked)
                        {
                            if (this._MaterialTypeIdHc != null
                                && this._MaterialTypeIdHc.Count > 0)
                            {
                                var vts = this.resultSdo.ExpMatyReqs.Where(p => !this._MaterialTypeIdHc.Contains(p.MATERIAL_TYPE_ID)).ToList();
                                listMatyReq.AddRange(vts);
                            }
                            else
                            {
                                listMatyReq = this.resultSdo.ExpMatyReqs;
                            }
                        }
                    }
                }
                //Review

                MPS.Processor.Mps000134.PDO.Mps000134PDO mps000134RDO = new MPS.Processor.Mps000134.PDO.Mps000134PDO(
                listMetyReq,
                listMatyReq,
                listMedicine,
                listMaterial,
                _ExpMest,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                txtTitlePrint.Text.ToString().ToUpper()
                            );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000134RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000134RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
                //if (result)
                //{
                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
