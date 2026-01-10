using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

RunGameLoop();
// ------------------- Метод меню
MainMenuCommands RunGameMenu()
{
    Console.Clear();

    string command = RequestValidCommand(UiText.GameMenuUi, CommandSets.GameMenuCommand);

    return ParseMainMenuCommand(command);
    
    static MainMenuCommands ParseMainMenuCommand(string command)
    {
        return command switch
        {
            "Рестарт" => MainMenuCommands.RestartRound,
            "Ресет" => MainMenuCommands.ResetGame,
            "Выход" => MainMenuCommands.ExitGame
        };
    }
}

// ------------------- Метод раундов
GameRoundResult RunGameRound(RoundContext context, GameStartSettings settings)
{
    DataCurrentPlayers data = GetDataCurrentPlayers(settings.Player1Name, settings.Player2Name, context.Player1IsX);
    
    string[,] board = InitializeBoardArr(settings.BoardSize);
    bool isPlayersTurnX = true;
    
    while (true)
    { 
        PrintStartTurnInfo(board, isPlayersTurnX, data);
        
        CellCoordinates playersTurnCell;

        ParseTurnCommandResult parseResult = RequestAndParsePlayerMove(board, out playersTurnCell);

        if (parseResult == ParseTurnCommandResult.Menu)
        {
            return GameRoundResult.GoToMenu;
        }

        if (parseResult != ParseTurnCommandResult.Parsed)
        {
            PrintInvalidMessage(GetParseErrorMassage(parseResult));
            continue;
        }
        
        ApplyBoardChanges(playersTurnCell, board, isPlayersTurnX);

        GameRoundResult roundResult = GetRoundResult(board, isPlayersTurnX, context.Player1IsX);

        if (roundResult == GameRoundResult.InProgress)
        {
            isPlayersTurnX = !isPlayersTurnX;
            
            Console.Clear();
            continue;
        }
        
        PrintEndRoundMessage(board, roundResult, data.Player1Name, data.Player2Name);
        
        return roundResult;
    }
}

DataCurrentPlayers GetDataCurrentPlayers(string player1Name, string player2Name, bool player1IsX)
{
    return player1IsX
        ? new DataCurrentPlayers(player1Name, player2Name, IconsForTables.IconX, IconsForTables.Icon0)
        : new DataCurrentPlayers(player1Name, player2Name, IconsForTables.Icon0, IconsForTables.IconX);
}

// ---------------- Основной метод игры
void RunGameLoop()
{
    bool continueGame;
    do
    { 
        Console.Clear();
        
        GameStartSettings startSettings = RequestStartSettings();
        RoundContext context = new RoundContext(1, true);
        ScoresRounds scores = new ScoresRounds(0, 0, 0);
        
        string roundStatisticsTable = "";

        bool continueCurrentGame = true;
        do
        {
            Console.Clear();
            
            GameRoundResult result = RunGameRound(context, startSettings);

            if (result == GameRoundResult.GoToMenu)
            {
                PrintGameStatistics(roundStatisticsTable, scores, startSettings.Player1Name, startSettings.Player2Name);

                MainMenuCommands command = RunGameMenu();

                switch (command)
                {
                    case MainMenuCommands.RestartRound: ;
                        continue;
                    case MainMenuCommands.ResetGame:
                        context = new RoundContext(1, true);
                        startSettings = RequestStartSettings();
                        roundStatisticsTable = "";
                        continue;
                    case MainMenuCommands.ExitGame:
                        return;
                }
            }

            roundStatisticsTable += UpdateStatisticsTableForRound(result, context.RoundNumber);
            scores = scores.WithResult(result);
            
            PrintGameStatistics(roundStatisticsTable, scores, startSettings.Player1Name, startSettings.Player2Name);

            context = new RoundContext(context.RoundNumber + 1, !context.Player1IsX);
            
            continueCurrentGame = RequestContinuation(UiText.ExitRoundMenuUi);
        } while (continueCurrentGame);

        continueGame = RequestContinuation(UiText.ExitGameMenuUi);
    } while (continueGame);
}

