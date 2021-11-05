using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VetClinicModelLibTest;

namespace VetClinicCilentTest2
{
    static class Requester
    {
        private delegate Task<HttpResponseMessage> Request(string url, StringContent content);

        public delegate void RequestSendingEventHandler();
        public delegate void ResponseReceivedEventHandler(bool isSuccess, string errorMessage);

        public static event RequestSendingEventHandler RequestSending;
        public static event ResponseReceivedEventHandler ResponseReceived;

        /// <summary>
        /// Отправляет Http запрос к api на получение записей из таблицы БД.
        /// </summary>
        /// <returns>Список объектов указанного типа при успешно выполненном запросе, иначе null.</returns>
        public static async Task<List<T>> GetAsync<T>(HttpClient client, string url) where T : ModelBase
        {
            Request request = async (_url, _content) => await client.GetAsync(_url);

            HttpResponseMessage response = await HandleRequest(url, null, request);
            if (response == null)
                return null;

            string jsonString = await response.Content.ReadAsStringAsync();
            List<T> entities = JsonConvert.DeserializeObject<List<T>>(jsonString);
            return entities;
        }

        /// <summary>
        /// Отправляет Http запрос к api на добавление записи в таблицу БД.
        /// </summary>
        /// <returns>true при успешно выполненном запросе, иначе false.</returns>
        public static async Task<bool> CreateAsync<T>(HttpClient client, string url, T entity) where T : ModelBase
        {
            Request request = async (_url, _content) => await client.PostAsync(_url, _content);

            string jsonString = JsonConvert.SerializeObject(entity);
            StringContent content = new(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HandleRequest(url, content, request);
            return response != null && response.StatusCode == HttpStatusCode.Created;
        }

        /// <summary>
        /// Отправляет Http запрос к api на обновление записи в таблицу БД.
        /// </summary>
        /// <returns>true при успешно выполненном запросе, иначе false.</returns>
        public static async Task<bool> UpdateAsync<T>(HttpClient client, string url, T entity) where T : ModelBase
        {
            Request request = async (_url, _content) => await client.PutAsync(_url, _content);

            string jsonString = JsonConvert.SerializeObject(entity);
            StringContent content = new(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HandleRequest(url, content, request);
            return response != null && response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Отправляет Http запрос к api на удаление записи из таблицы БД.
        /// </summary>
        /// <returns>true при успешно выполненном запросе, иначе false.</returns>
        public static async Task<bool> DeleteAsync(HttpClient client, string url)
        {
            Request request = async (_url, _content) => await client.DeleteAsync(_url);

            HttpResponseMessage response = await HandleRequest(url, null, request);
            return response != null && response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Выполняет отправку запроса и проверку результата.
        /// Показывает сообщение об ошибке в случае обработки исключения.
        /// </summary>
        /// <param name="url">Url адрес.</param>
        /// <param name="content">Json тело запроса.</param>
        /// <param name="request">Делегат, содержащий Http запрос.</param>
        /// <returns>Http ответ сервера при успешно выполненном запросе, иначе null.</returns>
        private static async Task<HttpResponseMessage> HandleRequest(string url, StringContent content, Request request)
        {
            RequestSending?.Invoke();

            try
            {
                HttpResponseMessage response = (await request(url, content)).EnsureSuccessStatusCode();
                ResponseReceived?.Invoke(true, null);
                return response;
            }
            catch (Exception e)
            {
                ResponseReceived?.Invoke(false, e.Message);
                return null;
            }
        }
    }

}
