using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;                              //поддержка кирилицы

const string president = "Путин";                                   //гравитация тут как будто не смотрелась

int userAge = 23;
string userName = "Олег";
string userCite = "Москва";

Console.WriteLine($@"==============================
* Имя пользователя:{userName}      *
* Возвраст пользователя: {userAge}  *
* Город пользователя: {userCite} *
* Президент: {president}           *
==============================");

Console.ReadKey();