# GChungMetroidvania
 Game dev project for CSI255 Spring Quarter 2024

## Documentation and Updates

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

#### Issue Encountered
- Recoil movement in editor is not consistent with recoil movement in builds. Resolved by moving recoil function into FixedUpdate instead of Update
- Enemy would get "stuck" when coming in to contact with player. Resolved by fixing collision code and adding some mass onto enemy rigidBody
- Enemy would deal infinite damage when coming into contact with player. Resolved by fixing health code that was not firing off correctly
- Dashing through an enemy can cause the game to freeze. Working on a fix

#### Lessons Learned
- Update and FixedUpdate while very similar, are still different enough that you need to make sure you are putting the proper functions into each one
- Code that updates the HUD can also cause issues with other code that need to run at the same time
- It's important to save code frequently since only after you hit save does Unity compile everything