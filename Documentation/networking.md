## Networking Features

In my game, I have used [Forge Networking Remastered](https://forgenetworkingremastered.readthedocs.io/en/latest/) for handling all networking tasks.

Fields are used to keep data consistent across all connected clients and can store information that will likely change.

RPCs are Remote Procedure Calls which allow for a function to run remotely on different clients.

---

## Forge Settings

Within forge, I utilise two NetworkObjects; one for players and another for the gameplay controller.

| Player | Gameplay Controller |
| :---: | :---: |
| ![](/img/Networking/PlayerNetworkObject.png) | ![](img/Networking/GameplayControllerNetworkObject.png) |

---

## Player
The `PlayerNetworkObject` contains two fields;

| Field | Type | Purpose |
| :---: | :---: | :---: |
| `playerIndex` | int | index of the current player |
| `playerMoney` | int | amount of money the current player has |

---

### RPC: `JoinTable()`

`JoinTable()` is for when the player joins the table. This allows data to be recived when a player has joined the table. This gets called when the local player joins and its PlayerActions script is set via `SetPlayerActions()`.

This uses no arguments.

| `SetPlayerActions()` | `JoinTable()` |
| :---: | :---: |
| ![](/img/Networking/PlayerSetPlayerActions.png) | ![](img/Networking/PlayerJoinTable.png) |

---

### RPC: `NetworkAction()`

`NetworkAction()` is for when a networked player has taken their action locally (`LocalAction()`) and needs to transmit that same data to the others connected. This uses two arguments.

| Argument | Type | Purpose |
| :---: | :---: | :---: |
| `Action` | int | the action the player took |
| `Money` | int | amount of money the action requires |

| `LocalAction()` | `NetworkAction()` |
| :---: | :---: |
| ![](/img/Networking/PlayerSetPlayerActions.png) | ![](img/Networking/PlayerNetworkAction.png) |

---

## GameplayController
The `GameplayControllerNetworkObject` contains two fields;

| Field | Type | Purpose |
| :---: | :---: | :---: |
| `deckSeed` | int | the current seed of the deck |
| `totalPlayers` | int | the amount of players in the game |

### RPC: `AddNetworkPlayer()`

`AddNetworkPlayer()` can be sent by any client for when they add a local player.

This sends two arguments.

| Argument | Type | Purpose |
| :---: | :---: | :---: |
| `playerIndex` | int | the index of the player |
| `isHost` | bool | is the player the host player |

| `AddPlayer()` | `AddNetworkPlayer()` |
| :---: | :---: |
| ![](/img/Networking/GameplayControllerAddPlayer.png) | ![](img/Networking/GameplayControllerAddNetworkPlayer.png) |

### RPC: `StartGame()`

`StartGame()` is sent by the server to forcibly start the game for all connected clients. This removes the risk of desync issues.

This sends no arguments.

| `StartGame()` |
| :---: |
| ![](/img/Networking/GameplayControllerStartGame.png) |

### RPC: `RestartGame()`

`RestartGame()` is similar to `StartGame()` as it is sent by the server to forcibly restart the game for all connected clients. This removes the risk of desync issues.

This sends no arguments.

| `RestartGame()` & `StartNextPhase()` |
| :---: |
| ![](/img/Networking/GameplayControllerRestartGameNetwork.png) |

### RPC: `UpdateDeckString()`

`UpdateDeckString()` is for when the server sends the seed for the deck to the other connections - this is to avoid issues where the deck becomes broken or desynced when trying to randomly generate from a seed.

The string that gets sent is an encoded version of a deck (converted from a list to a string) which can then be properly parsed when it arrives at the client.

An RPC is being used here - rather than a field - due to the fact that FNR doesn't support string fields.

| Argument | Type | Purpose |
| :---: | :---: | :---: |
| `deckString` | string | the string of the deck |

| `Initialize()` | `RestartGame()` | `UpdateDeckString()` |
| :---: | :---: | :---: |
| ![](/img/Networking/GameplayControllerInitialize.png) | ![](img/Networking/GameplayControllerRestartGame.png) | ![](img/Networking/GameplayControllerUpdateDeckString.png) |