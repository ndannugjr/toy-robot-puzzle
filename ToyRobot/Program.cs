
int width = 4;
int height = 4;
var tableCoordinates = GetTableCoordinates(width, height);

bool isValid = false;
string positionCommand = string.Empty;

Placing placing = new();

while (!isValid)
{
    string errorMsg = string.Empty;

    Console.WriteLine("Place your toy (PLACE X,Y,F): ");
    positionCommand = Console.ReadLine();

    placing = PlaceCommandValidator(tableCoordinates, positionCommand, Direction.NA, ref errorMsg);

    if (errorMsg == string.Empty)
        break;
    else
        Console.WriteLine(errorMsg);
}


while (true)
{
    string errorMsg = string.Empty;

    Console.WriteLine("Enter a command: ");
    string strCommand = Console.ReadLine();
    if (strCommand.Contains("PLACE"))
    {
        var tempPlacing = PlaceCommandValidator(tableCoordinates, strCommand, placing.F, ref errorMsg, false);
        if (errorMsg != string.Empty)
            Console.WriteLine(errorMsg);
        else
        {
            placing = tempPlacing;
            Console.WriteLine("-- Place command detected --");
        }
    }
    else
    {
        if (Enum.TryParse(strCommand, out Command command))
        {
            if (command == Command.MOVE)
            {
                var tempPlacing = MoveCommand(placing.X, placing.Y, placing.F);
                if (VerifyCoordinates(tableCoordinates, tempPlacing))
                {
                    placing = tempPlacing;
                }
            }
            else if (command == Command.LEFT)
            {
                placing.F = LeftCommand(placing.F);
            }
            else if (command == Command.RIGHT)
            {
                placing.F = RightCommand(placing.F);
            }
            else
            {
                Console.WriteLine($"{placing.X}, {placing.Y} {placing.F}");
            }

            Console.WriteLine("-- " + command + " command detected --");
        }
        else
            Console.WriteLine("Invalid command");
    }
}

static Placing PlaceCommandValidator(List<int[]> tableCoordinates, string positionCommand, Direction direction,
    ref string errorMsg, bool isDirectionRequired = true)
{
    Placing placing = new();
    string[] p = positionCommand.Split([' ', ',']);

    int x = 0;
    int y = 0;

    bool isValidLength = isDirectionRequired ? p.Length == 4 : p.Length == 3 || p.Length == 4;

    if (positionCommand.Where(s => s.Equals(' ')).Count() == 1 && isValidLength)
    {
        if (p[0] == "PLACE")
        {
            x = ValidateCoordinates(tableCoordinates, p[1], 0);
            if (x < 0)
                errorMsg = "Coordinate out of bounds!";

            y = ValidateCoordinates(tableCoordinates, p[2], 1);
            if (y < 0)
                errorMsg = "Coordinate out of bounds!";

            if (p.Length > 3)
            {
                if (!Enum.TryParse(p[3], out direction))
                {
                    errorMsg = "Invalid facing direction";
                }
            }

            if (errorMsg != string.Empty)
                return placing;
            else
                return new Placing() { X = x, Y = y, F = direction };

        }
        else
            errorMsg = "Invalid Place Command (e.g. PLACE 1,2,NORTH)";
    }
    else
    {
        errorMsg = "Invalid Place Command (e.g. PLACE 1,2,NORTH)";
    }

    return placing;
}

static int ValidateCoordinates(List<int[]> tableCoordinates, string coordinate, int intIndex)
{
    int intCoordinate = -1;
    if (int.TryParse(coordinate, out intCoordinate))
    {
        if (!tableCoordinates.Any(s => s[intIndex] == intCoordinate))
            intCoordinate = -1;
    }
    else
        intCoordinate = -1;

    return intCoordinate;
}

static bool VerifyCoordinates(List<int[]> tableCoordinates, Placing placing)
{
    return tableCoordinates.Any(s => s.SequenceEqual(new int[] { placing.X, placing.Y }));
}

static Direction LeftCommand(Direction f)
{
    switch (f)
    {
        case Direction.NORTH:
            f = Direction.WEST;
            break;
        case Direction.SOUTH:
            f = Direction.EAST;
            break;
        case Direction.EAST:
            f = Direction.NORTH;
            break;
        case Direction.WEST:
            f = Direction.SOUTH;
            break;
    }

    return f;
}

static Direction RightCommand(Direction f)
{
    switch (f)
    {
        case Direction.NORTH:
            f = Direction.EAST;
            break;
        case Direction.SOUTH:
            f = Direction.WEST;
            break;
        case Direction.EAST:
            f = Direction.SOUTH;
            break;
        case Direction.WEST:
            f = Direction.NORTH;
            break;
    }

    return f;
}

static Placing MoveCommand(int positionX, int positionY, Direction direction)
{
    var currentCoordinates = new int[] { positionX, positionY };
    int[] newCoordinates = new int[2];
    switch (direction)
    {
        case Direction.NORTH:
            newCoordinates = MoveNorth(currentCoordinates);
            break;
        case Direction.SOUTH:
            newCoordinates = MoveSouth(currentCoordinates);
            break;
        case Direction.EAST:
            newCoordinates = MoveEast(currentCoordinates);
            break;
        case Direction.WEST:
            newCoordinates = MoveWest(currentCoordinates);
            break;
    }

    return new Placing() { X = newCoordinates[0], Y = newCoordinates[1], F = direction };
}

static int[] MoveNorth(int[] coordinates)
{
    return [coordinates[0], coordinates[1] + 1];
}

static int[] MoveSouth(int[] coordinates)
{
    return [coordinates[0], coordinates[1] - 1];
}

static int[] MoveWest(int[] coordinates)
{
    return [coordinates[0] - 1, coordinates[1]];
}

static int[] MoveEast(int[] coordinates)
{
    return [coordinates[0] + 1, coordinates[1]];
}

static List<int[]> GetTableCoordinates(int width, int height)
{
    List<int[]> coordinates = [];

    for (int x = 0; x <= width; x++)
    {
        for (int y = 0; y <= height; y++)
        {
            coordinates.Add([x, y]);
        }
    }

    return coordinates;
}

public class Placing
{
    public int X { get; set; }
    public int Y { get; set; }
    public Direction F { get; set; }
}

public enum Command
{
    MOVE,
    LEFT,
    RIGHT,
    REPORT
}

public enum Direction
{
    NORTH,
    SOUTH,
    EAST,
    WEST,
    NA
}