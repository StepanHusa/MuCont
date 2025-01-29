module run_first
using Plots

import cont

function fun_eval(t, kmrgd, par_a)
    x = kmrgd[1]
    y= kmrgd[2]

    a = par_a[1]
    b = par_a[2]

    xdot = y
    ydot = a+b*x+x^2+x*y

    return [xdot, ydot]
end

function main()
    initials = [1.0, 2.0]
    params = [1.0, -3.0]

    tspan = [0.0, 10.0]
    
    timeser = integrate(fun_eval, initials, params, tspan)
    


    plot(timeser, title="Time Series", xlabel="Time", ylabel="Values", label=["x" "y"])

end

function integrate(eval_fun, initials, params, tspan)
    h = 0.1  # Step size
    t0, tf = tspan
    t = t0
    y = initials
    results = Float64[]
    times = Float64[]

    while t <= tf
        push!(results, y)
        push!(times, t)

        # Compute Runge-Kutta increments
        k1 = eval_fun(t, y, params)
        k2 = eval_fun(t + h/2, y .+ (h/2) .* k1, params)
        k3 = eval_fun(t + h/2, y .+ (h/2) .* k2, params)
        k4 = eval_fun(t + h, y .+ h .* k3, params)

        # Update solution
        y = y .+ (h/6) .* (k1 .+ 2 .* k2 .+ 2 .* k3 .+ k4)
        t += h
    end

    return hcat(times, hcat(results...)')  # Combine time and values into a matrix
end


end

