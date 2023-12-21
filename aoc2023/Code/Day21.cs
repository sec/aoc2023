namespace aoc2023.Code;

internal class Day21 : BaseDay
{
    record XY(int X, int Y);

    IEnumerable<int> Walk(int steps, bool wrap)
    {
        var data = ReadAllLines(true);

        var width = data[0].Length;
        var height = data.Length;
        var map = new char[height, width];

        XY start = new(0, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[y, x] = data[y][x];
                if (map[y, x] == 'S')
                {
                    start = new XY(x, y);
                }
            }
        }

        var s = new HashSet<XY>
        {
            start
        };

        for (int i = 0; i < steps; i++)
        {
            if (wrap && (i - 65) % 131 == 0)
            {
                yield return s.Count;
            }

            s = Expand(map, s, wrap, width, height);
        }

        yield return s.Count;
    }

    static HashSet<XY> Expand(char[,] map, HashSet<XY> start, bool wrap, int width, int height)
    {
        HashSet<XY> visited = [];

        foreach (var current in start)
        {
            foreach (var dir in new XY[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) })
            {
                var nx = current.X + dir.X;
                var ny = current.Y + dir.Y;

                var checkX = nx;
                var checkY = ny;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                {
                    if (!wrap)
                    {
                        continue;
                    }

                    if (checkX < 0)
                    {
                        var m = (-checkX) % width;
                        checkX = m == 0 ? 0 : width - m;
                    }
                    else if (checkX >= width)
                    {
                        checkX %= width;
                    }

                    if (checkY < 0)
                    {
                        var m = (-checkY) % height;
                        checkY = m == 0 ? 0 : height - m;

                    }
                    else if (checkY >= height)
                    {
                        checkY %= height;
                    }
                }

                var next = new XY(nx, ny);

                if (visited.Contains(next))
                {
                    continue;
                }
                if (map[checkY, checkX] == '#')
                {
                    continue;
                }

                visited.Add(next);
            }
        }

        return visited;
    }

    static long Solver(long x, long a, long b, long c) => a * x * x + b * x + c;

    protected override object Part1() => Walk(_testRun ? 6 : 64, false).First();

    protected override object Part2()
    {
        if (_testRun)
        {
            return -1;
        }

        var abc = Walk(65 + 2 * 131, true).ToArray();
        var c = abc[0];
        var x1 = abc[1];
        var x2 = abc[2];

        for (int a = 1; a < 50_000; a++)
        {
            for (int b = 1; b < 50_000; b++)
            {
                var f1 = Solver(1, a, b, c);
                var f2 = Solver(2, a, b, c);

                if (f1 == x1 && f2 == x2)
                {
                    return Solver(26501365 / 131, a, b, c);
                }
            }
        }

        throw new InvalidProgramException();
    }
}