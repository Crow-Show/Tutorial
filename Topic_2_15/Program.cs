using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string name = RequestNotEmptyString(Messages.NameMessage);
string schedule = RequestStringValid(Messages.ScheduleMessage, ScheduleTypes.FiveDay, ScheduleTypes.Shift);

int[] workLog;

ShiftSchedules workSchedules = InitializeWorkSchedule(out workLog);

int workScheduleAdjustments = AdjustmentWorkHours(workLog);

WorkHoursSummary summary = CalculateSummary();

Console.Clear();

PrintTotalStatistics(workLog, workSchedules.WorkHoursInDay);

Console.ReadKey();

string BuildTable(int[] workLog, int workHourInDay)
{
    var table = new StringBuilder();

    table.AppendLine(BuildTitleTable());
    
    for (int day = 0; day < workLog.Length; day++)
    {
        table.Append(BuildTableRow(workLog, day, workHourInDay));
    }
    
    return table.ToString();
    
    string BuildTableRow(int[] workLog,  int workHourInDay, int day)
    {
        WorkingStatsInDay stats = GetDayStats(workLog, workHourInDay, day);
        
        var sb = new StringBuilder();
        
        sb.Append("|" + Center(stats.Day.ToString(), TableTitlesData.WorkDay.Length + TableTitlesData.StandardIndentation));
        sb.Append("|" + Center(stats.Hours.ToString(), TableTitlesData.WorkHours.Length + TableTitlesData.StandardIndentation));
        sb.Append("|" + Center(stats.Overwork.ToString(), TableTitlesData.OverworkingHours.Length + TableTitlesData.StandardIndentation));
        sb.Append("|" + Center(stats.Penalty.ToString(), TableTitlesData.PenaltyHours.Length + TableTitlesData.StandardIndentation) + "|");
        sb.AppendLine();
        
        string lineSeparator = BuildLineSeparator();
        
        sb.AppendLine(lineSeparator);
        
        return sb.ToString();
    }
    
    string BuildLineSeparator()
    {
        int longRow = TableTitlesData.WorkDay.Length
                      + TableTitlesData.WorkHours.Length
                      + TableTitlesData.OverworkingHours.Length
                      + TableTitlesData.PenaltyHours.Length
                      + TableTitlesData.StandardIndentation * TableTitlesData.NumberOfCols
                      + TableTitlesData.NumberOfColSeparators;
        
        return new string('-', longRow);
    }
    
    string BuildTitleTable()
    {
        var sb = new StringBuilder();
    
        sb.Append("|" + Center(TableTitlesData.WorkDay,
            TableTitlesData.WorkDay.Length
            + TableTitlesData.StandardIndentation));
        
        sb.Append("|" + Center(TableTitlesData.WorkHours,
            TableTitlesData.WorkHours.Length
            + TableTitlesData.StandardIndentation));
        
        sb.Append("|" + Center(TableTitlesData.OverworkingHours,
            TableTitlesData.OverworkingHours.Length
            + TableTitlesData.StandardIndentation));
        
        sb.Append("|" + Center(TableTitlesData.PenaltyHours,
            TableTitlesData.PenaltyHours.Length
            + TableTitlesData.StandardIndentation) + "|");
        
        sb.AppendLine();
        
        string lineSeparator = BuildLineSeparator();
        
        sb.Append(lineSeparator);
        
        return sb.ToString();
    }
}

WorkingStatsInDay GetDayStats(int[] log, int day, int standardHours)
{
    int currentDay = day + 1;
    int workingHoursInDay = log[day];
    int overworking = NormalizerPenaltyAndOverworkFotTable(log[day] - standardHours);
    int penalty = NormalizerPenaltyAndOverworkFotTable(standardHours - log[day]);
    
    return new WorkingStatsInDay(currentDay, workingHoursInDay, overworking, penalty);
}

string Center(string text, int width)
{
    int padding = width - text.Length;

    if (padding <= 0)
    {
        return text;
    }
    
    int padLeft = padding / 2;
    int padRight = padding - padLeft;
    
    return new string(' ', padLeft) + text + new string(' ', padRight);
}

int NormalizerPenaltyAndOverworkFotTable(int value)
{
    return value >= 0 ? value : 0;
}

string BuildTotalStatistics(int [] workLog, int workingHoursInDay)
{
    string monthResults = GetMonthWorkStatus(summary.BalanceHours);
    string tableWorkingHoursByDay = BuildTable( workLog, workingHoursInDay);

    string employeeData = $"""
                           Данные сотрудника:
                           
                           Имя сотрудника: {name}
                           Тип графика: {schedule}
                           Рабочих часов в день: {workSchedules.WorkHoursInDay}
                           Месячная норма рабочих часов: {workSchedules.TotalWorkHours}
                           """ + "\n\n";

    string workingStatistics = $"""
                                Месячная статистика рабочих часов:
                                
                                Фактически отработанных часов: {summary.TotalWorkedHours}
                                Корректировок рабочего времени: {workScheduleAdjustments}
                                {monthResults}
                                
                                """ + "\n";

    string tableResultsWorkHours = "Таблица рабочих часов по дням: \n\n" +
                                   tableWorkingHoursByDay;

    return employeeData + workingStatistics + tableResultsWorkHours;
}

