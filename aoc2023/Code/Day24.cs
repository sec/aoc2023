namespace aoc2023.Code;

internal class Day24 : BaseDay
{
    record Stone(long X, long Y, long Z, long A, long B, long C)
    {
        public long X2 => X + A;
        public long Y2 => Y + B;

        public Stone VelocityDelta(long vx, long vy) => this with { X = X, Y = Y, Z = Z, A = A + vx, B = B + vy, C = C };

        public long PredictZ(long time, long vz) => Z + time * (C + vz);
    }

    static bool Collide(Stone a, Stone b, long min, long max, out long cx, out long cy, out long t1)
    {
        t1 = cx = cy = 0;

        if (a.X2 - a.X == 0 || b.X2 - b.X == 0)
        {
            return false;
        }

        var slope1 = (a.Y2 - a.Y) / (decimal) (a.X2 - a.X);
        var slope2 = (b.Y2 - b.Y) / (decimal) (b.X2 - b.X);

        if (slope1 == slope2)
        {
            return false;
        }

        var inter1 = a.Y - slope1 * a.X;
        var inter2 = b.Y - slope2 * b.X;

        var px = (inter2 - inter1) / (slope1 - slope2);
        var py = slope1 * px + inter1;

        if (px < min || px > max || py < min || py > max)
        {
            return false;
        }

        if ((px > a.X && a.A < 0) || (px < a.X && a.A > 0) || (py > a.Y && a.B < 0) || (py < a.Y && a.B > 0))
        {
            return false;
        }

        if ((px > b.X && b.A < 0) || (px < b.X && b.A > 0) || (py > b.Y && b.B < 0) || (py < b.Y && b.B > 0))
        {
            return false;
        }

        cx = (long) px;
        cy = (long) py;
        t1 = (long) (px - a.X) / a.A;

        return true;
    }

    List<Stone> GetStones()
    {
        var data = ReadAllLinesSplit("@", true)
            .Select(x => new
            {
                L = x[0].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList(),
                R = x[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList()
            })
            .Select(x => new Stone(x.L[0], x.L[1], x.L[2], x.R[0], x.R[1], x.R[2]))
            .ToList();

        return data;
    }

    protected override object Part1()
    {
        var min = _testRun ? 7 : 200000000000000;
        var max = _testRun ? 27 : 400000000000000;

        var result = 0;
        var data = GetStones();


        for (int i = 0; i < data.Count; i++)
        {
            for (int j = i + 1; j < data.Count; j++)
            {
                var first = data[i];
                var second = data[j];

                var flag = Collide(first, second, min, max, out _, out _, out _);

                result += flag ? 1 : 0;
            }
        }

        return result;
    }

    protected override object Part2()
    {
        const int RANGE = 500;

        var stones = GetStones();

        while (true)
        {
            var sample = stones.OrderBy(x => Random.Shared.Next()).Take(4).ToList();

            for (int dx = -RANGE; dx < RANGE; dx++)
            {
                for (int dy = -RANGE; dy < RANGE; dy++)
                {
                    var zero = sample.First().VelocityDelta(dx, dy);
                    var tmp = new List<(long X, long Y, long T)>();

                    foreach (var s in sample.Skip(1).Select(s => s.VelocityDelta(dx, dy)))
                    {
                        if (Collide(s, zero, long.MinValue, long.MaxValue, out var cx, out var cy, out var ct))
                        {
                            tmp.Add((cx, cy, ct));
                        }
                    }

                    if (tmp.Count != 3)
                    {
                        continue;
                    }

                    var (X, Y, T) = tmp.First();
                    if (!tmp.All(i => i.X == X && i.Y == Y))
                    {
                        continue;
                    }

                    for (int dz = -RANGE; dz < RANGE; dz++)
                    {
                        var z1 = sample[1].PredictZ(tmp[0].T, dz);
                        var z2 = sample[2].PredictZ(tmp[1].T, dz);
                        var z3 = sample[3].PredictZ(tmp[2].T, dz);

                        if (z1 == z2 && z2 == z3)
                        {
                            return X + Y + z1;
                        }
                    }
                }
            }
        }
    }
}