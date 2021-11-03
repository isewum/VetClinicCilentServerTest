using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using VetClinicModelLibTest;

namespace VetClinicCilentTest2
{
    class DataController<T> where T : ModelBase, new()
    {
        private readonly DataGridView table = new();
        private readonly HttpClient client;

        private bool /*isCellEditing,*/ isRowsInitialized;
        private object lastCellValue;

        public DataController(TabControl tabControl, string tabName, HttpClient client)
        {
            //isCellEditing = false;
            isRowsInitialized = false;
            lastCellValue = null;

            this.client = client;

            InitControls(tabControl, tabName);
        }
            
        #region Methods
        /// <summary>
        /// Инициализация элементов <see cref="TabPage"/> и <see cref="DataGridView"/>.
        /// </summary>
        /// <param name="tabControl">Родительский элемент TabControl, в котором будет создан элемент TabPage.</param>
        /// <param name="tabName">Отображаемое название элемента TabPage.</param>
        private void InitControls(TabControl tabControl, string tabName)
        {
            table.AllowUserToOrderColumns = true;
            table.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            table.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            table.BackgroundColor = System.Drawing.SystemColors.Menu;
            table.BorderStyle = BorderStyle.None;
            table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            table.Location = new System.Drawing.Point(2, 2);
            table.MultiSelect = false;
            table.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            table.RowTemplate = new DataGridViewRow{ Height = 25 };
            table.Size = new System.Drawing.Size(786, 344);

            table.CellBeginEdit += CellBeginEdit;
            table.CellEndEdit += CellEndEdit;
            table.CellValidating += CellValidating;

            TabPage tabPage = new()
            {
                Text = tabName,
                Name = typeof(T).Name,
                Size = new System.Drawing.Size(790, 352)
            };
            tabPage.Controls.Add(table);
            tabControl.TabPages.Add(tabPage);
        }

        public bool IsRowsSet()
        {
            return isRowsInitialized;
        }

        public async void UpdateRows()
        {
            //table.Rows.Clear();
            //table.Rows.Add(rows);
            table.DataSource = await GetRows();
            isRowsInitialized = true;
        }

        public async void CreateRow()
        {
            if (!isRowsInitialized)
                return;

            T entity = new();
            CreateDialog dialog = new CreateDialog(entity);
            dialog.ShowDialog();
            if (dialog.DialogResult != DialogResult.OK)
                return;

            bool created = await SaveCreatedRow(entity);
            if (created)
            {
                UpdateRows();
            }
        }

        public async void SaveRow(int id)
        {
            T entity = table.Rows[id].DataBoundItem as T;
            bool saved = await SaveEditedRow(entity);
            if (saved)
            {
                UpdateRows();
            }
        }

        public async void DeleteCurrentRow()
        {
            if (!isRowsInitialized)
                return;

            T entity = null;
            if (table.SelectedRows.Count == 1)
            {
                entity = table.SelectedRows[0].DataBoundItem as T;
            }
            else if (table.SelectedCells.Count == 1)
            {
                int row = table.SelectedCells[0].RowIndex;
                entity = table.Rows[row].DataBoundItem as T;
            }
            if (entity == null)
                return;
            
            DialogResult result = MessageBox.Show("Точно удалить строку?", "Внимание!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                bool deleted = await DeleteRow(entity.Id);
                if (deleted)
                {
                    UpdateRows();
                }
            }
        }

        private async Task<List<T>> GetRows()
        {
            return await Requester.GetAsync<T>(client, GetTypeUrl());
        }

        private async Task<bool> SaveEditedRow(T entity)
        {
            return await Requester.UpdateAsync<T>(client, GetTypeUrl(entity.Id), entity);
        }

        private async Task<bool> SaveCreatedRow(T entity)
        {
            return await Requester.CreateAsync<T>(client, GetTypeUrl(), entity);
        }

        private async Task<bool> DeleteRow(int id)
        {
            return await Requester.DeleteAsync(client, GetTypeUrl(id));
        }

        private static string GetTypeUrl(int? id = null)
        {
            if (id != null)
                return $"{typeof(T).Name}s/{id}";

            return $"{typeof(T).Name}s";
        }

        private static bool IsEqual(object obj1, object obj2)
        {
            if (obj1.GetType() != obj2.GetType())
                return false;

            if (obj1 is ValueType)
                return (dynamic)obj1 == (dynamic)obj2;

            if (obj1 is string)
                return String.Equals(obj1, obj2);

            return obj1.Equals(obj2);
        }
        #endregion

        #region EventHandlers
        private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //isCellEditing = true;
            lastCellValue = table.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //isCellEditing = false;
            object newCellValue = table.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (!IsEqual(lastCellValue, newCellValue))
            {
                SaveRow(e.RowIndex);
            }
        }

        private void CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //if (!isCellEditing) return;
        }
        #endregion
    }
}
