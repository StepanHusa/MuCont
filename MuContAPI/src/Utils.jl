module Utils

using HTTP
using JSON3
using MuContAPI.Errors

export require_param

using HTTP
using MuContAPI.Errors  # where MissingParamError is defined

function require_param(req::HTTP.Request, key::String)
    query = HTTP.URIs.queryparams(HTTP.URI(req.target))
    val = get(query, key, nothing)
    val === nothing && throw(MissingParamError(key))
    return val
end


end # module
