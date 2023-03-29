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
from keras.layers import Dense, Input, Lambda
from keras.optimizers import Adam, SGD, RMSprop
from keras import utils

BUFFER_SIZE = int(500) # replay buffer size
BATCH_SIZE = 6         # minibatch size
DISCRETE_RESOLUTION = 6

EPISODES = 200000
MAX_EPISODE_STEPS = 200

def isclose(a, b, rel_tol=1e-09, abs_tol=0.0):
    return abs(a-b) <= max(rel_tol * max(abs(a), abs(b)), abs_tol)

def num_to_linspace(output):
    return np.linspace(-1, 1, output)

class DQNAgent_Continuous:
    def __init__(self, state_size, continuous_size):
        self.state_size = state_size
        self.continuous_size = continuous_size
        self.memory = deque(maxlen=BUFFER_SIZE)
        self.gamma = 0.95    # discount rate
        self.epsilon = 1.0  # exploration rate
        self.epsilon_min = 0.01
        self.epsilon_decay = 0.995
        self.learning_rate = 0.001
        self.model = self._build_model()

    #ex, if given 6, it will return [-1, -.6, -.2, .2, .6, 1]



    def _build_model(self):
        # Neural Net for Deep-Q learning Model
        # model = Sequential()
        
        input_layer = Input(shape=(self.state_size,))
        hidden_layer_1 = Dense(24, activation='relu')(input_layer)
        hidden_layer_2 = Dense(24, activation='relu')(hidden_layer_1)
        hidden_layer_3 = Dense(24, activation='relu')(hidden_layer_2)
        
        output_layers = []
        for i in range(self.continuous_size):
            output_size = DISCRETE_RESOLUTION
            output_layer = Dense(output_size, activation='linear', name=f'output_{i}')(hidden_layer_3)
            output_layers.append(output_layer)
        
        model = Model(inputs=input_layer, outputs=output_layers)
        model.compile(loss='mse', optimizer=Adam(lr=self.learning_rate))
        model.summary()
        # utils.plot_model(model, show_shapes=True)

        return model
    
    def remember(self, state, action, reward, next_state, done):
        self.memory.append((state, action, reward, next_state, done))
        
    def act(self, state):
        # print(self.num_to_linspace(DISCRETE_RESOLUTION))
        if np.random.rand() <= self.epsilon:
            # print('end act')

            return [np.random.choice(DISCRETE_RESOLUTION) for _ in range(self.continuous_size)]
        # print(state.shape)

        q_values = self.model.predict(state, verbose=0) # shape: (1, num_branches)

        # exit()
        actions = [np.argmax(q_value[0]) for q_value in q_values]
        # print("act: ", actions)
        # actions = [np.random.choice(self.num_to_linspace(DISCRETE_RESOLUTION), p = action_prob[0]) for action_prob in action_probs]

        return actions
    
    def replay(self, batch_size):
        minibatch = random.sample(self.memory, batch_size)
        for state, action, reward, next_state, done in minibatch:
            targets = [reward]*self.continuous_size
            # print('action q value', done)

            if not done:
                next_state_pred = self.model.predict(next_state)
                for i, action_q_value in enumerate(next_state_pred):
                    # print('action q value', action_q_value)
                    targets[i] = reward + self.gamma * np.amax(action_q_value)
                # next_action_probs = [next_state_pred[i][0] for i in range(self.continuous_size)]
            targets = np.array([targets])
            # print('begin predict')
            target_f = self.model.predict(state, verbose=0)
            # print('end predict')

            # print("replay targets: ", targets, targets.shape)
            # print("replay targetf: ", target_f, target_f)
            # print("action: ", action, len(action))
            for i in range(self.continuous_size):
                # print(f'target_f_{i}: {target_f[i]}')
                # print(f'targets_0_{i}: {targets[0][i]}')

                target_f[i][0][action[i]] = targets[0][i]

            # print("modified targetf: ", target_f)
            # exit()
            # print('BEGIN FIT')
            self.model.fit(state, target_f, epochs=2, verbose=2)
            # print('ENDING FIT')

        if self.epsilon > self.epsilon_min:
            self.epsilon *= self.epsilon_decay
    
    
