namespace aoc2023.Code;

internal class Day05 : BaseDay
{
    record Map(long Dest, long Src, long Length);

    private static readonly StringSplitOptions _so = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    class Almanac
    {
        readonly string[] order = ["seed", "soil", "fertilizer", "water", "light", "temperature", "humidity"];
        readonly Dictionary<string, List<Map>> _map = [];

        public Almanac(string[] data)
        {
            var map = string.Empty;

            foreach (var line in data.Skip(1))
            {
                if (line.IndexOf(':') > -1)
                {
                    map = AddMap(line);
                }
                else
                {
                    AddRange(map, line);
                }
            }
        }

        string AddMap(string map)
        {
            var tmp = map.Split(' ', _so)[0].Split('-', _so);
            _map[tmp[0]] = [];

            return tmp[0];
        }

        void AddRange(string map, string ranges)
        {
            var tmp = ranges.Split(' ', _so).Select(long.Parse).ToArray();
            var dst = tmp[0];
            var src = tmp[1];
            var len = tmp[2];

            _map[map].Add(new(dst, src, len));
        }

        public long Process(long seed)
        {
            foreach (var o in order)
            {
                var map = _map[o];

                foreach (var kv in map)
                {
                    if (seed >= kv.Src && seed < kv.Src + kv.Length)
                    {
                        seed = kv.Dest + (seed - kv.Src);
                        break;
                    }
                }
            }

            return seed;
        }
    }

    protected override object Part1()
    {
        var data = ReadAllLines(true);
        var almanac = new Almanac(data);

        var seeds = data[0].Split(':', _so)[1].Split(' ', _so).Select(long.Parse);

        return seeds.Min(almanac.Process);
    }

    protected override object Part2()
    {
        var data = ReadAllLines(true);
        var almanac = new Almanac(data);

        var pairs = data[0].Split(':', _so)[1].Split(' ', _so).Select(long.Parse);

        var tasks = new List<Task<long>>();
        foreach (var pair in pairs.Chunk(2))
        {
            var start = pair[0];
            var length = pair[1];

            var t = Task.Run(() => Worker(almanac, start, length));
            tasks.Add(t);
        }
        Task.WaitAll([.. tasks]);

        return tasks.Min(x => x.Result);
    }

    static long Worker(Almanac almanac, long start, long length)
    {
        var min = long.MaxValue;
        for (var i = 0L; i < length; i++)
        {
            var m = almanac.Process(start + i);

            min = long.Min(m, min);
        }
        return min;
    }
}