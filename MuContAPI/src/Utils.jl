module Utils

using HTTP
using JSON3
using MuContAPI.Errors

export require_param, from_json


function require_param(req::HTTP.Request, key::String)
    query = HTTP.URIs.queryparams(HTTP.URI(req.target))
    val = get(query, key, nothing)
    val === nothing && throw(MissingParamError(key))
    return val
end

function from_json(req::HTTP.Request, key::String)
    body = String(req.body)
    data = get(JSON3.read(body), key, nothing)
    data === nothing && throw(MissingJsonParamError(key))
    return data
end 


end # module
