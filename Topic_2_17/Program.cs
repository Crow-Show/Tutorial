using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string moveCommandMessage = "w - движение вверх \n" +
                                  "s - движение вниз \n" +
                                  "a - движение влево \n" +
                                  "d - движение вправо \n" +
                                  "stop - завершить игру \n" +
                                  "Введите одну из команд: ";

const int mapHeight = 5;
const int mapWidth = 7;

const string characterIcon = "P";
const string voidIcon = " ";

CharacterLocation startLocation = new CharacterLocation(0, 0);

string[,] mapArr = MapInitializer();

RunGameLoop(mapArr);

Console.ReadKey();

string[,] MapInitializer()
{
    string[,] map = new string[mapHeight, mapWidth];
    
    int row = map.GetLength(0);
    int col = map.GetLength(1);

    for (int currentRow = 0; currentRow < row; currentRow++)
    {
        for (int currentCol = 0; currentCol < col; currentCol++)
        {
            map[currentRow, currentCol] = voidIcon;
        }
    }
    
    return map;
}

void MapBuildAndPrint(string[,] mapLog)
{
    var sb = new StringBuilder();

    string lineSeparator = "+" + new string('-', mapWidth * 2 - 1) + "+";
    sb.AppendLine(lineSeparator);

    for (int y = 0; y < mapLog.GetLength(0); y++)
    {
        sb.Append("|");
        for (int x = 0; x < mapLog.GetLength(1); x++)
        {
            sb.Append(mapLog[y, x]).Append("|");
        }

        sb.AppendLine();
        sb.AppendLine(lineSeparator);
    }
    
    string map = sb.ToString();
    Console.WriteLine(map);
}

void RunGameLoop(string[,] map)
{
    CharacterLocation currentLocation = startLocation;
    map[startLocation.Y, startLocation.X] = characterIcon;

    while (true)
    {
        MapBuildAndPrint(map);

        Direction direction = ConvertInputToDirectionCommand(ReadInput(moveCommandMessage));

        if (UserEnteredStop(direction))
        {
            Console.WriteLine("\nИгра завершена");
            return;
        }

        if (IsInvalidInput(direction) || IsGoingBeyondMap(direction))
        {
            PrintInvalidMessage();
            continue;
        }
        
        CharacterLocation newLocation = CalculateNewLocation(direction, currentLocation);

        ApplyChangeLocation(newLocation, ref currentLocation, map);
        
        Console.Clear();
    }


    bool UserEnteredStop(Direction direction)
    {
        return direction == Direction.Stop;
    }

    bool IsInvalidInput(Direction direction)
    {
        return direction == Direction.None;
    }

    bool IsGoingBeyondMap(Direction direction)
    {
        if (direction == Direction.Left) return currentLocation.X - 1 < 0;
        else if (direction == Direction.Right) return currentLocation.X + 1 >= mapWidth;
        else if (direction == Direction.Up) return currentLocation.Y - 1 < 0;
        else if (direction == Direction.Down) return currentLocation.Y + 1 >= mapHeight;
        
        else return false;
    }
    
    static void ApplyChangeLocation(CharacterLocation newLoc, ref CharacterLocation currentLoc, string[,] map)
    {
        map[currentLoc.Y, currentLoc.X] = voidIcon;
        map[newLoc.Y, newLoc.X] = characterIcon;
        currentLoc = newLoc;
    }
}

void PrintInvalidMessage()
    {
        Console.WriteLine("\nНевозможная команда. Введите команду движения или stop. \n" +
                          "Нажмите любую кнопку чтобы повторить ввод");
        
        Console.ReadKey();
        Console.Clear();
        
    }

CharacterLocation CalculateNewLocation(Direction direction, CharacterLocation currentLocation)
{
    switch (direction)
    {
        case Direction.Up: return new CharacterLocation(currentLocation.X, currentLocation.Y - 1);
        case Direction.Down: return new CharacterLocation(currentLocation.X, currentLocation.Y + 1);
        case Direction.Left: return new CharacterLocation(currentLocation.X - 1, currentLocation.Y);
        case Direction.Right: return new CharacterLocation(currentLocation.X + 1, currentLocation.Y);
        default: return new CharacterLocation(currentLocation.X, currentLocation.Y);
    }
}

string ReadInput(string message)
{
    Console.Write(message);
    return Console.ReadLine() ?? "";
}

Direction ConvertInputToDirectionCommand(string input)
{
    switch (input)
    {
        case "w": return Direction.Up;
        case "s": return Direction.Down;
        case "a": return Direction.Left;
        case "d": return Direction.Right;
        case "stop": return Direction.Stop;
        default: return Direction.None;
    }
}

enum Direction
{
    None,
    Stop,
    Up,
    Down,
    Left,
    Right
}

record CharacterLocation(int X, int Y);