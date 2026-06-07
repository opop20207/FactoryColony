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

## Debug Scene
Open `Assets/_Project/Scenes/BasePlanet/FactoryDebug.unity` in Unity and press Play to view the first debug grid.

Current visualization:
- 10x10 grid on the XZ plane
- resource nodes at `(1,1)` IronOre, `(3,2)` CopperOre, and `(5,5)` Coal
- orthographic camera setup for a top/quarter-view look
- primitive-based building debug views for Miner, Conveyor, Smelter, Storage, Generator, and Assembler
- simple direction indicators above buildings
- runtime simulation ticks through the debug production line after pressing Play
- OnGUI debug HUD with tick count, power state, and aggregated Storage resources
- WASD or arrow-key camera movement
- mouse-wheel orthographic zoom
- hovered grid-cell highlight with the current cell shown in the HUD
- number keys 1-6 select a debug building preview
- R rotates the selected preview when the building is rotatable
- placement preview uses green for valid placement and red for invalid placement

Buildings are temporary primitive placeholders. The simulation is quantity-based only: actual click-to-build placement, construction costs, selection UI, building inventory labels, and item movement animation are not visualized yet.
