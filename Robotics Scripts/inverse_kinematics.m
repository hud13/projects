%   This script implements an iterative kinematics function for the spatial
%   manipulator of Example 5.24
%   It utilizes the fk_spatial_manipulator, bodyJ_spatial_manipulator, and
%   extract_twist_coordinates functions provided in the homework solutions.
%   
%   Constants of manipulator: d = 0, a2 = 3, a3 = 3
%
%   Inputs: 
%   3-by-1 array of current joint angles
%   Desired pose of frame n, 0Tn
%
%   Author: Hudson Dalby
%   Date: 11/22/2025

function theta_next = inverse_kinematics(theta_current, T_desired)

% Ensures theta_current is a column vector
theta_current = theta_current(:);

% Extracts each joint angle to use in fk function call
th1 = theta_current(1);
th2 = theta_current(2);
th3 = theta_current(3);

% Gets the current pose of spatial manipulator 
T_current = fk_spatial_manipulator(th1,th2,th3);

% Solve for twist as written in (10.43) 
T_delta = T_desired - T_current;
twist = inv(T_current) * T_delta;

% Extract the twist coordinates from the calculated twist
xi = extract_twist_coordinates(twist);

% Find the body Jacobian at current location. 
Jb = bodyJ_spatial_manipulator(theta_current);

% Multiply pseudoinverse of jacobian with calculated twist
theta_delta = pinv(Jb) * xi; 

% Approximation of next theta as written in (10.43)
theta_next = theta_current + theta_delta; 

