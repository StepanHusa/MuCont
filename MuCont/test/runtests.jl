import MuCont as cont
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
    @test cont.simple_computer_add(2, 3) == 5
end

@testset "System Parser Tests" begin
    file = joinpath(@__DIR__, "test_mcsys/lv.mcsys")
    model = cont.SystemParser.parse_mcsys(file)

    @test model isa cont.SystemParser.MuSystem
    @test length(model.equations) > 0

    compiled_system = cont.SystemParser.compile_system(model)

    @test compiled_system isa cont.SystemParser.CompiledSystem
    @test compiled_system.f([1.0, 1, 1, 1, 1, 1]) isa Vector{Float64}
    # @test compiled_system.jacobian([1.0, 1, 1, 1, 1, 1]) isa Vector{Float64}

    result = cont.demo_system_cont(compiled_system)

    # @info "curve" result = result

end

@testset "System Parser Tests - dependent system" begin
    file = joinpath(@__DIR__, "test_mcsys/dependent.mcsys")
    model = cont.SystemParser.parse_mcsys(file)

    @test model isa cont.SystemParser.MuSystem
    @test length(model.equations) > 0

    compiled_system = cont.SystemParser.compile_system(model)

    @test compiled_system isa cont.SystemParser.CompiledSystem

end

@testset "System Parser Tests - functions like exp, cos in the system" begin
    file = joinpath(@__DIR__, "test_mcsys/functional.mcsys")
    model = cont.SystemParser.parse_mcsys(file)

    @test model isa cont.SystemParser.MuSystem
    @test length(model.equations) > 0

    compiled_system = cont.SystemParser.compile_system(model)

    @test compiled_system isa cont.SystemParser.CompiledSystem
end

