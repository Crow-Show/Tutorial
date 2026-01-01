using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string invalidMessage = "Некорректный ввод. \n";

StartingValues startingValues = ReadSourceNumberAndPowerBase();

RunDecompositionReport();
    
Console.ReadKey();

void RunDecompositionReport()
{
    string titleTableRow = "База".PadRight(13) + "Значение".PadRight(12) + "Результат".PadRight(12) + $"Степень[{startingValues.PowerBase}]";
    string lineSeparator = "\n--------------------------------------------\n";
    
    string titleTable = titleTableRow + lineSeparator;
     
    string report = GeneratePowerDecompositionReport(startingValues.SourceNumber, titleTable);
    
    Console.WriteLine(report);
}

DecompositionStep CalculateExponentDecomposition(int numberStep)
{
    PowerStep powerStep = CalculatePowerExponent();

    int remainder = numberStep;
    
    remainder -= powerStep.PowerValue;
    
    return new DecompositionStep(powerStep.Exponent, powerStep.PowerValue, remainder);
    
    PowerStep CalculatePowerExponent()
    {
        int exponentCounter = 1;
        int result = startingValues.PowerBase;
    
        while (result <= numberStep)
        {
            result *= startingValues.PowerBase;
            exponentCounter++;
        }
        
        exponentCounter--;
        result /= startingValues.PowerBase;
        
        return new PowerStep(exponentCounter, result);
    }
}

// понимаю, что надо бы вывести генерацию текста из рекурсии, но сил моих больше нет, пусть будет как есть
// я знаю как сделать, просто заебался с этой рекурсией
string GeneratePowerDecompositionReport(int numberState, string report)
{
    if (numberState < startingValues.PowerBase)
    {
        report += $"\nНеделимая часть: {numberState}";
        return report;
    }

    DecompositionStep decomposition = CalculateExponentDecomposition(numberState);
    
    int newSourceNumber = numberState - decomposition.PowerValue;

    report += BuildDecompositionTableRow(numberState, decomposition);
    
    return GeneratePowerDecompositionReport(newSourceNumber, report);
}

    string BuildDecompositionTableRow(int numberState, DecompositionStep decomposition)
    {
        string currentNumber = numberState.ToString().PadRight(10) + " - ".PadRight(3);
        string currentPower = decomposition.PowerValue.ToString().PadRight(9) + " =".PadRight(3);
        string resultNumber = decomposition.RemainingNumber.ToString().PadRight(12);
        string currentExponent = decomposition.Exponent.ToString();
        
        string resultRow = currentNumber + currentPower + resultNumber + currentExponent +"\n";
        
        return resultRow;
    }

string ReadInput(string message)
{
    Console.WriteLine(message);
    return Console.ReadLine() ?? "";
}

int ReadSourceNumber(string message)
{
    while (true)
    {
        string input = ReadInput(message);
        
        bool isValidInput = int.TryParse(input, out int sourceNumber);
        
        if (isValidInput && sourceNumber > 1)
            return sourceNumber;
        
        PrintStandardInvalidMassage();
    }
}

int ReadPowerBase(string message, int sourceNumber)
{
    while (true)
    {
        string input = ReadInput(message);
        
        bool isValidInput = int.TryParse(input, out int powerBase);
        
        if (isValidInput && powerBase <= sourceNumber)
            return powerBase;
        
        PrintStandardInvalidMassage();
    }
}

void PrintStandardInvalidMassage()
{
    Console.Clear();
    Console.WriteLine(invalidMessage);
}

StartingValues ReadSourceNumberAndPowerBase()
{
    string sourceMessage = "Введите целое положительное число число для деконструкции (число должно быть больше 1): ";
    string powerMassage = "Введите целое положительное число основания степени (оно должно быть не больше числа для деконструкции): ";
        
    int sourceNumber = ReadSourceNumber(sourceMessage);
    Console.Clear();
    
    int powerBase = ReadPowerBase(powerMassage, sourceNumber);
    Console.Clear();
    
    return new StartingValues(sourceNumber, powerBase);
}



record StartingValues (int SourceNumber, int PowerBase);

record PowerStep (int Exponent, int PowerValue);

record DecompositionStep(int Exponent, int PowerValue, int RemainingNumber);