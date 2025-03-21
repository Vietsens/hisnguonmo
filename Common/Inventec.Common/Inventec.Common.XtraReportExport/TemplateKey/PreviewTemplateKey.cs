﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.XtraReportExport.TemplateKey
{
    public partial class PreviewTemplateKey : Form
    {
        List<TemplateKeyAdo> listTemplateKey = new List<TemplateKeyAdo>();

        List<string> removeKey = new List<string>() { "APP_CREATOR", "APP_MODIFIER", "IS_DELETE", "_ID", ".ID" };

        public PreviewTemplateKey()
        {
            InitializeComponent();
        }

        public PreviewTemplateKey(Dictionary<string, object> data)
            : this()
        {
            try
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Key == "ID") continue;
                        if (CheckKey(item.Key)) continue;

                        var ado = new TemplateKeyAdo(String.Format("{0}", item.Key), (item.Value != null ? item.Value.ToString() : ""));
                        //try
                        //{
                        //    ado.Name = String.Format("<#{0};>", item.Key);
                        //    ado.Value = item.Value != null ? item.Value.ToString() : "";
                        //}
                        //catch (Exception ex)
                        //{
                        //    Inventec.Common.Logging.LogSystem.Error(item.Key);
                        //}
                        listTemplateKey.Add(ado);
                    }

                    if (listTemplateKey.Count > 0)
                    {
                        listTemplateKey = listTemplateKey.Where(o => o.Name != null && o.Name != "").ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckKey(string p)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(p))
                {
                    foreach (var item in removeKey)
                    {
                        if (p.EndsWith(item))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void PreviewTemplateKey_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                if (listTemplateKey.Count > 0)
                {
                    listTemplateKey = listTemplateKey.OrderBy(o => o.IsObject).ThenBy(o => String.IsNullOrEmpty(o.Value)).ThenBy(o => o.Name).ToList();
                }

                gridControlKey.BeginUpdate();
                gridControlKey.DataSource = listTemplateKey;
                gridControlKey.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message message, System.Windows.Forms.Keys keys)
        {
            try
            {
                switch (keys)
                {
                    case Keys.Escape:
                        this.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private void gridViewKey_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (TemplateKeyAdo)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var data = new List<TemplateKeyAdo>();

                var search = txtSearch.Text.Trim().ToLower();
                if (!String.IsNullOrEmpty(search))
                {
                    data = listTemplateKey.Where(o => o.Name.ToLower().Contains(search) || (o.Value != null && o.Value.ToLower().Contains(search))).ToList();
                }
                else
                {
                    data = listTemplateKey;
                }

                gridControlKey.BeginUpdate();
                gridControlKey.DataSource = data;
                gridControlKey.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                //this.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__FORM");
                //this.txtSearch.Properties.NullValuePrompt = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__TXT_SEARCH");
                //this.GcKey.Caption = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__GC_KEY");
                //this.GcStt.Caption = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__GC_STT");
                //this.GcValue.Caption = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__GC_VALUE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetResourceMessage(string key)
        {
            string result = "";
            try
            {
                //result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceMessage.languageMessageTemplateKey, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
