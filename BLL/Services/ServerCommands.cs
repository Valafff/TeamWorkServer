using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib;
using Server.DAL.Sqlite_services;
using Server.BLL.Mappers;
using static Dapper.SqlMapper;
using System.Net.Sockets;

namespace Server.BLL.Services
{
    internal class ServerCommands
    {
        Config Config;
        public ServerCommands(Config _conf)
        {
            Config = _conf;
        }

        public bool GetAdminDataToClient(NetworkStream _stream)
        {
            try
            {
                Admin_cafe_service DALservice = new Admin_cafe_service();
                var adminDataDAL = DALservice.GetAll();
                Admin_Cafe_bll adminData = Mappers_bll.MapAdmin_CafeToAdmin_Cafe_bll(adminDataDAL.FirstOrDefault());

                //Запись и чтение файлов идет по путям указанным в конфиге сервера и персональному Id пользователя. Передается только название файла
                //Пример: По умолчанию путь следующий: Папка приложения\Data\Admins\1\ВасяПупкин.jpg, где 1 - Id админа, ВасяПупкин.jpg - Image_name.
                // Папка приложения\Data\Admins\ либо берется по умолчанию, либо настраивается в стартовом конфиге сервера
                adminData.Image_byte = ImageWorks.ImageReader(adminData.Id_admin, Config.AdminsPath, adminData.Image_name);
                Courier courier = new Courier()
                {
                    Header = com.AnswerSetAdminData,
                    Entity = TransportServices.Packer(adminData)
                };
                TransportServices.PackerAndSender(_stream, courier);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка GetAdminDataToClient");
                Console.WriteLine("Error: GetAdminDataToClient");
                return false;
                throw;
            }

        }

        public bool GetAllDishes(NetworkStream _stream)
        {
            try
            {
                Dish_service DALservice = new Dish_service();
                var DALdishes = DALservice.GetAll();

                List<Dish_bll> dishesList = new List<Dish_bll>();
                foreach (var dish in DALdishes)
                {
                    var temp = Mappers_bll.MapDishToDish_bll(dish);
                    temp.Image_byte = ImageWorks.ImageReader(temp.Id_dish, Config.DishesPath, temp.Image_name);
                    dishesList.Add(temp);
                }

                Courier courier = new Courier()
                {
                    Header = com.AnswerSetAllDishes,
                    Entity = TransportServices.Packer(dishesList)
                };
                TransportServices.PackerAndSender(_stream, courier);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка GetAllDishes");
                Console.WriteLine("Error: GetAllDishes");
                return false;
                throw;
            }

        }

