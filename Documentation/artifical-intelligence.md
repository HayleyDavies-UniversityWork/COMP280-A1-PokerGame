## AI Features
The AI within my game is fairly simple but also produces some good behaviour that makes it difficult to tell when the AI is thinking.

The AI for the player exists within the `PlayerActions`, `EvaluateHand` and `PlayerAI` classes.

Within `PlayerAI` the AI has a variety of variables. These are as follows:

| Variable | Type | Purpose |
| :---: | :---: | :---: |
| `playerActions` | PlayerActions | a reference to the player actions |
| `handValue` | HandValue | the value of the AI's hand |
| `bluffChance` | int | the modifier for the AI to potentially bluff |
| `minBluffChance` | int | the minimum bluff chance |
| `maxBluffChance` | int | the maximum bluff chance |
| `maxValue` | int | the maximum hand value an AI will have |
| `value` | float | the normalized hand value |
| `betThreshold` | float | the value threshold that must be met in order to bet |

How the AI works `PlayerAI.CalculatePlay()`:
![](img/AI/CalculatePlay.png)

This gets called from `PlayerActions.HandlePlayerAI()`:
![](img/AI/HandlePlayerAI.png)
