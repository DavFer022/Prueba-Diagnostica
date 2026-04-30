namespace PruebaDiagnostica;

public record FenPosition(
    string Tablero,
    char Turno,
    string Enroque,
    string CapturaAlPaso,
    int SemiMovimientos,
    int Movimiento
);