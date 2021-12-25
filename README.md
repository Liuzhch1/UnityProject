# UnityProject

12.13 night taks distribute schedule:

连奕航: UI&Inventory

吕钧霆: Map

段欣: Player

刘志成: Enemy



## Player Features

- Movement & Rotation
- Model
- Advanced animation
  - base layer
    - idle
    - walk forward
    - walk backward
    - run forward 
    - turn back
    - jump
    - die
  - Top body layer
    - shoot
    - reload
- ammo
- health

## Log

### 12.14

#### Player

- player logic
  - move
  - rotate
  - jump
  - crouch/stand up
- model import
  - player
  - weapon
- simple move animation
  - hand gun idle
  - walk forward
  - run forward
  - walk backward

### 12.15

#### Player
- shoot(logic&animation)
- reload(logic&animation)

### 12.25

#### Player

##### done

- change weapon
  - <FPSplayerlogic.cs>
  - `public void changeWeapon(int type) `
  - type 0 => AR, type 1 => handgun
- weapon handgun animations
  - movement
  - take out
  - holster
  - reload
  - fire
- weapon AR animations
  - aim
  - fix reload
- player prefab

##### future work

- wepon handgun
  - aim
- add aim UI
- fix aim angle

## Inventory Features

- Interactive/collectable items
  - +health
  - ammo
  - weapon
  - scope
  
- Item Inventory UI

  - collectable items
  - scopes

- Weapon Inventory UI (Radial Panel)

- Player Status UI

  - health

  - weapon ammo

### 12.19

### Map
- nav mesh 
- door animations 

### Instructions

To create a new collectable item:

- Prepare your item prefab under `Assets/Prefabs/YOUR_WORK_FOLDER`
  - Add a `item` tag 
  - Add a `collider` and set it as trigger
  - Add `CollectableItem.cs` to it

- Under `Assets/Item Data`, create an item SO by click `Create/Inventory/Item Data`
  - Fill in `itemType`, `itemName`, `itemIcon`
  - 

