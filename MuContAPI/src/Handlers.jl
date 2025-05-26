module Handlers

using HTTP

using MuContAPI.Jobs


const hh = HTTP.Response(200, "OK")

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

end