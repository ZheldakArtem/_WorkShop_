using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IItemRepository 
    {
        void Create(Item e);
        void Update(Item e);
        Item GetById(int key);
        void Delete(int key);
        IEnumerable<Item> GetItems(int userId);
        IEnumerable<Item> GetItems();
    }
}
