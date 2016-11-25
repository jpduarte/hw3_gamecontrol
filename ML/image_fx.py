#from: https://github.com/scikit-image/scikit-image/blob/master/skimage/measure/_moments_cy.pyx
#https://github.com/scikit-image/scikit-image/blob/master/skimage/measure/_moments.py
import numpy as np
from numpy import mgrid, sum

def moments_central(image,cr,cc,order):
    mu = np.zeros((order + 1, order + 1))

    for r in range(image.shape[0]):
        dr = r - cr
        for c in range(image.shape[1]):
            dc = c - cc
            val = image[r, c]
            dcp = 1
            for p in range(order + 1):
                drq = 1
                for q in range(order + 1):
                    mu[p, q] += val * drq * dcp
                    drq *= dr
                dcp *= dc
    return np.asarray(mu)


def moments_normalized(mu, order=3):
    nu = np.zeros((order + 1, order + 1))
    for p in range(order + 1):
        for q in range(order + 1):
            if p + q >= 2:
                nu[p, q] = mu[p, q] / mu[0, 0] ** ((p + q) / 2 + 1)
            else:
                nu[p, q] = np.nan
    return np.asarray(nu)

def moments_hu(nu):
    hu = np.zeros((7, ))
    t0 = nu[3, 0] + nu[1, 2]
    t1 = nu[2, 1] + nu[0, 3]
    q0 = t0 * t0
    q1 = t1 * t1
    n4 = 4 * nu[1, 1]
    s = nu[2, 0] + nu[0, 2]
    d = nu[2, 0] - nu[0, 2]
    hu[0] = s
    hu[1] = d * d + n4 * nu[1, 1]
    hu[3] = q0 + q1
    hu[5] = d * (q0 - q1) + n4 * t0 * t1
    t0 *= q0 - 3 * q1
    t1 *= 3 * q0 - q1
    q0 = nu[3, 0]- 3 * nu[1, 2]
    q1 = 3 * nu[2, 1] - nu[0, 3]
    hu[2] = q0 * q0 + q1 * q1
    hu[4] = q0 * t0 + q1 * t1
    hu[6] = q1 * t0 - q0 * t1
    return np.asarray(hu)


def moments2e(image):
  """
  This function calculates the raw, centered and normalized moments
  for any image passed as a numpy array.
  Further reading:
  https://en.wikipedia.org/wiki/Image_moment
  https://en.wikipedia.org/wiki/Central_moment
  https://en.wikipedia.org/wiki/Moment_(mathematics)
  https://en.wikipedia.org/wiki/Standardized_moment
  http://opencv.willowgarage.com/documentation/cpp/structural_analysis_and_shape_descriptors.html#cv-moments

  compare with:
  import cv2
  cv2.moments(image)
  """
  assert len(image.shape) == 2 # only for grayscale images
  x, y = mgrid[:image.shape[0],:image.shape[1]]
  moments = {}
  moments['mean_x'] = sum(x*image)/sum(image)
  moments['mean_y'] = sum(y*image)/sum(image)

  # raw or spatial moments
  moments['m00'] = sum(image)
  moments['m01'] = sum(x*image)
  moments['m10'] = sum(y*image)
  moments['m11'] = sum(y*x*image)
  moments['m02'] = sum(x**2*image)
  moments['m20'] = sum(y**2*image)
  moments['m12'] = sum(x*y**2*image)
  moments['m21'] = sum(x**2*y*image)
  moments['m03'] = sum(x**3*image)
  moments['m30'] = sum(y**3*image)

  # central moments
  # moments['mu01']= sum((y-moments['mean_y'])*image) # should be 0
  # moments['mu10']= sum((x-moments['mean_x'])*image) # should be 0
  moments['mu11'] = sum((x-moments['mean_x'])*(y-moments['mean_y'])*image)
  moments['mu02'] = sum((y-moments['mean_y'])**2*image) # variance
  moments['mu20'] = sum((x-moments['mean_x'])**2*image) # variance
  moments['mu12'] = sum((x-moments['mean_x'])*(y-moments['mean_y'])**2*image)
  moments['mu21'] = sum((x-moments['mean_x'])**2*(y-moments['mean_y'])*image)
  moments['mu03'] = sum((y-moments['mean_y'])**3*image)
  moments['mu30'] = sum((x-moments['mean_x'])**3*image)


  # opencv versions
  #moments['mu02'] = sum(image*(x-m01/m00)**2)
  #moments['mu02'] = sum(image*(x-y)**2)

  # wiki variations
  #moments['mu02'] = m20 - mean_y*m10
  #moments['mu20'] = m02 - mean_x*m01

  # central standardized or normalized or scale invariant moments
  nu = np.zeros((4,4))
  nu[0,0] = moments['mu11'] / sum(image)**(2/2+1)
  nu[1,2] = moments['mu12'] / sum(image)**(3/2+1)
  nu[2,1] = moments['mu21'] / sum(image)**(3/2+1)
  nu[2,0] = moments['mu20'] / sum(image)**(2/2+1)
  nu[0,1] = moments['mu03'] / sum(image)**(3/2+1) # skewness
  nu[3,0] = moments['mu30'] / sum(image)**(3/2+1) # skewness
  return nu
