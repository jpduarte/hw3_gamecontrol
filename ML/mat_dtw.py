#clasify data for different position in mat, with data already trained
#Juan Duarte,CS298 fall2016
import numpy as np
import matplotlib.pyplot as plt
from numpy import loadtxt

def update_data_dtw(old_data_dtw,new_data_sum,min_len):
    data_dtw_sum = old_data_dtw[1:]
    data_dtw_sum = np.concatenate((data_dtw_sum,[new_data_sum]))
    return data_dtw_sum
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
#parameters
pathandfile = '../matplot/matdraw/josh_v1.txt'
pathandfile_basis = './basis3steps_josh_v1.txt'
pathandfile_mean = './mean3steps_josh_v1.txt'
pathandfile_cluster = './cluster3steps_josh_v1.txt'
bounds_sum = [[39,42],[43,46]]
bounds_proj0 = bounds_sum
bounds_proj1 = bounds_sum
bounds_proj2 = bounds_sum
smooth_order = 5
distance_weights = [1.0,0,0,0]
threshold_min=15000
threshold_min_ave=15000 #set a minimun for smoothing
distance_weigths = [0.0,1.0,0.0,0.0] #total sum, projection 1, 2 and 3
distance_order = 4.0
threshold_detection = 1.6e-15**4

#initialization
poly1d_train_sum = np.poly1d([0,0,0,0])
poly1d_train_proj0 = np.poly1d([0,0,0,0])
poly1d_train_proj1 = np.poly1d([0,0,0,0])
poly1d_train_proj2 = np.poly1d([0,0,0,0])
###################################################################################################################################    load data
#pathandfile = '../matplot/matdraw/feethandtrain.txt'
#pathandfile = '../matplot/matdraw/2016_11_26_test.txt'
#pathandfile = '../matplot/matdraw/feettrain.txt'
target = open( pathandfile, 'r')
datalist = loadtxt(pathandfile,delimiter=',',usecols=tuple(np.arange(97)))
target.close()
time = datalist[:,0]/1000

target = open( pathandfile_basis, 'r')
three_new_basis = loadtxt(pathandfile_basis,delimiter=',')
target.close()

target = open( pathandfile_mean, 'r')
three_mean = loadtxt(pathandfile_mean,delimiter=',')
target.close()

