using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string invalidMessage = "Некорректный ввод \n";
const string nameMessage = "Введите имя сотрудника: ";
const string scheduleMessage = "Варианты рабочего графика: \n" +
                               "Пятидневный: 5/2 \n" +
                               "Сменный: 2/2 \n" +
                               "\nУкажите график сотрудника: ";
const string adjustmentsMessage = "Требуется ли указать штрафы или переработки? Да/Нет: ";

string name = ReadNotEmptyString(nameMessage);
string schedule = ReadStringValid(scheduleMessage, "Пятидневный", "Сменный");

int[] workLog;

ShiftSchedules workSchedules = InitializeWorkSchedule(out workLog);

int workScheduleAdjustments = AdjustmentWorkHours(workLog);

WorkHoursSummary summary = CalculateSummary();

Console.Clear();

PrintTotalStatistics();

Console.ReadKey();

string BuildTotalTableStatistics()
{
    string result = String.Empty;

    string daysString = "Дни".PadRight(12) + BuildWorkingDaysStringFotTable(workLog) + "\n";
    string hourString = "Часы".PadRight(12) + BuildWorkingHourStringFotTable(workLog) + "\n";
    string penaltyString = "Штрафы".PadRight(12) + BuildPenaltyStringFotTable(workSchedules.WorkHoursInDay, workLog) +
                           "\n";
    string overworkString = "Переработки".PadRight(12) +
                            BuildOverworkingStringFotTable(workSchedules.WorkHoursInDay, workLog) + "\n";
    string separatorString = BuildLineSeparatorStringFotTable(workLog) + "\n";

    result += daysString + separatorString;
    result += hourString + separatorString;
    result += penaltyString + separatorString;
    result += overworkString + separatorString;

    return result;
}

string BuildLineSeparatorStringFotTable(int[] log)
{
    string lineSeparator = "-------------";

    for (int day = 0; day < log.Length; day++)
    {
        lineSeparator += "----";
    }

    return lineSeparator;
}

string BuildWorkingHourStringFotTable(int[] log)
{
    string workingHourString = String.Empty;

    for (int day = 0; day < log.Length; day++)
    {
        string dayWorkingHours = log[day].ToString();

        workingHourString += ("|" + dayWorkingHours.PadLeft(3));
    }

    return workingHourString + "|";
}

string BuildWorkingDaysStringFotTable(int[] log)
{
    string workingDayString = String.Empty;

    for (int day = 0; day < log.Length; day++)
    {
        string workingDay = (day + 1).ToString();

        workingDayString += ("|" + workingDay.PadLeft(3));
    }

    return workingDayString + "|";
}

string BuildPenaltyStringFotTable(int hours, int[] log)
{
    string penaltyHoursString = String.Empty;

    for (int day = 0; day < log.Length; day++)
    {
        string dayPenaltyHours = NormalizerPenaltyAndOverworkFotTable(hours - log[day]).ToString();

        penaltyHoursString += ("|" + dayPenaltyHours.PadLeft(3));
    }

    return penaltyHoursString + "|";
}

string BuildOverworkingStringFotTable(int hours, int[] log)
{
    string overworkingHoursString = String.Empty;

    for (int day = 0; day < log.Length; day++)
    {
        string dayOverworkingHours = NormalizerPenaltyAndOverworkFotTable(log[day] - hours).ToString();

        overworkingHoursString += ("|" + dayOverworkingHours.PadLeft(3));
    }

    return overworkingHoursString + "|";
}

int NormalizerPenaltyAndOverworkFotTable(int value)
{
    if (value >= 0)
        return value;
    else
        return 0;
}

string BuildTotalStatistics()
{
    string monthResults = GetMonthWorkStatus(summary.BalanceHours);
    string tableWorkingHoursByDay = BuildTotalTableStatistics();

    string employeeData = "Данные сотрудника: \n\n" +
                          $"Имя сотрудника: {name} \n" +
                          $"Тип графика: {schedule} \n" +
                          $"Рабочих часов в день: {workSchedules.WorkHoursInDay} \n" +
                          $"Месячная норма рабочих часов: {workSchedules.TotalWorkHours} \n\n";

    string workingStatistics = "Месячная статистика рабочих часов: \n\n" +
                               $"Фактически отработанных часов: {summary.TotalWorkedHours} \n" +
                               $"Корректировок рабочего времени: {workScheduleAdjustments} \n" +
                               monthResults + "\n\n";

    string tableResultsWorkHours = "Таблица рабочих часов по дням: \n\n" +
                                   tableWorkingHoursByDay;

    return employeeData + workingStatistics + tableResultsWorkHours;
}

void PrintTotalStatistics()
{
    string totalStatistics = BuildTotalStatistics();

    Console.WriteLine(totalStatistics);
}

string GetMonthWorkStatus(int balanceHour)
{
    if (balanceHour < 0)
        return $"Итог месяца: долг \nНедоработано часов: {-balanceHour}";

    else if (balanceHour > 0)
        return $"Итог месяца: закрыт с переработками \nПереработано часов: {balanceHour}";

    else
        return "Итог месяца: закрыт";
}

WorkHoursSummary CalculateSummary()
{
    int totalWorkedHours = CalculateFinalHours(workLog);
    int balanceHours = CalculateFinalBalance(totalWorkedHours);
    
    return new WorkHoursSummary(totalWorkedHours, balanceHours);
}

int CalculateFinalHours(in int[] log)
{
    int result = 0;

    foreach (int dayHour in log)
    {
        result += dayHour;
    }

    return result;
}

int CalculateFinalBalance(int finalHout)
{
    return finalHout - workSchedules.TotalWorkHours;
}

