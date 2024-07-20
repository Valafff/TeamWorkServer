using ClassLib;
using Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DAL.Sqlite_services
{
    public class Order_service : ICrud<Order>
    {
        private readonly Services.SqLiteService<Order> _service;
        public Order_service()
        {
            _service = new Services.SqLiteService<Order>();
        }
        public IEnumerable<Order> GetAll()
        {
            return _service.GetValues("Orders");
        }

        public IEnumerable<Order> GetAllById(int id)
        {
            return _service.GetValuesById("Orders", "Id_user", id);
        }

        public Order GetById(int id)//мало ли вдруг нужно будет
        {
            return _service.GetById("Orders", "Id_order", id);
        }

        public Order GetByStr(string str)//не используем
        {
            throw new NotImplementedException();
        }

        public void InsertObj(Order obj)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string sql = $"""
INSERT INTO Orders (Name_dishes, Date_order, Date_delivery, Cost, isActual, isPaid, Id_user)
VALUES ('{obj.Name_dishes}','{obj.Date_order}', '{obj.Date_delivery}', {obj.Cost}, {obj.isActual}, {obj.isPaid}, {obj.Id_user})
""";
            _service.UpdateAndInsert(sql);
        }

        public void UpdateObj(Order obj)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string sql = $"""
UPDATE Orders SET Name_dishes ='{obj.Name_dishes}', Date_order ='{obj.Date_order}', Date_delivery ='{obj.Date_delivery}', 
Cost ={obj.Cost}, isActual ={obj.isActual}, isPaid ={obj.isPaid}, Id_user = {obj.Id_user}  WHERE Id_order ={obj.Id_order}
""";
            _service.UpdateAndInsert(sql);
        }
    }
}
