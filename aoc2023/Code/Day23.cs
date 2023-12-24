namespace aoc2023.Code;

internal class Day23 : BaseDay
{
    record struct XY(int X, int Y);

    record struct State(XY Current, int Steps, HashSet<XY> Visited);

    class Node(XY Pos, HashSet<(Node Node, int Weight)> Connected)
    {
        public XY Pos = Pos;

        public HashSet<(Node Node, int Weight)> Connected = Connected;
    }

    static readonly Dictionary<char, XY[]> _steps = new()
    {
      { '.', [new(-1, 0), new(1, 0), new(0, -1), new(0, 1)] },
      { '>', [new(1, 0)] },
      { 'v', [new(0, 1)] },
    };

    int Walk()
    {
        var data = ReadAllLines(true);
        var width = data[0].Length;
        var height = data.Length;

        var map = new char[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[y, x] = data[y][x];
            }
        }

        var start = new XY(1, 0);
        var end = new XY(width - 2, height - 1);

        var fringe = new Queue<State>();
        fringe.Enqueue(new(start, 0, [start]));

        var max = 0;

        while (fringe.TryDequeue(out var state))
        {
            if (state.Current == end)
            {
                max = Math.Max(max, state.Steps);
                continue;
            }

            foreach (var step in _steps[map[state.Current.Y, state.Current.X]])
            {
                var np = new XY(state.Current.X + step.X, state.Current.Y + step.Y);
                if (np.X < 0 || np.X >= width || np.Y < 0 || np.Y >= height)
                {
                    continue;
                }
                if (map[np.Y, np.X] == '#')
                {
                    continue;
                }
                if (state.Visited.Contains(np))
                {
                    continue;
                }

                var ns = new State(np, state.Steps + 1, [.. state.Visited, np]);
                fringe.Enqueue(ns);
            }
        }

        return max;
    }

    static int WalkDeep(Dictionary<XY, Node> nodes, XY start, XY end, HashSet<XY> visited, int currentLength)
    {
        if (start == end)
        {
            return currentLength;
        }

        var root = nodes[start];
        var max = 0;

        visited.Add(start);
        foreach (var node in root.Connected)
        {
            if (!visited.Contains(node.Node.Pos))
            {
                max = Math.Max(max, WalkDeep(nodes, node.Node.Pos, end, visited, currentLength + node.Weight));
            }
        }
        visited.Remove(start);

        return max;
    }

    protected override object Part1() => Walk();

    protected override object Part2()
    {
        var data = ReadAllLines(true);
        var width = data[0].Length;
        var height = data.Length;

        var start = new XY(1, 0);
        var end = new XY(width - 2, height - 1);

        var map = new char[height, width];
        var nodes = new Dictionary<XY, Node>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[y, x] = data[y][x];

                if (map[y, x] != '#')
                {
                    var node = new Node(new(x, y), []);
                    nodes.Add(node.Pos, node);
                }
            }
        }

        foreach (var node in nodes.Values)
        {
            foreach (var next in _steps['.'])
            {
                var n = nodes.Values.SingleOrDefault(x => x.Pos == new XY(node.Pos.X + next.X, node.Pos.Y + next.Y));
                if (n is not null)
                {
                    node.Connected.Add((n, 1));
                    n.Connected.Add((node, 1));
                }
            }
        }

        foreach (var node in nodes.Values.Where(x => x.Connected.Count == 2))
        {
            nodes.Remove(node.Pos);

            var left = node.Connected.First();
            var right = node.Connected.Last();
            var sum = left.Weight + right.Weight;

            left.Node.Connected.RemoveWhere(x => x.Node.Pos == node.Pos);
            left.Node.Connected.Add((right.Node, sum));

            right.Node.Connected.RemoveWhere(x => x.Node.Pos == node.Pos);
            right.Node.Connected.Add((left.Node, sum));
        }

        return WalkDeep(nodes, start, end, [], 0);
    }
}