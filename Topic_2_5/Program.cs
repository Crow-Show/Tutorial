using System.Text;

Console.OutputEncoding = Encoding.UTF8; 
Console.InputEncoding = Encoding.UTF8;

string? name;
string? cite;
int age;
double number;

Console.Write("Введите своё имя: ");
name = Console.ReadLine();

Console.Write("Введите свой город: ");
cite = Console.ReadLine();

Console.Write("Введите свой возраст: ");
bool result1 = int.TryParse(Console.ReadLine(), out age);           

Console.Write("Введите своё любимое число: ");
bool result2 = double.TryParse(Console.ReadLine(), out number);

Console.Clear();

Console.WriteLine(@"=====================");   //смысла выводить вебратим строкой только рамку никакой,
Console.WriteLine($"Имя: {name}");             //так бы я все ей сделал. зачем болше комманд, когда можно меньше комманд
Console.WriteLine($"Возврат: {age}");
Console.WriteLine($"Число: {number}");
Console.WriteLine($"Город: {cite}");
Console.WriteLine(@"=====================");

Console.ReadKey();