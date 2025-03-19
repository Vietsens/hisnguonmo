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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Security;
using System.Windows.Forms;
using Microsoft.Win32;
using DocToolkit;
using System.IO;

namespace Inventec.DrawTools
{
    public partial class frmDrawTools : Form
    {
        #region Members
        private DrawArea drawArea;
        private DocManager docManager;
        private DragDropManager dragDropManager;
        private MruManager mruManager;
        private Action<Image> actSave;

        private string argumentFile = ""; // file name from command line

        private const string registryPath = "Software\\AlexF\\DrawTools";

        private bool _controlKey = false;
        private bool _panMode = false;
        #endregion

        #region Properties
        /// <summary>
        /// File name from the command line
        /// </summary>
        public string ArgumentFile
        {
            get { return argumentFile; }
            set { argumentFile = value; }
        }

        /// <summary>
        /// Get reference to Edit menu item.
        /// Used to show context menu in DrawArea class.
        /// </summary>
        /// <value></value>
        public ToolStripMenuItem ContextParent
        {
            get { return editToolStripMenuItem; }
        }
        public Image HinhAnh { get; set; }
        #endregion

        #region Constructor
        public frmDrawTools()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(MainForm_MouseWheel);
        }
        public frmDrawTools(Image hinh, Action<Image> _actSave = null)
            : this()
        {
            this.HinhAnh = hinh;
            this.actSave = _actSave;

            toolStripDropDownButton1.Visible = toolStripDropDownButton2.Visible = true;
            if (!toolStripButtonUndo.Visible && !toolStripButtonRedo.Visible)
            {
                toolStripSeparator3.Visible = false;
            }
        }

        public frmDrawTools(Image hinh, Action<Image> _actSave = null, bool? isNew = null, bool? isOpen = null, bool? isSaveAs = null, bool? isRoom = null, bool? isRotate = null, bool? isPan = null, bool? isFillColorAndConnectLine = null, bool? isLineAndPenSize = null)
            : this()
        {
            this.HinhAnh = hinh;
            this.actSave = _actSave;
            try
            {
                if (isNew.HasValue && !isNew.Value)
                {
                    toolStripButtonNew.Visible = false;
                }
                if (isOpen.HasValue && !isOpen.Value)
                {
                    toolStripButtonOpen.Visible = false;
                }
                if (isSaveAs.HasValue && !isSaveAs.Value)
                {
                    toolStripButtonSaveAs.Visible = false;
                }
                if (isRoom.HasValue && !isRoom.Value)
                {
                    tsbZoomIn.Visible = tsbZoomOut.Visible = tsbZoomReset.Visible = toolStripSeparator4.Visible = false;
                }
                if (isRotate.HasValue && !isRotate.Value)
                {
                    tsbRotateLeft.Visible = tsbRotateRight.Visible = tsbRotateReset.Visible = toolStripSeparator5.Visible = false;
                }
                if (isPan.HasValue && !isPan.Value)
                {
                    tsbPanMode.Visible = tsbPanReset.Visible = false;
                }
                if (isFillColorAndConnectLine.HasValue && !isFillColorAndConnectLine.Value)
                {
                    tsbFillColor.Visible = tsbConnector.Visible = tsbFilledRectangle.Visible = tsbPolyLine.Visible = tsbFilledEllipse.Visible = toolStripSeparator6.Visible = false;
                }

                if (!toolStripButtonUndo.Visible && !toolStripButtonRedo.Visible)
                {
                    toolStripSeparator3.Visible = false;
                }
                if (isLineAndPenSize.HasValue)
                {
                    toolStripDropDownButton1.Visible = toolStripDropDownButton2.Visible = isLineAndPenSize.Value;
                    if (!toolStripButtonUndo.Visible && !toolStripButtonRedo.Visible)
                    {
                        toolStripSeparator3.Visible = false;
                    }
                }
                else
                {
                    toolStripDropDownButton1.Visible = toolStripDropDownButton2.Visible = false;
                }
            }
            catch { }
        }

        #endregion

