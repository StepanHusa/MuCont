module DataTypes

using NamedDims

A = NamedDimsArray(rand(100, 3, 20), (:point, :coord, :time))

sum(A .* reshape(rand(3), (1, :, 1)); dims=:coord)


# struct LabeledTensor{T,N}
#     data::Array{T,N}
#     labels::NTuple{N,Symbol}
# end

# function LabeledTensor(data::AbstractArray{T,N}, labels::NTuple{N,Symbol}) where {T,N}
#     ndims(data) == length(labels) || throw(ArgumentError("Label count must match data dimensions"))
#     return LabeledTensor{T,N}(Array{T,N}(data), labels)
# end

# function axis(lt::LabeledTensor, label::Symbol)
#     i = findfirst(==(label), lt.labels)
#     i === nothing && error("Label $label not found")
#     return i
# end

# function get_slice(lt::LabeledTensor, label::Symbol, idx)
#     a = axis(lt, label)
#     inds = ntuple(i -> i == a ? idx : Colon(), length(lt.labels))
#     return view(lt.data, inds...)
# end

# function dot_along(lt::LabeledTensor, vec::AbstractVector, along::Symbol)
#     i = axis(lt, along)
#     size(lt.data, i) == length(vec) || error("Mismatched length")
#     return sum(lt.data .* reshape(vec, ntuple(j -> j == i ? length(vec) : 1, ndims(lt.data))...); dims=i)
# end


end