
%   This script takes the spatial manipulator of Example 5.24 and plans a
%   path in task space using cubic polynomials. 
%
%   Author: Hudson Dalby
%   Date: 11/22/2025

% Given inital and final joint velocities
theta_i = [0; 0; 0];
i1 = theta_i(1);
i2 = theta_i(2);
i3 = theta_i(3);

theta_f = [-pi/4; 0; pi/2];
f1 = theta_f(1);
f2 = theta_f(2);
f3 = theta_f(3);

time = 1;

% Get initial and final end-effector pose
Ti = fk_spatial_manipulator(i1, i2, i3);
Tf = fk_spatial_manipulator(f1, f2, f3);

% Extract components
Ri = Ti(1:3,1:3);
p_i = Ti(1:3,4);

Rf = Tf(1:3,1:3);
p_f = Tf(1:3,4);

% Convert initial and final orientations to k vectors 
rik = R_to_ktheta(Ri);
rfk = R_to_ktheta(Rf);

% Get position components in task space
px = cubic(p_i(1), p_f(1), 0, 0, time);
py = cubic(p_i(2), p_f(2), 0, 0, time);
pz = cubic(p_i(3), p_f(3), 0, 0, time);
p_desired = [px; py; pz];

% Get orientation components in task space
Rx = cubic(rik(1), rfk(1), 0, 0, time);
Ry = cubic(rik(2), rfk(2), 0, 0, time);
Rz = cubic(rik(3), rfk(3), 0, 0, time);
R_desired = [Rx; Ry; Rz];

% Setup plot variables 
N = size(p_desired, 2);
l = linspace(0,time,N);
theta_task = zeros(3, N);
p_task = zeros(3, N);
ktheta_task = zeros(3,N);

% Sets current theta to initial
theta_current = theta_i;

% Loop through the cubic trajectories 
for i = 1:N

    % Gets current loop orientation and rotation 
    p_des = p_desired(:,i);
    r_des = R_desired(:,i);

    % Get skew of rotation 
    R_des = expm(make_skew(r_des));

% Create transformation matrix
T_desired = [R_des, p_des;
             0 0 0 1];

% Call inverse kinematics function for approximation 
theta_current = inverse_kinematics(theta_current, T_desired);
theta_task(:,i) = theta_current;

th1 = theta_current(1);
th2 = theta_current(2);
th3 = theta_current(3);

% Forward kinematics for plotting task trajectory
T_fk = fk_spatial_manipulator(th1,th2,th3);
p_task(:,i) = T_fk(1:3,4);
ktheta_task(:,i) = R_to_ktheta(T_fk(1:3,1:3));

end

% Plot joint angles vs time 
figure;
plot(l, theta_task);
xlabel('Time');
ylabel('theta_i');
legend('theta_1','theta_2','theta_3');

% Plot origin frame position coordinates vs time 
figure;
plot(l, p_task);
xlabel('Time');
ylabel('Frame Position');
legend('x','y','z');

% Plot orientation of frame n vs time
figure;
plot(l, ktheta_task);
xlabel('Time');
ylabel('Orientation '); 
legend('ktheta_x','ktheta_y','ktheta_z');
