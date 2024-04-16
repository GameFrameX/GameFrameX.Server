namespace GameFrameX.Launcher.Transfer;

[Transfer(ServerType.Router, ServerType.Gateway)]
public class TransferRouterToGatewayHandler : ITransferHandler
{
    /*public async void Transfer(ITcpClientBase sender, IMessageEncoderHandler messageEncoderHandler, IMessage message)
    {
        var messageBytes = messageEncoderHandler.Handler(message);
        await sender.SendAsync(messageBytes);
    }*/
}