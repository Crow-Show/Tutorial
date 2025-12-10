using System.Reflection.Metadata.Ecma335;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string InvalidMessage = "Некорректное значение. Операция не засчитана";

const string CancelMessage = "Операция отменена";

const string OperationInstruction = "Введите тип операции: \n" +
                                    "'Продажа' - товар продан \n" +
                                    "'Возврат' - товар возвращен \n" +
                                    "'Стоп' - завершить кассовый день \n";

const string SaleInstruction = "Стоимость товара добавится к дневной выручке. Команда 'Отмена' отменяет операцию. \n" +
                               "\nВведите стоимсть или команду 'Отмена': ";

const string ReturnInstruction = "Стоимость возвращенного товара будет вычтена из дневной выручки. Команда 'Отмена' отменяет операцию. \n" +
                                 "\nВведите стоимсть или команду 'Отмена': ";

uint saleCounter = 0;
uint returnCounter = 0;
uint canselCounter = 0;

uint sumRevenue = 0;
uint sumReturns = 0;

int totalRevenue = 0;

uint minSale = 0;
uint maxSale = 0;

// Основной цикл кассового дня
// Принимает команды до ввода "Стоп"
do
{
    Console.WriteLine(OperationInstruction);
    
    string? operationInput =  Console.ReadLine();

    // Обработка продажи
    // Добаыляет сумму к выручке
    // Обновляет min/max
    // Увеличивает счетчик продаж
    if (operationInput == "Продажа")
    {
        Console.Clear();
        Console.Write(SaleInstruction);
        
        string? saleInput = Console.ReadLine();

        if (saleInput == "Отмена")
        {
            Console.Clear();
            Console.WriteLine(CancelMessage);
            
            canselCounter++;
            continue;
        }

        uint saleResult;
        
        if (uint.TryParse(saleInput, out saleResult))
        {
            sumRevenue += saleResult;

            if(maxSale < saleResult) maxSale = saleResult;
            if( minSale == 0 || minSale > saleResult)  minSale = saleResult;

            saleCounter++;
        }

        else
        {
            Console.Clear();
            Console.WriteLine(InvalidMessage);
            
            continue;
        }
        
        Console.Clear();
    }

    // Обработка возвратов
    // Добавляет значение к сумме возвратов
    // Увеличивает счетчик возвратов
    else if (operationInput == "Возврат")
    {
        Console.Clear();
        Console.WriteLine(ReturnInstruction);
        
        string? returnInput = Console.ReadLine();
        
        if (returnInput == "Отмена")
        {
            Console.Clear();
            Console.WriteLine(CancelMessage);
            
            canselCounter++;
            continue;
        }
        
        uint returnResult;
        
        if (uint.TryParse(returnInput, out returnResult))
        {
            sumReturns += returnResult;
            returnCounter++;
        }

        else
        {
            Console.Clear();
            Console.WriteLine(InvalidMessage);
            
            continue;
        }
        
        Console.Clear();
    }

    // Завершение кассового дня
    else if (operationInput == "Стоп") break;

    // Обработка некорретного ввода
    else
    {
        Console.Clear();
        Console.WriteLine("Некорректный ввод. Введите выриант из предложенных.");
    }
} while(true);

Console.Clear();

// Подсчет чистой выручки по итогу дня
totalRevenue = (int)(sumRevenue - sumReturns);

Console.WriteLine($@"======= Итог кассового дня =======

Продажи: {saleCounter}
Возврат: {returnCounter}
Отмены: {canselCounter}

Выручка: {sumRevenue}
Сумма возвратов: {sumReturns}
Чистая выручка: {totalRevenue}

Минимальная продажа: {minSale}
Максимальная продажа: {maxSale}
==================================");

Console.ReadLine();