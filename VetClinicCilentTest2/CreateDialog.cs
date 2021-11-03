using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VetClinicCilentTest2
{
    public partial class CreateDialog : Form
    {
        //public object ResultObject => propertyGrid.SelectedObject;

        public CreateDialog(object obj)
        {
            InitializeComponent();
            propertyGrid.SelectedObject = obj;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
