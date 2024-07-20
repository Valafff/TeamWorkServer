using ClassLib;
using Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DAL.Sqlite_services
{
    public class Dish_service : ICrud<Dish>
    {
        private readonly Services.SqLiteService<Dish> _service;
        public Dish_service()
        {
            _service = new Services.SqLiteService<Dish>();
        }
        public IEnumerable<Dish> GetAll()
        {
            return _service.GetValues("Dishes");
        }

        public IEnumerable<Dish> GetAllById(int id)//не нужно
        {
            throw new NotImplementedException();
        }

        public Dish GetById(int id)//вдруг нужно будет
        {
            return _service.GetById("Dishes", "Id_dish", id);
        }

        public Dish GetByStr(string str)
        {
			return _service.GetByStr("Dishes", "Name_dish", str);
		}

        public void InsertObj(Dish obj)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string sql = $"""
INSERT INTO Dishes (Name_dish, Price, Image_path, Description, isActual)
VALUES ('{obj.Name_dish}', {obj.Price}, '{obj.Image_path}', '{obj.Description}', {obj.isActual})
""";
            _service.UpdateAndInsert(sql);
        }

        public void UpdateObj(Dish obj)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string sql = $"""
UPDATE Dishes SET Name_dish ='{obj.Name_dish}', Price ={obj.Price}, Image_path ='{obj.Image_path}',
Description ='{obj.Description}', isActual ={obj.isActual} WHERE Id_dish ={obj.Id_dish}
""";
            _service.UpdateAndInsert(sql);
        }
    }
}
