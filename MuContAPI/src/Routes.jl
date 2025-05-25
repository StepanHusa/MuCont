module Routes

using HTTP

include("Handlers.jl")

using .Handlers

const ROUTES = Dict{Tuple{Symbol,String}, Function}()

function register(handler::Function,method::Symbol, path::String)
    ROUTES[(method, path)] = handler
end

function register_routes()
    register(:POST, "/start_job") do req
        # return Handlers.post_start_job()
        return HTTP.Response(200, "OK")
    end

    # register("/start_job", :POST, req -> Handlers.post_start_job())

    register(:GET, "/job_status") do req
        query = HTTP.URIs.queryparams(HTTP.URI(req.target))
        return Handlers.get_job_status(get(query, "id", nothing))
    end
end

end
