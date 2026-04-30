# Prueba Diagnóstica - Compiladores

Este repositorio contiene la implementación en C# (.NET 10) de los tres ejercicios de la prueba diagnóstica:

1. **Analizador Léxico**: Tokenizador de expresiones aritméticas con soporte para verificación de balanceo de paréntesis.
2. **Validador FEN**: Validador de notación de estados de tablero de ajedrez usando expresiones regulares y validación estructurada.
3. **Conjetura de Collatz**: Demostrador iterativo con optimización mediante memoización para evitar recalcular rutas comunes.

El proyecto cuenta con una interfaz de consola interactiva amigable construida con `Spectre.Console`.

---

## 🛠️ Requisitos Previos e Instalación

Para compilar y ejecutar este proyecto, necesitas tener instalado el **SDK de .NET 10.0** (o superior). A continuación, se detallan los scripts de instalación de acuerdo al sistema operativo:

### Windows (PowerShell)
Puedes instalarlo usando el gestor de paquetes `winget`:
```powershell
winget install Microsoft.DotNet.SDK.10
```

### Linux (Ubuntu / Debian)
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
```

### macOS (Homebrew)
```bash
brew install --cask dotnet-sdk
```

> **Verificación:** Tras la instalación, puedes verificar que todo está correcto ejecutando `dotnet --version` en la terminal.

---

## 🚀 Cómo ejecutar la solución

1. Clona este repositorio y accede a la carpeta del código:
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   cd PruebaDiagnostica
   ```

2. Restaura las dependencias (como `Spectre.Console`):
   ```bash
   dotnet restore
   ```

3. Compila y ejecuta la aplicación:
   ```bash
   dotnet run
   ```

El programa te presentará un menú interactivo manejable con las flechas del teclado, desde donde podrás seleccionar, configurar y poner a prueba la lógica de cada ejercicio.
