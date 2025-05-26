module Jobs

export JOBS, JOBS_LOCK, JobState, get_job, set_job, update_job!


struct JobState
    status::String             # e.g., "running", "done", "error"
    progress::Float64          # e.g., 0.0–100.0
    result::Vector{Tuple{Float64, Float64}}  # Or any structure for curve points
    error::Union{Nothing, String}
end


const JOBS = Dict{String, JobState}()
const JOBS_LOCK = ReentrantLock()

# Optional convenience functions:
function get_job(id::String)
    lock(JOBS_LOCK) do
        return get(JOBS, id, nothing)
    end
end

function set_job(id::String, state::JobState)
    lock(JOBS_LOCK) do
        JOBS[id] = state
    end
end

function update_job!(id::String, f::Function)
    lock(JOBS_LOCK) do
        f(JOBS[id])
    end
end
end