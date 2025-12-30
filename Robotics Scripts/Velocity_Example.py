# This function builds the body Jacobian of the robot in Example 5.24
# It uses this velocity to calculate the body velocity
# 
# Input: Three joint angles, Three joint velocities (6 inputs total)
# Output: 6-by-1 body velocity
#
# Author: Hudson Dalby
# Date: 10/25/2025

import numpy as np
from scipy.linalg import expm


# Given Parameters 
a2 = 2
a3 = 2
d = 0

# Zero configuration
zc = np.array([[1, 0, 0, a2 + a3],
              [0, 1, 0, 0],
              [0, 0, 1, d],
              [0, 0, 0, 1]])

# Screw parameters as found in the problem
s1 = np.array([0,0,0,0,1,0])
s2 = np.array([0,0,0,0,0,1])
s3 = np.array([0,-a2,0,0,0,1])

#Helper to find skew-symmetric matrix
def skew(a):
    ax, ay, az = a
    return np.array([[0, -az, ay],
                     [az, 0, -ax],
                     [-ay, ax, 0]])

# Helper to convert to 4x4 twist
def twist(screw):
    v = screw[:3]; r = screw[3:]
    t = np.zeros((4,4))
    t[:3,:3] = skew(r)
    t[:3, 3] = v
    return t

# Converted twist parameters for Jb use
tw1 = twist(s1)
tw2 = twist(s2)
tw3 = twist(s3)

# Helper to find the inverse adjoint matrix
def adjoint_inv(T):
    R = T[:3,:3]
    d = T[:3, 3]
    ad_i = np.zeros((6,6))
    RT = R.T
    ad_i[:3,:3] = RT
    ad_i[3:,3:] = RT
    ad_i[3:,:3] = -RT @ skew(d)
    return ad_i

# Compute body jacobian function
def body_jacobian(th1, th2, th3, jv1, jv2, jv3):

    ex1 = expm(tw1 * th1)
    ex2 = expm(tw2 * th2)
    ex3 = expm(tw3 * th3)

    fk1 = ex1 @ ex2 @ ex3 @ zc
    fk2 = ex2 @ ex3 @ zc
    fk3 = ex3 @ zc

    jacob1 = adjoint_inv(fk1) @ s1
    jacob2 = adjoint_inv(fk2) @ s2
    jacob3 = adjoint_inv(fk3) @ s3

    jacobian = np.column_stack([jacob1, jacob2, jacob3])
    velocity = np.array([jv1, jv2, jv3])

    body_velocity = jacobian @ velocity
    return body_velocity

test1 = body_jacobian(0, 0, 0, 5, 0, 0)
print(test1)

test2 = body_jacobian(0, 0, 0, 0, 5, 0)
print(test2)

test3 = body_jacobian(0, 0, 0, 0, 0, 5)
print(test3)

test4 = body_jacobian(0.1, 0.2, 0.3, 1, 2, 3)
print(test4)
    
