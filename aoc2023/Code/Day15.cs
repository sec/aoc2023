namespace aoc2023.Code;

internal partial class Day15 : BaseDay
{
    record Lens(string Label, int FocalLength);

    static int HASH(string input) => input.Aggregate(0, (a, b) => ((a + b) * 17) % 256);

    protected override object Part1() => ReadAllTextSplit(",").Sum(HASH);

    protected override object Part2()
    {
        var input = ReadAllTextSplit(",").Select(x => x.Split((char[]) ['=', '-'], options: StringSplitOptions.RemoveEmptyEntries));

        var boxes = new Dictionary<int, List<Lens>>();
        _ = Enumerable.Range(0, 256).Select(x => boxes[x] = []).ToList();

        foreach (var inst in input)
        {
            var label = inst[0];
            var box = boxes[HASH(label)];

            if (inst.Length == 1)
            {
                box.RemoveAll(x => x.Label == label);
            }
            else
            {
                var length = inst[1][0] - '0';
                var index = box.FindIndex(x => x.Label == label);
                var len = new Lens(label, length);

                if (index > -1)
                {
                    box[index] = len;
                }
                else
                {
                    box.Add(len);
                }
            }
        }

        return boxes.Sum(box => Enumerable.Range(0, box.Value.Count).Sum(slot => (box.Key + 1) * (slot + 1) * box.Value[slot].FocalLength));
    }
}