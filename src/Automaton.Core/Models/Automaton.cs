namespace Automaton.Core.Models;

public class Automaton
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string InitialState { get; set; }
    public List<string> Alphabet { get; set; } = new();
    public List<State> States { get; set; } = new();
    public List<Transition> Transitions { get; set; } = new();

    public State? GetState(string name)
        => States.FirstOrDefault(s => s.Name == name);

    public string? GetNextState(string currentState, char input)
    {
        var transition = Transitions.FirstOrDefault(t =>
            t.From == currentState && t.Input == input);

        return transition?.To;
    }

    public bool IsAccepting(string stateName)
        => States.Any(s => s.Name == stateName && s.IsAccepting);
}