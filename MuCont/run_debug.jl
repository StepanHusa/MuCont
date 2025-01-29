import MuCont as cont



# run_first
sys = cont.Models.ODESystem("Test", ["x","y"], ["a","b"], ["x'=a*x+17","y' =a*a+6"])
c = cont.simple_computer_add(1,2)
println(c)



include("run_first.jl")
import .run_first
run_first.main()