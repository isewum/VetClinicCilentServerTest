using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Forms;

namespace VetClinicCilentTest2
{
    public partial class EntityDialog : Form
    {
        private readonly string createTitle = "Добавление записи";
        private readonly string editTitle = "Редактирование записи";

        public EntityDialog(DialogTypes type, object entity)
        {
            InitializeComponent();
            propertyGrid.SelectedObject = entity;

            this.Text = type == DialogTypes.Create ? createTitle : editTitle;
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

    public enum DialogTypes
    {
        Create,
        Edit
    }
}
