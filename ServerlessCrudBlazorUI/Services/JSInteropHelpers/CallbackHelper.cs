using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.JSInteropHelpers
{
    /// <summary>
    /// A class to be passed as a DotNetObjectReference to IJSRuntime invoked Javascript, which can call its JSInvokable CallbackMethod.
    /// </summary>
    public class CallbackHelper
    {
        /// <summary>
        /// The delegate for the .NET method to be invoked by Javascript.
        /// </summary>
        /// <param name="args">The arguments supplied by Javascript to the .NET method.</param>
        public delegate Task Callback(params object[] args);
        /// <summary>
        /// Creates a new CallbackHelper instance.
        /// </summary>
        /// <param name="callbackMethod">The .NET method to be invoked by Javascript.</param>
        public CallbackHelper(Callback callbackMethod)
        {
            CallbackMethod = callbackMethod;
        }
        /// <summary>
        /// The .NET method to be invoked by Javascript.
        /// </summary>
        public Callback CallbackMethod { get; set; }
        /// <summary>
        /// The method invoked by Javascript, which in turn invokes the CallbackMethod supplied in the constructor.
        /// </summary>
        /// <param name="args">The arguments to be passed to the CallbackMethod supplied in the constructor.</param>
        [JSInvokable]
        public async Task InvokeCallback(params object[] args)
        {
            await CallbackMethod(args);
        }
    }
}
