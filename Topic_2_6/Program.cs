using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.Write("Введите число секунд: ");            // уточнение про секунды подразумевает ввод положительного числа
bool result = uint.TryParse(Console.ReadLine(), out uint input);        // uint для отсечения отрицательных чисел

uint day = input / 86400;
uint hour = (input % 86400) / 3600;
uint minute = (input % 3600) / 60;
uint second = input % 60;

Console.Clear();

Console.WriteLine($@"Исходное время: {input} секунд

Итоговое время:
Дни: {day}
Часы:{hour}
Минуты: {minute}
Секунды: {second}");

Console.ReadKey();