module Routes

using ..MuContAPI.Handlers
using ..MuContAPI.JobState
using HTTP

function register_routes()
    register(:POST, "/start_job") do req
        return handle_start_job()
    end

    register(:GET, "/job_status") do req
        query = HTTP.URIs.queryparams(HTTP.URI(req.target))
        return handle_job_status(get(query, "id", nothing))
    end
end

end
