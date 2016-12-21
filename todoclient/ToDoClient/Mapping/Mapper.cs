using DAL.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToDoClient.Models;

namespace todoclient.Mapping
{
    public static class Mapper
    {

        public static Item ToItem(this ToDoItemViewModel toDoItemViewModel)
        {
            if (ReferenceEquals(toDoItemViewModel, null))
                return null;

            return new Item()
            {
                ToDoId = toDoItemViewModel.ToDoId,
                Name = toDoItemViewModel.Name,
                IsCompleted = toDoItemViewModel.IsCompleted,
                UserId = toDoItemViewModel.UserId
            };

        }

        public static ToDoItemViewModel ToViewModel(this Item toDoItem)
        {
            if (ReferenceEquals(toDoItem, null))
                return null;

            return new ToDoItemViewModel()
            {
                ToDoId = toDoItem.ToDoId,
                Name = toDoItem.Name,
                IsCompleted = toDoItem.IsCompleted,
                UserId = toDoItem.UserId
            };

        }
    }
}