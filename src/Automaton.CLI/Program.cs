using Automaton.Core.Services;

// ── Hlavička ──────────────────────────────────────────
Console.WriteLine("╔════════════════════════════════╗");
Console.WriteLine("║       DFA Simulator v1.0       ║");
Console.WriteLine("╚════════════════════════════════╝");

// ── Argumenty z príkazového riadku ────────────────────
// Použitie: dotnet run --automaton <cesta> --input <retazec>

string? automatonPath = null;
string? inputString = null;

for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--automaton" && i + 1 < args.Length)
        automatonPath = args[i + 1];

    if (args[i] == "--input" && i + 1 < args.Length)
        inputString = args[i + 1];
}

// ── Interaktívny mód ak nie sú zadané argumenty ───────
if (automatonPath == null)
{
    Console.Write("\nZadaj cestu k JSON súboru automatu: ");
    automatonPath = Console.ReadLine()?.Trim();
}

if (inputString == null)
{
    Console.Write("Zadaj vstupný reťazec: ");
    inputString = Console.ReadLine()?.Trim() ?? "";
}

// ── Načítaj automat ───────────────────────────────────
try
{
    var loader = new TransitionLoader();
    var automaton = loader.LoadFromFile(automatonPath!);

    Console.WriteLine($"\nAutomat:     {automaton.Name}");
    Console.WriteLine($"Popis:       {automaton.Description}");
    Console.WriteLine($"Stavy:       {string.Join(", ", automaton.States.Select(s => s.Name))}");
    Console.WriteLine($"Abeceda:     {string.Join(", ", automaton.Alphabet)}");
    Console.WriteLine($"Poč. stav:   {automaton.InitialState}");
    Console.WriteLine($"Akc. stavy:  {string.Join(", ", automaton.States.Where(s => s.IsAccepting).Select(s => s.Name))}");

    // ── Spusti simuláciu ──────────────────────────────
    var engine = new DfaEngine(automaton);
    var result = engine.Run(inputString);

    result.PrintToConsole();
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"\n❌ Chyba: {ex.Message}");
}
catch (FormatException ex)
{
    Console.WriteLine($"\n❌ Chyba formátu JSON: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"\n❌ Chyba automatu: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Neočakávaná chyba: {ex.Message}");
}