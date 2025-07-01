namespace SkyRez.Pathfinder;

/// <summary>Статический класс для поиска пути на двумерной карте.</summary>
/// <remarks>Использует алгоритмы поиска пути, такие как A*.<br/>Может быть расширен для поддержки других алгоритмов.</remarks>
public static class Pathfinder
{
    /// <summary>Выполняет поиск кратчайшего пути между двумя точками на карте типа bool[,].</summary>
    /// <param name="start">Начальная точка поиска пути.</param>
    /// <param name="end">Конечная точка поиска пути.</param>
    /// <param name="map">Двумерный булевый массив, где <c>true</c> означает проходимую клетку, а <c>false</c> — препятствие.</param>
    /// <param name="invertXY">Флаг, указывающий, следует ли инвертировать оси X и Y при обработке карты.</param>
    /// <returns>Список точек типа <see cref="Point"/>, представляющий найденный путь от начальной до конечной точки.<br/>Если путь не найден, возвращается пустой список.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если начальная или конечная точка выходит за пределы карты.</exception>
    /// <exception cref="ArgumentNullException">Выбрасывается, если карта <c>map</c> равна <c>null</c>.</exception>
    /// <remarks>Использует <see cref="List{T}"/> для хранения последовательности точек пути.<br/>Реализует алгоритм поиска A* для поиска кратчайшего пути.</remarks>
    public static List<Point> FindPath(Point start, Point end, bool[,] map, bool invertXY = true)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(map);
#else
        if (map is null) throw new ArgumentNullException(nameof(map));
#endif
        int height = map.GetLength(0); // строки (Y)
        int width = map.GetLength(1);  // столбцы (X)

        /// <summary>Проверяет, является ли клетка проходимой с учётом инверсии осей.</summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Значение типа <c>bool</c>, указывающее, проходима ли клетка.</returns>
        bool IsWalkable(int x, int y) =>
            invertXY ? map[y, x] : map[x, y];

        /// <summary>Проверяет, находится ли клетка в пределах карты с учётом инверсии осей.</summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Значение типа <c>bool</c>, указывающее, находится ли клетка в пределах карты.</returns>
        bool InBounds(int x, int y) =>
            invertXY ? (x >= 0 && x < width && y >= 0 && y < height)
                     : (y >= 0 && y < height && x >= 0 && x < width);

        if (!InBounds(start.X, start.Y)) throw new ArgumentOutOfRangeException(nameof(start));
        if (!InBounds(end.X, end.Y)) throw new ArgumentOutOfRangeException(nameof(end));
        if (!IsWalkable(start.X, start.Y) || !IsWalkable(end.X, end.Y)) return [];

        SortedSet<(int f, int h, Point point)> openSet = new(Comparer<(int f, int h, Point point)>.Create((a, b) =>
        {
            int cmp = a.f.CompareTo(b.f);
            if (cmp == 0) cmp = a.h.CompareTo(b.h);
            if (cmp == 0) cmp = a.point.X.CompareTo(b.point.X);
            if (cmp == 0) cmp = a.point.Y.CompareTo(b.point.Y);
            return cmp;
        }));
        Dictionary<Point, Point> cameFrom = [];
        Dictionary<Point, int> gScore = new()
        {
            [start] = 0
        };

        /// <summary>Вычисляет расстояние Чебышёва между двумя точками.</summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Расстояние Чебышёва между точками типа <c>int</c>.</returns>
        int Heuristic(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            return 1000 * Math.Max(dx, dy); // Chebyshev distance
        }

        openSet.Add((Heuristic(start, end), Heuristic(start, end), start));

        (int dx, int dy)[] directions =
        [
            (0, -1), (0, 1), (-1, 0), (1, 0),
            (-1, -1), (1, -1), (-1, 1), (1, 1)
        ];

