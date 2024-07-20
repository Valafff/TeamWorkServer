using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DAL.Interfaces
{
    public interface ICrud<T>
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllById(int id);
        T GetById(int id);
        T GetByStr(string str);
        void InsertObj(T obj);
        void UpdateObj(T obj);
    }
}
