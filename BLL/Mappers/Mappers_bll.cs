using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.BLL.Mappers
{
	public class Mappers_bll
	{
		////Путь задается программно на сервере соответствует Id админа в папке Admins, Id клиента в папке Clients, Id блюда в папке Dishes 
		//public static string path = Directory.GetCurrentDirectory() + "Folder\\";//путь к папке если она в бин дебаг
		public static ClassLib.User_bll MapUserToUser_BLL(ClassLib.User user)
		{
			return new ClassLib.User_bll()
			{
				Id_user = user.Id_user,
				Name = user.Name,
				Password = user.Password,
				Phone = user.Phone,
				Address = user.Address,
				isActual = user.isActual
			};
		}

		public static ClassLib.User MapUser_bllToUser(ClassLib.User_bll user_bll)
		{
			return new ClassLib.User()
			{
				Id_user = user_bll.Id_user,//
				Name = user_bll.Name,
				Password = user_bll.Password,
				Phone = user_bll.Phone,
				Address = user_bll.Address,
				isActual = user_bll.isActual
			};
		}

		////Старая редакция
		//public static ClassLib.Dish_bll MapDishToDish_bll(ClassLib.Dish dish)
		//      {
		//          return new ClassLib.Dish_bll()
		//          {
		//              Id_dish = dish.Id_dish,
		//              Name_dish = dish.Name_dish,
		//              Price = dish.Price,
		//              Image_byte = File.ReadAllBytes(dish.Image_path),
		//              Image_name = Path.GetFileName(dish.Image_path),
		//              Description = dish.Description,
		//              isActual = dish.isActual,
		//          };
		//      }

		public static ClassLib.Dish_bll MapDishToDish_bll(ClassLib.Dish dish)
		{
			return new ClassLib.Dish_bll()
			{
				Id_dish = dish.Id_dish,
				Name_dish = dish.Name_dish,
				Price = dish.Price,
				Image_name = dish.Image_path,
				Description = dish.Description,
				isActual = dish.isActual,
			};
		}

		////Старая редакция
		//public static ClassLib.Dish MapDish_bllToDish(ClassLib.Dish_bll dish_bll)
		//{
		//	return new ClassLib.Dish()
		//	{
		//		Id_dish = dish_bll.Id_dish,
		//		Name_dish = dish_bll.Name_dish,
		//		Price = dish_bll.Price,
		//		Image_path = path + dish_bll.Image_name,
		//		Description = dish_bll.Description,
		//		isActual = dish_bll.isActual
		//	};
		//}


		public static ClassLib.Dish MapDish_bllToDish(ClassLib.Dish_bll dish_bll)
		{
			try
			{
				return new ClassLib.Dish()
				{
					Id_dish = dish_bll.Id_dish,
					Name_dish = dish_bll.Name_dish,
					Price = dish_bll.Price,
					Image_path = dish_bll.Image_name,
					Description = dish_bll.Description,
					isActual = dish_bll.isActual
				};
			}
			catch (Exception)
			{
				Console.WriteLine("Ошибка мапирования MapDish_bllToDish");
				throw;
			}

		}

		public static ClassLib.Order_bll MapOrderToOrder_bll(ClassLib.Order order)
		{
			string isPaid_temp, isActual_temp;
			if (order.isPaid == 0) isPaid_temp = "Не оплачен";
			else isPaid_temp = "Оплачен";

			if (order.isActual == 0) isActual_temp = "Готов";
			else isActual_temp = "Не готов";

			return new ClassLib.Order_bll()
			{
				Id_order = order.Id_order,
				Name_dishes = order.Name_dishes,
				Date_order = DateTime.Parse(order.Date_order),
				Date_delivery = DateTime.Parse(order.Date_delivery),
				Cost = order.Cost,
				isActual = isActual_temp,
				isPaid = isPaid_temp,
				Id_user = order.Id_user
			};
		}
		public static ClassLib.Order MapOrder_bllToOrder(ClassLib.Order_bll order_bll)
		{
			int isPaid_temp, isActual_temp;
			if (order_bll.isPaid == "Не оплачен") isPaid_temp = 0;
			else isPaid_temp = 1;

			if (order_bll.isActual == "Готов") isActual_temp = 0;
			else isActual_temp = 1;

			return new ClassLib.Order()
			{
				Id_order = order_bll.Id_order,
				Name_dishes = order_bll.Name_dishes,
				Date_order = order_bll.Date_order.ToString(),
				Date_delivery = order_bll.Date_delivery.ToString(),
				Cost = order_bll.Cost,
				isActual = isActual_temp,
				isPaid = isPaid_temp,
				Id_user = order_bll.Id_user
			};
		}

		////Старая редакция
		//public static ClassLib.Admin_Cafe_bll MapAdmin_CafeToAdmin_Cafe_bll(ClassLib.Admin_Cafe admin)
		//{
		//    return new ClassLib.Admin_Cafe_bll()
		//    {
		//        Id_admin = admin.Id_admin,
		//        Login = admin.Login,
		//        Password = admin.Password,
		//        Image_byte = File.ReadAllBytes(admin.Image_path),
		//        Image_name = Path.GetFileName(admin.Image_path),
		//        Name_cafe = admin.Name_cafe,
		//        Phone = admin.Phone,
		//        Address = admin.Address
		//    };
		//}

		public static ClassLib.Admin_Cafe_bll MapAdmin_CafeToAdmin_Cafe_bll(ClassLib.Admin_Cafe admin)
		{
			try
			{
				return new ClassLib.Admin_Cafe_bll()
				{
					Id_admin = admin.Id_admin,
					Login = admin.Login,
					Password = admin.Password,
					Image_name = admin.Image_path,
					Name_cafe = admin.Name_cafe,
					Phone = admin.Phone,
					Address = admin.Address
				};
			}
			catch (Exception)
			{
                Console.WriteLine("Ошибка мапирования MapAdmin_CafeToAdmin_Cafe_bll");
                throw;
			}
			
		}

		////Старая редакция
		//public static ClassLib.Admin_Cafe MapAdmin_Cafe_bllToAdmin_Cafe(ClassLib.Admin_Cafe_bll admin_bll)
		//      {
		//          return new ClassLib.Admin_Cafe()
		//          {
		//              Id_admin = admin_bll.Id_admin,
		//              Login = admin_bll.Login,
		//              Password = admin_bll.Password,
		//              Image_path = path + admin_bll.Image_name,
		//              Name_cafe = admin_bll.Name_cafe,
		//              Phone = admin_bll.Phone,
		//              Address = admin_bll.Address
		//          };
		//      }

		public static ClassLib.Admin_Cafe MapAdmin_Cafe_bllToAdmin_Cafe(ClassLib.Admin_Cafe_bll admin_bll)
		{
			return new ClassLib.Admin_Cafe()
			{
				Id_admin = admin_bll.Id_admin,
				Login = admin_bll.Login,
				Password = admin_bll.Password,
				Image_path = admin_bll.Image_name,
				Name_cafe = admin_bll.Name_cafe,
				Phone = admin_bll.Phone,
				Address = admin_bll.Address
			};
		}

		public static ClassLib.Cheque_bll MapChequeToCheque_bll(ClassLib.Cheque cheque)
		{
			return new ClassLib.Cheque_bll()
			{
				Id_cheque = cheque.Id_cheque,
				Amount = cheque.Amount,
				Date_cheque = DateTime.Parse(cheque.Date_cheque),
				Id_order = cheque.Id_order
			};
		}

		public static ClassLib.Cheque MapCheque_bllToCheque(ClassLib.Cheque_bll cheque_bll)
		{
			return new ClassLib.Cheque()
			{
				Id_cheque = cheque_bll.Id_cheque,
				Amount = cheque_bll.Amount,
				Date_cheque = cheque_bll.Date_cheque.ToString(),
				Id_order = cheque_bll.Id_order
			};
		}
	}
}
