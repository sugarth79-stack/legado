# GDD · El Legado del Rey Ahogado

Copia en el repositorio del documento de diseño aprobado por Nacho (26-11-2026).
Original editable: https://claude.ai/artifact/VEytX5S27kExsqJZCmNDTR
Si hay discrepancia, manda el original. Ningún agente cambia este fichero sin aprobación de Nacho.

## 1. Visión y alcance

MMO de mundo abierto de piratas, en un mundo original inspirado en One Piece, cuyo objetivo final es encontrar El Legado del Rey Ahogado.

- **Beta**: servidor privado con amigos.
- **Objetivo**: abrirlo al público si funciona. Desde el inicio: servidor autoritativo, cuentas reales, zonas que escalan por separado.
- **Bucle**: crear pirata, subir de nivel, mejorar barco y tripulación, mazmorras para equipo y frutas, historia de facción, carrera por el Legado.
- **No objetivos**: gráficos realistas; usar nombres, personajes o arcos de One Piece.
- **Antiobjetivos**: que el que paga gane; que el jugador sin clan quede fuera del contenido clave.

## 2. Decisiones técnicas

| Aspecto | Decisión |
| --- | --- |
| Motor | Unity (C#) |
| Estilo | Low-poly, estilo indie |
| Backend | Nakama: cuentas, inventarios, chat, clanes, mercado, clasificaciones, matchmaking |
| Servidores de zona | Unity headless autoritativo en Amazon GameLift; cada isla y zona de mar es un servidor |
| Infraestructura | Nakama en ECS o EKS + Aurora PostgreSQL |
| Plataformas | PC (Windows), Mac, Linux |
| Idiomas | Español e inglés desde el inicio |

Descartados: Unreal + backend propio; Godot + Nakama.

## 3. Mundo, facciones e historia

| Etapa | Zona (ES / EN) | Nivel | Qué hay |
| --- | --- | --- | --- |
| Mar de facción | Mar de Brea / Tar Sea (piratas) · Mar del Ancla / Anchor Sea (marines) · Mar de Ceniza / Ash Sea (cazarrecompensas) | 1-30 | Inicio, misiones pequeñas, mazmorras de historia |
| Ruta peligrosa | La Garganta / The Gullet | 30-50 | Las facciones se cruzan |
| Nuevo mundo | Aguas Bravas / Wild Waters | 50-70 | Final de la historia (deja entre nivel 70 y 80) |
| Cuarto mar | Mar del Rey Ahogado / Drowned King's Sea | 70-80 | Todo PvP salvo Liberty e islas de casas; bases; mazmorras de mayor nivel; eventos de nivel 80; el Legado |

Cuarto mar: **Liberty** (puerto seguro: mercado común, NPC vendedores, fabricación comunitaria); **una isla por facción** para casas individuales (segura); **islas libres** para fortalezas de clan.

Facciones jugables: pirata (arco "La Carta del Rey Ahogado"), marine ("El Juramento Roto", final con decisión sellar/entregar), cazarrecompensas ("La Última Recompensa"). Historia en misiones pequeñas, un arco por etapa.

### El Legado del Rey Ahogado (The Drowned King's Legacy)

Temporadas de **3 meses**. Completar la historia = trofeo personal. Victoria de temporada:

1. **Carrera de la carta**: el Legado aparece en un punto oculto del cuarto mar; fragmentos de la carta caen en mazmorras de mayor nivel, eventos de nivel 80 y tesoros enterrados; se roban en PvP y se venden. La tripulación que completa la carta y derrota al guardián gana la temporada (título permanente, estatua, recompensas únicas, leyenda).
2. **Asedio final**: la ubicación se vuelve isla fortificada con jefe final; el clan que lo derrota gana puntos extra.

Clasificación de clanes: puntos por renombre de miembros, fortalezas controladas y asedios; premios estéticos al final.
Reinicio de temporada: fragmentos, puntos de clan, isla del asedio, ubicación del Legado. Se conservan personajes, equipo, frutas, bases y renombre personal.

## 4. Personaje

Raza, clase y rol se eligen al crear. Nivel máximo **80**. Cualquier clase puede ser cualquier rol (tanque, sanador, daño).

Razas (9): Humano (sube profesiones más rápido), Bestiano (agilidad, olfato; animal estético), Diminuto (rápido, sigilo; menos vida), Brazolargo (alcance cuerpo a cuerpo), Zancudo (velocidad, patadas), Trioculado (ve ocultos y trampas, puntería), Alado (planea, resiste fuego; no vuela), Cornudo (fuerza, resistencia; menos agilidad), Cuellilargo (visión, esquiva golpes altos). Sin gigantes ni hombres pez.

Clases (11): Espadachín, Luchador, Pateador, Tirador, Artillero, Inventor, Tormentista, Médico, Bardo, Lancero, Ninja. 33 variantes clase×rol.

## 5. Combate, PvP y muerte

- Combate estilo WoW: fijar objetivo, habilidades con recarga.
- Zonas seguras: ciudades/puertos principales, islas de inicio de nivel bajo; en el cuarto mar solo Liberty e islas de casas. Resto PvP abierto, también entre miembros de la misma facción.
- Al morir (PvP o PvE) se pierde todo lo que se lleva encima, equipo puesto incluido.
- PvP: el que mata saquea. PvE: el botín queda en el cadáver; reaparición en el punto más cercano.
- En zona PvP cualquiera saquea cualquier cadáver. Cadáver: **30 minutos** máximo.
- Personaje desconectado vigilando: muere igual y pierde lo que lleva.

Renombre: suma por mazmorras (difíciles más), tiempo sin morir y kills PvP. Recompensa por cabeza según renombre, la cobra quien mata, en doblones. Cada muerte resta **20 %** del renombre. Cartel "Se busca" + tablón en Liberty. **Cinco rangos** con icono de color (gris a dorado).

## 6. Frutas del diablo

- No únicas. Tipos: animal, elemental, especial. Rarezas: común, infrecuente, rara, mítica (fija, nunca sube).
- Más rareza = más daño del portador y más habilidades.
- Caen en mazmorras de historia (la primera, hacia nivel 10, en el mar de facción) y en dificultad máxima de mazmorras del mundo; además por las 4 vías sin clan.
- Acumulables con poderes simultáneos. Huecos en niveles **10, 30, 50, 70, 80** (máximo 5).
- Se venden en el mercado.
- Se pierden solo al morir en PvP estando conectado (desaparece, no se saquea). PvE o desconectado: se conserva.

| Zona del jefe | Nivel | Probabilidades |
| --- | --- | --- |
| Mar de facción | 1-30 | 100 % común |
| La Garganta | 30-50 | 80 % común, 20 % infrecuente |
| Aguas Bravas | 50-70 | 80 % infrecuente, 20 % rara |
| Mar del Rey Ahogado | 70-80 | 80 % rara, 20 % mítica |

Desventajas: debilidad al agua (no nada, se hunde, pierde poder mojado); naufragio = pierde todo incluida la fruta; hierro abisal anula poderes; no puede ser buzo y pesca peor; desgaste (más hambre/cansancio).

## 7. Tripulación, clan y NPC

- Clan ≠ tripulación. Clan máximo **60** jugadores.
- Plazas fijas; para entrar un jugador sale un NPC. Barco más grande: **12** tripulantes.
- NPC aportan: combate (tanque/sanador/daño), puesto en barco (navegante, cañonero, carpintero), pasivas de oficio (cocinero, médico), tareas en segundo plano. Rinden menos que jugadores.
- Desconectado: el personaje sigue con su tarea asignada; sube profesión a **0,001** del ritmo activo; puede morir en asalto; conserva frutas.

## 8. Barco y navegación

Mejoras: resistencia, cañones, poder ofensivo, tamaño (más plazas, máx. 12). Viaje con espera según distancia. Combate naval: cañones, abordajes, hundimientos (= naufragio). Encuentros en ruta (zona de mar en tiempo real) y aguas de combate alrededor de cada isla.

## 9. Mazmorras y jefes

Mazmorras de historia (dentro de la aventura) y del mundo (fuera, 3 dificultades). Todas con nivel. Grupos de hasta **6**.

| Dificultad | Quién entra | Botín |
| --- | --- | --- |
| Básica | Jugadores y/o NPC | Equipo, fragmentos de fruta |
| Media | Jugadores y/o NPC | Equipo mejor, fragmentos de fruta |
| Máxima | Solo grupos de jugadores | Mejor equipo + frutas |

Cada mazmorra se completa **una vez por semana** en cualquier dificultad. Jefes originales inspirados en One Piece. Vías sin clan: buscador de grupo; fragmentos que combina el químico (solo común o infrecuente); tesoros enterrados (fruta común, muy rara); jefes de mundo (fruta a quien más contribuye).

## 10. Bases y asaltos

Todo en el Mar del Rey Ahogado: casas en la isla de facción (segura), fortalezas de clan en islas libres. Ventanas de asalto de **4 horas**, **todos los días**, franja elegida por el clan. Defienden los desconectados asignados. Hierro abisal en muros.

## 11. Oficios, monedas y mercado

**Tres oficios** por personaje. 21 oficios:
Recolección: leñador, minero (incl. hierro abisal), pescador, cazador, herborista, buzo.
Fabricación: herrero (armaduras metálicas, herramientas, lingotes), armero (armas), cuero y telas (armaduras ligeras, ropa, velas), joyero, cocinero, químico (pociones, explosivos, combinar fragmentos), médico (medicinas y vendas de combate), mecánico.
Construcción: constructor de barcos, arquitecto (planos), albañil (levanta y repara).
Exploración: creador de mapas, rastreador de tesoros, navegante (menos tiempo de viaje, clima), domador.
Descartados: músico, comerciante. Cazarrecompensas es facción.

Monedas: **Doblones / Doubloons** (solo jugando) y **Perlas / Pearls** (dinero real, solo tienda de aspectos). Mercado global.

## 12. Trofeos y monetización

Trofeos por historia, jefes, oficios, PvP y exploración: prestigio, títulos, aspectos, pequeñas bonificaciones. Free-to-play con tienda de aspectos (perlas). No se puede pagar por ventajas.
