# Soul game

A 2D top-down action-RPG built in Unity, inspired by the Souls-like genre — combining a stat-driven character build, equip-load-based dodge rolling, a stamina/sanity dual-resource system, and boss encounters.

## **Installation (How to Install)**

1. Clone the project from GitHub: https://github.com/Kazaki1/Echoes-of-memory
2. Open it with Unity

## **How to Play**

1. From the Menu scene, press **Start** to begin. This leads into a Visual Novel–style dialogue intro (built with Ink) with a player-driven choice that kicks off the run.
2. Build your character through five core attributes — **Vigor, Strength, Dexterity, Intelligence, Endurance** — raised by spending Souls (currency) at each **Soul Level** to shape health, stamina, damage, sanity, and carry capacity.
3. Manage your **Equip Load**: how much weapon/armor weight you carry determines your dodge-roll speed and duration, from a fast light roll down to being unable to roll at all if overloaded.
4. Fight through enemies using melee combat, manage your **Stamina** for attacks and dodges, and defeat the boss guarding each area to progress.
5. Use **Checkpoints** to save progress and respawn after death, and **Fast Travel points** (some free, some paid) to move quickly between explored areas.

## **Project Overview**

### **Core Mechanics**

- **Attribute & Leveling System**: The character build revolves around five RPG attributes — Vigor, Strength, Dexterity, Intelligence, and Endurance — tracked under a **Soul Level**. Raising these attributes recalculates derived stats (max health, max stamina, max sanity, equip capacity) automatically.
- **Equip Load & Dodge Roll**: Total equipped weight is compared against the character's max carry capacity to produce a load percentage, which is bucketed into five tiers — *Light Load (Fast Roll)*, *Medium Load (Mid Roll)*, *Heavy Load (Slow Roll)*, *Overloaded (Fat Roll)*, and *Severely Overloaded (No Roll)*. Each tier scales movement speed, sprint speed, dodge speed/duration, and dodge stamina cost, directly mirroring Dark Souls' iconic load-based mobility system. Dodging briefly grants invincibility frames.
- **Stamina System**: A regenerating resource that gates sprinting and dodge rolling; running out forces the player to fight or move more cautiously, scaled by the Endurance attribute.
- **Sanity System**: A secondary resource separate from health, scaled by Intelligence using a non-linear growth curve (fast early gains, a mid-level peak, then diminishing returns at high levels — a soft-cap curve similar to how HP scales with Vigor in Souls games). Sanity can be drained and restored by specific gameplay events, adding a psychological-pressure layer on top of standard HP/stamina management.
- **Combat System**: Real-time melee combat with weapon-specific damage values, animation-driven hitboxes, and knockback on enemy hits. Damage output is influenced by the Strength/Dexterity attributes and the equipped weapon's stats.
- **Boss Fight Areas**: Dedicated arena zones that lock the player in for a boss encounter, testing the build and resources accumulated so far.
- **Enemy AI**: Enemies use pathfinding (A* Pathfinding Project) to navigate the world and pursue the player, dealing both melee and ranged damage.
- **Checkpoint System**: Save points that record player progress and act as the respawn location on death — restoring health/stamina but requiring enemies to be re-cleared, in classic Souls-like fashion.
- **Fast Travel System**: Lets players warp between discovered points; some fast travel points are free while others require paying in-game currency (Souls/Money).
- **Currency & Item Buffs**: A Souls-like currency (Money) is earned and spent on upgrades or services. Consumable/equipment items can grant temporary buffs — extra damage, armor, health, stamina, or dodge speed.
- **Narrative Intro (Visual Novel)**: The opening sequence is driven by **Ink**, a branching dialogue scripting system, presenting story text and player choices before gameplay begins.

### **Tech Stack:**
- **Unity** — game engine (2D, URP)
- **C#** — gameplay systems and mechanics
- **Ink** — branching dialogue / narrative scripting
- **A\* Pathfinding Project** — enemy AI navigation

## **Team Responsibilities**

- **Vo Thanh Luan**: Level design, environment art direction, movement and combat mechanics, story writing
- **Tran Huy Cuong**: Audio implementation, level design, game UI (menu, pause, etc.), save system implementation

## **Implemented Systems**

- Player attribute & leveling system (Vigor / Strength / Dexterity / Intelligence / Endurance)
- Equip load–based movement and dodge roll system
- Stamina and Sanity resource systems
- Melee combat and weapon/armor stat system
- Boss fight arena system
- Enemy AI with pathfinding
- Checkpoint and respawn system
- Fast travel system (free and paid)
- Currency and item buff system
- Visual Novel intro powered by Ink
- Scene transition and Audio system

