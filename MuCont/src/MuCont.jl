module MuCont

using LinearAlgebra
using ForwardDiff


include("Models.jl")
include("SystemModels.jl")
include("demo_continuate_circle.jl")

import .Models
import .SystemModels


function simple_computer_add(a,b)
    return a+b
end

end 
