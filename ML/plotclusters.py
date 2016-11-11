import matplotlib.pyplot as plt
from numpy import loadtxt
import numpy as np

from numpy.matlib import repmat
from scipy.misc import factorial
from scipy import sparse
from scipy.sparse import lil_matrix
from scipy.sparse.linalg import spsolve
from numpy.linalg import solve, norm
from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FormatStrFormatter
import scipy.io

import scipy.cluster
from sklearn.cluster import KMeans

def find_nearest(array,value):
    idx = (np.abs(array-value)).argmin()
    return idx

#load file with data
pathandfile = '../matplot/matdraw/twofeets.txt'
#pathandfile = '../matplot/matdraw/twohands.txt'
target = open( pathandfile, 'r') 
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))

#create 
x = np.repeat(np.arange(8),12)
y = np.tile(np.arange(12),8)


time = datalist[:,0]

i=1
totalplot=10
threshold=1000
k=2
while (i<totalplot):
  z = (datalist[(i-1)*int(len(time)/totalplot),1:])
  j=0
  xaux = []
  yaux = []
  zaux = []
  alldata = []
  for value in z:
    if (value>threshold):
      xaux.append(x[j])
      yaux.append(y[j])
      zaux.append(z[j])
      alldata.append([x[j],y[j]])
    j=j+1
  
  fig = plt.figure(i)
  ax = fig.gca(projection='3d')
  
  #ax.plot_trisurf(x, y, z, cmap=cm.jet, linewidth=0.2)
  if (len(alldata)>0):
    print("plot: "+str(i)+", index: "+str((i-1)*int(len(time)/totalplot))+", time (ms): "+str(time[(i-1)*int(len(time)/totalplot)]))
    
    
    '''minX = np.amin(xaux)
    maxX = np.amax(xaux)
    minY = np.amin(yaux)
    maxY = np.amax(yaux) 
    centroids = np.random.rand(k,2)
    centroids[:,0] = centroids[:,0]*(-minX+maxX)+minX
    centroids[:,1] = centroids[:,1]*(-minY+maxY)+minY '''
    
    kmeans = KMeans(n_clusters=2,precompute_distances=True).fit(alldata)
    centroids = kmeans.cluster_centers_
    labels = kmeans.labels_  
    print(centroids)
    print(labels)
    ax.scatter(xaux, yaux, zaux,c=labels.astype(np.float))
    ax.scatter(centroids[:,0], centroids[:,1], [1000,1000],marker='s', s=400)
  
  i=i+1






plt.show()