void PrintTotalStatistics(int [] workLog, int workingHoursInDay)
{
    string totalStatistics = BuildTotalStatistics(workLog, workingHoursInDay);

    Console.WriteLine(totalStatistics);
}

string GetMonthWorkStatus(int balanceHour)
{
    return balanceHour switch
    {
        < 0 => $"Итог месяца: долг \nНедоработано часов: {-balanceHour}",
        > 0 => $"Итог месяца: закрыт с переработками \nПереработано часов: {balanceHour}",
        _ => "Итог месяца: закрыт"
    };
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

string RequestInput(string message)
{
    Console.Write(message);
    return Console.ReadLine() ?? string.Empty;
}

string RequestNotEmptyString(string message)
{
    while (true)
    {
        string value = RequestInput(message);

        if (value != String.Empty)
        {
            Console.Clear();
            return value;
        }

        PrintStandardInvalidInputMessage();
    }
}

string RequestStringValid(string message, params string[] options)
{
    while (true)
    {
        string value = RequestInput(message);

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

int RequestIntValidInRange(string message, int minValue, int maxValue)
{
    while (true)
    {
        int result;
        int.TryParse(RequestInput(message), out result);

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
    if (schedule == ScheduleTypes.FiveDay)
    {
        log = new int[22];

        for (int day = 0; day < log.Length; day++)
            log[day] = 8;

        return new ShiftSchedules(22, 8, 176);
    }

    if (schedule == ScheduleTypes.Shift)
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
        WorkAdjustment adjustment = RequestAdjustment();

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
    if (adjustment.ScheduleType == AdjustmentTypes.Penalty)
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

        int hour = RequestIntValidInRange(
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

        int hour = RequestIntValidInRange(
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
    Console.Clear();
    
    if (adjustment.ScheduleType == AdjustmentTypes.Penalty)
    {
        Console.WriteLine($"Недопустимая операция для дня {adjustment.Day}. Рабочие часы не могут быть меньше 0 \n");
    }

    else
    {
        Console.WriteLine(
            $"Недопустимая операция для дня {adjustment.Day}. Переработка не может превышать 6 часов в день \n");
    }
}

bool IsUnacceptableAdjustment(WorkAdjustment adjustment, int[] log)
{
    return adjustment.ScheduleType == AdjustmentTypes.Penalty ? IsUnacceptablePenalty() : IsUnacceptableOverworking();

    bool IsUnacceptablePenalty()
    {
        return log[adjustment.Day - 1] == 0;
    }

    bool IsUnacceptableOverworking()
    {
        return log[adjustment.Day - 1] >= (workSchedules.WorkHoursInDay + 6);
    }
}

WorkAdjustment RequestAdjustment()
{
    string dayMessage = $"Рабочих дней сотрудника: {workSchedules.WorkDays}. \n" +
                        "\nВведите день для корректировки: ";
    string typeMessage = "Типы корректировки: Штраф/Переработка \n" +
                         "\nВыберите тип корректировки: ";

    int day = RequestIntValidInRange(dayMessage, 1, workSchedules.WorkDays);
    string scheduleType = RequestStringValid(typeMessage, AdjustmentTypes.Penalty, AdjustmentTypes.Overworking);

    return new WorkAdjustment(day, scheduleType);
}

bool UserWantsAdjustment()
{
    string input = RequestStringValid(Messages.AdjustmentsMessage, "Да", "Нет");

    return input == "Да";
}

void PrintStandardInvalidInputMessage()
{
    Console.Clear();
    Console.WriteLine(Messages.InvalidMessage);
}

record WorkAdjustment(int Day, string ScheduleType);

record ShiftSchedules(int WorkDays, int WorkHoursInDay, int TotalWorkHours);

record WorkHoursSummary(int TotalWorkedHours, int BalanceHours);

record WorkingStatsInDay(int Day, int Hours, int Overwork, int Penalty);

static class ScheduleTypes
{
    public const string FiveDay = "Пятидневный";
    public const string Shift = "Сменный";
}

static class AdjustmentTypes
{
    public const string Penalty = "Штраф";
    public const string Overworking = "Переработка";
}

static class TableTitlesData
{
    public const int NumberOfCols = 4;
    public const int NumberOfColSeparators = NumberOfCols + 1;
    public const int StandardIndentation = 2;
    
    public const string WorkDay = "День";
    public const string WorkHours = "Часы";
    public const string OverworkingHours = "Переработки";
    public const string PenaltyHours = "Штрафы";
}

static class Messages
{
    public const string InvalidMessage = "Некорректный ввод \n";
    public const string NameMessage = "Введите имя сотрудника: ";
    public const string AdjustmentsMessage = "Требуется ли указать штрафы или переработки? Да/Нет: ";

    public const string ScheduleMessage = """
                                         Варианты рабочего графика:
                                         Пятидневный: 5/2
                                         Сменный: 2/2
                                         
                                         Укажите график сотрудника: 
                                         """;
    
}