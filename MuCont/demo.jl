import MuCont as cont
using Plots

file = joinpath(@__DIR__, "test/test_mcsys/conti.mcsys")
model = cont.SystemParser.parse_mcsys(file)

compiled_system = cont.SystemParser.compile_system(model)

p = [1, 1, 0.5, 0.5]
x0 = [2, 0.5]
tspan = (0.0, 300.0)


# rhs! = (du, u, p, t) -> f_in(du, vcat(u, p))
buf = zeros(length(x0) + length(p))
rhs! = (du, u, p, t) -> begin
    buf[1:length(u)] = u
    buf[length(u)+1:end] = p
    compiled_system.f_inplace!(du, buf)
end

sol = cont.Integration.integrate_ode(rhs!, x0, tspan, params=p)
# @info "ODE solution" t = sol.t x = sol.u
last_x = sol.u[end]
# @info "Last state" x = last_x

u_mat = reduce(hcat, sol.u)'
plot(u_mat[:, 1], u_mat[:, 2])

# equilibrium
eq = union(last_x, p[1])
f = y -> compiled_system.f(union(y, p[2:end]))
Jac = y -> compiled_system.Total_Diff(union(y, p[2:end]))[:, 1:3]

# curve = ContinuationNewton.continuate_newton(f, eq, 50) # not working with numerical Jacobian
curve = cont.ContinuationNewton.continuate_newton(f, eq, 5000, nothing, 0.01)


scatter(curve[coord=3], curve[coord=1])

cont.demo_system_cont(compiled_system)