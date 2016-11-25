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
#pathandfile = '../matplot/matdraw/5squats.txt'
pathandfile = '../matplot/matdraw/twohands.txt'
target = open( pathandfile, 'r')
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))

#create
x = np.repeat(np.arange(8),12)
y = np.tile(np.arange(12),8)


xlist = np.arange(8)
ylist = np.arange(12)
Y, X = np.meshgrid(ylist,xlist)

time = datalist[:,0]

maxdata = np.amax(np.amax(datalist[:,1:]))

i=1
totalplot=30
threshold=0
k=2
while (i<totalplot):
  z = (datalist[(i-1)*int(len(time)/totalplot),1:])
  Z = z.reshape((8, 12))
  #skimage.measure.moments_hu
  plt.figure(i)
  cp = plt.contourf(X, Y, Z,vmin=0, vmax=maxdata)
  plt.colorbar(cp)
  i=i+1

plt.show()
