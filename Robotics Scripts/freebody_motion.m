
%   This script simulates the motion of a free-floating body in
%   microgravity with no applied wrench. It uses Newton-Euler equations to
%   calculate the 6-DOF acceleration of the body, numerically integrates
%   forward in time to calculate the 6-DOF velocity of the body, and
%   updates pose accordingly. 
%
%   The following figures are generated: (in coordinates of static frame)
%   1 - Three components of the bodys position
%   2 - Three components of the body's velocity
%   3 - Four components of the quaternion representing the orientation
%   4 - Three components of the angular-velocity vector 
%   5 - Three componenets of the angular-momentum vector
%
%   Author: Hudson Dalby
%   Date: 12/5/25

% Physical parameters
m = 5;
Ib = diag([10, 6, 2]);

% Simulation parameters
dt = 0.0001;
time = 10;
n = round(time/dt);
t = linspace(0, time, n);

% Given body translational and angular velocities (spatial frame)

% Part 1
% vs0 = [2; 3; 4];
% ws0 = [10; 0; 0];

% Part 2
vs0 = [2; 3; 4];
ws0 = [2; 2; 0];

% Initial position of body origin 
ds0 = [0; 0; 0];

% Rotation matrix initial
R0 = eye(3);

% Initialize variables
ds = zeros(3,n);
vs = zeros(3,n);
ws  = zeros(3,n);
Ls  = zeros(3,n);
Rsb = zeros(3,3,n);
q   = zeros(4,n);

ds(:,1)    = ds0;
vs(:,1)    = vs0;
ws(:,1)     = ws0;
Rsb(:,:,1)  = R0;
q(:,1)      = rot_to_quat(R0);

% Time integration 
for k = 1:n-1

    % Solve using Euler's equation (0 wrench, 0 net force, 0 net torque)

    % Compute inertia in static frame
    Rk = Rsb(:,:,k);
    Is = Rk * Ib * Rk.';

    % Compute angular momentum in static frame 
    Ls(:,k) = Is * ws(:,k);

    % Angular acceleration
    ws_dot = -Is \ cross(ws(:,k), Ls(:,k));

    % Translational acceleration 
    vs_dot = [0; 0; 0];

    % Integrate angular and linear velocity 
    ws(:,k+1) = ws(:,k) + ws_dot * dt;
    vs(:,k+1) = vs(:,k) + vs_dot * dt;

    % Integrate position
    ds(:,k+1) = ds(:,k) + vs(:,k) * dt;

    % Integrate orientation using angular velocity 
    w = ws(:,k);
    s = make_skew(w);
    R_dot = expm(s * dt);
    Rsb(:,:,k+1) = R_dot * Rk;
end

% Find quaternion from rotation matrix
for k = 2:n
    q(:,k) = rot_to_quat(Rsb(:,:,k));
end 
    
% Generate Plots 

% 1 - Position components
figure(1);
plot(t, ds(1,:), 'LineWidth', 1.5); hold on;
plot(t, ds(2,:), 'LineWidth', 1.5);
plot(t, ds(3,:), 'LineWidth', 1.5);
grid on;
xlabel('Time (s)');
ylabel('Position (m)');
legend('x','y','z');
title('Body Origin Position in Static Frame');

% 2 - Velocity components
figure(2);
plot(t, vs(1,:), 'LineWidth', 1.5); hold on;
plot(t, vs(2,:), 'LineWidth', 1.5);
plot(t, vs(3,:), 'LineWidth', 1.5);
grid on;
xlabel('Time (s)');
ylabel('Velocity (m/s)');
legend('x','y','z');
title('Body Origin Velocity in Static Frame');

% 3 - Quaternion components
figure(3);
plot(t, q(1,:), 'LineWidth', 1.5); hold on;
plot(t, q(2,:), 'LineWidth', 1.5);
plot(t, q(3,:), 'LineWidth', 1.5);
plot(t, q(4,:), 'LineWidth', 1.5);
grid on;
xlabel('Time (s)');
ylabel('Quaternion components');
legend('q_w','q_x','q_y','q_z');
title('Quaternion Representation');

% 4 - Angular velocity components
figure(4);
plot(t, ws(1,:), 'LineWidth', 1.5); hold on;
plot(t, ws(2,:), 'LineWidth', 1.5);
plot(t, ws(3,:), 'LineWidth', 1.5);
grid on;
xlabel('Time (s)');
ylabel('Angular velocity (rad/s)');
legend('\omega_x','\omega_y','\omega_z');
title('Angular Velocity');

% 5) Angular momentum components 
figure(5);
plot(t, Ls(1,:), 'LineWidth', 1.5); hold on;
plot(t, Ls(2,:), 'LineWidth', 1.5);
plot(t, Ls(3,:), 'LineWidth', 1.5);
grid on;
xlabel('Time (s)');
ylabel('Angular momentum (kg * m^2)');
legend('L_x', 'L_y', 'L_z');
title('Angular Momentum');


% Rotation matrix to quaternion (2.57)
function q = rot_to_quat(R)

    r11 = R(1,1); r12 = R(1,2); r13 = R(1,3);
    r21 = R(2,1); r22 = R(2,2); r23 = R(2,3);
    r31 = R(3,1); r32 = R(3,2); r33 = R(3,3);
    
    % Compute the squared elements 
    q0_sq = 0.25 * (1 + r11 + r22 + r33);
    q1_sq = 0.25 * (1 + r11 - r22 - r33);
    q2_sq = 0.25 * (1 - r11 + r22 - r33);
    q3_sq = 0.25 * (1 - r11 - r22 + r33);
    
    % Pick largest qi^2 to solve for other parts
    [~, idx] = max([q0_sq, q1_sq, q2_sq, q3_sq]);
    
    % Solve remaining parts depending on largest qi^2
    switch idx
        case 1      % q0^2 is the largest
            q0 = sqrt(q0_sq);
            if q0 > 1e-12
                q1 = (r32 - r23) / (q0*4);
                q2 = (r13 - r31) / (q0*4);
                q3 = (r21 - r12) / (q0*4);
            else
                q1 = 0; q2 = 0; q3 = 0;
            end 
    
        case 2      % q1^2 is the largest
            q1 = sqrt(q1_sq);
            if q1 > 1e-12
                q0 = (r32 - r23) / (q1*4);
                q2 = (r12 + r21) / (q1*4);
                q3 = (r13 + r31) / (q1*4);
            else
                q1 = 0; q2 = 0; q3 = 0;
            end 
    
        case 3     % q2^2 is the largest 
            q2 = sqrt(q2_sq);
            if q2 > 1e-12
                q0 = (r13 - r31) / (q2*4);
                q1 = (r12 + r21) / (q2*4);
                q3 = (r23 + r32) / (q2*4);
            else
                q1 = 0; q2 = 0; q3 = 0;
            end 
    
        case 4      % q3^2 is the largest
            q3 = sqrt(q3_sq);
            if q3 > 1e-12
                q0 = (r21 - r12) / (q3*4);
                q1 = (r13 + r31) / (q3*4);
                q2 = (r23 + r32) / (q3*4);
            else
                q1 = 0; q2 = 0; q3 = 0;
            end 
    end
    
    % Take positive q0 values 
    if q0 < 0
        q0 = -q0; q1 = -q1; q2 = -q2; q3 = -q3;
    end
    
    % Normalize quaternion
    q = [q0; q1; q2; q3];
    q = q / norm(q);
end

