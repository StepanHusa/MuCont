module Handlers

using HTTP
using JSON3

import MuCont as cont

using MuContAPI.Jobs

# TODO: Merge Handlers and Routes into a single module.

function post_simple_compute_request(data)
    result = cont.simple_computer_add(data[1], data[2])
    return HTTP.Response(200, JSON3.write(Dict("result" => result)))
end

function get_all_systems()
    systems = cont.SystemManager.get_systems()

    @info "Retrieved systems" systems = systems
    return HTTP.Response(200, JSON3.write(Dict(
        "systems" => systems
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

function demo_continuation()
    @info "Running demo continuation..."

    # Simulate a computation
    # file = joinpath(@__DIR__, "test/test_mcsys/conti.mcsys")
    systems = cont.SystemManager.get_systems()
    model = filter(s -> s.name == "Test System for continuation", systems)
    if isempty(model)
        return HTTP.Response(404, "System 'Test System for continuation' not found")
    end
    model = first(model)
    parsed = cont.SystemParser.parse_mcsys(model.file)
    compiled_system = cont.SystemParser.compile_system(parsed)
    result = cont.demo_system_cont(compiled_system)

    @info "Demo computation result" result = result
    return HTTP.Response(200, JSON3.write(Dict("result" => result)))
end

function post_new_system(data)
    @info "Received new system data" data = data

    cont.SystemManager.write_new_system(data)

    systems = cont.SystemManager.get_systems()
    model = filter(s -> s.name == data.name, systems)

    # For now, we just return a success message
    return HTTP.Response(200, "New system added successfully")
end

# function get_systems(filter=nothing)

# end

# function plot_something()
#     # Placeholder for a function that would plot something
#     # This could be used to visualize data or results
#     @info "Plotting something..."

#     plot([0, 1, 2], [0, 1, 4], label="Example Plot")
#     return HTTP.Response(200, "Plotting successful")
# end

end