using System.Text.RegularExpressions;

namespace PruebaDiagnostica;

/// <summary>
/// Valida cadenas en notación FEN (Forsyth-Edwards Notation).
/// Un FEN tiene 6 campos separados por espacios:
///   1. Posición del tablero  (8 filas separadas por '/')
///   2. Turno                 ('w' o 'b')
///   3. Disponibilidad de enroque ('K','Q','k','q' o '-')
///   4. Casilla de captura al paso (coordenada como 'e3' o '-')
///   5. Contador de semi-movimientos (entero ≥ 0)
///   6. Número de movimiento completo (entero ≥ 1)
/// </summary>
public class FenValidator
{
    // Regex para validar la fila de piezas: solo letras de piezas y dígitos 1-8
    private static readonly Regex _patronFila =
        new(@"^[rnbqkpRNBQKP1-8]+$", RegexOptions.Compiled);

    // Regex para el campo de enroque
    private static readonly Regex _patronEnroque =
        new(@"^(K?Q?k?q?|-)$", RegexOptions.Compiled);

    // Regex para captura al paso: coordenada como 'e3', 'a6', o '-'
    private static readonly Regex _patronCapturaAlPaso =
        new(@"^([a-h][36]|-)$", RegexOptions.Compiled);

    /// <summary>
    /// Intenta parsear la cadena como una posición FEN válida.
    /// Retorna true y rellena <paramref name="resultado"/> si es válida.
    /// Retorna false y describe el fallo en <paramref name="error"/> si no lo es.
    /// </summary>
    public bool TryParse(string input, out FenPosition? resultado, out string error)
    {
        resultado = null;

        // ── Campo 0: dividir en exactamente 6 partes ──────────────────────
        string[] partes = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length != 6)
        {
            if (partes.Length < 6)
            {
                var camposFaltantes = new List<string>();
                if (partes.Length < 1) camposFaltantes.Add("Tablero (ej. rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR)");
                if (partes.Length < 2) camposFaltantes.Add("Turno (ej. w o b)");
                if (partes.Length < 3) camposFaltantes.Add("Enroque (ej. KQkq o -)");
                if (partes.Length < 4) camposFaltantes.Add("Captura al paso (ej. e3 o -)");
                if (partes.Length < 5) camposFaltantes.Add("Semi-movimientos (ej. 0)");
                if (partes.Length < 6) camposFaltantes.Add("Movimiento completo (ej. 1)");

                error = $"Se esperaban 6 campos, pero se encontraron {partes.Length}.\n\nFaltan los siguientes campos:\n" + 
                        string.Join("\n", camposFaltantes.Select(c => $"• {c}"));
            }
            else
            {
                error = $"Se esperaban 6 campos separados por espacio, pero se encontraron {partes.Length}.";
            }
            return false;
        }

        // ── Campo 1: tablero ──────────────────────────────────────────────
        if (!ValidarTablero(partes[0], out error))
            return false;

        // ── Campo 2: turno ────────────────────────────────────────────────
        if (!ValidarTurno(partes[1], out char turno, out error))
            return false;

        // ── Campo 3: enroque ──────────────────────────────────────────────
        if (!_patronEnroque.IsMatch(partes[2]))
        {
            error = $"Campo de enroque inválido: '{partes[2]}'. " +
                    "Se esperaba una combinación de K, Q, k, q o '-'.";
            return false;
        }

        // ── Campo 4: captura al paso ──────────────────────────────────────
        if (!_patronCapturaAlPaso.IsMatch(partes[3]))
        {
            error = $"Casilla de captura al paso inválida: '{partes[3]}'. " +
                    "Se esperaba una coordenada como 'e3' o '-'.";
            return false;
        }

        // ── Campo 5: semi-movimientos (≥ 0) ──────────────────────────────
        if (!int.TryParse(partes[4], out int semiMov) || semiMov < 0)
        {
            error = $"Contador de semi-movimientos inválido: '{partes[4]}'. " +
                    "Se esperaba un entero ≥ 0.";
            return false;
        }

        // ── Campo 6: número de movimiento (≥ 1) ──────────────────────────
        if (!int.TryParse(partes[5], out int movimiento) || movimiento < 1)
        {
            error = $"Número de movimiento inválido: '{partes[5]}'. " +
                    "Se esperaba un entero ≥ 1.";
            return false;
        }

        // Todo válido: construir el objeto de dominio
        resultado = new FenPosition(partes[0], turno, partes[2], partes[3], semiMov, movimiento);
        error = string.Empty;
        return true;
    }

    // ── Validaciones privadas por campo ───────────────────────────────────

    /// <summary>
    /// Valida el campo de tablero: 8 filas separadas por '/',
    /// cada fila debe sumar exactamente 8 casillas
    /// (dígito = ese número de casillas vacías, letra = 1 pieza).
    /// </summary>
    private static bool ValidarTablero(string tablero, out string error)
    {
        string[] filas = tablero.Split('/');

        if (filas.Length != 8)
        {
            error = $"El campo de tablero debe tener 8 filas separadas por '/', " +
                    $"se encontraron {filas.Length}.";
            return false;
        }

        for (int i = 0; i < filas.Length; i++)
        {
            string fila = filas[i];

            if (!_patronFila.IsMatch(fila))
            {
                error = $"Fila {i + 1} contiene caracteres no válidos: '{fila}'.";
                return false;
            }

            int casillas = 0;
            foreach (char c in fila)
            {
                if (char.IsDigit(c))
                    casillas += c - '0';   // '3' → 3 casillas vacías
                else
                    casillas += 1;         // pieza ocupa 1 casilla
            }

            if (casillas != 8)
            {
                error = $"Fila {i + 1} suma {casillas} casillas, se esperaban 8: '{fila}'.";
                return false;
            }
        }

        error = string.Empty;
        return true;
    }

    private static bool ValidarTurno(string campo, out char turno, out string error)
    {
        if (campo == "w") { turno = 'w'; error = string.Empty; return true; }
        if (campo == "b") { turno = 'b'; error = string.Empty; return true; }

        turno = '\0';
        error = $"Turno inválido: '{campo}'. Se esperaba 'w' (blancas) o 'b' (negras).";
        return false;
    }
}
