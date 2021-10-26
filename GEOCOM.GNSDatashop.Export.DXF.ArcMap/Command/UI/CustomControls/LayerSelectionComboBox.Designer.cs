namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls
{
    partial class LayerSelectionComboBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayerSelectionComboBox));
            this.imglLayerTypes = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // imglLayerTypes
            // 
            this.imglLayerTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglLayerTypes.ImageStream")));
            this.imglLayerTypes.TransparentColor = System.Drawing.Color.Transparent;
            this.imglLayerTypes.Images.SetKeyName(0, "imgPointLayer");
            this.imglLayerTypes.Images.SetKeyName(1, "imgPolylineLayer");
            this.imglLayerTypes.Images.SetKeyName(2, "imgPolygonLayer");
            this.imglLayerTypes.Images.SetKeyName(3, "imgNoLayer");
            // 
            // LayerSelectionComboBox
            // 
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LayerSelectionComboBox_DrawItem);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imglLayerTypes;
    }
}
