namespace aoc2023.Code;

internal class Day19 : BaseDay
{
    record Range(int Start, int End, string Target, char Rating, int RevStart, int RevEnd, bool Expanded = false);

    class Part(string data)
    {
        public int Rating => Ratings.Values.Sum();

        public Dictionary<char, int> Ratings { get; } = data[1..^1]
            .Split(',')
            .Select(x => x.Split('='))
            .ToDictionary(x => x[0][0], x => int.Parse(x[1]));
    }

    class Rule
    {
        readonly string _target;
        readonly int _value;
        readonly char _rating;
        readonly Func<Part, string>? _check;
        readonly Range _range;

        public Rule(string rule)
        {
            var data = rule.Split(':');
            if (data.Length == 2)
            {
                var cond = data[0].Split('<', '>');

                _rating = cond[0][0];
                _target = data[1];
                _value = int.Parse(cond[1]);

                if (data[0].IndexOf('>') > -1)
                {
                    _check = p => p.Ratings[_rating] > _value ? _target : string.Empty;
                    _range = new Range(_value + 1, 4000, _target, _rating, 1, _value);
                }
                else
                {
                    _check = p => p.Ratings[_rating] < _value ? _target : string.Empty;
                    _range = new Range(1, _value - 1, _target, _rating, _value, 4000);
                }
            }
            else
            {
                _target = rule;
                _range = new Range(1, 1, _target, ' ', 1, 1);
            }
        }

        internal string? Process(Part part) => _check is not null ? _check(part) : _target;

        internal Range Range => _range;
    }

    class Workflow(string[] rules)
    {
        readonly IEnumerable<Rule> _rules = rules.Select(x => new Rule(x));

        public string Process(Part part)
        {
            foreach (var rule in _rules)
            {
                var r = rule.Process(part);
                if (!string.IsNullOrEmpty(r))
                {
                    return r;
                }
            }
            throw new InvalidProgramException();
        }

        public IEnumerable<Range> Ranges() => _rules.Select(x => x.Range);
    }

    class System
    {
        public List<Part> Accepted { get; }
        public List<Part> Parts { get; }
        public Dictionary<string, Workflow> Workflows { get; }

        public System(string[] data)
        {
            Accepted = [];
            Parts = [];
            Workflows = [];

            foreach (var d in data)
            {
                if (d.StartsWith('{'))
                {
                    AddPart(d);
                }
                else
                {
                    AddWorkflow(d.Split('{'));
                }
            }
        }

        void AddWorkflow(string[] d) => Workflows[d[0]] = new Workflow(d[1][..^1].Split(','));

        void AddPart(string d) => Parts.Add(new Part(d));

        string Process(Part part)
        {
            var wf = "in";
            while (true)
            {
                wf = Workflows[wf].Process(part);
                if (wf is "A" or "R")
                {
                    return wf;
                }
            }
        }

        internal long Process() => Parts.Where(p => Process(p) == "A").Sum(x => x.Rating);

        internal long Expand()
        {
            var fringe = new List<List<Range>>
            {
                Workflows["in"].Ranges().ToList()
            };

            while (true)
            {
                var rules = fringe.FirstOrDefault(x => x.Any(x => !x.Expanded));
                if (rules is null)
                {
                    break;
                }

                for (int i = 0; i < rules.Count; i++)
                {
                    var rule = rules[i];
                    if (rule.Expanded)
                    {
                        continue;
                    }

                    fringe.Remove(rules);

                    List<Range> exp = [.. rules[0..i], rule with { Expanded = true }];
                    if (Workflows.TryGetValue(exp[^1].Target, out Workflow? wf))
                    {
                        exp.AddRange(wf.Ranges());
                    }
                    fringe.Add(exp);

                    if (rule.Rating != ' ')
                    {
                        fringe.Add([.. rules[0..i], rule with { Expanded = true, Start = rule.RevStart, End = rule.RevEnd }, .. rules[(i + 1)..]]);
                    }

                    break;
                }
            }

            return fringe.Sum(SumUp);
        }

        long SumUp(List<Range> path)
        {
            if (path.Last().Target == "R")
            {
                return 0;
            }

            "xmas".ToList().ForEach(c => path.Add(new(1, 4000, string.Empty, c, 0, 0)));

            return path
                .GroupBy(x => x.Rating)
                .Where(x => x.Key != ' ')
                .Select(x => new { x.MaxBy(y => y.Start)!.Start, x.MinBy(y => y.End)!.End })
                .Aggregate(1L, (a, b) => a * (b.End - b.Start + 1));
        }
    }

    protected override object Part1() => new System(ReadAllLines(true)).Process();

    protected override object Part2() => new System(ReadAllLines(true)).Expand();
}