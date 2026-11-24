# Unity Pathfinding: Stealth AI Prototype

Prototipo desarrollado en **Unity 6 (6000.6.3f1)** y **C#** para el taller de resolución de problemas de navegación autónoma (*Pathfinding*).

## Demostración en Video
* **Video explicativo (YouTube / Loom):** [Insertar enlace aquí una vez subido]

## Conceptos y Algoritmos Implementados
* **Algoritmo A\* (A-Star) y Funnel Algorithm:** Utilizados internamente por el `NavMeshAgent` sobre una malla de polígonos convexos (`NavMeshSurface`) para calcular la ruta óptima $f(n) = g(n) + h(n)$ y suavizar las trayectorias en tiempo real.
* **Máquina de Estados Finitos (`GuardiaIA.cs`):**
  * **Patrullando:** Recorrido cíclico entre cuatro puntos de control (`WP_1` a `WP_4`).
  * **Persiguiendo:** Detección sensorial mediante cono de visión y `Physics.Raycast` (evitando detección a través de muros).
  * **Investigando:** Búsqueda activa en la última posición conocida del jugador antes de retomar la ronda.
* **Obstáculos Dinámicos (`NavMeshObstacle` + Carving):** Compuerta móvil (`PuertaMovil.cs`) que perfora el NavMesh en tiempo real al presionar `Espacio`, obligando al agente a recalcular su ruta al instante.
* **Conexión de Plataformas (`NavMeshLink`):** Enlace bidireccional que permite al agente saltar hacia la plataforma elevada durante su patrulla o persecución.

## Controles del Prototipo
* **W, A, S, D / Flechas:** Mover al jugador (`JugadorController.cs`).
* **E / Shift Izquierdo:** Saltar hacia la plataforma elevada.
* **Espacio:** Abrir/cerrar la puerta dinámica amarilla (`PuertaMovil.cs`).
