module MuCont

include("Models.jl")
include("SystemModels.jl")

import .Models
import .SystemModels




function simple_computer_add(a,b)
    return a+b
end

end