        while (openSet.Count > 0)
        {
            Point current = openSet.Min.point;
            if (current == end)
            {
                List<Point> path = [];
                while (cameFrom.ContainsKey(current))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }
            openSet.Remove(openSet.Min);

            foreach ((int dx, int dy) in directions)
            {
                int nx = current.X + dx;
                int ny = current.Y + dy;
                if (!InBounds(nx, ny) || !IsWalkable(nx, ny)) continue;

                // Защита от среза углов
                if (dx != 0 && dy != 0)
                {
                    if (!IsWalkable(current.X + dx, current.Y) || !IsWalkable(current.X, current.Y + dy))
                        continue;
                }

                Point neighbor = new(nx, ny);
                int stepCost = (dx != 0 && dy != 0) ? 1414 : 1000; // 1.414 и 1.0, умноженные на 1000
                int tentativeG = gScore[current] + stepCost;
                if (!gScore.TryGetValue(neighbor, out int neighborG) || tentativeG < neighborG)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    int f = tentativeG + Heuristic(neighbor, end);
                    openSet.Add((f, Heuristic(neighbor, end), neighbor));
                }
            }
        }
        return [];
    }

    /// <summary>Выполняет поиск кратчайшего пути между двумя точками на карте типа float[,].</summary>
    /// <param name="start">Начальная точка поиска пути.</param>
    /// <param name="end">Конечная точка поиска пути.</param>
    /// <param name="map">Двумерный массив типа float, где максимальное значение считается стеной.</param>
    /// <param name="invertXY">Флаг, указывающий, следует ли инвертировать оси X и Y при обработке карты.</param>
    /// <returns>Список точек типа <see cref="Point"/>, представляющий найденный путь от начальной до конечной точки.<br/>Если путь не найден, возвращается пустой список.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если начальная или конечная точка выходит за пределы карты.</exception>
    /// <exception cref="ArgumentNullException">Выбрасывается, если карта <c>map</c> равна <c>null</c>.</exception>
    /// <remarks>Использует <see cref="List{T}"/> для хранения последовательности точек пути.<br/>Реализует алгоритм поиска A* с учётом стоимости клетки.</remarks>
    public static List<Point> FindPath(Point start, Point end, float[,] map, bool invertXY = true)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(map);
#else
        if (map is null) throw new ArgumentNullException(nameof(map));