string ReadInput(string message)
{
    Console.Write(message);
    return Console.ReadLine() ?? string.Empty;
}

string ReadNotEmptyString(string message)
{
    while (true)
    {
        string value = ReadInput(message);

        if (value != String.Empty)
        {
            Console.Clear();
            return value;
        }

        PrintStandardInvalidInputMessage();
    }
}

// тут params скорее компромисс в отсутствии enum
string ReadStringValid(string message, params string[] options)
{
    while (true)
    {
        string value = ReadInput(message);

        foreach (string op in options)
        {
            if (value == op || options.Length == 0)
            {
                Console.Clear();
                return value;
            }
        }

        PrintStandardInvalidInputMessage();
    }
}

int ReadIntValidInRange(string message, int minValue, int maxValue)
{
    while (true)
    {
        int result;
        int.TryParse(ReadInput(message), out result);

        if (result >= minValue && result <= maxValue)
        {
            Console.Clear();
            return result;
        }

        PrintStandardInvalidInputMessage();
    }
}

ShiftSchedules InitializeWorkSchedule(out int[] log)
{
    if (schedule == "Пятидневный")
    {
        log = new int[22];

        for (int day = 0; day < log.Length; day++)
            log[day] = 8;

        return new ShiftSchedules(22, 8, 176);
    }

    else if (schedule == "Сменный")
    {
        log = new int[15];

        for (int day = 0; day < log.Length; day++)
            log[day] = 12;

        return new ShiftSchedules(15, 12, 180);
    }

    else
    {
        log = new int[0];

        return new ShiftSchedules(0, 0, 0);
    }
}

int AdjustmentWorkHours(int[] log)
{
    int adjustmentСounter = 0;

    while (UserWantsAdjustment())
    {
        WorkAdjustment adjustment = ReadAdjustment();

        if (IsUnacceptableAdjustment(adjustment, workLog))
        {
            PrintErrorMessageAdjustment(adjustment);
            continue;
        }

        ApplyAdjustment(adjustment, log);

        adjustmentСounter++;
    }

    return adjustmentСounter;
}

// метод сделан вложенный для минимизации аргументов и построения логичного порядка применения корректировок
void ApplyAdjustment(WorkAdjustment adjustment, int[] log)
{
    if (adjustment.ScheduleType == "Штраф")
    {
        ApplyPenalty(adjustment.Day, log);
    }

    else
    {
        ApplyOvertime(adjustment.Day, log);
    }

    // не функциональный аргумент сознательно оставлен для понимания, что метод меняет массив
    void ApplyPenalty(int day, int[] log)
    {
        string rules = "Штрафные часы не могут быть меньше 1 и превышать рабочее время.";

        int hour = ReadIntValidInRange(
            BuildAdjustmentMessage(day, rules),
            1,
            log[day - 1]);

        AddPenaltyHours();

        void AddPenaltyHours()
        {
            log[day - 1] -= hour;
        }
    }

    void ApplyOvertime(int day, int[] log)
    {
        string rules = "Переработка не может быть меньше 1 часа и превышать 6 часов в день.";

        int hour = ReadIntValidInRange(
            BuildAdjustmentMessage(day, rules),
            1,
            6);

        AddOvertimeHours();

        void AddOvertimeHours()
        {
            log[day - 1] += hour;
        }
    }

    string BuildAdjustmentMessage(int day, string rule)
    {
        string template = "День: {0}. Рабочих часов: {1}.\n" +
                          "{2} \n" +
                          "\nВведите количество часов в виде целого положительно числа: ";

        return string.Format(template, day, log[day - 1], rule);
    }
}

void PrintErrorMessageAdjustment(WorkAdjustment adjustment)
{
    if (adjustment.ScheduleType == "Штраф")
    {
        Console.Clear();
        Console.WriteLine($"Недопустимая операция для дня {adjustment.Day}. Рабочие часы не могут быть меньше 0 \n");
    }

    else
    {
        Console.Clear();
        Console.WriteLine(
            $"Недопустимая операция для дня {adjustment.Day}. Переработка не может превышать 6 часов в день \n");
    }
}

bool IsUnacceptableAdjustment(WorkAdjustment adjustment, int[] log)
{
    if (adjustment.ScheduleType == "Штраф")
    {
        bool flag = IsUnacceptablePenalty();

        return flag;
    }

    else
    {
        bool flag = IsUnacceptableOverworking();

        return flag;
    }

    bool IsUnacceptablePenalty()
    {
        return log[adjustment.Day - 1] == 0;
    }

    bool IsUnacceptableOverworking()
    {
        return log[adjustment.Day - 1] >= (workSchedules.WorkHoursInDay + 6);
    }
}

WorkAdjustment ReadAdjustment()
{
    string dayMessage = $"Рабочих дней сотрудника: {workSchedules.WorkDays}. \n" +
                        "\nВведите день для корректировки: ";
    string typeMessage = "Типы корректировки: Штраф/Переработка \n" +
                         "\nВыберите тип корректировки: ";

    int day = ReadIntValidInRange(dayMessage, 1, workSchedules.WorkDays);
    string scheduleType = ReadStringValid(typeMessage, "Штраф", "Переработка");

    return new WorkAdjustment(day, scheduleType);
}

bool UserWantsAdjustment()
{
    string input = ReadStringValid(adjustmentsMessage, "Да", "Нет");

    return input == "Да";
}

void PrintStandardInvalidInputMessage()
{
    Console.Clear();
    Console.WriteLine(invalidMessage);
}

record WorkAdjustment(int Day, string ScheduleType);

record ShiftSchedules(int WorkDays, int WorkHoursInDay, int TotalWorkHours);

record WorkHoursSummary(int TotalWorkedHours, int BalanceHours);