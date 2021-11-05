using Shared.ComponentModel.SortableBindingList;
using System.ComponentModel;
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
        private int? lastSortedColumn;

        public DataController(TabControl tabControl, string tabName, HttpClient client)
        {
            isRowsInitialized = false;
            lastSortedColumn = null;
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
            table.ColumnHeaderMouseClick += ColumnHeaderClick;
        }

        /// <summary>
        /// Возвращает флаг начальной инициализации <see cref="DataGridView"/>.
        /// </summary>
        /// <returns>true, если инициализация <see cref="DataGridView"/> произошла, иначе false.</returns>
        public bool IsRowsSet()
        {
            return isRowsInitialized;
        }

        /// <summary>
        /// Отправляет запрос на получение записей.
        /// В случае успеха обновляет содержимое <see cref="DataGridView"/>.
        /// При первом вызове устанавливает флаг начальной инициализации.
        /// </summary>
        public async void UpdateRows()
        {
            var rows = await Requester.GetAsync<T>(client, GetTypeUrl());
            if (rows == null)
                return;

            table.DataSource = new SortableBindingList<T>(rows);
            isRowsInitialized = true;
        }

        /// <summary>
        /// Открывает диалог создания записи и отправляет запрос на ее добавление.
        /// В случае успеха обновляет содержимое <see cref="DataGridView"/>.
        /// </summary>
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

        /// <summary>
        /// Открывает диалог редактирования текущей записи.
        /// Если запись была изменена, отправляет запрос на ее обновление.
        /// В случае успеха обновляет содержимое <see cref="DataGridView"/>.
        /// </summary>
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

        /// <summary>
        /// Отправляет запрос на удаление текущей записи.
        /// Предварительно выводит сообщение для подтверждения действия.
        /// </summary>
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

        /// <summary>
        /// Получает строку пути к api для текущего типа.
        /// </summary>
        /// <param name="id">ID записи.</param>
        /// <returns>Url строка.</returns>
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
            if (e.RowIndex != -1)
                EditCurrentRow();
        }

        private void ColumnHeaderClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lastSortedColumn == null || lastSortedColumn != e.ColumnIndex)
            {
                table.Sort(table.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                lastSortedColumn = e.ColumnIndex;
            }
            else
            {
                table.Sort(table.Columns[e.ColumnIndex], ListSortDirection.Descending);
                lastSortedColumn = null;
            }
        }
        #endregion
    }
}
