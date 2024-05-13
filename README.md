# GChungMetroidvania
 Game dev project for CSI255 Spring Quarter 2024

## Documentation and Updates

### May 12, 2024
#### Features Added/Updated Since Last Update
- Added mana regeneration via attacking enemies
- Added in player sprite
- Added in player animations for idling, running, jumping, dashing, attacking, and dying
- Adjusted camera offset to make the world appear a little more zoomed in
- Added in second scene to test scene transitions
- Added in scene transitions from the first test scene to the second and back
- Adjusted movement settings to make movement less drastic
- Added in death/respawn UI
- Added in "checkpoint" object for the player to interact with in order to set a new respawn point

#### Issues Encountered
- Dashing through an enemy can cause the game to freeze. Fixed by changing the order in which functions are called in Update
- Attacking while in the air would cause the attack animation to play only after the player has landed. Found a missing animation transition and added it in
- Screen used for scene fade in/out was showing all black on game start. Fixed by disabling it in the editor and let the script handle the on/off toggle for it
- Interact function fires, but checkpoint object does not recognize the action. Working on a fix
- Player character will continue moving in the direction they died in. Working on a fix

#### Lessons Learned
- Sometimes, things just break for no reason. Had my UI not show up while running the game, but running it again right after had it back. This happened with a few other components during testing and I'm still not sure why or how it happened
- Making sure the order of operations is correct is very important as having something just one line off of where it needs to be can cause a game breaking bug
- It takes time and effort to design good levels. Just slapping something things together and hoping it works is not good level design (usually)

### April 28, 2024
#### Features Added/Updated Since Last Update
- Added in basic movement to the player
    - Horizontal movement, jumping
- Added in a double jump and dash movement to the player
- Added attack zones to the player
- Added basic enemy
- Added Zombie enemy
- Added player health
- Added player healing
- Added player mana
- Added in player and enemy recoil when player is attacking
- Added effects for when player gets hit
- Added iframes that activate when player gets hit

#### Issues Encountered
- Recoil movement in editor is not consistent with recoil movement in builds. Resolved by moving recoil function into FixedUpdate instead of Update
- Enemy would get "stuck" when coming in to contact with player. Resolved by fixing collision code and adding some mass onto enemy rigidBody
- Enemy would deal infinite damage when coming into contact with player. Resolved by fixing health code that was not firing off correctly
- Dashing through an enemy can cause the game to freeze. Working on a fix

#### Lessons Learned
- Update and FixedUpdate while very similar, are still different enough that you need to make sure you are putting the proper functions into each one
- Code that updates the HUD can also cause issues with other code that need to run at the same time
- It's important to save code frequently since only after you hit save does Unity compile everything
