using System.Collections.Immutable;

namespace aoc2023.Code;

internal class Day12 : BaseDay
{
    class Row
    {
        private readonly static Dictionary<(string, ImmutableStack<int>), long> _cache = [];

        private readonly char[] _row;
        private readonly List<int> _check;
        private readonly List<int> _index;

        public Row(string springs, string groups)
        {
            _row = springs.ToCharArray();
            _check = groups.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            _index = [];
            for (int i = 0; i < _row.Length; i++)
            {
                if (_row[i] == '?')
                {
                    _index.Add(i);
                }
            }
        }

        internal long Count()
        {
            var cnt = 0L;
            var total = (ulong) Math.Pow(2, _index.Count);
            for (ulong i = 0; i < total; i++)
            {
                for (int j = 0; j < _index.Count; j++)
                {
                    var bit = (i >> j) & 1;
                    var index = _index[j];

                    _row[index] = bit == 0 ? '.' : '#';
                }

                if (IsValid())
                {
                    cnt++;
                }
            }

            return cnt;
        }

        internal bool IsValid()
        {
            var tmp = new List<int>();
            var cnt = 0;

            for (int i = 0; i < _row.Length; i++)
            {
                switch (_row[i])
                {
                    case '#':
                        cnt++;
                        break;
                    case '.':
                        if (cnt > 0)
                        {
                            tmp.Add(cnt);
                            cnt = 0;
                        }
                        break;
                }
            }

            if (cnt > 0)
            {
                tmp.Add(cnt);
            }

            return Enumerable.SequenceEqual(tmp, _check);
        }

        internal long Count2() => Process(string.Join(string.Empty, _row), ImmutableStack.CreateRange(_check.Reverse<int>()));

        static long Process(string row, ImmutableStack<int> groups)
        {
            var key = (row, groups);
            if (_cache.TryGetValue(key, out var v))
            {
                return v;
            }

            if (string.IsNullOrEmpty(row))
            {
                return groups.IsEmpty ? 1 : 0;
            }

            return _cache[key] = row[0] switch
            {
                '#' => ProcessHash(row, groups),
                '.' => ProcessDot(row, groups),
                '?' => ProcessMark(row, groups),
                _ => throw new NotImplementedException()
            };
        }

        static long ProcessMark(string row, ImmutableStack<int> groups) => Process('.' + row[1..], groups) + Process('#' + row[1..], groups);

        static long ProcessDot(string row, ImmutableStack<int> groups) => Process(row[1..], groups);

        static long ProcessHash(string row, ImmutableStack<int> groups)
        {
            if (groups.IsEmpty)
            {
                return 0;
            }

            groups = groups.Pop(out var n);

            int broken = row.TakeWhile(x => x == '#' || x == '?').Count();
            if (broken < n)
            {
                return 0;
            }

            if (row.Length == n)
            {
                return Process(string.Empty, groups);
            }
            if (row[n] == '#')
            {
                return 0;
            }

            return Process(row[(n + 1)..], groups);
        }
    }

    protected override object Part1() => ReadAllLinesSplit(" ", true)
        .Select(x => new { S = x[0], G = x[1] })
        .Select(x => new Row(x.S, x.G))
        .Sum(x => x.Count());

    protected override object Part2() => ReadAllLinesSplit(" ", true)
        .Select(x => new { S = string.Join("?", Enumerable.Repeat(x[0], 5)), G = string.Join(",", Enumerable.Repeat(x[1], 5)) })
        .Select(x => new Row(x.S, x.G))
        .Sum(x => x.Count2());
}