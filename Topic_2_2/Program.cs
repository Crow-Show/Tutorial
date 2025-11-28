using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;                      //подключает кирилицу

const string President = "Путин";                           //мне казалось что константа гравитации тут не смотрится

int userAge = 23;
string userName = "Олег";
string userCite = "Москва";

Console.WriteLine($@"==============================
* Имя пользователя:{userName}      *
* Возвраст пользователя: {userAge}  *
* Город пользователя: {userCite} *
* Президент: {President}           *
==============================");

Console.ReadKey();