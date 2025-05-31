module Errors

export APIError, MissingParamError, ValidationError, ComputationError

abstract type APIError <: Exception end

struct MissingParamError <: APIError
    key::String
end

struct MissingJsonParamError <: APIError
    key::String
end

struct ValidationError <: APIError
    msg::String
end

struct ComputationError <: APIError
    job_id::String
    reason::String
end

end