#endif
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        float min = float.MaxValue, max = float.MinValue;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float v = invertXY ? map[y, x] : map[x, y];
                if (v < min) min = v;
                if (v > max) max = v;
            }

        /// <summary>Проверяет, является ли клетка стеной.</summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Значение типа <c>bool</c>, указывающее, является ли клетка стеной.</returns>
        bool IsWall(int x, int y)
        {
            float v = invertXY ? map[y, x] : map[x, y];
            return v >= max;
        }

        /// <summary>Проверяет, находится ли клетка в пределах карты с учётом инверсии осей.</summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Значение типа <c>bool</c>, указывающее, находится ли клетка в пределах карты.</returns>
        bool InBounds(int x, int y) =>
            invertXY ? (x >= 0 && x < width && y >= 0 && y < height)
                     : (y >= 0 && y < height && x >= 0 && x < width);

        if (!InBounds(start.X, start.Y)) throw new ArgumentOutOfRangeException(nameof(start));
        if (!InBounds(end.X, end.Y)) throw new ArgumentOutOfRangeException(nameof(end));
        if (IsWall(start.X, start.Y) || IsWall(end.X, end.Y)) return [];

        SortedSet<(float f, float h, Point point)> openSet = new(Comparer<(float f, float h, Point point)>.Create((a, b) =>
        {
            int cmp = a.f.CompareTo(b.f);
            if (cmp == 0) cmp = a.h.CompareTo(b.h);
            if (cmp == 0) cmp = a.point.X.CompareTo(b.point.X);
            if (cmp == 0) cmp = a.point.Y.CompareTo(b.point.Y);
            return cmp;
        }));
        Dictionary<Point, Point> cameFrom = [];
        Dictionary<Point, float> gScore = new()
        {
            [start] = 0
        };

        /// <summary>Вычисляет расстояние Чебышёва между двумя точками.</summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Расстояние Чебышёва между точками типа <c>float</c>.</returns>
        float Heuristic(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            return Math.Max(dx, dy); // Chebyshev distance
        }

        openSet.Add((Heuristic(start, end), Heuristic(start, end), start));

        (int dx, int dy)[] directions =
        [
            (0, -1), (0, 1), (-1, 0), (1, 0),
            (-1, -1), (1, -1), (-1, 1), (1, 1)
        ];

        while (openSet.Count > 0)
        {
            Point current = openSet.Min.point;
            if (current == end)
            {
                List<Point> path = [];
                while (cameFrom.ContainsKey(current))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }
            openSet.Remove(openSet.Min);

            foreach ((int dx, int dy) in directions)
            {
                int nx = current.X + dx;
                int ny = current.Y + dy;
                if (!InBounds(nx, ny) || IsWall(nx, ny)) continue;

                // Защита от среза углов
                if (dx != 0 && dy != 0)
                {
                    if (IsWall(current.X + dx, current.Y) || IsWall(current.X, current.Y + dy))
                        continue;
                }

                Point neighbor = new(nx, ny);
                float v = invertXY ? map[ny, nx] : map[nx, ny];
                float norm = (max - min) > 0 ? (v - min) / (max - min) : 0;
                float stepCost = (dx != 0 && dy != 0) ? 1.4142f : 1f;
                float cost = stepCost + norm;

                float tentativeG = gScore[current] + cost;
                if (!gScore.TryGetValue(neighbor, out float neighborG) || tentativeG < neighborG)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    float f = tentativeG + Heuristic(neighbor, end);
                    openSet.Add((f, Heuristic(neighbor, end), neighbor));
                }
            }
        }
        return [];
    }

    /// <summary>Выполняет поиск кратчайшего пути между двумя точками на карте типа int[,].</summary>
    /// <param name="start">Начальная точка поиска пути.</param>
    /// <param name="end">Конечная точка поиска пути.</param>
    /// <param name="map">Двумерный массив типа int, где максимальное значение считается стеной.</param>
    /// <param name="invertXY">Флаг, указывающий, следует ли инвертировать оси X и Y при обработке карты.</param>
    /// <returns>Список точек типа <see cref="Point"/>, представляющий найденный путь от начальной до конечной точки.<br/>Если путь не найден, возвращается пустой список.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если начальная или конечная точка выходит за пределы карты.</exception>
    /// <exception cref="ArgumentNullException">Выбрасывается, если карта <c>map</c> равна <c>null</c>.</exception>
    /// <remarks>Использует <see cref="List{T}"/> для хранения последовательности точек пути.<br/>Реализует алгоритм поиска A* с учётом стоимости клетки.</remarks>
    public static List<Point> FindPath(Point start, Point end, int[,] map, bool invertXY = true)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(map);
#else
        if (map is null) throw new ArgumentNullException(nameof(map));
