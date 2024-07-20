using ClassLib;
using Server.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DAL.Sqlite_services
{
	public class User_service : ICrud<User>
	{
		private readonly Services.SqLiteService<User> _service;
		public User_service()
		{
			_service = new Services.SqLiteService<User>();
		}

		public IEnumerable<User> GetAll()
		{
			return _service.GetValues("Users");
		}

		public IEnumerable<User> GetAllById(int id)//не используем
		{
			throw new NotImplementedException();
		}

		public User GetById(int id)
		{
			return _service.GetById("Users", "Id_user", id);
		}

		public User GetByStr(string str)
		{
			return _service.GetByStr("Users", "Phone", str);//у нас логин это телефон
		}

		public void InsertObj(User obj)
		{
			string sql = $"""
INSERT INTO Users (Name, Phone, Address, isActual, Password)
VALUES ('{obj.Name}','{obj.Phone}', '{obj.Address}', {obj.isActual}, '{obj.Password}')
""";
			_service.UpdateAndInsert(sql);
		}

		public void UpdateObj(User obj)
		{
			string sql = $"""
UPDATE Users SET Name ='{obj.Name}', Phone ='{obj.Phone}', Address ='{obj.Address}', isActual ={obj.isActual}, Password ='{obj.Password}' WHERE Id_user ={obj.Id_user}
""";
			_service.UpdateAndInsert(sql);
		}
	}
}
