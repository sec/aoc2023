namespace aoc2023.Code;

internal class Day16 : BaseDay
{
    enum Direction { Left, Right, Up, Down };

    record Pos(int X, int Y);

    class Contraption
    {
        public readonly int Width;
        public readonly int Height;

        readonly char[,] _map;

        static readonly Dictionary<Direction, Pos> _move = new()
        {
            { Direction.Left, new(-1, 0) },
            { Direction.Right, new(1, 0) },
            { Direction.Up, new(0, -1) },
            { Direction.Down, new(0, 1) }
        };

        public Contraption(string[] data)
        {
            Width = data[0].Length;
            Height = data.Length;

            _map = new char[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _map[i, j] = data[i][j];
                }
            }
        }

        internal int Beam(Pos start, Direction dir)
        {
            var fringe = new Queue<(Pos, Direction)>();
            var energized = new HashSet<Pos>();
            var visited = new HashSet<(Pos, Direction)>();

            fringe.Enqueue((start, dir));

            while (fringe.Count > 0)
            {
                var (p, where) = fringe.Dequeue();

                energized.Add(p);
                visited.Add((p, where));

                var dirs = new List<Direction>();

                switch (_map[p.Y, p.X])
                {
                    case '.':
                        dirs.Add(where);
                        break;

                    case '/':
                        where = where switch
                        {
                            Direction.Left => Direction.Down,
                            Direction.Right => Direction.Up,
                            Direction.Up => Direction.Right,
                            Direction.Down => Direction.Left,
                            _ => throw new NotImplementedException()
                        };
                        dirs.Add(where);
                        break;

                    case '\\':
                        where = where switch
                        {
                            Direction.Left => Direction.Up,
                            Direction.Right => Direction.Down,
                            Direction.Up => Direction.Left,
                            Direction.Down => Direction.Right,
                            _ => throw new NotImplementedException()
                        };
                        dirs.Add(where);
                        break;

                    case '-':
                        if (where is Direction.Left or Direction.Right)
                        {
                            dirs.Add(where);
                        }
                        else
                        {
                            dirs.Add(Direction.Left);
                            dirs.Add(Direction.Right);
                        }
                        break;

                    case '|':
                        if (where is Direction.Up or Direction.Down)
                        {
                            dirs.Add(where);
                        }
                        else
                        {
                            dirs.Add(Direction.Up);
                            dirs.Add(Direction.Down);
                        }
                        break;
                }

                foreach (var d in dirs)
                {
                    var v = _move[d];
                    var next = new Pos(p.X + v.X, p.Y + v.Y);
                    var nxt = (next, d);

                    if (visited.Contains(nxt))
                    {
                        continue;
                    }

                    if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
                    {
                        continue;
                    }

                    fringe.Enqueue(nxt);
                }
            }

            return energized.Count;
        }
    }

    protected override object Part1() => new Contraption(ReadAllLines(true)).Beam(new(0, 0), Direction.Right);

    protected override object Part2()
    {
        var r = new List<int>();
        var c = new Contraption(ReadAllLines(true));
        var w = c.Width;
        var h = c.Height;

        for (int i = 0; i < h; i++)
        {
            r.Add(c.Beam(new(0, i), Direction.Right));
            r.Add(c.Beam(new(w - 1, i), Direction.Left));
        }
        for (int i = 0; i < w; i++)
        {
            r.Add(c.Beam(new(i, 0), Direction.Down));
            r.Add(c.Beam(new(i, h - 1), Direction.Up));
        }

        return r.Max();
    }
}