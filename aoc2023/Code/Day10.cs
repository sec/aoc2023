namespace aoc2023.Code;

internal class Day10 : BaseDay
{
    record Pos(int X, int Y);

    enum Pipe { NS, EW, NE, NW, SW, SE, Ground, Start };

    class Loop
    {
        readonly private int _width, _height;
        readonly private Pipe[,] _map;
        readonly private Pos _start;

        public Loop(string[] source, char startPipe)
        {
            _width = source[0].Length;
            _height = source.Length;
            _map = new Pipe[_height, _width];
            _start = new(0, 0);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _map[y, x] = ToPipe(source[y][x]);

                    if (_map[y, x] == Pipe.Start)
                    {
                        _map[y, x] = ToPipe(startPipe);
                        _start = new(x, y);
                    }
                }
            }
        }

        internal int Solve()
        {
            var visited = new HashSet<Pos>();
            var queue = new Queue<(Pos, int)>();
            var max = 0;

            visited.Add(_start);
            queue.Enqueue((_start, 0));

            while (queue.Count > 0)
            {
                var (check, dist) = queue.Dequeue();
                if (dist > max)
                {
                    max = dist;
                }

                var next = WhereNext(check);
                foreach (var n in next)
                {
                    if (visited.Contains(n))
                    {
                        continue;
                    }
                    visited.Add(n);
                    queue.Enqueue((n, dist + 1));
                }
            }

            return max;
        }

        internal int Solve2()
        {
            var visited = new HashSet<Pos>();
            var fringe = new Stack<Pos>();
            var polygon = new List<Pos>();

            visited.Add(_start);
            fringe.Push((_start));

            while (fringe.TryPop(out var check))
            {
                if (_map[check.Y, check.X] is not Pipe.EW and not Pipe.NS)
                {
                    polygon.Add(check);
                }

                foreach (var n in WhereNext(check))
                {
                    if (visited.Contains(n))
                    {
                        continue;
                    }
                    visited.Add(n);
                    fringe.Push(n);
                }
            }

            var cnt = 0;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (visited.Contains(new(x, y)))
                    {
                        continue;
                    }

                    if (IsInside(x, y, polygon))
                    {
                        cnt++;
                    }
                }
            }
            return cnt;
        }

        static bool IsInside(int x, int y, List<Pos> _edges)
        {
            var inside = false;
            for (int i = 0, j = _edges.Count - 1; i < _edges.Count; j = i++)
            {
                var xi = _edges[i].X;
                var yi = _edges[i].Y;
                var xj = _edges[j].X;
                var yj = _edges[j].Y;

                var intersect = ((yi > y) != (yj > y)) && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
                if (intersect)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private IEnumerable<Pos> WhereNext(Pos c)
        {
            switch (_map[c.Y, c.X])
            {
                case Pipe.NS:
                    yield return new(c.X, c.Y - 1);
                    yield return new(c.X, c.Y + 1);
                    break;
                case Pipe.EW:
                    yield return new(c.X - 1, c.Y);
                    yield return new(c.X + 1, c.Y);
                    break;
                case Pipe.NE:
                    yield return new(c.X, c.Y - 1);
                    yield return new(c.X + 1, c.Y);
                    break;
                case Pipe.NW:
                    yield return new(c.X, c.Y - 1);
                    yield return new(c.X - 1, c.Y);
                    break;
                case Pipe.SW:
                    yield return new(c.X - 1, c.Y);
                    yield return new(c.X, c.Y + 1);
                    break;
                case Pipe.SE:
                    yield return new(c.X, c.Y + 1);
                    yield return new(c.X + 1, c.Y);
                    break;
            }
        }

        static Pipe ToPipe(char v)
        {
            return v switch
            {
                '|' => Pipe.NS,
                '-' => Pipe.EW,
                'L' => Pipe.NE,
                'J' => Pipe.NW,
                '7' => Pipe.SW,
                'F' => Pipe.SE,
                '.' => Pipe.Ground,
                'S' => Pipe.Start,
                _ => throw new NotImplementedException()
            };
        }
    }

    protected override object Part1() => new Loop(ReadAllLines(true), _testRun ? '7' : 'L').Solve();

    protected override object Part2() => new Loop(ReadAllLines(true), _testRun ? '7' : 'L').Solve2();
}