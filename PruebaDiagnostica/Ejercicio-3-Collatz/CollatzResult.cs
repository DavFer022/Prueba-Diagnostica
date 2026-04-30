namespace PruebaDiagnostica;

public record CollatzResult(
    long NumeroInicial,
    List<long> Secuencia,
    int Pasos
);