def get_relevant_observation(spec):
    for observation in spec.observation_specs:
        if(observation.shape[0] == 10):
            return observation
        
def train_continuous(env : UE):
    results = []

    try:
        behavior_name = list(env.behavior_specs)[0]
        print(f"Behavior name: {behavior_name}")
        
        spec = env.behavior_specs[behavior_name]  
        print("Spec: ", spec)
        
        # state_size = sum([spec.observation_specs[i].shape[0] for i in range(len(spec.observation_specs))])
        state_size = get_relevant_observation(spec).shape[0]
        action_size = 5
        # discrete_branches = np.arange(-1, 1, action_size
        print("state size: ", state_size)
        print("action size: ", action_size)
        
        agent = DQNAgent_Continuous(state_size, action_size)

        
        for episode in range(EPISODES):
            env.reset()
            reward = 0
            decision_steps, terminal_steps = env.get_steps(behavior_name)
            agent_count = len(decision_steps.agent_id)
            done = [False for _ in range(agent_count)]

            for t in range(MAX_EPISODE_STEPS): #do an action for every agent in decision steps
                # print(f'step {t}')
                decision_steps, terminal_steps = env.get_steps(behavior_name)

                agents = []
                states = []
                actions = []
                # print(f'AGENTS: {decision_steps}')
                # print(f'len: {len(decision_steps.agent_id)}')

                for tracked_agent in decision_steps.agent_id:
                    # temp = 
                    # state = np.array([e for sublist in [decision_steps[tracked_agent].obs[i] for i in range(len(decision_steps[tracked_agent].obs))] for e in sublist]).reshape((1, state_size))
                    # print(decision_steps[tracked_agent].obs[-1]) #hardcoded
                    state = np.array(decision_steps[tracked_agent].obs[-1]).reshape((1, state_size))

                    # exit()


                    action_indicies = agent.act(state)
                    true_action = [num_to_linspace(DISCRETE_RESOLUTION)[action_index] for action_index in action_indicies]
                    # print('taking action:', true_action, 'at indices:', action_indicies)

                    continous = np.array(true_action).reshape((1,action_size))
                    action_tuple = ActionTuple(continuous=continous)                
                    env.set_action_for_agent(behavior_name, tracked_agent, action_tuple)
                
                    agents.append(tracked_agent)
                    states.append(state)
                    actions.append(action_indicies)
                    
                env.step()
                
                decision_steps, terminal_steps = env.get_steps(behavior_name)

                for tracked_agent, tracked_state, tracked_action in zip(agents, states, actions):
                    if tracked_agent in decision_steps:
                        # temp = [decision_steps[tracked_agent].obs[i] for i in range(len(decision_steps[tracked_agent].obs))]
                        next_state = np.array(decision_steps[tracked_agent].obs[-1]).reshape((1, state_size))

                        reward += decision_steps[tracked_agent].reward
                    elif tracked_agent in terminal_steps:
                        print('should not hit in hummingbird context')
                        # temp = [decision_steps[tracked_agent].obs[i] for i in range(len(decision_steps[tracked_agent].obs))]
                        next_state = np.array(decision_steps[tracked_agent].obs[-1]).reshape((1, state_size))
                        reward += terminal_steps[tracked_agent].reward
                        done[tracked_agent] = True

                    agent.remember(tracked_state, tracked_action, reward, next_state, done[tracked_agent])
                
                if all(done):
                    break
            # if episode % 20 == 0:
            print("episode: {}/{}, reward: {}"
                .format(episode, EPISODES, reward))
                
            results.append(reward)
            agent.replay(BATCH_SIZE)

        env.close()
        print("Closed environment")
    finally:
        with open('dqn_results_continuous.txt', 'w') as f:
            for item in results:
                f.write(f'{item}'+"\n")
        plt.plot(results)
        plt.show()    

if __name__ == '__main__':
        
    print("Press Play...")
    channel = EngineConfigurationChannel()
    env = UE(side_channels=[channel])
    channel.set_configuration_parameters(time_scale=20)
    env.reset()
    train_continuous(env)


    