module SystemsManager

using MuCont.SystemParser

MUCONT_FOLDER = ""

function SetMuContFolder(path::String)
    # TODO add validation
    MUCONT_FOLDER = path
end


function assert_mucont_folder()
    MUCONT_FOLDER === "" && throw("System folder not set, call SystemsManager.SetMuContFolder()")
end


function get_systems()
    assert_mucont_folder()
    files = readdir(MUCONT_FOLDER)

    systems = []
    for file in files
        if endswith(file, ".mcsys")
            s = SystemParser.parse_mcsys(file)
            append!(systems, s)
        end
    end

    return systems
end
    
end