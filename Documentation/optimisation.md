## Optimisation of Features

As part of optimisation, I have explored two different Deck Shuffling algorithms, `ShuffleDeck()` and `Shuffle()`.

I have put both of them to the test to showcase how poorly optimised code can be detrimental to a program.

| `Shuffle()` | `ShuffleDeck()` |
| :---: | :---: |
| Well Optimised | Poorly Optimised | 
| <img src="./img/Optimisation/Shuffle.png" width=75%> | <img src="./img/Optimisation/ShuffleDeck.png" width=75%> |

The `Shuffle()` is better because it will never prouce duplicate values unlike `ShuffleDeck()`, this means that a lot of redundant computation is mitgated by optimisation.

Whilst I have used the Unity Profiler for aide in finding which is more performant, I have also used Stopwatches. These allow a programmer to measure the computation time of their program. Whilst both scripts run 10 times, `Shuffle()` manages to run that within ~0ms compared to `ShuffleDeck()` which has a computation time of ~154ms which is still 15ms per run on average.

![](img/Optimisation/StopwatchTimes.png)

The profiler goes further into these timings, showcasing 155.27ms for `ShuffleDeck()` and 0.25ms for `Shuffle()`

As for the profiler, you can see in the image below that there is a spike which is primarily taken by `Deck.ShuffleDeck()`. Whilst this uses 41.6% of the total thread, only 0.8% is used by the function directly with the remaining 40.8% primarily being used from calling `Debug.Log()`.

Regardless, 0.8% of the thread is huge in comparison to the 0.0% used by `Shuffle()`

![](img/Optimisation/Profiler.png)