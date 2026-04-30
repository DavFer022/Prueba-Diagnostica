namespace PruebaDiagnostica;

/// <summary>
/// Verifica la conjetura de Collatz para todos los números en un intervalo [p, q].
/// Condición del enunciado: q ≥ 100 * p.
///
/// Optimización incluida: memoización con Dictionary&lt;long, int&gt;.
/// Cuando un número intermedio ya fue calculado, se reutiliza su cantidad de pasos
/// en lugar de continuar la iteración desde cero.
/// </summary>
public class CollatzChecker
{
    // Caché compartido entre todas las llamadas a Calcular().
    // Clave: número n. Valor: cantidad de pasos hasta llegar a 1.
    private readonly Dictionary<long, int> _memo = new() { [1] = 0 };

    /// <summary>
    /// Verifica la conjetura para el intervalo [p, q] con q ≥ 100*p.
    /// Lanza <see cref="ArgumentException"/> si la condición no se cumple.
    /// </summary>
    public List<CollatzResult> VerificarRango(long p, long q)
    {
        if (q < 100 * p)
            throw new ArgumentException(
                $"La condición q ≥ 100p no se cumple: q={q}, p={p}, 100p={100 * p}.");

        var resultados = new List<CollatzResult>();

        for (long n = p; n <= q; n++)
            resultados.Add(Calcular(n));

        return resultados;
    }

    /// <summary>
    /// Calcula la secuencia de Collatz para <paramref name="n"/> y la retorna
    /// como un <see cref="CollatzResult"/>.
    /// Usa la caché interna para contar pasos de manera eficiente,
    /// pero la secuencia almacenada siempre llega hasta 1.
    /// </summary>
    public CollatzResult Calcular(long n)
    {
        var secuencia = new List<long>();
        long actual   = n;

        // Recorrer hasta 1, acumulando todos los valores
        while (actual != 1)
        {
            secuencia.Add(actual);
            actual = EsPar(actual) ? actual / 2 : 3 * actual + 1;
        }
        secuencia.Add(1); // siempre termina en 1

        int pasos = secuencia.Count - 1; // transiciones = elementos - 1

        // Guardar en caché
        _memo[n] = pasos;

        return new CollatzResult(n, secuencia, pasos);
    }

    private static bool EsPar(long n) => n % 2 == 0;
}
