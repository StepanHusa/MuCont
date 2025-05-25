module MuContAPI

using HTTP
using JSON3
using Logging
using LoggingExtras

import MuCont as cont

include("Jobs.jl")
include("Routes.jl")

using .Routes


function handle_request(req::HTTP.Request)
    method = Symbol(req.method)
    path = split(String(req.target), '?')[1]
    key = (method, path)

    @debug "Executing request" request = req key=key

    if haskey(Routes.ROUTES, key)
        return Routes.ROUTES[key](req)
    else
        @warn "Unknown route accessed" route = route
        return HTTP.Response(404, "Endpoint not found")
    end
end




function start_api_server(host::String, port::Int)
    @info "Starting API server" host = host port = port

    HTTP.serve(handle_request, host, port)
end

# function __init__()
#     Routes.register_routes()
# end

function main()
    CONFIG = load_config() # TODO research if it is good idea to make CONFIG global

    log_file = joinpath(CONFIG["log_path"], "MuContAPI.log")
    setup_log(log_file)

    @info "Logs location" log_file = log_file

    Routes.register_routes()

    host = CONFIG["host"]
    port = CONFIG["port"]
    start_api_server(host, port)
end

function setup_log(log_file)
    log_dir = dirname(log_file)
    if !isdir(log_dir)
        mkpath(log_dir)
    end

    # global_logger(FileLogger(log_file))
    logger = TeeLogger(ConsoleLogger(stderr, Logging.Debug), FileLogger(log_file)) # debug
    global_logger(logger)
end

function load_config()
    config_path = get(ENV, "MUCONT_API_CONFIG", "config.json")  # Default to "config.json" if there is not variable API_CONFIG
    try
        config_data = JSON3.read(read(config_path, String))
        return config_data
    catch e
        # dont use this as an example of throwing... 
        #this is only before the log file is initialized
        println("ERROR: Failed to parse config file at $config_path: " * string(e))
        error("Failed to parse config file at $config_path: " * string(e))
    end
end



end # module MuContAPI
