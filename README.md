# 2Death

Defend your wizard's castle in this unique castle-defender game! 

**[Play here on Itch.io!](https://olindner.itch.io/2death)**

**Controls**

- Click an Enemy - Cause base click damage, also sets temporary targetting for Turrets
- Click on Upgrade Menu - Open/close the panel to allow Turret building and upgrading

**Gameplay**

Include screenshots here.

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
