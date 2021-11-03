using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
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
            Requester.RequestSending += RequestSending;
            Requester.ResponseReceived += ResponseReceived;

            dataControllers = new List<object>
            {
                new DataController<Doctor>(tabControl, "Ветеринары", client),
                new DataController<Owner>(tabControl, "Клиенты", client),
                new DataController<Animal>(tabControl, "Животные", client),
                new DataController<Vaccine>(tabControl, "Прививки", client),
                new DataController<Service>(tabControl, "Услуги", client)
            };
        }

        #region Methods
        private void UpdateCurrentTab()
        {
            InvokeDataViewerMethod("UpdateRows");
        }
        private void CreateCurrentTabRow()
        {
            InvokeDataViewerMethod("CreateRow");
        }

        private void DeleleCurrentTabRow()
        {
            InvokeDataViewerMethod("DeleteCurrentRow");
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

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleleCurrentTabRow();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            CreateCurrentTabRow();
        }

        private void RequestSending()
        {
            connectionStatusLabel.Text = "Отправка запроса...";
            this.Enabled = false;
            this.UseWaitCursor = true;
        }

        private void ResponseReceived(bool isSuccess, string errorMessage)
        {
            connectionStatusLabel.Text = isSuccess ? $"Готово." : errorMessage;
            this.Enabled = true;
            this.UseWaitCursor = false;
            /*if (isSuccess)
            {
                MessageBox.Show(errorMessage);
            }*/
        }
        #endregion

    }
}
