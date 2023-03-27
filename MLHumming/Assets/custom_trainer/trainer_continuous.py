import torch
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import matplotlib.pyplot as plt
import numpy as np
import random
from collections import namedtuple, deque
from mlagents_envs.environment import UnityEnvironment as UE
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from mlagents_envs.base_env import ActionTuple
from keras.models import Sequential
from keras.models import Model
from keras.layers import Dense, Input
from keras.optimizers import Adam, SGD, RMSprop

BUFFER_SIZE = int(1000) # replay buffer size
BATCH_SIZE = 32         # minibatch size
GAMMA = 0.99            # discount factor
TAU = 1e-3              # for soft update of target parameters
UPDATE_EVERY = 4        # how often to update the network
LR = 0.0005             # learning rate

EPISODES = 50000
MAX_EPISODE_STEPS = 500

device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")

class DQNAgent_Discrete:
    def __init__(self, state_size, action_size):
        self.state_size = state_size
        self.action_size = action_size
        self.memory = deque(maxlen=2000)
        self.gamma = 0.95    # discount rate
        self.epsilon = 1.0  # exploration rate
        self.epsilon_min = 0.01
        self.epsilon_decay = 0.995
        self.learning_rate = 0.001
        self.model = self._build_model()
    def _build_model(self):
        model = Sequential()
        model.add(Dense(24, input_dim=self.state_size, activation='relu'))
        model.add(Dense(24, activation='relu'))
        model.add(Dense(24, activation='relu'))
        model.add(Dense(self.action_size, activation='tanh'))
        model.compile(loss='mae',
                      optimizer=Adam(learning_rate=self.learning_rate))
        model.summary()
        return model
    
    def remember(self, state, action, reward, next_state, done):
        self.memory.append((state, action, reward, next_state, done))
        
    def act(self, state):
        if np.random.rand() <= self.epsilon:
            return np.random.uniform(low=-1, high=1, size=self.action_size)
        return self.model.predict(state)[0]
        # return [value * 2 - 1 for value in act_values[0]]
    
    def replay(self, batch_size):
        minibatch = random.sample(self.memory, batch_size)
        for state, action, reward, next_state, done in minibatch:
            target = reward
            if not done:
              target = reward + self.gamma * \
                       np.amax(self.model.predict(next_state)[0])
            target_f = self.model.predict(state)
            target_f[0][action] = target
            self.model.fit(state, target_f, epochs=2, verbose=0)
        if self.epsilon > self.epsilon_min:
            self.epsilon *= self.epsilon_decay
    
    
def train_continuous(env : UE):
    behavior_name = list(env.behavior_specs)[0]
    print(f"Behavior name: {behavior_name}")
    
    spec = env.behavior_specs[behavior_name]  
    print("Spec: ", spec)
    
    state_size = sum([spec.observation_specs[i].shape[0] for i in range(len(spec.observation_specs))])
    action_size = spec.action_spec.continuous_size
    print("state size: ", state_size)
    print("action size: ", action_size)
    
    agent = DQNAgent_Discrete(state_size, action_size)

    results = []
    
    for episode in range(EPISODES):
        env.reset()
        
        for t in range(MAX_EPISODE_STEPS): #do an action for every agent in decision steps
            decision_steps, terminal_steps = env.get_steps(behavior_name)

            agents = []
            states = []
            actions = []
            for tracked_agent in decision_steps.agent_id:
                # state = decision_steps[tracked_agent].obs[0]
                # state = np.array([decision_steps[tracked_agent].obs[i] for i in range(len(decision_steps[tracked_agent].obs))]).reshape((1, state_size))
                temp = [decision_steps[tracked_agent].obs[i] for i in range(len(decision_steps[tracked_agent].obs))]
                state = np.array([e for sublist in temp for e in sublist]).reshape((1, state_size))
                # print('state: ', state)
                # print('FIXED: ', state)
                action = agent.act(state)
                # print("action??: ", action)
                continous = np.array(action).reshape((1,action_size))
                action_tuple = ActionTuple(continuous=continous)
                # print("action: ", action_tuple.continuous)
                
                env.set_action_for_agent(behavior_name, tracked_agent, action_tuple)
            
                agents.append(tracked_agent)
                states.append(state)
                actions.append(action)
                
            env.step()
            
            decision_steps, terminal_steps = env.get_steps(behavior_name)

            for tracked_agent in agents:
                if tracked_agent in decision_steps:
                    temp = [decision_steps[tracked_agent].obs[i] for i in range(len(decision_steps[tracked_agent].obs))]
                    next_state = np.array([e for sublist in temp for e in sublist]).reshape((1, state_size))
                    reward = decision_steps[tracked_agent].reward
                    done = False
                elif tracked_agent in terminal_steps:
                    temp = [decision_steps[tracked_agent].obs[i] for i in range(len(decision_steps[tracked_agent].obs))]
                    next_state = np.array([e for sublist in temp for e in sublist]).reshape((1, state_size))
                    reward = terminal_steps[tracked_agent].reward
                    done = True
                    
                agent.remember(state, action, reward, next_state, done)
                
            if done:
                print("episode: {}/{}, reward: {}"
                    .format(episode, EPISODES, reward))
                results.append(reward)
                break


    env.close()
    print("Closed environment")
    plt.plot(results)
    
if __name__ == '__main__':
        
    print("Press Play...")
    channel = EngineConfigurationChannel()
    env = UE(side_channels=[channel])
    channel.set_configuration_parameters(time_scale=20)
    env.reset()
    train_continuous(env)