namespace aoc2023.Code;

internal class Day22 : BaseDay
{
    class AABB(int x, int y, int z, int a, int b, int c)
    {
        public int X = x, Y = y, Z = z, A = a, B = b, C = c;

        internal bool Intersect(AABB other) => X <= other.A && A >= other.X && Y <= other.B && B >= other.Y && Z <= other.C && C >= other.Z;

        internal AABB Clone() => new(X, Y, Z, A, B, C);
    }

    List<AABB> Grid() => ReadAllLinesSplit("~", true)
        .Select(x => new { L = x[0].Split(',').Select(int.Parse).ToArray(), R = x[1].Split(',').Select(int.Parse).ToArray() })
        .Select(x => new AABB(x.L[0], x.L[1], x.L[2], x.R[0], x.R[1], x.R[2]))
        .ToList();

    protected override object Part1()
    {
        List<AABB> grid = Grid();
        Settle(grid, false);

        return grid.Select(x => grid.Where(y => y != x).Select(x => x.Clone()).ToList())
            .Where(x => Settle(x, true) == 0)
            .AsParallel()
            .Count();
    }

    protected override object Part2()
    {
        List<AABB> grid = Grid();
        Settle(grid, false);

        return grid.Select(x => grid.Where(y => y != x).Select(x => x.Clone()).ToList())
            .AsParallel()
            .Sum(x => Settle(x, false));
    }

    static int Settle(List<AABB> grid, bool exitOnMove)
    {
        var bag = new HashSet<AABB>();

        while (true)
        {
            var moved = false;
            foreach (var cube in grid.Where(x => x.Z > 1))
            {
                cube.Z--;
                cube.C--;

                if (grid.Any(x => x != cube && x.Intersect(cube)))
                {
                    cube.Z++;
                    cube.C++;
                    continue;
                }

                bag.Add(cube);
                moved = true;

                if (exitOnMove)
                {
                    return bag.Count;
                }
            }

            if (!moved)
            {
                return bag.Count;
            }
        }
    }
}