using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ToDoClient.Models;
using DAL.Interfaces;
using DAL.Repositories;
using todoclient.Mapping;
using System.Linq;
using System.Threading;
using DAL.Entities;

namespace ToDoClient.Services
{
    /// <summary>
    /// Works with ToDo backend.
    /// </summary>
    public class ToDoService
    {
        /// <summary>
        /// The service URL.
        /// </summary>
        private readonly string serviceApiUrl = ConfigurationManager.AppSettings["ToDoServiceUrl"];

        /// <summary>
        /// The url for getting all todos.
        /// </summary>
        private const string GetAllUrl = "ToDos?userId={0}";

        /// <summary>
        /// The url for updating a todo.
        /// </summary>
        private const string UpdateUrl = "ToDos";

        /// <summary>
        /// The url for a todo's creation.
        /// </summary>
        private const string CreateUrl = "ToDos";

        /// <summary>
        /// The url for a todo's deletion.
        /// </summary>
        private const string DeleteUrl = "ToDos/{0}";

        private readonly HttpClient httpClient;
        private IItemRepository _itemRepository;
        private MapIdRepository _mapId;
        private static int blankId;
        private static int maxId;
        private static bool toggle;
        private object thisLock = new object();
        /// <summary>
        /// Creates the service.
        /// </summary>
        public ToDoService()
        {
            httpClient = new HttpClient();
            _mapId = new MapIdRepository();
            _itemRepository = new ItemRepository();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Gets all todos for the user.
        /// </summary>
        /// <param name="userId">The User Id.</param>
        /// <returns>The list of todos.</returns>
        public IList<ToDoItemViewModel> GetItems(int userId)
        {
            var itemResult = _itemRepository.GetItems(userId).Select(i => i.ToViewModel()).ToList();

            if (itemResult.Count != 0)
            {
                if (!toggle)
                {
                    maxId = _itemRepository.GetItems().Last().ToDoId;
                    blankId = maxId + 1;
                    toggle = true;
                }

                return itemResult;
            }
            else
            {

                var dataAsString = httpClient.GetStringAsync(string.Format(serviceApiUrl + GetAllUrl, userId)).Result;
                var userViewItems = JsonConvert.DeserializeObject<IList<ToDoItemViewModel>>(dataAsString);
                var items = userViewItems.Select(i => i.ToItem()).ToList();
                if (!toggle)
                {
                    maxId = items.Count == 0 ? 1 : items.Last().ToDoId;
                    blankId = maxId + 1;
                    toggle = true;
                }
                foreach (var elem in items)
                {
                    _itemRepository.Create(elem);
                }
                return userViewItems;
            }

        }

        /// <summary>
        /// Creates a todo. UserId is taken from the model.
        /// </summary>
        /// <param name="item">The todo to create.</param>
        public void CreateItem(ToDoItemViewModel item)
        {
            item.ToDoId = maxId;
            _itemRepository.Create(item.ToItem());


            ThreadPool.QueueUserWorkItem(t =>
            {
                httpClient.PostAsJsonAsync(serviceApiUrl + CreateUrl, item).Result.EnsureSuccessStatusCode();
                MapAzureId(item, maxId++);
            });


        }

        /// <summary>
        /// Updates a todo.
        /// </summary>
        /// <param name="item">The todo to update.</param>
        public void UpdateItem(ToDoItemViewModel item)
        {
            _itemRepository.Update(item.ToItem());

            if (blankId > item.ToDoId)
            {
                ThreadPool.QueueUserWorkItem(t => httpClient.DeleteAsync(string.Format(serviceApiUrl + DeleteUrl, item.ToDoId))
                .Result.EnsureSuccessStatusCode());

            }
            else
            {
                ThreadPool.QueueUserWorkItem(t => httpClient.DeleteAsync(string.Format(serviceApiUrl + DeleteUrl, _mapId.GetAzureId(item.ToDoId)))
                .Result.EnsureSuccessStatusCode());
            }

        }

        /// <summary>
        /// Deletes a todo.
        /// </summary>
        /// <param name="id">The todo Id to delete.</param>
        public void DeleteItem(int id)
        {
            _itemRepository.Delete(id);

            if (blankId > id)
            {
                ThreadPool.QueueUserWorkItem(t => httpClient.DeleteAsync(string.Format(serviceApiUrl + DeleteUrl, id))
                .Result.EnsureSuccessStatusCode());
                _mapId.Delete(id);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(t =>
                {
                    httpClient.DeleteAsync(string.Format(serviceApiUrl + DeleteUrl, _mapId.GetAzureId(id)))
                    .Result.EnsureSuccessStatusCode();
                    _mapId.Delete(id);
                });
            }

        }

        private void MapAzureId(ToDoItemViewModel item, int maxId)
        {
            lock (thisLock)
            {
                var dataAsString = httpClient.GetStringAsync(string.Format(serviceApiUrl + GetAllUrl, item.UserId)).Result;
                var userViewItems = JsonConvert.DeserializeObject<IList<ToDoItemViewModel>>(dataAsString);
                var lastNewIdFromAzure = userViewItems.Last().ToDoId;
                _mapId.Create(new MapId()
                {
                    idAzure = lastNewIdFromAzure,
                    idUi = maxId
                });
            }

        }
    }
}