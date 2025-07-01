using SkyRez.Pathfinder;
using System.Drawing;

internal class Program
{
    private static void Main()
    {
        bool[,] maze = new bool[20, 20]
        {
            { true, true, true, false, true, true, true, true, true, false, true, true, true, true, true, false, true, true, true, true },
            { true, false, true, false, true, false, false, false, true, false, true, false, false, false, true, false, false, false, true, true },
            { true, false, true, true, true, true, true, false, true, true, true, true, true, false, true, true, true, false, true, true },
            { true, false, false, false, false, false, true, false, false, false, false, false, true, false, false, false, true, false, false, true },
            { true, true, true, true, true, false, true, true, true, true, true, false, true, true, true, false, true, true, true, true },
            { false, false, false, false, true, false, false, false, false, false, true, false, false, false, true, false, false, false, false, true },
            { true, true, true, false, true, true, true, true, true, false, true, true, true, false, true, true, true, true, false, true },
            { true, false, true, false, false, false, false, false, true, false, false, false, true, false, false, false, false, true, false, true },
            { true, false, true, true, true, true, true, false, true, true, true, false, true, true, true, true, false, true, true, true },
            { true, false, false, false, false, false, true, false, false, false, true, false, false, false, false, true, false, false, false, true },
            { true, true, true, true, true, false, true, true, true, false, true, true, true, true, false, true, true, true, true, true },
            { true, false, false, false, true, false, false, false, true, false, false, false, false, true, false, false, false, false, false, true },
            { true, true, true, false, true, true, true, false, true, true, true, true, false, true, true, true, true, true, false, true },
            { true, false, true, false, false, false, true, false, false, false, false, true, false, false, false, false, false, true, false, true },
            { true, false, true, true, true, false, true, true, true, true, false, true, true, true, true, true, false, true, true, true },
            { true, false, false, false, true, false, false, false, false, true, false, false, false, false, false, true, false, false, false, true },
            { true, true, true, false, true, true, true, true, false, true, true, true, true, true, false, true, true, true, true, true },
            { true, false, true, false, false, false, false, true, false, false, false, false, false, true, false, false, false, false, true, true },
            { true, false, true, true, true, true, false, true, true, true, true, true, false, true, true, true, true, false, true, true },
            { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true }
        };
        Point start = new(0, 0);
        Point end = new(5, 12);
        Queue<Point> path = Pathfinder.FindPath(start, end, maze);
        Console.WriteLine(string.Join(" -> ", path));
        Pathfinder.PrintMap(maze, path);
        int[,] mazeInt = new int[30, 30]
        {
            {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
            {10,-3, 1, 2, 3, 4, 5,10, 6, 7, 8,10, 9, 0,-1,10, 1, 2, 3,10, 4, 5, 6,10, 7, 8, 9,10, 0,10},
            {10,-4, 2, 3, 4, 5, 6, 1, 7, 8, 9, 1, 0,-1,-2, 1, 2, 3, 4, 1, 5, 6, 7, 1, 8, 9, 0, 1,-1,10},
            {10,-5, 3, 4, 5, 6, 7,10, 8, 9, 0,10,-1,-2,-3,10, 3, 4, 5,10, 6, 7, 8,10, 9, 0,-1,10,-2,10},
            {10,10,10, 1,10,10,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10,10},
            {10,-6, 4, 5, 6, 7, 8,10, 9, 0,-1,10,-2,-3,-4,10, 4, 5, 6,10, 7, 8, 9,10, 0,-1,-2,10,-3,10},
            {10,-7, 5, 6, 7, 8, 9, 1, 0,-1,-2, 1,-3,-4,-5, 1, 5, 6, 7, 1, 8, 9, 0, 1,-1,-2,-3, 1,-4,10},
            {10,-8, 6, 7, 8, 9, 0,10,-1,-2,-3,10,-4,-5,-6,10, 6, 7, 8,10, 9, 0,-1,10,-2,-3,-4,10,-5,10},
            {10,-9, 7, 8, 9, 0,-1,-1,-2,-3,-4,10,-5,-6,-7,10, 7, 8, 9,10, 0,-1,-2,10,-3,-4,-5,10,-6,10},
            {10,10, 1,10,10,-1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10,10},
            {10, 0,-1,-2,-3,-4,-5,10,-6,-7,-8,10,-9, 1, 2,10, 3, 4, 5,10, 6, 7, 8,10, 9, 0,-1,10,-2,10},
            {10, 1,-2,-3,-4,-5,-6, 1,-7,-8,-9,10, 0, 2, 3,10, 4, 5, 6,10, 7, 8, 9,10, 0, 1, 2,10,-3,10},
            {10, 2,-3,-4,-5,-6,-7,10,-8,-9, 0, 1, 1, 3, 4,-1, 5, 6, 7, 1, 8, 9, 0, 1, 1, 2, 3, 1,-4,10},
            {10, 3,-4,-5,-6,-7,-8,10,-9, 0, 1,10, 2, 4, 5,10, 6, 7, 8,10, 9, 0, 1,10, 2, 3, 4,10,-5,10},
            {10,10,10, 1,10,10,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10,10},
            {10, 4, 5, 6, 7, 8, 9,10, 0,-1,-2,10,-3,-4,-5,10, 5, 6, 7,10, 8, 9, 0,10,-1,-2,-3,10,-4,10},
            {10, 5, 6, 7, 8, 9, 0, 1,-1,-2,-3,10,-4,-5,-6,10, 6, 7, 8,10, 9, 0,-1,10,-2,-3,-4,10,-5,10},
            {10, 6, 7, 8, 9, 0,-1,10,-2,-3,-4, 1,-5,-6,-7, 1, 7, 8, 9, 1, 0,-1,-2,-1,-3,-4,-5, 1,-6,10},
            {10, 7, 8, 9, 0,-1,-2,10,-3,-4,-5,10,-6,-7,-8,10, 8, 9, 0,10,-1,-2,-3,10,-4,-5,-6,10,-7,10},
            {10,10,10,10, 1,10,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10,-1,10,10,10, 1,10,10,10,10},
            {10, 8, 9, 0,-1,-2,-3,10,-4,-5,-6,10,-7,-8,-9,10, 9, 0, 1,10, 2, 3, 4,10, 5, 6, 7,10,-8,10},
            {10, 9, 0,-1,-2,-3,-4, 1,-5,-6,-7,10,-8,-9, 0,10, 1, 2, 3,10, 4, 5, 6,10, 7, 8, 9,10,-9,10},
            {10, 0,-1,-2,-3,-4,-5,10,-6,-7,-8, 1,-9, 0, 1, 1, 2, 3, 4, 1, 5, 6, 7, 1, 8, 9, 0, 1, 1,10},
            {10, 1,-2,-3,-4,-5,-6,10,-7,-8,-9,10, 0, 1, 2,10, 3, 4, 5,10, 6, 7, 8,10, 9, 0, 1,10, 2,10},
            {10,10,10, 1,10,10,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10, 1,10,10,10,10},
            {10, 2, 3, 4, 5, 6, 7,10, 8, 9, 0,10, 1, 2, 3,10, 4, 5, 6,10, 7, 8, 9,10, 0, 1, 2,10, 3,10},
            {10, 3, 4, 5, 6, 7, 8, 1, 9, 0, 1,10, 2, 3, 4,10, 5, 6, 7,10, 8, 9, 0,10, 1, 2, 3,10, 4,10},
            {10, 4, 5, 6, 7, 8, 9,10, 0, 1, 2, 1, 3, 4, 5, 1, 6, 7, 8, 1, 9, 0, 1, 1, 2, 3, 4, 1, 5,10},
            {10, 5, 6, 7, 8, 9, 0,10, 1, 2, 3,10, 4, 5, 6,10, 7, 8, 9,10, 0, 1, 2,10, 3, 4, 5,10, 6,10},
            {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10}
        };
        start = new(1, 1);
        end = new(28, 28);
        path = Pathfinder.FindPath(start, end, mazeInt);
        Console.WriteLine(string.Join(" -> ", path));
        Pathfinder.PrintMap(mazeInt, path);
    }
}