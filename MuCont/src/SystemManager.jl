module SystemManager

using MuCont.SystemParser

const MUCONT_FOLDER = Ref("")

function SetMuContFolder(path::String)
    # TODO add validation
    MUCONT_FOLDER[] = path
end


function assert_mucont_folder()
    MUCONT_FOLDER[] === "" && throw("System folder not set, call SystemsManager.SetMuContFolder()")
end


function get_systems()
    assert_mucont_folder()
    files = readdir(MUCONT_FOLDER[])

    systems = SystemParser.MuSystemInfo[]
    for file in files

        if endswith(file, ".mcsys")
            fullname = joinpath(MUCONT_FOLDER[], file)
            s = SystemParser.get_system_basic_info(fullname)
            try
                push!(systems, s)
            catch
                @warn "Could not parse the file" file = fullname
            end
        end
    end

    return systems
end

function write_new_system(data)
    assert_mucont_folder()


    filename = data["id"]
    path = joinpath(MUCONT_FOLDER[], "$(filename).mcsys")


    file = SystemParser.convert_new_system_to_json(data)

    write(path, file)
end

end