using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DF2
{
    public partial class ChooseObject : Form
    {
        public System.Drawing.Bitmap h_Bitmap;
        public ChooseObject()
        {
            InitializeComponent();
            h_Bitmap = null;
            btnOK.DialogResult = DialogResult.OK;
        }

        public string hidetext
        { get { return txtbox.Text; } }

        public bool rbtnaeschecked
        { get { return rbtnAES.Checked; } }

        public bool rbtnplainchecked
        { get { return rbtnPlain.Checked; } }

    }
}
