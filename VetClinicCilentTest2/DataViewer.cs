using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VetClinicModelLibTest;

namespace VetClinicCilentTest2
{
    class DataViewer<T> where T : ModelBase
    {
        private readonly DataGridView table = new();
        private readonly List<int> changedRows = new(), createdRows = new();
        private readonly List<T> initialStateRows = new();

        private readonly DataGridViewCellStyle defaultCellStyle = new() { BackColor = System.Drawing.Color.White };
        private readonly DataGridViewCellStyle changedCellStyle = new() { BackColor = System.Drawing.Color.LightPink };

        private bool isCellEditing, isRowChanged;

        public DataViewer(TabControl tabControl, string tabName)
        {
            isCellEditing = false;
            isRowChanged = false;
            InitControls(tabControl, tabName);
        }

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

        #region Methods
        public bool IsRowsSet()
        {
            return table.DataSource != null;
        }

        public void SetRows(List<T> rows)
        {
            //table.Rows.Clear();
            //table.Rows.Add(rows);
            table.DataSource = rows;

            changedRows.Clear();
            createdRows.Clear();
            initialStateRows.Clear();
        }

        public List<T> GetChangedRows()
        {
            return GetRows(changedRows);
        }

        public List<T> GetCreatedRows()
        {
            return GetRows(createdRows);
        }

        public List<T> GetRows(IEnumerable<int> rowIndexes)
        {
            List<T> rows = new();
            foreach (int id in rowIndexes)
            {
                rows.Add(table.Rows[id].DataBoundItem as T);
            }
            return rows;
        }

        private static bool IsEqual(object obj1, object obj2)
        {
            if (obj1.GetType() != obj2.GetType())
                return false;

            if (obj1 is ValueType)
                return obj1 == obj2;

            return obj1.Equals(obj2);
        }

        private static bool IsEqual(T entity1, T entity2)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetValue(entity1) is ValueType)
                {
                    if (property.GetValue(entity1) != property.GetValue(entity2))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!property.GetValue(entity1).Equals(property.GetValue(entity2)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region EventHandlers
        private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            isCellEditing = true;
        }

        private void CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            isCellEditing = false;

            if (isRowChanged)
            {
                int row = e.RowIndex;

                /*T newValue = table.Rows[row].DataBoundItem as T;
                T oldValue = initialStateRows.FirstOrDefault(t => t.Id == newValue.Id);

                if (IsEqual(newValue, oldValue))
                {
                    initialStateRows.Remove(oldValue);
                    changedRows.Remove(oldValue.Id);

                    table.Rows[row].DefaultCellStyle = defaultCellStyle;
                }
                else
                {*/
                    table.Rows[row].DefaultCellStyle = changedCellStyle;
                //}

                isRowChanged = false;
            }
        }

        private void CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!isCellEditing) return;

            int row = e.RowIndex;
            string colName = table.Columns[e.ColumnIndex].Name;

            T entity = table.Rows[e.RowIndex].DataBoundItem as T;
            object oldPropValue = typeof(T).GetProperty(colName).GetValue(entity);

            e.Cancel = false;
            if (IsEqual(oldPropValue, e.FormattedValue))
            {
                return;
            }

            if (!changedRows.Contains(entity.Id))
            {
                changedRows.Add(entity.Id);
                //initialStateRows.Add(new T(entity));
            }
            isRowChanged = true;
        }
        #endregion
    }
}
