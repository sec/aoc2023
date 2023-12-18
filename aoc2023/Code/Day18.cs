namespace aoc2023.Code;

internal class Day18 : BaseDay
{
    record DigPlan(char D, int S, string Color);

    record XY(long X, long Y);

    class Lagoon
    {
        readonly List<DigPlan> _plan;
        readonly List<XY> _edges;

        public Lagoon(IEnumerable<string[]> digPlan)
        {
            _plan = [];
            _edges = [];

            foreach (var plan in digPlan)
            {
                _plan.Add(new DigPlan(plan[0][0], int.Parse(plan[1]), plan[2]));
            }
        }

        internal Lagoon Trench(bool fix)
        {
            var edge = new XY(0, 0);
            _edges.Add(edge);

            foreach (var plan in _plan)
            {
                var length = plan.S;
                var direction = plan.D;

                if (fix)
                {
                    length = int.Parse(plan.Color[2..7], System.Globalization.NumberStyles.HexNumber);
                    direction = "RDLU"[plan.Color[^2] - '0'];
                }

                edge = direction switch
                {
                    'R' => new(edge.X + length, edge.Y),
                    'L' => new(edge.X - length, edge.Y),
                    'U' => new(edge.X, edge.Y - length),
                    'D' => new(edge.X, edge.Y + length),
                    _ => throw new NotImplementedException()
                };

                _edges.Add(edge);
            }

            return this;
        }

        internal long Shoelace()
        {
            var s = 0L;
            var p = 0L;

            for (int i = 0; i < _edges.Count - 1; i++)
            {
                var a = _edges[i];
                var b = _edges[i + 1];

                s += (a.X * b.Y - b.X * a.Y);
                p += Math.Abs(a.X - b.X + a.Y - b.Y);
            }

            return (s + p) / 2 + 1;
        }
    }

    protected override object Part1() => new Lagoon(ReadAllLinesSplit(" ", true)).Trench(false).Shoelace();

    protected override object Part2() => new Lagoon(ReadAllLinesSplit(" ", true)).Trench(true).Shoelace();
}