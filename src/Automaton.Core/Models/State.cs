namespace Automaton.Core.Models;

public class State
{
    public string Name { get; set; }
    public bool IsAccepting { get; set; }

    public State(string name, bool isAccepting = false)
    {
        Name = name;
        IsAccepting = isAccepting;
    }

    public override string ToString() => Name;
}