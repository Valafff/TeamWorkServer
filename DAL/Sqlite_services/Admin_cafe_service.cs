using ClassLib;
using Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DAL.Sqlite_services
{
    public class Admin_cafe_service : ICrud<Admin_Cafe>
    {
        private readonly Services.SqLiteService<Admin_Cafe> _service;
        public Admin_cafe_service()
        {
            _service = new Services.SqLiteService<Admin_Cafe>();
        }
        public IEnumerable<Admin_Cafe> GetAll()//не нужно но сделаю на всякий
        {
            return _service.GetValues("Admin_Cafe");
        }

        public IEnumerable<Admin_Cafe> GetAllById(int id)//не нужно
        {
            throw new NotImplementedException();
        }

        public Admin_Cafe GetById(int id)
        {
            return _service.GetById("Admin_Cafe", "Id_admin", id);
        }

        public Admin_Cafe GetByStr(string str)//возвращаем по логину регистрация/авторизация
        {
           return _service.GetByStr("Admin_Cafe", "Login", str);
        }

        public void InsertObj(Admin_Cafe obj)
        {
            string sql = $"""
INSERT INTO Admin_Cafe (Login, Password, Image_path, Name_cafe, Phone, Address)
VALUES ('{obj.Login}','{obj.Password}', '{obj.Image_path}', '{obj.Name_cafe}', '{obj.Phone}', '{obj.Address}')
""";
            _service.UpdateAndInsert(sql);
        }

        public void UpdateObj(Admin_Cafe obj)
        {
            string sql = $"""
UPDATE Admin_Cafe SET Login ='{obj.Login}', Password ='{obj.Password}', Image_path ='{obj.Image_path}', Name_cafe ='{obj.Name_cafe}',
Phone ='{obj.Phone}', Address ='{obj.Address}' WHERE Id_admin ={obj.Id_admin}
""";
            _service.UpdateAndInsert(sql);
        }
    }
}
