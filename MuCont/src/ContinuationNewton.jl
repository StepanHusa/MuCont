module ContinuationNewton

export continuate # TODO make a rule of what to export and what not

function direction(fun_jacobi, x)
    J = fun_jacobi(x) # a 2x3 matrix computed from the variables and one free parameter
    
    v = nullspace(J)
    v = vec(v)
    
    return normalize(v)
    end

function continuate(f, J, initials, npoints)
    x0 = initials

    stepsize = 0.1
    
    x = []

    for i in 1:npoints
        direction = direction(J, initials)
        x1 = x0 + stepsize * direction

        x2 = x1 - J(x1)\f(x1) # newton correction
        # there could be a second correction

        push!(x, x2)

        x0 = x2        
    end

    return x
end

end # module
