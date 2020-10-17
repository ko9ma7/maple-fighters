# Maple Fighters
This is a small game similar to MapleStory. 

- Click [here](https://benzuk.github.io/) to play.

The server-side will be able to accommodate many players who can play together in the game world.

## Technology

**Client**: Unity WebGL   
**Server**: C#, Rust   
**Database**: MongoDB, PostgreSQL   
**Reverse Proxy**: Nginx   

## Service Architecture

| Service                                              | Language      | Description                                                    														|
| ---------------------------------------------------- | ------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [auth-service](./src/auth-service)                   | C#            | Stores user data and verifies user. 			   															|
| [game-service](./src/game-service)                   | C#            | Creates a player in the game world to play with others. Also creates objects of the game world (e.g. mobs, NPCs, etc.). 	|
| [gameprovider-service](./src/gameprovider-service)   | Rust          | Provides a list of game servers. 																|
| [character-service](./src/character-service)         | Rust          | Creates and receives player character data. 																|
