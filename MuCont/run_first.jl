using Plots
using LinearAlgebra

import MuCont as cont

function fun_eval(t, kmrgd, par_a)
    x = kmrgd[1]
    y= kmrgd[2]

    a = par_a[1]
    b = par_a[2]

    ## possibly hopf
    # xdot = y
    # ydot = a+b*x+x^2+x*y

    
    #just some linear system
    # xdot = y
    # ydot = -x

        #fold
    xdot = x^2 + a
    ydot = -y

    return [xdot, ydot]
end

function fun_jacobi(kmrgd,par_a)
    x = kmrgd[1]
    y= kmrgd[2]

    a = par_a[1]
    b = par_a[2]

    J = zeros(2, 4)  # 2 equations, 2 variables + 2 parameters
    J[1, 1] = 2*x   # ∂xdot/∂x
    J[1, 2] = 0     # ∂xdot/∂y
    J[1, 3] = 1     # ∂xdot/∂a
    J[1, 4] = 0     # ∂xdot/∂b
    J[2, 1] = 0     # ∂ydot/∂x
    J[2, 2] = -1    # ∂ydot/∂y
    J[2, 3] = 0     # ∂ydot/∂a
    J[2, 4] = 0     # ∂ydot/∂b

    return J

    
end


function integrate_rk(fun, initials, params, tspan)
    h = 0.1  # Step size
    t0, tf = tspan
    t = t0
    y = initials
    results = []
    times = []

    while t <= tf
        push!(results, y')
        push!(times, t)

        # Compute Runge-Kutta increments
        k1 = fun_eval(t, y, params)
        k2 = fun_eval(t + h/2, y .+ (h/2) .* k1, params)
        k3 = fun_eval(t + h/2, y .+ (h/2) .* k2, params)
        k4 = fun_eval(t + h, y .+ h .* k3, params)

        # Update solution
        y = y .+ (h/6) .* (k1 .+ 2 .* k2 .+ 2 .* k3 .+ k4)
        t += h
    end

    results_matrix = vcat(results...)  # Convert results to a matrix
    return times, results_matrix  # Return times and matrix of values
end

function integrate_euler(fun, initials, params, tspan)
    h = 0.1  # Step size
    t0, tf = tspan
    t = t0
    y = initials
    results = []
    times = []

    while t <= tf
        push!(results, y')
        push!(times, t)

        # Compute Euler increment
        k1 = fun_eval(t, y, params)

        # Update solution
        y = y .+ h .* k1
        t += h
    end

    results_matrix = vcat(results...)  # Convert results to a matrix
    return times, results_matrix  # Return times and matrix of values
end


function continuate(f, J, initials, npoints)
    x0 = initials

    stepsize = 0.1
    direction = initDirection(f, J, initials)

    x = []

    for i in 1:npoints
        x1 = x0 + stepsize * direction

        x2 = x1 - J(x1)\f(x1) # newton correction
        # there could be a second correction

        push!(x, x2)

        x0 = x2        
    end

    return x
end

function NewtonCorrection(f, J, x)
    return x - J(x)\f(x)
end

function initDirection(fun_eval, fun_jacobi, initials)
    
x = [];
v = [];


J = fun_jacobi(initials) # a 2x3 matrix computed from the variables and one free parameter

v = nullspace(J)
v = vec(v)

return normalize(v)
end