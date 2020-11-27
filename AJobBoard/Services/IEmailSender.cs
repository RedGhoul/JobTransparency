using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string v1, string v2);
    }
}
