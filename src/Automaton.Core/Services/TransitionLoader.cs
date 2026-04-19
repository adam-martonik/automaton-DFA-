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

        var automaton = new Models.Automaton
        {
            Name = root.GetProperty("name").GetString() ?? "unknown",
            Description = root.GetProperty("description").GetString() ?? "",
            InitialState = root.GetProperty("initial_state").GetString() ?? ""
        };

        // Načítaj abecedu
        foreach (var symbol in root.GetProperty("alphabet").EnumerateArray())
            automaton.Alphabet.Add(symbol.GetString() ?? "");

        // Načítaj akceptačné stavy
        var acceptingStates = new HashSet<string>();
        foreach (var s in root.GetProperty("accepting_states").EnumerateArray())
            acceptingStates.Add(s.GetString() ?? "");

        // Načítaj stavy
        foreach (var s in root.GetProperty("states").EnumerateArray())
        {
            string name = s.GetString() ?? "";
            automaton.States.Add(new State(name, acceptingStates.Contains(name)));
        }

        // Načítaj prechody
        foreach (var t in root.GetProperty("transitions").EnumerateArray())
        {
            string from = t.GetProperty("from").GetString() ?? "";
            string inputStr = t.GetProperty("input").GetString() ?? "";
            string to = t.GetProperty("to").GetString() ?? "";

            if (inputStr.Length != 1)
                throw new FormatException($"Vstup prechodu musí byť jeden znak, dostali sme: '{inputStr}'");

            automaton.Transitions.Add(new Transition(from, inputStr[0], to));
        }

        // Validácia
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