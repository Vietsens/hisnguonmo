using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.DrugUsageAnalysis.ADO;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis
{
    public partial class UCDrugUsageAnalysis : UserControlBase
    {
        private void treeListDateTime_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            var data = (TrackingDrugUseAnalysisADO)treeListDateTime.GetDataRecordByNode(e.Node);

            if (data != null && e.Node.ParentNode == null)
            {
                if (e.Column.FieldName == "DrugUsageAnalysisDetail")
                {
                    e.RepositoryItem = repositoryItemButtonEdit_DrugUsageAnalysisDetail;
                }

            }
        }

        private void treeListDateTime_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (TrackingDrugUseAnalysisADO)treeListDateTime.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Node.ParentNode == null)
                    {
                        if (data.IsColorDeepSkyBlue)
                        {
                            e.Appearance.ForeColor = Color.DeepSkyBlue;
                        }
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                        if (data.NOrder == 0)
                        {
                            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListDateTime_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    var data = (TrackingDrugUseAnalysisADO)treeListDateTime.GetDataRecordByNode(e.Node);
                    if (data != null)
                    {
                        if (e.Node.ParentNode == null)
                        {
                            if (e.Column.FieldName == "NUM_ORDER")
                            {
                                int index = e.Node.TreeList.Nodes.IndexOf(e.Node);
                                e.Value = index + 1;
                            }
                            else if (e.Column.FieldName == "TRACKING_TIME_STR")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRACKING_TIME);
                            }
                            else if (e.Column.FieldName == "SHEET_ORDER_STR")
                            {
                                e.Value = data.SHEET_ORDER.HasValue ? data.SHEET_ORDER.ToString() : "";

                            }
                            else if (e.Column.FieldName == "ED_STT_NAME")
                            {
                                e.Value = data.EMR_DOCUMENT_STT_NAME;
                            }
                            else if (e.Column.FieldName == "ICD_NAME_STR")
                            {
                                e.Value = data.ICD_NAME;
                            }
                            else if (e.Column.FieldName == "ICD_TEXT_STR")
                            {
                                e.Value = data.ICD_TEXT;
                            }
                            else if (e.Column.FieldName == "DEPARTMENT_NAME_STR")
                            {
                                e.Value = data.DEPARTMENT_NAME;
                            }
                            else if (e.Column.FieldName == "ROOM_NAME_STR")
                            {
                                e.Value = data.ROOM_NAME;
                            }
                        }
                        else
                        {
                            if (e.Column.FieldName == "TRACKING_TIME_STR")
                            {
                                if (data.NOrder == 0)
                                {
                                    e.Value = "Thuốc";
                                }
                                else
                                {
                                    e.Value = data.ServiceName;
                                }
                            }
                            else if (e.Column.FieldName == "SHEET_ORDER_STR")
                            {
                                if (data.NOrder == 0)
                                {
                                    e.Value = "Số lượng";
                                }
                                else
                                {
                                    e.Value = data.Amount;
                                }
                            }
                            else if (e.Column.FieldName == "ED_STT_NAME")
                            {
                                if (data.NOrder == 0)
                                {
                                    e.Value = "Đơn vị tính";
                                }
                                else
                                {
                                    e.Value = data.ServiceUnitName;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void repositoryItemButtonEdit_DrugUsageAnalysisDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var focusedNode = treeListDateTime.FocusedNode;
                if (focusedNode == null) return;
                var data = (TrackingDrugUseAnalysisADO)treeListDateTime.GetDataRecordByNode(focusedNode);
                if (data != null)
                {
                    List<object> listArgs = new List<object>();
                    {
                        // V_HIS_TRACKING
                        var trackingData = new V_HIS_TRACKING();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TRACKING>(trackingData, data);
                        listArgs.Add(trackingData);
                        // ISALOWEDITPHARMACIST
                        bool isAlowEditPharmacist = true;
                        bool isAlowEditDoctor = false;
                        listArgs.Add(Tuple.Create<bool,bool>(isAlowEditPharmacist, isAlowEditDoctor));
                        // DelegateSelectData
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataDrugUsageAnalysisDetail);
                        // Module
                        listArgs.Add(currentModule);
                    }
                    
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                    "HIS.Desktop.Plugins.DrugUsageAnalysisDetail",
                    this.currentModule.RoomId,
                    this.currentModule.RoomTypeId,
                    listArgs);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }
        private void DelegateSelectDataDrugUsageAnalysisDetail(object data)
        {
            try
            {
                if (data != null && data is HIS_DRUG_USE_ANALYSIS his_drug_use)
                {
                    LoadDataToTreeList(his_drug_use.TDL_TREATMENT_ID.Value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
