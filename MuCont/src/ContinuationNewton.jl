module ContinuationNewton

using NamedDims
using ForwardDiff
using LinearAlgebra


function direction(fun_jacobi, x)
    J = fun_jacobi(x) # a 2x3 matrix computed from the variables and one free parameter

    v = nullspace(J)
    v = vec(v)

    return normalize(v)
end

function continuate_newton(f, initial, npoints, Jac=nothing, h=0.01)
    ncoords = length(initial)
    moore_penrose = false
    # f = y -> [dot(y,y)- 1, y[1]]

    tol = 1e-10
    if Jac === nothing
        Jac = (x) -> ForwardDiff.jacobian(f, x)
    end


    @assert rank(Jac(initial), tol) == ncoords - 1
    N = nullspace(Jac(initial))

    v = N[:, 1]
    v = v / norm(v)

    x = initial

    # determine the dirrection for the first time 
    data = Array{Float64}(undef, npoints, ncoords)

    curve = NamedDimsArray(data, (:point, :coord))

    curve[point=1] = x

    i = 2

    dx = zeros(ncoords)
    while i <= npoints
        # prediction for the next point
        x = x + h * v

        # correction (2 times) 
        for j in 1:1
            J = Jac(x)
            @assert rank(J, tol) == ncoords - 1 
            fx = f(x)

            dx  = J \ fx # this is no good, since it is ambiguous

            # if moore_penrose
            #     R = zeros(ncoords)
            #     R[end] = 1
            #     B = hcat(J,v)
            #     Q = hcat(fx,0)
                
            #     D = B / vcat(Q,R)
            # end

            # TODO assert the dx is small and make a smaller step then.
            @assert norm(dx) < 0.2 * h 
            
            x = x - dx

            if norm(dx) > 0.2 * h 
                @info "s" dxlen = norm(dx) dd = norm(J*dx-fx) vdx = dot(v,dx)/norm(dx)
            end
        end


        #storing the point
        curve[point=i] = x


        # computing the direction for the next iteration
        # J = Jac(x)
        # @assert rank(J, tol) == ncoords - 1
        # N = nullspace(J)
        # removed and aproxibated from the previous (last) computation
        v = v - dx
        v = v / norm(v)

        i += 1
    end



    return curve
end

end # module
