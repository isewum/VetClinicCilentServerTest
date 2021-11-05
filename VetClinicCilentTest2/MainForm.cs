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

            dataControllers = new List<object>
            {
                new DataController<Doctor>(tabControl, "Ветеринары", client),
                new DataController<Owner>(tabControl, "Клиенты", client),
                new DataController<Animal>(tabControl, "Животные", client),
                new DataController<Vaccine>(tabControl, "Прививки", client),
                new DataController<Service>(tabControl, "Услуги", client)
            };

            Requester.RequestSending += RequestSending;
            Requester.ResponseReceived += ResponseReceived;
        }

        #region Methods
        private void UpdateCurrentTab()
        {
            InvokeDataControllerMethod("UpdateRows");
        }

        private void CreateCurrentTabRow()
        {
            InvokeDataControllerMethod("CreateRow");
        }

        private void DeleleCurrentTabRow()
        {
            InvokeDataControllerMethod("DeleteCurrentRow");
        }

        private void EditCurrentTabRow()
        {
            InvokeDataControllerMethod("EditCurrentRow");
        }


        private bool IsCurrentTabSet()
        {
            return (bool)InvokeDataControllerMethod("IsRowsSet");
        }

        /// <summary>
        /// Вызывает метод класса <see cref="DataController{T}"/> с использованием отражения.
        /// </summary>
        /// <param name="methodName">Имя метода.</param>
        /// <param name="parameters">Аргументы, принимаемые методом.</param>
        /// <returns>Объект, возвращаемый вызываемым методом.</returns>
        private object InvokeDataControllerMethod(string methodName, object[] parameters = null)
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

        private void editButton_Click(object sender, EventArgs e)
        {
            EditCurrentTabRow();
        }

        private void RequestSending()
        {
            connectionStatusLabel.Text = "Отправка запроса...";
            parentPanel.Enabled = false;
            this.UseWaitCursor = true;
        }

        private void ResponseReceived(bool isSuccess, string errorMessage)
        {
            connectionStatusLabel.Text = isSuccess ? $"Готово." : errorMessage;
            parentPanel.Enabled = true;
            this.UseWaitCursor = false;

            bool buttonState = isSuccess || IsCurrentTabSet();
            deleteButton.Enabled = buttonState;
            createButton.Enabled = buttonState;
            editButton.Enabled = buttonState;

            /*if (!isSuccess)
            {
                MessageBox.Show(errorMessage, "Ошибка!");
            }*/
        }
        #endregion
    }
}
