using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Forms;

namespace VetClinicCilentTest2
{
    public partial class CreateDialog : Form
    {
        public CreateDialog(object entity)
        {
            InitializeComponent();
            propertyGrid.SelectedObject = entity;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var context = new ValidationContext(propertyGrid.SelectedObject);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(propertyGrid.SelectedObject, context, results, true);

            if (isValid)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            StringBuilder builder = new(results.Count);
            foreach (var res in results)
            {
                builder.AppendLine(res.ErrorMessage);
            }
            MessageBox.Show(builder.ToString(), "Ошибка!");
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
