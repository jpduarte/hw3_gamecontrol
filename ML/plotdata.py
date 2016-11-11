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

def find_nearest(array,value):
    idx = (np.abs(array-value)).argmin()
    return idx

#load file with data
pathandfile = '../matplot/matdraw/twofeets.txt'
target = open( pathandfile, 'r') 
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))

#create 
x = np.repeat(np.arange(8),12)
y = np.tile(np.arange(12),8)


time = datalist[:,0]

i=5
totalplot=10
while (i<totalplot):
  z = (datalist[(i-1)*int(len(time)/totalplot),1:])
  fig = plt.figure(i)
  ax = fig.gca(projection='3d')
  ax.plot_trisurf(x, y, z, cmap=cm.jet, linewidth=0.2)
  print("plot: "+str(i)+", index: "+str((i-1)*int(len(time)/totalplot))+", time (ms): "+str(time[(i-1)*int(len(time)/totalplot)]))
  i=i+1


plt.show()


