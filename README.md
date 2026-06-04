# FactoryColony

FactoryColony is a PC single-player 2.5D top-view factory automation game built with Unity and C#.

## Current Direction
The factory is the primary game. Players must be able to progress through the main factory goals without expedition content. Expeditions are optional high-risk, high-reward activities that provide shortcuts, rare loot, cosmetics, side-grades, and combat-focused challenges.

## MVP Goals
- Base planet with grid-based factory placement
- Core production buildings such as miner, conveyor, smelter, storage, generator, research lab, and equipment workshop
- Deterministic resource and production chains
- Basic research progression
- Optional expedition planet content that supports, but does not gate, factory progression

## Development Principles
- Keep core simulation in pure C# where possible
- Use MonoBehaviour only for Unity binding, views, input, and scene lifecycle
- Prefer explicit ticks over core simulation in Update()
- Use data-driven definitions for static game content
- Avoid large god classes and unnecessary abstraction

## Environment
- Unity: TBD
- C#: TBD
- IDE: TBD
- Target platform: PC
