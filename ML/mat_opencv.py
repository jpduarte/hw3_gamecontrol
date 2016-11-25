#clasify data for different position in mat, with data already trained
#Juan Duarte,CS298 fall2016
import numpy as np
import scipy.io
import scipy.cluster
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from numpy import loadtxt
from sklearn.cluster import KMeans
from skimage import data, io, filters#,moments,moments_central,moments_hu,moments_normalized
import skimage

from numpy import mgrid, sum
from image_fx import moments_central,moments_normalized,moments_hu,moments2e

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
import cv2
###################################################################################################################################

###################################################################################################################################    load data
pathandfile = '../matplot/matdraw/feethandtrain.txt'
#pathandfile = '../matplot/matdraw/feettrain.txt'
target = open( pathandfile, 'r')
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))
target.close()
time = datalist[:,0]/1000

######################################countour plot
i=28
totalplot=30
z = (datalist[(i)*int(len(time)/totalplot),1:])
Z = z.reshape((8, 12))
'''
xlist = np.arange(8)
ylist = np.arange(12)
Y, X = np.meshgrid(ylist,xlist)

time = datalist[:,0]

maxdata = np.amax(np.amax(datalist[:,1:]))



threshold=0
k=2


  #skimage.measure.moments_hu
plt.figure(1)
cp = plt.contourf(X, Y, Z,vmin=0, vmax=maxdata)
plt.colorbar(cp)
i=i+1
'''
#######################################plot with open cv
plt.figure(2)
image = z.reshape((8, 12))#*255/2000
image = image.astype(np.uint8)
ret,thresh = cv2.threshold(image,100,255,0)
plt.imshow(image)

plt.figure(3)
plt.imshow(thresh)


print(cv2.HuMoments(cv2.moments(image)).flatten())

"""
x = np.repeat(np.arange(8),12)
y = np.tile(np.arange(12),8)
img = cv2.imread('sudoku.png',0)
plt.figure(3)
plt.imshow(img)
print(img)"""
from image_fx import moments_central,moments_normalized,moments_hu,moments2e

image = datalist[i,1:].reshape((8, 12))
m = moments_central(image,0,0,7)
cr = m[0, 1] / m[0, 0]
cc = m[1, 0] / m[0, 0]
#print(cr,cc)
mu = moments_central(image,cr,cc,7)
#print(mu)
nu = moments_normalized(mu,7)
#print(nu)
hu = moments_hu(nu)
print(hu)
plt.show()
