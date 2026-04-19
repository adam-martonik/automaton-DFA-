namespace Automaton.Core.Models;

public class Automaton
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Alphabet { get; set; }
    public List<State> States { get; set; }
    public string InitialState { get; set; }
    public List<Transition> Transitions { get; set; }

    public Automaton()
    {
        Alphabet = new List<string>();
        States = new List<State>();
        Transitions = new List<Transition>();
    }

    // Vráti stav podľa mena
    public State? GetState(string name)
        => States.FirstOrDefault(s => s.Name == name);

    // Vráti nasledujúci stav podľa aktuálneho stavu a vstupu
    public string? GetNextState(string currentState, char input)
    {
        var transition = Transitions.FirstOrDefault(t =>
            t.From == currentState && t.Input == input);

        return transition?.To;
    }

    // Je daný stav akceptačný?
    public bool IsAccepting(string stateName)
        => States.Any(s => s.Name == stateName && s.IsAccepting);
}