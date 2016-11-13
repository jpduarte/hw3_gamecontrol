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

from sklearn import datasets, svm, metrics

def find_nearest(array,value):
    idx = (np.abs(array-value)).argmin()
    return idx

#load file with data
pathandfile = '../matplot/matdraw/twofeets.txt'
pathandfile2 = '../matplot/matdraw/5squats.txt'
#pathandfile = '../matplot/matdraw/twohands.txt'
target = open( pathandfile, 'r') 
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))
datalist2 = loadtxt(pathandfile2,delimiter=',',usecols=tuple(np.arange(97)))
#create 

time = datalist[:,0]/1000
time2 = datalist2[:,0]/1000

data = datalist[:,1:]/2000
print (data)
i=0
datatotrain = data[i,:]
target = [0]
expected = []

i=0
for timeaux in time:
  '''if (timeaux<1.5):
    datatotrain = np.vstack((datatotrain , datalist[i,1:]))
    target = np.concatenate((target,[0]),axis=0)'''
    
  if ((timeaux>6.3) and (timeaux<6.7)):
    datatotrain = np.vstack((datatotrain , data[i,:]))
    target = np.concatenate((target,[1]),axis=0)  

  if ((timeaux>7.5) and (timeaux<8)):
    datatotrain = np.vstack((datatotrain , data[i,:]))
    target = np.concatenate((target,[2]),axis=0)      
  
  if (timeaux<5.9):
    expected = np.concatenate((expected,[0]),axis=0)
    
  if ((timeaux>5.9) and (timeaux<6.7)):
    expected = np.concatenate((expected,[1]),axis=0) 
        
  if ((timeaux>6.7) and (timeaux<10)):
    expected = np.concatenate((expected,[2]),axis=0)  

  if ((timeaux>10) and (timeaux<10.7)):
    expected = np.concatenate((expected,[1]),axis=0)   
    
  if (timeaux>10.7):
    expected = np.concatenate((expected,[0]),axis=0)        
    
  i=i+1
#######################################################training############################################

print(len(target))


# Create a classifier: a support vector classifier
classifier = svm.SVC(gamma=0.0001)
# We learn from sample
classifier.fit(datatotrain[1:,:], target[1:])


i=0
predicted = []
for dataaux in data:
  if (np.sum(dataaux)/96<0.1):
    predicted = np.concatenate((predicted,[0]),axis=0) 
  else:
    predicted = np.concatenate((predicted,classifier.predict(dataaux)),axis=0)    

plt.figure(1)
plt.plot( time,predicted,'o')
plt.plot( time,expected)

plt.figure(2)
plt.plot( time,np.sum(data,axis=1)/96)
#plt.plot( time2,np.sum(datalist2[:,1:],axis=1)/96)


print("Classification report for classifier %s:\n%s\n" % (classifier, metrics.classification_report(expected, predicted)))
print("Confusion matrix:\n%s" % metrics.confusion_matrix(expected, predicted))

print(len(time))
plt.show()


