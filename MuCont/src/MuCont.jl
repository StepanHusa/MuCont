module MuCont

include("Models.jl")
include("SystemModels.jl")
include("SystemParser.jl")

import .Models
import .SystemModels
import .SystemParser

function simple_computer_add(a, b)
    return a + b
end

end