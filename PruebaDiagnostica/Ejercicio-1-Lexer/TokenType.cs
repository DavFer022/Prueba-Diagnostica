namespace PruebaDiagnostica;

public enum TokenType
{
    NUMERO,
    OPERADOR,
    PAREN_IZQ,
    PAREN_DER,
    OPERANDO,
    ERROR,

    /// <summary>
    /// Uso interno del Lexer: representa espacios en blanco que se descartan
    /// antes de construir la lista de tokens definitiva. No aparece en la salida.
    /// </summary>
    ESPACIO
}

public record Token(TokenType Tipo, string Lexema);
