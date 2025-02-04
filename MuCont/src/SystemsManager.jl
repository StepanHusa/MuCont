module SystemsManager

function NewSystem(name::String, coordinates::Vector{String}, parameters::Vector{String}, equations::Vector{String})
    
    
    return ODESystem(name, coordinates, parameters, equations)
    
end
    
end