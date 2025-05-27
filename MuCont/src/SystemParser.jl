module SystemParser

using JSON3
using Symbolics

export MuSystem, parse_mcsys

struct MuSystem
    name::String
    coordinates::Vector{Symbol}
    parameters::Vector{Symbol}
    functions::Vector{Pair{Symbol,Symbolics.Num}}
    equations::Vector{Symbolics.Num}
    displayfunctions::Vector{Pair{Symbol,Symbolics.Num}}
    latexnames::Dict{Symbol,String}

    # all_variables::Symbolics.Num[]
end

struct CompiledSystem
    model::MuSystem
    ncoords::Int
    nparams::Int
    f::Function
    jacobian::Function
end

const RESERVED = Set(["period", "length", "step", "solver", "method", "jacobian", "t"])

function compile_system(model::MuSystem)::CompiledSystem
    syms = [Symbolics.scalarize(Symbolics.variable(s)) for s in vcat(model.coordinates, model.parameters)]
    coords = [Symbolics.scalarize(Symbolics.variable(s)) for s in model.coordinates]

    f_out, f_in = Symbolics.build_function(model.equations, syms; expression=Val(false)) |> eval # the in function is good for loops and prealocated arrays

    J = Symbolics.jacobian(model.equations, coords)
    J_out, J_in = Symbolics.build_function(J, syms; expression=Val(false)) |> eval

    return CompiledSystem(model, length(model.coordinates), length(model.parameters), f_out, J_out)
end


function parse_mcsys(filename::String)::MuSystem
    data = JSON3.read(filename)

    name = String(data["name"])
    coords = Symbol.(data["coordinates"])
    parameters = Symbol.(data["parameters"])

    func_list = collect(data["functions"])
    eqn_list = collect(data["equations"])
    disp_dict = data["displayfunctions"]
    latex_dict = haskey(data, "latexnaming") ? data["latexnaming"] : Dict()

    func_keys = Symbol.(first.(func_list))
    eq_keys = Symbol.(first.(eqn_list))
    disp_keys = Symbol.(keys(disp_dict))
    latex_keys = Symbol.(keys(latex_dict))

    validate_symbols(coords, parameters, func_keys, eq_keys, disp_keys, latex_keys)


    env = Dict{Symbol,Symbolics.Num}()

    env[:t] = Symbolics.scalarize(Symbolics.variable(:t))


    # Define coordinates and parameters one-by-one
    for s in vcat(coords, parameters)
        env[s] = Symbolics.scalarize(Symbolics.variable(s))
    end


    functions = Pair{Symbol,Symbolics.Num}[]
    for (k, v) in func_list
        symk = Symbol(k)
        mexpr = Meta.parse(v)
        expr = Symbolics.parse_expr_to_symbolic.([mexpr], (Main,))[1]
        push!(functions, symk => expr)
        # env[symk] = expr
    end

    equations = Symbolics.Num[]
    for (k, v) in eqn_list
        symk = Symbol(k)
        mexpr = Meta.parse(v)
        expr = Symbolics.parse_expr_to_symbolic.([mexpr], (Main,))[1] # a bit of hacking here, the libraries are not ready for this.
        push!(equations, expr)
    end

    displayfunctions = Pair{Symbol,Symbolics.Num}[]
    for (k, v) in disp_dict
        mexpr = Meta.parse(v)
        expr = Symbolics.parse_expr_to_symbolic.([mexpr], (Main,))[1]
        push!(displayfunctions, Symbol(k) => expr)
    end

    latexnames = Dict{Symbol,String}()
    for (k, v) in latex_dict
        latexnames[Symbol(k)] = String(v)
    end

    return MuSystem(name, coords, parameters, functions, equations, displayfunctions, latexnames)
end

function validate_symbols(coords, parameters, func_keys, eq_keys, disp_keys, latex_keys)
    all_defined = union(coords, parameters)

    # 0. Validate order of eqs
    if coords != eq_keys
        error("Order of coordinates and equations must match.")
    end

    # 1. Reserved word check
    for s in union(coords, parameters, func_keys, eq_keys, disp_keys, latex_keys)
        if s in RESERVED
            error("Name '$s' is reserved and cannot be used.")
        end
    end

    # 2. No duplicates between categories
    if length(unique(coords)) != length(coords)
        error("Duplicate names in coordinates.")
    end
    if length(unique(parameters)) != length(parameters)
        error("Duplicate names in parameters.")
    end
    if !isempty(intersect(coords, parameters))
        error("A name cannot be both coordinate and parameter.")
    end

    # 3. Functions/display must not redefine coordinates or parameters
    for s in union(func_keys, disp_keys)
        if s in all_defined
            error("Name '$s' used in function/display conflicts with coordinates or parameters.")
        end
    end

    # 4. Optional: check uniqueness in functions/equations
    for (category, syms) in [("functions", func_keys), ("displayfunctions", disp_keys)]
        if length(unique(syms)) != length(syms)
            error("Duplicate names in $category.")
        end
    end
end
end # module
