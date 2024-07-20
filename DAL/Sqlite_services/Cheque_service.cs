using ClassLib;
using Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DAL.Sqlite_services
{
    public class Cheque_service : ICrud<Cheque>
    {
        private readonly Services.SqLiteService<Cheque> _service;
        public Cheque_service()
        {
            _service = new Services.SqLiteService<Cheque>();
        }
        public IEnumerable<Cheque> GetAll()
        {
            return _service.GetValues("Cheques");
        }

        public IEnumerable<Cheque> GetAllById(int id)//не нужно
        {
            throw new NotImplementedException();
        }

        public Cheque GetById(int id)
        {
            return _service.GetById("Cheques", "Id_order", id);
        }

        public Cheque GetByStr(string str)//не нужно
        {
            throw new NotImplementedException();
        }

        public void InsertObj(Cheque obj)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string sql = $"""
INSERT INTO Cheques (Date_cheque, Amount, Id_order)
VALUES ('{obj.Date_cheque}', {obj.Amount}, {obj.Id_order})
""";
            _service.UpdateAndInsert(sql);
        }

        public void UpdateObj(Cheque obj)//не нужно
        {
            throw new NotImplementedException();
        }
    }
}
