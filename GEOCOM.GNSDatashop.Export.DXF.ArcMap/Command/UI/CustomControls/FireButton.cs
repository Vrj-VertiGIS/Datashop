using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls
{
    public partial class FireButton : Button
    {
        public enum Mode { Fire, Cancel };

        private Mode _operationMode = Mode.Fire;

        public Mode OperationMode
        {
            get => _operationMode;
            set
            {
                _operationMode = value;
                Refresh();
            }
        }

        public string FireCaption { get; set; }
        public string CancelCaption { get; set; }

        public event EventHandler FireClick;
        public event EventHandler CancelClick;

        public override string Text
        {
            get => (_operationMode == Mode.Fire) ? FireCaption : CancelCaption;
            set
            {
                if (_operationMode == Mode.Fire)
                    FireCaption = value;
                else
                    CancelCaption = value;

                base.Refresh();
            }
        }
            
        public FireButton()
        {
            InitializeComponent();
        }

        public FireButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (_operationMode == Mode.Cancel)
                CancelClick?.Invoke(this, e);
            else
                FireClick?.Invoke(this, e);
        }
    }
}
