using cont
using Test

function validate_results(results, expected_length, tol)
    @test typeof(results) == Vector{Float64}
    @test length(results) == expected_length
    @test all(x -> abs(x) < tol, results)
end

# @testset "Continuation Tests" begin
#     results = continue_ode(system, params)
#     validate_results(results, 100, 1e-5)
# end

@testset "Continuation Tests" begin
    @test simple_computer_add(2,3) == 5
end