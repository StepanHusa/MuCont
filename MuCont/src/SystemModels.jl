module SystemModels

using JSON3

@enum SymbolType Variable Coordinate Time


Base.@kwdef struct CurveDataModel
    array::Array{Float64,3}  # Equivalent to double[,,] in C#
    free_parameters::Vector{String}  # Equivalent to string[]
end


Base.@kwdef struct CurveModel
    id::UInt
    system_id::UInt
    name::String
    data::CurveDataModel
end


Base.@kwdef struct SymbolModel
    name::String
    latex_name::String
    type::SymbolType
end


Base.@kwdef struct SystemModel
    id::UInt
    name::String
    info::Vector{String}
    notes::Vector{String}
    variables::Vector{SymbolModel}
end



Base.@kwdef struct DiagramsModel
    system_id::UInt
    name::String
    curves::Vector{CurveModel}
end




end # module
