# Super Mario Bros (2D) - Unity 6

Proyecto basado en el tutorial de Super Mario Bros de Zigurous, ampliado con nuevos niveles y mejoras.

- **Versión de Unity**: 6000.3.16f1
- **Proyecto base**: [Jesusbautistah/SUPER-MArio](https://github.com/Jesusbautistah/SUPER-MArio) (tutorial original de [Zigurous](https://github.com/zigurous/unity-super-mario-tutorial))

## Mejoras agregadas

1. **Nivel subterráneo 1-2 (réplica del original)**: reconstruido casilla por casilla desde el mapa original: entrada con castillo, escalones, ladrillos, cuarto secreto de monedas, plataformas que suben y bajan, zona de warp ("WELCOME TO WARP ZONE!") y salida con bandera. Goombas y Koopas azules y Planta Piraña.
2. **Nivel acuático 2-2 (réplica del original)**: mapa original con roca de coral, algas y superficie del agua. Mario nada (cada salto es una brazada) con su animación original, burbujas, Cheep Cheeps rojos y grises y Bloopers que persiguen a Mario.
3. **Escena de Game Over**: réplica de la pantalla original (fondo negro, HUD y "GAME OVER" con su jingle). Al terminar vuelve al 1-1; manteniendo Enter se continúa en el mismo mundo.
4. **Castillo de Bowser 2-4 (diseño propio)**: recorrido original hecho con los tiles de los castillos del juego: fosos de lava con Podoboos, barras de fuego, bloques flotantes, Bowser que salta y lanza fuego, puente que se derrumba al tocar el hacha y pantalla final.

Además: pantalla de título como la original (cartel, cursor de honguito, 1 JUGADOR / 2 JUGADORES y TOP con el mejor puntaje), modo de 2 jugadores con Luigi por turnos (cada uno con sus vidas, monedas, puntaje y nivel), música y efectos de sonido en todo el juego, HUD con fuente pixelada (puntaje, monedas, mundo, tiempo y vidas), pantalla de inicio de nivel, pausa (P / Esc), ladrillos que se rompen, cámara suave con límites y zonas por área.

## Controles

| Acción | Teclas |
| --- | --- |
| Elegir en el menú | W / S o flechas, Enter o Espacio para confirmar, Esc para salir |
| Moverse | A / D o flechas |
| Saltar / nadar | Espacio |
| Entrar a tubería | S / flecha abajo, D / flecha derecha (laterales) |
| Pausa | P o Esc |

## Créditos

- **Sprites y mapas originales** (Mario, Luigi, pantalla de título, enemigos, Bowser y los mapas 1-2, 2-2 y 7-4 usados como referencia y fuente de tiles): [The Spriters Resource - NES - Super Mario Bros.](https://www.spriters-resource.com/nes/supermariobros/)
- **Proyecto base y assets del tutorial**: [Zigurous - Unity Super Mario Tutorial](https://github.com/zigurous/unity-super-mario-tutorial)
- **Super Mario Bros.** es propiedad de Nintendo. Este proyecto es solo con fines educativos.
- La música y los efectos de sonido son composiciones propias generadas para este proyecto.









# 🍄 Super Mario Bros (2D) - Unity 6

![Unity Version](https://img.shields.io/badge/Unity-6000.3.16f1-black?style=flat-square&logo=unity)
![Status](https://img.shields.io/badge/Estado-Completado-success?style=flat-square)
![License](https://img.shields.io/badge/Licencia-Educativa-blue?style=flat-square)

Proyecto basado en el tutorial de Super Mario Bros de Zigurous, expandido significativamente con nuevos niveles, mecánicas clásicas y un pulido general para recrear la experiencia original.

* **Proyecto base:** [Jesusbautistah/SUPER-MArio](https://github.com/Jesusbautistah/SUPER-MArio)
* **Tutorial original:** [Zigurous](https://github.com/zigurous/unity-super-mario-tutorial)

---

## 👨‍💻 Equipo de Desarrollo

Este proyecto fue mejorado y expandido por:

* **Alvaro Del Carpio**
* **Eduardo Condoreno**
* **Jhoel Herrera**
* **Carlos Valverde**

---

## ✨ Características y Mejoras Agregadas

Se han implementado expansiones importantes respecto al proyecto base, construyendo una experiencia mucho más completa:

### 🗺️ Nuevos Niveles Recreados
1. **Nivel Subterráneo (1-2) - Réplica fiel:** Reconstruido casilla por casilla desde el mapa original. Incluye la entrada con el castillo, escalones, cuarto secreto de monedas, plataformas móviles, zona de warp (*"WELCOME TO WARP ZONE!"*) y salida con bandera. Enemigos: Goombas, Koopas azules y Plantas Piraña.
2. **Nivel Acuático (2-2) - Réplica fiel:** Mapa original con roca de coral, algas y superficie del agua. Físicas de nado integradas (cada salto es una brazada), animaciones originales, sistema de burbujas, Cheep Cheeps (rojos y grises) y Bloopers que persiguen a Mario.
3. **Castillo de Bowser (2-4) - Diseño propio:** Nivel personalizado construido con los *tiles* clásicos de castillos. Incluye fosos de lava con Podoboos, barras de fuego giratorias, bloques flotantes, enfrentamiento final con Bowser (salta y lanza fuego), puente colapsable al tocar el hacha y pantalla de victoria.

### 🕹️ Sistema de Juego y UI
* **Pantalla de Game Over:** Réplica de la pantalla original (fondo negro, HUD clásico, texto "GAME OVER" y su respectivo *jingle*). Vuelve al nivel 1-1 al terminar; manteniendo la tecla `Enter` se puede continuar en el mismo mundo.
* **Menú Principal Original:** Cartel clásico, cursor interactivo con forma de champiñón, selector de "1 JUGADOR / 2 JUGADORES" y registro del TOP (mejor puntaje).
* **Modo Multijugador (Por turnos):** Posibilidad de jugar con Luigi en modo de 2 jugadores. Cada personaje mantiene sus propias vidas, monedas, puntaje y progreso de nivel de forma independiente.
* **Interfaz HUD Clásica:** Uso de fuente pixelada original para mostrar Puntaje, Monedas, Mundo, Tiempo restante y Vidas.
* **Pantallas de Transición:** Pantalla negra de inicio de nivel ("World 1-1").
* **Pulido general:** Sistema de pausa (`P` / `Esc`), destrucción dinámica de ladrillos, cámara suave con límites (*clamping*) y delimitación estricta de zonas por área.
* **Audio Completo:** Música de fondo y efectos de sonido en todo el transcurso del juego.

---

## 🎮 Controles

El juego está diseñado para jugarse cómodamente con el teclado:

| Acción | Tecla / Comando |
| :--- | :--- |
| **Navegar en el menú** | `W` / `S` o `Flechas Arriba/Abajo` |
| **Confirmar (Menú)** | `Enter` o `Espacio` |
| **Moverse** | `A` / `D` o `Flechas Izquierda/Derecha` |
| **Saltar / Nadar** | `Espacio` |
| **Entrar a tubería (Vertical)** | `S` o `Flecha Abajo` |
| **Entrar a tubería (Lateral)** | `D` o `Flecha Derecha` |
| **Pausa / Salir** | `P` o `Esc` |

---

## 📝 Créditos y Reconocimientos

* **Assets Gráficos:** Sprites, mapas originales (Mario, Luigi, menú, enemigos, Bowser) y referencias de diseño obtenidos de [The Spriters Resource - NES - Super Mario Bros](https://www.spriters-resource.com/nes/supermariobros/).
* **Motor Base:** Código y assets iniciales del tutorial de [Zigurous - Unity Super Mario Tutorial](https://github.com/zigurous/unity-super-mario-tutorial).
* **Audio:** La música y los efectos de sonido son composiciones y arreglos propios generados específicamente para este proyecto.
* **Aviso Legal:** *Super Mario Bros* y todos sus personajes son propiedad intelectual de **Nintendo**. Este proyecto ha sido desarrollado estrictamente con **fines educativos y sin ánimo de lucro**.
