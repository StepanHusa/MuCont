using NamedDims
using ForwardDiff
using LinearAlgebra


function demo_continuate_circle()



    ncoords = 3
    f = y -> [dot(y, y) - 1, y[1]]

    initial_p = [0, 1, 0]
    h = 0.01
    npoints = 100
    tol = 1e-10


    J = ForwardDiff.jacobian(f, initial_p)


    @assert rank(J, tol) == 2
    N = nullspace(J)

    v = N[:, 1]
    v = v / norm(v)

    x = initial_p


    # determine the dirrection for the first time 
    data = Array{Float64}(undef, npoints, ncoords)

    curve = NamedDimsArray(data, (:point, :coord))

    curve[point=1] = x

    i = 2

    while i < npoints
        x = x + h * v

        for j in 1:2
            J = ForwardDiff.jacobian(f, x)
            fx = f(x)

            x = x + -J \ fx
        end


        curve[point=i] = x

        J = ForwardDiff.jacobian(f, x)
        @assert rank(J, tol) == 2
        N = nullspace(J)

        v = N[:, 1]
        v = v / norm(v)

        i += 1
    end



    return curve
end

