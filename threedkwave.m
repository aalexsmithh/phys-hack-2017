% create the computational grid
Nx = 256;            % number of grid points in the x direction
Ny = 256;            % number of grid points in the y direction
Nz = 256;            % number of grid points in the z direction
sensor_step = 2;    % discrete steps between sensor points
dx = 0.1e-2;        % grid point spacing in the x direction [m]
dy = 0.1e-2;        % grid point spacing in the y direction [m]
dz = 0.1e-2;        % grid point spacing in the z direction [m]

dt = 5e-7; % [s]
t_end = 0.00001; % [s]

kgrid = kWaveGrid(Nx, dx, Ny, dy, Nz, dz);
kgrid.setTime(round(t_end / dt)+1, dt);
%kgrid.setTime(100, kgrid.dt);

% define the properties of the propagation medium
medium.sound_speed = 343 * ones(Nx, Ny, Nz);
%medium.sound_speed(25:35,25:35, 25:35) = 343;
medium.density = 1.225 * ones(Nx,Ny, Nz);
%medium.density(25:35,25:35,25:35) = 600;
%medium.density(27:32,27:33,27:33) = 1.225;
medium.alpha_coeff = 0.75;
medium.alpha_power = 1.5;

% create initial pressure distribution using makeBall
ball_magnitude = 10;    % [Pa]
ball_x_pos = 5;        % [grid points]
ball_y_pos = 5;        % [grid points]
ball_z_pos = 5;        % [grid points]
ball_radius = 2;        % [grid points]
ball_1 = ball_magnitude * makeBall(Nx, Ny, Nz, ball_x_pos, ball_y_pos, ball_z_pos, ball_radius);

% ball_magnitude = 10;    % [Pa]
% ball_x_pos = 20;        % [grid points]
% ball_y_pos = 20;        % [grid points]
% ball_z_pos = 20;        % [grid points]
% ball_radius = 3;        % [grid points]
% ball_2 = ball_magnitude * makeBall(Nx, Ny, Nz, ball_x_pos, ball_y_pos, ball_z_pos, ball_radius);

source.p0 = ball_1; %+ ball_2;

% define a series of Cartesian points to collect the data
sensor.mask = ones(Nx, Ny, Nz);
% x = (-(Nx/2)+10:sensor_step:(Nx/2)-10) * dx;          % [m]
% y = ((Ny/2)-10) * dy * ones(size(x));                % [m]
% z = (-(Nz/2)+10:sensor_step:(Nz/2)-10) * dz;          % [m]
% sensor.mask = [x; y; z];

% input arguments
input_args = {'PlotLayout', false, 'PlotPML', false, ...
    'DataCast', 'gpuArray-single', 'CartInterp', 'nearest', ...
    'PMLInside', true, 'RecordMovie', false, 'PMLAlpha', 1e10, 'PlotFreq', 1, 'PlotSim', false, 'DataRecast', true};

% run the simulation
sensor_data = kspaceFirstOrder3D(kgrid, medium, source, sensor, input_args{:});
fid = fopen('big-data.bin', 'w');
fwrite(fid, size(sensor_data), 'integer*4');
fwrite(fid, sensor_data, 'float');
fclose(fid);
% save('data.mat', 'sensor_data', '-v6');
