# El Legado del Rey Ahogado

MMO de piratas low-poly en un mundo original inspirado en One Piece. Diseño de Nacho; desarrollo con el equipo de agentes de `.claude/agents/`.

## Documentos
- GDD (fuente de verdad): `docs/superpowers/specs/2026-11-26-legado-design.md`
- Hoja de ruta: `docs/superpowers/plans/2026-11-26-roadmap.md`
- Plan activo: `docs/superpowers/plans/2026-11-26-legado-core.md` (subproyecto 0)

## Reglas del proyecto
- Nadie inventa diseño. Si falta un dato en el GDD, se pregunta a Nacho por medio del agente `productor`.
- Nada de nombres, personajes ni textos de One Piece.
- Las reglas de juego viven en `src/Legado.Core` y se comparten entre cliente y servidor; nunca se duplican.
- El servidor es autoritativo. Las Perlas (moneda de pago) solo compran estética.
- Todo texto visible se localiza en español e inglés.
- TDD y commits pequeños. Cada tarea la aprueba `qa-revisor` antes de seguir.

## Pila técnica
Unity (C#) · Nakama · Amazon GameLift · ECS/EKS + Aurora PostgreSQL · Terraform · GitHub Actions.
