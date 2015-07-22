using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace CertiPay.Common.WebServices
{
    /// <summary>
    /// Behavior that can be added to the WCF client to getting request and response raw XML.
    /// </summary>
    public class MessageInspectorBehavior : IEndpointBehavior
    {
        private IClientMessageInspector _inspector;

        public MessageInspectorBehavior(Action<Message> OnSend, Action<Message> OnReceive)
        {
            this._inspector = new RawMessageInspector { OnSend = OnSend, OnReceive = OnReceive };
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(_inspector);
        }

        private class RawMessageInspector : IClientMessageInspector
        {
            public Action<Message> OnSend;

            public Action<Message> OnReceive;

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                if (OnReceive != null) OnReceive(reply);
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                if (OnSend != null) OnSend(request);
                return null;
            }
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // Nothing to do here.
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // Nothing to do here.
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // Nothing to do here.
        }
    }
}