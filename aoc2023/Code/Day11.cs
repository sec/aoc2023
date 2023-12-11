namespace aoc2023.Code;

internal class Day11 : BaseDay
{
    record Galaxy(int X, int Y)
    {
        public int ManhattanDistance(Galaxy other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    record Info(int A, int B, int D);

    class Space
    {
        readonly List<Galaxy> _galaxies;
        readonly List<string> _image;

        public Space(string[] scan, int age)
        {
            _galaxies = [];
            _image = [.. scan];

            List<int> expandRow = [];
            List<int> expandCol = [];

            // find row to expand
            for (int i = 0; i < _image.Count; i++)
            {
                if (_image[i].All(x => x == '.'))
                {
                    expandRow.Add(i);
                }
            }

            // find column to expand
            for (int i = 0; i < _image[0].Length; i++)
            {
                var empty = true;
                for (int j = 0; j < _image.Count; j++)
                {
                    if (_image[j][i] != '.')
                    {
                        empty = false;
                        break;
                    }
                }
                if (empty)
                {
                    expandCol.Add(i);
                }
            }

            // expand row
            foreach (var row in expandRow.Reverse<int>())
            {
                for (int j = 0; j < age; j++)
                {
                    _image.Insert(row, _image[row]);
                }
            }

            // expand col
            foreach (var col in expandCol.Reverse<int>())
            {
                for (int i = 0; i < _image.Count; i++)
                {
                    var tmp = _image[i].ToList();
                    for (int j = 0; j < age; j++)
                    {
                        tmp.Insert(col, '.');
                    }
                    _image[i] = string.Join(string.Empty, tmp);
                }
            }

            // find galaxies
            for (int y = 0; y < _image.Count; y++)
            {
                for (int x = 0; x < _image[y].Length; x++)
                {
                    if (_image[y][x] == '#')
                    {
                        _galaxies.Add(new(x, y));
                    }
                }
            }
        }

        internal IEnumerable<Info> Solve()
        {
            foreach (var pair in Ext.GetCombinations(Enumerable.Range(0, _galaxies.Count), 2))
            {
                var a = pair.First();
                var b = pair.Last();
                var d = _galaxies[a].ManhattanDistance(_galaxies[b]);

                yield return new(a, b, d);
            }
        }
    }

    protected override object Part1() => new Space(ReadAllLines(true), 1).Solve().Sum(x => x.D);

    protected override object Part2()
    {
        var age = 1_000_000;

        var one = new Space(ReadAllLines(true), 1).Solve().ToList();
        var two = new Space(ReadAllLines(true), 2).Solve().ToList();

        var sum = 0L;

        for (int i = 0; i < one.Count; i++)
        {
            var a = one[i];
            var b = two[i];
            var d = b.D - a.D;

            sum += a.D + ((age - 2) * d);
        }

        return sum;
    }
}