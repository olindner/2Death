# 2Death

Defend your wizard's castle in this unique castle-defender game! 

**[Play here on Itch.io!](https://olindner.itch.io/2death)**

**Controls**

- Click an Enemy - Cause base click damage, also sets temporary targetting for Turrets
- Click on Upgrade Menu - Open/close the panel to allow Turret building and upgrading

**Gameplay**

You find yourself, the Light Wizard, sitting atop your noble watchtower post on a warm summer day. All of a sudden, you spy a horde of green goblins attempting to storm the wall and break into the peaceful city!

<img src="Assets/Screenshots/WallAndTurrets.png" title="Build your defenses!" width="200px" height="600px"> <img src="Assets/Screenshots/TurretMenu.png" title="Use your arsenal to defend your Wizard's castle!" width="350px" height="350px"> 
<img src="Assets/Screenshots/AutoAttack.png" title="Save up money for better upgrades!" width="350px" height="500px">

It's time to use all your wizarding spells to send these goblins back from whence they came!

<img src="Assets/Screenshots/ActiveShots.png" title="Repel the enemies!" width="600px" height="350px">

Use your staff to instantly deliver small damage spells directly to their scaly bodies! Collect gold and build up your magical turret defense to help you in the fight against evil!

**Behind the Scenes**

While there will likely always be design and UI updates, I am most focused on creating a clear and scalable code base. Some of my design highlights are:
- Game Manager as a Singleton
    - Uses static state and duplication guards, allowing single source-of-truth and access from any script
- Observables/Subscribers
    - Allows functional cascading when object state changes (e.g. updating the UI display when an enemy is damaged)
- State System
    - Routes game logic (Gameplay, Various Menus, Spawning Enemies, etc)
- Auto Targeting
    - Efficiently calculates the closeset enemy for turret firing and makes the game feel more developed

**Inspiration**

This project was originally started to compete in a game jam, but turned into an effort of understanding how state-based games work. I have always been fascinated by tower-defense games, so I was curious how difficult it was to create one from scratch!
