sensor_step = 2;    % discrete steps between sensor points
% create the computational grid
Nx = 100;           % number of grid points in the x (row) direction
Ny = 100;           % number of grid points in the y (column) direction
dx = 0.1e-3;        % grid point spacing in the x direction [m]
dy = 0.1e-3;        % grid point spacing in the y direction [m]
kgrid = kWaveGrid(Nx, dx, Ny, dy);

% define the properties of the propagation medium
medium.sound_speed = 343 * ones(Nx, Ny);
medium.sound_speed(25:35,25:35) = 343;
medium.density = 1.225 * ones(Nx,Ny);
medium.density(25:35,25:35) = 600;
medium.density(27:32,27:33) = 1.225;
medium.alpha_coeff = 0.75;
medium.alpha_power = 1.5;


% create initial pressure distribution using makeDisc
disc_magnitude = 5; % [Pa]
disc_x_pos = Nx/2;    % [grid points]
disc_y_pos = Ny*0.25;    % [grid points]
disc_radius = Nx/100;    % [grid points]
disc_1 = disc_magnitude * makeDisc(Nx, Ny, disc_x_pos, disc_y_pos, disc_radius);

disc_magnitude = 3; % [Pa]
disc_x_pos = Nx/2;    % [grid points]
disc_y_pos = Ny*0.75;    % [grid points]
disc_radius = Nx/100;    % [grid points]
disc_2 = disc_magnitude * makeDisc(Nx, Ny, disc_x_pos, disc_y_pos, disc_radius);

source.p0 = disc_1; %+ disc_2;

% define a centered circular sensor
sensor.mask = ones(Nx, Ny)
% sensor_radius = 4e-3;   % [m]
% num_sensor_points = 50;
% sensor.mask = makeCartCircle(sensor_radius, num_sensor_points);

% input arguments
input_args = {'PlotLayout', false, 'PlotPML', false, ...
    'DataCast', 'single', 'CartInterp', 'nearest', ...
    'RecordMovie', false, 'PMLAlpha', 1e10, 'PlotFreq', 100, 'PlotSim', false};

% run the simulation
sensor_data = kspaceFirstOrder2D(kgrid, medium, source, sensor, input_args{:});

% sensor_data = kspaceFirstOrder2D(kgrid, medium, source, sensor, input_args{:});