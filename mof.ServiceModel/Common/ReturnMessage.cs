using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace mof.ServiceModels.Common
{
    public enum eMessageType
    {
        Exception,
        Error,
        Informaion
    }
    public class MessageData
    {
        public string Code { get; set; }
        public string Language { get; set; }
        public string MessageHint { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
        public Exception EX { get; set; }
     
    }
    
    public class ReturnMessage
    {
    
        private List<MessageData> _message = new List<MessageData>();
        public List<MessageData> Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value == null)
                    _message = new List<MessageData>();
                else
                    _message = value;
            }
        }
        public bool IsCompleted { get; set; }
        public bool IsWarned { get; set; }
        public Exception AppException { get; set; }
        private IStringLocalizer<MessageLocalizer> _msglocalizer;

        public ReturnMessage(IStringLocalizer<MessageLocalizer> msglocalizer)
        {
            _msglocalizer = msglocalizer;
        }
        public void AddError(Exception ex)
        {
            _message.Add(new MessageData { Code = "SYSERR", Message = ex.Message, MessageHint = "Exception", MessageType = eMessageType.Exception.ToString(), Language = "en",EX = ex.InnerException});
            AppException = ex;
            //try
            //{
            //    // Create the source, if it does not already exist.
            //    if (!EventLog.SourceExists("PDMO-API"))
            //    {
            //        //An event log source should not be created and immediately used.
            //        //There is a latency time to enable the source, it should be created
            //        //prior to executing the application that uses the source.
            //        //Execute this sample a second time to use the new source.
            //        EventLog.CreateEventSource("MySource", "MyNewLog");
            //        // The source is created.  Exit the application to allow it to be registered.

            //    }

            //    // Create an EventLog instance and assign its source.
            //    EventLog myLog = new EventLog();
            //    myLog.Source = "PDMO-API";

            //    // Write an informational entry to the event log.    
            //    myLog.WriteEntry(JsonConvert.SerializeObject(ex), EventLogEntryType.Error);
            //    this.Message.Insert(0, new MessageData { Code = "SYSTEMLOG", Message = "Success", MessageType = "Information" });
            //}
            //catch (Exception e)
            //{
            //    this.Message.Insert(0, new MessageData { Code = "SYSTEMLOG", Message = "Cannot write system log", EX = e, MessageType = "Error" });
            //}
        }
   
        public void AddMessage(string code, string hint, eMessageType messageType, string[] options = null)
        {
            var msg = new MessageData { Code = code, MessageHint = hint, MessageType = messageType.ToString() };
            var lz = _msglocalizer[code];
            if (lz.ResourceNotFound)
            {
                msg.Message = hint;
            }
            msg.Message = _msglocalizer[code];
            if (options != null)
            {
                msg.Message = string.Format(msg.Message, options);
            }
            _message.Add(msg);
        }
        public void CloneMessage(List<MessageData> msgs)
        {
            foreach (var m in msgs)
            {
                var msg = new MessageData { Code = m.Code, Message = m.Message, MessageType = m.MessageType, MessageHint = m.MessageHint, Language = m.Language };
                this.Message.Add(msg);
            }
        }
    }

    public class ReturnObject<T> : ReturnMessage
    {
        public ReturnObject(IStringLocalizer<MessageLocalizer> msglocalizer) : base(msglocalizer)
        {

        }
        public T Data { get; set; }
    }

    public class ReturnQueryData<T> : ReturnObject<List<T>>
    {
        public ReturnQueryData(IStringLocalizer<MessageLocalizer> msglocalizer) : base(msglocalizer)
        {

        }
        public Int32 TotalRow { get; set; }
    }
    public class ReturnList<T> : ReturnObject<List<T>>
    {
        public ReturnList(IStringLocalizer<MessageLocalizer> msglocalizer) : base(msglocalizer)
        {
            this.Data = new List<T>();
        }
        public Int32 TotalRow { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
    }

    public class MessageLocalizer {
       
        public MessageLocalizer()
        {
           
        }

    }
 
}
