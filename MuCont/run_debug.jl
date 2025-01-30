import MuCont as cont





include("run_first.jl")
import .run_first
# run_first.main()

initials = [-8.0, -7.0]
params = [-2, 0]

tspan = [0.0, 10.0]

t, y = integrate_euler(fun_eval, initials, params, tspan)
plot(t, y[:,1], title="Time Series", xlabel="Time", ylabel="Values", label=["x" "y"])


equi = y[end,:]

continuate(fun_eval,fun_grad, equi, params, npoints)