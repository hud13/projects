# This is a function that implements the Angle-Axis representation mentioned in section 2.3.1
# It takes inputs of a rotation matrix
# Returns K and theta
#   
# Author: Hudson Dalby
# Date: 9/4/2025

import numpy as np

def Axis_angle(R):

    # Validates the input rotation matrix
    R = np.asarray(R, dtype=float)
    assert R.shape == (3,3)
    
    # Calculates trace and solves for cos theta (2.36)
    Tr = np.trace(R)
    cost = (Tr - 1)/2

    # Estimates sin theta by adding diagonal cos of theta (2.38)
    diag = np.array([R[2,1] - R[1,2],
                    R[0,2] - R[2,0],
                    R[1,0] - R[0,1]])
    sint = 0.5 * np.linalg.norm(diag)

    # arctan function to solve for theta
    theta = np.arctan2(sint, cost)

    # Sets cuttof for double point precision rounding
    cutoff = 1e-10

    # Sets theta to the cutoff amount to avoid undefined point at 0
    if sint < cutoff:
        print("k is undefined at this point, divide by zero occurred")
        return None, theta

    k = diag / (2.0 * sint)
    k = k / np.linalg.norm(k)
    print("k:", k)
    print("theta: ", np.rad2deg(theta))
    return k, theta


test1 = np.array([[ 1.,         0.,         0. ,      ],
                 [ 0.,        -0.8660254, -0.5,      ],
                 [ 0.,         0.5 ,      -0.8660254]])

test2 = np.array([[ 1.,  0.,  0.],
                 [ 0., -1.,  0.],
                 [ 0., -0., -1.]])

test3 = np.array([[ 1.,  0. , 0.],
                [ 0.,  0.,  1.],
                [ 0., -1.,  0.]])

test4 = np.array([[ 1.,          0. ,         0.        ],
                [ 0.,          0.70710678, 0.70710678],
                 [ 0.,         -0.70710678,  0.70710678]])

test5 = np.array([[1., 0., 0.],
                 [0., 1., 0.],
                 [0., 0., 1.]])

test6 = np.array([[ 1.,          0.,          0.        ],
                 [ 0. ,         0.70710678, -0.70710678],
                 [ 0.,          0.70710678,  0.70710678]])

test7 = np.array([[ 1.,  0.,  0.],
                 [ 0.,  0., -1.],
                [ 0.,  1.,  0.]])

test8 = np.array([[ 1.,  0.,  0.],
                 [ 0., -1., -0.],
                 [ 0.,  0., -1.]])

test9 = np.array([[ 1. ,        0.,         0.,       ],
                 [ 0.,        -0.8660254,  0.5      ],
                 [ 0. ,       -0.5 ,      -0.8660254]])

Axis_angle(test1)
Axis_angle(test2)
Axis_angle(test3)
Axis_angle(test4)
Axis_angle(test5)
Axis_angle(test6)
Axis_angle(test7)
Axis_angle(test8)
Axis_angle(test9)