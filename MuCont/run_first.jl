using Plots

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

    J = zeros(2, 2)
    J[1, 1] = 2*x
    J[1, 2] = 0
    J[2, 1] = 0
    J[2, 2] = -1

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


function continuate(fun_eval, fun_jacobi, initials, params, npoints)
    x0 = initials

    stepsize = 0.01

    J = fun_jacobi(x0, params)
    f = fun_eval(0, x0, params)


    


end