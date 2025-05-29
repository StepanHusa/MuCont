module MuCont

include("Models.jl")
include("SystemModels.jl")
include("SystemParser.jl")
include("Integration.jl")

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

    p = [1, 1, 0.5, 0.5]
    x0 = [1, 1]
    tspan = (0.0, 10.0)

    f = (x,p,t) -> compiled_system.f([x,p]) 

    sol = Integration.integrate_ode(f, x0, tspan, params=p)
    @info "ODE solution" t = sol.t x = sol.u
    last_x = sol.u[end, :]
    @info "Last state" x = last_x

    # equilibrium
    eq = [last_x, p[1]]
    f = y -> compiled_system.f(y[1:end-1], [y[end], p[2:end]])

    curve = ContinuationNewton.continuate_newton(f, eq, Jac=nothing, h=0.01)

    return curve
end
end