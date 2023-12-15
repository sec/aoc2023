namespace aoc2023.Code;

internal class Day14 : BaseDay
{
    /*
    class Rock(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    class Platform
    {
        readonly int _height;
        readonly int _width;
        readonly List<Rock> _rocks;
        readonly HashSet<(int, int)> _walls;

        public Platform(string[] strings)
        {
            _height = strings.Length;
            _width = strings[0].Length;

            _rocks = [];
            _walls = [];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    switch (strings[i][j])
                    {
                        case 'O':
                            _rocks.Add(new Rock(j, _height - i));
                            break;
                        case '#':
                            _walls.Add((j, _height - i));
                            break;
                    }
                }
            }
        }

        internal int TotalLoad() => _rocks.Sum(x => x.Y);

        internal bool Blocked(int x, int y) => _walls.Contains((x, y)) || _rocks.Any(r => r.X == x && r.Y == y);

        internal Platform North()
        {
            foreach (var r in _rocks.OrderByDescending(x => x.Y))
            {
                while (r.Y < _height && !Blocked(r.X, r.Y + 1))
                {
                    r.Y++;
                }
            }
            return this;
        }

        internal Platform South()
        {
            foreach (var r in _rocks.OrderBy(x => x.Y))
            {
                while (r.Y > 1 && !Blocked(r.X, r.Y - 1))
                {
                    r.Y--;
                }
            }
            return this;
        }

        internal Platform West()
        {
            foreach (var r in _rocks.OrderBy(x => x.X))
            {
                while (r.X > 0 && !Blocked(r.X - 1, r.Y))
                {
                    r.X--;
                }
            }
            return this;
        }

        internal Platform East()
        {
            foreach (var r in _rocks.OrderByDescending(x => x.X))
            {
                while (r.X < _width - 1 && !Blocked(r.X + 1, r.Y))
                {
                    r.X++;
                }
            }
            return this;
        }

        internal Platform Cycle() => North().West().South().East();

        internal List<(int, int)> State() => _rocks.Select(r => (r.X, r.Y)).ToList();
    }
    */

    class PlatformMap
    {
        readonly int _height;
        readonly int _width;
        readonly char[,] _map;

        public PlatformMap(string[] strings)
        {
            _height = strings.Length;
            _width = strings[0].Length;
            _map = new char[_height, _width];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _map[i, j] = strings[i][j];
                }
            }
        }

        internal int TotalLoad()
        {
            var s = 0;

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    s += _map[i, j] == 'O' ? _height - i : 0;
                }
            }

            return s;
        }

        internal bool Blocked(int x, int y) => _map[y, x] != '.';

        internal PlatformMap North()
        {
            for (int i = 1; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_map[i, j] == 'O')
                    {
                        var y = i;
                        while (y - 1 >= 0 && !Blocked(j, y - 1))
                        {
                            _map[y - 1, j] = 'O';
                            _map[y, j] = '.';
                            y--;
                        }
                    }
                }
            }

            return this;
        }

        internal PlatformMap South()
        {
            for (int i = _height - 2; i >= 0; i--)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_map[i, j] == 'O')
                    {
                        var y = i;
                        while (y + 1 < _height && !Blocked(j, y + 1))
                        {
                            _map[y + 1, j] = 'O';
                            _map[y, j] = '.';
                            y++;
                        }
                    }
                }
            }

            return this;
        }

        internal PlatformMap West()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 1; j < _width; j++)
                {
                    if (_map[i, j] == 'O')
                    {
                        var x = j;
                        while (x - 1 >= 0 && !Blocked(x - 1, i))
                        {
                            _map[i, x - 1] = 'O';
                            _map[i, x] = '.';
                            x--;
                        }
                    }
                }
            }

            return this;
        }

        internal PlatformMap East()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = _width - 2; j >= 0; j--)
                {
                    if (_map[i, j] == 'O')
                    {
                        var x = j;
                        while (x + 1 < _width && !Blocked(x + 1, i))
                        {
                            _map[i, x + 1] = 'O';
                            _map[i, x] = '.';
                            x++;
                        }
                    }
                }
            }

            return this;
        }

        internal PlatformMap Cycle() => North().West().South().East();

        internal IEnumerable<(int, int)> State()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_map[i, j] == 'O')
                    {
                        yield return (i, j);
                    }
                }
            }
        }
    }

    protected override object Part1() => new PlatformMap(ReadAllLines(true)).North().TotalLoad();

    protected override object Part2()
    {
        var p = new PlatformMap(ReadAllLines(true));

        var limit = 1_000_000_000;
        var jump = -1;
        var states = new List<List<(int, int)>>();

        for (int i = 0; i < limit; i++, p.Cycle())
        {
            if (jump > 0)
            {
                continue;
            }

            var state = p.State().ToList();

            for (int s = 0; s < states.Count; s++)
            {
                if (!states[s].All(state.Contains))
                {
                    continue;
                }

                jump = i - s;
                while (i + jump < limit)
                {
                    i += jump;
                }
                break;
            }

            if (jump < 0)
            {
                states.Add(state);
            }
        }

        return p.TotalLoad();
    }
}