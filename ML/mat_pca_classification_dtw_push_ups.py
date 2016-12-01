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
from scipy.signal import savgol_filter
from math import factorial
from numpy import mgrid, sum
from image_fx import moments_central,moments_normalized,moments_hu,moments2e

from dtw_v1 import dtw

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

def selecttraindata(time,data,bounds):
  datatotrain = [None] * len(bounds)
  i=0
  for bound in bounds:
    firstelement=0 #flag for first element
    j=0
    for t in time:
      if ((t>bound[0]) and (t<bound[1])):
        if(firstelement==0):
          datatotrain[i] = data[j,:]
          firstelement=1
        else:
          datatotrain[i] = np.vstack((datatotrain[i] , data[j,:]))
      j=j+1
    i=i+1
  return datatotrain

def selecttraindata1D(time,data,bounds):
  datatotrain = [None] * len(bounds)
  i=0
  for bound in bounds:
    firstelement=0 #flag for first element
    j=0
    for t in time:
      if ((t>bound[0]) and (t<bound[1])):
        if(firstelement==0):
          datatotrain[i] = [data[j]]
          firstelement=1
        else:
          datatotrain[i].append(data[j])
      j=j+1
    i=i+1
  return datatotrain
###################################################################################################################################

###################################################################################################################################    load data
#pathandfile = '../matplot/matdraw/feethandtrain.txt'
pathandfile = '../matplot/matdraw/2016_11_26_test.txt'
#pathandfile = '../matplot/matdraw/feettrain.txt'
target = open( pathandfile, 'r')
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))
target.close()
time = datalist[:,0]/1000


plotelement = []

#plt.figure(1)
#plt.plot( time/60,np.sum(datalist[:,1:],axis=1),'o')

plt.figure(2)
plt.plot( time,np.sum(datalist[:,1:],axis=1),'o')
###################select data for dynamic time Warping
bounds_squad = [[52.8,55],[56.2,58.6],[60.2,62.8]]
squad_sample = selecttraindata(time,datalist[:,1:],bounds_squad)

plt.figure(5)
min_len = np.inf

for sample in squad_sample:
    average_wave = np.sum(sample,axis=1)
    average_sample =  average_wave.mean()
    plt.plot( range(len(sample)),average_wave-average_sample,'o')
    min_len_aux = len(average_wave)
    if (min_len_aux<min_len):
        min_len=min_len_aux

add_all = np.zeros(min_len)
print(len(add_all))
i=0
for sample in squad_sample:
    average_wave = np.sum(sample,axis=1)
    print(len(average_wave))
    add_all = add_all + average_wave[0:min_len]
    i=i+1
add_all = add_all/3
plt.plot( range(len(add_all)),add_all-add_all.mean(),'-')

