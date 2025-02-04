import MuCont as cont

SymbolModel = cont.SystemModels.SymbolModel
SystemModel = cont.SystemModels.SystemModel
Coordinate = cont.SystemModels.Coordinate
Time = cont.SystemModels.Time

variables = [
    SymbolModel(name="x", latex_name="x", type=SymbolType(0)),
    SymbolModel(name="y", latex_name="y", type=Coordinate),
    SymbolModel(name="t", latex_name="t", type=Time)
]

system_model = SystemModel(
    id=1,
    name="Example System",
    info=["This is an example system model."],
    notes=["No additional notes."],
    variables=variables
)

