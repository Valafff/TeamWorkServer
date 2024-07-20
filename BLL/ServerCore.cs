using ClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.BLL
{
    public delegate void MessageFromServer(string s);
    internal class ServerCore
	{
        public event MessageFromServer MessageFromServerEvent;
        //Время проверки активных клиентов
        const int delayActiveClients = 10;

		TcpListener tcpListener;
		Config Config;
		public ServerCore(Config _config)
		{
			Config = _config;
			tcpListener = new TcpListener(IPAddress.Any, Config.ServerPort);
		}
		public void StartServer()
		{
			try
			{
				tcpListener.Start();
                //Console.WriteLine("Сервер запущен");
                Console.WriteLine($"Server is running... Port: {Config.ServerPort}\t {DateTime.Now}");
				MessageFromServerEvent($"Server is running... Port: {Config.ServerPort}");

                while (true)
				{
					//Ждем входящее подключение
					TcpClient tcpClient = tcpListener.AcceptTcpClient();
					//Создаем новое подключение для клиента в отдельном потоке
					_ = Task.Factory.StartNew(() => ProcessClientAsync(tcpClient), TaskCreationOptions.LongRunning);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
                //Console.WriteLine("Ошибка сервера");
                Console.WriteLine("Server error");
            }
			finally
			{
                //Console.WriteLine("Сервер выключен");
                Console.WriteLine("Server is turned off");
                MessageFromServerEvent($"Server stopped");
                tcpListener.Stop();
			}
        }

		async Task ProcessClientAsync(TcpClient tcpClient)
		{
			NetworkStream stream = tcpClient.GetStream();
			
			while (true)
			{
				Courier courier = TransportServices.ReciverAndUnpacker(stream);
				Services.ServerCommands command = new Services.ServerCommands(Config);

				if (courier.Header == com.CommandGetAdminData)
				{
					if (command.GetAdminDataToClient(stream))
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} recive admin data");
					}
					else
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} did not receive admin data");
					}

				}
				else if (courier.Header == com.CommandGetDishes)
				{
					if (command.GetAllDishes(stream))
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} recive diches");
					}
					else
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} did not recive diches");
					}
				}
				else if (courier.Header == com.CommandRegisterMe)
				{
					if (command.RegistrationUser(stream, courier))
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} success registered");
					}
					else
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} registration failed");
					}
				}
				else if (courier.Header == com.CommandAuthorizeMe)
				{
					if (command.AuthorizationUser(stream, courier))
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} success authrized");
					}
					else
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} authorization failed");
					}
				}
				else if (courier.Header == com.CommandGetAllOrdersByID)
				{
					if (command.GetAllOrdersById(stream, courier))
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} recive all orders by ID");
					}
					else
					{
						MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} did not recive all orders by ID");
					}
				}
				else if (courier.Header == com.CommandTakeMyOrder)
				{
					if (command.TakeOrder(stream, courier))
					{
                        MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} order was accepted ");
                    }
					else
					{
                        MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} order was rejected ");
                    }
				}
				else if (courier.Header == com.CommandShutUpAndTakeMyMoney)
				{
					if (command.OrderPayment(stream, courier))
					{
                        MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} order has been paid");
                    }
					else
					{
                        MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} order has not been paid");
                    }
				}
				else if (courier.Header == com.CommandAdminRegistration)
				{
					if(command.AdminRegistration(stream, courier))
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} success registered");
                    }
					else
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} registration failed");
                    }
				}
				else if (courier.Header == com.CommandAdminAuthorization)
				{
					if(command.AdminAuthorization(stream, courier))
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} success authrized");
                    }
					else
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} authorization failed");
                    }
				}
				else if (courier.Header == com.CommandGetAllUsers)
				{
					if(command.GetAllUsers(stream))
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} recive all users");
                    }
					else
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} did not recive all users");
                    }
				}
				else if (courier.Header == com.CommandOrderClosed)
				{
					if (command.OrderClose(stream, courier))
					{
                        MessageFromServerEvent($"Order is closed on {tcpClient.Client.RemoteEndPoint}");
                    }
					else
					{
                        MessageFromServerEvent($"Order did not closed on {tcpClient.Client.RemoteEndPoint}");
                    }
				}
				else if (courier.Header == com.CommandAddNewDish)
				{
					if (command.AddNewDish(stream, courier))
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} added new dish successfully");
                    }
					else
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} added new dish rejected");
                    }
				}
                //Изменения 15.07.2024 Добавление новых команд "получить все заказы", "апдейт блюда", "апдейт админа"
				else if (courier.Header == com.CommandGetAllOrders)
				{
					if (command.GetAllOrders(stream))
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} recive all orders");
                    }
					else
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} did not recive all orders");
                    }	
                }
				else if (courier.Header == com.CommandDishUpdate)
				{
					if (command.DishUpdate(stream, courier))
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} updated dish successfully");
                    }
					else
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} updated dish was rejected");
                    }
                }
				else if (courier.Header == com.CommandAdminUpdate)
				{
					if(command.AdminUpdate(stream, courier))
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} updated admin data successfully");
                    }
					else
					{
                        MessageFromServerEvent($"Admin {tcpClient.Client.RemoteEndPoint} updated admin data was rejected");
                    }
                }
                else
                {
                    //await Console.Out.WriteLineAsync($"Клиент {tcpClient.Client.RemoteEndPoint} выполнил недопустимую операцию: {courier.Header}. Связь разорвана\t{DateTime.Now}");
                    await Console.Out.WriteLineAsync($"Client {tcpClient.Client.RemoteEndPoint} request: {courier.Header}. Сonnection closed\t{DateTime.Now}");
                    MessageFromServerEvent($"Client {tcpClient.Client.RemoteEndPoint} request: {courier.Header}. Сonnection closed");
                    tcpClient.Close();
					break;
				}
			}
		}
	}
}