        public bool RegistrationUser(NetworkStream _stream, Courier _courier)
        {
            try
            {
                User_bll newUser = TransportServices.Unpacker<User_bll>(_courier.Entity);
                User user = Mappers_bll.MapUser_bllToUser(newUser);
                try
                {
                    User_service DALservice = new User_service();
                    DALservice.InsertObj(user);

                    //регистрация прошла успешна
                    //Console.WriteLine($"Пользователь {user.Phone} зарегистрирован");
                    Console.WriteLine($"User {user.Phone} is registered");
                    newUser = Mappers_bll.MapUserToUser_BLL(DALservice.GetByStr(user.Phone));

                    _courier.Header = com.AnswerRegistrationOK;
                    _courier.Entity = TransportServices.Packer(newUser);
                    TransportServices.PackerAndSender(_stream, _courier);
                    return true;
                }
                catch (Exception)
                {
                    //Регистрация отклонена
                    //Console.WriteLine($"Регистрация пользователя {user.Phone} отклонена");
                    Console.WriteLine($"User  {user.Phone} registration rejected");
                    byte[] bytes = TransportServices.Packer(com.AnswerRegistrationFailed);
                    TransportServices.WriteData(_stream, bytes);
                    return false;
                    throw;
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка регистрации RegistrationUser");
                Console.WriteLine("Registration error: RegistrationUser");
                byte[] bytes = TransportServices.Packer(com.AnswerRegistrationFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        public bool AuthorizationUser(NetworkStream _stream, Courier _courier)
        {
            try
            {
                User_service DALservice = new User_service();
                User_bll user = TransportServices.Unpacker<User_bll>(_courier.Entity);
                User DALuser = DALservice.GetByStr(user.Phone);
                if (user.Password == DALuser.Password)
                {
                    //Авторизация ОК
                    //Console.WriteLine($"Пользователь {user.Phone} авторизовался");
                    Console.WriteLine($"User {user.Phone} authorize");
                    user = Mappers_bll.MapUserToUser_BLL(DALuser);

                    _courier.Header = com.AnswerAuthorizeOK;
                    _courier.Entity = TransportServices.Packer(user);
                    TransportServices.PackerAndSender(_stream, _courier);
                    return true;
                }
                else
                {
                    //Авторизация отклонена
                    //Console.WriteLine($"Авторизация пользователя {user.Phone} отклонена");
                    Console.WriteLine($"User {user.Phone} authorization denied");
                    byte[] bytes = TransportServices.Packer(com.AnswerAuthorizeFailed);
                    TransportServices.WriteData(_stream, bytes);
                    return false;
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка авторизации AuthorizationUser");
                Console.WriteLine("Authorize error: AuthorizationUser");
                byte[] bytes = TransportServices.Packer(com.AnswerAuthorizeFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }

        }

        public bool GetAllOrdersById(NetworkStream _stream, Courier _courier)
        {
            try
            {
                User_bll user = TransportServices.Unpacker<User_bll>(_courier.Entity);
                Order_service DALservice = new Order_service();
                List<Order> ordersList = DALservice.GetAllById(user.Id_user).ToList();
                List<Order_bll> BLLorders = new List<Order_bll>();
                foreach (Order order in ordersList)
                {
                    var temp = Mappers_bll.MapOrderToOrder_bll(order);
                    BLLorders.Add(temp);
                }
                _courier.Header = com.AnswerCatchOrdersListById;
                _courier.Entity = TransportServices.Packer(BLLorders);
                TransportServices.PackerAndSender(_stream, _courier);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка получения заказов GetAllOrdersById");
                Console.WriteLine("Receiving order error: GetAllOrdersById");
                return false;
                throw;
            }

        }

        public bool TakeOrder(NetworkStream _stream, Courier _courier)
        {
            try
            {
                Order_bll newOrder = TransportServices.Unpacker<Order_bll>(_courier.Entity);
                Order_service DALservice = new Order_service();
                DALservice.InsertObj(Mappers_bll.MapOrder_bllToOrder(newOrder));

                //Console.WriteLine($"Заказ пользователя {newOrder.Id_user} принят {DateTime.Now}");
                Console.WriteLine($"User order  {newOrder.Id_user} accepted {DateTime.Now}");
                byte[] bytes = TransportServices.Packer(com.AnswerOrderAccepted);
                TransportServices.WriteData(_stream, bytes);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine($"Ошибка формирования заказа {DateTime.Now}");
                Console.WriteLine($"Order formation error: {DateTime.Now}");
                byte[] bytes = TransportServices.Packer(com.AnswerOrderFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        public bool OrderPayment(NetworkStream _stream, Courier _courier)
        {
            try
            {
                int orderId = TransportServices.Unpacker<int>(_courier.Entity);
                Order_service DALservice = new Order_service();
                Order DALorder = DALservice.GetById(orderId);
                DALorder.isPaid = 1;
                DALservice.UpdateObj(DALorder);

                //Console.WriteLine($"Заказ № {orderId} пользователя {DALorder.Id_user} оплачен");
                Console.WriteLine($"Order № {orderId} of user  {DALorder.Id_user} is paid");
                byte[] bytes = TransportServices.Packer(com.AnswerPaidOK);
                TransportServices.WriteData(_stream, bytes);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine($"Ошибка оплаты заказа");
                Console.WriteLine($"Order payment error");
                byte[] bytes = TransportServices.Packer(com.AnswerPaidFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        public bool AdminRegistration(NetworkStream _stream, Courier _courier)
        {
            try
            {
                Admin_Cafe_bll BLLadmin = TransportServices.Unpacker<Admin_Cafe_bll>(_courier.Entity);
                byte[] imageByte = BLLadmin.Image_byte;
                Admin_Cafe DALadmin = Mappers_bll.MapAdmin_Cafe_bllToAdmin_Cafe(BLLadmin);
                try
                {
                    Admin_cafe_service service = new Admin_cafe_service();
                    service.InsertObj(DALadmin);
                    BLLadmin = Mappers_bll.MapAdmin_CafeToAdmin_Cafe_bll(service.GetByStr(DALadmin.Login));
                    BLLadmin.Image_byte = imageByte;
                    //Пишем картинку в папку с Id администратора
                    ImageWorks.ImageWriter(BLLadmin.Id_admin, Config.AdminsPath, BLLadmin.Image_name, imageByte);
                    //регистрация прошла успешна
                    //Console.WriteLine($"Администратор {BLLadmin.Login} зарегистрирован");
                    Console.WriteLine($"Administrator  {BLLadmin.Login} is registered");

                    _courier.Header = com.AnswerAdminRegistrationOK;
                    _courier.Entity = TransportServices.Packer(BLLadmin);
                    TransportServices.PackerAndSender(_stream, _courier);
                    return true;
                }
                catch (Exception)
                {
                    //Регистрация отклонена
                    //Console.WriteLine($"Регистрация админа {BLLadmin.Login} отклонена");
                    Console.WriteLine($"Administrator registration  {BLLadmin.Login} rejected");
                    byte[] bytes = TransportServices.Packer(com.AnswerAdminRegistrationFailed);
                    TransportServices.WriteData(_stream, bytes);
                    return false;
                    throw;
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка регистрации AdminRegistration");
                Console.WriteLine("Registration error: AdminRegistration");
                byte[] bytes = TransportServices.Packer(com.AnswerAdminRegistrationFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        public bool AdminAuthorization(NetworkStream _stream, Courier _courier)
        {
            try
            {
                Admin_Cafe_bll BLLadmin = TransportServices.Unpacker<Admin_Cafe_bll>(_courier.Entity);
                Admin_cafe_service service = new Admin_cafe_service();
                Admin_Cafe DALadmin = service.GetByStr(BLLadmin.Login);
                //Авторизация успешна
                if (DALadmin.Password == BLLadmin.Password)
                {
                    BLLadmin = Mappers_bll.MapAdmin_CafeToAdmin_Cafe_bll(DALadmin);
                    BLLadmin.Image_byte = ImageWorks.ImageReader(BLLadmin.Id_admin, Config.AdminsPath, BLLadmin.Image_name);
                    //Console.WriteLine($"Авторизация администратора {BLLadmin.Login} прошла успешно");
                    Console.WriteLine($"Authorization of the administrator {BLLadmin.Login} was successful");

                    _courier.Header = com.AnswerAdminAuthorizationOK;
                    _courier.Entity = TransportServices.Packer(BLLadmin);
                    TransportServices.PackerAndSender(_stream, _courier);
                    return true;
                }
                //Авторизация отклонена
                else
                {
                    //Console.WriteLine($"Попытка авторизации администратора {BLLadmin.Login} закончилась неудачно");
                    Console.WriteLine($"Administrator {BLLadmin.Login} login attempt failed");
                    byte[] bytes = TransportServices.Packer(com.AnswerAdminAuthorizationFailed);
                    TransportServices.WriteData(_stream, bytes);
                    return false;
                }
            }
            catch (Exception)
            {
                //Console.WriteLine($"Ошибка авторизации AdminAuthorization");
                Console.WriteLine($"Authorize error: AdminAuthorization");
                byte[] bytes = TransportServices.Packer(com.AnswerAdminAuthorizationFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        public bool GetAllUsers(NetworkStream _stream)
        {
            try
            {
                User_service service = new User_service();
                List<User_bll> usersList = new List<User_bll>();
                var DALusers = service.GetAll().ToList();
                foreach (var user in DALusers)
                {
                    User_bll tempUser = Mappers_bll.MapUserToUser_BLL(user);
                    usersList.Add(tempUser);
                }

                Courier courier = new Courier()
                {
                    Header = com.AnswerCatchAllUsers,
                    Entity = TransportServices.Packer(usersList)
                };
                TransportServices.PackerAndSender(_stream, courier);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка GetAllUsers");
                Console.WriteLine("Error: GetAllUsers");
                return false;
                throw;
            }
        }

        public bool OrderClose(NetworkStream _stream, Courier _courier)
        {
            try
            {
                Order_bll BLLorder = TransportServices.Unpacker<Order_bll>(_courier.Entity);
                Order_service service = new Order_service();
                Order DALorder = service.GetById(BLLorder.Id_order);
                DALorder.isActual = 0;
                service.UpdateObj(DALorder);

                //Заказ закрыт
                //Console.WriteLine($"Заказ {DALorder.Id_order} закрыт {DateTime.Now}");
                Console.WriteLine($"Order {DALorder.Id_order} is closed {DateTime.Now}");
                byte[] bytes = TransportServices.Packer(com.AnswerOrderClosedOK);
                TransportServices.WriteData(_stream, bytes);
                return true;
            }
            catch (Exception)
            {
                //Не удалось закрыть заказ
                //Console.WriteLine("Ошибка закрытия заказа OrderClose");
                Console.WriteLine("Order error: OrderClose");
                byte[] bytes = TransportServices.Packer(com.AnswerOrderClosedFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        public bool AddNewDish(NetworkStream _stream, Courier _courier)
        {
            try
            {
                Dish_bll BLLdish = TransportServices.Unpacker<Dish_bll>(_courier.Entity);
                Dish_service service = new Dish_service();
                service.InsertObj(Mappers_bll.MapDish_bllToDish(BLLdish));
                var tempDish = service.GetByStr(BLLdish.Name_dish);
                ImageWorks.ImageWriter(tempDish.Id_dish, Config.DishesPath, BLLdish.Image_name, BLLdish.Image_byte);

                //Console.WriteLine($"Добавлено новое блюдо {BLLdish.Name_dish} {DateTime.Now}");
                Console.WriteLine($"New dish added {BLLdish.Name_dish} {DateTime.Now}");
                byte[] bytes = TransportServices.Packer(com.AnswerNewDishInsertedOK);
                TransportServices.WriteData(_stream, bytes);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка добавления блюда AddNewDish");
                Console.WriteLine("Dish error: AddNewDish");
                byte[] bytes = TransportServices.Packer(com.AnswerNewDishInsertedFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        //Изменения 15.07.2024 Добавление новых команд "получить все заказы", "апдейт блюда", "апдейт админа"
        public bool GetAllOrders(NetworkStream _stream)
        {
            try
            {
                Order_service DALservice = new Order_service();
                List<Order> ordersList = DALservice.GetAll().ToList();
                List<Order_bll> BLLorders = new List<Order_bll>();
                foreach (Order order in ordersList)
                {
                    var temp = Mappers_bll.MapOrderToOrder_bll(order);
                    BLLorders.Add(temp);
                }
                Courier courier = new Courier()
                {
                    Header = com.AnswerCatchAllOrders,
                    Entity = TransportServices.Packer(BLLorders)
                };
                TransportServices.PackerAndSender(_stream, courier);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка получения заказов CommandGetAllOrders");
                Console.WriteLine("Dish error: CommandGetAllOrders");
                return false;
                throw;
            }
        }

        //Id блюда передается тоже
        public bool DishUpdate(NetworkStream _stream, Courier _courier)
        {
            try
            {
                Dish_bll BLLdish = TransportServices.Unpacker<Dish_bll>(_courier.Entity);
                Dish_service service = new Dish_service();
                service.UpdateObj(Mappers_bll.MapDish_bllToDish(BLLdish)); ;
                ImageWorks.ImageWriter(BLLdish.Id_dish, Config.DishesPath, BLLdish.Image_name, BLLdish.Image_byte);

                //Console.WriteLine($"Блюдо {BLLdish.Name_dish} обновлено {DateTime.Now}");
                Console.WriteLine($"Dish {BLLdish.Name_dish} updated {DateTime.Now}");
                byte[] bytes = TransportServices.Packer(com.AnswerDishUpdateOK);
                TransportServices.WriteData(_stream, bytes);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка обновления блюда DishUpdate");
                Console.WriteLine("Dish error: DishUpdate");
                byte[] bytes = TransportServices.Packer(com.AnswerDishUpdateFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }

        //Id админа передается тоже
        public bool AdminUpdate(NetworkStream _stream, Courier _courier)
        {
            try
            {
                Admin_Cafe_bll BLLadmin = TransportServices.Unpacker<Admin_Cafe_bll>(_courier.Entity);
                Admin_Cafe DALadmin = Mappers_bll.MapAdmin_Cafe_bllToAdmin_Cafe(BLLadmin);
                try
                {
                    Admin_cafe_service service = new Admin_cafe_service();
                    service.UpdateObj(DALadmin);
                    //Пишем картинку в папку с Id администратора
                    ImageWorks.ImageWriter(BLLadmin.Id_admin, Config.AdminsPath, BLLadmin.Image_name, BLLadmin.Image_byte);
                    //Console.WriteLine($"Администратор {BLLadmin.Login} обновлен");
                    Console.WriteLine($"Administrator {BLLadmin.Login} updated");

                    _courier.Header = com.AnswerAdminUpdateOK;
                    TransportServices.PackerAndSender(_stream, _courier);
                    return true;
                }
                catch (Exception)
                {
                    //Console.WriteLine($"Ошибка обновления админа {BLLadmin.Login} \t {DateTime.Now}");
                    Console.WriteLine($"Admin error: {BLLadmin.Login} \t {DateTime.Now}");
                    byte[] bytes = TransportServices.Packer(com.AnswerAdminUpdateFailed);
                    TransportServices.WriteData(_stream, bytes);
                    return false;
                    throw;
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Ошибка обновления AdminUpdate");
                Console.WriteLine("Admin error: AdminUpdate");
                byte[] bytes = TransportServices.Packer(com.AnswerAdminUpdateFailed);
                TransportServices.WriteData(_stream, bytes);
                return false;
                throw;
            }
        }
    }
}
