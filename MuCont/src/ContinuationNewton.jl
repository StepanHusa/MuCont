module ContinuationNewton

using NamedDims
using ForwardDiff
using LinearAlgebra

export continuate # TODO make a rule of what to export and what not

function direction(fun_jacobi, x)
    J = fun_jacobi(x) # a 2x3 matrix computed from the variables and one free parameter

    v = nullspace(J)
    v = vec(v)

    return normalize(v)
end

function continuate_newton(f, initial, npoints, Jac=nothing, h=0.01)
    ncoords = length(initial)
    # f = y -> [dot(y,y)- 1, y[1]]

    tol = 1e-10
    if Jac === nothing
        Jac = (x) -> ForwardDiff.jacobian(f, x)
    end


    @assert rank(Jac(initial), tol) == ncoords
    N = nullspace(Jac(initial))

    v = N[:, 1]
    v = v / norm(v)

    x = initial

    # determine the dirrection for the first time 
    data = Array{Float64}(undef, npoints, ncoords)

    curve = NamedDimsArray(data, (:point, :coord))

    curve[point=1] = x

    i = 2

    while i < npoints
        x = x + h * v

        for j in 1:2
            J = Jac(x)
            @assert rank(J, tol) == ncoords
            fx = f(x)

            x = x + -J \ fx
        end


        curve[point=i] = x

        J = Jac(x)
        @assert rank(J, tol) == ncoords
        N = nullspace(J)

        v = N[:, 1]
        v = v / norm(v)

        i += 1
    end



    return curve
end

end # module
