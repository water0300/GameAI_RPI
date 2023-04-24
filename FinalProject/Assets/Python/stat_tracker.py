import matplotlib.pyplot as plt

class Data:

	def __init__(self, timestamp, population, speed, forwardBias, metabolism, detection, desire, gestation):
		self.timestamp = timestamp
		self.population = population
		self.speed = speed
		self.forwardBias = forwardBias
		self.metabolism = metabolism
		self.detection = detection
		self.desire = desire
		self.gestation = gestation

def parse_file(filename):
	data = []
	with open(filename, 'r') as f:
		for line in f:
			l = line.replace(',', '').split()
			# print(float(l[0]))
			data.append( Data(float(l[0]), int(l[1]), float(l[2]), float(l[3]), float(l[4]), float(l[5]), float(l[6]), float(l[7])) )
	return data


def graph_data(herbivore, carnivore):
    plt.xlabel("Timestep")
    plt.ylabel("population count")
    plt.plot([d.timestamp for d in herbivore], [d.population for d in herbivore], label='herbivore')
    plt.plot([d.timestamp for d in carnivore], [d.population for d in carnivore], label='carnivore')
    plt.legend()
    plt.show()
    plt.xlabel("Timestep")
    plt.ylabel("speed")
    plt.plot([d.timestamp for d in herbivore], [d.speed for d in herbivore], label='herbivore')
    plt.plot([d.timestamp for d in carnivore], [d.speed for d in carnivore], label='carnivore')
    plt.legend()
    plt.show()	
    plt.xlabel("Timestep")
    plt.ylabel("forward bias")
    plt.plot([d.timestamp for d in herbivore], [d.forwardBias for d in herbivore], label='herbivore')
    plt.plot([d.timestamp for d in carnivore], [d.forwardBias for d in carnivore], label='carnivore')
    plt.legend()
    plt.show()	
    plt.xlabel("Timestep")
    plt.ylabel("metabolism")
    plt.plot([d.timestamp for d in herbivore], [d.metabolism for d in herbivore], label='herbivore')
    plt.plot([d.timestamp for d in carnivore], [d.metabolism for d in carnivore], label='carnivore')
    plt.legend()
    plt.show()
    plt.xlabel("Timestep")
    plt.ylabel("detection radius")
    plt.plot([d.timestamp for d in herbivore], [d.detection for d in herbivore], label='herbivore')
    plt.plot([d.timestamp for d in carnivore], [d.detection for d in carnivore], label='carnivore')
    plt.legend()
    plt.show()
    plt.xlabel("Timestep")
    plt.ylabel("desirability")
    plt.plot([d.timestamp for d in herbivore], [d.desire for d in herbivore], label='herbivore')
    plt.plot([d.timestamp for d in carnivore], [d.desire for d in carnivore], label='carnivore')
    plt.legend()
    plt.show()
    plt.xlabel("Timestep")
    plt.ylabel("gestation duration")
    plt.plot([d.timestamp for d in herbivore], [d.gestation for d in herbivore], label='herbivore')
    plt.plot([d.timestamp for d in carnivore], [d.gestation for d in carnivore], label='carnivore')
    plt.legend()
    plt.show()
 
if __name__ == '__main__':
	herbivore_data = parse_file('herbivore_data1.txt')
	carnivore_data = parse_file('carnivore_data1.txt')

	graph_data(herbivore_data, carnivore_data)
 
