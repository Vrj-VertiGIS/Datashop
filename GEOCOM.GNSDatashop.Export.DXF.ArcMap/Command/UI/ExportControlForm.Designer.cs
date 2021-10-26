namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI
{
    public partial class ExportControlForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelSelectFeatures = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbMaskingLayer = new GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls.LayerSelectionComboBox(this.components);
            this.cmbSelectionLayer = new GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls.LayerSelectionComboBox(this.components);
            this.lblMaskingLayer = new System.Windows.Forms.Label();
            this.lblSelectionLayer = new System.Windows.Forms.Label();
            this.cbRestrictToVisibleLayers = new System.Windows.Forms.CheckBox();
            this.cbRestrictToSelection = new System.Windows.Forms.CheckBox();
            this.cbRestrictToScreenExtent = new System.Windows.Forms.CheckBox();
            this.panelChooseFile = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowseOutputFile = new System.Windows.Forms.Button();
            this.txtOutfileSspec = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelStart = new System.Windows.Forms.Panel();
            this.btnExport = new GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls.FireButton(this.components);
            this.panelFormatOptions = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbBinary = new System.Windows.Forms.CheckBox();
            this.cmbDxfVersion = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.panel1.SuspendLayout();
            this.panelSelectFeatures.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panelChooseFile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelStart.SuspendLayout();
            this.panelFormatOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panelSelectFeatures);
            this.panel1.Controls.Add(this.panelChooseFile);
            this.panel1.Controls.Add(this.panelStart);
            this.panel1.Controls.Add(this.panelFormatOptions);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(337, 539);
            this.panel1.TabIndex = 0;
            // 
            // panelSelectFeatures
            // 
            this.panelSelectFeatures.Controls.Add(this.groupBox3);
            this.panelSelectFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSelectFeatures.Location = new System.Drawing.Point(0, 163);
            this.panelSelectFeatures.Name = "panelSelectFeatures";
            this.panelSelectFeatures.Size = new System.Drawing.Size(337, 325);
            this.panelSelectFeatures.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbMaskingLayer);
            this.groupBox3.Controls.Add(this.cmbSelectionLayer);
            this.groupBox3.Controls.Add(this.lblMaskingLayer);
            this.groupBox3.Controls.Add(this.lblSelectionLayer);
            this.groupBox3.Controls.Add(this.cbRestrictToVisibleLayers);
            this.groupBox3.Controls.Add(this.cbRestrictToSelection);
            this.groupBox3.Controls.Add(this.cbRestrictToScreenExtent);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(337, 325);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "10104Auswahl der zu exportierenden Daten";
            // 
            // cmbMaskingLayer
            // 
            this.cmbMaskingLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMaskingLayer.CausesValidation = false;
            this.cmbMaskingLayer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMaskingLayer.FormattingEnabled = true;
            this.cmbMaskingLayer.Location = new System.Drawing.Point(17, 269);
            this.cmbMaskingLayer.Name = "cmbMaskingLayer";
            this.cmbMaskingLayer.NoLayerEntry = true;
            this.cmbMaskingLayer.SelectedLayer = null;
            this.cmbMaskingLayer.Size = new System.Drawing.Size(305, 21);
            this.cmbMaskingLayer.TabIndex = 5;
            // 
            // cmbSelectionLayer
            // 
            this.cmbSelectionLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSelectionLayer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSelectionLayer.FormattingEnabled = true;
            this.cmbSelectionLayer.Location = new System.Drawing.Point(17, 177);
            this.cmbSelectionLayer.Name = "cmbSelectionLayer";
            this.cmbSelectionLayer.NoLayerEntry = true;
            this.cmbSelectionLayer.SelectedLayer = null;
            this.cmbSelectionLayer.Size = new System.Drawing.Size(305, 21);
            this.cmbSelectionLayer.TabIndex = 3;
            this.toolTips.SetToolTip(this.cmbSelectionLayer, "10214Selection layer");
            // 
            // lblMaskingLayer
            // 
            this.lblMaskingLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMaskingLayer.Location = new System.Drawing.Point(14, 210);
            this.lblMaskingLayer.Name = "lblMaskingLayer";
            this.lblMaskingLayer.Size = new System.Drawing.Size(320, 56);
            this.lblMaskingLayer.TabIndex = 6;
            this.lblMaskingLayer.Text = "10212Maskierungslayer. Erlaubt, Features ganz oder teilweise vom Export auszuschl" +
    "iessen. Features, welche von Polygonen dieses Layers abgececkt werden, werden ni" +
    "cht exportiert.";
            // 
            // lblSelectionLayer
            // 
            this.lblSelectionLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectionLayer.Location = new System.Drawing.Point(15, 129);
            this.lblSelectionLayer.Name = "lblSelectionLayer";
            this.lblSelectionLayer.Size = new System.Drawing.Size(308, 45);
            this.lblSelectionLayer.TabIndex = 4;
            this.lblSelectionLayer.Text = "10210Features nur exportieren, wenn sie innerhalb eines Polygons dieses Selektion" +
    "slayers liegen. Pro Polygon des Selektionslayers wird ein DXF erstellt.";
            // 
            // cbRestrictToVisibleLayers
            // 
            this.cbRestrictToVisibleLayers.AutoSize = true;
            this.cbRestrictToVisibleLayers.Location = new System.Drawing.Point(18, 30);
            this.cbRestrictToVisibleLayers.Name = "cbRestrictToVisibleLayers";
            this.cbRestrictToVisibleLayers.Size = new System.Drawing.Size(203, 17);
            this.cbRestrictToVisibleLayers.TabIndex = 2;
            this.cbRestrictToVisibleLayers.Text = "10204Nur sichtbare Layer exportieren";
            this.cbRestrictToVisibleLayers.UseVisualStyleBackColor = true;
            // 
            // cbRestrictToSelection
            // 
            this.cbRestrictToSelection.AutoSize = true;
            this.cbRestrictToSelection.Location = new System.Drawing.Point(17, 98);
            this.cbRestrictToSelection.Name = "cbRestrictToSelection";
            this.cbRestrictToSelection.Size = new System.Drawing.Size(231, 17);
            this.cbRestrictToSelection.TabIndex = 1;
            this.cbRestrictToSelection.Text = "10208Nur ausgewählte Objekte exportieren";
            this.cbRestrictToSelection.UseVisualStyleBackColor = true;
            // 
            // cbRestrictToScreenExtent
            // 
            this.cbRestrictToScreenExtent.AutoSize = true;
            this.cbRestrictToScreenExtent.Location = new System.Drawing.Point(17, 65);
            this.cbRestrictToScreenExtent.Name = "cbRestrictToScreenExtent";
            this.cbRestrictToScreenExtent.Size = new System.Drawing.Size(232, 17);
            this.cbRestrictToScreenExtent.TabIndex = 0;
            this.cbRestrictToScreenExtent.Text = "10206Auf Bildschirmauschnitt einschränken";
            this.cbRestrictToScreenExtent.UseVisualStyleBackColor = true;
            // 
            // panelChooseFile
            // 
            this.panelChooseFile.Controls.Add(this.groupBox2);
            this.panelChooseFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelChooseFile.Location = new System.Drawing.Point(0, 95);
            this.panelChooseFile.Name = "panelChooseFile";
            this.panelChooseFile.Size = new System.Drawing.Size(337, 68);
            this.panelChooseFile.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBrowseOutputFile);
            this.groupBox2.Controls.Add(this.txtOutfileSspec);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(337, 68);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "10102Ausgabedatei(en):";
            // 
            // btnBrowseOutputFile
            // 
            this.btnBrowseOutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutputFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseOutputFile.Location = new System.Drawing.Point(279, 25);
            this.btnBrowseOutputFile.Name = "btnBrowseOutputFile";
            this.btnBrowseOutputFile.Size = new System.Drawing.Size(44, 24);
            this.btnBrowseOutputFile.TabIndex = 2;
            this.btnBrowseOutputFile.Text = "...";
            this.btnBrowseOutputFile.UseVisualStyleBackColor = true;
            this.btnBrowseOutputFile.Click += new System.EventHandler(this.btnBrowseOutputFile_Click);
            // 
            // txtOutfileSspec
            // 
            this.txtOutfileSspec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutfileSspec.Location = new System.Drawing.Point(133, 28);
            this.txtOutfileSspec.Name = "txtOutfileSspec";
            this.txtOutfileSspec.Size = new System.Drawing.Size(140, 20);
            this.txtOutfileSspec.TabIndex = 1;
            this.txtOutfileSspec.TextChanged += new System.EventHandler(this.txtOutfileSspec_TextChanged);
            this.txtOutfileSspec.Leave += new System.EventHandler(this.txtOutfileSspec_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "10106Export Datei:";
            // 
            // panelStart
            // 
            this.panelStart.Controls.Add(this.btnExport);
            this.panelStart.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStart.Location = new System.Drawing.Point(0, 488);
            this.panelStart.Name = "panelStart";
            this.panelStart.Size = new System.Drawing.Size(337, 51);
            this.panelStart.TabIndex = 1;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.CancelCaption = "10222Export a&bbrechen";
            this.btnExport.FireCaption = "10220Export";
            this.btnExport.Location = new System.Drawing.Point(227, 8);
            this.btnExport.Name = "btnExport";
            this.btnExport.OperationMode = GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls.FireButton.Mode.Fire;
            this.btnExport.Size = new System.Drawing.Size(102, 33);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "10220Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.FireClick += new System.EventHandler(this.btnExport_Click);
            this.btnExport.CancelClick += new System.EventHandler(this.btnExport_CancelClick);
            // 
            // panelFormatOptions
            // 
            this.panelFormatOptions.Controls.Add(this.groupBox1);
            this.panelFormatOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFormatOptions.Location = new System.Drawing.Point(0, 0);
            this.panelFormatOptions.Name = "panelFormatOptions";
            this.panelFormatOptions.Size = new System.Drawing.Size(337, 95);
            this.panelFormatOptions.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbBinary);
            this.groupBox1.Controls.Add(this.cmbDxfVersion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 95);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "10108Formatoptionen";
            // 
            // cbBinary
            // 
            this.cbBinary.AutoSize = true;
            this.cbBinary.Location = new System.Drawing.Point(18, 68);
            this.cbBinary.Name = "cbBinary";
            this.cbBinary.Size = new System.Drawing.Size(219, 17);
            this.cbBinary.TabIndex = 5;
            this.cbBinary.Text = "10112Binary DXF (storage space saving)";
            this.cbBinary.UseVisualStyleBackColor = true;
            // 
            // cmbDxfVersion
            // 
            this.cmbDxfVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDxfVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDxfVersion.FormattingEnabled = true;
            this.cmbDxfVersion.Location = new System.Drawing.Point(106, 32);
            this.cmbDxfVersion.Name = "cmbDxfVersion";
            this.cmbDxfVersion.Size = new System.Drawing.Size(217, 21);
            this.cmbDxfVersion.TabIndex = 2;
            this.cmbDxfVersion.Enter += new System.EventHandler(this.cmbDxfVersion_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "10110DXF Version:";
            // 
            // toolTips
            // 
            this.toolTips.IsBalloon = true;
            // 
            // ExportControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(320, 0);
            this.Name = "ExportControlForm";
            this.Size = new System.Drawing.Size(337, 539);
            this.Leave += new System.EventHandler(this.ExportControlForm_Leave);
            this.panel1.ResumeLayout(false);
            this.panelSelectFeatures.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panelChooseFile.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelStart.ResumeLayout(false);
            this.panelFormatOptions.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelChooseFile;
        private System.Windows.Forms.Panel panelStart;
        private System.Windows.Forms.Panel panelFormatOptions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtOutfileSspec;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDxfVersion;
        private System.Windows.Forms.Button btnBrowseOutputFile;
        private System.Windows.Forms.Panel panelSelectFeatures;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbRestrictToVisibleLayers;
        private System.Windows.Forms.CheckBox cbRestrictToSelection;
        private System.Windows.Forms.CheckBox cbRestrictToScreenExtent;
        private System.Windows.Forms.Label lblSelectionLayer;
        private DXF.ArcMap.Command.UI.CustomControls.LayerSelectionComboBox cmbSelectionLayer;
        private DXF.ArcMap.Command.UI.CustomControls.LayerSelectionComboBox cmbMaskingLayer;
        private System.Windows.Forms.Label lblMaskingLayer;
        private CustomControls.FireButton btnExport;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.CheckBox cbBinary;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}
