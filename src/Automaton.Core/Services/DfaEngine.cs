namespace Automaton.Core.Services;

public class DfaEngine
{
    private readonly Models.Automaton _automaton;

    public DfaEngine(Models.Automaton automaton)
    {
        _automaton = automaton;
    }

    public DfaResult Run(string input)
    {
        string currentState = _automaton.InitialState;
        var steps = new List<DfaStep>();

        foreach (char symbol in input)
        {
            // Je symbol v abecede?
            if (!_automaton.Alphabet.Contains(symbol.ToString()))
            {
                return new DfaResult(
                    accepted: false,
                    input: input,
                    steps: steps,
                    reason: $"Symbol '{symbol}' nie je v abecede automatu"
                );
            }

            string? nextState = _automaton.GetNextState(currentState, symbol);

            // Neexistuje prechod – mŕtvy stav
            if (nextState == null)
            {
                steps.Add(new DfaStep(currentState, symbol, null));
                return new DfaResult(
                    accepted: false,
                    input: input,
                    steps: steps,
                    reason: $"Neexistuje prechod z '{currentState}' pre '{symbol}'"
                );
            }

            steps.Add(new DfaStep(currentState, symbol, nextState));
            currentState = nextState;
        }

        bool isAccepted = _automaton.IsAccepting(currentState);

        return new DfaResult(
            accepted: isAccepted,
            input: input,
            steps: steps,
            reason: isAccepted
                ? $"Automat skončil v akceptačnom stave '{currentState}'"
                : $"Automat skončil v neakceptačnom stave '{currentState}'"
        );
    }
}

// Jeden krok automatu
public record DfaStep(string FromState, char Input, string? ToState)
{
    public override string ToString() => $"{FromState} --{Input}--> {ToState ?? "DEAD"}";
}

// Výsledok behu automatu
public record DfaResult(bool Accepted, string Input, List<DfaStep> Steps, string Reason)
{
    public void PrintToConsole()
    {
        Console.WriteLine($"\nVstup: '{Input}'");
        Console.WriteLine("Kroky:");
        foreach (var step in Steps)
            Console.WriteLine($"  {step}");

        Console.WriteLine($"\n{(Accepted ? "✅ ACCEPTED" : "❌ REJECTED")}");
        Console.WriteLine($"Dôvod: {Reason}");
    }
}