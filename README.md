# Lootorial

<a href="https://github.com/Raul125/Lootorial/releases"><img src="https://img.shields.io/github/v/release/Raul125/Lootorial?include_prereleases&label=Release" alt="Releases"></a>
<a href="https://github.com/Raul125/Lootorial/releases"><img src="https://img.shields.io/github/downloads/Raul125/Lootorial/total?label=Downloads" alt="Downloads"></a>

Description:
Configurable Npcs appear in static positions around the map, when you kill them, they drop items, such as piñatas.

https://github.com/gamehunt/CustomNPCs is a dependency.

Permissions for commands:
- lootorial.point
- lootorial.main
- lootorial.spawn

Commands:
- `lt point`
  - Description: It gives you your current position, rotation and room name. This is usefull for adding new pinhatas positions in the config.

- `lt spawn`
  - Usage: `lt spawn Name RoleType Health`
  - Example: `lt spawn Pinhata Tutorial 100`
  - Description: With this command, you can spawn a pinhata in your current position.

Default Config:
```yaml
lootorial:
  is_enabled: true
  # Should spawn Pinhatas on round start?
  spawn_on_round_start: true
  # Every x seconds it will try to spawn a pinhata, 0 is disabled
  spawn_randomly_pinhatas: 200
  # Chance of spawn for the pinhatas
  spawn_chance: 60
  # Displayed broadcast when a randomly pinhata spawns
  pinhata_broadcast: <color=cyan>A</color><color=green><b> Pinhata </b></color><color=cyan>has spawned in<b> %room </b>Room</color>
  # Max Items per pinhata
  max_items_per_pinhata: 8
  # Should a grenade explode?
  grenade: true
  # Grenade should damage?
  grenade_should_damage: false
  # Throw force of the items, it can be negative
  throw_force: 17
  # setting to 0 disables the random spin, otherwise the items will randomly spin
  random_spin_force: 20
  # List of the Possible items of the Pinhatas
  droppable_items:
  - Flashlight
  - Radio
  - KeycardGuard
  pinhatas_positions:
  # Name:RoleType:RoomName:Position:Rotation:Scale:Health
    positions:
    - Pinhata:ClassD:LCZ_ClassDSpawn (1):-31.1,1.4,-0.2:0.5,90.1:0.9,0.9,0.9:100
```
