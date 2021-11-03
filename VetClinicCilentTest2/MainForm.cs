using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using VetClinicModelLibTest;

namespace VetClinicCilentTest2
{
    public partial class MainForm : Form
    {
        private readonly string host = "http://localhost:5000/api/";
        private readonly HttpClient client;
        private readonly List<object> dataControllers;

        public MainForm()
        {
            client = new() { BaseAddress = new Uri(host) };

            InitializeComponent();

            dataControllers = new List<object>();
            dataControllers.Add(new DataViewer<Doctor>(tabControl, "Ветеринары"));
            dataControllers.Add(new DataViewer<Owner>(tabControl, "Клиенты"));
            dataControllers.Add(new DataViewer<Animal>(tabControl, "Животные"));
            dataControllers.Add(new DataViewer<Vaccine>(tabControl, "Прививки"));
            dataControllers.Add(new DataViewer<Service>(tabControl, "Услуги"));
        }

        #region Methods
        private async void UpdateCurrentTab()
        {
            connectionStatusLabel.Text = "Отправка запроса...";
            Update();

            int tabIndex = tabControl.SelectedIndex;
            Type dataType = dataControllers[tabIndex].GetType().GetGenericArguments()[0];
            string url = dataType.Name + "s";

            dynamic result = await (dynamic)InvokeRequesterGenericMethod("GetAsync", new object[] { client, url } );

            if (result == null)
            {
                connectionStatusLabel.Text = "Ошибка подключения.";
                return;
            }

            InvokeDataViewerMethod("SetRows", new object[] { result });
            connectionStatusLabel.Text = $"Готово. Получено записей: {result.Count}";
        }

        private void SaveCurrentTabChanges()
        {
            // TODO
            //object changedRows = InvokeDataViewerMethod("GetChangedRows");
        }

        private void DeleleCurrentTabRow()
        {
            // TODO
        }

        private bool IsCurrentTabSet()
        {
            return (bool)InvokeDataViewerMethod("IsRowsSet");
        }

        private object InvokeDataViewerMethod(string methodName, object[] parameters = null)
        {
            int tabIndex = tabControl.SelectedIndex;
            MethodInfo method = dataControllers[tabIndex].GetType().GetMethod(methodName);
            return method.Invoke(dataControllers[tabIndex], parameters);
        }

        private object InvokeRequesterGenericMethod(string methodName, object[] parameters = null)
        {
            int tabIndex = tabControl.SelectedIndex;
            Type dataType = dataControllers[tabIndex].GetType().GetGenericArguments()[0];
            MethodInfo method = typeof(Requester).GetMethod(methodName);
            MethodInfo genericMethod = method.MakeGenericMethod(dataType);

            return genericMethod.Invoke(null, parameters);
        }
        #endregion

        #region EventHandlers
        private void MainForm_Shown(object sender, EventArgs e)
        {
            UpdateCurrentTab();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Dispose();
        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            connectionStatusLabel.Text = "";
            Update();

            if (!IsCurrentTabSet())
                UpdateCurrentTab();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateCurrentTab();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveCurrentTabChanges();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleleCurrentTabRow();
        }
        #endregion
    }
}
