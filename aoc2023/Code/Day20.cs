namespace aoc2023.Code;

internal class Day20 : BaseDay
{
    enum Pulse { Low, High };

    record Item(Pulse Pulse, IModule Src, IModule Dst);

    interface IModule
    {
        void ConnectTo(IModule module);

        void Connected(IModule module);

        void Receive(Pulse pulse, IModule source);

        void Attach(Queue<Item> cable);

        bool IsConnectedTo(IModule module);

        Pulse[] GetState();
    }

    abstract class Module : IModule
    {
        Queue<Item> _queue = [];

        protected readonly List<IModule> _modules = [];

        public void Attach(Queue<Item> cable) => _queue = cable;

        public void ConnectTo(IModule module)
        {
            _modules.Add(module);
            module.Connected(this);
        }

        public void Receive(Pulse pulse, IModule source) => ProcessPulse(pulse, source);

        protected void Send(Pulse pulse) => _modules.ForEach(x => _queue.Enqueue(new Item(pulse, this, x)));

        protected abstract void ProcessPulse(Pulse pulse, IModule source);

        public virtual void Connected(IModule module)
        {
        }

        public bool IsConnectedTo(IModule module) => _modules.Any(x => x == module);

        public abstract Pulse[] GetState();
    }

    class Broadcaster : Module
    {
        public override Pulse[] GetState() => throw new NotImplementedException();

        protected override void ProcessPulse(Pulse pulse, IModule source) => Send(pulse);
    }

    class FlipFlow : Module
    {
        bool _state = false;

        protected override void ProcessPulse(Pulse pulse, IModule source)
        {
            if (pulse == Pulse.Low)
            {
                Send(_state ? Pulse.Low : Pulse.High);

                _state = !_state;
            }
        }

        public override Pulse[] GetState() => throw new NotImplementedException();
    }

    class Conjunction : Module
    {
        readonly Dictionary<IModule, Pulse> _state = [];

        protected override void ProcessPulse(Pulse pulse, IModule source)
        {
            _state[source] = pulse;

            Send(_state.Values.Any(x => x == Pulse.Low) ? Pulse.High : Pulse.Low);
        }

        public override void Connected(IModule module) => _state[module] = Pulse.Low;

        public override Pulse[] GetState() => _state.Values.ToArray();
    }

    class System
    {
        readonly Dictionary<string, IModule> _modules;
        readonly Broadcaster _button;
        readonly Queue<Item> _queue;

        public System(Dictionary<string, List<string>> modules)
        {
            _modules = [];
            _queue = [];

            foreach (var name in modules.Keys)
            {
                _modules[name[1..]] = Create(name);
            }

            foreach (var kv in modules)
            {
                var module = _modules[kv.Key[1..]];

                kv.Value.ForEach(x =>
                {
                    if (!_modules.TryGetValue(x, out var m))
                    {
                        _modules[x] = m = new Broadcaster();
                    }
                    module.ConnectTo(m);
                });
            }

            _button = new Broadcaster();
            _button.ConnectTo(_modules["roadcaster"]);

            _modules["button"] = _button;
            _modules.Values.ToList().ForEach(x => x.Attach(_queue));
        }

        static IModule Create(string name)
        {
            return name[0] switch
            {
                'b' => new Broadcaster(),
                '%' => new FlipFlow(),
                '&' => new Conjunction(),
                _ => throw new NotImplementedException()
            };
        }

        internal long PushTheButton(int times)
        {
            var stats = new long[2];

            for (int i = 0; i < times; i++)
            {
                _button.Receive(Pulse.Low, _button);

                while (_queue.TryDequeue(out var msg))
                {
                    stats[msg.Pulse == Pulse.Low ? 0 : 1]++;

                    msg.Dst.Receive(msg.Pulse, msg.Src);
                }
            }

            return stats[0] * stats[1];
        }

        internal long StartTheMachine()
        {
            if (!_modules.TryGetValue("rx", out var rx))
            {
                return 0;
            }
            var hf = _modules.Values.Single(x => x.IsConnectedTo(rx));

            long[] magic = new long[hf.GetState().Length];
            long i = 0;

            while (true)
            {
                i++;
                _button.Receive(Pulse.Low, _button);

                while (_queue.TryDequeue(out var msg))
                {
                    msg.Dst.Receive(msg.Pulse, msg.Src);

                    var states = hf.GetState();
                    for (int j = 0; j < magic.Length; j++)
                    {
                        if (magic[j] == 0 && states[j] == Pulse.High)
                        {
                            magic[j] = i;
                        }
                    }

                    if (magic.All(x => x != 0))
                    {
                        // assume all are primes, if not LCM
                        return magic.Aggregate(1L, (a, b) => a * b);
                    }
                }
            }
        }
    }

    System Spawn() => new(ReadAllLines(true)
            .Select(x => x.Split("->", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Select(x => new { Name = x[0], Connected = x[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) })
            .ToDictionary(x => x.Name, x => x.Connected.ToList()));

    protected override object Part1() => Spawn().PushTheButton(1000);

    protected override object Part2() => Spawn().StartTheMachine();
}