module Jobs

const JOBS = Dict{String, JobState}()

struct JobState
    status::String             # e.g., "running", "done", "error"
    progress::Float64          # e.g., 0.0–100.0
    result::Vector{Tuple{Float64, Float64}}  # Or any structure for curve points
    error::Union{Nothing, String}
end
end