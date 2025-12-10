using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const double AddFactor = 2;
const double MultiplyFactor = 1.5;
const double Divide = 3;

double first, second, third;

Console.Write("Введите значение A: ");
bool check = double.TryParse(Console.ReadLine(), out first);

Console.Write("Введите значение B: ");
bool check2 = double.TryParse(Console.ReadLine(), out second);

Console.Write("Введите значение C: ");
bool check3 = double.TryParse(Console.ReadLine(), out third);

Console.Clear();

double result1 = (first + second) * AddFactor;
double result2 = second * MultiplyFactor;
double result3 = (first + second + third) / Divide;
double result4 = (second - first) * (third + AddFactor);

Console.WriteLine($@"=====================
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