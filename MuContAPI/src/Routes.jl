module Routes

using HTTP

using MuContAPI.Handlers
using MuContAPI.Utils

const ROUTES = Dict{Tuple{Symbol,String},Function}()

function register(handler::Function, method::Symbol, path::String)
    ROUTES[(method, "/api/" * path)] = handler
end

function register_routes()
    register(:POST, "simple_computer_add") do req
        data = from_json(req, "numbers")
        return Handlers.post_simple_compute_request(data)
    end

    register(:POST, "start_job") do req
        return Handlers.post_start_job()
        # return HTTP.Response(200, "OK")
    end

    # register("start_job", :POST, req -> Handlers.post_start_job())

    register(:GET, "job_status") do req
        id = require_param(req, "id")

        return Handlers.get_job_status(id)
    end

    # register(:GET, "job_status") do req
    #     query = HTTP.URIs.queryparams(HTTP.URI(req.target))
    #     @debug "query" q = query
    #     id = get(query, "id", nothing)

    #     return Handlers.get_job_status()
    # end
end

end
