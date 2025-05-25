module Handlers

function get_job_status(id)

    return HTTP.Response(200, JSON3.write(Dict("message" => "hello")); headers = ["Content-Type" => "application/json"])
end

function post_start_job()
    @info "Starting new job."
    return HTTP.Response(200, "OK")
end

end