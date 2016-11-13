import numpy as np
import scipy.io
import scipy.cluster
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D



def _make_training_set(data):
    """ Separate data set into 2 sets. 
    1/6 of the dataset is training set and the rest is test set
    Parameter:
        data: waveform data (width = number of samples per spike)
    """
    n = data.shape[0]
    idx_training = np.random.choice(n, n//6, replace=False)
    training_set = data[idx_training]
    test_set = [data[i] for i in range(n) if n not in idx_training]
    return training_set, test_set


def PCA_train(training_set, n_components):
    """ Use np.linalg.svd to perform PCA
    Parameters:
        training_set: the data set to perform PCA on (MxN)
        n_components: the dimensionality of the basis to return (i.e. number of neurons)
    Returns: 
        The n_components principal components with highest significants and 
        the mean of each column of the original data
    Hint1: Subtract the mean of the data first so the average of each column is 0. The axis 
        parameter of np.mean is helpful here.
    Hint2: Find out a special property of the "s" returned by np.linalg.svd either by printing
        it out or reading the documentation.
    """    
    # YOUR CODE HERE #
    # SOLN START #
    mean = np.mean(training_set, axis=0)
    training_set = training_set - mean
    U, s, V = np.linalg.svd(training_set)
    basis_components = V[:n_components]     # the larger components are given first
    # SOLN END #
    
    return basis_components, mean

def PCA_classify(data, new_basis, mean):
    """ Project the data set, adjusted by the mean, into the new basis vectors
    Parameters:
        data: data to project (MxN)
        new_basis: new bases (KxN)
        mean: mean of each timestamp from PCA (list of length N)
    Returns: 
        Data projected onto new_basis (MxK)
    Hint: Don't forget to adjust the data with the PCA training mean!
    """
    # YOUR CODE HERE #
    # SOLN START #
    return np.dot(data-mean, new_basis.T)
    # SOLN END #
    
def plot_3D(data, view_from_top=False):
    """ Takes list of arrays (x, y, z) coordinate triples
    One array of triples per color
    """
    fig=plt.figure(figsize=(10,7))
    ax = fig.add_subplot(111, projection='3d')
    colors = ['#0000ff', '#00ff00', '#ff0000']
    for dat, color in zip(data, colors):
        Axes3D.scatter(ax, *dat.T, c=color)
    if view_from_top:
        ax.view_init(elev=90.,azim=0)                # Move perspective to view from top

def find_centroids(clustered_data, num_of_clusters):
    """ Use scipy.cluster.vq.kmeans to determine centroids of clusters
    Parameters:
        clustered_data: the data projected onto the new basis
        num_of_clusters: the expected number of clusters in the data
    Returns: 
        The centroids of the clusters
    Hint 1: make sure to first 'whiten' the data (refer to docs)
    """
    whitened_data = scipy.cluster.vq.whiten(clustered_data)
    return scipy.cluster.vq.kmeans(whitened_data, num_of_clusters)[0] 
    
def which_neuron(data_point, centroid1, centroid2):
    """ Determine which centroid is closest to the data point
    Inputs:
        data_point: 1x2 array containing x/y coordinates of data point
        centroid1: 1x2 array containing x/y coordinates of centroid 1
        centroid2: 1x2 array containing x/y coordinates of centroid 1
    Returns: 
        The centroid closest to the data point
    """
    
    # YOUR CODE HERE
    # SOLN START #
    dist1 = np.linalg.norm(data_point - centroid1)
    dist2 = np.linalg.norm(data_point - centroid2)
    
    if dist1 >= dist2:
        return 1
    else:
        return 2
    # SOLN END #    
###################################################################################################################################    
    
# Load data
presorted = {k: v for k, v in scipy.io.loadmat('spike_waveforms').items() \
             if k in ('sig118a_wf', 'sig118b_wf', 'sig118c_wf')}
presorted = [presorted['sig118a_wf'], presorted['sig118b_wf'], presorted['sig118c_wf']]

# Create training and testing dataset
two_neurons_training, two_neurons_test = _make_training_set(np.concatenate(presorted[1:]))
three_neurons_training, three_neurons_test = _make_training_set(np.concatenate(presorted))

# Plot 100 random spikes
for waveforms in three_neurons_training[:100]:
    plt.plot(waveforms)
plt.xlim((0,31))
plt.title('100 random spikes')

# Plot the 3 spike shapes based on the presorted data
plt.figure()
for waveforms in presorted:
    plt.plot(np.mean(waveforms, axis=0))
plt.xlim((0,31))
plt.title('Averaged presorted 3 neuron spikes')

# Perform PCA and plot the first 2 principal components.

# YOUR CODE HERE #
# START SOLN #
plt.figure()
two_new_basis, two_mean = PCA_train(two_neurons_training, 2)
for comp in two_new_basis:
    plt.plot(comp)
plt.xlim((0,31))      
# END SOLN #

# Project the test data two_neurons_test to the basis you found earlier

# YOUR CODE HERE #
# SOLN START #
two_classified = PCA_classify(two_neurons_test, two_new_basis, two_mean)
# SOLN END #

plt.figure()
plt.scatter(*two_classified.T)
plt.title('two_neurons_test')

# Project the presorted data and plot it
plt.figure()
presorted_two_classified = [PCA_classify(spikes, two_new_basis, two_mean) for spikes in presorted[1:]]
colors = ['#0000ff', '#00ff00']
for dat, color in zip(presorted_two_classified, colors):
    plt.scatter(*dat.T, c=color)
plt.title('Presorted data')

# Repeat training with three neuron data, producing 3 principal components

# YOUR CODE HERE #
# START SOLN #
plt.figure()
three_new_basis, three_mean = PCA_train(three_neurons_training, 3)
for comp in three_new_basis:
    plt.plot(comp)
# END SOLN #

# YOUR CODE HERE #
# BEGIN SOLN #
three_classified = PCA_classify(three_neurons_test, three_new_basis, three_mean)
# END SOLN #
plt.figure()
plot_3D([three_classified], False)
plt.title('three_neurons_test projected to 3 principal components')


presorted_classified = [PCA_classify(spikes, three_new_basis, three_mean) for spikes in presorted]
plot_3D(np.array(presorted_classified), False)
plt.title('Presorted data projected to 3 principal components') 

# Determine the centroids in the 2-neuron data

# YOUR CODE HERE #

# SOLN START #
centroid_list = find_centroids(two_classified, 2)
# SOLN END #

# Print the centroid locations
centroid1 = centroid_list[0]
centroid2 = centroid_list[1]

print('The first centroid is at: ' + str(centroid1))
print('The second centroid is at: ' + str(centroid2))

# Determine how many times neuron1 and neuron2 fired in the two_classifed data
# Hint: remember that we "whitened" the data to determine the centroids

num_of_firings = np.zeros(2)

# YOUR CODE HERE
# SOLN START #
two_classified = scipy.cluster.vq.whiten(two_classified)
for i in range(0,len(two_classified)):
    neuron_number = which_neuron(two_classified[i:], centroid1, centroid2)
    num_of_firings[neuron_number-1] +=1

# SOLN END #    
    
# Print the results
print('Neuron 1 Fired ' + str(num_of_firings[0]) + ' times')
print('Neuron 2 Fired ' + str(num_of_firings[1]) + ' times')

plt.show()
