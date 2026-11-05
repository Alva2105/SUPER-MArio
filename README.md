# 🍄 Super Mario Bros (2D) - Unity 6

![Unity Version](https://img.shields.io/badge/Unity-6000.3.16f1-black?style=flat-square&logo=unity)
![Status](https://img.shields.io/badge/Estado-Completado-success?style=flat-square)
![License](https://img.shields.io/badge/Licencia-Educativa-blue?style=flat-square)

Proyecto basado en el tutorial de Super Mario Bros de Zigurous, expandido con nuevos niveles, mecánicas clásicas y un pulido general para recrear la experiencia original.

* **Versión de Unity:** 6000.3.16f1
* **Proyecto base:** [Jesusbautistah/SUPER-MArio](https://github.com/Jesusbautistah/SUPER-MArio)
* **Tutorial original:** [Zigurous](https://github.com/zigurous/unity-super-mario-tutorial)

---

## 👨‍💻 Equipo de Desarrollo

| Usuario | Integrante | Aporte |
| :--- | :--- | :--- |
| [Alva2105](https://github.com/Alva2105) | **Alvaro Del Carpio** | Contadores del HUD: puntaje flotante (100, 200, 1000, 1UP) desde enemigos, monedas, hongos y bandera; combo de pisotones como el original (100 → 8000 y 1UP); moneda animada del HUD con los tonos originales; pantalla "TIME UP" al acabarse el tiempo. Corrección de la importación de texturas y README con los aportes del equipo. En la actividad anterior: niveles 1-2, 2-2 y 2-4 y pantalla de Game Over. |
| [NazoVruytX](https://github.com/NazoVruytX) | **Carlos Valverde** | Mejora de SFX: salto distinto para Mario grande y pequeño, animaciones originales de crecer y encoger (powerup) con el juego congelado, sonido de daño con 2 segundos de invulnerabilidad, pausa antes del salto de muerte sincronizada con el jingle y pisotón que sube de tono con cada enemigo pisado seguido. Diseño del README. |
| [EduardoC2006](https://github.com/EduardoC2006) | **Eduardo Condoreno** | Escena de menú inicial como la pantalla de título original (cartel, cursor de honguito, TOP con el mejor puntaje): la opción 1 juega con Mario y la opción 2 con Luigi. Luigi como personaje jugable, golpe desde abajo que elimina a los enemigos sobre el bloque, estrella con el ciclo de colores original y corrección de la muerte por tiempo. |
| [MrPirate-29](https://github.com/MrPirate-29) | **Jhoel Herrera** | Mejora del sonido ambiente: música y efectos de todo el juego en formato .ogg, música apurada cuando quedan menos de 100 segundos y música de estrella mientras dura la estrella. |

---

## ✨ Mejoras de esta actividad (trabajo grupal)

### 🎬 Escena inicial de menú
* Pantalla de título como la original: cartel de **SUPER MARIO BROS.**, cursor de honguito y **TOP** con el mejor puntaje guardado.
* Interactuable con el teclado; cualquiera de las dos opciones inicia el juego: **1** con Mario y **2** con Luigi.

### 🔊 Sonido
* **Sonido ambiente:** toda la música y los efectos en formato .ogg. Cada zona tiene su música (exterior, subterráneo, agua y castillo); con menos de 100 segundos cambia a su versión apurada y con la estrella suena la música de estrella, como en el original.
* **SFX mejorados:**
  * **Salto:** sonido distinto para el salto de Mario pequeño y de Mario grande.
  * **Pisotón:** sube de tono con cada enemigo pisado seguido sin tocar el suelo.
  * **Powerup:** animación original de crecer (pequeño, intermedio y grande) con el juego congelado, como en el NES.
  * **Daño:** sonido propio, animación de encoger y 2 segundos de invulnerabilidad parpadeando.
  * **Muerte del personaje:** se queda quieto medio segundo y salta fuera de la pantalla al ritmo del jingle.

### 🔢 Contadores
* **Tiempo restante:** baja como en el original; al llegar a 0 el personaje muere y aparece la pantalla **TIME UP**. Al final del nivel el tiempo sobrante se suma al puntaje.
* **Puntaje:** se gana con las monedas y los enemigos eliminados; los puntos aparecen flotando donde se consiguieron. Pisar enemigos seguidos sin tocar el suelo vale 100, 200, 400, 500, 800, 1000, 2000, 4000, 5000, 8000 y luego 1UP.
* **Monedas:** contador con la moneda animada original del HUD; cada moneda vale 200 puntos y 100 monedas dan una vida.

---

## 🗺️ Mejoras anteriores

### Niveles
1. **Nivel subterráneo 1-2 (réplica del original):** reconstruido casilla por casilla desde el mapa original: entrada con castillo, escalones, cuarto secreto de monedas, plataformas móviles, zona de warp (*"WELCOME TO WARP ZONE!"*) y salida con bandera. Goombas, Koopas azules y Plantas Piraña.
2. **Nivel acuático 2-2 (réplica del original):** roca de coral, algas y superficie del agua. Mario nada (cada salto es una brazada) con su animación original, burbujas, Cheep Cheeps rojos y grises y Bloopers que lo persiguen.
3. **Castillo de Bowser 2-4 (diseño propio):** fosos de lava con Podoboos, barras de fuego, bloques flotantes, Bowser que salta y lanza fuego, puente que se derrumba al tocar el hacha y pantalla final.

### Sistema de juego y UI
* **Pantalla de Game Over:** réplica de la original (fondo negro, HUD y "GAME OVER" con su jingle). Al terminar vuelve al menú; manteniendo `Enter` se continúa en el mismo mundo.
* **HUD clásico** con fuente pixelada: puntaje, monedas, mundo, tiempo y vidas.
* **Pantalla de inicio de nivel**, pausa (`P` / `Esc`), ladrillos que se rompen, cámara suave con límites y zonas por área.
* Música y efectos de sonido en todo el juego.

---

## 🎮 Controles

| Acción | Tecla |
| :--- | :--- |
| **Elegir en el menú** | `W` / `S` o flechas arriba / abajo |
| **Confirmar (menú)** | `Enter` o `Espacio` |
| **Salir del juego (menú)** | `Esc` |
| **Moverse** | `A` / `D` o flechas izquierda / derecha |
| **Saltar / nadar** | `Espacio` |
| **Entrar a tubería (vertical)** | `S` o flecha abajo |
| **Entrar a tubería (lateral)** | `D` o flecha derecha |
| **Pausa** | `P` o `Esc` |

---

## 📝 Créditos

* **Assets gráficos:** sprites y mapas originales (Mario, Luigi, pantalla de título, HUD, enemigos y Bowser) de [The Spriters Resource - NES - Super Mario Bros.](https://www.spriters-resource.com/nes/supermariobros/)
* **Motor base:** código y assets iniciales del tutorial de [Zigurous - Unity Super Mario Tutorial](https://github.com/zigurous/unity-super-mario-tutorial).
* **Audio:** música y efectos de sonido en formato .ogg reunidos por el equipo; los sonidos de daño, puente, fin de nivel y la música del título son composiciones propias generadas para este proyecto.
* **Aviso legal:** *Super Mario Bros.* y todos sus personajes son propiedad de **Nintendo**. Este proyecto es solo con fines educativos y sin ánimo de lucro.
