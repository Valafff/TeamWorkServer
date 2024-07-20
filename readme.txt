ClassLib - исходники для сборки dll
TestApp - мое консольное приложение для тестов, может пригодится...
При сборке Server по умолчанию пришется пустая база.
Если надо добавить базу с тестовыми записями нужно в папку Data\Clients\ и Data\Dishes\ поместить нужные папки  с Id и добавить в эти папки картинки с соответствующими названиями картинок в БД. Иначе глюканет. Ручками редактировать внимательно!

15.07.2024 - добавлены новые команды  "получить все заказы", "апдейт блюда", "апдейт админа"
    public const string CommandGetAllOrders = "Get_All_Orders";
    public const string AnswerCatchAllOrders = "Catch_All_Orders";
    public const string CommandDishUpdate = "Dish_Update";
    public const string AnswerDishUpdateOK = "Dish_Update_Ok";
    public const string AnswerDishUpdateFailed = "Dish_Update_Failed";
    public const string CommandAdminUpdate = "Admin_Update";
    public const string AnswerAdminUpdateOK = "Admin_Update_OK";
    public const string AnswerAdminUpdateFailed = "Admin_Update_Failed";
15.07.2024 - сообщения сервера теперь на english

16.07.2024 - исправлен баг с одинаковым названием команд: было CommandGetAllOrdersByID = "Get_All_Orders", стало CommandGetAllOrdersByID = "Get_All_Orders_By_Id";


