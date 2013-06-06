using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrameMarker
{
    public partial class MarkerControl : UserControl
    {
        public MarkerControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.AllowDrop = true;            
        }       
    }
}
