using System.Reflection.Metadata.Ecma335;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const double EconomyMode = 0.7;
const double NormalMode = 1.0;
const double MaxMode = 1.3;
const double loadFactor = 1.15;

// --- Запрос энергопотребления ---
Console.Write("Введи базовое энергопотребление в ватт-часах: ");
string? energyInput = Console.ReadLine();

if (!uint.TryParse(energyInput, out uint baseEnergy))
{
    Console.Clear();
    Console.WriteLine("Некорректный ввод. Устнановлено стадартное потебление в 1000 ватт-час");
    baseEnergy = 1000;
    
    Console.Write("Нажмите любую клавишу чтобы продолжить");
    Console.ReadKey();
}

Console.Clear();

// --- Запрос времени работы ---
Console.WriteLine("Задайте часы работы: 1, 2 или 3");
string? timeInput = Console.ReadLine();

if (!uint.TryParse(timeInput, out uint time) || time > 3)
{
    Console.Clear();
    Console.WriteLine("Некорректный ввод. Поумолчанию установлено время работы: 1 час");
    time = 1;
    
    Console.Write("Нажмите любую клавишу чтобы продолжить");
    Console.ReadKey();
}

Console.Clear();

// --- Запрос режима работы ---
Console.WriteLine("Введи рабочий режим: 1 - Экономный; 2 - Стандартный; 3 - Производительный");
string? modeInput = Console.ReadLine();

if (!uint.TryParse(modeInput, out uint modeResult) || modeResult > 3)
{
    Console.Clear();
    Console.WriteLine("Некорректный ввод. Поумолчанию установл установлен режим: Стандартный");
    modeResult = 2;
    
    Console.Write("Нажмите любую клавишу чтобы продолжить");
    Console.ReadKey();
}

Console.Clear();

// --- Запрос режима повышенной мощности ---
Console.WriteLine("Чтобы включить режим повышенной нагрузки (увеличивает производительность на 15%) введите 'Да'");
string? loadInput = Console.ReadLine();

bool increasedLoad = loadInput == "Да";

Console.Clear();

// --- Завимисоти и математика ---
double activeMode = NormalMode;              //Normal по умолчанию как значение ввода 2

if (modeResult == 1) activeMode = EconomyMode;
else activeMode = MaxMode;

double totalMode = increasedLoad ? activeMode * loadFactor: activeMode;

// Convert для округления
int totalEnergy = Convert.ToInt32(baseEnergy * time * totalMode);

// --- Итоговый вывод ---
Console.WriteLine($"Итоговое энергопотребление: {totalEnergy}");

Console.ReadKey();



