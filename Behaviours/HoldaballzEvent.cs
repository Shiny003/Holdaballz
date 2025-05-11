using Photon.Realtime;

namespace Holdaballz.Behaviours
{
    public enum BallEventType
    {
        Throw,
        Grab
    }
    
    public class HoldaballzEvent
    {
        public Player Sender;
        public BallEventType Type;
        public object Data;
    }
}