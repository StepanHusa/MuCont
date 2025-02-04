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

# a is freee
# b is fixed
b = params[2]
n_var = 2
free = 1 # later make a list possible (fold or something)

f = (x) -> fun_eval(0, x[1:end-1], [x[end], b])
J = (x) -> fun_jacobi(x[1:end-1], [x[end], b])
J_sub = (x) -> J(x)[:, 1:end-1]

x = vcat(equi, params[1])
js = J_sub(x)
using LinearAlgebra


v = nullspace(js)
v = vec(v)

initials = vcat(equi, params[1])

npoints = 100
curve = continuate(f, J_sub, initials, npoints)
using Plots

x_vals = [point[1] for point in curve]
y_vals = [point[2] for point in curve]
a_vals = [point[3] for point in curve]

plot3d(x_vals, y_vals, z_vals, title="Continuation Curve", xlabel="x", ylabel="y", zlabel="Parameter")
plot(x_vals, a_vals, title="Continuation Curve", xlabel="x", ylabel="y", label="Parameter")

xx = [0.2,0,0]

nc = NewtonCorrection(f, J_sub, xx)