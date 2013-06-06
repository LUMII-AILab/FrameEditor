namespace FrameMarker
{
    partial class frmNamedEntities
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lvNamedEntities = new System.Windows.Forms.ListView();
            this.clmID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgAlias = new System.Windows.Forms.DataGridView();
            this.headerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbCategory = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgAlias)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(538, 304);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(457, 304);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // lvNamedEntities
            // 
            this.lvNamedEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvNamedEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmID,
            this.clmName,
            this.clmType,
            this.clmAlias});
            this.lvNamedEntities.FullRowSelect = true;
            this.lvNamedEntities.GridLines = true;
            this.lvNamedEntities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvNamedEntities.HideSelection = false;
            this.lvNamedEntities.Location = new System.Drawing.Point(12, 12);
            this.lvNamedEntities.MultiSelect = false;
            this.lvNamedEntities.Name = "lvNamedEntities";
            this.lvNamedEntities.Size = new System.Drawing.Size(279, 277);
            this.lvNamedEntities.TabIndex = 7;
            this.lvNamedEntities.UseCompatibleStateImageBehavior = false;
            this.lvNamedEntities.View = System.Windows.Forms.View.Details;
            this.lvNamedEntities.SelectedIndexChanged += new System.EventHandler(this.lvNamedEntities_SelectedIndexChanged);
            // 
            // clmID
            // 
            this.clmID.Text = "ID";
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
            this.clmAlias.Width = 210;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(309, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(312, 28);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(304, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Category:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(312, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Alias:";
            // 
            // dgAlias
            // 
            this.dgAlias.AllowUserToResizeRows = false;
            this.dgAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgAlias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAlias.ColumnHeadersVisible = false;
            this.dgAlias.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.headerName});
            this.dgAlias.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgAlias.Location = new System.Drawing.Point(312, 130);
            this.dgAlias.MultiSelect = false;
            this.dgAlias.Name = "dgAlias";
            this.dgAlias.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgAlias.RowHeadersVisible = false;
            this.dgAlias.ShowCellErrors = false;
            this.dgAlias.ShowCellToolTips = false;
            this.dgAlias.ShowEditingIcon = false;
            this.dgAlias.ShowRowErrors = false;
            this.dgAlias.Size = new System.Drawing.Size(301, 159);
            this.dgAlias.TabIndex = 15;
            this.dgAlias.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgAlias_CellEndEdit);
            this.dgAlias.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgAlias_RowsAdded);
            this.dgAlias.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgAlias_UserAddedRow);
            this.dgAlias.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgAlias_KeyDown);
            // 
            // headerName
            // 
            this.headerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.headerName.HeaderText = "Name";
            this.headerName.MinimumWidth = 290;
            this.headerName.Name = "headerName";
            this.headerName.Width = 290;
            // 
            // cbCategory
            // 
            this.cbCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbCategory.FormattingEnabled = true;
            this.cbCategory.Items.AddRange(new object[] {
            "persona",
            "loc",
            "org",
            "facility",
            "event",
            "prod",
            "time",
            "other"});
            this.cbCategory.Location = new System.Drawing.Point(312, 79);
            this.cbCategory.Name = "cbCategory";
            this.cbCategory.Size = new System.Drawing.Size(304, 21);
            this.cbCategory.TabIndex = 16;
            this.cbCategory.SelectedIndexChanged += new System.EventHandler(this.cbCategory_SelectedIndexChanged);
            this.cbCategory.TextUpdate += new System.EventHandler(this.cbCategory_TextUpdate);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 304);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSearch.Location = new System.Drawing.Point(62, 304);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(229, 20);
            this.txtSearch.TabIndex = 18;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // frmNamedEntities
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(625, 339);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbCategory);
            this.Controls.Add(this.dgAlias);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvNamedEntities);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNamedEntities";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Named Entities";
            ((System.ComponentModel.ISupportInitialize)(this.dgAlias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ListView lvNamedEntities;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.ColumnHeader clmAlias;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader clmType;
        private System.Windows.Forms.ColumnHeader clmID;
        private System.Windows.Forms.DataGridView dgAlias;
        private System.Windows.Forms.DataGridViewTextBoxColumn headerName;
        private System.Windows.Forms.ComboBox cbCategory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSearch;

    }
}