#clasify data for different position in mat, with data already trained
#Juan Duarte,CS298 fall2016
import numpy as np
import scipy.io
import scipy.cluster
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from numpy import loadtxt
from sklearn.cluster import KMeans

def PCA_classify(data, new_basis, mean):
    """ Project the data set, adjusted by the mean, into the new basis vectors
    Parameters:
        data: data to project (MxN)
        new_basis: new bases (KxN)
        mean: mean of each timestamp from PCA (list of length N)
    Returns:
        Data projected onto new_basis (MxK)
    """
    return np.dot(data-mean, new_basis.T)

def which_cluster(data_point, centroid_list):
    i=0
    dist=100000.0
    for centroid in centroid_list:
      distaux = np.linalg.norm(data_point - centroid)
      if (distaux<dist):
        dist=distaux
        group=i
      i=i+1
    return group

###################################################################################################################################

###################################################################################################################################    load data
pathandfile = '../matplot/matdraw/feetcheck.txt'
#pathandfile = '../matplot/matdraw/feettrain.txt'
target = open( pathandfile, 'r')
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))
target.close()
time = datalist[:,0]/1000
plt.figure(1)
plt.plot( time,np.sum(datalist[:,1:],axis=1),'o')

dataall = datalist[:,1:]
###################################################################################################################################  Perform PCA and centroid classification

pathandfile = './basis3steps.txt'
target = open( pathandfile, 'r')
three_new_basis = loadtxt(pathandfile,delimiter=',')
target.close()

pathandfile = './mean3steps.txt'
target = open( pathandfile, 'r')
three_mean = loadtxt(pathandfile,delimiter=',')
target.close()


pathandfile = './cluster3steps.txt'
target = open( pathandfile, 'r')
centroid_list = loadtxt(pathandfile,delimiter=',')
target.close()

clusterindex = []
for data in dataall:
  if (np.sum(data)<25000):
    clusterindex = np.concatenate((clusterindex,[-1]),axis=0)
  else:
    three_classified = PCA_classify(data, three_new_basis, three_mean)
    clusterindex = np.concatenate((clusterindex,[which_cluster(three_classified, centroid_list)]),axis=0)

plt.figure(2)
plt.plot( time,clusterindex,'o')


plt.show()
