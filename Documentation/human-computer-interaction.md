**Sources:
https://www.nngroup.com/articles/usability-heuristics-applied-video-games/**

### Heuristics List:
1. Game should make the player aware of when they have queued an action.
2. Game should make the player aware that they are timed via a visual.
3. Game should make it clear when a player has folded.
4. Game should display which player won chips.
5. Game should display which actions they cannot take.
6. Game should display whether the player is Checking or Calling (and how much that will cost)
7. Game should display how many chips a player and their opponents have.
8. Game should display active players' hands at the end of the round.

---

### 1. Game should make the player aware of when they have queued an action.

#### Problem:
The player doesn't have any direct confirmation that they had a queued action which could lead to players accidentally queuing or unqueuing an action - resulting in frustration.

#### Heuristic Met Initially:
No

#### Solution:
Change the way the button looks when an action is queued.

| Before | After |
| :---: | :---: |
| ![](img/HCI/QueueActionBefore.png) | ![](img/HCI/QueueActionAfter.png) |

#### Changes Made:
As you can see in the before image, the "Call Any" button doesn't have any visual effect but in the after image, it has a transparency to it which should more easily distinguish to the player that they have queued an action.

#### Heuristic Met Now:
Yes

---

### 2. Game should make the player aware that they are timed via a visual.

#### Problem:
Before, the player wasn't aware they they were being timed and by adding a timer, it displays clearly that they must act fast to take their turn as to not keep other players waiting.

#### Heuristic Met Initially:
No

#### Solution:
This can be solved by adding a UI element that counts down when its the players turn.

| Before |
| :---: |
| ![](img/HCI/TimedVisualBefore.png)

#### Changes Made:
None

#### Heuristic Met Now:
No

---

### 3. Game should make it clear when a player has folded.

#### Problem:
Before, it was unclear as to whether a player had folded or not.

#### Heuristic Met Initially:
No

#### Solution:
This can be improved by greying out the players' cards when they have folded - which clearly distinguishes them from those still in the game.

| Before | After |
| :---: | :---: |
| ![](img/HCI/FoldedBefore.png) | ![](img/HCI/FoldedAfter.png) |

#### Changes Made:
The cards which get displayed by the player now change to a grey color when a player folds and back to white when the player isn't folded.

#### Heuristic Met Now:
Yes

---

### 4. Game should display which player won chips.

#### Problem:
Before, the game didn't clearly identify which player won chips at the end of the round.

#### Heuristic Met Initially:
No

#### Solution:
This can be solved by adding a pop-up in the middle of the screen when a player wins money.

| Before | After 1 | After 2 |
| :---: | :---: | :---: |
| ![](img/HCI/WinnerBefore.png) | ![](img/HCI/WinnerAfter.png) | ![](img/HCI/WinnerAfter2.png) |

#### Changes Made:
There is now a display in the middle of the screen which states what player has won what. This works with multiple winners also.

#### Heuristic Met:
Yes

---

### 5. Game should display which actions they cannot take.

#### Problem:
The game should show the player what actions they can and can't take in a clearly outlined and consistent way.

#### Heuristic Met Initially:
Yes

| Before |
| :---: |
| ![](img/HCI/ImpossibleActionBefore.png) |
| As you can see, the button for "Bet" and "Check" are both red, meaning they aren't interactable. |

#### Changes Made:
None

#### Heuristic Met:
Yes

---

### 6. Game should display whether the player is Checking or Calling (and how much that will cost).

#### Problem:
The game should display to the player whether they are going to be checking or calling. This should also be clear for the player by showcasing how much money they will be spending if they do call.

#### Heuristic Met Initially:
Yes

| Before |
| :---: |
| ![](img/HCI/CheckCallBefore.png) |
| ![](img/HCI/CheckCallAfter.png) |
| As you can see, the "Check" button also shows "Call $50" for when a bet needs to be placed by the player. |

#### Changes Made:
None

#### Heuristic Met:
Yes

---

### 7. Game should display how many chips a player and their opponents have.

#### Problem:
The game should show to the player how many chips both them and their opponents have. This is done via a HUD above each player.

#### Heuristic Met Initially:
Yes

| Before |
| :---: |
| ![](img/HCI/ChipsBefore.png) |
| As you can see, all the players have a monetary value shown in front of them, this is their table chips. |

#### Changes Made:
None

#### Heuristic Met:
Yes

---

### 8. Game should display active players' hands at the end of the round.

#### Problem:
Before, the game displayed all players' hands at the end of the game, this made it confusing to know who was and wasn't competing for the pot at that time.

#### Heuristic Met Initially:
No

#### Solution:
This can be improved by excluding players who have folded from showing their cards automatically.

| Before | After |
| :---: | :---: |
| ![](img/HCI/HandsBefore.png) | ![](img/HCI/HandsAfter.png) |

#### Changes Made:
I have made it so that players only show their hands at the end of the round if they haven't folded. This means that players who folded won't also show their cards.

#### Heuristic Met:
Yes