using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToyRobotPuzzle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int width = 4;
            int height = 4;
            var tableCoordinates = GetTableCoordinates(width, height);

            bool isValid = false;
            string positionCommand = string.Empty;

            Placing placing = new Placing();

            while (!isValid)
            {
                string errorMsg = string.Empty;

                Console.WriteLine("Place your toy (PLACE X,Y,F): ");
                positionCommand = Console.ReadLine();

                placing = PlaceCommandValidator(tableCoordinates, positionCommand, ref errorMsg);

                if (errorMsg == string.Empty)
                    break;
            }


            while (true)
            {
                Console.WriteLine("Enter a command (MOVE, LEFT, RIGHT, REPORT): ");
                string strCommand = Console.ReadLine();
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
                        Console.WriteLine($"{placing.X}, {placing.Y} {placing.F.ToString()}");
                    }
                }
            }
        }

        private static Placing PlaceCommandValidator(List<int[]> tableCoordinates, string positionCommand, ref string errorMsg)
        {
            Placing placing = new Placing();
            var p = positionCommand.Split(new char[] { ' ', ',' });
            if (positionCommand.Where(s => s.Equals(' ')).Count() == 1 && p.Length == 4)
            {
                if (p[0] == "PLACE")
                {
                    placing.X = ValidateCoordinates(tableCoordinates, p[1], 0);
                    if (placing.X < 0)
                        errorMsg = "Coordinate out of bounds!";

                    placing.Y = ValidateCoordinates(tableCoordinates, p[2], 1);
                    if (placing.Y < 0)
                        errorMsg = "Coordinate out of bounds!";

                    if (!Enum.TryParse(p[3], out Direction direction))
                        errorMsg = "Invalid facing direction";
                    else
                        placing.F = direction;

                    if (errorMsg != string.Empty)
                        return new Placing();

                }
                else
                    Console.WriteLine("Invalid Place Command (e.g. PLACE 1,2,NORTH)");
            }
            else
            {
                Console.WriteLine("Invalid Place Command (e.g. PLACE 1,2,NORTH)");
            }

            return placing;
        }

        private static int ValidateCoordinates(List<int[]> tableCoordinates, string coordinate, int intIndex)
        {
            int intCoordinate = -1;
            if (int.TryParse(coordinate, out intCoordinate) 
                && !tableCoordinates.Any(s => s[intIndex] == intCoordinate))
            {
                intCoordinate = -1;
            }

            return intCoordinate;
        }

        private static bool VerifyCoordinates(List<int[]> tableCoordinates, Placing placing)
        {
            return tableCoordinates.Any(s => s.SequenceEqual(new int[] { placing.X, placing.Y }));
        }

        private static Direction LeftCommand(Direction f)
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

        private static Direction RightCommand(Direction f)
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

        private static Placing MoveCommand(int positionX, int positionY, Direction direction)
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

        public static int[] MoveNorth(int[] coordinates)
        {
            return new int[] { coordinates[0], coordinates[1] + 1 };
        }

        public static int[] MoveSouth(int[] coordinates)
        {
            return new int[] { coordinates[0], coordinates[1] - 1 };
        }

        public static int[] MoveWest(int[] coordinates)
        {
            return new int[] { coordinates[0] - 1, coordinates[1] };
        }

        public static int[] MoveEast(int[] coordinates)
        {
            return new int[] { coordinates[0] + 1, coordinates[1] };
        }

        private static List<int[]> GetTableCoordinates(int width, int height)
        {
            List<int[]> coordinates = new List<int[]>();

            int count = 0;
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    coordinates.Add(new int[] { x, y });
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
            WEST
        }
    }
}