        #region Toolbar Event Handlers
        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            CommandNew();
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            CommandOpen();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            this.unselectAllToolStripMenuItem_Click(null, null);
            CommandPolygon();
            int width = Convert.ToInt32(drawArea.Width);
            int height = Convert.ToInt32(drawArea.Height);
            Bitmap bmp = new Bitmap(width, height);
            drawArea.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
            this.HinhAnh = new Bitmap(bmp);
            if (this.actSave != null)
            {
                this.actSave(this.HinhAnh);
            }
        }

        private void toolStripButtonSaveAndExit_Click(object sender, EventArgs e)
        {
            this.unselectAllToolStripMenuItem_Click(null, null);
            toolStripButtonSave_Click(null, null);
            this.Close();
        }
        private void toolStripButtonSaveAs_Click(object sender, EventArgs e)
        {

        }
        private void toolStripButtonPointer_Click(object sender, EventArgs e)
        {
            CommandPointer();
        }

        private void toolStripButtonRectangle_Click(object sender, EventArgs e)
        {
            CommandRectangle();
        }

        private void toolStripButtonEllipse_Click(object sender, EventArgs e)
        {
            CommandEllipse();
        }

        private void toolStripButtonLine_Click(object sender, EventArgs e)
        {
            CommandLine();
        }

        private void toolStripButtonPencil_Click(object sender, EventArgs e)
        {
            CommandPolygon();
        }

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            CommandAbout();
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            CommandUndo();
        }

        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            CommandRedo();
        }
        #endregion Toolbar Event Handlers

        #region Menu Event Handlers
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandNew();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandOpen();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandSaveAs();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = drawArea.TheLayers.ActiveLayerIndex;
            drawArea.TheLayers[x].Graphics.SelectAll();
            drawArea.Refresh();
        }

        private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = drawArea.TheLayers.ActiveLayerIndex;
            drawArea.TheLayers[x].Graphics.UnselectAll();
            drawArea.Refresh();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = drawArea.TheLayers.ActiveLayerIndex;
            CommandDelete command = new CommandDelete(drawArea.TheLayers);

            if (drawArea.TheLayers[x].Graphics.DeleteSelection())
            {
                drawArea.Refresh();
                drawArea.AddCommandToHistory(command);
            }
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = drawArea.TheLayers.ActiveLayerIndex;
            CommandDeleteAll command = new CommandDeleteAll(drawArea.TheLayers);

            if (drawArea.TheLayers[x].Graphics.Clear())
            {
                drawArea.Refresh();
                drawArea.AddCommandToHistory(command);
            }
        }

        private void moveToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = drawArea.TheLayers.ActiveLayerIndex;
            if (drawArea.TheLayers[x].Graphics.MoveSelectionToFront())
            {
                drawArea.Refresh();
            }
        }

        private void moveToBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int x = drawArea.TheLayers.ActiveLayerIndex;
            if (drawArea.TheLayers[x].Graphics.MoveSelectionToBack())
            {
                drawArea.Refresh();
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (drawArea.GraphicsList.ShowPropertiesDialog(drawArea))
            //{
            //    drawArea.SetDirty();
            //    drawArea.Refresh();
            //}
        }

        private void pointerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandPointer();
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandRectangle();
        }

        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandEllipse();
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandLine();
        }

        private void pencilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandPolygon();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandAbout();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandRedo();
        }
        #endregion Menu Event Handlers

        #region DocManager Event Handlers
        /// <summary>
        /// Load document from the stream supplied by DocManager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void docManager_LoadEvent(object sender, SerializationEventArgs e)
        {
            // DocManager asks to load document from supplied stream
            try
            {
                drawArea.TheLayers = (Layers)e.Formatter.Deserialize(e.SerializationStream);
            }
            catch (ArgumentNullException ex)
            {
                HandleLoadException(ex, e);
            }
            catch (SerializationException ex)
            {
                HandleLoadException(ex, e);
            }
            catch (SecurityException ex)
            {
                HandleLoadException(ex, e);
            }
        }


        /// <summary>
        /// Save document to stream supplied by DocManager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void docManager_SaveEvent(object sender, SerializationEventArgs e)
        {
            // DocManager asks to save document to supplied stream
            try
            {
                e.Formatter.Serialize(e.SerializationStream, drawArea.TheLayers);
            }
            catch (ArgumentNullException ex)
            {
                HandleSaveException(ex, e);
            }
            catch (SerializationException ex)
            {
                HandleSaveException(ex, e);
            }
            catch (SecurityException ex)
            {
                HandleSaveException(ex, e);
            }
        }
        #endregion

        #region Event Handlers
        private void frmDrawTools_Load(object sender, EventArgs e)
        {
            // Create draw area
            drawArea = new DrawArea();
            drawArea.Location = new Point(0, 0);
            drawArea.Size = new Size(10, 10);
            drawArea.Owner = this;
            Controls.Add(drawArea);

            // Helper objects (DocManager and others)
            InitializeHelperObjects();

            drawArea.Initialize(this, docManager);
            if (this.HinhAnh != null)
            {
                int originalW = this.HinhAnh.Width;
                int originalH = this.HinhAnh.Height;
                int originalScreenW = Screen.PrimaryScreen.Bounds.Width - 366;
                int originalScreenH = Screen.PrimaryScreen.Bounds.Height - 120;
                if (this.HinhAnh.Width > originalScreenW)
                {
                    originalW = originalScreenW;
                    originalH = (originalScreenW * this.HinhAnh.Height) / this.HinhAnh.Width;
                }
                if (this.HinhAnh.Height > originalScreenH)
                {
                    originalH = originalScreenH;
                    originalW = (originalScreenH * this.HinhAnh.Width) / this.HinhAnh.Height;
                }
                if (this.Width < originalW)
                {
                    this.Width = originalW + 15;
                }
                this.Height = originalH + menuStrip1.Height + toolStrip1.Height + toolStripStatus.Height + 40;
                this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
                MouseEventArgs mouse = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
                CommandNew();
                drawArea.ActiveTool = DrawArea.DrawToolType.Image;
                drawArea.DrawArea_MouseDown(null, mouse);
                mouse = new MouseEventArgs(MouseButtons.Left, 1, originalW, originalH, 0);
                drawArea.DrawArea_MouseMove(null, mouse);
                drawArea.DrawArea_MouseUp(null, mouse);
                CommandPolygon();
            }
            ResizeDrawArea();
            LoadSettingsFromRegistry();
            // Submit to Idle event to set controls state at idle time
            Application.Idle += delegate { SetStateOfControls(); };

            // Open file passed in the command line
            if (ArgumentFile.Length > 0)
                OpenDocument(ArgumentFile);

            // Subscribe to DropDownOpened event for each popup menu
            // (see details in MainForm_DropDownOpened)
            foreach (ToolStripItem item in menuStrip1.Items)
            {
                if (item.GetType() ==
                    typeof(ToolStripMenuItem))
                {
                    ((ToolStripMenuItem)item).DropDownOpened += MainForm_DropDownOpened;
                }
            }

            drawArea.PenType = DrawingPens.PenType.DashedArrowPen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.DashedArrowPen);

            //CommandLine();


            drawArea.LineWidth = 2;
            drawArea.PenType = DrawingPens.PenType.Generic;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.Generic);
            CommandPolygon();
        }

        /// <summary>
        /// Resize draw area when form is resized
        /// </summary>
        private void frmDrawTools_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized &&
                drawArea != null)
            {
                ResizeDrawArea();
            }
        }

        /// <summary>
        /// Form is closing
        /// </summary>
        private void frmDrawTools_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason ==
                CloseReason.UserClosing)
            {
                if (!docManager.CloseDocument())
                    e.Cancel = true;
            }

            SaveSettingsToRegistry();

            try
            {
                MouseWheel -= new MouseEventHandler(MainForm_MouseWheel);
                this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.frmDrawTools_KeyDown);
                this.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.frmDrawTools_KeyUp);
                this.Resize -= new System.EventHandler(this.frmDrawTools_Resize);
                this.newToolStripMenuItem.Click -= new System.EventHandler(this.newToolStripMenuItem_Click);
                this.openToolStripMenuItem.Click -= new System.EventHandler(this.openToolStripMenuItem_Click);
                this.saveToolStripMenuItem.Click -= new System.EventHandler(this.saveToolStripMenuItem_Click);
                this.saveAsToolStripMenuItem.Click -= new System.EventHandler(this.saveAsToolStripMenuItem_Click);
                this.exportToolStripMenuItem.Click -= new System.EventHandler(this.exportToolStripMenuItem_Click);
                this.exitToolStripMenuItem.Click -= new System.EventHandler(this.exitToolStripMenuItem_Click);
                this.selectAllToolStripMenuItem.Click -= new System.EventHandler(this.selectAllToolStripMenuItem_Click);
                this.unselectAllToolStripMenuItem.Click -= new System.EventHandler(this.unselectAllToolStripMenuItem_Click);
                this.deleteToolStripMenuItem.Click -= new System.EventHandler(this.deleteToolStripMenuItem_Click);
                this.deleteAllToolStripMenuItem.Click -= new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
                this.moveToFrontToolStripMenuItem.Click -= new System.EventHandler(this.moveToFrontToolStripMenuItem_Click);
                this.moveToBackToolStripMenuItem.Click -= new System.EventHandler(this.moveToBackToolStripMenuItem_Click);
                this.undoToolStripMenuItem.Click -= new System.EventHandler(this.undoToolStripMenuItem_Click);
                this.redoToolStripMenuItem.Click -= new System.EventHandler(this.redoToolStripMenuItem_Click);
                this.propertiesToolStripMenuItem.Click -= new System.EventHandler(this.propertiesToolStripMenuItem_Click);
                this.rectangleToolStripMenuItem.Click -= new System.EventHandler(this.rectangleToolStripMenuItem_Click);
                this.pointerToolStripMenuItem.Click -= new System.EventHandler(this.pointerToolStripMenuItem_Click);
                this.ellipseToolStripMenuItem.Click -= new System.EventHandler(this.ellipseToolStripMenuItem_Click);
                this.lineToolStripMenuItem.Click -= new System.EventHandler(this.lineToolStripMenuItem_Click);
                this.pencilToolStripMenuItem.Click -= new System.EventHandler(this.pencilToolStripMenuItem_Click);
                this.aboutToolStripMenuItem.Click -= new System.EventHandler(this.aboutToolStripMenuItem_Click);
                this.toolStripButtonNew.Click -= new System.EventHandler(this.toolStripButtonNew_Click);
                this.toolStripButtonOpen.Click -= new System.EventHandler(this.toolStripButtonOpen_Click);
                this.toolStripButtonSave.Click -= new System.EventHandler(this.toolStripButtonSave_Click);
                this.toolStripButtonSaveAndExit.Click -= new System.EventHandler(this.toolStripButtonSaveAndExit_Click);
                this.toolStripButtonSaveAs.Click -= new System.EventHandler(this.toolStripButtonSaveAs_Click);
                this.saveAsDrawToolStripMenuItem.Click -= new System.EventHandler(this.saveAsDrawToolStripMenuItem_Click);
                this.saveAsImageToolStripMenuItem.Click -= new System.EventHandler(this.saveAsImageToolStripMenuItem_Click);
                this.toolStripButtonPointer.Click -= new System.EventHandler(this.toolStripButtonPointer_Click);
                this.toolStripButtonRectangle.Click -= new System.EventHandler(this.toolStripButtonRectangle_Click);
                this.toolStripButtonEllipse.Click -= new System.EventHandler(this.toolStripButtonEllipse_Click);
                this.toolStripButtonLine.Click -= new System.EventHandler(this.toolStripButtonLine_Click);
                this.toolStripButtonPencil.Click -= new System.EventHandler(this.toolStripButtonPencil_Click);
                this.tsbLineColor.Click -= new System.EventHandler(this.tsbSelectLineColor_Click);
                this.tsbText.Click -= new System.EventHandler(this.tsbDrawText_Click);
                this.tsbImage.Click -= new System.EventHandler(this.tsbImage_Click);
                this.tsbConnector.Click -= new System.EventHandler(this.tsbConnector_Click);
                this.tsbFilledRectangle.Click -= new System.EventHandler(this.tsbFilledRectangle_Click);
                this.tsbPolyLine.Click -= new System.EventHandler(this.tsbPolyLine_Click);
                this.tsbFilledEllipse.Click -= new System.EventHandler(this.tsbFilledEllipse_Click);
                this.tsbFillColor.Click -= new System.EventHandler(this.tsbSelectFillColor_Click);
                this.thinnestToolStripMenuItem.Click -= new System.EventHandler(this.tsbLineThinnest_Click);
                this.thinToolStripMenuItem.Click -= new System.EventHandler(this.tsbLineThin_Click);
                this.mediumToolStripMenuItem.Click -= new System.EventHandler(this.tsbThickLine_Click);
                this.thickToolStripMenuItem.Click -= new System.EventHandler(this.tsbThickerLine_Click);
                this.thickestToolStripMenuItem.Click -= new System.EventHandler(this.tsbThickestLine_Click);
                this.toolStripMenuItemGenericPen.Click -= new System.EventHandler(this.toolStripMenuItemGenericPen_Click);
                this.redToolStripMenuItem.Click -= new System.EventHandler(this.redToolStripMenuItem_Click);
                this.blueToolStripMenuItem.Click -= new System.EventHandler(this.blueToolStripMenuItem_Click);
                this.greenToolStripMenuItem.Click -= new System.EventHandler(this.greenToolStripMenuItem_Click);
                this.redDottedToolStripMenuItem.Click -= new System.EventHandler(this.redDottedToolStripMenuItem_Click);
                this.redDotDashToolStripMenuItem.Click -= new System.EventHandler(this.redDotDashToolStripMenuItem_Click);
                this.doubleLineToolStripMenuItem.Click -= new System.EventHandler(this.doubleLineToolStripMenuItem_Click);
                this.dashedArrowLineToolStripMenuItem.Click -= new System.EventHandler(this.dashedArrowLineToolStripMenuItem_Click);
                this.toolStripButtonUndo.Click -= new System.EventHandler(this.toolStripButtonUndo_Click);
                this.toolStripButtonRedo.Click -= new System.EventHandler(this.toolStripButtonRedo_Click);
                this.toolStripButtonAbout.Click -= new System.EventHandler(this.toolStripButtonAbout_Click);
                this.tsbZoomIn.Click -= new System.EventHandler(this.tsbZoomIn_Click);
                this.tsbZoomOut.Click -= new System.EventHandler(this.tsbZoomOut_Click);
                this.tsbZoomReset.Click -= new System.EventHandler(this.tsbZoomReset_Click);
                this.tsbRotateLeft.Click -= new System.EventHandler(this.tsbRotateLeft_Click);
                this.tsbRotateRight.Click -= new System.EventHandler(this.tsbRotateRight_Click);
                this.tsbRotateReset.Click -= new System.EventHandler(this.tsbRotateReset_Click);
                this.tsbPanMode.Click -= new System.EventHandler(this.tsbPanMode_Click);
                this.tsbPanReset.Click -= new System.EventHandler(this.tsbPanReset_Click);
                this.toolStripDropDownButton3.Click -= new System.EventHandler(this.toolStripDropDownButton3_Click);
                this.buttonLineToolStripMenuItem.Click -= new System.EventHandler(this.buttonLineToolStripMenuItem_Click);
                this.pencilToolStripMenuItem1.Click -= new System.EventHandler(this.pencilToolStripMenuItem1_Click);
                this.setLineColorToolStripMenuItem.Click -= new System.EventHandler(this.setLineColorToolStripMenuItem_Click);
                this.rectangleToolStripMenuItem1.Click -= new System.EventHandler(this.rectangleToolStripMenuItem1_Click);
                this.ellipseToolStripMenuItem1.Click -= new System.EventHandler(this.ellipseToolStripMenuItem1_Click);
                this.pointToolStripMenuItem.Click -= new System.EventHandler(this.pointToolStripMenuItem_Click);
                this.saveToolStripMenuItem1.Click -= new System.EventHandler(this.saveToolStripMenuItem1_Click);
                this.saveExitToolStripMenuItem.Click -= new System.EventHandler(this.saveExitToolStripMenuItem_Click);
                this.tslCurrentLayer.Click -= new System.EventHandler(this.tslCurrentLayer_Click);
                this.cutToolStripMenuItem.Click -= new System.EventHandler(this.cutToolStripMenuItem_Click);
                Application.Idle -= delegate { SetStateOfControls(); };

                if (drawArea != null)
                {
                    drawArea.Dispose();
                    drawArea = null;
                }
                if (dragDropManager != null)
                {
                    dragDropManager.FileDroppedEvent -= delegate (object sender1, FileDroppedEventArgs ee) { OpenDocument(ee.FileArray.GetValue(0).ToString()); };
                    mruManager.MruOpenEvent -= delegate (object sender1, MruFileOpenEventArgs ee) { OpenDocument(ee.FileName); };

                    dragDropManager = null;
                }
                if (mruManager != null)
                    mruManager = null;
                if (actSave != null)
                    actSave = null;
                if (HinhAnh != null)
                    HinhAnh.Dispose();

                // Subscribe to DocManager events.
                if (docManager != null)
                {
                    docManager.SaveEvent -= docManager_SaveEvent;
                    docManager.LoadEvent -= docManager_LoadEvent;

                    // Make "inline subscription" using anonymous methods.
                    docManager.OpenEvent -= delegate (object sender1, OpenFileEventArgs e1)
                    {
                        // Update MRU List
                        if (e1.Succeeded)
                            mruManager.Add(e1.FileName);
                        else
                            mruManager.Remove(e1.FileName);
                    };

                    docManager.DocChangedEvent -= delegate
                    {
                        drawArea.Refresh();
                        drawArea.ClearHistory();
                    };

                    docManager.ClearEvent -= delegate
                    {
                        bool haveObjects = false;
                        for (int i = 0; i < drawArea.TheLayers.Count; i++)
                        {
                            if (drawArea.TheLayers[i].Graphics.Count > 1)
                            {
                                haveObjects = true;
                                break;
                            }
                        }
                        if (haveObjects)
                        {
                            drawArea.TheLayers.Clear();
                            drawArea.ClearHistory();
                            drawArea.Refresh();
                        }
                    };

                    docManager = null;
                }                

                try
                {
                    foreach (ToolStripItem item in menuStrip1.Items)
                    {
                        if (item.GetType() ==
                            typeof(ToolStripMenuItem))
                        {
                            ((ToolStripMenuItem)item).DropDownOpened -= MainForm_DropDownOpened;
                        }
                    }
                }
                catch (Exception exx12)
                {

                }



                this.Dispose(true);

                GC.SuppressFinalize(this);

                // Force garbage collection.
                GC.Collect();
                // Wait for all finalizers to complete
                GC.WaitForPendingFinalizers();
                // Force garbage collection again.
                GC.Collect();
            }
            catch (Exception exx)
            {
                //TODO
            }
        }

        /// <summary>
        /// Popup menu item (File, Edit ...) is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DropDownOpened(object sender, EventArgs e)
        {
            // Reset active tool to pointer.
            // This prevents bug in rare case when non-pointer tool is active, user opens
            // main main menu and after this clicks in the drawArea. MouseDown event is not
            // raised in this case (why ??), and MouseMove event works incorrectly.
            drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
        }
        #endregion Event Handlers

        #region Other Functions
        /// <summary>
        /// Set state of controls.
        /// Function is called at idle time.
        /// </summary>
        public void SetStateOfControls()
        {
            if ((drawArea != null && drawArea.IsDisposed) || drawArea == null)
            {
                return;
            }
            // Select active tool
            toolStripButtonPointer.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Pointer);
            toolStripButtonRectangle.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Rectangle);
            toolStripButtonEllipse.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Ellipse);
            toolStripButtonLine.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Line);
            toolStripButtonPencil.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Polygon);

            pointerToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Pointer);
            rectangleToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Rectangle);
            ellipseToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Ellipse);
            lineToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Line);
            pencilToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Polygon);

            int x = drawArea.TheLayers.ActiveLayerIndex;
            bool objects = (drawArea.TheLayers[x].Graphics.Count > 0);
            bool selectedObjects = (drawArea.TheLayers[x].Graphics.SelectionCount > 0);
            // File operations
            saveToolStripMenuItem.Enabled = objects;
            toolStripButtonSave.Enabled = objects;
            toolStripButtonSaveAndExit.Enabled = objects;
            toolStripButtonSaveAs.Enabled = objects;
            saveAsToolStripMenuItem.Enabled = objects;

            // Edit operations
            deleteToolStripMenuItem.Enabled = selectedObjects;
            deleteAllToolStripMenuItem.Enabled = objects;
            selectAllToolStripMenuItem.Enabled = objects;
            unselectAllToolStripMenuItem.Enabled = objects;
            moveToFrontToolStripMenuItem.Enabled = selectedObjects;
            moveToBackToolStripMenuItem.Enabled = selectedObjects;
            propertiesToolStripMenuItem.Enabled = selectedObjects;

            // Undo, Redo
            undoToolStripMenuItem.Enabled = drawArea.CanUndo;
            toolStripButtonUndo.Enabled = drawArea.CanUndo;

            redoToolStripMenuItem.Enabled = drawArea.CanRedo;
            toolStripButtonRedo.Enabled = drawArea.CanRedo;

            // Status Strip
            tslCurrentLayer.Text = drawArea.TheLayers[x].LayerName;
            tslNumberOfObjects.Text = drawArea.TheLayers[x].Graphics.Count.ToString();
            tslPanPosition.Text = drawArea.PanX + ", " + drawArea.PanY;
            tslRotation.Text = drawArea.Rotation + " deg";
            tslZoomLevel.Text = (Math.Round(drawArea.Zoom * 100)) + " %";

            // Pan button
            tsbPanMode.Checked = drawArea.Panning;
        }

        /// <summary>
        /// Set draw area to all form client space except toolbar
        /// </summary>
        private void ResizeDrawArea()
        {
            Rectangle rect = ClientRectangle;

            drawArea.Left = rect.Left;
            drawArea.Top = rect.Top + menuStrip1.Height + toolStrip1.Height;
            drawArea.Width = rect.Width;
            drawArea.Height = rect.Height - menuStrip1.Height - toolStrip1.Height;
            ;
        }

        /// <summary>
        /// Initialize helper objects from the DocToolkit Library.
        /// 
        /// Called from Form1_Load. Initialized all objects except
        /// PersistWindowState wich must be initialized in the
        /// form constructor.
        /// </summary>
        private void InitializeHelperObjects()
        {
            // DocManager
            DocManagerData data = new DocManagerData();
            data.FormOwner = this;
            data.UpdateTitle = true;
            data.FileDialogFilter = "DrawTools files (*.dtl)|*.dtl|All Files (*.*)|*.*";
            data.NewDocName = "EditImage.dtl";
            data.RegistryPath = registryPath;

            docManager = new DocManager(data);
            docManager.RegisterFileType("dtl", "dtlfile", "DrawTools File");

            // Subscribe to DocManager events.
            docManager.SaveEvent += docManager_SaveEvent;
            docManager.LoadEvent += docManager_LoadEvent;

            // Make "inline subscription" using anonymous methods.
            docManager.OpenEvent += delegate (object sender, OpenFileEventArgs e)
                                        {
                                            // Update MRU List
                                            if (e.Succeeded)
                                                mruManager.Add(e.FileName);
                                            else
                                                mruManager.Remove(e.FileName);
                                        };

            docManager.DocChangedEvent += delegate
                                            {
                                                drawArea.Refresh();
                                                drawArea.ClearHistory();
                                            };

            docManager.ClearEvent += delegate
                                        {
                                            bool haveObjects = false;
                                            for (int i = 0; i < drawArea.TheLayers.Count; i++)
                                            {
                                                if (drawArea.TheLayers[i].Graphics.Count > 1)
                                                {
                                                    haveObjects = true;
                                                    break;
                                                }
                                            }
                                            if (haveObjects)
                                            {
                                                drawArea.TheLayers.Clear();
                                                drawArea.ClearHistory();
                                                drawArea.Refresh();
                                            }
                                        };

            docManager.NewDocument();

            // DragDropManager
            dragDropManager = new DragDropManager(this);
            dragDropManager.FileDroppedEvent += delegate (object sender, FileDroppedEventArgs e) { OpenDocument(e.FileArray.GetValue(0).ToString()); };

            // MruManager
            mruManager = new MruManager();
            mruManager.Initialize(
                this, // owner form
                recentFilesToolStripMenuItem, // Recent Files menu item
                fileToolStripMenuItem, // parent
                registryPath); // Registry path to keep MRU list

            mruManager.MruOpenEvent += delegate (object sender, MruFileOpenEventArgs e) { OpenDocument(e.FileName); };
        }

        /// <summary>
        /// Handle exception from docManager_LoadEvent function
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="e"></param>
        private void HandleLoadException(Exception ex, SerializationEventArgs e)
        {
            MessageBox.Show(this,
                            "Open File operation failed. File name: " + e.FileName + "\n" +
                            "Reason: " + ex.Message,
                            Application.ProductName);

            e.Error = true;
        }

        /// <summary>
        /// Handle exception from docManager_SaveEvent function
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="e"></param>
        private void HandleSaveException(Exception ex, SerializationEventArgs e)
        {
            MessageBox.Show(this,
                            "Save File operation failed. File name: " + e.FileName + "\n" +
                            "Reason: " + ex.Message,
                            Application.ProductName);

            e.Error = true;
        }

        /// <summary>
        /// Open document.
        /// Used to open file passed in command line or dropped into the window
        /// </summary>
        /// <param name="file"></param>
        public void OpenDocument(string file)
        {
            docManager.OpenDocument(file);
        }

        /// <summary>
        /// Load application settings from the Registry
        /// </summary>
        private void LoadSettingsFromRegistry()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);

                DrawObject.LastUsedColor = Color.FromArgb((int)key.GetValue(
                                                                "Color",
                                                                Color.Black.ToArgb()));

                DrawObject.LastUsedPenWidth = (int)key.GetValue(
                                                    "Width",
                                                    1);
            }
            catch (ArgumentNullException ex)
            {
                HandleRegistryException(ex);
            }
            catch (SecurityException ex)
            {
                HandleRegistryException(ex);
            }
            catch (ArgumentException ex)
            {
                HandleRegistryException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                HandleRegistryException(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                HandleRegistryException(ex);
            }
        }

        /// <summary>
        /// Save application settings to the Registry
        /// </summary>
        private void SaveSettingsToRegistry()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);

                key.SetValue("Color", DrawObject.LastUsedColor.ToArgb());
                key.SetValue("Width", DrawObject.LastUsedPenWidth);
            }
            catch (SecurityException ex)
            {
                HandleRegistryException(ex);
            }
            catch (ArgumentException ex)
            {
                HandleRegistryException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                HandleRegistryException(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                HandleRegistryException(ex);
            }
        }

        private void HandleRegistryException(Exception ex)
        {
            Trace.WriteLine("Registry operation failed: " + ex.Message);
        }

        /// <summary>
        /// Set Pointer draw tool
        /// </summary>
        private void CommandPointer()
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
        }

        /// <summary>
        /// Set Rectangle draw tool
        /// </summary>
        private void CommandRectangle()
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Rectangle;
            drawArea.DrawFilled = false;
        }

        /// <summary>
        /// Set Ellipse draw tool
        /// </summary>
        private void CommandEllipse()
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Ellipse;
            drawArea.DrawFilled = false;
        }

        /// <summary>
        /// Set Line draw tool
        /// </summary>
        private void CommandLine()
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Line;
        }

        /// <summary>
        /// Set Polygon draw tool
        /// </summary>
        private void CommandPolygon()
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Polygon;
        }

        /// <summary>
        /// Show About dialog
        /// </summary>
        private void CommandAbout()
        {
            FrmAbout frm = new FrmAbout();
            frm.ShowDialog(this);
        }

        /// <summary>
        /// Open new file
        /// </summary>
        private void CommandNew()
        {
            docManager.NewDocument();
        }

        /// <summary>
        /// Open file
        /// </summary>
        private void CommandOpen()
        {
            docManager.OpenDocument("");
        }

        /// <summary>
        /// Save file
        /// </summary>
        private void CommandSave()
        {
            docManager.SaveDocument(DocManager.SaveType.Save);
        }

        /// <summary>
        /// Save As
        /// </summary>
        private void CommandSaveAs()
        {
            docManager.SaveDocument(DocManager.SaveType.SaveAs);
        }

        /// <summary>
        /// Undo
        /// </summary>
        private void CommandUndo()
        {
            drawArea.Undo();
        }

        /// <summary>
        /// Redo
        /// </summary>
        private void CommandRedo()
        {
            drawArea.Redo();
        }
        #endregion

        #region Mouse Functions
        private void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if ((drawArea != null && drawArea.IsDisposed) || drawArea == null)
                {
                    return;
                }
                if (_controlKey)
                {
                    // We are panning up or down using the wheel
                    if (e.Delta > 0)
                        drawArea.PanY += 10;
                    else
                        drawArea.PanY -= 10;
                    Invalidate();
                }
                else
                {
                    // We are zooming in or out using the wheel
                    if (e.Delta > 0)
                        AdjustZoom(.1f);
                    else
                        AdjustZoom(-.1f);
                }
                SetStateOfControls();
                return;
            }
        }
        #endregion Mouse Functions

        #region Keyboard Functions
        private void frmDrawTools_KeyDown(object sender, KeyEventArgs e)
        {
            int al = drawArea.TheLayers.ActiveLayerIndex;
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    drawArea.TheLayers[al].Graphics.DeleteSelection();
                    drawArea.Invalidate();
                    break;
                case Keys.Right:
                    drawArea.PanX -= 10;
                    drawArea.Invalidate();
                    break;
                case Keys.Left:
                    drawArea.PanX += 10;
                    drawArea.Invalidate();
                    break;
                case Keys.Up:
                    if (e.KeyCode == Keys.Up &&
                        e.Shift)
                        AdjustZoom(.1f);
                    else
                        drawArea.PanY += 10;
                    drawArea.Invalidate();
                    break;
                case Keys.Down:
                    if (e.KeyCode == Keys.Down &&
                        e.Shift)
                        AdjustZoom(-.1f);
                    else
                        drawArea.PanY -= 10;
                    drawArea.Invalidate();
                    break;
                case Keys.ControlKey:
                    _controlKey = true;
                    break;
                default:
                    break;
            }
            drawArea.Invalidate();
            SetStateOfControls();
        }

        private void frmDrawTools_KeyUp(object sender, KeyEventArgs e)
        {
            _controlKey = false;
        }
        #endregion Keyboard Functions

        #region Zoom, Pan, Rotation Functions
        /// <summary>
        /// Adjust the zoom by the amount given, within reason
        /// </summary>
        /// <param name="_amount">float value to adjust zoom by - may be positive or negative</param>
        private void AdjustZoom(float _amount)
        {
            drawArea.Zoom += _amount;
            if (drawArea.Zoom < .1f)
                drawArea.Zoom = .1f;
            if (drawArea.Zoom > 10)
                drawArea.Zoom = 10f;
            drawArea.Invalidate();
            SetStateOfControls();
        }

        private void tsbZoomIn_Click(object sender, EventArgs e)
        {
            AdjustZoom(.1f);
        }

        private void tsbZoomOut_Click(object sender, EventArgs e)
        {
            AdjustZoom(-.1f);
        }

        private void tsbZoomReset_Click(object sender, EventArgs e)
        {
            drawArea.Zoom = 1.0f;
            drawArea.Invalidate();
        }

        private void tsbRotateRight_Click(object sender, EventArgs e)
        {
            int al = drawArea.TheLayers.ActiveLayerIndex;
            if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
                RotateObject(10);
            else
                RotateDrawing(10);
        }

        private void tsbRotateLeft_Click(object sender, EventArgs e)
        {
            int al = drawArea.TheLayers.ActiveLayerIndex;
            if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
                RotateObject(-10);
            else
                RotateDrawing(-10);
        }

        private void tsbRotateReset_Click(object sender, EventArgs e)
        {
            int al = drawArea.TheLayers.ActiveLayerIndex;
            if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
                RotateObject(0);
            else
                RotateDrawing(0);
        }

        /// <summary>
        /// Rotate the selected Object(s)
        /// </summary>
        /// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
        private void RotateObject(int p)
        {
            int al = drawArea.TheLayers.ActiveLayerIndex;
            for (int i = 0; i < drawArea.TheLayers[al].Graphics.Count; i++)
            {
                if (drawArea.TheLayers[al].Graphics[i].Selected)
                    if (p == 0)
                        drawArea.TheLayers[al].Graphics[i].Rotation = 0;
                    else
                        drawArea.TheLayers[al].Graphics[i].Rotation += p;
            }
            drawArea.Invalidate();
            SetStateOfControls();
        }

        /// <summary>
        /// Rotate the entire drawing
        /// </summary>
        /// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
        private void RotateDrawing(int p)
        {
            if (p == 0)
                drawArea.Rotation = 0;
            else
            {
                drawArea.Rotation += p;
                if (p < 0) // Left Rotation
                {
                    if (drawArea.Rotation <
                        -360)
                        drawArea.Rotation = 0;
                }
                else
                {
                    if (drawArea.Rotation > 360)
                        drawArea.Rotation = 0;
                }
            }
            drawArea.Invalidate();
            SetStateOfControls();
        }

        private void tsbPanMode_Click(object sender, EventArgs e)
        {
            _panMode = !_panMode;
            if (_panMode)
                tsbPanMode.Checked = true;
            else
                tsbPanMode.Checked = false;
            drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
            drawArea.Panning = _panMode;
        }

        private void tsbPanReset_Click(object sender, EventArgs e)
        {
            _panMode = false;
            if (tsbPanMode.Checked)
                tsbPanMode.Checked = false;
            drawArea.Panning = false;
            drawArea.PanX = 0;
            drawArea.PanY = drawArea.OriginalPanY;
            drawArea.Invalidate();
        }
        #endregion  Zoom, Pan, Rotation Functions

        private void tslCurrentLayer_Click(object sender, EventArgs e)
        {
            LayerDialog ld = new LayerDialog(drawArea.TheLayers);
            ld.ShowDialog();
            // First add any new layers
            for (int i = 0; i < ld.layerList.Count; i++)
            {
                if (ld.layerList[i].LayerNew)
                {
                    Layer layer = new Layer();
                    layer.LayerName = ld.layerList[i].LayerName;
                    layer.Graphics = new GraphicsList();
                    drawArea.TheLayers.Add(layer);
                }
            }
            drawArea.TheLayers.InactivateAllLayers();
            for (int i = 0; i < ld.layerList.Count; i++)
            {
                if (ld.layerList[i].LayerActive)
                    drawArea.TheLayers.SetActiveLayer(i);

                if (ld.layerList[i].LayerVisible)
                    drawArea.TheLayers.MakeLayerVisible(i);
                else
                    drawArea.TheLayers.MakeLayerInvisible(i);

                drawArea.TheLayers[i].LayerName = ld.layerList[i].LayerName;
            }
            // Lastly, remove any deleted layers
            for (int i = 0; i < ld.layerList.Count; i++)
            {
                if (ld.layerList[i].LayerDeleted)
                    drawArea.TheLayers.RemoveLayer(i);
            }
            drawArea.Invalidate();
        }

        #region Additional Drawing Tools
        /// <summary>
        /// Draw PolyLine objects - a polyline is a series of straight lines of various lengths connected at their end points.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbPolyLine_Click(object sender, EventArgs e)
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.PolyLine;
            drawArea.DrawFilled = false;
        }

        private void tsbConnector_Click(object sender, EventArgs e)
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Connector;
            drawArea.DrawFilled = false;
        }
        /// <summary>
        /// Draw Text objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbDrawText_Click(object sender, EventArgs e)
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Text;
        }

        private void tsbFilledRectangle_Click(object sender, EventArgs e)
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Rectangle;
            drawArea.DrawFilled = true;
        }

        private void tsbFilledEllipse_Click(object sender, EventArgs e)
        {
            drawArea.ActiveTool = DrawArea.DrawToolType.Ellipse;
            drawArea.DrawFilled = true;
        }

        private void tsbImage_Click(object sender, EventArgs e)
        {
            this.HinhAnh = null;
            drawArea.ActiveTool = DrawArea.DrawToolType.Image;
        }

        private void tsbSelectLineColor_Click(object sender, EventArgs e)
        {
            dlgColor.AllowFullOpen = true;
            dlgColor.AnyColor = true;
            if (dlgColor.ShowDialog() == DialogResult.OK)
            {
                drawArea.LineColor = Color.FromArgb(255, dlgColor.Color);
                tsbLineColor.BackColor = Color.FromArgb(255, dlgColor.Color);
            }
        }

        private void tsbSelectFillColor_Click(object sender, EventArgs e)
        {
            dlgColor.AllowFullOpen = true;
            dlgColor.AnyColor = true;
            if (dlgColor.ShowDialog() == DialogResult.OK)
            {
                drawArea.FillColor = Color.FromArgb(255, dlgColor.Color);
                tsbFillColor.BackColor = Color.FromArgb(255, dlgColor.Color);
            }
        }

        private void tsbLineThinnest_Click(object sender, EventArgs e)
        {
            drawArea.LineWidth = -1;
        }

        private void tsbLineThin_Click(object sender, EventArgs e)
        {
            drawArea.LineWidth = 2;
        }

        private void tsbThickLine_Click(object sender, EventArgs e)
        {
            drawArea.LineWidth = 5;
        }

        private void tsbThickerLine_Click(object sender, EventArgs e)
        {
            drawArea.LineWidth = 10;
        }

        private void tsbThickestLine_Click(object sender, EventArgs e)
        {
            drawArea.LineWidth = 15;
        }
        #endregion Additional Drawing Tools

        private void toolStripMenuItemGenericPen_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.Generic;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.Generic);
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.RedPen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedPen);
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.BluePen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.BluePen);
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.GreenPen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.GreenPen);
        }

        private void redDottedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.RedDottedPen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedDottedPen);
        }

        private void redDotDashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.RedDotDashPen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedDotDashPen);
        }

        private void doubleLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.DoubleLinePen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.DoubleLinePen);
        }

        private void dashedArrowLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.PenType = DrawingPens.PenType.DashedArrowPen;
            drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.DashedArrowPen);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.unselectAllToolStripMenuItem_Click(null, null);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|Fireworks (*.png)|*.png|GIF (*.gif)|*.gif|Icon (*.ico)|*.ico|All files|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(drawArea.Width);
                int height = Convert.ToInt32(drawArea.Height);
                Bitmap bmp = new Bitmap(width, height);
                drawArea.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Bmp);
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawArea.CutObject();
        }

        private void saveAsDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.unselectAllToolStripMenuItem_Click(null, null);
            CommandSave();
        }

        private void saveAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportToolStripMenuItem_Click(null, null);
        }

        private void toolStripDropDownButton3_Click(object sender, EventArgs e)
        {

        }

        private void rectangleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripButtonRectangle_Click(null, null);
        }

        private void setLineColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbSelectLineColor_Click(null, null);
        }

        private void pencilToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripButtonPencil_Click(null, null);
        }

        private void buttonLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonLine_Click(null, null);
        }

        private void ellipseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripButtonEllipse_Click(null, null);
        }

        private void pointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonPointer_Click(null, null);
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripButtonSave_Click(null, null);
        }

        private void saveExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonSaveAndExit_Click(null, null);
        }
    }
}
