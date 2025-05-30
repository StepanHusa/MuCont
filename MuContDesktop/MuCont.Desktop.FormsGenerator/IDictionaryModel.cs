using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.FormsGenerator;
public interface IDictionaryModel
{
    Type KeyType { get; }
    Type ValueType { get; }

    
}
