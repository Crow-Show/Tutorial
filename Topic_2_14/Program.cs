using System.Text;

Console.OutputEncoding = Encoding.UTF8; 
Console.InputEncoding = Encoding.UTF8;

// консольная программа расчета заказа
// запрашивает данные пользователя, рассчитывает скидку и кэшбэк
// выводит итоговую информацию
const string invalidMassage = "Некорректный ввод\n";
const string statusMassage = @"Виды аккаунтов: Стандарт, Премиум, VIP
Введите статус аккаунта: ";
const string nameMassage = "Введите имя покупателя: ";
const string costMassage = "Введи стоимость товаров в виде целого положительного числа: ";

string name = ReadNotEmptyString(nameMassage, invalidMassage);
Console.Clear();

string status = ReadAccountStatus(statusMassage, invalidMassage);
Console.Clear();

uint cost = ReadUint(costMassage, invalidMassage);
Console.Clear();

double discount = CalculateDiscount(status);
uint cashback = CalculateCashback(cost);

double discountCount = cost * discount;

Console.Clear();

PrintOrderSummary(name, status, cost, discountCount, cashback);


void PrintOrderSummary(string name, string status, uint cost, double discountCost, uint cashbak)
{
    Console.Write(@$"Имя: {name}!
Стоимость товаров: {cost}
Статус аккаунта: {status}
Стоимость с учетом скидки: {discountCost}
Начислено кэшбэка : {cashbak}");
    
    Console.ReadKey();
}

// методы безопасного ввода
// гарантированно возвращают валидное значение
string ReadNotEmptyString(string massage, string errorMassage)
{
    while (true)
    {
        string value = ReadInput(massage);

        if (value != "")
            return value;
    
        InvalidInput(errorMassage);
    }
}

string ReadAccountStatus(string massage, string errorMassage)
{
    while (true)
    {
        string value = ReadInput(massage);

        if (value == "Стандарт" || value == "Премиум" || value == "VIP")
            return value;
    
        InvalidInput(errorMassage);
    }
}
    
uint ReadUint(string massage, string errorMassage)
{
    while (true)
    {
        uint result = 0;
        string value = ReadInput(massage);
        
        if (UintTryParse(value, out result))
            return result;
    
        InvalidInput(errorMassage);
    }
}

bool UintTryParse(string value, out uint result)
{
    return uint.TryParse(value, out result);
}

string ReadInput(string? massage)
{
    Console.Write(massage);
    return Console.ReadLine() ?? "";
}

void InvalidInput(string massage)
{
    Console.Clear();
    Console.WriteLine(massage);
}

// скидка за аккаунт. Стандарт = 0%, Премиум = 5%, VIP = 10%
double CalculateDiscount(string status)
{
    if (status == "Стандарт") return 1.0;
    if (status == "Премиум")  return 0.95;
    if (status == "VIP")  return 0.90;
    return 1;
}

// кэшбэк - до 1000 = 1%, до 5000 = 2%, больше 5000 = 3%
uint CalculateCashback(uint cost)
{
    if (cost >= 5000)
        return Convert.ToUInt32(cost * 0.03);
    if (cost >= 1000) 
        return Convert.ToUInt32(cost * 0.02);
    if (cost > 0) 
        return Convert.ToUInt32(cost * 0.01);
    return 0;
}
