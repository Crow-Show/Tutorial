using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.Write("Введите положительное число для кодировки цветов: ");

bool result = uint.TryParse(Console.ReadLine(), out uint color);

uint blue = color & 0xFF;
uint green = (color >> 8) & 0xFF;
uint red = (color >> 16) & 0xFF;

Console.Clear();
Console.WriteLine($@"Исходное число: {color}

Раскодировка на цвета:
Синий: {blue}
Зеленый: {green}
Красный: {red}");

Console.ReadKey();