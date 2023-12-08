namespace aoc2023.Code;

internal class Day08 : BaseDay
{
    class Map
    {
        class Node(string name)
        {
            public string Name = name;

            public Node? Left;
            public Node? Right;

            public bool Start { get; } = name[2] == 'A';
            public bool End { get; } = name[2] == 'Z';
        }

        private readonly List<Node> _nodes = [];
        private readonly string _inst;

        public Map(string[] input)
        {
            _inst = input[0];
            var map = input.Skip(1).Select(x => x.Split("=(),".ToCharArray(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

            foreach (var dir in map)
            {
                var name = dir[0];
                var l = dir[1];
                var r = dir[2];

                var root = TryAdd(name);
                var left = TryAdd(l);
                var right = TryAdd(r);

                root.Left = left;
                root.Right = right;
            }
        }

        Node TryAdd(string name)
        {
            var node = _nodes.SingleOrDefault(x => x.Name == name);
            if (node is null)
            {
                node = new(name);
                _nodes.Add(node);
            }
            return node;
        }

        internal int Navigate()
        {
            var i = 0;
            var root = _nodes.Single(x => x.Name == "AAA");

            while (true)
            {
                if (_inst[i++ % _inst.Length] == 'L')
                {
                    root = root.Left!;
                }
                else
                {
                    root = root.Right!;
                }
                if (root.Name == "ZZZ")
                {
                    return i;
                }
            }
        }

        internal ulong NavigateGhost()
        {
            ulong count = 0;
            var i = 0;

            var roots = _nodes.Where(x => x.Start).ToList();
            var check = new Dictionary<string, ulong>();

            while (true)
            {
                count++;

                if (i >= _inst.Length)
                {
                    i = 0;
                }

                if (_inst[i++] == 'L')
                {
                    roots = roots.Select(x => x.Left!).ToList();
                }
                else
                {
                    roots = roots.Select(x => x.Right!).ToList();
                }

                foreach (var ghost in roots.Where(x => x.End))
                {
                    if (!check.TryGetValue(ghost.Name, out var _))
                    {
                        check[ghost.Name] = count;
                    }
                }

                if (check.Keys.Count == roots.Count)
                {
                    return check.Values.SelectMany(Factor).Distinct().Aggregate(1UL, (a, b) => a * b);
                }
            }
        }
    }

    static IEnumerable<ulong> Factor(ulong number)
    {
        for (var d = 2UL; d * d <= number; d++)
        {
            while (number % d == 0)
            {
                yield return d;
                number /= d;
            }
        }
        if (number > 1)
        {
            yield return number;
        }
    }

    protected override object Part1() => new Map(ReadAllLines(true)).Navigate();

    protected override object Part2() => new Map(ReadAllLines(true)).NavigateGhost();
}