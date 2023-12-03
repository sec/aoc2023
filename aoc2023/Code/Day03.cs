namespace aoc2023.Code;

internal class Day03 : BaseDay
{
    readonly HashSet<(int, int)> _visited = [];

    class Part
    {
        public char Item { get; set; }

        public bool IsSymbol => Item != '.' && !IsDigit;
        public bool IsDigit => char.IsDigit(Item);
        public bool IsGear => Item == '*';
    }

    protected override object Part1()
    {
        var (width, height, map) = ReadSchematic();

        var sum = 0;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (map[i, j].IsSymbol)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            if (!(x == 0 && y == 0))
                            {
                                sum += GetNumber(x, y, i, j, map, width, height);
                            }
                        }
                    }
                }
            }
        }

        return sum;
    }

    protected override object Part2()
    {
        _visited.Clear();

        var (width, height, map) = ReadSchematic();
        var sum = 0;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (map[i, j].IsGear)
                {
                    var parts = new List<int>();

                    for (int y = -1; y < 2; y++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            if (!(x == 0 && y == 0))
                            {
                                var val = GetNumber(x, y, i, j, map, width, height);
                                if (val != 0)
                                {
                                    parts.Add(val);
                                }
                            }
                        }
                    }
                    if (parts.Count == 2)
                    {
                        sum += parts[0] * parts[1];
                    }
                }
            }
        }

        return sum;
    }

    private (int, int, Part[,]) ReadSchematic()
    {
        var input = ReadAllLines(true);

        var width = input[0].Length;
        var height = input.Length;
        var map = new Part[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var item = new Part()
                {
                    Item = input[i][j]
                };
                map[i, j] = item;
            }
        }

        return (width, height, map);
    }

    private int GetNumber(int moveX, int moveY, int row, int col, Part[,] map, int width, int height)
    {
        var nx = moveX + col;
        var ny = moveY + row;

        if (nx < 0 || nx >= width || ny < 0 || ny >= height || !map[ny, nx].IsDigit)
        {
            return 0;
        }

        while (nx >= 0 && map[ny, nx].IsDigit)
        {
            nx--;
        }
        nx++;

        if (_visited.Contains((ny, nx)))
        {
            return 0;
        }
        _visited.Add((ny, nx));

        var sb = new StringBuilder();
        while (nx < width && map[ny, nx].IsDigit)
        {
            sb.Append(map[ny, nx].Item);
            nx++;
        }

        return int.Parse(sb.ToString());
    }
}