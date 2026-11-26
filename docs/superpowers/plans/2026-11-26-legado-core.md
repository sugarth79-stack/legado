# Legado.Core (reglas del juego) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Una librería C# sin dependencias de Unity que contiene las reglas numéricas del GDD, para que cliente, servidor de zona y herramientas usen exactamente las mismas reglas.

**Architecture:** Proyecto `Legado.Core` (netstandard2.1, C# 9, la versión que Unity consume) con un fichero por regla, agrupado por área. Proyecto de tests `Legado.Core.Tests` (net8.0, xUnit). Sin estado global: la aleatoriedad y el tiempo se inyectan (`IRandomSource`, `DateTimeOffset`) para que todo sea determinista en tests y autoritativo en servidor.

**Tech Stack:** .NET 8 SDK, netstandard2.1, xUnit, GitHub Actions.

**Spec:** `docs/superpowers/specs/2026-11-26-legado-design.md`

## Global Constraints

- `Legado.Core` apunta a `netstandard2.1` con `<LangVersion>9.0</LangVersion>`, sin paquetes NuGet externos.
- Identificadores de código en inglés; comentarios y mensajes de commit en español.
- Ningún texto visible para el jugador vive en esta librería (la localización ES/EN es del cliente).
- Nivel máximo: 80. Huecos de fruta: niveles 10, 30, 50, 70, 80 (máximo 5).
- Rareza por zona: Mar de facción 100 % común; La Garganta 80 % común / 20 % infrecuente; Aguas Bravas 80 % infrecuente / 20 % rara; Mar del Rey Ahogado 80 % rara / 20 % mítica.
- Cadáver PvE: 30 minutos. Penalización de renombre por muerte: 20 %.
- Ventana de asalto: 4 horas, todos los días, inicio elegido por el clan **en la zona horaria del clan** (sigue los cambios de horario de verano), siempre 4 horas reales también en los días de cambio de hora (decisión de Nacho del 26-11-2026, posterior a este plan).
- Reinicio semanal de mazmorras: **miércoles 04:00 UTC**. Reinicio diario (recálculo de rangos de renombre): **04:00 UTC**.
- Rangos de renombre por posición en el servidor: el número 1 siempre es dorado; además, dorado el 1 % superior, morado hasta el 10 %, azul hasta el 25 %, verde hasta el 50 % y gris el resto.
- Buzo con frutas: el oficio queda **congelado** (conserva su nivel, no se usa ni sube) y se reactiva al quedarse sin frutas.
- Tripulación máxima: 12. Clan máximo: 60. Grupo de mazmorra máximo: 6. Oficios por personaje: 3.
- Ganancia de profesión desconectado: 0,001 × ganancia activa.
- Si una regla necesita un valor que el GDD no fija, **no se inventa**: se pregunta a Nacho por medio del productor.

## Valores fijados por Nacho el 26-11-2026

Los cuatro huecos que tenía este plan ya están decididos e integrados en las tareas 4, 5, 7 y 8: reinicio semanal, zona horaria de la ventana, rangos de renombre y buzo con frutas.

## Review Focus

1. Niveles fuera de rango (0, -1, 81): se espera `ArgumentOutOfRangeException`, nunca 0 huecos silenciosos → tarea 1.
2. Tirada de rareza en el borde (79 frente a 80) y tirada fuera de 0-99 → tarea 2.
3. Ventana de asalto que cruza la medianoche (inicio 22:00 → fin 02:00) y cambio al horario de verano → tarea 5.
4. Jugador que entra en una tripulación de 12 sin ningún NPC que sustituir → tarea 6.
5. Naufragio de un personaje desconectado: pierde también la fruta → tarea 3.

---

### Task 1: Solución, proyectos y huecos de fruta

**Files:**
- Create: `Legado.sln`, `src/Legado.Core/Legado.Core.csproj`, `src/Legado.Core/Progression/FruitSlots.cs`
- Create: `tests/Legado.Core.Tests/Legado.Core.Tests.csproj`, `tests/Legado.Core.Tests/Progression/FruitSlotsTests.cs`
- Create: `.gitignore` (plantilla `dotnet new gitignore`)

**Interfaces:**
- Produces: `public static class GameLimits { public const int MaxLevel = 80; }` en `src/Legado.Core/GameLimits.cs`; `public static int FruitSlots.ForLevel(int level)`.

- [ ] **Step 1: Crear la solución**: `dotnet new sln -n Legado`, `dotnet new classlib -o src/Legado.Core -f netstandard2.1`, `dotnet new xunit -o tests/Legado.Core.Tests -f net8.0`, añadir ambos a la solución y la referencia del test a Core. Fijar `<LangVersion>9.0</LangVersion>` y `<Nullable>enable</Nullable>` en Core. Borrar `Class1.cs` y `UnitTest1.cs`.
- [ ] **Step 2: Escribir los tests que fallan**

```csharp
[Theory]
[InlineData(1, 0)] [InlineData(9, 0)] [InlineData(10, 1)] [InlineData(29, 1)]
[InlineData(30, 2)] [InlineData(50, 3)] [InlineData(69, 3)] [InlineData(70, 4)]
[InlineData(79, 4)] [InlineData(80, 5)]
public void ForLevel_returns_slots_unlocked(int level, int expected)
    => Assert.Equal(expected, FruitSlots.ForLevel(level));

[Theory] [InlineData(0)] [InlineData(-1)] [InlineData(81)]
public void ForLevel_rejects_out_of_range(int level)
    => Assert.Throws<ArgumentOutOfRangeException>(() => FruitSlots.ForLevel(level));
```

- [ ] **Step 3: Ejecutar y ver que fallan**: `dotnet test` → FAIL de compilación (`FruitSlots` no existe).
- [ ] **Step 4: Implementar** `GameLimits` y `FruitSlots.ForLevel(int level)`, con los umbrales `{10, 30, 50, 70, 80}` como `static readonly int[]`.
- [ ] **Step 5: Ejecutar**: `dotnet test` → PASS (13 tests).
- [ ] **Step 6: Commit**: `git commit -m "feat(core): huecos de fruta por nivel"`

### Task 2: Rareza de fruta por zona

**Files:**
- Create: `src/Legado.Core/World/Zone.cs`, `src/Legado.Core/Fruits/Rarity.cs`, `src/Legado.Core/Fruits/IRandomSource.cs`, `src/Legado.Core/Fruits/RarityTable.cs`
- Test: `tests/Legado.Core.Tests/Fruits/RarityTableTests.cs`, `tests/Legado.Core.Tests/FixedRandom.cs`

**Interfaces:**
- Produces: `enum Zone { FactionSea, Gullet, WildWaters, DrownedKingsSea }`; `enum Rarity { Common, Uncommon, Rare, Mythic }`; `interface IRandomSource { int NextPercent(); }` (0-99 inclusive); `static Rarity RarityTable.Roll(Zone zone, IRandomSource rng)`. Test helper: `FixedRandom(int value) : IRandomSource`.

- [ ] **Step 1: Tests que fallan**

```csharp
[Theory]
[InlineData(Zone.FactionSea, 0, Rarity.Common)]   [InlineData(Zone.FactionSea, 99, Rarity.Common)]
[InlineData(Zone.Gullet, 79, Rarity.Common)]       [InlineData(Zone.Gullet, 80, Rarity.Uncommon)]
[InlineData(Zone.WildWaters, 79, Rarity.Uncommon)] [InlineData(Zone.WildWaters, 80, Rarity.Rare)]
[InlineData(Zone.DrownedKingsSea, 79, Rarity.Rare)][InlineData(Zone.DrownedKingsSea, 99, Rarity.Mythic)]
public void Roll_maps_percent_to_rarity(Zone zone, int roll, Rarity expected)
    => Assert.Equal(expected, RarityTable.Roll(zone, new FixedRandom(roll)));

[Theory] [InlineData(-1)] [InlineData(100)]
public void Roll_rejects_percent_outside_0_99(int roll)
    => Assert.Throws<ArgumentOutOfRangeException>(() => RarityTable.Roll(Zone.Gullet, new FixedRandom(roll)));
```

- [ ] **Step 2: Ejecutar**: `dotnet test --filter RarityTable` → FAIL.
- [ ] **Step 3: Implementar** `RarityTable.Roll`: cada zona tiene `(Rarity low, Rarity high, int highFromPercent)`, con `FactionSea` = `(Common, Common, 100)` y el resto `highFromPercent = 80`.
- [ ] **Step 4: Ejecutar**: `dotnet test` → PASS.
- [ ] **Step 5: Commit**: `git commit -m "feat(core): tabla de rareza de frutas por zona"`

### Task 3: Reglas de muerte, saqueo y cadáver

**Files:**
- Create: `src/Legado.Core/Combat/DeathContext.cs`, `src/Legado.Core/Combat/DeathOutcome.cs`, `src/Legado.Core/Combat/DeathRules.cs`
- Test: `tests/Legado.Core.Tests/Combat/DeathRulesTests.cs`

**Interfaces:**
- Produces: `enum DeathCause { Monster, Player, Shipwreck }`; `record DeathContext(DeathCause Cause, bool VictimOnline, bool InPvpZone)`; `record DeathOutcome(bool LosesCarriedItems, bool LosesFruits, bool LootableByAnyone, bool KillerLoots, TimeSpan? CorpseLifetime)`; `static DeathOutcome DeathRules.Resolve(DeathContext ctx)`; `static readonly TimeSpan DeathRules.PveCorpseLifetime = TimeSpan.FromMinutes(30)`.

- [ ] **Step 1: Tests que fallan**

```csharp
[Fact] public void Pvp_online_loses_items_and_fruits_killer_loots() {
    var o = DeathRules.Resolve(new(DeathCause.Player, VictimOnline: true, InPvpZone: true));
    Assert.True(o.LosesCarriedItems); Assert.True(o.LosesFruits); Assert.True(o.KillerLoots); }
[Fact] public void Pvp_offline_guard_keeps_fruits() {
    var o = DeathRules.Resolve(new(DeathCause.Player, VictimOnline: false, InPvpZone: true));
    Assert.True(o.LosesCarriedItems); Assert.False(o.LosesFruits); }
[Fact] public void Pve_keeps_fruits_corpse_lasts_30_minutes() {
    var o = DeathRules.Resolve(new(DeathCause.Monster, true, InPvpZone: false));
    Assert.True(o.LosesCarriedItems); Assert.False(o.LosesFruits);
    Assert.False(o.LootableByAnyone); Assert.Equal(TimeSpan.FromMinutes(30), o.CorpseLifetime); }
[Fact] public void Pve_in_pvp_zone_corpse_lootable_by_anyone() =>
    Assert.True(DeathRules.Resolve(new(DeathCause.Monster, true, InPvpZone: true)).LootableByAnyone);
[Theory] [InlineData(true)] [InlineData(false)]
public void Shipwreck_loses_everything_including_fruits(bool online) {
    var o = DeathRules.Resolve(new(DeathCause.Shipwreck, online, InPvpZone: true));
    Assert.True(o.LosesCarriedItems); Assert.True(o.LosesFruits); }
```

- [ ] **Step 2: Ejecutar** → FAIL.
- [ ] **Step 3: Implementar `DeathRules.Resolve`**: `LosesCarriedItems` siempre es true. `LosesFruits` = (Player y online) o Shipwreck. `LootableByAnyone` = `InPvpZone`. `KillerLoots` = Player. `CorpseLifetime` = 30 min salvo en Shipwreck, que es null porque no hay cadáver recuperable.
- [ ] **Step 4: Ejecutar** → PASS.
- [ ] **Step 5: Commit**: `git commit -m "feat(core): reglas de muerte, saqueo y cadáver"`

### Task 4: Renombre

**Files:**
- Create: `src/Legado.Core/Renown/RenownRules.cs`
- Test: `tests/Legado.Core.Tests/Renown/RenownRulesTests.cs`

**Interfaces:**
- Produces: `static long RenownRules.AfterDeath(long renown)` (resta el 20 % y redondea hacia abajo). `enum RenownRank { Gray = 1, Green, Blue, Purple, Gold }`. `static RenownRank RenownRules.Rank(int position, int population)`: `position` 1 = más renombre del servidor; el porcentaje superior es `position / population` y da Gold si ≤ 1 %, Purple si ≤ 10 %, Blue si ≤ 25 %, Green si ≤ 50 % y Gray en el resto.

- [ ] **Step 1: Tests que fallan**:
  - `AfterDeath(1000) == 800`, `AfterDeath(999) == 799`, `AfterDeath(0) == 0`; `AfterDeath(-1)` lanza `ArgumentOutOfRangeException`.
  - Con `population = 1000`: posición 1 y 10 dan Gold; 11 y 100 dan Purple; 101 y 250 dan Blue; 251 y 500 dan Green; 501 y 1000 dan Gray.
  - `Rank(0, 10)`, `Rank(11, 10)` y `Rank(1, 0)` lanzan `ArgumentOutOfRangeException`.
- [ ] **Step 2: Ejecutar** → FAIL.
- [ ] **Step 3: Implementar** con aritmética entera: `renown - renown / 5` no sirve para 999, así que usar `renown * 4 / 5`.
- [ ] **Step 4: Ejecutar** → PASS.
- [ ] **Step 5: Commit**: `git commit -m "feat(core): penalización y rangos de renombre"`

### Task 5: Ventana de asalto

**Files:**
- Create: `src/Legado.Core/Bases/RaidWindow.cs`
- Test: `tests/Legado.Core.Tests/Bases/RaidWindowTests.cs`

**Interfaces:**
- Produces: `sealed class RaidWindow { RaidWindow(TimeSpan localStart, TimeZoneInfo clanTimeZone); static readonly TimeSpan Duration = TimeSpan.FromHours(4); bool IsOpen(DateTimeOffset now); }`. `localStart` es la hora de inicio en la zona horaria del clan, entre 00:00 inclusive y 24:00 exclusive; fuera de ese rango lanza `ArgumentOutOfRangeException`.

- [ ] **Step 1: Tests que fallan** (zona `TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid")`):
  - Inicio 20:00 en invierno: abierta a las `2026-01-15T19:00Z` (20:00 local) y a las `22:59Z`; cerrada a las `23:00Z` (24:00 local).
  - Inicio 20:00 en verano: abierta a las `2026-07-15T18:00Z` (20:00 local, UTC+2) y cerrada a las `17:59Z`. La ventana sigue la hora local del clan.
  - Inicio 22:00 (cruza la medianoche): abierta a las 23:30 y a la 01:59 locales del día siguiente; cerrada a las 02:00 y a las 21:00 locales.
  - Un `now` con cualquier offset se evalúa por su instante.
  - `new RaidWindow(TimeSpan.FromHours(24), madrid)` lanza.
- [ ] **Step 2: Ejecutar** → FAIL.
- [ ] **Step 3: Implementar `IsOpen`**: `var t = TimeZoneInfo.ConvertTime(now, clanTimeZone).TimeOfDay`; el final es `(localStart + Duration)` módulo 24 h. Está abierta si `start <= t < end` o, cuando cruza la medianoche, si `t >= start || t < end`.
- [ ] **Step 4: Ejecutar** → PASS.
- [ ] **Step 5: Commit**: `git commit -m "feat(core): ventana diaria de asalto de 4 horas"`

### Task 6: Tripulación y clan

**Files:**
- Create: `src/Legado.Core/Crews/CrewMember.cs`, `src/Legado.Core/Crews/Crew.cs`, `src/Legado.Core/Crews/Clan.cs`
- Test: `tests/Legado.Core.Tests/Crews/CrewTests.cs`, `tests/Legado.Core.Tests/Crews/ClanTests.cs`

**Interfaces:**
- Produces: `abstract record CrewMember(string Id)`, `record PlayerMember(string Id) : CrewMember(Id)`, `record NpcMember(string Id) : CrewMember(Id)`. `sealed class Crew { const int MaxCapacity = 12; Crew(int capacity); IReadOnlyList<CrewMember> Members; bool TryAddNpc(NpcMember); JoinResult AddPlayer(PlayerMember); }` con `enum JoinResult { Joined, ReplacedNpc, Full }`. `sealed class Clan { const int MaxMembers = 60; bool TryAdd(string playerId); int Count; }`.

- [ ] **Step 1: Tests que fallan**:
  - `new Crew(13)` y `new Crew(0)` lanzan.
  - Una tripulación de 3 plazas con 3 NPC: `AddPlayer` devuelve `ReplacedNpc`, sigue habiendo 3 miembros y 1 es jugador.
  - Una tripulación de 12 plazas con 12 jugadores: el jugador 13 devuelve `Full`.
  - Con hueco libre: devuelve `Joined`.
  - `TryAddNpc` devuelve false si está llena.
  - `Clan`: el jugador 61 devuelve false y un id repetido devuelve false.
- [ ] **Step 2: Ejecutar** → FAIL.
- [ ] **Step 3: Implementar**: si la tripulación está llena, `AddPlayer` sustituye al último NPC añadido. Es una decisión de implementación: el GDD solo dice que "sale un NPC".
- [ ] **Step 4: Ejecutar** → PASS.
- [ ] **Step 5: Commit**: `git commit -m "feat(core): plazas de tripulación y límite de clan"`

### Task 7: Bloqueo semanal y acceso a mazmorras

**Files:**
- Create: `src/Legado.Core/Dungeons/Difficulty.cs`, `src/Legado.Core/Dungeons/DungeonRules.cs`, `src/Legado.Core/Dungeons/WeeklyLockout.cs`
- Test: `tests/Legado.Core.Tests/Dungeons/DungeonRulesTests.cs`, `tests/Legado.Core.Tests/Dungeons/WeeklyLockoutTests.cs`

**Interfaces:**
- Produces: `enum Difficulty { Basic, Normal, Max }`. `static bool DungeonRules.CanEnter(Difficulty d, int players, int npcs)`: un grupo tiene entre 1 y 6 miembros; Max exige `npcs == 0`. `static bool DungeonRules.CanDropFruit(bool storyDungeon, Difficulty d)`: true si es de historia o si la dificultad es Max. `static class Resets { const DayOfWeek WeeklyDay = DayOfWeek.Wednesday; static readonly TimeSpan TimeUtc = TimeSpan.FromHours(4); }` (el diario y el semanal a las 04:00 UTC). `sealed class WeeklyLockout { WeeklyLockout(); void RecordCompletion(string dungeonId, DateTimeOffset at); bool IsLocked(string dungeonId, DateTimeOffset now); }`, que usa `Resets`.

- [ ] **Step 1: Tests que fallan**:
  - `CanEnter(Max, 5, 1)` es false.
  - `CanEnter(Max, 6, 0)` es true.
  - `CanEnter(Basic, 1, 5)` es true.
  - `CanEnter(Basic, 4, 3)` es false (son 7 miembros).
  - `CanEnter(Basic, 0, 0)` es false.
  - `CanDropFruit(false, Normal)` es false y `CanDropFruit(true, Basic)` es true.
  - Completada el lunes `2026-11-30T20:00Z`: bloqueada el miércoles `2026-12-02T03:59Z` y libre a las `04:00Z`.
  - Completada el miércoles `2026-12-02T04:00Z`, justo en el reinicio: bloqueada hasta el miércoles siguiente a las 04:00Z.
  - Completada en Basic, también bloquea Max, porque el bloqueo es por mazmorra y no por dificultad.
- [ ] **Step 2: Ejecutar** → FAIL.
- [ ] **Step 3: Implementar `IsLocked`**: calcular el primer reinicio (miércoles 04:00 UTC) estrictamente posterior a la última finalización y comparar con `now`.
- [ ] **Step 4: Ejecutar** → PASS.
- [ ] **Step 5: Commit**: `git commit -m "feat(core): acceso a mazmorras y bloqueo semanal"`

### Task 8: Oficios

**Files:**
- Create: `src/Legado.Core/Professions/Profession.cs`, `src/Legado.Core/Professions/ProfessionRules.cs`
- Test: `tests/Legado.Core.Tests/Professions/ProfessionRulesTests.cs`

**Interfaces:**
- Produces: `enum Profession` con los 21 valores del GDD, §11: `Lumberjack, Miner, Fisher, Hunter, Herbalist, Diver, Blacksmith, Weaponsmith, LeatherAndCloth, Jeweler, Cook, Chemist, Medic, Mechanic, Shipwright, Architect, Mason, Cartographer, TreasureHunter, Navigator, Tamer`. `const int ProfessionRules.MaxPerCharacter = 3`. `static bool ProfessionRules.IsValidCombination(IReadOnlyCollection<Profession> ps)` (como mucho 3 y sin repetidos). `static bool ProfessionRules.IsActive(Profession p, int fruitsEaten)`: Diver está inactivo (congelado) si `fruitsEaten > 0`; el resto siempre está activo. `static bool ProfessionRules.CanGainSkill(Profession p, int fruitsEaten)`, igual que `IsActive`. `const double ProfessionRules.OfflineGainFactor = 0.001`. `static double ProfessionRules.OfflineGain(double activeGain)`.

- [ ] **Step 1: Tests que fallan**:
  - `Enum.GetValues<Profession>().Length == 21`.
  - Tres oficios distintos son válidos; cuatro no; uno repetido no.
  - `[Diver, Fisher, Cook]` sigue siendo una combinación válida aunque el personaje tenga frutas: el hueco sigue ocupado.
  - `IsActive(Diver, 0)` es true; `IsActive(Diver, 1)` es false; `IsActive(Fisher, 3)` es true.
  - Reactivación: `IsActive(Diver, 0)` vuelve a ser true después de pasar de 2 frutas a 0.
  - `CanGainSkill(Diver, 1)` es false.
  - `OfflineGain(1000) == 1.0`, con tolerancia de 1e-9.
- [ ] **Step 2: Ejecutar** → FAIL.
- [ ] **Step 3: Implementar.** El nivel del oficio congelado no se toca: el congelado es solo el resultado de `IsActive` y `CanGainSkill`. El aviso antes de comer fruta es del cliente y está localizado.
- [ ] **Step 4: Ejecutar** → PASS.
- [ ] **Step 5: Commit**: `git commit -m "feat(core): límite de oficios y ganancia desconectado"`

### Task 9: Integración continua

**Files:**
- Create: `.github/workflows/core-tests.yml`

- [ ] **Step 1: Escribir el workflow**: en cada `push` y `pull_request`, usar `ubuntu-latest`, `actions/setup-dotnet@v4` con `dotnet-version: 8.0.x` y ejecutar `dotnet test Legado.sln --configuration Release`.
- [ ] **Step 2: Verificar en local**: `dotnet test Legado.sln -c Release` → todos los tests en PASS, sin avisos de nulabilidad.
- [ ] **Step 3: Commit**: `git commit -m "ci: tests de Legado.Core en cada push"`
