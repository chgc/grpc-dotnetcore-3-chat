using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Google.Protobuf.WellKnownTypes;
using Greet;
using Grpc.Core;

namespace chatwithgrpc
{
    public class ChatService : Chat.ChatBase
    {
        private readonly Server.ChatRoom _chatroomService;

        public ChatService(Server.ChatRoom chatRoomService)
        {
            _chatroomService = chatRoomService;
        }

        public override async Task join(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {            
            if(!await requestStream.MoveNext())
            {
                return;
            }

            do
            {                
                _chatroomService.join(requestStream.Current.User, responseStream);
                await _chatroomService.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());

            _chatroomService.Remove(context.Peer);

        }


    }
}
