namespace aoc2023.Code;

internal class Day25 : BaseDay
{
    record struct Node(string Name, List<Node> Nodes);

    class Graph
    {
        readonly Dictionary<string, Node> _nodes = [];
        List<string> _keys = [];

        Node Get(string name)
        {
            if (!_nodes.TryGetValue(name, out var node))
            {
                node = new Node(name, []);
                _nodes[name] = node;
            }
            return node;
        }

        internal void Connect(string l, string r)
        {
            Get(l).Nodes.Add(Get(r));
            Get(r).Nodes.Add(Get(l));
        }

        internal void Cut(string l, string r)
        {
            Get(l).Nodes.Remove(Get(r));
            Get(r).Nodes.Remove(Get(l));
        }

        internal int Count(string start)
        {
            var visited = new HashSet<string>();
            var fringe = new Queue<string>();

            visited.Add(start);
            fringe.Enqueue(start);

            while (fringe.TryDequeue(out var current))
            {
                var node = Get(current);

                var next = node.Nodes.Select(x => x.Name)
                    .Union(_nodes.Where(kv => kv.Value.Nodes.Contains(node)).Select(kv => kv.Key));

                foreach (var n in next)
                {
                    if (visited.Contains(n))
                    {
                        continue;
                    }
                    fringe.Enqueue(n);
                    visited.Add(n);
                }
            }

            return visited.Count;
        }

        internal List<string> Keys() => _nodes.Keys.ToList();

        internal void RandomCut(Random rnd)
        {
            if (_keys.Count == 0)
            {
                _keys = [.. _nodes.Keys];
            }

            while (true)
            {
                var node = _keys[rnd.Next(0, _keys.Count)];
                var nodes = _nodes[node].Nodes;
                if (_nodes.Count == 0)
                {
                    continue;
                }
                Cut(node, nodes[rnd.Next(0, nodes.Count)].Name);

                break;
            }
        }
    }

    Graph Read()
    {
        var g = new Graph();
        var data = ReadAllLinesSplit(":", true)
                .Select(x => new { L = x[0], R = x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries) })
                .ToList();
        foreach (var line in data)
        {
            foreach (var r in line.R)
            {
                g.Connect(line.L, r);
            }
        }
        return g;
    }

    protected override object Part1()
    {
        var data = ReadAllLinesSplit(":", true)
            .Select(x => new { L = x[0], R = x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries) })
            .ToList();

        var graph = new Dictionary<string, string>();
        var sb = new StringBuilder();

        sb.AppendLine("digraph foo {");

        foreach (var line in data)
        {
            foreach (var r in line.R)
            {
                sb.AppendLine($"{line.L} -> {r};");
            }
        }
        sb.AppendLine("}");

        // save sb to d25.txt
        return 0;
    }

    protected override object Part2()
    {
        if (!_testRun)
        {
            // dot.exe -Tpng d25.txt -Grankdir=TB -Kneato -o d25.png
            // use eyes to find 3 nodes :)
            var g = Read();
            g.Cut("htb", "bbg");
            g.Cut("pjj", "dlk");
            g.Cut("pcc", "htj");

            return g.Count("htb") * g.Count("bbg");
        }
        else
        {
            var g = Read();
            var keys = g.Keys();
            var result = 0;

            Parallel.ForEach(Ext.GetCombinations(keys, 6), (edge, pls) =>
            {
                foreach (var ee in Ext.GetPermutations(edge, 6))
                {
                    var e = ee.ToArray();
                    var g = Read();

                    g.Cut(e[0], e[1]);
                    g.Cut(e[2], e[3]);
                    g.Cut(e[4], e[5]);

                    var cnt = new int[3];

                    cnt[0] = g.Count(e[0]);
                    if (cnt[0] != keys.Count)
                    {
                        cnt[1] = g.Count(e[1]);
                        cnt[2] = g.Count(e[2]);

                        Interlocked.CompareExchange(ref result, cnt.Distinct().Aggregate(1, (a, b) => a * b), 0);

                        pls.Stop();
                        break;
                    }
                }
            });

            return result;
        }
    }
}