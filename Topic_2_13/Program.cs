using System.Text;

Console.OutputEncoding = Encoding.UTF8; 
Console.InputEncoding = Encoding.UTF8;

const string invalidInput = "Некорректный ввод. Введите целое неотрицательное значение \n";

uint[][] cpuLoadByDay;
uint[] cpuLoadList;

//Общее число измерений
uint totalMeasurements = 0;

uint days = 0;

// ------ Ввод количества дней -------
while (true)
{
    Console.Write("Укажите за сколько дней собирается статистика CPU: ");
    string? daysInput =  Console.ReadLine(); 
    
    if(!uint.TryParse(daysInput, out days))
    {
        Console.Clear();
        Console.WriteLine(invalidInput);
        
        continue;
    }

    else break;
}

Console.Clear();

cpuLoadByDay = new uint[days][];

// ------- Ввод числа измерений в каждом дне ---------
for (int i = 0; i < days; i++)
{
    while (true)
    {
        Console.WriteLine($"Введите число измерений за день. \nДень: {i + 1}");
        string? daysMeasurementsInput = Console.ReadLine();

        uint daysMeasurements;
        if (uint.TryParse(daysMeasurementsInput, out daysMeasurements) && daysMeasurements > 0)
        {
            cpuLoadByDay[i] = new uint[daysMeasurements];
            totalMeasurements += daysMeasurements;
            
            Console.Clear();
            break;
        }

        else
        {
            Console.Clear();
            Console.WriteLine(invalidInput);
            
            continue;
        }
    }
}

Console.Clear();

cpuLoadList = new uint[totalMeasurements];

// Индекс Листа для записи значений внутри цикла
int listCounter = 0;

// ---------- Ввод самих измерений ----------
for (int i = 0; i < cpuLoadByDay.Length; i++)
{
    for (int j = 0; j < cpuLoadByDay[i].Length; j++)
    {
        while (true)
        {
            Console.Write($"Введите нагрузку CPU от 1 до 100 \n" + 
                          $"День {i + 1}. Измерений CPU: {cpuLoadByDay[i].Length} \n" + 
                          $"Замер {j + 1}: ");
            
            string? measurementInput = Console.ReadLine();
            uint measurement;
            if (uint.TryParse(measurementInput, out measurement) && measurement > 0 && measurement <= 100)
            {
                cpuLoadByDay[i][j] = measurement;
                
                cpuLoadList[listCounter] = measurement;
                listCounter++;
                
                Console.Clear();
                break;
            }

            else
            {
                Console.Clear();
                Console.WriteLine(invalidInput);
                
                continue;
            }
        }
    }
}

Console.Clear();

// ------- Сортировка Листа и математика ---------
Array.Sort(cpuLoadList);

uint totalMinLoad = cpuLoadList[0];
uint totalMaxLoad = cpuLoadList[cpuLoadList.Length - 1];

double totalMidLoad = 0;

for(int i = 0; i < cpuLoadList.Length; i++) totalMidLoad += cpuLoadList[i];

totalMidLoad = (totalMidLoad / cpuLoadList.Length);

// --------- Вывод таблицы --------
uint day = 0;

Console.WriteLine("Статистика по дням \n" +
"|День".PadRight(5) + "|" +
"Изм".PadRight(4) + "|" +
"Min".PadRight(4) + "|" +
"Max".PadRight(4) + "|" +
"Mid".PadRight(5) + "|");

foreach (uint[] i in cpuLoadByDay)
{
    ++day;
    int measurementsCount = i.Length;
    uint min = i[0];
    uint max = i[0];
    double mid = 0;
            
    foreach (uint j in i)
    {
        if (j < min) min = j;
        if (j > max) max = j;

        mid += j;
    }

    mid = (mid / i.Length);
    
    Console.WriteLine("|"+ day.ToString().PadLeft(4) + "|"+
                      measurementsCount.ToString().PadLeft(4) + "|" +
                      min.ToString().PadLeft(4) + "|" + 
                      max.ToString().PadLeft(4) + "|" + 
                      mid.ToString("0.0").PadLeft(5) + "|");
}

// ---------- Вывод общей статистики и сортированного массива --------
Console.WriteLine($@"
Общая статистика:

Дней: {days}
Минимальная нагрузка: {totalMinLoad}
Максимальная нагрузка: {totalMaxLoad}
Средняя нагрузка: {totalMidLoad}");

Console.WriteLine("\nСписок всех измерений по возрастанию: \n");
for (int i = 0; i < cpuLoadList.Length; i++) Console.Write(cpuLoadList[i] + "; ");

Console.ReadKey();