namespace aoc2023.Code;

internal class Day09 : BaseDay
{
    static List<List<int>> Fill(List<int> start)
    {
        var all = new List<List<int>>
        {
            start
        };

        var current = start;
        while (current.Any(x => x != 0))
        {
            var n = new List<int>();
            for (int i = 0; i < current.Count - 1; i++)
            {
                n.Add(current[i + 1] - current[i]);
            }
            all.Add(n);

            current = n;
        }

        return all;
    }

    static int Process(List<int> start, Func<List<int>, int> selector, Func<int, int, int> extrapolate, Action<int, List<int>> insert)
    {
        var all = Fill(start);

        insert(0, all.Last());

        for (int i = all.Count - 2; i >= 0; i--)
        {
            var cur = selector(all[i]);
            var nxt = selector(all[i + 1]);
            var val = extrapolate(cur, nxt);

            insert(val, all[i]);
        }

        return selector(start);
    }

    private IEnumerable<List<int>> Input() => ReadAllLinesSplit(" ", true).Select(x => x.Select(int.Parse).ToList());

    protected override object Part1() => Input().Sum(x => Process(x, y => y.Last(), (c, n) => c + n, (v, l) => l.Add(v)));

    protected override object Part2() => Input().Sum(x => Process(x, y => y.First(), (c, n) => c - n, (v, l) => l.Insert(0, v)));
}