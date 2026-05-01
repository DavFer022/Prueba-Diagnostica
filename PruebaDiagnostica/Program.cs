// ============================================================
//  Prueba Diagnóstica – C# / .NET 10
//  UI: Spectre.Console – menú con flechas + entrada del usuario
// ============================================================

using PruebaDiagnostica;
using Spectre.Console;

// ─── Bucle principal ─────────────────────────────────────────
while (true)
{
    AnsiConsole.Clear();
    MostrarEncabezado();

    var opcion = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[grey]Usa las flechas  y presiona [green]Enter[/] para seleccionar:[/]")
            .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
            .AddChoices(
                "1 · Analizador Léxico",
                "2 · Validador de Notación FEN",
                "3 · Conjetura de Collatz",
                "[red]Salir[/]"
            ));

    AnsiConsole.Clear();

    if (opcion.StartsWith("1")) EjercicioLexer();
    else if (opcion.StartsWith("2")) EjercicioFEN();
    else if (opcion.StartsWith("3")) EjercicioCollatz();
    else break;

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para volver al menú...[/]");
    Console.ReadKey(intercept: true);
}

AnsiConsole.MarkupLine("\n[green]¡Hasta luego![/]\n");

// ═══════════════════════════════════════════════════════════
//  EJERCICIO 1 – Analizador Léxico
// ═══════════════════════════════════════════════════════════
static void EjercicioLexer()
{
    MostrarTitulo("Ejercicio 1", "Analizador Léxico", Color.DodgerBlue1);

    // ── Entrada del usuario ──────────────────────────────────
    string expresion = AnsiConsole.Prompt(
        new TextPrompt<string>("[bold]Ingresa una expresión aritmética:[/]")
            .PromptStyle("cyan")
            .DefaultValue("12 + b * (3.5 - x)")
            .AllowEmpty());

    AnsiConsole.WriteLine();

    var lexer = new Lexer();
    var tokens = lexer.Tokenizar(expresion);

    // ── Tabla de tokens ──────────────────────────────────────
    var tabla = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.DodgerBlue1)
        .Title($"[bold dodgerblue1]Tokens de:[/] [italic]{Markup.Escape(expresion)}[/]")
        .AddColumn(new TableColumn("[bold]Lexema[/]").Centered())
        .AddColumn(new TableColumn("[bold]Tipo[/]").Centered());

    foreach (var tok in tokens)
    {
        string color = tok.Tipo switch
        {
            TokenType.NUMERO => "yellow",
            TokenType.OPERADOR => "cyan",
            TokenType.PAREN_IZQ => "magenta",
            TokenType.PAREN_DER => "magenta",
            TokenType.OPERANDO => "green",
            TokenType.ERROR => "red",
            _ => "white"
        };
        tabla.AddRow(
            $"[{color}]{Markup.Escape($"\"{tok.Lexema}\"")}[/]",
            $"[{color}]{tok.Tipo}[/]"
        );
    }

    AnsiConsole.Write(tabla);
    AnsiConsole.WriteLine();

    // ── Balanceo ─────────────────────────────────────────────
    bool ok = lexer.VerificarBalanceo(tokens, out string msg);

    var panel = new Panel(ok
            ? $"[green bold]✓ {Markup.Escape(msg)}[/]"
            : $"[red bold]✗ {Markup.Escape(msg)}[/]")
        .Header("[bold]Balance de paréntesis[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(ok ? Color.Green : Color.Red);

    AnsiConsole.Write(panel);
}

