using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Exceptions;

public class EmailAlreadyExistsException(string email) :
    Exception($"Email {email} в настоящий момент уже используется")
{ 
}