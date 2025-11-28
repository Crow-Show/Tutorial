using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string name = "Олег";
int age = 18;
char initial = 'О';
bool isRegistred = true;
const double PI = 3.14159;

Console.WriteLine($@"---------------------
Имя: {name}
Возраст: {age}
Инициал: {initial}
Зарегистрирован: {isRegistred}
Число PI: {PI}
---------------------");

Console.ReadKey();