// ═══════════════════════════════════════════════════════════
//  EJERCICIO 2 – Validador de Notación FEN
// ═══════════════════════════════════════════════════════════
static void EjercicioFEN()
{
    MostrarTitulo("Ejercicio 2", "Validador de Notación FEN", Color.MediumOrchid);

    bool continuar = true;
    while (continuar)
    {
        // ── Entrada del usuario ──────────────────────────────────
        string fen = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold]Ingresa una cadena FEN:[/]")
                .PromptStyle("magenta")
                .DefaultValue("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
                .AllowEmpty());

        AnsiConsole.WriteLine();

        var validator = new FenValidator();
        bool valido = validator.TryParse(fen, out FenPosition? pos, out string error);

        // ── FEN ingresado (en panel) ─────────────────────────────
        AnsiConsole.Write(new Panel($"[italic]{Markup.Escape(fen)}[/]")
            .Header("[bold]Cadena analizada[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.MediumOrchid));

        AnsiConsole.WriteLine();

        if (valido && pos is not null)
        {
            // Tabla de campos válidos
            var tabla = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .Title("[bold green]✓ FEN VÁLIDO[/]")
                .AddColumn(new TableColumn("[bold]Campo[/]"))
                .AddColumn(new TableColumn("[bold]Valor[/]"));

            tabla.AddRow("Turno", pos.Turno == 'w' ? "[white]Blancas[/]" : "[grey]Negras[/]");
            tabla.AddRow("Enroque", $"[cyan]{Markup.Escape(pos.Enroque)}[/]");
            tabla.AddRow("Captura al paso", $"[cyan]{Markup.Escape(pos.CapturaAlPaso)}[/]");
            tabla.AddRow("Semi-movimientos", $"[yellow]{pos.SemiMovimientos}[/]");
            tabla.AddRow("Movimiento", $"[yellow]{pos.Movimiento}[/]");

            AnsiConsole.Write(tabla);
            continuar = false; // Sale del bucle de validación con éxito
        }
        else
        {
            AnsiConsole.Write(new Panel($"[red bold]{Markup.Escape(error)}[/]")
                .Header("[bold red]✗ FEN INVÁLIDO[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Red));

            AnsiConsole.WriteLine();
            if (!AnsiConsole.Confirm("[yellow]¿Deseas intentar de nuevo?[/]", defaultValue: true))
            {
                continuar = false;
            }
            else
            {
                AnsiConsole.Clear();
                MostrarTitulo("Ejercicio 2", "Validador de Notación FEN", Color.MediumOrchid);
            }
        }
    }
}

// ═══════════════════════════════════════════════════════════
//  EJERCICIO 3 – Conjetura de Collatz
// ═══════════════════════════════════════════════════════════
static void EjercicioCollatz()
{
    MostrarTitulo("Ejercicio 3", "Conjetura de Collatz", Color.Gold1);

    // ── Entrada del usuario ──────────────────────────────────
    long p = AnsiConsole.Prompt(
        new TextPrompt<long>("[bold]Ingresa el límite inferior [yellow](p)[/]:[/]")
            .PromptStyle("yellow")
            .DefaultValue(1)
            .Validate(v => v >= 1
                ? ValidationResult.Success()
                : ValidationResult.Error("[red]p debe ser ≥ 1[/]")));

    long q = AnsiConsole.Prompt(
        new TextPrompt<long>($"[bold]Ingresa el límite superior [yellow](q)[/] [grey](debe ser ≥ 100×{p} = {100 * p})[/]:[/]")
            .PromptStyle("yellow")
            .DefaultValue(100 * p)
            .Validate(v => v >= 100 * p
                ? ValidationResult.Success()
                : ValidationResult.Error($"[red]q debe ser ≥ 100p = {100 * p}[/]")));

    AnsiConsole.WriteLine();

    var checker = new CollatzChecker();
    List<CollatzResult> resultados;

    // Spinner mientras calcula (útil si el rango es grande)
    AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("yellow"))
        .Start($"Calculando Collatz para [[{p}, {q}]]...", _ =>
        {
            resultados = checker.VerificarRango(p, q);
        });

    // Volver a calcular fuera del spinner para usar los resultados
    resultados = checker.VerificarRango(p, q);

    // ── Tabla de resultados (primeras 10 filas) ───────────────
    int mostrar = Math.Min(10, resultados.Count);

    var tabla = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Gold1)
        .Title($"[bold gold1]Intervalo [[{p}, {q}]][/]  [grey](mostrando {mostrar} de {resultados.Count})[/]")
        .AddColumn(new TableColumn("[bold]n[/]").RightAligned())
        .AddColumn(new TableColumn("[bold]Pasos[/]").RightAligned())
        .AddColumn(new TableColumn("[bold]Secuencia (máx. 12 valores)[/]"));

    for (int i = 0; i < mostrar; i++)
    {
        var r = resultados[i];
        string sec = string.Join(" [grey]→[/] ", r.Secuencia.Take(12).Select(v => $"[yellow]{v}[/]"));
        if (r.Secuencia.Count > 12) sec += " [grey]→ ...[/]";
        tabla.AddRow($"[cyan]{r.NumeroInicial}[/]", $"[green]{r.Pasos}[/]", sec);
    }

    AnsiConsole.Write(tabla);
    AnsiConsole.WriteLine();

    // ── Resumen ───────────────────────────────────────────────
    var maxPasos = resultados.MaxBy(r => r.Pasos)!;

    var resumen = new Table()
        .HideHeaders()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Gold1)
        .AddColumn("")
        .AddColumn("");

    resumen.AddRow("[bold]Números verificados[/]", $"[green]{resultados.Count}  ✓ todos llegan a 1[/]");
    resumen.AddRow("[bold]Con más pasos[/]", $"[cyan]n = {maxPasos.NumeroInicial}[/]  → [green]{maxPasos.Pasos} pasos[/]");

    AnsiConsole.Write(resumen);

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[grey]Presiona [bold white]V[/] para ver todos los resultados, o cualquier otra tecla para continuar...[/]");
    
    var keyInfo = Console.ReadKey(intercept: true);
    if (keyInfo.Key == ConsoleKey.V)
    {
        AnsiConsole.Clear();
        MostrarTitulo("Ejercicio 3", "Conjetura de Collatz (Completo)", Color.Gold1);

        var tablaCompleta = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Gold1)
            .Title($"[bold gold1]Intervalo [[{p}, {q}]][/]  [grey](Todos los {resultados.Count} números)[/]")
            .AddColumn(new TableColumn("[bold]n[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Pasos[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Secuencia (máx. 12 valores)[/]"));

        foreach (var r in resultados)
        {
            string sec = string.Join(" [grey]→[/] ", r.Secuencia.Take(12).Select(v => $"[yellow]{v}[/]"));
            if (r.Secuencia.Count > 12) sec += " [grey]→ ...[/]";
            tablaCompleta.AddRow($"[cyan]{r.NumeroInicial}[/]", $"[green]{r.Pasos}[/]", sec);
        }

        AnsiConsole.Write(tablaCompleta);
    }
}

// ─── Helpers de presentación ──────────────────────────────────
static void MostrarEncabezado()
{
    AnsiConsole.Write(
        new FigletText("Compiladores")
            .Centered()
            .Color(Color.DodgerBlue1));

    AnsiConsole.Write(new Rule("[bold grey]Prueba Diagnóstica – C# / .NET 10[/]")
        .RuleStyle(Style.Parse("grey")));

    AnsiConsole.WriteLine();
}

static void MostrarTitulo(string numero, string nombre, Color color)
{
    AnsiConsole.Write(new Rule($"[bold {color}]{numero} · {nombre}[/]")
        .RuleStyle(Style.Parse(color.ToString())));
    AnsiConsole.WriteLine();
}
