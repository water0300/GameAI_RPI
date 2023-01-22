## Steering Behaviors are contained as follows:

### From the bottom up:

- `ITarget[...]Updaters()` handle the updating of the agent target depending on the Behaviors
- `I[...]Steer()` uses `ITarget[...]Updater`(s) to produce a COMPONENT of steering behavior
- `I[...]AgentState` contains `I[...]Steer()`(s) to produce the ACTUAL steering behavior as a STATE (ex. pursue, flee, wander, path following)
  
For example to replicate the pursue steering behavior, you would instantiate as follows:
```cpp
pursue =  MultiSteerContainer(new ArriveSteer(new LookaheadTargetPositionUpdater()), new AlignSteer(new FaceTargetRotationUpdater())), //pursue
```

In plain english: pursue is just a combination of arrive with some target lookahead (via `LookaheadTargetPositionUpdater()`) to handle position, and aligning to face the target (via `FaceTargetRotationUpdater()`) to handle rotation.

This sub-in approach via compision works for easily modifable behaviors like pursue and flee. For more complex behaviors that have more strict requirements (such as wander), you can see

Is this a little overengineered? Maybe. But does it work? Yes lol.
