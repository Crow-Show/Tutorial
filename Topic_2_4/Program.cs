using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const double addFactor = 2;
const double multiplyFactor = 1.5;
const double divide = 3;

double first, second, third;

Console.Write("Введите значение A: ");
double.TryParse(Console.ReadLine(), out first);

Console.Write("Введите значение B: ");
double.TryParse(Console.ReadLine(), out second);

Console.Write("Введите значение C: ");
double.TryParse(Console.ReadLine(), out third);

Console.Clear();

double result1 = (first + second) * addFactor;
double result2 = second * multiplyFactor;
double result3 = (first + second + third) / divide;
double result4 = (second - first) * (third + addFactor);

Console.WriteLine($@"
=====================
Вводные значения:
A: {first}
B: {second}
C: {third}

Результат вычислений:
1: {result1}
2: {result2}
3: {result3}
4: {result4}
=====================");

Console.ReadKey();