data_to_filter = add_all-add_all.mean()
time_fit = np.arange(len(add_all),dtype=float)
z = np.poly1d(np.polyfit(time_fit, data_to_filter, 2))
y = z(time_fit)
plt.plot(time_fit,y,'--')
print(time_fit)
time_fit_new= time_fit*2
print(time_fit_new)
y=z(time_fit_new)
plt.plot(time_fit[:len(time_fit)//2],y[:len(y)//2],'-.')
#yhat = savgol_filter(data_to_filter, (len(data_to_filter) // 2) * 2-1, 3)
#plt.plot( range(len(add_all)),yhat,'-')


###################################################################################################################################  Perform PCA and centroid classification

pathandfile = './basis3steps_v3.txt'
target = open( pathandfile, 'r')
three_new_basis = loadtxt(pathandfile,delimiter=',')
target.close()

pathandfile = './mean3steps_v3.txt'
target = open( pathandfile, 'r')
three_mean = loadtxt(pathandfile,delimiter=',')
target.close()


pathandfile = './cluster3steps_v3.txt'
target = open( pathandfile, 'r')
centroid_list = loadtxt(pathandfile,delimiter=',')
target.close()

clusterindex = []
dataall = datalist[:,1:]



data_dtw = np.zeros(min_len)
from sklearn.metrics.pairwise import euclidean_distances
data_dtw_reference = z(range(min_len))

std_array = []

def update_data_dtw(old_data_dtw,new_data,min_len):
    data_dtw = old_data_dtw[1:]
    data_dtw = np.concatenate((data_dtw,[new_data]))
    return data_dtw
'''
distance_all = []
three_classified_0 = []
three_classified_1 = []
three_classified_2 = []
i=0
for data in dataall[:len(dataall)//1]:

  if (np.sum(data)<42000):
    clusterindex = np.concatenate((clusterindex,[-1]),axis=0)
    three_classified_0.append(0)
    three_classified_1.append(0)
    three_classified_2.append(0)
  else:
    three_classified = PCA_classify(data, three_new_basis, three_mean)
    clusterindex = np.concatenate((clusterindex,[which_cluster(three_classified, centroid_list)]),axis=0)
    three_classified_0.append(three_classified[0])
    three_classified_1.append(three_classified[1])
    three_classified_2.append(three_classified[2])

  i=i+1

  new_data_aux = np.sum(data)
  if (np.sum(data)>50000):
      new_data = (np.sum(data_dtw[-4:])+new_data_aux)/5
  else:
      new_data = new_data_aux
  data_dtw = update_data_dtw(data_dtw,new_data,min_len)
  std_array.append(np.std(data_dtw))
  #dist = dtw(data_dtw, data_dtw_reference, euclidean_distances) #, cost, acc, path
  dist = np.linalg.norm(data_dtw_reference-(data_dtw-data_dtw.mean()),ord=4)
  distance_all.append(1/dist)


  if ((1/dist)>2.2e-4 and i>min_len):
    print("Time: ",time[i])
    plt.figure(2)
    #print(new_data_aux,data_dtw,i,len(time[i-min_len:i]),len((data_dtw_reference+data_dtw.mean())))
    plt.plot( time[i-min_len:i],(data_dtw_reference+data_dtw.mean()),'s')
    plt.plot( time[i-min_len:i],data_dtw,'*')


##########################
plt.figure(3)
plt.plot( time[:len(three_classified_0)],three_classified_0,'o',color='k')
plt.plot(  time[:len(three_classified_0)],three_classified_1,'o',color='r')
plt.plot(  time[:len(three_classified_0)],three_classified_2,'o',color='b')
plt.figure(4)
plt.plot(  time[:len(three_classified_0)],clusterindex,'o')

plt.figure(6)
plt.plot(  time[:len(three_classified_0)],distance_all,'o')


plt.figure(7)
plt.plot( range(min_len),data_dtw_reference,'o')

plt.figure(8)
plt.plot(  time[:len(three_classified_0)],std_array,'o')
'''

##############33
dataall = datalist[:,1:]
three_classified_0 = []
three_classified_1 = []
three_classified_2 = []


for data in dataall:
  if (np.sum(data)<42000):

    three_classified_0.append(0)
    three_classified_1.append(0)
    three_classified_2.append(0)
  else:
    three_classified = PCA_classify(data, three_new_basis, three_mean)
    three_classified_0.append(three_classified[0])
    three_classified_1.append(three_classified[1])
    three_classified_2.append(three_classified[2])
plt.figure(3)
plt.plot( time[:len(three_classified_0)],three_classified_0,'o',color='k')
#step 1: gather training data
bounds_pushup = [[198,200.5],[202.5,205.9],[212,215]]
pushup_sample = selecttraindata1D(time,three_classified_0,bounds_pushup)

plt.figure(9)
min_len = np.inf

for sample in pushup_sample:
    average_wave = np.asarray(sample)
    average_sample =  average_wave.mean()
    plt.plot( range(len(sample)),average_wave-average_sample,'o')
    min_len_aux = len(average_wave)
    if (min_len_aux<min_len):
        min_len=min_len_aux

min_len=44
add_all = np.zeros(min_len)
print(len(add_all))
i=0
for sample in pushup_sample:
    average_wave = np.asarray(sample)
    print(len(average_wave))
    add_all = add_all + average_wave[0:min_len]
    i=i+1
add_all = add_all/3
plt.plot( range(len(add_all)),add_all-add_all.mean(),'-')

data_to_filter = add_all-add_all.mean()
time_fit = np.arange(len(add_all),dtype=float)
z = np.poly1d(np.polyfit(time_fit, data_to_filter, 3))
y = z(time_fit)
plt.plot(time_fit,y,'--')
print(time_fit)
time_fit_new= time_fit*2
print(time_fit_new)
y=z(time_fit_new)
plt.plot(time_fit[:len(time_fit)//2],y[:len(y)//2],'-.')


data_dtw = np.zeros(min_len)
data_dtw_reference = z(range(min_len))
distance_all = []
i=0
for three_classified_0_aux in three_classified_0:
    new_data = (np.sum(data_dtw[-4:])+three_classified_0_aux)/5
    data_dtw = update_data_dtw(data_dtw,new_data,min_len)
    std_array.append(np.std(data_dtw))
    dist = np.linalg.norm(data_dtw_reference-(data_dtw-data_dtw.mean()),ord=4)
    distance_all.append(1/dist)


    if ((1/dist)>3.4e-3 and i>min_len):
       print("Time: ",time[i])
       plt.figure(3)
       #print(new_data_aux,data_dtw,i,len(time[i-min_len:i]),len((data_dtw_reference+data_dtw.mean())))
       plt.plot( time[i-min_len:i],(data_dtw_reference+data_dtw.mean()),'s')
       plt.plot( time[i-min_len:i],data_dtw,'*')
    i=i+1
plt.figure(6)
plt.plot(  time[:len(three_classified_0)],distance_all,'o')

print(z)
print(min_len)
plt.show()
