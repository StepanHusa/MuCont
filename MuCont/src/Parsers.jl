module Parsers

export parse_odesystem

using Models

function parse_odesystem(name::String, equations::Vector{String}, possible_params::Vector{String})
    # Extract coordinates (assume LHS variables in the form "dx/dt", "dy/dt")
    coordinates = [split(eqn, "=")[1] |> strip |> replace("d/dt", "") for eqn in equations]
    
    # Extract parameter names from the equations
    param_names = Set{String}()
    for eqn in equations
        for param in possible_params
            if occursin(param, eqn)
                push!(param_names, param)
            end
        end
    end
    
    # Create the ODESystem
    return Models.ODESystem(name, coordinates, collect(param_names), equations)
end

function parse_system_matcont(name, coordinates, parameters, equations)
    # string_jac=''
    # ;string_jacp='';string_hess='';string_hessp='';string_tensor3='';string_tensor4='';string_tensor5='';
    # if isempty(name)
    #     errordlg('You have to give the system a name','Name');
    #     return
    #     end


    return parsed_system
end

end
