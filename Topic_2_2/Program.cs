//поддержка кирилицы
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;                              

//гравитация тут как будто не смотрелась
const string president = "Путин";                                   

int userAge = 23;
string userName = "Олег";
string userCite = "Москва";

Console.WriteLine($@"
==============================
* Имя пользователя:{userName}      *
* Возраст пользователя: {userAge}  *
* Город пользователя: {userCite} *
* Президент: {president}           *
==============================
");

Console.ReadKey();