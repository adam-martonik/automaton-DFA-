namespace Automaton.Core.Models;

public class Transition
{
    public string From { get; set; }
    public char Input { get; set; }
    public string To { get; set; }

    public Transition(string from, char input, string to)
    {
        From = from;
        Input = input;
        To = to;
    }

    public override string ToString() => $"{From} --{Input}--> {To}";
}