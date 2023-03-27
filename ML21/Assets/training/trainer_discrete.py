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

EPISODES = 2000000
MAX_EPISODE_STEPS = 500

class DQNAgent_Discrete:
    def __init__(self, state_size, discrete_branches):
        self.state_size = state_size
        self.discrete_branches = discrete_branches
        self.memory = deque(maxlen=2000)
        self.gamma = 0.95    # discount rate
        self.epsilon = 1.0  # exploration rate
        self.epsilon_min = 0.01
        self.epsilon_decay = 0.995
        self.learning_rate = 0.001
        self.model = self._build_model()
    def _build_model(self):
        # Neural Net for Deep-Q learning Model
        # model = Sequential()
        
        input_layer = Input(shape=(self.state_size,))
        hidden_layer_1 = Dense(24, activation='relu')(input_layer)
        hidden_layer_2 = Dense(24, activation='relu')(hidden_layer_1)
        hidden_layer_3 = Dense(24, activation='relu')(hidden_layer_2)
        
        output_layers = []
        for i in range(len(self.discrete_branches)):
            output_size = self.discrete_branches[i]
            output_layer = Dense(output_size, activation='softmax', name=f'output_{i}')(hidden_layer_3)
            output_layers.append(output_layer)
        
        model = Model(inputs=input_layer, outputs=output_layers)
        model.compile(loss='categorical_crossentropy', optimizer=Adam(lr=self.learning_rate))
        model.summary()
        return model
    
    def remember(self, state, action, reward, next_state, done):
        self.memory.append((state, action, reward, next_state, done))
        
    def act(self, state):
        if np.random.rand() <= self.epsilon:
            return [np.random.choice(self.discrete_branches[i]) for i in range(len(self.discrete_branches))]
        # print(state.shape)
        action_probs = self.model.predict(state) # shape: (1, num_branches)
        actions = [np.random.choice(self.discrete_branches[i], p=action_probs[i][0]) for i in range(len(self.discrete_branches))]
        return actions
    
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
    
    
def train_discrete(env : UE):
    try:
        behavior_name = list(env.behavior_specs)[0]
        print(f"Behavior name: {behavior_name}")
        
        spec = env.behavior_specs[behavior_name]  
        print("Spec: ", spec)
        
        state_size = spec.observation_specs[0].shape[0]
        discrete_branches = spec.action_spec.discrete_branches
        print("state size: ", state_size)
        print("discrete branches: ", discrete_branches)
        
        agent = DQNAgent_Discrete(state_size, discrete_branches)

        results = []
        
        for episode in range(EPISODES):
            env.reset()
            reward = 0
            decision_steps, terminal_steps = env.get_steps(behavior_name)
            agent_count = len(decision_steps.agent_id)
            done = [False for _ in range(agent_count)]

            for t in range(MAX_EPISODE_STEPS): #do an action for every agent in decision steps
                decision_steps, terminal_steps = env.get_steps(behavior_name)

                agents = []
                states = []
                actions = []
                for tracked_agent in decision_steps.agent_id:
                    state = decision_steps[tracked_agent].obs[0].reshape((1, 11))
                    
                    action = agent.act(state)
                    discrete = np.array(action).reshape((1,len(discrete_branches)))
                    action_tuple = ActionTuple(discrete=discrete)
                    # print("action: ", action_tuple.discrete)
                    
                    env.set_action_for_agent(behavior_name, tracked_agent, action_tuple)
                
                    agents.append(tracked_agent)
                    states.append(state)
                    actions.append(action)
                    
                env.step()
                
                decision_steps, terminal_steps = env.get_steps(behavior_name)

                for tracked_agent in agents:
                    if tracked_agent in decision_steps:
                        next_state = decision_steps[tracked_agent].obs[0]
                        reward += decision_steps[tracked_agent].reward
                    elif tracked_agent in terminal_steps:
                        # print('terminal:', done)
                        next_state = terminal_steps[tracked_agent].obs[0]
                        reward += terminal_steps[tracked_agent].reward
                        done[tracked_agent] = True
                        print(done)
   
                    agent.remember(state, action, reward, next_state, done)
                
                if all(done):
                    break
            if episode % 1 == 0:
                print("episode: {}/{}, reward: {}"
                    .format(episode, EPISODES, reward))
                
            results.append(reward)


        env.close()
        print("Closed environment")
        
    finally:
        with open('dqn_results.txt', 'w') as f:
            for item in results:
                f.write(f'{item}'+"\n")
        plt.plot(results)
        plt.show()
        
    
if __name__ == '__main__':
        
    print("Press Play...")
    channel = EngineConfigurationChannel()
    env = UE(side_channels=[channel])
    # channel.set_configuration_parameters(time_scale=20)
    env.reset()
    train_discrete(env)