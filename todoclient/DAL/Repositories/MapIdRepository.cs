using DAL.Context;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MapIdRepository
    {
        public static int lastAthureId;

        static MapIdRepository()
        {
            lastAthureId = 0;
        }

        private readonly ToDoListContext _context;

        public MapIdRepository()
        {
            _context = new ToDoListContext();
        }

        public void Create(MapId item)
        {
            if (ReferenceEquals(item, null))
                throw new ArgumentNullException();

            _context.MapIds.Add(item);
            _context.SaveChanges();

        }

        public void Delete(int id)
        {
            var elem = _context.MapIds.FirstOrDefault(i => i.idUi == id);
            if (!ReferenceEquals(elem, null))
            {
                _context.MapIds.Remove(elem);
                _context.SaveChanges();
              
            }
        }
        public int GetAzureId(int idUi)
        {
            var result = _context.MapIds.FirstOrDefault(i => i.idUi == idUi);

            return result.idAzure;
        }

    }
}
