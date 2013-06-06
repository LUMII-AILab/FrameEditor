namespace FrameMarker
{
    partial class frmFrameMarker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFrameMarker));
            this.label1 = new System.Windows.Forms.Label();
            this.cbFrames = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbFrameElements = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmImportLegacy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmImportConll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmExportBitmap = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmShowMorph = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmShowLemmas = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmShowNamedEntities = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.butCreateFrame = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lbFrameEntities = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lbOtherElements = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelSentences = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbToStart = new System.Windows.Forms.ToolStripButton();
            this.tsbPrevious = new System.Windows.Forms.ToolStripButton();
            this.txtCurrentSentence = new System.Windows.Forms.ToolStripTextBox();
            this.tsbNext = new System.Windows.Forms.ToolStripButton();
            this.tsbToLast = new System.Windows.Forms.ToolStripButton();
            this.markerControl = new FrameMarker.MarkerControl();
            this.cmTreeNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmCreateNamedEntity = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDeleteEntity = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDeleteEntityInstance = new System.Windows.Forms.ToolStripMenuItem();
            this.cmPanelSentences = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmDeleteSentence = new System.Windows.Forms.ToolStripMenuItem();
            this.cmNamedEntities = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmNamedEntitiesDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.clmID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvEntities = new System.Windows.Forms.ListView();
            this.txtNamedEntitySearch = new System.Windows.Forms.TextBox();
            this.tsmImportFrames = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.cmTreeNode.SuspendLayout();
            this.cmPanelSentences.SuspendLayout();
            this.cmNamedEntities.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 560);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Frame:";
            // 
            // cbFrames
            // 
            this.cbFrames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbFrames.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbFrames.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbFrames.FormattingEnabled = true;
            this.cbFrames.Location = new System.Drawing.Point(102, 557);
            this.cbFrames.Name = "cbFrames";
            this.cbFrames.Size = new System.Drawing.Size(146, 21);
            this.cbFrames.TabIndex = 2;
            this.cbFrames.SelectedIndexChanged += new System.EventHandler(this.cbFrames_SelectedIndexChanged);
            this.cbFrames.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cbFrames_MouseDown);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 586);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Core Elements:";
            // 
            // lbFrameElements
            // 
            this.lbFrameElements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbFrameElements.FormattingEnabled = true;
            this.lbFrameElements.Location = new System.Drawing.Point(102, 586);
            this.lbFrameElements.Name = "lbFrameElements";
            this.lbFrameElements.Size = new System.Drawing.Size(243, 69);
            this.lbFrameElements.TabIndex = 4;
            this.lbFrameElements.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbFrameElements_MouseDown);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(12, 549);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1082, 2);
            this.label3.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1106, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmNew,
            this.tsmOpen,
            this.tsmSave,
            this.tsmSaveAs,
            this.toolStripMenuItem4,
            this.tsmImportLegacy,
            this.tsmImportFrames,
            this.tsmImportConll,
            this.toolStripMenuItem5,
            this.tsmExportBitmap,
            this.toolStripMenuItem1,
            this.tsmExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // tsmNew
            // 
            this.tsmNew.Image = global::FrameMarker.Properties.Resources.page;
            this.tsmNew.Name = "tsmNew";
            this.tsmNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmNew.Size = new System.Drawing.Size(202, 22);
            this.tsmNew.Text = "&New";
            this.tsmNew.Click += new System.EventHandler(this.tsmNew_Click);
            // 
            // tsmOpen
            // 
            this.tsmOpen.Image = global::FrameMarker.Properties.Resources.folder;
            this.tsmOpen.Name = "tsmOpen";
            this.tsmOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmOpen.Size = new System.Drawing.Size(202, 22);
            this.tsmOpen.Text = "&Open";
            this.tsmOpen.Click += new System.EventHandler(this.tsmOpen_Click);
            // 
            // tsmSave
            // 
            this.tsmSave.Image = global::FrameMarker.Properties.Resources.disk;
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmSave.Size = new System.Drawing.Size(202, 22);
            this.tsmSave.Text = "&Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmSaveAs
            // 
            this.tsmSaveAs.Name = "tsmSaveAs";
            this.tsmSaveAs.Size = new System.Drawing.Size(202, 22);
            this.tsmSaveAs.Text = "Save &As...";
            this.tsmSaveAs.Click += new System.EventHandler(this.tsmSaveAs_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(199, 6);
            // 
            // tsmImportLegacy
            // 
            this.tsmImportLegacy.Name = "tsmImportLegacy";
            this.tsmImportLegacy.Size = new System.Drawing.Size(202, 22);
            this.tsmImportLegacy.Text = "Import Legacy...";
            this.tsmImportLegacy.Click += new System.EventHandler(this.tsmImportLegacy_Click);
            // 
            // tsmImportConll
            // 
            this.tsmImportConll.Name = "tsmImportConll";
            this.tsmImportConll.Size = new System.Drawing.Size(202, 22);
            this.tsmImportConll.Text = "Import &CONLL...";
            this.tsmImportConll.Click += new System.EventHandler(this.tsmImportConll_Click);
            // 
            // tsmExportBitmap
            // 
            this.tsmExportBitmap.Image = global::FrameMarker.Properties.Resources.picture_save;
            this.tsmExportBitmap.Name = "tsmExportBitmap";
            this.tsmExportBitmap.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.tsmExportBitmap.Size = new System.Drawing.Size(202, 22);
            this.tsmExportBitmap.Text = "&Export Bitmap...";
            this.tsmExportBitmap.Click += new System.EventHandler(this.tsmExportBitmap_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
            // 
            // tsmExit
            // 
            this.tsmExit.Image = global::FrameMarker.Properties.Resources.door;
            this.tsmExit.Name = "tsmExit";
            this.tsmExit.ShortcutKeyDisplayString = "Alt+F4";
            this.tsmExit.Size = new System.Drawing.Size(202, 22);
            this.tsmExit.Text = "E&xit";
            this.tsmExit.Click += new System.EventHandler(this.tsmExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmUndo,
            this.tsmRedo,
            this.toolStripMenuItem2,
            this.tsmDelete,
            this.toolStripMenuItem3,
            this.tsmSelectAll});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // tsmUndo
            // 
            this.tsmUndo.Enabled = false;
            this.tsmUndo.Name = "tsmUndo";
            this.tsmUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.tsmUndo.Size = new System.Drawing.Size(167, 22);
            this.tsmUndo.Text = "&Undo";
            this.tsmUndo.Click += new System.EventHandler(this.tsmUndo_Click);
            // 
            // tsmRedo
            // 
            this.tsmRedo.Enabled = false;
            this.tsmRedo.Name = "tsmRedo";
            this.tsmRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.tsmRedo.Size = new System.Drawing.Size(167, 22);
            this.tsmRedo.Text = "&Redo";
            this.tsmRedo.Click += new System.EventHandler(this.tsmRedo_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(164, 6);
            // 
            // tsmDelete
            // 
            this.tsmDelete.Image = global::FrameMarker.Properties.Resources.cross;
            this.tsmDelete.Name = "tsmDelete";
            this.tsmDelete.ShortcutKeyDisplayString = "Del";
            this.tsmDelete.Size = new System.Drawing.Size(167, 22);
            this.tsmDelete.Text = "&Delete";
            this.tsmDelete.Click += new System.EventHandler(this.tsmDelete_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(164, 6);
            // 
            // tsmSelectAll
            // 
            this.tsmSelectAll.Name = "tsmSelectAll";
            this.tsmSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.tsmSelectAll.Size = new System.Drawing.Size(167, 22);
            this.tsmSelectAll.Text = "Select A&ll";
            this.tsmSelectAll.Click += new System.EventHandler(this.tsmSelectAll_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmShowMorph,
            this.tsmShowLemmas,
            this.tsmShowNamedEntities});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // tsmShowMorph
            // 
            this.tsmShowMorph.Checked = true;
            this.tsmShowMorph.CheckOnClick = true;
            this.tsmShowMorph.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmShowMorph.Name = "tsmShowMorph";
            this.tsmShowMorph.Size = new System.Drawing.Size(185, 22);
            this.tsmShowMorph.Text = "Show &Morphology";
            this.tsmShowMorph.Click += new System.EventHandler(this.tsmShowMorph_Click);
            // 
            // tsmShowLemmas
            // 
            this.tsmShowLemmas.Checked = true;
            this.tsmShowLemmas.CheckOnClick = true;
            this.tsmShowLemmas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmShowLemmas.Name = "tsmShowLemmas";
            this.tsmShowLemmas.Size = new System.Drawing.Size(185, 22);
            this.tsmShowLemmas.Text = "Show &Lemmas";
            this.tsmShowLemmas.Click += new System.EventHandler(this.tsmShowLemmas_Click);
            // 
            // tsmShowNamedEntities
            // 
            this.tsmShowNamedEntities.Checked = true;
            this.tsmShowNamedEntities.CheckOnClick = true;
            this.tsmShowNamedEntities.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmShowNamedEntities.Name = "tsmShowNamedEntities";
            this.tsmShowNamedEntities.Size = new System.Drawing.Size(185, 22);
            this.tsmShowNamedEntities.Text = "Show &Named Entities";
            this.tsmShowNamedEntities.Click += new System.EventHandler(this.tsmShowNamedEntities_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(692, 562);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Named Entities:";
            // 
            // butCreateFrame
            // 
            this.butCreateFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butCreateFrame.Location = new System.Drawing.Point(254, 558);
            this.butCreateFrame.Name = "butCreateFrame";
            this.butCreateFrame.Size = new System.Drawing.Size(91, 20);
            this.butCreateFrame.TabIndex = 12;
            this.butCreateFrame.Text = "Drag and Drop";
            this.butCreateFrame.UseVisualStyleBackColor = true;
            this.butCreateFrame.Click += new System.EventHandler(this.butCreateFrame_Click);
            this.butCreateFrame.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butCreateFrame_MouseDown);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(362, 562);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Frame Entities:";
            // 
            // lbFrameEntities
            // 
            this.lbFrameEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbFrameEntities.FormattingEnabled = true;
            this.lbFrameEntities.Location = new System.Drawing.Point(444, 559);
            this.lbFrameEntities.Name = "lbFrameEntities";
            this.lbFrameEntities.Size = new System.Drawing.Size(225, 147);
            this.lbFrameEntities.TabIndex = 15;
            this.lbFrameEntities.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbFrameEntities_KeyDown);
            this.lbFrameEntities.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbFrameEntities_MouseDown);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 665);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Other Elements:";
            // 
            // lbOtherElements
            // 
            this.lbOtherElements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbOtherElements.FormattingEnabled = true;
            this.lbOtherElements.Location = new System.Drawing.Point(102, 665);
            this.lbOtherElements.Name = "lbOtherElements";
            this.lbOtherElements.Size = new System.Drawing.Size(243, 43);
            this.lbOtherElements.TabIndex = 17;
            this.lbOtherElements.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbOtherElements_MouseDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(12, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelSentences);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.markerControl);
            this.splitContainer1.Size = new System.Drawing.Size(1082, 519);
            this.splitContainer1.SplitterDistance = 137;
            this.splitContainer1.TabIndex = 18;
            // 
            // panelSentences
            // 
            this.panelSentences.AllowDrop = true;
            this.panelSentences.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelSentences.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSentences.Location = new System.Drawing.Point(0, 25);
            this.panelSentences.Name = "panelSentences";
            this.panelSentences.Size = new System.Drawing.Size(1082, 112);
            this.panelSentences.TabIndex = 19;
            this.panelSentences.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelSentences_DragDrop);
            this.panelSentences.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSentences_Paint);
            this.panelSentences.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSentences_MouseDown);
            this.panelSentences.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSentences_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbToStart,
            this.tsbPrevious,
            this.txtCurrentSentence,
            this.tsbNext,
            this.tsbToLast});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(1082, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbToStart
            // 
            this.tsbToStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToStart.Image = global::FrameMarker.Properties.Resources.control_start_blue;
            this.tsbToStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToStart.Name = "tsbToStart";
            this.tsbToStart.Size = new System.Drawing.Size(23, 22);
            this.tsbToStart.Text = "toolStripButton1";
            this.tsbToStart.ToolTipText = "Go to First Sentence";
            this.tsbToStart.Click += new System.EventHandler(this.tsbToStart_Click);
            // 
            // tsbPrevious
            // 
            this.tsbPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPrevious.Image = global::FrameMarker.Properties.Resources.control_rewind_blue;
            this.tsbPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPrevious.Name = "tsbPrevious";
            this.tsbPrevious.Size = new System.Drawing.Size(23, 22);
            this.tsbPrevious.Text = "toolStripButton3";
            this.tsbPrevious.ToolTipText = "Go to Previous Sentence";
            this.tsbPrevious.Click += new System.EventHandler(this.tsbPrevious_Click);
            // 
            // txtCurrentSentence
            // 
            this.txtCurrentSentence.Name = "txtCurrentSentence";
            this.txtCurrentSentence.Size = new System.Drawing.Size(64, 25);
            this.txtCurrentSentence.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCurrentSentence.TextChanged += new System.EventHandler(this.txtCurrentSentence_TextChanged);
            // 
            // tsbNext
            // 
            this.tsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNext.Image = ((System.Drawing.Image)(resources.GetObject("tsbNext.Image")));
            this.tsbNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.Size = new System.Drawing.Size(23, 22);
            this.tsbNext.Text = "toolStripButton2";
            this.tsbNext.ToolTipText = "Go to Next Sentence";
            this.tsbNext.Click += new System.EventHandler(this.tsbNext_Click);
            // 
            // tsbToLast
            // 
            this.tsbToLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToLast.Image = global::FrameMarker.Properties.Resources.control_end_blue;
            this.tsbToLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToLast.Name = "tsbToLast";
            this.tsbToLast.Size = new System.Drawing.Size(23, 22);
            this.tsbToLast.Text = "toolStripButton4";
            this.tsbToLast.ToolTipText = "Go to Last Sentence";
            this.tsbToLast.Click += new System.EventHandler(this.tsbToLast_Click);
            // 
            // markerControl
            // 
            this.markerControl.AllowDrop = true;
            this.markerControl.BackColor = System.Drawing.Color.White;
            this.markerControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.markerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.markerControl.Location = new System.Drawing.Point(0, 0);
            this.markerControl.Name = "markerControl";
            this.markerControl.Size = new System.Drawing.Size(1082, 378);
            this.markerControl.TabIndex = 13;
            this.markerControl.DragDrop += new System.Windows.Forms.DragEventHandler(this.markerControl_DragDrop);
            this.markerControl.DragEnter += new System.Windows.Forms.DragEventHandler(this.markerControl_DragEnter);
            this.markerControl.DragOver += new System.Windows.Forms.DragEventHandler(this.markerControl_DragOver);
            this.markerControl.Paint += new System.Windows.Forms.PaintEventHandler(this.markerControl_Paint);
            this.markerControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.markerControl_KeyDown);
            this.markerControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.markerControl_KeyUp);
            this.markerControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.markerControl_MouseDown);
            this.markerControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.markerControl_MouseMove);
            this.markerControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.markerControl_MouseUp);
            // 
            // cmTreeNode
            // 
            this.cmTreeNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmCreateNamedEntity,
            this.tsmDeleteEntity,
            this.tsmDeleteEntityInstance});
            this.cmTreeNode.Name = "cmTreeNode";
            this.cmTreeNode.Size = new System.Drawing.Size(215, 70);
            // 
            // tsmCreateNamedEntity
            // 
            this.tsmCreateNamedEntity.Image = global::FrameMarker.Properties.Resources.add;
            this.tsmCreateNamedEntity.Name = "tsmCreateNamedEntity";
            this.tsmCreateNamedEntity.Size = new System.Drawing.Size(214, 22);
            this.tsmCreateNamedEntity.Text = "Create Named Entity";
            this.tsmCreateNamedEntity.Click += new System.EventHandler(this.tsmCreateNamedEntity_Click);
            // 
            // tsmDeleteEntity
            // 
            this.tsmDeleteEntity.Image = global::FrameMarker.Properties.Resources.cross;
            this.tsmDeleteEntity.Name = "tsmDeleteEntity";
            this.tsmDeleteEntity.ShortcutKeyDisplayString = "Shift + Del";
            this.tsmDeleteEntity.Size = new System.Drawing.Size(214, 22);
            this.tsmDeleteEntity.Text = "Delete Entity";
            this.tsmDeleteEntity.Click += new System.EventHandler(this.tsmDeleteEntity_Click);
            // 
            // tsmDeleteEntityInstance
            // 
            this.tsmDeleteEntityInstance.Image = global::FrameMarker.Properties.Resources.cross;
            this.tsmDeleteEntityInstance.Name = "tsmDeleteEntityInstance";
            this.tsmDeleteEntityInstance.ShortcutKeyDisplayString = "Del";
            this.tsmDeleteEntityInstance.Size = new System.Drawing.Size(214, 22);
            this.tsmDeleteEntityInstance.Text = "Delete Entity Instance";
            this.tsmDeleteEntityInstance.Click += new System.EventHandler(this.tsmDeleteEntityInstance_Click);
            // 
            // cmPanelSentences
            // 
            this.cmPanelSentences.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmDeleteSentence});
            this.cmPanelSentences.Name = "cmPanelSentences";
            this.cmPanelSentences.Size = new System.Drawing.Size(165, 26);
            // 
            // tsmDeleteSentence
            // 
            this.tsmDeleteSentence.Image = global::FrameMarker.Properties.Resources.cross;
            this.tsmDeleteSentence.Name = "tsmDeleteSentence";
            this.tsmDeleteSentence.Size = new System.Drawing.Size(164, 22);
            this.tsmDeleteSentence.Text = "Delete Sentence";
            this.tsmDeleteSentence.Click += new System.EventHandler(this.tsmDeleteSentence_Click);
            // 
            // cmNamedEntities
            // 
            this.cmNamedEntities.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmNamedEntitiesDialog});
            this.cmNamedEntities.Name = "cmNamedEntities";
            this.cmNamedEntities.Size = new System.Drawing.Size(178, 26);
            // 
            // tsmNamedEntitiesDialog
            // 
            this.tsmNamedEntitiesDialog.Name = "tsmNamedEntitiesDialog";
            this.tsmNamedEntitiesDialog.Size = new System.Drawing.Size(177, 22);
            this.tsmNamedEntitiesDialog.Text = "Edit Named Entities";
            this.tsmNamedEntitiesDialog.Click += new System.EventHandler(this.tsmNamedEntitiesDialog_Click);
            // 
            // clmID
            // 
            this.clmID.Text = "ID";
            this.clmID.Width = 30;
            // 
            // clmName
            // 
            this.clmName.Text = "Name";
            // 
            // clmType
            // 
            this.clmType.Text = "Category";
            // 
            // clmAlias
            // 
            this.clmAlias.Text = "Alias";
            this.clmAlias.Width = 120;
            // 
            // lvEntities
            // 
            this.lvEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmID,
            this.clmName,
            this.clmType,
            this.clmAlias});
            this.lvEntities.FullRowSelect = true;
            this.lvEntities.GridLines = true;
            this.lvEntities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvEntities.HideSelection = false;
            this.lvEntities.Location = new System.Drawing.Point(779, 559);
            this.lvEntities.MultiSelect = false;
            this.lvEntities.Name = "lvEntities";
            this.lvEntities.Size = new System.Drawing.Size(315, 128);
            this.lvEntities.TabIndex = 19;
            this.lvEntities.UseCompatibleStateImageBehavior = false;
            this.lvEntities.View = System.Windows.Forms.View.Details;
            this.lvEntities.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbNamedEntities_KeyDown);
            this.lvEntities.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbNamedEntities_MouseDown);
            // 
            // txtNamedEntitySearch
            // 
            this.txtNamedEntitySearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNamedEntitySearch.Location = new System.Drawing.Point(779, 688);
            this.txtNamedEntitySearch.Name = "txtNamedEntitySearch";
            this.txtNamedEntitySearch.Size = new System.Drawing.Size(315, 20);
            this.txtNamedEntitySearch.TabIndex = 20;
            this.txtNamedEntitySearch.TextChanged += new System.EventHandler(this.txtNamedEntitySearch_TextChanged);
            // 
            // tsmImportFrames
            // 
            this.tsmImportFrames.Name = "tsmImportFrames";
            this.tsmImportFrames.Size = new System.Drawing.Size(202, 22);
            this.tsmImportFrames.Text = "Import &Frames...";
            this.tsmImportFrames.Click += new System.EventHandler(this.tsmImportFrames_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(199, 6);
            // 
            // frmFrameMarker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 717);
            this.Controls.Add(this.txtNamedEntitySearch);
            this.Controls.Add(this.lvEntities);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lbOtherElements);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbFrameEntities);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.butCreateFrame);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbFrameElements);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbFrames);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1024, 400);
            this.Name = "frmFrameMarker";
            this.Text = "Frame Marker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);            
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.cmTreeNode.ResumeLayout(false);
            this.cmPanelSentences.ResumeLayout(false);
            this.cmNamedEntities.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbFrames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbFrameElements;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmNew;
        private System.Windows.Forms.ToolStripMenuItem tsmOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ToolStripMenuItem tsmSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmExit;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmUndo;
        private System.Windows.Forms.ToolStripMenuItem tsmRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsmDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsmSelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem tsmExportBitmap;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmShowMorph;
        private System.Windows.Forms.ToolStripMenuItem tsmShowLemmas;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem tsmShowNamedEntities;
        private System.Windows.Forms.Button butCreateFrame;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lbFrameEntities;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lbOtherElements;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private MarkerControl markerControl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbToStart;
        private System.Windows.Forms.ToolStripButton tsbPrevious;
        private System.Windows.Forms.ToolStripTextBox txtCurrentSentence;
        private System.Windows.Forms.ToolStripButton tsbNext;
        private System.Windows.Forms.ToolStripButton tsbToLast;
        private System.Windows.Forms.Panel panelSentences;
        private System.Windows.Forms.ContextMenuStrip cmTreeNode;
        private System.Windows.Forms.ToolStripMenuItem tsmDeleteEntity;
        private System.Windows.Forms.ToolStripMenuItem tsmCreateNamedEntity;
        private System.Windows.Forms.ToolStripMenuItem tsmImportConll;
        private System.Windows.Forms.ContextMenuStrip cmPanelSentences;
        private System.Windows.Forms.ToolStripMenuItem tsmDeleteSentence;
        private System.Windows.Forms.ContextMenuStrip cmNamedEntities;
        private System.Windows.Forms.ToolStripMenuItem tsmNamedEntitiesDialog;
        private System.Windows.Forms.ColumnHeader clmID;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.ColumnHeader clmType;
        private System.Windows.Forms.ColumnHeader clmAlias;
        private System.Windows.Forms.ListView lvEntities;
        private System.Windows.Forms.ToolStripMenuItem tsmDeleteEntityInstance;
        private System.Windows.Forms.TextBox txtNamedEntitySearch;
        private System.Windows.Forms.ToolStripMenuItem tsmImportLegacy;
        private System.Windows.Forms.ToolStripMenuItem tsmImportFrames;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;

    }
}

