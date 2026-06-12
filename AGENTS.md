# Project Instructions

## Project Summary
This is a PC single-player 2.5D factory automation and planetary expedition game made with Unity and C#.

The game is inspired by Satisfactory for factory automation and resource progression, Thronefall for simplified 2.5D low-poly visual direction, and extraction games for short high-risk expedition loops.

Previous roguelike puzzle concepts are discarded. Do not implement roguelike puzzle mechanics.

## Core Game Identity
The main game is a factory automation game.
The player builds a base on an alien planet, mines resources, processes materials, researches technology, crafts weapons and expedition equipment, and may optionally send the player or units to dangerous planets to recover rare resources and unique items.

Factory gameplay is the primary experience and must stand alone.
Expedition gameplay is optional high-risk, high-reward content.

Do not design expedition as a mandatory progression gate.

Target ratio:
- Factory gameplay: 70%
- Expedition gameplay: 30%

## Updated Core Direction
This is primarily a factory automation game.

The player must be able to enjoy and complete the main factory progression without doing expedition content.

Expedition gameplay is optional high-risk, high-reward content.

Do not design expedition as a mandatory progression gate.

## Progression Rule
Factory-only players must be able to:
- progress through the main technology tiers
- automate resources
- build defenses
- craft standard weapons
- complete major factory goals
- reach the main endgame objective

Expedition players can gain:
- faster access to advanced materials
- rare loot
- special weapons
- cosmetic items
- unique companion unit variants
- defense-oriented upgrades
- optional side-grade technologies

## Reward Design Rule
Expedition rewards should be attractive but not mandatory.

Allowed expedition reward types:
- rare material bundles
- advanced components
- alternative crafting ingredients
- special weapon variants
- cosmetic building skins
- player suit skins
- companion unit skins
- defensive side-grade modules
- high-risk crafting shortcuts
- optional lore artifacts

Avoid expedition-exclusive rewards that are required for:
- main factory progression
- basic automation
- core power generation
- main ending condition
- essential defense systems

## Factory Priority
Factory gameplay should stand alone.

Before implementing expedition systems, make sure factory gameplay has:
- satisfying resource extraction
- deterministic production chains
- meaningful logistics
- power management
- research progression
- defense preparation
- scalable production goals

## Expedition Purpose
Expedition exists to support these player motivations:
1. Faster growth
2. Rare item acquisition
3. Combat-focused gameplay
4. Cosmetic collection
5. Stronger defense preparation
6. Optional challenge

Expedition should never feel like required homework for factory-focused players.

## Visual Direction
- 2.5D low-poly style
- Quarter-view camera
- Clean silhouettes
- Readable buildings
- Simplified materials
- Strong color palettes per planet
- Do not pursue realistic graphics
- Do not implement first-person gameplay

## Technical Stack
- Engine: Unity
- Language: C#
- Target platform: PC
- IDE: Rider or Visual Studio
- Architecture: Data-driven, simulation-first

## Architecture Rules
- Core simulation must be written in pure C# where possible.
- MonoBehaviour should be used for Unity binding, input, views, and scene lifecycle.
- Do not put core resource simulation directly inside UI classes.
- Do not create god classes.
- Each system must have a clear responsibility.
- Use ScriptableObjects for static data such as buildings, resources, recipes, research nodes, equipment, and planet definitions.
- Keep factory simulation deterministic.
- Avoid Update() for core simulation logic.
- Prefer explicit Tick(), SimulateStep(), or SimulateTurn() methods.

## Core Systems
The project should be organized around these systems:

1. World System
- Planet maps
- Terrain tiles
- Resource nodes
- Buildable areas

2. Factory System
- Buildings
- Production recipes
- Conveyor/resource movement
- Storage
- Power
- Construction and deconstruction

3. Research System
- Technology tree
- Unlocks
- Recipe unlocks
- Building unlocks
- Expedition unlocks

4. Equipment System
- Weapons
- Armor
- Tools
- Expedition supplies
- Crafted from factory resources

5. Expedition System
- Dangerous planets
- Short missions
- Rare resources
- Unique items
- Extraction points
- Risk and reward

6. Combat System
- Simple real-time combat
- Player or unit weapons
- Enemy creatures or drones
- Damage, health, death, loot

7. Save System
- Factory state
- Research progress
- Inventory
- Unlocked planets
- Player progression

## Folder Rules
Use this structure:

Assets/_Project/Scripts/Core
Assets/_Project/Scripts/Camera
Assets/_Project/Scripts/Input
Assets/_Project/Scripts/World
Assets/_Project/Scripts/Grid
Assets/_Project/Scripts/Buildings
Assets/_Project/Scripts/Factory
Assets/_Project/Scripts/Resources
Assets/_Project/Scripts/Power
Assets/_Project/Scripts/Research
Assets/_Project/Scripts/Equipment
Assets/_Project/Scripts/Expedition
Assets/_Project/Scripts/Combat
Assets/_Project/Scripts/UI
Assets/_Project/Scripts/Data
Assets/_Project/Scripts/Save

## Coding Style
- Classes: PascalCase
- Methods: PascalCase
- Private fields: _camelCase
- Serialized fields: [SerializeField] private
- Interfaces start with I
- Avoid abbreviations
- Avoid magic numbers
- Prefer small files
- Prefer explicit names over clever names

## Initial MVP
The first playable milestone must include:

Factory:
- One base planet
- Grid-based building placement
- Two resource nodes
- Miner
- Conveyor
- Smelter
- Storage
- Generator
- Research Lab
- Equipment Workshop

Expedition:
- One dangerous planet
- One enemy type
- One rare resource
- One extraction point
- Basic player movement
- Basic weapon
- Successful extraction brings rare resources back to base

Progression:
- Unlock one new factory building or recipe through factory progression
- Expeditions may provide faster or alternative access to materials, but must not be required for main progression

## Non-goals for MVP
Do not implement:
- Multiplayer
- First-person camera
- Full open world
- Procedural planets
- Complex enemy AI
- Complex physics
- Full Satisfactory-style conveyor complexity
- Full Tarkov-style inventory complexity
- Online economy
- Roguelike puzzle mechanics

## Token Budget / Caveman Harness

Default mode:
- compact
- direct
- no long prose
- no repeated context

Do not repeat:
- project summary
- current prompt
- old roadmap
- obvious rules
- non-goals list
- design background

Use short output.

Prefer:
- short words
- short lines
- bullets
- file paths
- status only

Avoid:
- long explanation
- design essay
- motivation text
- full recap
- many options
- verbose reasoning

## Work Search Rules

Before edit:
- read needed files only
- search narrow folders
- avoid whole-project scan
- do not inspect unrelated systems

During edit:
- small diff
- reuse existing code
- no broad refactor
- no unrelated rewrite
- keep model/view split

After edit:
- run relevant tests if possible
- report only result
- mention failed tests with cause
- mention manual checks

If unclear:
- make safe local choice
- state assumption in one short line
- ask only if blocked

## Report Format

Use this exact format:

Done.

Changed:
- path/file.cs
- path/file.cs

Tests:
- Compile: pass/fail/not run
- EditMode: pass/fail/not run

Manual:
- FactoryDebug Play: pass/fail/not run
- Feature check: pass/fail/not run

Risk:
- short note

Next:
- short next step

No extra closing text.
