@testset "SystemParser - Write a simple system" begin


    id = "test_system"

    data = Dict(
        "id" => id,
        "name" => "Test System for continuation",
        "equations" => ["x' = x + 1", "y' = y + 2"],
        "variables" => ["x", "y"],
        "parameters" => ["p1", "p2"]
    )

    folder = joinpath(@__DIR__, "test_mcsys/")
    cont.SystemManager.SetMuContFolder(folder)

    cont.SystemManager.write_new_system(data)
end