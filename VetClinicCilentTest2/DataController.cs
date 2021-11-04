using System.Net.Http;
using System.Windows.Forms;
using VetClinicModelLibTest;

namespace VetClinicCilentTest2
{
    class DataController<T> where T : ModelBase, new()
    {
        private readonly DataGridView table = new();
        private readonly HttpClient client;

        private bool isRowsInitialized;

        public DataController(TabControl tabControl, string tabName, HttpClient client)
        {
            isRowsInitialized = false;
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
            TabPage tabPage = new()
            {
                Text = tabName,
                Name = typeof(T).Name,
                Size = new System.Drawing.Size(790, 352)
            };
            tabPage.Controls.Add(table);
            tabControl.TabPages.Add(tabPage);

            table.AllowUserToOrderColumns = true;
            table.AllowUserToAddRows = false;
            table.AllowUserToDeleteRows = false;
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
            table.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            table.ReadOnly = true;

            table.CellDoubleClick += CellDoubleClick;
        }

        public bool IsRowsSet()
        {
            return isRowsInitialized;
        }

        public async void UpdateRows()
        {
            var rows = await Requester.GetAsync<T>(client, GetTypeUrl());
            if (rows == null)
                return;

            table.DataSource = rows;
            isRowsInitialized = true;
        }

        public async void CreateRow()
        {
            if (!isRowsInitialized)
                return;

            T entity = new();
            EntityDialog dialog = new EntityDialog(DialogTypes.Create, entity);
            dialog.ShowDialog();
            if (dialog.DialogResult != DialogResult.OK)
                return;

            bool created = await Requester.CreateAsync<T>(client, GetTypeUrl(), entity);
            if (created)
            {
                UpdateRows();
            }
        }

        public async void EditCurrentRow()
        {
            if (!isRowsInitialized || table.SelectedRows.Count == 0)
                return;

            T entity = table.SelectedRows[0].DataBoundItem as T;
            T copy = (T)entity.Clone();
            EntityDialog dialog = new EntityDialog(DialogTypes.Edit, copy);
            dialog.ShowDialog();
            if (dialog.DialogResult != DialogResult.OK || entity.Equals(copy))
                return;

            bool saved = await Requester.UpdateAsync<T>(client, GetTypeUrl(entity.Id), copy);
            if (saved)
            {
                UpdateRows();
            }
        }

        public async void DeleteCurrentRow()
        {
            if (!isRowsInitialized || table.SelectedRows.Count == 0)
                return;

            T entity = table.SelectedRows[0].DataBoundItem as T;
            
            DialogResult result = MessageBox.Show("Точно удалить строку?", "Внимание!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                bool deleted = await Requester.DeleteAsync(client, GetTypeUrl(entity.Id));
                if (deleted)
                {
                    UpdateRows();
                }
            }
        }

        private static string GetTypeUrl(int? id = null)
        {
            if (id != null)
                return $"{typeof(T).Name}s/{id}";

            return $"{typeof(T).Name}s";
        }
        #endregion

        #region EventHandlers
        private void CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditCurrentRow();
        }
        #endregion
    }
}
