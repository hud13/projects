# This function implements the forward kinematics equation of a SCARA robot (5.22)
# The full parameters of this robot are given in problem 5
# Input: Four joint angles (theta)
# Output: homogeneous transformation matrix
#
# Author: Hudson Dalby
# Date: 9/25/2025

import numpy as np
from scipy.linalg import expm

# Helper method for skew
def skew(m):
     m = np.asarray(m, float)
     return np.array([[0, -m[2], m[1]],
                      [m[2], 0, -m[0]],
                      [-m[1], m[0], 0]])


# Apply forward kinematics of SCARA robot
def forward_kinematics_scara(th1, th2, th3, th4):

    # Link length between joint 1 and 2
    a1 = 1
    
    # Link length between joint 2 and 3
    a2 = 0.9

    # added distances d3 and d4
    d = 1.2

    # Defines zero configuration of spatial manipulator 
    zc = np.asarray([[1, 0, 0, a1 + a2],
                     [0, -1, 0, 0],
                     [0, 0, -1, -d], 
                     [0, 0, 0, 1]])
    
     # Screw matrix of joint 1
    k1 = np.array([0,0,1])
    v1 = np.array([0,0,0])

    screw1 = np.zeros((4,4))
    screw1[:3,:3] = skew(k1)
    screw1[:3, 3] = v1

    # Screw matrix of joint 2
    k2 = np.array([0,0,1])
    v2 = np.array([0,-a1,0])

    screw2 = np.zeros((4,4))
    screw2[:3,:3] = skew(k2)
    screw2[:3, 3] = v2

    # Screw matrix of joint 3
    k3 = np.array([0,0,0])
    v3 = np.array([0,0,-1])

    screw3 = np.zeros((4,4))
    screw3[:3,:3] = skew(k3)
    screw3[:3, 3] = v3

    # Screw matrix of joint 4
    k4 = np.array([0,0,-1])
    v4 = np.array([0,a1 + a2,0])

    screw4 = np.zeros((4,4))
    screw4[:3,:3] = skew(k4)
    screw4[:3, 3] = v4

    # Product of exponentials
    exp1 = expm(screw1 * th1)
    exp2 = expm(screw2 * th2)
    exp3 = expm(screw3 * th3)
    exp4 = expm(screw4 * th4)

    return  exp1 @ exp2 @ exp3 @ exp4 @ zc

t1 = forward_kinematics_scara(0,0,0,0)
print(t1)

t2 = forward_kinematics_scara(np.pi/2,0,0,0)
print(t2)

t3 = forward_kinematics_scara(0,np.pi/2,0,0)
print(t3)

t4 = forward_kinematics_scara(0,0,-0.1,0)
print(t4)

t5 = forward_kinematics_scara(0,0,0,np.pi/2)
print(t5)