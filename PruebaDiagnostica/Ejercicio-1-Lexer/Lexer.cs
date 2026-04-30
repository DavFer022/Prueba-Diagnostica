using System.Text.RegularExpressions;

namespace PruebaDiagnostica;

/// <summary>
/// Analizador léxico para expresiones aritméticas.
/// Clasifica cada componente en: NUMERO, OPERADOR, PAREN_IZQ, PAREN_DER, OPERANDO o ERROR.
/// </summary>
public class Lexer
{
    // Patrón unificado: el orden importa (de mayor a menor especificidad).
    // Cada alternativa tiene un nombre de grupo que coincide con el TokenType.
    private static readonly Regex _patron = new(
        @"(?<NUMERO>\d+(\.\d+)?)"   +   // entero o real sin signo: 42, 3.14
        @"|(?<OPERANDO>[A-Za-z_][A-Za-z0-9_]*)" + // identificador: x, miVar
        @"|(?<OPERADOR>[+\-*/])"    +   // operadores aritméticos
        @"|(?<PAREN_IZQ>\()"        +   // paréntesis izquierdo
        @"|(?<PAREN_DER>\))"        +   // paréntesis derecho
        @"|(?<ESPACIO>\s+)"         +   // espacios (se ignoran)
        @"|(?<ERROR>.)",                // cualquier otro carácter → error
        RegexOptions.Compiled
    );

    /// <summary>
    /// Tokeniza la expresión de entrada y retorna la lista de tokens encontrados.
    /// Los espacios se descartan; los caracteres no reconocidos se marcan como ERROR.
    /// </summary>
    public List<Token> Tokenizar(string expresion)
    {
        var tokens = new List<Token>();

        foreach (Match coincidencia in _patron.Matches(expresion))
        {
            // Determinar qué grupo capturó
            TokenType tipo = DeterminarTipo(coincidencia);

            // Los espacios no son tokens: se ignoran
            if (tipo == TokenType.ESPACIO)
                continue;

            tokens.Add(new Token(tipo, coincidencia.Value));
        }

        return tokens;
    }

    /// <summary>
    /// Verifica si los paréntesis de la lista de tokens están balanceados.
    /// Retorna true si el balance es correcto; false en caso contrario.
    /// También retorna el mensaje de error en el parámetro de salida.
    /// </summary>
    public bool VerificarBalanceo(List<Token> tokens, out string mensaje)
    {
        int balance = 0;

        foreach (var token in tokens)
        {
            if (token.Tipo == TokenType.PAREN_IZQ)
                balance++;
            else if (token.Tipo == TokenType.PAREN_DER)
            {
                balance--;
                // Cierre antes de apertura: error inmediato
                if (balance < 0)
                {
                    mensaje = "Error: ')' sin '(' correspondiente.";
                    return false;
                }
            }
        }

        if (balance == 0)
        {
            mensaje = "Paréntesis balanceados.";
            return true;
        }
        else
        {
            mensaje = $"Error: {balance} paréntesis '(' sin cerrar.";
            return false;
        }
    }

    // Mapea el nombre del grupo capturado por el Regex al enum TokenType.
    private static TokenType DeterminarTipo(Match m)
    {
        if (m.Groups["NUMERO"].Success)   return TokenType.NUMERO;
        if (m.Groups["OPERANDO"].Success) return TokenType.OPERANDO;
        if (m.Groups["OPERADOR"].Success) return TokenType.OPERADOR;
        if (m.Groups["PAREN_IZQ"].Success) return TokenType.PAREN_IZQ;
        if (m.Groups["PAREN_DER"].Success) return TokenType.PAREN_DER;
        if (m.Groups["ESPACIO"].Success)  return TokenType.ESPACIO;
        return TokenType.ERROR;
    }
}

// Tipo interno usado solo dentro del Lexer para descartar espacios.
// No forma parte del enum público TokenType, por eso se maneja como constante separada.
// Se agrega ESPACIO al enum solo para no contaminar la interfaz pública; ver nota en TokenType.cs.
