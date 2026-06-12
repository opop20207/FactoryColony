# FactoryColony Dev Notes

## FactoryDebug Scene

Open `Assets/_Project/Scenes/BasePlanet/FactoryDebug.unity` and press Play.

`FactoryDebugSceneBootstrap` is the temporary entry point for the current playable debug environment. There is no formal map system yet. On Play, the bootstrap creates and connects the runtime test map, views, simulation, input controllers, HUD, and build menu.

Generated test map:
- Grid: 10x10 on the XZ plane, cell size 1
- Resource nodes:
  - `(1, 1)`: IronOre
  - `(1, 3)`: CopperOre
  - `(1, 5)`: Coal
- Initial Base Inventory:
  - IronPlate x20
  - CopperWire x20
- Initial lines:
  - IronOre Miner
  - Conveyor
  - Smelter
  - Conveyor
  - Assembler using IronPlate recipe
  - Conveyor
  - Storage
  - CopperOre Miner
  - Conveyor
  - Smelter
  - Conveyor
  - Assembler using CopperWire recipe
  - Conveyor
  - Storage
  - preloaded Gear Assembler
  - Conveyor
  - Storage
  - three Generators

Current debug goals:
- IronPlate x50 in Base Inventory
- CopperWire x30 in Base Inventory
- Gear x5 in Base Inventory
- Goal progress is calculated from Base Inventory only.
- Storage resources do not count toward goals until they are collected with `C`.
- This is a first-pass debug goal system, not the final tech tree or mission system.

Current controls:
- WASD / Arrow Keys: move player
- LeftShift + WASD / Arrow Keys: move camera
- Mouse Wheel: zoom camera
- F: toggle camera follow
- E: select nearby building
- Shift+E: collect nearby Storage resources
- Mouse Hover: highlight grid cell
- Build Menu buttons: select building
- Number keys 1-6: debug building selection
- R: rotate selected preview if rotatable
- Left Click: place selected building when valid
- Right Click: select hovered building for detail inspection
- Delete / Backspace: remove hovered building
- C: collect Storage resources into Base Inventory
- F5: save FactoryDebug state
- F9: load FactoryDebug state
- ESC: clear build preview and inspected building selection

Debug inspection:
- The right-side Building Detail panel shows the selected building's type, instance id, direction, size, power values, inventory, selected recipe, and basic capability flags.
- The selected building is marked with a cyan `BuildingSelectionHighlight`.
- The panel updates while simulation ticks change the selected building inventory.

Debug save/load:
- Save path: `Application.persistentDataPath/factory_debug_save.json`
- Saved data: grid size, resource nodes, placed buildings, building inventories, selected recipe ids, Base Inventory, and goal progress snapshot.
- Not saved: power snapshot, Storage aggregate cache, FactorySimulation internals, camera position, UI selection state, and tick count.
- Power and Storage totals are recalculated after load.
- Goal progress is recalculated from Base Inventory after load.
- This is a first-pass FactoryDebug-only save/load path, not the final save slot system.

Visual prototype:
- Current visuals are a 2.5D low-poly prototype, not final art.
- The style is testing a readable miniature factory direction inspired by Thronefall.
- No external assets are used; buildings, resource nodes, highlights, lighting, and materials are generated from Unity primitives.
- Visual styling is kept in View/Visual code so it can be replaced later by real prefabs or model assets without changing pure simulation models.

Resource visual tokens:
- Current resource movement visualization is a first-pass debug token view.
- Tokens summarize each building Inventory by resource type.
- There is no smooth item movement animation yet.
- Exact amounts remain in HUD and Building Detail panel.
- Tokens are capped per building to keep the scene readable.

Conveyor token animation:
- Conveyor token movement is View-only prototype feedback.
- Actual resource movement still happens in `FactorySimulation` ticks.
- There is no conveyor slot or segment model yet.
- Tokens loop across the conveyor direction to make Inventory state easier to read.

Player movement:
- FactoryDebug spawns a primitive `Player` unit near the map center.
- Movement is Transform-based on the XZ plane.
- Camera movement uses LeftShift plus movement keys to avoid input conflict.
- Camera follow can be toggled with `F`.
- Combat, equipment, interaction, and building collision are not implemented yet.
- The player can currently pass through buildings.

Player interaction:
- Nearby buildings are detected from the player's XZ position and building occupied cells.
- `E` selects the nearby building and shows it in the Building Detail panel.
- `Shift+E` collects resources from the nearby Storage only.
- Mouse right-click selection and `C` collect-all Storage remain available.
- Proximity interaction does not change recipes, upgrades, build rules, or equipment.
- Combat, player inventory, and gear systems are not implemented yet.

ScriptableObject data definitions:
- Create building data with `Create > FactoryColony > Data > Building Definition`.
- Create recipe data with `Create > FactoryColony > Data > Recipe Definition`.
- Create resource display data with `Create > FactoryColony > Data > Resource Definition`.
- Create a database with `Create > FactoryColony > Data > Definition Database`.
- Put assets under `Assets/_Project/ScriptableObjects/Buildings`, `Recipes`, and `Resources`.
- Assign the database to `FactoryDebugSceneBootstrap.definitionDatabase`.
- If no database is assigned, FactoryDebug uses `DebugBuildingDefinitions` and default recipes.
- ScriptableObjects are data input only; simulation still uses pure C# models.

Current limitations:
- The debug map is generated by `FactoryDebugSceneBootstrap`, not by a real map/loading system.
- Resource movement has no animation yet; simulation is quantity-based.
- Building visuals are primitive debug shapes.
- Miner debug definition currently requires a resource node, but does not lock to a specific resource type.
- Building menus are first-pass debug UI, not final game UI.
- Building Detail is still a debug inspector panel. It does not support recipe changes, inventory transfer, upgrades, or building configuration.
- The Gear goal is supported by a preloaded debug Gear Assembler until proper recipe/input controls exist.
