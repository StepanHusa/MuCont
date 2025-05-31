module MuCont

include("Models.jl")
include("SystemModels.jl")
include("SystemParser.jl")
include("SystemManager.jl")
include("Integration.jl")
include("ContinuationNewton.jl")
include("demo_continuate_circle.jl")

# import MuCont.Models
# import MuCont.SystemModels
# import MuCont.SystemParser
# import MuCont.Integration

function simple_computer_add(a, b)
    return a + b
end

function demo_system_cont(compiled_system)
    # path = "D:\\9_SOURCE\\mucont\\MuCont\\test\\test_odes\\lv.ode"
    # model = SystemParser.parse_mcsys(path)
    # compiled_system = SystemParser.compile_system(model)
    @info "Compiled system" model = compiled_system.model.name ncoords = compiled_system.ncoords nparams = compiled_system.nparams

    p = [0, 1, 0.5, 0.5]
    x0 = [1, .5]
    tspan = (0.0, 300.0)


# rhs! = (du, u, p, t) -> f_in(du, vcat(u, p))
    buf = zeros(length(x0) + length(p))
    rhs! = (du, u, p, t) -> begin
        buf[1:length(u)] = u
        buf[length(u)+1:end] = p
        compiled_system.f_inplace!(du,buf)
    end

    sol = Integration.integrate_ode(rhs!, x0, tspan, params=p)
    @info "ODE solution" t = sol.t x = sol.u
    last_x = sol.u[end]
    @info "Last state" x = last_x

    # equilibrium
    eq = union(last_x, p[1])
    f = y -> compiled_system.f(y[1:end-1], [y[end], p[2:end]])
    Jac = y -> compiled_system.Total_Diff_out(union(y,p))

    # curve = ContinuationNewton.continuate_newton(f, eq, 50) # not working with numerical Jacobian
    curve = ContinuationNewton.continuate_newton(f, eq, 50, Jac) 

    return curve
end
end