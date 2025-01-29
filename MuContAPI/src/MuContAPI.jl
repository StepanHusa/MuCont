module MuContAPI

using HTTP
using JSON3
using MuCont
using Logging
using LoggingExtras

function handle_request(req::HTTP.Request)
    # Extract the path from the request
    route = String(req.target)
    @info "Received request" route=route method=req.method

    if route == "/"
        # Home endpoint
        return HTTP.Response(200, "Welcome to MuCont API!")
    elseif startswith(route, "/compute")
        # Example computation endpoint: expects JSON input
        try
            # Parse JSON payload from the request body
            body = String(req.body)
            data = JSON3.read(body)["numbers"]

            # Call a function from the computation package
            result = MuContCore.compute_task(data)

            @info "Computation successful" input=data result=result
            # Return the result as JSON
            return HTTP.Response(200, JSON3.write(Dict("result" => result)))
        catch e
            @error "Error processing request" error=string(e)
            # Handle errors
            return HTTP.Response(400, JSON3.write(Dict("error" => string(e))))
        end
    else
        # Unknown route
        @warn "Unknown route accessed" route=route
        return HTTP.Response(404, "Endpoint not found")
    end
end


function start_api_server(host::String, port::Int)
    @info "Starting API server" host=host port=port

    println("Starting API server at $host:$port...")
    HTTP.serve(handle_request, host, port)
end

function main()
    CONFIG = load_config() # TODO research if it is good idea to make CONFIG global
    host = CONFIG["host"]
    port = CONFIG["port"]

    log_file = joinpath(CONFIG["log_path"], "MuContAPI.log")
    setup_log(log_file)  

    start_api_server(host, port)
end

function setup_log(log_file)
    log_dir = dirname(log_file)
    if !isdir(log_dir)
        mkpath(log_dir)
    end

    global_logger(FileLogger(log_file))
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