target = open( pathandfile_cluster, 'r')
centroid_list = loadtxt(pathandfile_cluster,delimiter=',')
target.close()
clusterindex = []
dataall = datalist[:,1:]
plt.figure(1)
plt.plot( time,np.sum(datalist[:,1:],axis=1),'o')
################################################################################## calculate all projections
three_classified_0 = []
three_classified_1 = []
three_classified_2 = []
i=0
for data in dataall[:len(dataall)//1]:
  if (np.sum(data)<threshold_min):
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
###################################################################################select data for dynamic time Warping, and training
plt.figure(2)
if(len(bounds_sum)>0):
    squad_sample = selecttraindata(time,datalist[:,1:],bounds_sum)
    min_len = np.inf
    for sample in squad_sample:
        average_wave = np.sum(sample,axis=1)
        average_sample =  average_wave.mean()
        plt.plot( range(len(sample)),average_wave-average_sample,'o')
        min_len_aux = len(average_wave)
        if (min_len_aux<min_len):
            min_len=min_len_aux
    add_all = np.zeros(min_len)
    i=0
    for sample in squad_sample:
        average_wave = np.sum(sample,axis=1)
        add_all = add_all + average_wave[0:min_len]
        i=i+1
    add_all = add_all/len(squad_sample)
    plt.plot( range(len(add_all)),add_all-add_all.mean(),'-')

    data_to_filter = add_all-add_all.mean()
    time_fit = np.arange(len(add_all),dtype=float)
    poly1d_train_sum = np.poly1d(np.polyfit(time_fit, data_to_filter, 3))
    y = poly1d_train_sum(time_fit)
    plt.plot(time_fit,y,'--')
    time_fit_new= time_fit*2
    y=poly1d_train_sum(time_fit_new)
    plt.plot(time_fit[:len(time_fit)//2],y[:len(y)//2],'-.')
plt.figure(3)
if(len(bounds_proj0)>0):
    squad_sample = selecttraindata1D(time,three_classified_0,bounds_proj0)
    min_len = np.inf
    for sample in squad_sample:
        average_wave = sample
        average_sample =  np.sum(sample)/len(sample)
        plt.plot( range(len(sample)),average_wave-average_sample,'o')
        min_len_aux = len(average_wave)
        if (min_len_aux<min_len):
            min_len=min_len_aux
    add_all = np.zeros(min_len)
    i=0
    #get average wave from all samples
    for sample in squad_sample:
        average_wave = sample
        add_all = add_all + average_wave[0:min_len]
        i=i+1
    add_all = add_all/len(squad_sample)
    plt.plot( range(len(add_all)),add_all-add_all.mean(),'-')

    data_to_filter = add_all-add_all.mean()
    time_fit = np.arange(len(add_all),dtype=float)
    poly1d_train_proj0 = np.poly1d(np.polyfit(time_fit, data_to_filter, 3))
    y = poly1d_train_proj0(time_fit)
    plt.plot(time_fit,y,'--')
    time_fit_new= time_fit*2
    y=poly1d_train_proj0(time_fit_new)
    plt.plot(time_fit[:len(time_fit)//2],y[:len(y)//2],'-.')
plt.figure(4)
if(len(bounds_proj1)>0):
    squad_sample = selecttraindata1D(time,three_classified_1,bounds_proj1)
    min_len = np.inf
    for sample in squad_sample:
        average_wave = sample
        average_sample =  np.sum(sample)/len(sample)
        plt.plot( range(len(sample)),average_wave-average_sample,'o')
        min_len_aux = len(average_wave)
        if (min_len_aux<min_len):
            min_len=min_len_aux
    add_all = np.zeros(min_len)
    i=0
    #get average wave from all samples
    for sample in squad_sample:
        average_wave = sample
        add_all = add_all + average_wave[0:min_len]
        i=i+1
    add_all = add_all/len(squad_sample)
    plt.plot( range(len(add_all)),add_all-add_all.mean(),'-')

    data_to_filter = add_all-add_all.mean()
    time_fit = np.arange(len(add_all),dtype=float)
    poly1d_train_proj1 = np.poly1d(np.polyfit(time_fit, data_to_filter, 3))
    y = poly1d_train_proj1(time_fit)
    plt.plot(time_fit,y,'--')
    time_fit_new= time_fit*2
    y=poly1d_train_proj1(time_fit_new)
    plt.plot(time_fit[:len(time_fit)//2],y[:len(y)//2],'-.')
plt.figure(5)
if(len(bounds_proj2)>0):
    squad_sample = selecttraindata1D(time,three_classified_2,bounds_proj1)
    min_len = np.inf
    for sample in squad_sample:
        average_wave = sample
        average_sample =  np.sum(sample)/len(sample)
        plt.plot( range(len(sample)),average_wave-average_sample,'o')
        min_len_aux = len(average_wave)
        if (min_len_aux<min_len):
            min_len=min_len_aux
    add_all = np.zeros(min_len)
    i=0
    #get average wave from all samples
    for sample in squad_sample:
        average_wave = sample
        add_all = add_all + average_wave[0:min_len]
        i=i+1
    add_all = add_all/len(squad_sample)
    plt.plot( range(len(add_all)),add_all-add_all.mean(),'-')

    data_to_filter = add_all-add_all.mean()
    time_fit = np.arange(len(add_all),dtype=float)
    poly1d_train_proj2 = np.poly1d(np.polyfit(time_fit, data_to_filter, 3))
    y = poly1d_train_proj2(time_fit)
    plt.plot(time_fit,y,'--')
    time_fit_new= time_fit*2
    y=poly1d_train_proj2(time_fit_new)
    plt.plot(time_fit[:len(time_fit)//2],y[:len(y)//2],'-.')
#################################################################################  Perform PCA and centroid classification
data_dtw_sum = np.zeros(min_len)
data_dtw_proj0 = np.zeros(min_len)
data_dtw_proj1 = np.zeros(min_len)
data_dtw_proj2 = np.zeros(min_len)
data_dtw_reference_sum = poly1d_train_sum(range(min_len))
data_dtw_reference_proj0 = poly1d_train_proj0(range(min_len))
data_dtw_reference_proj1 = poly1d_train_proj1(range(min_len))
data_dtw_reference_proj2 = poly1d_train_proj2(range(min_len))

distance_all = []
i=-1
for data in dataall[:len(dataall)//1]:
  i=i+1
  new_data_aux_sum = np.sum(data)
  new_data_aux_proj0 = three_classified_0[i]
  new_data_aux_proj1 = three_classified_1[i]
  new_data_aux_proj2 = three_classified_2[i]
  if ((np.sum(data)>threshold_min_ave) and (smooth_order>1)):
      new_data_sum = (np.sum(data_dtw_sum[-(smooth_order-1):])+new_data_aux_sum)/smooth_order
      new_data_proj0 = (np.sum(data_dtw_proj0[-(smooth_order-1):])+new_data_aux_proj0)/smooth_order
      new_data_proj1 = (np.sum(data_dtw_proj1[-(smooth_order-1):])+new_data_aux_proj1)/smooth_order
      new_data_proj2 =(np.sum(data_dtw_proj2[-(smooth_order-1):])+new_data_aux_proj2)/smooth_order
  else:
      new_data_sum = new_data_aux_sum
      new_data_proj0 = new_data_aux_proj0
      new_data_proj1 = new_data_aux_proj1
      new_data_proj2 = new_data_aux_proj2

  #update FIFO variables
  data_dtw_sum = update_data_dtw(data_dtw_sum,new_data_sum,min_len)
  data_dtw_proj0 = update_data_dtw(data_dtw_proj0,new_data_proj0,min_len)
  data_dtw_proj1 = update_data_dtw(data_dtw_proj1,new_data_proj1,min_len)
  data_dtw_proj2 = update_data_dtw(data_dtw_proj2,new_data_proj2,min_len)
  #calculate distances
  dist_sum = (np.linalg.norm(data_dtw_reference_sum-(data_dtw_sum-data_dtw_sum.mean()),ord=distance_order))**distance_order
  dist_proj0 = (np.linalg.norm(data_dtw_reference_proj0-(data_dtw_proj0-data_dtw_proj0.mean()),ord=distance_order))**distance_order
  dist_proj1 = (np.linalg.norm(data_dtw_reference_proj1-(data_dtw_proj1-data_dtw_proj1.mean()),ord=distance_order))**distance_order
  dist_proj2 = (np.linalg.norm(data_dtw_reference_proj2-(data_dtw_proj2-data_dtw_proj2.mean()),ord=distance_order))**distance_order
  #final distance with wieght
  dist = dist_sum*distance_weights[0]+dist_proj0*distance_weights[1]+dist_proj1*distance_weights[2]+dist_proj2*distance_weights[3]
  distance_all.append(1/dist)
  #print(new_data_aux_proj0,new_data_aux_proj1,new_data_aux_proj2 )
  if ((1/dist)>(threshold_detection)**(1/distance_order) and i>min_len):
    plt.figure(1)
    #print(data_dtw_reference_sum-(data_dtw_sum-data_dtw_sum.mean()),data_dtw_reference_proj0-(data_dtw_proj0-data_dtw_proj0.mean()))
    plt.plot( time[i-min_len:i],(data_dtw_reference_sum+data_dtw_sum.mean()),'s')
    plt.plot( time[i-min_len:i],data_dtw_sum,'*')

plt.figure(6)
plt.plot( time[:len(three_classified_0)],three_classified_0,'o',color='k')
plt.plot(  time[:len(three_classified_0)],three_classified_1,'o',color='r')
plt.plot(  time[:len(three_classified_0)],three_classified_2,'o',color='b')
plt.figure(7)
plt.plot(  time[:len(three_classified_0)],clusterindex,'o')
plt.figure(8)
plt.plot(  time[:len(three_classified_0)],distance_all,'o')
plt.figure(9)
plt.plot( range(min_len),data_dtw_reference_sum,'o')

print(poly1d_train_sum,poly1d_train_proj0,poly1d_train_proj1,poly1d_train_proj2)
print(min_len)
plt.show()
