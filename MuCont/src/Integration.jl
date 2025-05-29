module Integration

# using DifferentialEquations
using OrdinaryDiffEq

function integrate_ode(f, initials, tspan; params=nothing, solver=Tsit5())
    # Define the ODE problem
    prob = ODEProblem(f, initials, tspan, params)

    # Solve the ODE
    sol = solve(prob,solver)

    return sol
end

end