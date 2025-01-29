module Models

struct ODESystem
    name::String
    coordinates::Vector{String}
    parameters::Vector{String}
    equations::Vector{String}


    function ODESystem(name::String, coordinates::Vector{String}, parameters::Vector{String}, equations::Vector{String})
        new(name, coordinates, parameters, equations)
    end
end

end
