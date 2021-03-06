﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by wsdl, Version=4.0.30319.1.
// 
namespace LensControlNs
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "LensControl", Namespace = "urn:schemas-pelco-com:service:LensControl:1")]
    public partial class LensControl : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback AutoFocusOperationCompleted;

        private System.Threading.SendOrPostCallback AutoIrisOperationCompleted;

        private System.Threading.SendOrPostCallback FocusOperationCompleted;

        private System.Threading.SendOrPostCallback GetMagOperationCompleted;

        private System.Threading.SendOrPostCallback GetMaxAOVOperationCompleted;

        private System.Threading.SendOrPostCallback GetMaxDigitalMagOperationCompleted;

        private System.Threading.SendOrPostCallback GetMaxMagOperationCompleted;

        private System.Threading.SendOrPostCallback GetMaxOpticalMagOperationCompleted;

        private System.Threading.SendOrPostCallback IrisOperationCompleted;

        private System.Threading.SendOrPostCallback SetMagOperationCompleted;

        private System.Threading.SendOrPostCallback StopOperationCompleted;

        private System.Threading.SendOrPostCallback ZoomOperationCompleted;

        /// <remarks/>
        public LensControl()
        {
            this.Url = "http://localhost:49152/control/LensControl-1";
        }

        /// <remarks/>
        public event AutoFocusCompletedEventHandler AutoFocusCompleted;

        /// <remarks/>
        public event AutoIrisCompletedEventHandler AutoIrisCompleted;

        /// <remarks/>
        public event FocusCompletedEventHandler FocusCompleted;

        /// <remarks/>
        public event GetMagCompletedEventHandler GetMagCompleted;

        /// <remarks/>
        public event GetMaxAOVCompletedEventHandler GetMaxAOVCompleted;

        /// <remarks/>
        public event GetMaxDigitalMagCompletedEventHandler GetMaxDigitalMagCompleted;

        /// <remarks/>
        public event GetMaxMagCompletedEventHandler GetMaxMagCompleted;

        /// <remarks/>
        public event GetMaxOpticalMagCompletedEventHandler GetMaxOpticalMagCompleted;

        /// <remarks/>
        public event IrisCompletedEventHandler IrisCompleted;

        /// <remarks/>
        public event SetMagCompletedEventHandler SetMagCompleted;

        /// <remarks/>
        public event StopCompletedEventHandler StopCompleted;

        /// <remarks/>
        public event ZoomCompletedEventHandler ZoomCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#AutoFocus", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void AutoFocus([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] int onOff)
        {
            this.Invoke("AutoFocus", new object[] {
                        onOff});
        }

        /// <remarks/>
        public System.IAsyncResult BeginAutoFocus(int onOff, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AutoFocus", new object[] {
                        onOff}, callback, asyncState);
        }

        /// <remarks/>
        public void EndAutoFocus(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void AutoFocusAsync(int onOff)
        {
            this.AutoFocusAsync(onOff, null);
        }

        /// <remarks/>
        public void AutoFocusAsync(int onOff, object userState)
        {
            if ((this.AutoFocusOperationCompleted == null))
            {
                this.AutoFocusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAutoFocusOperationCompleted);
            }
            this.InvokeAsync("AutoFocus", new object[] {
                        onOff}, this.AutoFocusOperationCompleted, userState);
        }

        private void OnAutoFocusOperationCompleted(object arg)
        {
            if ((this.AutoFocusCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AutoFocusCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#AutoIris", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void AutoIris([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] int onOff)
        {
            this.Invoke("AutoIris", new object[] {
                        onOff});
        }

        /// <remarks/>
        public System.IAsyncResult BeginAutoIris(int onOff, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AutoIris", new object[] {
                        onOff}, callback, asyncState);
        }

        /// <remarks/>
        public void EndAutoIris(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void AutoIrisAsync(int onOff)
        {
            this.AutoIrisAsync(onOff, null);
        }

        /// <remarks/>
        public void AutoIrisAsync(int onOff, object userState)
        {
            if ((this.AutoIrisOperationCompleted == null))
            {
                this.AutoIrisOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAutoIrisOperationCompleted);
            }
            this.InvokeAsync("AutoIris", new object[] {
                        onOff}, this.AutoIrisOperationCompleted, userState);
        }

        private void OnAutoIrisOperationCompleted(object arg)
        {
            if ((this.AutoIrisCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AutoIrisCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#Focus", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Focus([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] uint nearFar)
        {
            this.Invoke("Focus", new object[] {
                        nearFar});
        }

        /// <remarks/>
        public System.IAsyncResult BeginFocus(uint nearFar, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Focus", new object[] {
                        nearFar}, callback, asyncState);
        }

        /// <remarks/>
        public void EndFocus(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void FocusAsync(uint nearFar)
        {
            this.FocusAsync(nearFar, null);
        }

        /// <remarks/>
        public void FocusAsync(uint nearFar, object userState)
        {
            if ((this.FocusOperationCompleted == null))
            {
                this.FocusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnFocusOperationCompleted);
            }
            this.InvokeAsync("Focus", new object[] {
                        nearFar}, this.FocusOperationCompleted, userState);
        }

        private void OnFocusOperationCompleted(object arg)
        {
            if ((this.FocusCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.FocusCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#GetMag", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("magnification", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint GetMag()
        {
            object[] results = this.Invoke("GetMag", new object[0]);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetMag(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetMag", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public uint EndGetMag(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public void GetMagAsync()
        {
            this.GetMagAsync(null);
        }

        /// <remarks/>
        public void GetMagAsync(object userState)
        {
            if ((this.GetMagOperationCompleted == null))
            {
                this.GetMagOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMagOperationCompleted);
            }
            this.InvokeAsync("GetMag", new object[0], this.GetMagOperationCompleted, userState);
        }

        private void OnGetMagOperationCompleted(object arg)
        {
            if ((this.GetMagCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMagCompleted(this, new GetMagCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#GetMaxAOV", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("aovMax", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint GetMaxAOV()
        {
            object[] results = this.Invoke("GetMaxAOV", new object[0]);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetMaxAOV(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetMaxAOV", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public uint EndGetMaxAOV(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public void GetMaxAOVAsync()
        {
            this.GetMaxAOVAsync(null);
        }

        /// <remarks/>
        public void GetMaxAOVAsync(object userState)
        {
            if ((this.GetMaxAOVOperationCompleted == null))
            {
                this.GetMaxAOVOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMaxAOVOperationCompleted);
            }
            this.InvokeAsync("GetMaxAOV", new object[0], this.GetMaxAOVOperationCompleted, userState);
        }

        private void OnGetMaxAOVOperationCompleted(object arg)
        {
            if ((this.GetMaxAOVCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMaxAOVCompleted(this, new GetMaxAOVCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#GetMaxDigitalMag", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("magnification", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint GetMaxDigitalMag()
        {
            object[] results = this.Invoke("GetMaxDigitalMag", new object[0]);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetMaxDigitalMag(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetMaxDigitalMag", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public uint EndGetMaxDigitalMag(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public void GetMaxDigitalMagAsync()
        {
            this.GetMaxDigitalMagAsync(null);
        }

        /// <remarks/>
        public void GetMaxDigitalMagAsync(object userState)
        {
            if ((this.GetMaxDigitalMagOperationCompleted == null))
            {
                this.GetMaxDigitalMagOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMaxDigitalMagOperationCompleted);
            }
            this.InvokeAsync("GetMaxDigitalMag", new object[0], this.GetMaxDigitalMagOperationCompleted, userState);
        }

        private void OnGetMaxDigitalMagOperationCompleted(object arg)
        {
            if ((this.GetMaxDigitalMagCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMaxDigitalMagCompleted(this, new GetMaxDigitalMagCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#GetMaxMag", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("magnification", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint GetMaxMag()
        {
            object[] results = this.Invoke("GetMaxMag", new object[0]);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetMaxMag(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetMaxMag", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public uint EndGetMaxMag(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public void GetMaxMagAsync()
        {
            this.GetMaxMagAsync(null);
        }

        /// <remarks/>
        public void GetMaxMagAsync(object userState)
        {
            if ((this.GetMaxMagOperationCompleted == null))
            {
                this.GetMaxMagOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMaxMagOperationCompleted);
            }
            this.InvokeAsync("GetMaxMag", new object[0], this.GetMaxMagOperationCompleted, userState);
        }

        private void OnGetMaxMagOperationCompleted(object arg)
        {
            if ((this.GetMaxMagCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMaxMagCompleted(this, new GetMaxMagCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#GetMaxOpticalMag", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("magnification", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint GetMaxOpticalMag()
        {
            object[] results = this.Invoke("GetMaxOpticalMag", new object[0]);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetMaxOpticalMag(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetMaxOpticalMag", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public uint EndGetMaxOpticalMag(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((uint)(results[0]));
        }

        /// <remarks/>
        public void GetMaxOpticalMagAsync()
        {
            this.GetMaxOpticalMagAsync(null);
        }

        /// <remarks/>
        public void GetMaxOpticalMagAsync(object userState)
        {
            if ((this.GetMaxOpticalMagOperationCompleted == null))
            {
                this.GetMaxOpticalMagOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetMaxOpticalMagOperationCompleted);
            }
            this.InvokeAsync("GetMaxOpticalMag", new object[0], this.GetMaxOpticalMagOperationCompleted, userState);
        }

        private void OnGetMaxOpticalMagOperationCompleted(object arg)
        {
            if ((this.GetMaxOpticalMagCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetMaxOpticalMagCompleted(this, new GetMaxOpticalMagCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#Iris", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Iris([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] uint openClose)
        {
            this.Invoke("Iris", new object[] {
                        openClose});
        }

        /// <remarks/>
        public System.IAsyncResult BeginIris(uint openClose, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Iris", new object[] {
                        openClose}, callback, asyncState);
        }

        /// <remarks/>
        public void EndIris(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void IrisAsync(uint openClose)
        {
            this.IrisAsync(openClose, null);
        }

        /// <remarks/>
        public void IrisAsync(uint openClose, object userState)
        {
            if ((this.IrisOperationCompleted == null))
            {
                this.IrisOperationCompleted = new System.Threading.SendOrPostCallback(this.OnIrisOperationCompleted);
            }
            this.InvokeAsync("Iris", new object[] {
                        openClose}, this.IrisOperationCompleted, userState);
        }

        private void OnIrisOperationCompleted(object arg)
        {
            if ((this.IrisCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.IrisCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#SetMag", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SetMag([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] uint magnification)
        {
            this.Invoke("SetMag", new object[] {
                        magnification});
        }

        /// <remarks/>
        public System.IAsyncResult BeginSetMag(uint magnification, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SetMag", new object[] {
                        magnification}, callback, asyncState);
        }

        /// <remarks/>
        public void EndSetMag(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void SetMagAsync(uint magnification)
        {
            this.SetMagAsync(magnification, null);
        }

        /// <remarks/>
        public void SetMagAsync(uint magnification, object userState)
        {
            if ((this.SetMagOperationCompleted == null))
            {
                this.SetMagOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetMagOperationCompleted);
            }
            this.InvokeAsync("SetMag", new object[] {
                        magnification}, this.SetMagOperationCompleted, userState);
        }

        private void OnSetMagOperationCompleted(object arg)
        {
            if ((this.SetMagCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetMagCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#Stop", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Stop()
        {
            this.Invoke("Stop", new object[0]);
        }

        /// <remarks/>
        public System.IAsyncResult BeginStop(System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Stop", new object[0], callback, asyncState);
        }

        /// <remarks/>
        public void EndStop(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void StopAsync()
        {
            this.StopAsync(null);
        }

        /// <remarks/>
        public void StopAsync(object userState)
        {
            if ((this.StopOperationCompleted == null))
            {
                this.StopOperationCompleted = new System.Threading.SendOrPostCallback(this.OnStopOperationCompleted);
            }
            this.InvokeAsync("Stop", new object[0], this.StopOperationCompleted, userState);
        }

        private void OnStopOperationCompleted(object arg)
        {
            if ((this.StopCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.StopCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:schemas-pelco-com:service:LensControl:1#Zoom", RequestNamespace = "urn:schemas-pelco-com:service:LensControl:1", ResponseNamespace = "urn:schemas-pelco-com:service:LensControl:1", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Zoom([System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)] uint inOut)
        {
            this.Invoke("Zoom", new object[] {
                        inOut});
        }

        /// <remarks/>
        public System.IAsyncResult BeginZoom(uint inOut, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Zoom", new object[] {
                        inOut}, callback, asyncState);
        }

        /// <remarks/>
        public void EndZoom(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void ZoomAsync(uint inOut)
        {
            this.ZoomAsync(inOut, null);
        }

        /// <remarks/>
        public void ZoomAsync(uint inOut, object userState)
        {
            if ((this.ZoomOperationCompleted == null))
            {
                this.ZoomOperationCompleted = new System.Threading.SendOrPostCallback(this.OnZoomOperationCompleted);
            }
            this.InvokeAsync("Zoom", new object[] {
                        inOut}, this.ZoomOperationCompleted, userState);
        }

        private void OnZoomOperationCompleted(object arg)
        {
            if ((this.ZoomCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ZoomCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void AutoFocusCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void AutoIrisCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void FocusCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void GetMagCompletedEventHandler(object sender, GetMagCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetMagCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetMagCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public uint Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((uint)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void GetMaxAOVCompletedEventHandler(object sender, GetMaxAOVCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetMaxAOVCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetMaxAOVCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public uint Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((uint)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void GetMaxDigitalMagCompletedEventHandler(object sender, GetMaxDigitalMagCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetMaxDigitalMagCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetMaxDigitalMagCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public uint Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((uint)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void GetMaxMagCompletedEventHandler(object sender, GetMaxMagCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetMaxMagCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetMaxMagCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public uint Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((uint)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void GetMaxOpticalMagCompletedEventHandler(object sender, GetMaxOpticalMagCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetMaxOpticalMagCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetMaxOpticalMagCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public uint Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((uint)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void IrisCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void SetMagCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void StopCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void ZoomCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}
