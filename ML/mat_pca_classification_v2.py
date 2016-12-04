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

###################################################################################################################################

###################################################################################################################################    load data
#pathandfile = '../matplot/matdraw/feethandtrain.txt'
#pathandfile = '../matplot/matdraw/2016_11_26_test.txt'
#pathandfile = '../matplot/matdraw/juan_v1.txt'
pathandfile = '../matplot/matdraw/josh_v1.txt'
#pathandfile = '../matplot/matdraw/feettrain.txt'
target = open( pathandfile, 'r')
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))
target.close()
time = datalist[:,0]/1000
plt.figure(1)

plotelement = []
'''for i in range(4,len(time)):
    #this implementation of Hu components doesnt work well
    #plotelement = -np.sum(datalist[i,1:])+8*np.sum(datalist[i-1,1:])+0*np.sum(datalist[i-2,1:])-8*np.sum(datalist[i-3,1:])+np.sum(datalist[i-4,1:]) #five point derivative is quite noisy
    #plotelement.append((np.sum(datalist[i,1:])+np.sum(datalist[i-1,1:])+np.sum(datalist[i-2,1:])+np.sum(datalist[i-3,1:])+np.sum(datalist[i-4,1:]))/5.0) #average (filtered gives similar results)
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
    #print(hu)
    #hu_moments = moments_hu(datalist[i,1:].reshape((8, 12))/4000)
    plt.plot( time[i],hu[0],'o',color='r')
    plt.plot( time[i],hu[1],'o',color='b')
    plt.plot( time[i],hu[2],'o',color='g')
    plt.plot( time[i],hu[3],'o',color='k')
'''
'''area=0
    for node in datalist[i,1:]:
        if node>500:
            area=area+1
    #plt.plot( time[i],area,'o') #this give ok results'''
    #plt.plot( time[i],np.sum(datalist[i,1:])/area,'o') #this did not give clear results
#plt.plot( time[4:],plotelement,'o')
plt.figure(2)
plt.plot( time,np.sum(datalist[:,1:],axis=1),'o')
'''plt.figure(3)
    plt.plot( time[i],np.amax(datalist[i,1:],),'o')'''

#print(moments2e(datalist[0,1:].reshape((8, 12))))
#image = np.zeros((20, 20), dtype=np.double)
#image[13:17, 13:17] = 1


###################################################################################################################################  Perform PCA and centroid classification

pathandfile = './basis3steps_josh_v1.txt'
target = open( pathandfile, 'r')
three_new_basis = loadtxt(pathandfile,delimiter=',')
target.close()

pathandfile = './mean3steps_josh_v1.txt'
target = open( pathandfile, 'r')
three_mean = loadtxt(pathandfile,delimiter=',')
target.close()


pathandfile = './cluster3steps_josh_v1.txt'
target = open( pathandfile, 'r')
centroid_list = loadtxt(pathandfile,delimiter=',')
target.close()

clusterindex = []
dataall = datalist[:,1:]
i=0
plt.figure(3)
for data in dataall:
  if (np.sum(data)<15000):
    clusterindex = np.concatenate((clusterindex,[-1]),axis=0)
  else:
    three_classified = PCA_classify(data, three_new_basis, three_mean)
    clusterindex = np.concatenate((clusterindex,[which_cluster(three_classified, centroid_list)]),axis=0)
    plt.plot( time[i],three_classified[0],'o',color='k')
    plt.plot( time[i],three_classified[1],'o',color='r')
    plt.plot( time[i],three_classified[2],'o',color='b')
  i=i+1

plt.figure(4)
plt.plot( time,clusterindex,'o')


plt.show()
