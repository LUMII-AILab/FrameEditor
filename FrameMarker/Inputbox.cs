using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CommonForms
{
    public partial class frmInput : Form
    {
        Func<string, string> verify_del;
        public frmInput(string title, string prompt, string value): this(title,prompt, value, null)
        {            
        }
        public frmInput(string title, string prompt, string value, Func<string, string> ver)
        {                        
            InitializeComponent();

            verify_del = ver;
            this.Text = title;
            lblDesc.Text = prompt;
            InputText = value;
        }
        

        public string InputText
        {
            get{return txtInput.Text;}
            set { txtInput.Text = value; }
        }

        private void frmInput_Validating(object sender, CancelEventArgs e)
        {
            
        }

        private void txtInput_Validating(object sender, CancelEventArgs e)
        {
            if (verify_del != null)
            {
                string error = verify_del(txtInput.Text);
                if (error != null)
                {
                    //MessageBox.Show(error, "Error");
                    errProvider.SetError(txtInput, error);
                    e.Cancel = true;
                }
                else
                {
                    errProvider.SetError(txtInput, "");
                }
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
    
    public class InputBox
    {
        public static string Show(string title, string prompt, string value)
        {
            return Show(title, prompt, value, null);
        }
        public static string Show(string title, string prompt, string value, Func<string, string> ver)
        {
            var form = new frmInput(title, prompt, value, ver);
            var res = form.ShowDialog();
            
            if (res == DialogResult.OK)
            {
                return form.InputText;
            }
            else if (res == DialogResult.Cancel)
            {
                return null;
            }

            return null;
        }
    }
}
