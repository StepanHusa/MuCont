module Handlers

using HTTP
using JSON3

import MuCont as cont

using MuContAPI.Jobs

function post_simple_compute_request(data)
    result = cont.simple_computer_add(data[1], data[2])
    return HTTP.Response(200, JSON3.write(Dict("result" => result)))
end

function get_all_systems()
    systems = cont.SystemManager.get_systems()

    @info "Retrieved systems" systems = systems
    return HTTP.Response(200, JSON3.write(Dict(
        "systems" => "hello"
    )))
end

function get_job_status(job_id)
    job = get_job(job_id)
    if job === nothing
        return HTTP.Response(404, "Job not found")
    end

    return HTTP.Response(200, JSON3.write(Dict(
        "status" => job.status,
        "progress" => job.progress,
        "result" => job.result,
        "error" => job.error
    )))
end

function post_start_job()
    @info "Starting new job."

    return HTTP.Response(200, "OK")
end

# function get_systems(filter=nothing)

# end

end