GameStartSettings RequestStartSettings()
{
    string players1Name = RequestPlayerName(1);
    Console.Clear();
    
    string players2Name = RequestPlayerName(2);
    Console.Clear();
    
    int boardSize = RequestBoardSize();
    Console.Clear();
    
    return new GameStartSettings(players1Name, players2Name, boardSize);
}

string RequestPlayerName(int playersNumber)
{
    while (true)
    {
        string input = RequestInput($"Введите имя игрока {playersNumber}: ");

        if (!string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        PrintInvalidMessage("Недопустимое имя \n");
    }
}
// ---------------- Инициализация поля
string[,] InitializeBoardArr(int boardSize)
{
    string[,] board = new string[boardSize, boardSize];
    
    for (int rowIndex = 0; rowIndex < board.GetLength(0); rowIndex++)
    {
        for (int colIndex = 0; colIndex < board.GetLength(1); colIndex++)
        {
            board[rowIndex, colIndex] = IconsForTables.IconVoid;
        }   
    }
    
    return board;
}
 
// ---------------- Показать поле и чей ход
void PrintBoard(string[,] boardLog)
{
    char[] alphabet = CreateAlphabetArr();
    
    var sb = new StringBuilder();

    sb.Append("  |");
    
    for (int currentLiter = 0; currentLiter < boardLog.GetLength(0); currentLiter++)
    {
        sb.Append(" " + alphabet[currentLiter] + " |");
    }
    
    sb.AppendLine();
    
    string lineSeparator = "---" + new string('-', boardLog.GetLength(0) * 4);
    sb.AppendLine(lineSeparator);

    for (int y = 0; y < boardLog.GetLength(0); y++)
    {
        sb.Append((y + 1).ToString().PadRight(2) + "|");
        for (int x = 0; x < boardLog.GetLength(1); x++)
        {
            sb.Append(boardLog[y, x]).Append("|");
        }
        
        sb.AppendLine();
        sb.AppendLine(lineSeparator);
    }
    
    string board = sb.ToString();
    Console.WriteLine(board);

    char[] CreateAlphabetArr()
    {
        char[] alphabetArr = new char[26];

        for (int currentLetter = 0; currentLetter < 26; currentLetter++)
        {
            alphabetArr[currentLetter] = (char)('A' + currentLetter);
        }
        
        return alphabetArr;
    }
}

void PrintWhoseTurn(bool isPlayersTurnX, DataCurrentPlayers data)
{
    string currentPlayerIcon = isPlayersTurnX ? IconsForTables.IconX : IconsForTables.Icon0;
    string currentPlayerName = GetNameCurrentPlayer(currentPlayerIcon, data);

    Console.WriteLine($"Ход игрока {currentPlayerIcon} ({currentPlayerName}) \n");
}

string GetNameCurrentPlayer(string currentPlayerIcon, DataCurrentPlayers data)
{
    return currentPlayerIcon == data.Player1Icon
        ? data.Player1Name
        : data.Player2Name;
}

// ---------------- Показать статистику
void PrintGameStatistics(string statisticsTable, ScoresRounds scores, string player1Name, string player2Name)
{
    Console.Clear();
    Console.WriteLine(statisticsTable);
    
    Console.WriteLine();
    Console.WriteLine(BuildScoresStatisticsMessage(scores, player1Name, player2Name));
    
    Console.WriteLine(UiText.StandardReadKyeMassage);
    Console.ReadKey();
}

string BuildScoresStatisticsMessage(ScoresRounds scores, string player1Name, string player2Name)
{
    return $"""
            Побед игрока 1 ({player1Name}): {scores.P1}
            Побед игрока 2 ({player2Name}): {scores.P2}
            Ничьих: {scores.Draw}
            """;
}

// ---------------- Центрирование для таблицы
string Center(string text, int width)
{
    int padding = width - text.Length;

    if (padding <= 0)
    {
        return text;
    }
    
    int padLeft =  padding / 2;
    int padRight =  padding - padLeft;
    
    return new string(' ', padLeft) + text + new string(' ', padRight);
}

// ---------------- Обновить таблицу
string UpdateStatisticsTableForRound(GameRoundResult result, int roundCounter)
{
    TableHeaders headers = new TableHeaders(
        "Раунд", 
        "    ", 
        "Игрок 1", 
        "Игрок 2", 
        "Ничья");

    TableLayout layout = new TableLayout(
        headers.Round.Length,
        headers.Spacer.Length,
        headers.Player1.Length,
        headers.Player2.Length,
        headers.Draw.Length);

    string currentTable = "";

    if (roundCounter == 1)
    {
        currentTable += BuildTitleTable(headers);
    }

    currentTable += BuildRoundRow(result, roundCounter, layout);
    
    return currentTable;
    
    string BuildRoundRow(GameRoundResult result, int roundCounter, TableLayout layout)
    {
        var sb = new StringBuilder();
    
        string lineSeparator = BuildLineSeparator(layout);
    
        sb.AppendLine();
        sb.AppendLine(lineSeparator);
        
        IconsStatisticTable icons = RoundStatisticsInterpreter(result);
        
        sb.Append("|" + Center((roundCounter).ToString(), layout.Round));
        sb.Append("|" + new string(' ',layout.Spacer) + "|");
        sb.Append(Center(icons.P1, layout.Player1) + "|");
        sb.Append(Center(icons.P2, layout.Player2) + "|");
        sb.Append(Center(icons.Dh, layout.Draw) + "|");
        
        return sb.ToString();
    }
    
    string BuildLineSeparator(TableLayout layout)
    {
        string lineSeparator = 
            new string('-', layout.Round + 2) +
            new string(' ', layout.Spacer) +
            new string('-', layout.Player1 + layout.Player2 + layout.Draw + 4);
    
        return lineSeparator;
    }
    
    string BuildTitleTable(TableHeaders headers)
    {
        var sb = new StringBuilder();
        
        sb.Append("|" + headers.Round);
        sb.Append("|" + headers.Spacer);
        sb.Append("|" + headers.Player1);
        sb.Append("|" + headers.Player2);
        sb.Append("|" + headers.Draw + "|");
        
        return sb.ToString();
    }
    
    IconsStatisticTable RoundStatisticsInterpreter(GameRoundResult result)
    {
        return result switch
        {
            GameRoundResult.IsWinPlayer1 => new IconsStatisticTable(
                IconsForTables.IconWin,
                IconsForTables.IconVoid,
                IconsForTables.IconVoid),
            
            GameRoundResult.IsWinPlayer2 => new IconsStatisticTable(
                IconsForTables.IconVoid,
                IconsForTables.IconWin,
                IconsForTables.IconVoid),
            
            GameRoundResult.IsDeadHeat => new IconsStatisticTable(
                IconsForTables.IconVoid,
                IconsForTables.IconVoid,
                IconsForTables.IconDraw),
            
            _ => new IconsStatisticTable(
                IconsForTables.IconVoid, 
                IconsForTables.IconVoid, 
                IconsForTables.IconVoid)
        };
    }
}

// ---------------- Запросить размер поля
int RequestBoardSize()
{
    while (true)
    {
        string sizeInput = RequestInput(UiText.StartMenuUi);

        int size;
        
        if (int.TryParse(sizeInput, out size) && size >= 3 && size <= 26)
        {
            Console.Clear();
            return size;
        }
        
        PrintInvalidMessage("Недопустимый размер поля \n");
    }
}

// ---------------- Запросить продолжение игры
bool RequestContinuation(string message)
{
    Console.Clear();
    
    string command = RequestValidCommand(message, CommandSets.ExitMenuCommand);
    return command == "Да";
}

// ---------------- Ввод валидной команды в меню
string RequestValidCommand(string message, string[] commands)
{
    do
    {
        string input = RequestInput(message).ToUpperInvariant();

        foreach (var command in commands)
        {
            if (String.Equals(command, input, StringComparison.OrdinalIgnoreCase))
                return command;
        }
        
        PrintInvalidMessage();
    } while (true);
}

// ---------------- Показать невалидное сообщение
void PrintInvalidMessage(string invalidMessage = UiText.StandardInvalidMessage)
{
    Console.Clear();
    Console.WriteLine(invalidMessage);
}

// ---------------- Запросить ввод
string RequestInput(string message)
{
    Console.Write(message);
    return  Console.ReadLine() ?? "";
}

// ------------------- Запрос хода и парсинг
ParseTurnCommandResult RequestAndParsePlayerMove(string[,] board, out CellCoordinates playersTurnCell)
{
    string command = RequestInput (UiText.CellSelectorUi).ToUpperInvariant();
    
    return ParsePlayerMove(command, board, out playersTurnCell);
}

// ------------------- Показ поля и чей ход
void PrintStartTurnInfo(string[,] board, bool isPlayersTurnX, DataCurrentPlayers data)
{
    PrintWhoseTurn(isPlayersTurnX, data);
    PrintBoard(board);
}

// ------------------- Выбор сообщения об ошибке исходя из самой ошибки
string GetParseErrorMassage(ParseTurnCommandResult parseResult)
{
    return parseResult switch
    {
        ParseTurnCommandResult.InvalidFormat => "Введены некорректные координаты клетки\n",
        ParseTurnCommandResult.OutOfBounds => "Введены координаты несуществующей клетки\n",
        ParseTurnCommandResult.CellOccupied => "Клетка уже занята\n",
        _ => ""
    };
}

// ------------------- Проверка состояния раунда
GameRoundResult GetRoundResult(string[,] board, bool isPlayersTurnX, bool player1IsX)
{
    if (IsWin(board))
    {
        return GetRoundWinner(isPlayersTurnX, player1IsX);
    }

    if (IsDraw(board))
    {
        return GameRoundResult.IsDeadHeat;
    }
    
    return GameRoundResult.InProgress;
    
    static GameRoundResult GetRoundWinner(bool isPlayersTurnX, bool player1IsX)
    {
        return player1IsX == isPlayersTurnX
            ? GameRoundResult.IsWinPlayer1
            : GameRoundResult.IsWinPlayer2;
    }
}

void PrintEndRoundMessage(string[,] board, GameRoundResult result, string player1, string player2)
{
    Console.Clear();
    PrintBoard(board);

    Console.WriteLine(GetResultRoundMassage(result, player1, player2));
    Console.WriteLine(UiText.StandardReadKyeMassage);
    
    Console.ReadLine();
    
    static string GetResultRoundMassage(GameRoundResult result, string player1, string player2)
    {
        return result switch
        {
            GameRoundResult.IsWinPlayer1 => $"Победа игрока 1({player1})",
            GameRoundResult.IsWinPlayer2 => $"Победа игрока 2({player2})",
            GameRoundResult.IsDeadHeat => "Ничья",
            _ => ""
        };
    }
}

// --------------- Обработка хода
ParseTurnCommandResult ParsePlayerMove(string command, string[,] boardLog, out CellCoordinates playersTurnCell)
{
    if (command == "МЕНЮ")
    {
        playersTurnCell = new CellCoordinates(-1, -1);
        return ParseTurnCommandResult.Menu;
    }
    
    if (!TryParseCellCoordinates(command, out playersTurnCell))
    {
        PrintInvalidMessage("Введены некорректные координаты клетки\n");
        return ParseTurnCommandResult.InvalidFormat;
    }

    if (!IsExistingCoordinates(playersTurnCell, boardLog))
    {
        PrintInvalidMessage("Введены координаты несуществующей клетки\n");
        return ParseTurnCommandResult.OutOfBounds;
    }

    if (!IsAcceptableMove(playersTurnCell, boardLog))
    {
        PrintInvalidMessage("Клетка уже занята\n");
        return ParseTurnCommandResult.CellOccupied;
    }
    
    return ParseTurnCommandResult.Parsed;
    
    static bool IsAcceptableMove(CellCoordinates coordinates, string[,] boardLog)
    {
        return boardLog[coordinates.Row, coordinates.Col] == IconsForTables.IconVoid;
    }
    
    static bool IsExistingCoordinates(CellCoordinates coordinates, string[,] board)
    {
        bool isExistingRow = coordinates.Row >= 0 && coordinates.Row < board.GetLength(0);
        bool isExistingCol = coordinates.Col >= 0 && coordinates.Col < board.GetLength(1);
        
        return isExistingRow && isExistingCol;
    }
}

// -------------- Проверки победы ----------------
bool IsWin(string[,] board)
{
    foreach (LineDirection direction in Enum.GetValues(typeof(LineDirection)))
    {
        if (IsWinLine(board, direction))
        {
            return true;
        }
    }
    return false;
    
    bool IsWinLine(string[,] board, LineDirection lineDirection)
    {
        int size = board.GetLength(0);
    
        for (int line = 0; line < size; line++)
        {
            CellCoordinates startCell = GetCell(lineDirection, line, 0, size);
    
            if (board[startCell.Row, startCell.Col] == IconsForTables.IconVoid)
            {
                continue;
            }
            
            bool isWin = true;
    
            for (int sideLine = 1; sideLine < size; sideLine++)
            {
                CellCoordinates currentCell = GetCell(lineDirection, line, sideLine, size);
                
                if (board[startCell.Row, startCell.Col] != board[currentCell.Row, currentCell.Col])
                {
                    isWin = false;
                    break;
                }
            }
    
            if (isWin)
            {
                return true;
            }
        }
        return false;
    }
}

// --------------- Проверки ничьей ----------------
bool IsDraw(string[,] board)
{
    foreach (LineDirection direction in Enum.GetValues(typeof(LineDirection)))
    {
        if (!IsDeadLine(board, direction))
        {
            return false;
        }
    }
    return true;
    
    bool IsDeadLine(string[,] board, LineDirection lineDirection)
    {
        int size = board.GetLength(0);
    
        for (int line = 0; line < size; line++)
        {
            bool hasX = false;
            bool has0 = false;
    
            for (int sideLine = 0; sideLine < size; sideLine++)
            {
                CellCoordinates currentCell = GetCell(lineDirection, line, sideLine, size);
                
                if (board[currentCell.Row, currentCell.Col] == IconsForTables.IconX)
                {
                    hasX = true;
                }
                
                else if (board[currentCell.Row, currentCell.Col] == IconsForTables.Icon0)
                {
                    has0 = true;
                }
    
                if (hasX && has0)
                {
                    break;
                }
            }
    
            if (hasX && has0)
            {
                return true;
            }
        }
        return false;
    }
}

// -------------- Проверка какая линия проверяется ------------
CellCoordinates GetCell(LineDirection lineDirection, int line, int sideLine, int size)
{
    return lineDirection switch
    {
        LineDirection.Horizontal => new CellCoordinates(line, sideLine),
        LineDirection.Vertical => new CellCoordinates(sideLine, line),
        LineDirection.DiagonalLeftToRight => new CellCoordinates(sideLine, sideLine),
        LineDirection.DiagonalRightToLeft => new CellCoordinates(sideLine, size - 1 - sideLine),
        _ => new CellCoordinates(0, 0)
    };
}

// ------------------- Применение изменений ----------
void ApplyBoardChanges(CellCoordinates coordinates, string[,] board, bool isPlayerTurnX)
{
    string currentPlayerIcon = isPlayerTurnX ? IconsForTables.IconX : IconsForTables.Icon0;
    
    board[coordinates.Row, coordinates.Col] = currentPlayerIcon;
}

// --------------- Парсинг хода
bool TryParseCellCoordinates(string command, out CellCoordinates coordinates)
{
    coordinates = new CellCoordinates(-1, -1);

    if (command.Length < 2 || command.Length > 3)
    {
        return false;
    }

    if (command[0] < 'A' || command[0] > 'Z')
    {
        return false;
    }

    int col = command[0] - 'A';

    int row;
    
    if (!Int32.TryParse(command.Substring(1), out row) || row <= 0)
    {
        return false;
    }

    row--;
    
    coordinates = new CellCoordinates(row, col);
    return true;
}

record TableHeaders(
    string Round,
    string Spacer,
    string Player1,
    string Player2,
    string Draw
);

record TableLayout(
    int Round,
    int Spacer,
    int Player1,
    int Player2,
    int Draw
);

record IconsStatisticTable(string P1, string P2, string Dh);

record CellCoordinates(int Row, int Col);

record GameStartSettings(string Player1Name, string Player2Name, int BoardSize);

// неиспользуемая вторая иконка оставлена на будущее. она тут логично сидит
record DataCurrentPlayers(string Player1Name, string Player2Name, string Player1Icon, string Player2Icon);

record RoundContext(int RoundNumber, bool Player1IsX);

record ScoresRounds(int P1, int P2, int Draw)
{
    public ScoresRounds WithResult(GameRoundResult result)
    {
        return result switch
        {
            GameRoundResult.IsWinPlayer1 => this with { P1 = P1 + 1 },
            GameRoundResult.IsWinPlayer2 => this with { P2 = P2 + 1 },
            GameRoundResult.IsDeadHeat   => this with { Draw = Draw + 1 },
            _ => this
        };
    }
};

enum GameRoundResult
{
    InProgress,
    IsWinPlayer1,
    IsWinPlayer2,
    IsDeadHeat,
    GoToMenu
}

enum LineDirection
{
    Horizontal,
    Vertical,
    DiagonalLeftToRight,
    DiagonalRightToLeft
}

enum ParseTurnCommandResult
{
    Parsed,
    Menu,
    InvalidFormat,
    OutOfBounds,
    CellOccupied
}

enum MainMenuCommands
{
    RestartRound,
    ResetGame,
    ExitGame
}

public static class UiText
{
    public const string StandardInvalidMessage = "Недопустимая команда \n";

    public const string StandardReadKyeMassage = "\nНажмите любую кнопку чтобы продолжить";
        
    public const string GameMenuUi = """
                                       Рестарт: перезапустить текущий раунд 
                                       Ресет: полностью перезагрузить игру
                                       Выход: завершить игру

                                       Введите команду из списка: 
                                       """;

    public const string StartMenuUi = "Введите размер поля от 3 до 26: ";

    public const string ExitRoundMenuUi = """
                                     Да: начать новый раунд
                                     Нет: выход из текущей игры
                                     
                                     Введите команду из списка: 
                                     """;
    
    public const string ExitGameMenuUi = """
                                          Да: начать новую игру
                                          Нет: закрытие игры
                                          
                                          Введите команду из списка: 
                                          """;
    
    public const string CellSelectorUi = """
                                         Указание клетки (Например A1): выбрать клетку для хода
                                         Меню: выход в главное меню (завершает текущий раунд)

                                         Введите команду из списка:  
                                         """;
}

public static class CommandSets
{
    public static readonly string[] GameMenuCommand =
    {
        "РЕСТАРТ",
        "РЕСЕТ",
        "ВЫХОД"
    };
    
    public static readonly string[] ExitMenuCommand =
    {
        "ДА",
        "НЕТ"
    };
}

public static class IconsForTables
{
    public const string IconVoid = "   ";
    public const string IconX = " X ";
    public const string Icon0 = " 0 ";
    
    public const string IconWin = "Win";
    public const string IconDraw = "Draw";
}