using System.Text.Json;
using Automaton.Core.Models;

namespace Automaton.Core.Services;

public class TransitionLoader
{
    public Models.Automaton LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Súbor '{path}' neexistuje.");

        string json = File.ReadAllText(path);
        return ParseJson(json);
    }

    public Models.Automaton LoadFromJson(string json)
    {
        return ParseJson(json);
    }

    private Models.Automaton ParseJson(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Helper na čitateľné chyby
        string GetString(string key)
        {
            if (!root.TryGetProperty(key, out var prop))
                throw new KeyNotFoundException($"JSON neobsahuje povinný kľúč: '{key}'");
            return prop.GetString() ?? "";
        }

        var automaton = new Models.Automaton
        {
            Name        = GetString("name"),
            Description = GetString("description"),
            InitialState = GetString("initial_state")
        };

        // Abeceda
        if (!root.TryGetProperty("alphabet", out var alphabet))
            throw new KeyNotFoundException("JSON neobsahuje kľúč: 'alphabet'");
        foreach (var symbol in alphabet.EnumerateArray())
            automaton.Alphabet.Add(symbol.GetString() ?? "");

        // Akceptačné stavy
        if (!root.TryGetProperty("accepting_states", out var acceptingRaw))
            throw new KeyNotFoundException("JSON neobsahuje kľúč: 'accepting_states'");
        var acceptingStates = new HashSet<string>();
        foreach (var s in acceptingRaw.EnumerateArray())
            acceptingStates.Add(s.GetString() ?? "");

        // Stavy
        if (!root.TryGetProperty("states", out var statesRaw))
            throw new KeyNotFoundException("JSON neobsahuje kľúč: 'states'");
        foreach (var s in statesRaw.EnumerateArray())
        {
            string name = s.GetString() ?? "";
            automaton.States.Add(new State(name, acceptingStates.Contains(name)));
        }

        // Prechody
        if (!root.TryGetProperty("transitions", out var transitionsRaw))
            throw new KeyNotFoundException("JSON neobsahuje kľúč: 'transitions'");
        foreach (var t in transitionsRaw.EnumerateArray())
        {
            if (!t.TryGetProperty("from", out var fromProp))
                throw new KeyNotFoundException("Prechod neobsahuje kľúč: 'from'");
            if (!t.TryGetProperty("input", out var inputProp))
                throw new KeyNotFoundException("Prechod neobsahuje kľúč: 'input'");
            if (!t.TryGetProperty("to", out var toProp))
                throw new KeyNotFoundException("Prechod neobsahuje kľúč: 'to'");

            string from     = fromProp.GetString() ?? "";
            string inputStr = inputProp.GetString() ?? "";
            string to       = toProp.GetString() ?? "";

            if (inputStr.Length != 1)
                throw new FormatException($"Vstup prechodu musí byť jeden znak, dostali sme: '{inputStr}'");

            automaton.Transitions.Add(new Transition(from, inputStr[0], to));
        }

        Validate(automaton);
        return automaton;
    }

    private void Validate(Models.Automaton automaton)
    {
        if (string.IsNullOrEmpty(automaton.InitialState))
            throw new InvalidOperationException("Automat nemá definovaný počiatočný stav.");

        if (automaton.States.Count == 0)
            throw new InvalidOperationException("Automat nemá žiadne stavy.");

        if (!automaton.States.Any(s => s.Name == automaton.InitialState))
            throw new InvalidOperationException($"Počiatočný stav '{automaton.InitialState}' neexistuje v zozname stavov.");

        if (!automaton.States.Any(s => s.IsAccepting))
            throw new InvalidOperationException("Automat nemá žiadny akceptačný stav.");
    }
}