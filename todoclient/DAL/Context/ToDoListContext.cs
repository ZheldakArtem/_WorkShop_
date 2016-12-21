using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Entities;
using System.Data.Entity;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;

namespace DAL.Context
{
    public class ToDoListContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        public DbSet<MapId> MapIds { get; set; }

        static ToDoListContext()
        {
        }

        public ToDoListContext()
            : base("name=ToDoListDB")
        {
        }
        

    }
}