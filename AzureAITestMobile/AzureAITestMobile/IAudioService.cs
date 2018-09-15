using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureAITestMobile
{
    public interface IAudioService
    {
        Task PlayOnce(System.IO.Stream stream);
    }
}