#endif
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        int min = int.MaxValue, max = int.MinValue;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int v = invertXY ? map[y, x] : map[x, y];
                if (v < min) min = v;
                if (v > max) max = v;
            }

        /// <summary>Проверяет, является ли клетка стеной.</summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Значение типа <c>bool</c>, указывающее, является ли клетка стеной.</returns>
        bool IsWall(int x, int y)
        {
            int v = invertXY ? map[y, x] : map[x, y];
            return v >= max;
        }

        /// <summary>Проверяет, находится ли клетка в пределах карты с учётом инверсии осей.</summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Значение типа <c>bool</c>, указывающее, находится ли клетка в пределах карты.</returns>
        bool InBounds(int x, int y) =>
            invertXY ? (x >= 0 && x < width && y >= 0 && y < height)
                     : (y >= 0 && y < height && x >= 0 && x < width);

        if (!InBounds(start.X, start.Y)) throw new ArgumentOutOfRangeException(nameof(start));
        if (!InBounds(end.X, end.Y)) throw new ArgumentOutOfRangeException(nameof(end));
        if (IsWall(start.X, start.Y) || IsWall(end.X, end.Y)) return [];

        SortedSet<(float f, float h, Point point)> openSet = new(Comparer<(float f, float h, Point point)>.Create((a, b) =>
        {
            int cmp = a.f.CompareTo(b.f);
            if (cmp == 0) cmp = a.h.CompareTo(b.h);
            if (cmp == 0) cmp = a.point.X.CompareTo(b.point.X);
            if (cmp == 0) cmp = a.point.Y.CompareTo(b.point.Y);
            return cmp;
        }));
        Dictionary<Point, Point> cameFrom = [];
        Dictionary<Point, float> gScore = new()
        {
            [start] = 0
        };

        /// <summary>Вычисляет расстояние Чебышёва между двумя точками.</summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Расстояние Чебышёва между точками типа <c>float</c>.</returns>
        float Heuristic(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            return Math.Max(dx, dy); // Chebyshev distance
        }

        openSet.Add((Heuristic(start, end), Heuristic(start, end), start));

        (int dx, int dy)[] directions =
        [
            (0, -1), (0, 1), (-1, 0), (1, 0),
            (-1, -1), (1, -1), (-1, 1), (1, 1)
        ];

        while (openSet.Count > 0)
        {
            Point current = openSet.Min.point;
            if (current == end)
            {
                List<Point> path = [];
                while (cameFrom.ContainsKey(current))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }
            openSet.Remove(openSet.Min);

            foreach ((int dx, int dy) in directions)
            {
                int nx = current.X + dx;
                int ny = current.Y + dy;
                if (!InBounds(nx, ny) || IsWall(nx, ny)) continue;

                if (dx != 0 && dy != 0)
                {
                    if (IsWall(current.X + dx, current.Y) || IsWall(current.X, current.Y + dy))
                        continue;
                }

                Point neighbor = new(nx, ny);
                int v = invertXY ? map[ny, nx] : map[nx, ny];
                float norm = (max - min) > 0 ? (v - min) / (float)(max - min) : 0;
                float stepCost = (dx != 0 && dy != 0) ? 1.4142f : 1f;
                float cost = stepCost + norm;

                float tentativeG = gScore[current] + cost;
                if (!gScore.TryGetValue(neighbor, out float neighborG) || tentativeG < neighborG)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    float f = tentativeG + Heuristic(neighbor, end);
                    openSet.Add((f, Heuristic(neighbor, end), neighbor));
                }
            }
        }
        return [];
    }

    /// <summary>Возвращает цвет консоли для значения карты на основе градиента.</summary>
    /// <param name="value">Значение клетки.</param>
    /// <param name="min">Минимальное значение на карте.</param>
    /// <param name="max">Максимальное значение на карте.</param>
    /// <returns>Цвет типа <see cref="ConsoleColor"/>, соответствующий значению клетки.</returns>
    /// <remarks>Используется для визуализации float и int карт.<br/>Чем выше значение, тем "горячее" цвет.</remarks>
    private static ConsoleColor GetGradientColor(float value, float min, float max)
    {
        if (value is 0) return ConsoleColor.White;
        float norm = (max - min) > 0 ? (value - min) / (max - min) : 0;
        return norm switch
        {
            < 0.1f => ConsoleColor.DarkGreen,
            < 0.2f => ConsoleColor.Green,
            < 0.3f => ConsoleColor.DarkCyan,
            < 0.4f => ConsoleColor.Cyan,
            < 0.5f => ConsoleColor.Blue,
            < 0.6f => ConsoleColor.DarkYellow,
            < 0.7f => ConsoleColor.Yellow,
            < 0.8f => ConsoleColor.DarkRed,
            _ => ConsoleColor.Red
        };
    }

    /// <summary>Визуализирует карту типа bool[,] в консоли с использованием символов.<br/>░ — пустая клетка, ▓ — стена, путь — фиолетовые ▒.</summary>
    /// <param name="map">Двумерный булевый массив, где <c>true</c> — проходимо, <c>false</c> — препятствие.</param>
    /// <param name="path">Необязательный путь для отображения типа <see cref="IEnumerable{Point}"/> (может быть <c>null</c>).</param>
    /// <param name="invertXY">Флаг инверсии осей, как в FindPath.</param>
    /// <remarks>Использует <see cref="Console"/> для вывода карты.<br/>Путь отображается цветом Magenta.</remarks>
    public static void PrintMap(bool[,] map, IEnumerable<Point>? path = null, bool invertXY = true)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (map == null) throw new ArgumentNullException(nameof(map));
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        HashSet<Point>? pathSet = path is not null ? [.. path] : null;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int px = invertXY ? x : y;
                int py = invertXY ? y : x;
                bool walkable = map[py, px];
                Point pt = new(x, y);

                if (!walkable)
                {
                    Console.ForegroundColor = ConsoleColor.White; // Стена — белая
                    Console.Write("▓▓");
                }
                else if (pathSet is not null && pathSet.Contains(pt))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta; // Путь — фиолетовый
                    Console.Write("▒▒");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("░░");
                }
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    /// <summary>Визуализирует карту типа float[,] в консоли с использованием символов.<br/>░ — пустая клетка, ▓ — стена, путь — фиолетовые ▒.</summary>
    /// <param name="map">Двумерный массив типа float, где максимальное значение считается стеной.</param>
    /// <param name="path">Необязательный путь для отображения типа <see cref="IEnumerable{Point}"/> (может быть <c>null</c>).</param>
    /// <param name="invertXY">Флаг инверсии осей, как в FindPath.</param>
    /// <remarks>Использует <see cref="Console"/> для вывода карты.<br/>Путь отображается цветом Magenta.<br/>Цвет клетки зависит от значения и рассчитывается через <see cref="GetGradientColor"/>.</remarks>
    public static void PrintMap(float[,] map, IEnumerable<Point>? path = null, bool invertXY = true)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (map == null) throw new ArgumentNullException(nameof(map));
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        float min = float.MaxValue, max = float.MinValue;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float v = invertXY ? map[y, x] : map[x, y];
                if (v < min) min = v;
                if (v > max) max = v;
            }

        HashSet<Point>? pathSet = path is not null ? [.. path] : null;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int px = invertXY ? x : y;
                int py = invertXY ? y : x;
                float v = map[py, px];
                Point pt = new(x, y);

                if (v >= max)
                {
                    Console.ForegroundColor = ConsoleColor.White; // Стена — белая
                    Console.Write("▓▓");
                }
                else if (pathSet is not null && pathSet.Contains(pt))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta; // Путь — фиолетовый
                    Console.Write("▒▒");
                }
                else
                {
                    Console.ForegroundColor = GetGradientColor(v, min, max);
                    Console.Write("░░");
                }
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    /// <summary>Визуализирует карту типа int[,] в консоли с использованием символов.<br/>░ — пустая клетка, ▓ — стена, путь — фиолетовые ▒.</summary>
    /// <param name="map">Двумерный массив типа int, где максимальное значение считается стеной.</param>
    /// <param name="path">Необязательный путь для отображения типа <see cref="IEnumerable{Point}"/> (может быть <c>null</c>).</param>
    /// <param name="invertXY">Флаг инверсии осей, как в FindPath.</param>
    /// <remarks>Использует <see cref="Console"/> для вывода карты.<br/>Путь отображается цветом Magenta.<br/>Цвет клетки зависит от значения и рассчитывается через <see cref="GetGradientColor"/>.</remarks>
    public static void PrintMap(int[,] map, IEnumerable<Point>? path = null, bool invertXY = true)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (map == null) throw new ArgumentNullException(nameof(map));
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        int min = int.MaxValue, max = int.MinValue;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int v = invertXY ? map[y, x] : map[x, y];
                if (v < min) min = v;
                if (v > max) max = v;
            }

        HashSet<Point>? pathSet = path is not null ? [.. path] : null;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int px = invertXY ? x : y;
                int py = invertXY ? y : x;
                int v = map[py, px];
                Point pt = new(x, y);

                if (v >= max)
                {
                    Console.ForegroundColor = ConsoleColor.White; // Стена — белая
                    Console.Write("▓▓");
                }
                else if (pathSet is not null && pathSet.Contains(pt))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta; // Путь — фиолетовый
                    Console.Write("▒▒");
                }
                else
                {
                    Console.ForegroundColor = GetGradientColor(v, min, max);
                    Console.Write("░░");
                }
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
}
