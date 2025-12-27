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

string name = ReadNotEmptyString(nameMassage);
Console.Clear();

string status = ReadAccountStatus(statusMassage);
Console.Clear();

uint cost = ReadValidUint(costMassage);
Console.Clear();

double discount = CalculateDiscount();
uint cashback = CalculateCashback();

double discountCount = cost * discount;

Console.Clear();

PrintOrderSummary();


void PrintOrderSummary()
{
    Console.Write(@$"
Имя: {name}
Стоимость товаров: {cost}
Статус аккаунта: {status}
Стоимость с учетом скидки: {discountCount}
Начислено кэшбэка : {cashback}");
    
    Console.ReadKey();
}

// методы безопасного ввода
// гарантированно возвращают валидное значение
string ReadNotEmptyString(string massage)
{
    while (true)
    {
        string value = ReadInput(massage);

        if (value != "")
            return value;
    
        InvalidInput();
    }
}

string ReadAccountStatus(string massage)
{
    while (true)
    {
        string value = ReadInput(massage);

        if (value == "Стандарт" || value == "Премиум" || value == "VIP")
            return value;
    
        InvalidInput();
    }
}
    
uint ReadValidUint(string massage)
{
    while (true)
    {
        uint result;
        string value = ReadInput(massage);
        
        if (TryParseUint(value, out result))
            return result;
    
        InvalidInput();
    }
}

bool TryParseUint(string value, out uint result)
{
    return uint.TryParse(value, out result);
}

string ReadInput(string? massage)
{
    Console.Write(massage);
    return Console.ReadLine() ?? "";
}

void InvalidInput()
{
    Console.Clear();
    Console.WriteLine(invalidMassage);
}

// скидка за аккаунт. Стандарт = 0%, Премиум = 5%, VIP = 10%
double CalculateDiscount()
{
    if (status == "Стандарт") return 1.0;
    if (status == "Премиум")  return 0.95;
    if (status == "VIP")  return 0.90;
    return 1;
}

// кэшбэк - до 1000 = 1%, до 5000 = 2%, больше 5000 = 3%
uint CalculateCashback()
{
    if (cost >= 5000)
        return Convert.ToUInt32(cost * 0.03);
    if (cost >= 1000) 
        return Convert.ToUInt32(cost * 0.02);
    if (cost > 0) 
        return Convert.ToUInt32(cost * 0.01);
    return 0;
}